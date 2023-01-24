using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.Samples;
using PlayFabCloudScript;
using PlayFab.ServerModels;
using Utilities;

namespace PlayFabCloudScript.OnLogin
{
    /*
    REQUIRES THE ARGUMENTS
    PlayFabIDs - list of playfabIDs to supply to the GetPlayerProfile API call
    ProfileConstraints - an object of type PlayerProfileViewContraints (PlayFab SDK) that will be applied to all profile gets
    */

    public static class GetModifiedProfileData
    {
        static string secretKey = "PlayFabSecretKey";
        static string titleID = "titleId";
        static string Id = "";
        public static ILogger logger = null;

        private static List<BasicProfile> returnProfiles = new List<BasicProfile>();
        public static List<string> errorStrings = new List<string>();
        [FunctionName("GetProfile")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,ILogger log)
        {
            if(returnProfiles.Count > 0)
            {
                returnProfiles.Clear();
            }

            PlayFabSettings.staticSettings.TitleId = Environment.GetEnvironmentVariable(titleID);
            PlayFabSettings.staticSettings.DeveloperSecretKey = Environment.GetEnvironmentVariable(secretKey);
            
            logger = log;
            FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());
            dynamic args = context.FunctionArgument;
            
           
            Id = context.CallerEntityProfile.Lineage.MasterPlayerAccountId;
            
            dynamic idList = null;
            if (args != null && args["PlayFabIDs"] != null)
            {
                idList = args["PlayFabIDs"];
            }

            dynamic profileConstraint = null;
            if (args != null && args["ProfileConstraints"] != null)
            {
                profileConstraint = args["ProfileConstraints"];
            }

            List<string> pfIds = new List<string>();
            PlayerProfileViewConstraints constraints = new PlayerProfileViewConstraints();
            try
            {
                pfIds = JsonConvert.DeserializeObject<List<string>>(idList.ToString());
                constraints = JsonConvert.DeserializeObject<PlayerProfileViewConstraints>(profileConstraint.ToString());
            }
            catch(Exception e)
            {
                errorStrings.Add(e.ToString());
                errorStrings.Add(PlayFabHelper.CaptureException(e, log));
                errorStrings.Add(PlayFabHelper.CaptureException(new Exception(idList.ToString()), log));
                errorStrings.Add(profileConstraint.ToString());
                errorStrings.Add(idList.ToString());
                string requestContent = await new StreamReader(req.Body).ReadToEndAsync();
                return new OkObjectResult(JsonConvert.SerializeObject(errorStrings) + " " + requestContent);
            }
            

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation($"RequestBody: {requestBody}");
            

            
            List<Task<PlayFabResult<GetPlayerProfileResult>>> profileAPIResults = new List<Task<PlayFabResult<GetPlayerProfileResult>>>();

            foreach(string id in pfIds)
            {
                logger.LogInformation("Calling GameServer/GetPlayerProfile for user: " + id);
                var httpTask = PlayFabHelper.GetPlayerProfile(id, constraints, logger);
                profileAPIResults.Add(httpTask);
            }

            logger.LogInformation("Awaiting PF API calls");
            //
            //
            foreach(var profileCall in profileAPIResults)
            {
                if(profileCall.IsCompleted)
                {
                    OnComplete(profileCall.Result);
                }
                else
                {
                    await profileCall;
                    OnComplete(profileCall.Result);
                }
            }
            
            

            return new OkObjectResult(JsonConvert.SerializeObject(returnProfiles));
        }

        public static void OnComplete(PlayFabResult<GetPlayerProfileResult> profileResult)
        {
            logger.LogInformation("Checking to see if the calls were successful");
            if(PlayFabHelper.WasPlayFabCallSuccessful<GetPlayerProfileResult>(profileResult, logger))
            {
                logger.LogInformation("The calls were successful");
                PlayerProfileModel actualProfile = profileResult.Result.PlayerProfile;
                returnProfiles.Add( new BasicProfile(actualProfile.AvatarUrl, actualProfile.PlayerId, actualProfile.DisplayName, actualProfile.Statistics));
            }
            else
            {
                logger.LogInformation("UpdateStatistic Failed");
            }
        }
    }
}
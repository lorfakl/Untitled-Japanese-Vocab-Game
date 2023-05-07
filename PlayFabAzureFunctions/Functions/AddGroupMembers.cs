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
using PlayFab.GroupsModels;
using Utilities;

namespace PlayFabCloudScript.OnLogin
{
    /*
    REQUIRES THE ARGUMENTS
    PlayFabIDs - list of playfabIDs to supply to the GetPlayerProfile API call
    ProfileConstraints - an object of type PlayerProfileViewContraints (PlayFab SDK) that will be applied to all profile gets
    */

    public static class AddGroupMembers
    {
        static string secretKey = "PlayFabSecretKey";
        static string titleID = "titleId";
        static string Id = "";
        public static ILogger logger;

        public static List<string> errorStrings = new List<string>();
        [FunctionName("AddMembers")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,ILogger log)
        {
            logger = log;
            logger.LogInformation("Starting");
            PlayFabSettings.staticSettings.TitleId = Environment.GetEnvironmentVariable(titleID);
            PlayFabSettings.staticSettings.DeveloperSecretKey = Environment.GetEnvironmentVariable(secretKey);
            
            logger.LogInformation("Parsing Context");
            FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());
            dynamic args = context.FunctionArgument;
            logger.LogInformation("Context parse complete");
            
            Id = context.CallerEntityProfile.Lineage.MasterPlayerAccountId;
            logger.LogInformation("Grabbed ID");
            
            var entityTokenTask = PlayFabAuthenticationAPI.GetEntityTokenAsync(new PlayFab.AuthenticationModels.GetEntityTokenRequest());
            await entityTokenTask;

            logger.LogInformation("auth complete");
            
            dynamic groupID = null;
            if (args != null && args["GroupID"] != null)
            {
                groupID = args["GroupID"];
            }

            logger.LogInformation("Grabbed group id");

            dynamic memberKeys = null;
            if (args != null && args["MemberKeys"] != null)
            {
                memberKeys = args["MemberKeys"];
            }

            logger.LogInformation("memberkeys ID");

            List<string> memberIds = new List<string>();
            string groupIDArgument = groupID.ToString();



            try
            {
                memberIds = JsonConvert.DeserializeObject<List<string>>(memberKeys.ToString());

            }
            catch(Exception e)
            {
                errorStrings.Add(e.ToString());
                errorStrings.Add(PlayFabHelper.CaptureException(e, log));
                errorStrings.Add(PlayFabHelper.CaptureException(new Exception(memberKeys.ToString()), log));
                errorStrings.Add(memberKeys.ToString());
                string requestContent = await new StreamReader(req.Body).ReadToEndAsync();
                return new OkObjectResult(JsonConvert.SerializeObject(errorStrings) + " " + requestContent);
            }
            

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation($"RequestBody: {requestBody}");
            
            List<PlayFab.GroupsModels.EntityKey> memberKeysToAdd = new List<PlayFab.GroupsModels.EntityKey>();
            foreach(string id in memberIds)
            {
                memberKeysToAdd.Add( new PlayFab.GroupsModels.EntityKey {
                    Type = EntityTypes.title_player_account.ToString(),
                    Id = id
                });
            }

            PlayFab.GroupsModels.EntityKey groupKey = new PlayFab.GroupsModels.EntityKey
            {
                Type = EntityTypes.group.ToString(),
                Id = groupIDArgument
            };
            var addMembersTask = PlayFabHelper.AddMembers(groupKey, memberKeysToAdd, log);

            logger.LogInformation("Awaiting PF API calls");
            //
            //
            await addMembersTask;

            string apiResult = "";
            if(WasSuccessful(addMembersTask.Result))
            {
                apiResult = "Add members was successful";
            }
            else
            {
                apiResult = "Add members was not successful";
            }
            

            return new OkObjectResult(apiResult);
        }

        public static bool WasSuccessful(PlayFabResult<PlayFab.GroupsModels.EmptyResponse> addGroupResult)
        {
            logger.LogInformation("Checking to see if the calls were successful");
            if(PlayFabHelper.WasPlayFabCallSuccessful<PlayFab.GroupsModels.EmptyResponse>(addGroupResult, logger))
            {
                logger.LogInformation("The calls were successful");
                return true;
            }
            else
            {
                logger.LogInformation("UpdateStatistic Failed");
                return false;
            }
        }
    }
}
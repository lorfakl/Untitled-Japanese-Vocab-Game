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
    numOfWordsToAdd - the number of new words to add to the players level Zero list
    wordsSeen - a count of the total number of new word the player has seen
    */

    public static class UpdateTag
    {
        static string responseString = "";
        static string secretKey = "PlayFabSecretKey";
        static string titleID = "titleId";
        static string Id = "";
        public static ILogger logger = null;

        [FunctionName("UpdateTag")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,ILogger log)
        {

            PlayFabSettings.staticSettings.TitleId = Environment.GetEnvironmentVariable(titleID);
            PlayFabSettings.staticSettings.DeveloperSecretKey = Environment.GetEnvironmentVariable(secretKey);
            logger = log;
            FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());
            dynamic args = context.FunctionArgument;
            
           
            Id = context.CallerEntityProfile.Lineage.MasterPlayerAccountId;
            
            dynamic tag = null;
            if (args != null && args["TagName"] != null)
            {
                tag = args["TagName"];
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation($"RequestBody: {requestBody}");
            

            logger.LogInformation("Calling GameServer/SetTag for user: " + Id);
            var addPlayerTagTask = PlayFabHelper.AddPlayerTag(Id, tag.ToString(), logger);
            await addPlayerTagTask;

            logger.LogInformation("Awaiting PF API calls");
            //
            //

            logger.LogInformation("Calling OnCompleteFor PF calls");
            OnComplete(addPlayerTagTask.Result);

            return new OkObjectResult(responseString);
        }

        public static void OnComplete(PlayFabResult<AddPlayerTagResult> addPlayerTagResult)
        {
            logger.LogInformation("Checking to see if the calls were successful");
            if(PlayFabHelper.WasPlayFabCallSuccessful<AddPlayerTagResult>(addPlayerTagResult, logger))
            {
                logger.LogInformation("The calls were successful");
             
            }
            else
            {
                logger.LogInformation("AddPlayerTag Failed");
            }
        }
    }
}
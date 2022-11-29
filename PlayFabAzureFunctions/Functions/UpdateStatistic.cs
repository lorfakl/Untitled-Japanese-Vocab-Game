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
    Entries - a list of statistic values that need to be updated the CloudScriptStatArgument class is used to represent the information supplied 
    from the client. 
    */

    public static class UpdateStatistic
    {
        static string responseString = "";
        static string secretKey = "PlayFabSecretKey";
        static string titleID = "titleId";
        static string Id = "";
        public static ILogger logger = null;

        [FunctionName("Record")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,ILogger log)
        {

            PlayFabSettings.staticSettings.TitleId = Environment.GetEnvironmentVariable(titleID);
            PlayFabSettings.staticSettings.DeveloperSecretKey = Environment.GetEnvironmentVariable(secretKey);
            logger = log;
            FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());
            dynamic args = context.FunctionArgument;
            
           
            Id = context.CallerEntityProfile.Lineage.MasterPlayerAccountId;
            
            dynamic statisticUpdateEntries = null;
            if (args != null && args["Entries"] != null)
            {
                statisticUpdateEntries = args["Entries"];
            }

            List<CloudScriptStatArgument> statEntries = new List<CloudScriptStatArgument>();
            try
            {
                statEntries = JsonConvert.DeserializeObject<List<CloudScriptStatArgument>>(statisticUpdateEntries.ToString());
            }
            catch(Exception e)
            {
                PlayFabHelper.CaptureException(e, log);
                PlayFabHelper.CaptureException(new Exception(statisticUpdateEntries.ToString()), log);

            }
            

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation($"RequestBody: {requestBody}");
            

            logger.LogInformation("Calling GameServer/UpdateStatistic for user: " + Id);

            var updateStatTask = PlayFabHelper.UpdateUserStatistic(Id, statEntries, logger);
            await updateStatTask;

            logger.LogInformation("Awaiting PF API calls");
            //
            //

            logger.LogInformation("Calling OnCompleteFor PF calls");
            OnComplete(updateStatTask.Result);

            return new OkObjectResult(responseString);
        }

        public static void OnComplete(PlayFabResult<UpdatePlayerStatisticsResult> addPlayerTagResult)
        {
            logger.LogInformation("Checking to see if the calls were successful");
            if(PlayFabHelper.WasPlayFabCallSuccessful<UpdatePlayerStatisticsResult>(addPlayerTagResult, logger))
            {
                logger.LogInformation("The calls were successful");
             
            }
            else
            {
                logger.LogInformation("UpdateStatistic Failed");
            }
        }
    }
}
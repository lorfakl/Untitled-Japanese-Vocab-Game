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
using PlayFab.DataModels;
using Utilities;

namespace PlayFabCloudScript.Rivals
{
    public static class GetOtherPlayerStatistics
    {
        static string secretKey = "PlayFabSecretKey";
        static string titleID = "titleId";
        public static ILogger logger = null;

        static List<string> listOfLogs = null;
        

        [FunctionName("GetOtherStatistics")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,ILogger log)
        {

            PlayFabSettings.staticSettings.TitleId = Environment.GetEnvironmentVariable(titleID);
            PlayFabSettings.staticSettings.DeveloperSecretKey = Environment.GetEnvironmentVariable(secretKey);
            logger = log;
            listOfLogs = new List<string>();

            PlayFabHelper.LogInfo("Log List Initialized", log, listOfLogs);

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            
            FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(requestBody);
            
            PlayFabHelper.LogInfo("Context parsed", log, listOfLogs);
            PlayFabHelper.GetEntityToken();
            
            if(context != null)
            {
                PlayFabHelper.LogInfo("Not Nulol just testing", log, listOfLogs);
            }
            else
            {
                PlayFabHelper.LogInfo("Exception Thrown!", log, listOfLogs);
                PlayFabHelper.LogInfo("Request Body: " + requestBody, log, listOfLogs);
                return new OkObjectResult(listOfLogs);
            }
            
            dynamic args = context.FunctionArgument;
            GetOtherPlayerStatisticArgument argInfo = null;

            try
            {
                argInfo = JsonConvert.DeserializeObject<GetOtherPlayerStatisticArgument>(args.ToString());
            }
            catch(Exception e)
            {
                PlayFabHelper.LogInfo("Exception Thrown!", log, listOfLogs);
                PlayFabHelper.LogInfo(PlayFabHelper.CaptureException(e, log), log, listOfLogs);
                PlayFabHelper.LogInfo("Request Body: " + requestBody, log, listOfLogs);
                return new OkObjectResult(listOfLogs);
            }

            PlayFabHelper.LogInfo($"Created requested ID count of {argInfo.playFabIDs.Count}", log, listOfLogs);

            //Id = context.CallerEntityProfile.Lineage.MasterPlayerAccountId;
            
            List<Task<PlayFabResult<GetPlayerStatisticsResult>>> getStatistics = new List<Task<PlayFabResult<GetPlayerStatisticsResult>>>();
            foreach(var id in argInfo.playFabIDs)
            {
                getStatistics.Add(PlayFabHelper.GetPlayerStatisitic(id, argInfo.StatisticName, logger));
            }

            PlayFabHelper.LogInfo($"API calls made", log, listOfLogs);
            List<GetOtherPlayerStatisticsResult> returnData = new List<GetOtherPlayerStatisticsResult>();
            foreach(var task in getStatistics)
            {
                await task;
                PlayFabHelper.LogInfo($"Task Awaited", log, listOfLogs);
                var entry = OnComplete(task.Result);
                if(entry != default)
                {
                    returnData.Add(entry);
                }
            }

            return new OkObjectResult(returnData);
        }

        static GetOtherPlayerStatisticsResult OnComplete(PlayFabResult<GetPlayerStatisticsResult> getStatResult)
        {
            logger.LogInformation("Checking to see if the calls were successful");
            if(PlayFabHelper.WasPlayFabCallSuccessful<GetPlayerStatisticsResult>(getStatResult, logger))
            {
                logger.LogInformation("The calls were successful");
                
                return new GetOtherPlayerStatisticsResult{
                    ID = getStatResult.Result.PlayFabId,
                    Value = getStatResult.Result.Statistics[0].Value
                };
            }
            else
            {
                return default;
            }
        }
    
    }
}
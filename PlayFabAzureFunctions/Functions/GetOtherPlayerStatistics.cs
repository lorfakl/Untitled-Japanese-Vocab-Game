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
using System.Linq;
using PlayFab.ServerModels;
using PlayFab.DataModels;
using Utilities;

namespace PlayFabCloudScript.Rivals
{
    public static class GetOtherPlayerStatistics
    {
        static string secretKey = "PlayFabSecretKey";
        static string titleID = "titleId";

        static string requestedStat = "";
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
            logger.LogInformation($"Request Body: {requestBody}");

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
            
            logger.LogInformation($"i hate this so much");
            dynamic args = context.FunctionArgument;
            logger.LogInformation($"y u no work");
            GetOtherPlayerStatisticArgument argInfo = null;
            logger.LogInformation($"stop failing");

            try
            {
                logger.LogInformation($"wgatfdasg");
                argInfo = JsonConvert.DeserializeObject<GetOtherPlayerStatisticArgument>(args.ToString());
                logger.LogInformation($"fsadffsdds");
            }
            catch(Exception e)
            {
                PlayFabHelper.LogInfo("Exception Thrown!", log, listOfLogs);
                PlayFabHelper.LogInfo(PlayFabHelper.CaptureException(e, log), log, listOfLogs);
                PlayFabHelper.LogInfo("Request Body: " + requestBody, log, listOfLogs);
                return new OkObjectResult(listOfLogs);
            }
            logger.LogInformation($"strhtrhtr");

            try
            {
                logger.LogInformation($"y r u crying");
                logger.LogInformation($"this is really null");
                logger.LogInformation($"{argInfo == null}");
                logger.LogInformation($"{argInfo.playFabIDs == null}");
                logger.LogInformation($"i honestly hate this");
                PlayFabHelper.LogInfo($"Created requested ID count of {argInfo.playFabIDs.Count}", log, listOfLogs);
            }
            catch(Exception e)
            {
                logger.LogInformation($"huh?");
                PlayFabHelper.CaptureException(e, logger);
            }
            

            //Id = context.CallerEntityProfile.Lineage.MasterPlayerAccountId;
            requestedStat = argInfo.StatisticName.ToString();
            GetPlayerCombinedInfoRequestParams param = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerStatistics = true,
                GetPlayerProfile = true,
                PlayerStatisticNames = new List<string> { requestedStat },
                ProfileConstraints = new PlayerProfileViewConstraints
                {
                    ShowDisplayName = true,
                    ShowLastLogin = true,
                    //ShowStatistics = true
                }
            };

            List<Task<PlayFabResult<GetPlayerCombinedInfoResult>>> getStatistics = new List<Task<PlayFabResult<GetPlayerCombinedInfoResult>>>();
            foreach(var id in argInfo.playFabIDs)
            {
                getStatistics.Add(PlayFabHelper.GetPlayerInfo(param, id, logger));
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
            
            returnData = returnData.OrderByDescending(entry => entry.Value).ToList();

            return new OkObjectResult(returnData);
        }

        static GetOtherPlayerStatisticsResult OnComplete(PlayFabResult<GetPlayerCombinedInfoResult> getStatResult)
        {
            logger.LogInformation("Checking to see if the calls were successful");
            if(PlayFabHelper.WasPlayFabCallSuccessful<GetPlayerCombinedInfoResult>(getStatResult, logger))
            {
                logger.LogInformation("The calls were successful");
                logger.LogInformation($"Result JSON: {JsonConvert.SerializeObject(getStatResult.Result)}");
                int statVal = 0;
                try
                {
                    foreach(var stat in getStatResult.Result.InfoResultPayload.PlayerStatistics)
                    {
                        logger.LogInformation($"Returned Statistic: {JsonConvert.SerializeObject(stat)}");
                        if(stat.StatisticName == requestedStat)
                        {
                            statVal = stat.Value;
                        }
                    }
                }
                catch(Exception e)
                {
                    PlayFabHelper.CaptureException(e, logger);
                }
                
                
                GetOtherPlayerStatisticsResult resultToReturn = default;

                try
                {
                   resultToReturn = new GetOtherPlayerStatisticsResult{
                    ID = getStatResult.Result.PlayFabId,
                    Value = statVal,
                    DisplayName = getStatResult.Result.InfoResultPayload.PlayerProfile.DisplayName};
                }
                catch(Exception e)
                {
                    PlayFabHelper.CaptureException(e, logger);
                }

                return resultToReturn;
            }
            else
            {
                return default;
            }
        }
    
    }
}
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
using System.Reflection;
using System.ComponentModel;
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

    public static class ModifyTag
    {
        static string responseString = "";
        static string secretKey = "PlayFabSecretKey";
        static string titleID = "titleId";
        static string Id = "";
        public static ILogger logger = null;

        [FunctionName("ModifyTag")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,ILogger log)
        {
            List<string> listOfLogs = new List<string>();
            log.LogInformation("We have started");
            PlayFabSettings.staticSettings.TitleId = Environment.GetEnvironmentVariable(titleID);
            PlayFabSettings.staticSettings.DeveloperSecretKey = Environment.GetEnvironmentVariable(secretKey);
            logger = log;
            log.LogInformation("Got static setttings ");
            PlayerPlayStreamFunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<PlayerPlayStreamFunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());
            dynamic args = context.FunctionArgument;
            log.LogInformation("Transformed the dynamic object to something specific ");
           
            Id = context.PlayerProfile.PlayerId;
            log.LogInformation($"Assigned ID {Id}");
            PlayFabHelper.LogInfo($"Assigned ID {Id}", log, listOfLogs);
            
            ModifyTagParameter param = null;

            try
            {
                param = JsonConvert.DeserializeObject<ModifyTagParameter>(context.FunctionArgument.ToString());
            }
            catch(Exception e)
            {
                string functionarg = context.FunctionArgument.ToString();
                PlayFabHelper.LogInfo("There was an error here's the function argument no reason: " + functionarg, log, listOfLogs);
                PlayFabHelper.LogInfo(PlayFabHelper.CaptureException(e, log), log, listOfLogs);
            }
            
            if(param != null)
            {
                foreach(string t in param.TagNames)
                {
                    log.LogInformation($"Parsed tag number {param.TagNames.IndexOf(t)} : {t}");
                }

                if(param.Operation.Contains("add"))
                {
                    foreach(string i in param.TagNames)
                    {
                        var addPlayerTagTask = PlayFabHelper.AddPlayerTag(Id, i, logger);
                        await addPlayerTagTask;
                        PlayFabHelper.LogInfo(OnComplete(addPlayerTagTask.Result), log, listOfLogs);
                    }
                }
                else
                {
                    foreach(string i in param.TagNames)
                    {
                        var removePlayerTagTask = PlayFabHelper.RemovePlayerTag(Id, i, logger);
                        await removePlayerTagTask;
                        PlayFabHelper.LogInfo(OnComplete(removePlayerTagTask.Result), log, listOfLogs);
                    }
                }

                return new OkObjectResult(listOfLogs);

            }

            /*log.LogInformation($"Parsed first tag {param.TagNames[0]}");

            dynamic operation = null;
            if (args != null && args["Operation"] != null)
            {
                operation = args["Operation"];
            }

            log.LogInformation($"Parsed Second parameter {param.TagNames[1]}");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation($"RequestBody: {requestBody}");
            PlayFabHelper.LogInfo($"RequestBody: {requestBody}", log, listOfLogs);

            if(operation.ToString().Contains("add"))
            {
                logger.LogInformation("Calling GameServer/SetTag for user: " + Id);
                PlayFabHelper.LogInfo($"Calling GameServer/SetTag for user: {Id}", log, listOfLogs);
                var addPlayerTagTask = PlayFabHelper.AddPlayerTag(Id, tag.ToString(), logger);
                await addPlayerTagTask;

                logger.LogInformation("Awaiting PF API calls");
                //
                //

                logger.LogInformation("Calling OnCompleteFor PF calls");
                OnComplete(addPlayerTagTask.Result);
            }
            else
            {
                logger.LogInformation("Calling GameServer/RemoveTag for user: " + Id);
                PlayFabHelper.LogInfo($"Calling GameServer/RemoveTag for user: {Id}", log, listOfLogs);
                var removePlayerTagTask = PlayFabHelper.RemovePlayerTag(Id, tag.ToString(), logger);
                await removePlayerTagTask;

                logger.LogInformation("Awaiting PF API calls");
                //
                //

                logger.LogInformation("Calling OnCompleteFor PF calls");
                OnComplete(removePlayerTagTask.Result);
            }*/
            

            return new OkObjectResult(listOfLogs);
        }

        public static string OnComplete(PlayFabResult<AddPlayerTagResult> addPlayerTagResult)
        {
            logger.LogInformation("Checking to see if the calls were successful");
            //PlayFabHelper.LogInfo("Checking to see if the calls were successful", logger, listOfLogs);
            if(PlayFabHelper.WasPlayFabCallSuccessful<AddPlayerTagResult>(addPlayerTagResult, logger))
            {
                return "The calls were successful";
             
            }
            else
            {
                return "AddPlayerTag Failed";
            }
        }

        public static string OnComplete(PlayFabResult<RemovePlayerTagResult> removePlayerTagResult)
        {
            logger.LogInformation("Checking to see if the calls were successful");
            if(PlayFabHelper.WasPlayFabCallSuccessful<RemovePlayerTagResult>(removePlayerTagResult, logger))
            {
                return "The calls were successful";
             
            }
            else
            {
                return "RemovePlayerTag Failed";
            }
        }
    }
}
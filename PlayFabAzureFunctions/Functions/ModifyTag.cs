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
        static string secretKey = "PlayFabSecretKey";
        static string titleID = "titleId";
        static string Id = "";
        public static ILogger logger = null;
        public static List<string> listOfLogs = new List<string>();
        [FunctionName("ModifyTag")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,ILogger log)
        {
            listOfLogs.Clear();
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
                log.LogInformation($"The rquest as a string: {context.FunctionArgument.ToString()}");
            }
            catch(Exception e)
            {
                string functionarg = context.FunctionArgument.ToString();
                PlayFabHelper.LogInfo("There was an error here's the function argument no reason: " + functionarg, log, listOfLogs);
                PlayFabHelper.LogInfo(PlayFabHelper.CaptureException(e, log), log, listOfLogs);
            }
            
            if(param != null)
            {
                log.LogInformation($"Param is not null");

                try
                {
                    foreach(string t in param.TagNames)
                    {
                        log.LogInformation($"Parsed tag number {param.TagNames.IndexOf(t)} : {t}");
                    }
                }
                catch(Exception e)
                {
                    log.LogInformation("Exception Thrown");
                    listOfLogs.Add(PlayFabHelper.CaptureException(e, logger));
                }

                
                    logger.LogInformation("Checking the operation type");
                    if(param.Operation.Contains("add"))
                    {
                        logger.LogInformation("Checking the operation type");
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
                            PlayFabHelper.LogInfo(OnComplete(removePlayerTagTask.Result), log, listOfLogs);
                        }
                    }
                
                

                return new OkObjectResult(listOfLogs);
            }
            else
            {
                log.LogInformation($"Param object is infact null");
            }
            

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

        public static void OnComplete(PlayFabResult<PlayFab.GroupsModels.ListMembershipResponse> listMembershipResponse)
        {
            PlayFabHelper.LogInfo("Checking List Membership API Status", logger, listOfLogs);
            if(PlayFabHelper.WasPlayFabCallSuccessful<PlayFab.GroupsModels.ListMembershipResponse>(listMembershipResponse, logger, listOfLogs))
            {
                logger.LogInformation("List Membership call success");
                logger.LogInformation($"List Membership Response: {JsonConvert.SerializeObject(listMembershipResponse.Result)}");
                foreach(var g in listMembershipResponse.Result.Groups)
                {
                    PlayFabHelper.DeleteGroup(g.Group);
                }
                logger.LogInformation($"Looped through all {listMembershipResponse.Result.Groups.Count} Groups for User {Id}");
            }
        }
    }
}
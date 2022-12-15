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

    public static class CompletePurchase
    {
        static string responseString = "";
        static string secretKey = "PlayFabSecretKey";
        static string titleID = "titleId";
        static string Id = "";
        public static ILogger logger = null;

        [FunctionName("CompletePurchase")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,ILogger log)
        {
            List<string> listOfLogs = new List<string>();
            log.LogInformation("We have started");
            
            PlayFabSettings.staticSettings.TitleId = Environment.GetEnvironmentVariable(titleID);
            PlayFabSettings.staticSettings.DeveloperSecretKey = Environment.GetEnvironmentVariable(secretKey);
            
            logger = log;
            log.LogInformation("Got static setttings ");
            
            FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());
            dynamic args = context.FunctionArgument;
            log.LogInformation("Transformed the dynamic object to something specific ");
           
            Id = context.CallerEntityProfile.Lineage.MasterPlayerAccountId;

            log.LogInformation($"Assigned ID {Id}");
            PlayFabHelper.LogInfo($"Assigned ID {Id}", log, listOfLogs);
            
            CompletePurchaseArg param = null;

            try
            {
                param = JsonConvert.DeserializeObject<CompletePurchaseArg>(context.FunctionArgument.ToString());
            }
            catch(Exception e)
            {
                string functionarg = context.FunctionArgument.ToString();
                PlayFabHelper.LogInfo("There was an error here's the function argument no reason: " + functionarg, log, listOfLogs);
                PlayFabHelper.LogInfo(PlayFabHelper.CaptureException(e, log), log, listOfLogs);
            }
            
            if(param != null)
            {
                foreach(string t in param.ItemIDs)
                {
                    log.LogInformation($"Parsed tag number {param.ItemIDs.IndexOf(t)} : {t}");
                }

                var catalogTask = PlayFabHelper.GetCatalogItem();
                await catalogTask;

                if(OnComplete(catalogTask.Result))
                {
                    listOfLogs.Add("Catalog Get was successful");
                    uint cost = 0;
                    List<string> verifiedItemIDs = new List<string>();
                    
                    foreach(var item in catalogTask.Result.Result.Catalog)
                    {
                        if(param.ItemIDs.IndexOf(item.ItemId) > -1)
                        {
                            verifiedItemIDs.Add(item.ItemId);
                            cost += item.VirtualCurrencyPrices[param.VC];
                        }
                    }

                    var grantItemsTask = PlayFabHelper.GrantItemsToUser(Id, verifiedItemIDs, logger, listOfLogs);
                    await grantItemsTask;
                    
                    if(OnComplete(grantItemsTask.Result))
                    {
                        var subtractTask = PlayFabHelper.SubTractUserVC((int)cost, Id, param.VC);
                    }
                    else
                    {
                        listOfLogs.Add("Grant Items call was not successful");
                        return new OkObjectResult(listOfLogs);
                    }

                    


                }
                else
                {
                        
                    listOfLogs.Add("Catalog Get has failed");
                    return new OkObjectResult(listOfLogs);
                }
                

            }
            else
            {
                listOfLogs.Add("Param was null");
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
            
            listOfLogs.Add("Param was null");
            return new OkObjectResult(listOfLogs);
        }

        public static bool OnComplete(PlayFabResult<GetCatalogItemsResult> catalogResult)
        {
            logger.LogInformation("Checking to see if the calls were successful");
            //PlayFabHelper.LogInfo("Checking to see if the calls were successful", logger, listOfLogs);
            if(PlayFabHelper.WasPlayFabCallSuccessful<GetCatalogItemsResult>(catalogResult, logger))
            {
                logger.LogInformation("Successful GetCatalog call. Now preparing call to subtract VC");
                return true;

            }
            else
            {
                logger.LogInformation("GetCatalog Failed");
                return false;
            }
        }

        public static bool OnComplete(PlayFabResult<GrantItemsToUserResult> grantResult)
        {
            logger.LogInformation("Checking to see if the calls were successful");
            if(PlayFabHelper.WasPlayFabCallSuccessful<GrantItemsToUserResult>(grantResult, logger))
            {
                logger.LogInformation("The Item grant operation was successful");
                return true;
            }
            else
            {
                logger.LogInformation("Grant operation Failed");
                return false; 
                
            }
        }
    }
}
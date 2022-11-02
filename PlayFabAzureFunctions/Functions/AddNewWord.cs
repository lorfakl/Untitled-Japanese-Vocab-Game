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

    public static class AddNewWord
    {
        static string responseString = "";
        static string secretKey = "PlayFabSecretKey";
        static string titleID = "titleId";
        static string Id = "";
        static int numOfWordsToAdd = 0;
        static int wordsSeen = 0;
        public static ILogger logger = null;

        [FunctionName("AddNewWords")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,ILogger log)
        {

            PlayFabSettings.staticSettings.TitleId = Environment.GetEnvironmentVariable(titleID);
            PlayFabSettings.staticSettings.DeveloperSecretKey = Environment.GetEnvironmentVariable(secretKey);
            logger = log;
            FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());
            dynamic args = context.FunctionArgument;
            
           
            Id = context.CallerEntityProfile.Lineage.MasterPlayerAccountId;
            

            string count = req.Query["count"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation($"RequestBody: {requestBody}");

            dynamic data = JsonConvert.DeserializeObject(requestBody);
            count = count ?? data?.FunctionArgument?.count;
    
            
            try
            {
                numOfWordsToAdd = Int32.Parse(count);
            }
            catch(Exception ex)
            {
                PlayFabHelper.CaptureException(ex, logger);
            }
            

            logger.LogInformation("Calling GameServer/GetUserData for user: " + Id);
            var getTitleDataTask = PlayFabHelper.GetTitleData(new List<string> { "CommonWords" });
            await getTitleDataTask;
            var getUserDataTask = PlayFabHelper.GetUserData(Id, new List<string> {"LeitnerLevels", "WordsSeen"});
            await getUserDataTask;
            logger.LogInformation("Awaiting PF API calls");
            //
            //

            logger.LogInformation("Calling OnCompleteFor PF calls");
            OnComplete(getTitleDataTask.Result, getUserDataTask.Result);

            return new OkObjectResult(responseString);
        }

        public static async void OnComplete(PlayFabResult<GetTitleDataResult> titleResult, PlayFabResult<GetUserDataResult> userResult)
        {
            logger.LogInformation("Checking to see if the calls were successful");
            if(PlayFabHelper.WasPlayFabCallSuccessful<GetTitleDataResult>(titleResult, logger) && PlayFabHelper.WasPlayFabCallSuccessful<GetUserDataResult>(userResult, logger))
            {
                logger.LogInformation("The calls were successful");
                List<JapaneseWord> wordsToAdd = new List<JapaneseWord>();
                try
                {
                    wordsSeen = Int32.Parse(userResult.Result.Data["WordsSeen"].Value);
                    logger.LogInformation("Words Seen conversion compete. Number of words seen: " + wordsSeen);
                    logger.LogInformation("Attempting to deserialize result and parse");

              
                    List<JapaneseWord> newWords = JsonConvert.DeserializeObject<List<JapaneseWord>>(titleResult.Result.Data["CommonWords"]);
                    
                    
                    

                    if(numOfWordsToAdd == 0)
                    {
                        numOfWordsToAdd = 10;
                    }

                    wordsToAdd = newWords.GetRange(wordsSeen, numOfWordsToAdd);
                    wordsSeen += numOfWordsToAdd;
                }
                catch(Exception ex)
                {
                    PlayFabHelper.CaptureException(ex, logger);
                    logger.LogInformation("Something went wrong with the response processing");
                }

                Dictionary<string, List<JapaneseWord>> currentLeitnerDict = new Dictionary<string, List<JapaneseWord>>();
                logger.LogInformation("Adding " + wordsToAdd.Count + " words to level Zero");
                try
                {
                    string leitnerJson = userResult.Result.Data["LeitnerLevels"].Value;
                    logger.LogInformation("GetUserDataResult: " + leitnerJson);
                    currentLeitnerDict = JsonConvert.DeserializeObject<Dictionary<string, List<JapaneseWord>>>(leitnerJson);
                    currentLeitnerDict[ProficiencyLevels.Zero.ToString()].AddRange(wordsToAdd);
                }
                catch(Exception ex)
                {
                    PlayFabHelper.CaptureException(ex, logger);
                }
                
                logger.LogInformation("Calling GameServer/UpdateUserData for user: " + Id);
                var updateTask = PlayFabHelper.UpdateUserData(Id, new Dictionary<string, string>
                    {
                        {"WordsSeen", wordsSeen.ToString()},
                        {"LeitnerLevels", JsonConvert.SerializeObject(currentLeitnerDict)}
                    });
                await updateTask;
                PlayFabHelper.WasPlayFabCallSuccessful<UpdateUserDataResult>(updateTask.Result, logger);
             
            }
            else
            {
                logger.LogInformation("One of the calls failed");
            }
        }
    }
}
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

        static int progress = 0;
        static int expectedListCount = 0;
        static readonly int numberOfKana = 92;
        static readonly int numberOfKanji = 1000;
        static TitleDataKeys wordSelection;


        public static ILogger logger = null;

        static List<string> listOfLogs = null;

        [FunctionName("AddNewWords")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,ILogger log)
        {

            PlayFabSettings.staticSettings.TitleId = Environment.GetEnvironmentVariable(titleID);
            PlayFabSettings.staticSettings.DeveloperSecretKey = Environment.GetEnvironmentVariable(secretKey);
            logger = log;
            FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());
            //dynamic args = context.FunctionArgument;
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation($"RequestBody: {requestBody}");
            AddWordArgument args = null;
            
            try
            {
                args = JsonConvert.DeserializeObject<AddWordArgument>(context.FunctionArgument.ToString());
            }
            catch(Exception e)
            {
                PlayFabHelper.CaptureException(e, logger);
                PlayFabHelper.LogInfo(requestBody, logger, listOfLogs);
            }
            
            
           
            Id = context.CallerEntityProfile.Lineage.MasterPlayerAccountId;
            

            numOfWordsToAdd = args.NumToAdd;
            progress = args.Progress;
            bool isKanjiStudy = args.IsKanjiStudyTopic;

        
            logger.LogInformation("Calling GameServer/GetUserData for user: " + Id);


            if(isKanjiStudy)
            {
                wordSelection = TitleDataKeys.CommonWords;
                expectedListCount = numberOfKanji;
            }
            else
            {
                wordSelection = TitleDataKeys.Kana;
                expectedListCount = numberOfKana;
            }

            var getTitleDataTask = PlayFabHelper.GetTitleData(new List<string>{ wordSelection.ToString()});
            await getTitleDataTask;
            var getUserDataTask = PlayFabHelper.GetUserData(Id, new List<string> {UserDataKey.LeitnerLevels.ToString()});
            await getUserDataTask;
            logger.LogInformation("Awaiting PF API calls");
            //
            //

            logger.LogInformation("Calling OnCompleteFor PF calls");
            OnComplete(getTitleDataTask.Result, getUserDataTask.Result);

            return new OkObjectResult(listOfLogs);
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
                    List<JapaneseWord> newWords = JsonConvert.DeserializeObject<List<JapaneseWord>>(titleResult.Result.Data[wordSelection.ToString()]);
                    
                    if(newWords.Count == expectedListCount)
                    {
                        if(progress + numOfWordsToAdd < newWords.Count-1)
                        {
                            wordsToAdd = newWords.GetRange(progress, numOfWordsToAdd);
                            progress += numOfWordsToAdd;
                        }
                        else if(progress < newWords.Count - 1)
                        {
                            int maxIndex = newWords.Count - 1;
                            wordsToAdd = newWords.GetRange(progress, maxIndex - progress);
                            progress += maxIndex;
                        }
                    }
                    else
                    {
                        throw new Exception($"The list from pf is not equal to what it should be. Expected List Count: {expectedListCount} List Count:{newWords.Count} Get Title Result: {titleResult.Result.ToString()}");
                    }
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
                    string leitnerJson = userResult.Result.Data[UserDataKey.LeitnerLevels.ToString()].Value;
                    logger.LogInformation("GetUserDataResult: " + leitnerJson);
                    currentLeitnerDict = JsonConvert.DeserializeObject<Dictionary<string, List<JapaneseWord>>>(leitnerJson);
                    currentLeitnerDict[ProficiencyLevels.Zero.ToString()].AddRange(wordsToAdd);
                }
                catch(Exception ex)
                {
                    PlayFabHelper.CaptureException(ex, logger);
                    PlayFabHelper.LogInfo($"Leiter JSON: {userResult.Result.Data[UserDataKey.LeitnerLevels.ToString()].Value}", logger, listOfLogs);
                }
                
                logger.LogInformation("Calling GameServer/UpdateUserData for user: " + Id);
                string leitnerUpdate = JsonConvert.SerializeObject(currentLeitnerDict);
                PlayFabHelper.LogInfo($"Adding Value: {progress} to User Key:{UserDataKey.WordsSeen.ToString()}", logger, listOfLogs);
                PlayFabHelper.LogInfo($"Adding Value: {leitnerUpdate} to User Key:{UserDataKey.LeitnerLevels.ToString()}", logger, listOfLogs);
                
                var updateTask = PlayFabHelper.UpdateUserData(Id, new Dictionary<string, string>
                    {
                        {UserDataKey.WordsSeen.ToString(), progress.ToString()},
                        {UserDataKey.LeitnerLevels.ToString(), leitnerUpdate}
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
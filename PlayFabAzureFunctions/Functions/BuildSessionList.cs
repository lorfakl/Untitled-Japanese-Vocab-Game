using System;
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
    public static class BuildSessionList
    {
        static string responseString = "";
        static string secretKey = "PlayFabSecretKey";
        static string Id = "";
        static string sessionWordString = "";

        [FunctionName("BuildSessionList")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,ILogger log)
        {

            PlayFabSettings.staticSettings.DeveloperSecretKey = Environment.GetEnvironmentVariable(secretKey);

            FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());

            Id = context.CallerEntityProfile.Lineage.MasterPlayerAccountId;
         


            GetUserDataRequest rq = new GetUserDataRequest
            {
                Keys = new List<string> { "LeitnerLevels", "PrestigeLevels", "NextSession" },
                PlayFabId = Id
            };
            
            var getUserDataTask = PlayFabServerAPI.GetUserDataAsync(rq);
            await getUserDataTask;
        
            OnComplete(getUserDataTask.Result);

            return new OkObjectResult(responseString);
        }

        public static void OnComplete(PlayFabResult<GetUserDataResult> result)
        {
            if(result.Error == null)
            {
                List<JapaneseWord> sessionWords = new List<JapaneseWord>();

                string leitnerJson = result.Result.Data[UserDataKey.LeitnerLevels.ToString()].Value.ToString();
                string prestigeJson = result.Result.Data[UserDataKey.PrestigeLevels.ToString()].Value.ToString();
                string nextSessionJson = result.Result.Data[UserDataKey.NextSession.ToString()].Value.ToString();

                Dictionary<string, List<JapaneseWord>> currentLeitnerDict = JsonConvert.DeserializeObject<Dictionary<string, List<JapaneseWord>>>(leitnerJson);
                Dictionary<string, List<JapaneseWord>> currentPrestigeDict = JsonConvert.DeserializeObject<Dictionary<string, List<JapaneseWord>>>(prestigeJson);
                List<ProficiencyLevels> sessionKeys = JsonConvert.DeserializeObject<List<ProficiencyLevels>>(nextSessionJson);
                
                foreach(string key in currentLeitnerDict.Keys)
                {
                    if(sessionKeys.Contains(HelperFunctions.ParseEnum<ProficiencyLevels>(key)))
                    {
                        sessionWords.AddRange(currentLeitnerDict[key]);
                    }
                }

                foreach(string key in currentPrestigeDict.Keys)
                {
                    if(sessionKeys.Contains(HelperFunctions.ParseEnum<ProficiencyLevels>(key)))
                    {
                        sessionWords.AddRange(currentPrestigeDict[key]);
                    }
                }
                
                sessionWordString = JsonConvert.SerializeObject(sessionWords);
                UpdateSessionWords();
                
            }
            else
            {
                responseString = result.Error.GenerateErrorReport();
            }
        }

        public static void UpdateSessionWords()
        {
            PlayFabServerAPI.UpdateUserDataAsync(
                new UpdateUserDataRequest {
                    PlayFabId = Id,
                    Data = new Dictionary<string, string>
                    {
                        {"Session Words", sessionWordString}
                    }
                }
            );
        }
    }

    public class LeitnerLevels
    {
        public List<JapaneseWord> levelZero { get; set; }
        public List<JapaneseWord> levelOne { get; set; }
        public List<JapaneseWord> levelTwo { get; set; }
        public List<JapaneseWord> levelThree { get; set; }
        public List<JapaneseWord> levelFour { get; set; }
        public List<JapaneseWord> levelFive { get; set; }
        public List<JapaneseWord> levelSix { get; set; }
    }

    public class PrestigeLevels
    {
        public List<JapaneseWord> levelZero { get; set; }
        public List<JapaneseWord> levelOne { get; set; }
        public List<JapaneseWord> levelTwo { get; set; }
        public List<JapaneseWord> levelThree { get; set; }
        public List<JapaneseWord> levelFour { get; set; }
        public List<JapaneseWord> levelFive { get; set; }
        public List<JapaneseWord> levelSix { get; set; }
        public List<JapaneseWord> levelSeven { get; set; }
        public List<JapaneseWord> levelEight { get; set; }
    }
}
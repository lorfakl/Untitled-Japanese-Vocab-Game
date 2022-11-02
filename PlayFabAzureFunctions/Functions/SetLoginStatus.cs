using System;
using System.Linq;
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
    

    public static class SetLoginStatus
    {
        static string responseString = "";
        static string secretKey = "PlayFabSecretKey";
        static string titleID = "titleId";
        static string Id = "";

        [FunctionName("SetLoginStatus")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,ILogger log)
        {

            PlayFabSettings.staticSettings.TitleId = Environment.GetEnvironmentVariable(titleID);
            PlayFabSettings.staticSettings.DeveloperSecretKey = Environment.GetEnvironmentVariable(secretKey);

            FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());
            dynamic args = context.FunctionArgument;

           
            Id = context.CallerEntityProfile.Lineage.MasterPlayerAccountId;
            GetUserDataRequest rq = new GetUserDataRequest
            {
                Keys = new List<string> { "LoginCount" },
                PlayFabId = context.CallerEntityProfile.Lineage.MasterPlayerAccountId
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
                List<string> sessionLevels = new List<string>();
                int numOfLogins = 1;
                try
                {
                    string loginCount = result.Result.Data["LoginCount"].Value;
                    numOfLogins = Int32.Parse(loginCount);
                    
                    numOfLogins++;
                    
                    sessionLevels.Add(ProficiencyLevels.Zero.ToString());
                    if(numOfLogins % 2 == 1)
                    {
                        sessionLevels.Add(ProficiencyLevels.One.ToString());
                    }
                    
                    foreach(ProficiencyLevels level in Enum.GetValues(typeof(ProficiencyLevels)).Cast<ProficiencyLevels>())
                    {
                        if(level != ProficiencyLevels.Zero && level != ProficiencyLevels.One)
                        {
                            if( numOfLogins % (int)level == 0)
                            {
                                sessionLevels.Add(level.ToString());
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    responseString += ex.Message;
                    sessionLevels.Add(ProficiencyLevels.Zero.ToString());
                }
                

                PlayFabServerAPI.UpdateUserDataAsync(
                new UpdateUserDataRequest 
                {
                    PlayFabId = Id,
                    Data = new Dictionary<string, string>
                    {
                        {"LoginCount", numOfLogins.ToString()},
                        {"NextSession", JsonConvert.SerializeObject(sessionLevels)}
                    }
                }
                );
                
            }
            else
            {
                responseString += result.Error.ErrorMessage + "\n" + result.Error.ErrorDetails + "\n" + result.Error.GenerateErrorReport();
            }
        }
    }

    

  
}
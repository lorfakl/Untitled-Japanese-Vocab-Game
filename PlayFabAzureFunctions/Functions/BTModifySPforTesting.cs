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
using PlayFab.ServerModels;

namespace PlayFabCloudScript.OnLogin
{
    public static class BTModifySPforTesting
    {
        static string responseString = "";
        static string secretKey = "PlayFabSecretKey";
        static string Id = "";

        [FunctionName("BotTestingModifySP")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,ILogger log)
        {
            FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());

            Id = context.CallerEntityProfile.Lineage.MasterPlayerAccountId;

            PlayFabSettings.staticSettings.DeveloperSecretKey = Environment.GetEnvironmentVariable(secretKey);

            AddUserVirtualCurrencyRequest rq = new AddUserVirtualCurrencyRequest
            {
                Amount = 50,
                PlayFabId = Id,
                VirtualCurrency = "SP"
            };

            var addVCTask = PlayFabServerAPI.AddUserVirtualCurrencyAsync(rq);
            await addVCTask;
        
            OnComplete(addVCTask.Result);

            return new OkObjectResult(responseString);
        }

        public static void OnComplete(PlayFabResult<ModifyUserVirtualCurrencyResult> result)
        {
            if(result.Error == null)
            {
                responseString = "PlayFabID: " + result.Result.PlayFabId + " has modified " + result.Result.VirtualCurrency + " by " + result.Result.BalanceChange +
                " The new total is: " + result.Result.Balance;
            }
            else
            {
                responseString = result.Error.GenerateErrorReport();
            }
        }

    }
}
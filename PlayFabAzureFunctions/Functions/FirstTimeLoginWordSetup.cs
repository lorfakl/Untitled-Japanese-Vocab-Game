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
 public static class FirstTimeLoginWordSetup
 {
    static string responseString = "";
    static string secretKey = "PlayFabSecretKey";
    static string Id = "";

    [FunctionName("FirstTimeWordSetup")]
    public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,ILogger log)
    {
        FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());

        Id = context.CallerEntityProfile.Lineage.MasterPlayerAccountId;

        PlayFabSettings.staticSettings.DeveloperSecretKey = Environment.GetEnvironmentVariable(secretKey);

        GetTitleDataRequest rq = new GetTitleDataRequest
        {
            Keys = new List<string> { "Starter Words" }
        };
       var getTitleDataTask = PlayFabServerAPI.GetTitleDataAsync(rq);
       await getTitleDataTask;
    
        OnComplete(getTitleDataTask.Result);

        return new OkObjectResult(responseString);
    }

    public static void OnComplete(PlayFabResult<GetTitleDataResult> result)
    {
        if(result.Error == null)
        {
            OnSuccess(result.Result);
        }
        else
        {
            responseString = result.Error.GenerateErrorReport();
        }
    }

    public static async void OnSuccess(GetTitleDataResult result)
    {
        var updateDataTask = PlayFabServerAPI.UpdateUserDataAsync( new UpdateUserDataRequest 
        {
            Data = new Dictionary<string, string> 
            {
                {"Session Kana", result.Data["StarterKana"]},
                {"Session Words", result.Data["Starter Words"]},
                {"LoginCount", "0"},
                {"WeeklySP", "0"},
                {"TotalSP", "0"}
            },
            PlayFabId = Id
        });

        await updateDataTask;
        OnComplete(updateDataTask.Result);
    }

    public static void OnComplete(PlayFabResult<UpdateUserDataResult> result)
    {
        if(result.Error == null)
        {
            responseString = "Initial Data transfer successful";
        }
        else
        {
            responseString = result.Error.GenerateErrorReport();
        }
    }

 }
}
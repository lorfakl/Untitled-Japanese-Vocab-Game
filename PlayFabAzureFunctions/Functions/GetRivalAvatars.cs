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
using PlayFab.DataModels;
using Utilities;

namespace PlayFabCloudScript.Rivals
{
    public static class GetRivalAvatars
    {
        static string responseString = "";
        static string secretKey = "PlayFabSecretKey";
        static string titleID = "titleId";
        static string Id = "";
        public static ILogger logger = null;

        static List<string> listOfLogs = null;
        

        [FunctionName("GetRivalAvatars")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,ILogger log)
        {

            PlayFabSettings.staticSettings.TitleId = Environment.GetEnvironmentVariable(titleID);
            PlayFabSettings.staticSettings.DeveloperSecretKey = Environment.GetEnvironmentVariable(secretKey);
            logger = log;
            listOfLogs = new List<string>();
            List<PlayFabFileInfo> returnList = new List<PlayFabFileInfo>();
            PlayFabHelper.LogInfo("Log List Initialized", log, listOfLogs);

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            
            FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(requestBody);
            
            PlayFabHelper.LogInfo("Context parsed", log, listOfLogs);
            await PlayFabHelper.GetEntityToken();
            /*try
            {
                context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());
                PlayFabHelper.LogInfo("Context parsed", log, listOfLogs);
            }
            catch(Exception e)
            {
                PlayFabHelper.LogInfo("Exception Thrown!", log, listOfLogs);
                PlayFabHelper.LogInfo(PlayFabHelper.CaptureException(e, log), log, listOfLogs);
                PlayFabHelper.LogInfo("Request Body: " + requestBody, log, listOfLogs);
                return new OkObjectResult(listOfLogs);
            }*/
            
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
            

            /*try
            {
                args 
            }
            catch(Exception e)
            {
                PlayFabHelper.LogInfo("Exception Thrown!", log, listOfLogs);
                PlayFabHelper.LogInfo(PlayFabHelper.CaptureException(e, log), log, listOfLogs);
                PlayFabHelper.LogInfo("Request Body: " + requestBody, log, listOfLogs);
                return new OkObjectResult(listOfLogs);
            }*/
            
            dynamic args = context.FunctionArgument;
            List<UniversalEntityKey> rivalKeys = null;
            try
            {
                rivalKeys = JsonConvert.DeserializeObject<List<UniversalEntityKey>>(args["Rivals"].ToString());
            }
            catch(Exception e)
            {
                PlayFabHelper.LogInfo("Exception Thrown!", log, listOfLogs);
                PlayFabHelper.LogInfo(PlayFabHelper.CaptureException(e, log), log, listOfLogs);
                PlayFabHelper.LogInfo("Request Body: " + requestBody, log, listOfLogs);
                return new OkObjectResult(listOfLogs);
            }
            PlayFabHelper.LogInfo($"Created EKey List Count of {rivalKeys.Count}", log, listOfLogs);

            //Id = context.CallerEntityProfile.Lineage.MasterPlayerAccountId;
            
            List<Task<PlayFabResult<GetFilesResponse>>> getFileRequest = new List<Task<PlayFabResult<GetFilesResponse>>>();
            foreach(var key in rivalKeys)
            {
                getFileRequest.Add(PlayFabHelper.GetEntityFiles(key));
            }

            PlayFabHelper.LogInfo($"API calls made", log, listOfLogs);
            foreach(var task in getFileRequest)
            {
                await task;
                PlayFabHelper.LogInfo($"Task Awaitede", log, listOfLogs);
                PlayFabFileInfo f = OnComplete(task.Result);
                if(f != null)
                {
                    returnList.Add(f);
                }
            }

            return new OkObjectResult(returnList);
        }

        static PlayFabFileInfo OnComplete(PlayFabResult<GetFilesResponse> getFilesResponse)
        {
            logger.LogInformation("Checking to see if the calls were successful");
            if(PlayFabHelper.WasPlayFabCallSuccessful<GetFilesResponse>(getFilesResponse, logger))
            {
                logger.LogInformation("The calls were successful");
                
                return FindAvatarFileInfo(getFilesResponse.Result.Metadata);
            }
            else
            {
                PlayFabHelper.LogInfo("Get Files Response", logger, listOfLogs);
                return default;
            }
        }
    
        static PlayFabFileInfo FindAvatarFileInfo(Dictionary<string, GetFileMetadata> metaDataDict)
        {
            PlayFabFileInfo avatarFileInfo = null;
            foreach(var pair in metaDataDict)
            {
                if(pair.Value.FileName.Contains("vatar"))
                {
                    logger.LogInformation("Checking to see if the calls were successful");
                    avatarFileInfo = new PlayFabFileInfo
                    (
                        pair.Value.FileName,
                        pair.Value.DownloadUrl,
                        String.Empty,
                        pair.Value.Size,
                        pair.Value.LastModified
                    );
                    break;
                }
            }

            return avatarFileInfo;
        }
    }
}

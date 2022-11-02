using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using PlayFab;
using PlayFab.Samples;
using PlayFabCloudScript;
using PlayFab.ServerModels;
using PlayFabCloudScript.OnLogin;

public enum UserDataKey
{
    LeitnerLevels,
    PrestigeLevels,
    SessionWords,
    LoginCount,
    NextSession,
    WordsSeen,
    TotalSP,
    WeeklySP
}

public enum TitleDataKeys
{
    StarterWords,
    CommonWords
}

public static class PlayFabHelper
    {
        #region PlayFab Custom Event Name Enums
        public enum CustomEventNames
        {
            
        }
        #endregion

        public static string TitleID
        {
            get;
            private set;
        }

        public static string PlayFabID
        {
            get;
            private set;
        }

        public static string EntityToken
        {
            get;
            private set;
        }

        public static string SessionTicket
        {
            get;
            private set;
        }


        public static void WritePlayStreamEvent(WriteServerPlayerEventRequest eventData)
        {
            
        }

        
        
        public static Task<PlayFabResult<GetUserDataResult>> GetUserData(string id, List<string> keys)
        {
            var playfabHttpTask = PlayFabServerAPI.GetUserDataAsync( new GetUserDataRequest 
            {
                PlayFabId = id,
                Keys = keys
            });

            return playfabHttpTask;
        }

        public static Task<PlayFabResult<GetTitleDataResult>> GetTitleData(List<string> keys)
        {
            var playfabHttpTask = PlayFabServerAPI.GetTitleDataAsync( new GetTitleDataRequest 
            {
                Keys = keys
            });

            //playfabHttpTask.ContinueWith(ProcessPlayFabRequest);
            return playfabHttpTask;
        }

        public static Task<PlayFabResult<UpdateUserDataResult>> UpdateUserData(string id, Dictionary<string, string> dict)
        {
            AddNewWord.logger.LogInformation("Attempting to call GameServer/UpdateUserData on ID: " + id);
            var playfabHttpTask = PlayFabServerAPI.UpdateUserDataAsync( new UpdateUserDataRequest 
            {
                PlayFabId = id,
                Data = dict
            });

            return playfabHttpTask;
        }

        public static void ProcessPlayFabRequest<T>(PlayFabResult<T> playFabResult, Action<PlayFabResult<T>> callback, ILogger log) where T : PlayFab.Internal.PlayFabResultCommon
        {
            if(playFabResult.Error == null)
            {
                callback(playFabResult);
            }
            else
            {
                CapturePlayFabError(playFabResult.Error, log);
            }
        }

        public static bool WasPlayFabCallSuccessful<T>(PlayFabResult<T> playFabResult, ILogger log) where T : PlayFab.Internal.PlayFabResultCommon
        {
            if(playFabResult.Error == null)
            {
                return true;
            }
            else
            {
                CapturePlayFabError(playFabResult.Error, log);
                return false;
            }
        }

        public static void CaptureException(Exception ex, ILogger log)
        {
            string err = ex.Message + "Inner Exception:  " + ex.InnerException + " Stack Trace:" + ex.StackTrace+ " Source:" + ex.Source;
            log.Log(LogLevel.Error, err);
        }

        public static void CapturePlayFabError(PlayFabError error, ILogger log)
        {
            string fullErrorDetails = "Error in PlayFab API: " + error.RequestId + "\n" +
                "Error: " + error.Error.ToString() + "\n" + "Error Message: " + error.ErrorMessage
                + "\n" + "Error Details: " + error.ErrorDetails.ToString() + "\n" + error.GenerateErrorReport();
            log.Log(LogLevel.Error, fullErrorDetails);
        }
    }
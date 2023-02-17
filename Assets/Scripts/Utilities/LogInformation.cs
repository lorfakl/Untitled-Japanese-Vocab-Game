using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

namespace Utilities.Logging
{
    public enum EventNamespace
    {
        example,
        UserStudyData,
        StoreData,
        ProfileData,
        Logging,
        Errors
    }

    public enum EventName
    {
        something_happened,
        study_object_selection_made,
        entered_store,
        left_store,
        purchase_made,
        entered_profile,
        left_profile,
        avatar_item_equipped,
        unhandled_exception_logged,
        playfab_api_error,
        user_ui_event,
        generalized_debug,
        unity_exception_received
    }

    public abstract class LogBody
    { }

    public class UnhandledExceptionLogBody : LogBody
    {
        [JsonProperty("Message")]
        public string message;

        [JsonProperty("StackTrace")]
        public string stackTrace;

        [JsonProperty("Source")]
        public string source;

        [JsonProperty("InnerException")]
        public string innerException;

        [JsonProperty("Data")]
        public string data;

        [JsonProperty("HResult")]
        public string hresult;

        public UnhandledExceptionLogBody() { }

        public UnhandledExceptionLogBody(Exception e)
        {
            message = e.Message;
            stackTrace = e.StackTrace;
            source = e.Source;
            innerException = e.InnerException.ToString();
            data = HelperFunctions.PrintDictContent(e.Data);
            hresult = e.HResult.ToString();
        }
    }

    public class GeneralLogBody : LogBody
    {
        [JsonProperty("Class")]
        public string classOrigin;

        [JsonProperty("Function")]
        public string function;

        [JsonProperty("StackTrace")]
        public string stackTrace;

        [JsonProperty("LogType")]
        public string logType;

        [JsonProperty("Context")]
        public string context;

        [JsonProperty("Message")]
        public string message;

        public GeneralLogBody() { }

        public GeneralLogBody(string classOrigin, LogType t, string function, string context, string message)
        {
            this.classOrigin = classOrigin.ToString();
            this.function = function;
            this.context = context;
            this.message = message;
            this.logType = t.ToString();
        }
    }

    public class PlayFabErrorLog : LogBody
    {
        [JsonProperty("Endpoint")]
        public string apiEndpoint;

        [JsonProperty("Message")]
        public string message;

        [JsonProperty("Error Summary")]
        public string errorSummary;

        [JsonProperty("PlayFab ErrorCode")]
        public string errorCode;

        [JsonProperty("HTTP Code")]
        public string httpCode;

        [JsonProperty("HTTP Status")]
        public string httpStatus;

        [JsonProperty("Error Details")]
        public string errorDetails;

        public PlayFabErrorLog(PlayFab.PlayFabError error)
        {
            apiEndpoint = error.ApiEndpoint;
            message = error.ErrorMessage;
            errorSummary = error.GenerateErrorReport();
            httpCode = error.HttpCode.ToString();
            httpStatus = error.HttpStatus;
            errorDetails = "";
            if(error.ErrorDetails != null)
            {
                foreach (var k in error.ErrorDetails.Keys)
                {
                    List<string> l = error.ErrorDetails[k];
                    foreach (var v in l)
                    {
                        errorDetails += v + "\n";
                    }
                }
            }
  
            int specificCode = (int)error.Error;
            errorCode = specificCode.ToString() + " " + error.Error.ToString();
        }

        public override string ToString()
        {
            return $"Endpoint: {apiEndpoint} \n" +
                $"Message: {message} \n" +
                $"Error Summary: {errorSummary} \n" +
                $"PlayFab ErrorCode: {errorCode} \n" +
                $"HTTP Code: {httpCode} \n" +
                $"HTTP Status: {httpStatus} \n" +
                $"Error Details: {errorDetails}";
        }
    }
}


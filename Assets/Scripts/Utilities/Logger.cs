using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Utilities.PlayFabHelper;
using Utilities.Logging;
using Newtonsoft.Json;


public class Logger : MonoBehaviour
{
    private static List<TelemetryWrapper> logEvents = new List<TelemetryWrapper>();
    private static int eventBatch = 10;
    private void Awake()
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledExceptionHandler;
        Application.logMessageReceivedThreaded += UnityLogMessageReceived_Handler;
        Application.logMessageReceived += UnityLogMessageReceived_Handler;
    }
 
    private void OnDisable()
    {
        Application.logMessageReceivedThreaded -= UnityLogMessageReceived_Handler;
        Application.logMessageReceived -= UnityLogMessageReceived_Handler;
        AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledExceptionHandler;
        PublishTelemtry();
    }
    public static void LogGeneralDebug(GeneralLogBody e)
    {

    }

    public static void LogPlayFabError(PlayFabErrorLog e)
    {
        TelemetryWrapper t = new TelemetryWrapper(EventNamespace.ProfileData, EventName.playfab_api_error, JsonConvert.SerializeObject(e));
        logEvents.Add(t);
    }

    private void UnityLogMessageReceived_Handler(string condition, string stackTrace, LogType type)
    {
        bool immediatePublish = false;
        if (type == LogType.Error || type == LogType.Exception)
        {
            immediatePublish = true;
            GeneralLogBody log = new GeneralLogBody
            {
                message = condition,
                stackTrace = stackTrace,
                logType = type.ToString()
            };
            CreateLogEntry(EventName.unity_exception_received, EventNamespace.Errors, log, immediatePublish);
            return;
        }

        if (Playfab.VerboseModeEnabled)
        {
            GeneralLogBody log = new GeneralLogBody
            {
                message = condition,
                stackTrace = stackTrace,
                logType = type.ToString()
            };

            CreateLogEntry(EventName.generalized_debug, EventNamespace.Logging, log, immediatePublish);
        }
    }

    private void CurrentDomain_UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
    {
        Exception unhandledException = e.ExceptionObject as Exception;
        UnhandledExceptionLogBody exceptionLogBody = new UnhandledExceptionLogBody(unhandledException);
        CreateLogEntry(EventName.unhandled_exception_logged, EventNamespace.Logging, exceptionLogBody, true);
    }

    private void CreateLogEntry(EventName name, EventNamespace nameSpace, LogBody l, bool shouldPublishNow = false)
    {
        TelemetryWrapper t = new TelemetryWrapper(nameSpace, name, JsonConvert.SerializeObject(l));

        logEvents.Add(t);
        if(shouldPublishNow || logEvents.Count >= eventBatch)
        {
            PublishTelemtry();
        }
    }

    private void PublishTelemtry()
    {
        PlayFabController.WriteTelemetryEvents(logEvents);
        logEvents.Clear();
    }

        
    
}
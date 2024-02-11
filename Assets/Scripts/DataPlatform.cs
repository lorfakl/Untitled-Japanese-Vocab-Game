using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Utilities.Events;
using Utilities.PlayFabHelper;
using Utilities.PlayFabHelper.CurrentUser;
using Utilities.SaveOperations;
using Utilities.Logging;
using ProjectSpecificGlobals;
using System.Diagnostics;
using System.Linq;

public class DataPlatform : MonoBehaviour
{
    [SerializeField]
    GameEvent statReportReady;

    bool hasStartStudying = false;
    Stopwatch studySessionTimer = null;

    Dictionary<string, UserStudyData> sessionData = new Dictionary<string, UserStudyData>();
    StudyRecord StudyRecord = null;
    int correctCount = 0;
    double totalTimeStudied = 0;
    public void OnNextWordSelected()
    {
        if (!hasStartStudying)
        {
            hasStartStudying = true;
            studySessionTimer.Start();
            return;
        }

        if (hasStartStudying)
        {
            totalTimeStudied += studySessionTimer.Elapsed.TotalSeconds;
            studySessionTimer.Reset();
            studySessionTimer.Start();
        }
    }

    public void OnStudyObjectSelected()
    {
        studySessionTimer.Stop();
        JapaneseWord currentWord = JSONWordLibrary.CurrentWord;
        string currentWordID = currentWord.ID;

        if (!sessionData.ContainsKey(currentWordID))
        {
            sessionData.Add(currentWordID, new UserStudyData(currentWord));
        }
        
        sessionData[currentWordID].TimesSeen++;
        sessionData[currentWordID].TotalTimeOnScreen += studySessionTimer.Elapsed.TotalSeconds;
        HelperFunctions.Log("On Select CaLLED");
        
    }

    public void OnCorrectAnswer()
    {
        JapaneseWord currentWord = JSONWordLibrary.CurrentWord;
        string currentWordID = currentWord.ID;
        if(currentWord.ID == null)
        {
            int i = 0;
        }

        if (!sessionData.ContainsKey(currentWordID))
        {
            //HelperFunctions.Error($"WordID {currentWordID} should already been in the sessionData {sessionData.Keys.ToList()} this should have already been in the Dict. Investigate");
            sessionData.Add(currentWordID, new UserStudyData(currentWord));
            //sessionData[currentWordID].TimesSeen++;
            //sessionData[currentWordID].TimesCorrect++;
            //sessionData[currentWordID].AnswerSpeed = studySessionTimer.Elapsed.TotalSeconds;
        }
       
        sessionData[currentWordID].TimesCorrect++;
        sessionData[currentWordID].AnswerSpeed = studySessionTimer.Elapsed.TotalSeconds;
        correctCount++;
        HelperFunctions.Log("On CORRECT CaLLED");
    }

    public void OnStudySessionComplete()
    {
        StudySession studySession = new StudySession(sessionData, correctCount, totalTimeStudied);
        StudyRecord.AddSession(studySession);

        /*List<TelemetryWrapper> telemtry = new List<TelemetryWrapper>();
        foreach (var key in StudyRecord.Record.Keys)
        {

            TelemetryWrapper t = new TelemetryWrapper
            {
                Entity = Playfab.UserEntityKey,
                EventName = EventName.study_object_selection_made,
                Namespace = EventNamespace.UserStudyData,
                PayloadJSON = JsonConvert.SerializeObject(StudyRecord.Record[key])
            };
            telemtry.Add(t);
        }
        PlayFabController.WriteTelemetryEvents(telemtry);*/

        SaveSystem.Save(StudyRecord, DataCategory.StatisticRecord);
        statReportReady.Raise(studySession);
    }

    
    
    // Start is called before the first frame update
    void Start()
    {
        if(Globals.UserDataLoaded)
        {
            StudyRecord = Globals.LoadedStudyRecord;
        }
        else
        {
            var data = SaveSystem.Load<StudyRecord>(DataCategory.StatisticRecord);
            if (data == default)
            {
                HelperFunctions.Warning("No data no problem....probably");
                StudyRecord = new StudyRecord();
            }
            else
            {
                StudyRecord = data;
            }
        }
        
        studySessionTimer = new Stopwatch();
        correctCount = 0;
        totalTimeStudied = 0;
    }


    private void OnDisable()
    {
        
        
    }

    public static double GetOverallAverage()
    {
        if(CheckIfStudyRecordLoaded())
        {
            return Globals.LoadedStudyRecord.GetCurrentOverallAverage();
        }
        else 
        {
            return 0;
        }
        throw new NotImplementedException();
    }

    public static int GetWordsKnownCount()
    {
        if (CheckIfStudyRecordLoaded())
        {
            return Globals.LoadedStudyRecord.GetCurrentWordsKnown();
        }
        else
        {
            return -1;
        }
        throw new NotImplementedException();
    }

    public static int GetCurrentStreak()
    {
        if (CheckIfStudyRecordLoaded())
        {
            return Globals.LoadedStudyRecord.GetCurrentStreak();
        }
        else
        {
            return -1;
        }
        throw new NotImplementedException();
    }

    public static int GetLongestStreak()
    {
        if (CheckIfStudyRecordLoaded())
        {
            return Globals.LoadedStudyRecord.GetLongestStreak();
        }
        else
        {
            return -1;
        }
        throw new NotImplementedException();
    }

    public static bool CheckIfStudyRecordLoaded()
    {
        if(Globals.UserDataLoaded) 
        { 
            return true;
        }
        else
        {
            var data = SaveSystem.Load<StudyRecord>(DataCategory.StatisticRecord);
            if(data == default(StudyRecord))
            {
                return false;
            }
            else
            {
                data.InitializeWordIDs();
                Globals.UpdateGlobalStudyRecord(data);
                return true;
            }
        }
    }
}








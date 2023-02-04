using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Utilities.Events;
using Utilities.PlayFabHelper;
using Utilities.SaveOperations;

public class DataPlatform : MonoBehaviour
{
    [SerializeField]
    GameEvent statReportReady;


    static StudyRecord StudyRecord = new StudyRecord();
    public void OnStudyObjectSelected(object s)
    {
        StudyObject studyObject = (StudyObject)s;
        string key = studyObject.Word.ToString();
        if (StudyRecord.Record.ContainsKey(studyObject.Word.ToString()))
        {
            StudyRecord.Record[key].TimesSeen++;
            StudyRecord.Record[key].TimeInFlight = studyObject.TimeInFlight;
        }
        else
        {
            StudyRecord.Record.Add(key, new UserStudyData(studyObject));
        }
    }

    public void OnCorrectAnswer(object s)
    {
        StudyObject studyObject = (StudyObject)s;
        string key = studyObject.Word.ToString();
        if (StudyRecord.Record.ContainsKey(studyObject.Word.ToString()))
        {
            StudyRecord.Record[key].TimesCorrect++;
        }
        else
        {
            StudyRecord.Record.Add(key, new UserStudyData(studyObject));
            StudyRecord.Record[key].TimesCorrect++;
        }
    }

    public void OnStudySessionComplete()
    {
        UpdateStreak();
        List<TelemetryWrapper> telemtry = new List<TelemetryWrapper>();

        foreach (var key in StudyRecord.Record.Keys)
        {

            TelemetryWrapper t = new TelemetryWrapper
            {
                Entity = Playfab.UserEntityKey,
                EventName = EventName.StudyObjectSelectionMade,
                Namespace = EventNamespace.UserStudyData,
                PayloadJSON = JsonConvert.SerializeObject(StudyRecord.Record[key])
            };
            telemtry.Add(t);
        }
        PlayFabController.WriteTelemetryEvents(telemtry);
        SaveSystem.Save(StudyRecord, DataCategory.StatisticRecord);

        statReportReady.Raise(new StudyRecord(StudyRecord));
    }

    public static StudyRecord GetStudyRecord()
    {
        var data = SaveSystem.Load<StudyRecord>(DataCategory.StatisticRecord);
        if (data == default)
        {
            HelperFunctions.Warning("No data no problem....probably");
            return new StudyRecord();
        }
        else
        {
            StudyRecord = data;
            return new StudyRecord(data);
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        var data = SaveSystem.Load<StudyRecord>(DataCategory.StatisticRecord);
        if(data == default)
        {
            HelperFunctions.Warning("No data no problem....probably");
        }
        else
        {
            StudyRecord = data;
        }
    }


    private void OnDisable()
    {
        
        
    }

    private void UpdateStreak()
    {
        if(Playfab.CurrentLoginTime - Playfab.LastLogin > new TimeSpan(24, 0, 0))
        {
            StudyRecord.CurrentStreak++;
            if(StudyRecord.CurrentStreak >= StudyRecord.LongestStreak)
            {
                StudyRecord.LongestStreak = StudyRecord.CurrentStreak;
            }
        }
        else
        {
            StudyRecord.CurrentStreak = 0;
        }
    }
}

[Serializable]
public class UserStudyData
{
    [JsonProperty("Word")]
    public JapaneseWord Word;

    [JsonProperty("TimesSeen")]
    public int TimesSeen;

    [JsonProperty("TimesCorrect")]
    public int TimesCorrect;

    [JsonProperty("TimeInFlight")]
    public float TimeInFlight;

    [JsonConstructor]
    public UserStudyData()
    { }

    public UserStudyData(StudyObject s)
    {
        Word = s.Word;
        TimesSeen = 0;
        TimesCorrect = 0;
        TimeInFlight = s.TimeInFlight;
    }

    public UserStudyData(UserStudyData u)
    {
        this.Word = u.Word;
        this.TimesSeen = u.TimesSeen;
        this.TimesCorrect = u.TimesCorrect;
        this.TimeInFlight = u.TimeInFlight;
    }
}

[Serializable]
public class StudyRecord
{
    public Dictionary<string, UserStudyData> Record = new Dictionary<string, UserStudyData>();

    public int CurrentStreak;

    public int LongestStreak;

    public StudyRecord() { }

    public StudyRecord(StudyRecord s)
    {
        if(s == null)
        {
            new StudyRecord();
        }

        this.Record = new Dictionary<string, UserStudyData>();
        this.CurrentStreak = s.CurrentStreak;
        this.LongestStreak = s.LongestStreak;
        foreach(var pair in s.Record)
        {
            Record.Add(pair.Key, new UserStudyData(pair.Value));
        }
    }

    public Dictionary<string, string> GenerateRecordReport()
    {
        if(Record.Count == 0)
        {
            return new Dictionary<string, string>();
        }

        Dictionary<string, string> report = new Dictionary<string, string>();
        report.Add("Average Answer Speed", CalculateAverageFlightTime().ToString() + "s");
        report.Add("Number of words known", CalculateNumberOfKnownWords().ToString());
        report.Add("Current Streak", this.CurrentStreak.ToString());
        report.Add("Longest Streak", this.LongestStreak.ToString());
        return report;

    }

    public float CalculateAverageFlightTime()
    {
        float averageFlightTime = 0;
        foreach(KeyValuePair<string, UserStudyData> pair in Record)
        {
            averageFlightTime += Record[pair.Key].TimeInFlight;
        }
        averageFlightTime /= Record.Count;
        return averageFlightTime;
    }

    public int CalculateNumberOfKnownWords()
    {
        int knownCount = 0;
        foreach (KeyValuePair<string, UserStudyData> pair in Record)
        {
            try
            {
                float recognitionPercentage = pair.Value.TimesCorrect / pair.Value.TimesSeen;
                if (recognitionPercentage > 0.75f)
                {
                    knownCount++;
                }
            }
            catch(DivideByZeroException)
            {
                return 0;
            }
            
        }
        return knownCount;
    }
}
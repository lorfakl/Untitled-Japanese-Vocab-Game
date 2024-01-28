using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Profiling;

[Serializable]
public class StudySession
{
    [JsonProperty("ID")]
    public Guid ID { get; private set; }

    [JsonProperty("CompletedDate")]
    public DateTime CompletedDate { get; private set; }

    [JsonProperty("NumberOfWords")]
    public int NumberOfWords { get; private set; }

    [JsonProperty("PercentCorrect")]
    public float PercentCorrect { get; private set; }

    [JsonProperty("TimeToComplete")]
    public double TimeToComplete { get; private set; }

    [JsonProperty("AverageSpeed")]
    public double AverageSpeed { get; private set; }

    [JsonProperty("SessionStudyData")]
    public Dictionary<string, UserStudyData> SessionStudyData { get; private set; }

    [JsonConstructor]
    public StudySession()
    {

    }

    public StudySession(Dictionary<string, UserStudyData> sessionStudyData, float wordsCorrect, double timeSpentStudying)
    {
        ID = Guid.NewGuid();
        this.SessionStudyData = sessionStudyData;
        NumberOfWords = sessionStudyData.Count;
        CompletedDate = DateTime.UtcNow;
        TimeToComplete = timeSpentStudying;
        AverageSpeed = TimeToComplete / NumberOfWords;
        PercentCorrect = (wordsCorrect / (float)NumberOfWords) * 100f;
    }

    public Dictionary<string, string> GenerateRecordReport()
    {
        Dictionary<string, string> report = new Dictionary<string, string>();
        report.Add("Average Answer Speed", CalculateAverageFlightTime().ToString() + "s");
        report.Add("Number of words known", CalculateNumberOfKnownWords().ToString());
        return report;
    }

    private double CalculateAverageFlightTime()
    {
        double averageFlightTime = 0;
        SessionStudyData.Keys.ToList().ForEach(key =>
        {
            averageFlightTime += SessionStudyData[key].TotalTimeOnScreen;
        });
        averageFlightTime = averageFlightTime / SessionStudyData.Count;
        
        return averageFlightTime;
    }

    private int CalculateNumberOfKnownWords()
    {
        int knownCount = 0;
        SessionStudyData.Keys.ToList().ForEach(key =>
        {
            if(SessionStudyData[key].GetCorrectPercentage() > 50f)
            {
                knownCount++;
            }   
        });
        return knownCount;
    }
}


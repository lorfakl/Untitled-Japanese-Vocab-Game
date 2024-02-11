using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.PlayFabHelper.CurrentUser;
using Utilities;
using Newtonsoft.Json;

[Serializable]
public class StudyRecord
{
    [JsonProperty("ID")]
    public Guid ID { get; private set; }

    [JsonProperty("StudySessions")]
    private List<StudySession> Sessions { get; set; }

    [JsonProperty("WordStatistics")]
    private Dictionary<string, StatOverTime<UserStudyData>> WordStatistics { get; set; }

    [JsonProperty("AvgSpeedOverTime")]
    StatOverTime<double> AvgSpeed { get; set; }

    [JsonProperty("FastestSpeedOverTime")]
    StatOverTime<double> FastestSpeed { get; set; }

    [JsonProperty("SlowestSpeedOverTime")]
    StatOverTime<double> SlowestSpeed { get; set; }

    [JsonProperty("CurrentStreak")]
    private int CurrentStreak;

    [JsonProperty("LongestStreak")]
    private int LongestStreak;

    List<JapaneseWord> knownWords { get; set; }
    List<JapaneseWord> studiedWords { get; set; }
    List<JapaneseWord> masteredWords { get; set; }
    List<JapaneseWord> difficultWords { get; set; }
    List<JapaneseWord> recognizedWords { get; set; }

    public StudyRecord()
    {
        ID = Guid.NewGuid();
        Sessions = new List<StudySession>();
        WordStatistics = new Dictionary<string, StatOverTime<UserStudyData>>();
        AvgSpeed = new StatOverTime<double>();
        FastestSpeed = new StatOverTime<double>();
        SlowestSpeed = new StatOverTime<double>();

    }

    public UserStudyData TryGetWordData(string id)
    {
        if(WordStatistics.TryGetValue(id, out var stat))
        {
            return stat.MostRecent();
        }
        else
        {
            return default(UserStudyData);
        }
    }

    public double GetAverageAnswerSpeedForWord(string id)
    {
        if(WordStatistics.TryGetValue(id, out var statOverTime))
        {
            double total = 0.0;
            foreach (var stat in statOverTime)
            {
                total += stat.Value.AnswerSpeed;
            }
            return total / statOverTime.Count;
        }
        else
        {
            return -1;
        }

    }

    public double GetCurrentOverallAverage()
    {
        double totalAvg = 0.0;
        foreach (var avgStat in AvgSpeed)
        {
            totalAvg += avgStat.Value;
        }
        return totalAvg/AvgSpeed.Count;
    }

    public int GetCurrentStreak()
    {
        return CurrentStreak;
    }

    public int GetCurrentWordsKnown()
    {
        int knownWords = 0;
        WordStatistics.Keys.ToList().ForEach(key => 
        {
            if(WordStatistics[key].MostRecent().GetCorrectPercentage() > 50f)
            {
                knownWords++;
            }
        });
        return knownWords;
    }

    public int GetLongestStreak()
    {
        return LongestStreak;
    }

    public void AddSession(StudySession studySession)
    {
        double fastestAnswer = 5;
        double slowestAnswer = 3;

        
        studySession.SessionStudyData.Keys.ToList().ForEach((key) => 
        { 
            UserStudyData data = studySession.SessionStudyData[key];
            if(data.AnswerSpeed < fastestAnswer && data.AnswerSpeed > 0) 
            { 
                fastestAnswer = data.AnswerSpeed;
            }
            else if(data.AnswerSpeed > slowestAnswer)
            {
                slowestAnswer = data.AnswerSpeed;
            }
            if(WordStatistics.ContainsKey(key))
            {
                WordStatistics[key].AddStat(studySession.SessionStudyData[key]);
            }
            else
            {
                StatOverTime<UserStudyData> studyDataInstance = new StatOverTime<UserStudyData>();
                studyDataInstance.AddStat(studySession.SessionStudyData[key]);
                WordStatistics.Add(key, studyDataInstance);
            }
        });

        AvgSpeed.AddStat(studySession.AverageSpeed);
        FastestSpeed.AddStat(fastestAnswer);
        SlowestSpeed.AddStat(slowestAnswer);
        var lastSession = GetMostRecentSession();
        if(lastSession != null) 
        {
            if (studySession.CompletedDate - lastSession.CompletedDate < TimeSpan.FromHours(24))
            {
                CurrentStreak++;
                if (CurrentStreak >= LongestStreak)
                {
                    LongestStreak = CurrentStreak;
                }
            }
        }
        else
        {
            CurrentStreak = 1;
        }
        
        Sessions.Add(studySession);
    }

    public StudySession GetMostRecentSession()
    {
        TimeSpan smallestTimeSpan = TimeSpan.Zero;
        DateTime now = DateTime.UtcNow;
        StudySession sessionToReturn = null;
        foreach (var session in Sessions)
        {
            if(smallestTimeSpan == TimeSpan.Zero)
            {
                smallestTimeSpan = now - session.CompletedDate;
                sessionToReturn = session;
            }
            else
            {
                var possibleSmallest = now - session.CompletedDate;
                if (possibleSmallest < smallestTimeSpan) 
                { 
                    smallestTimeSpan = possibleSmallest;
                    sessionToReturn = session;
                }
            }
        }

        return sessionToReturn;
    }

    public List<JapaneseWord> GetCurrentStudiedWords()
    {
        if(studiedWords != null)
        { 
            return studiedWords; 
        }
        else
        {
            studiedWords = new List<JapaneseWord>();
            foreach(var statOverTime in WordStatistics.Values.ToList())
            {
                var statInstance = statOverTime.MostRecent();
                if (statInstance.TimesSeen > 0) 
                {
                    studiedWords.Add(statInstance.Word);
                }
            }
            return studiedWords;
        }
    }

    public List<JapaneseWord> GetCurrentMasteredWords()
    {
        if (masteredWords != null)
        {
            return masteredWords;
        }
        else
        {
            masteredWords = new List<JapaneseWord>();
            foreach (var statOverTime in WordStatistics.Values.ToList())
            {
                var statInstance = statOverTime.MostRecent();
                float correctPercentage = statInstance.GetCorrectPercentage();
                if (correctPercentage >= 95)
                {
                    masteredWords.Add(statInstance.Word);
                }
            }
            return masteredWords;
        }
    }

    public List<JapaneseWord> GetCurrentDifficultWords()
    {
        if (difficultWords != null)
        {
            return difficultWords;
        }
        else
        {
            difficultWords = new List<JapaneseWord>();
            foreach (var statOverTime in WordStatistics.Values.ToList())
            {
                var statInstance = statOverTime.MostRecent();
                float correctPercentage = statInstance.GetCorrectPercentage();
                if (correctPercentage < 10f)
                {
                    difficultWords.Add(statInstance.Word);
                }
            }
            return difficultWords;
        }
    }

    public List<JapaneseWord> GetCurrentRecognizedWords()
    {
        if (recognizedWords != null)
        {
            return recognizedWords;
        }
        else
        {
            recognizedWords = new List<JapaneseWord>();
            foreach (var statOverTime in WordStatistics.Values.ToList())
            {
                var statInstance = statOverTime.MostRecent();
                float correctPercentage = statInstance.GetCorrectPercentage();
                if (correctPercentage < 30f)
                {
                    recognizedWords.Add(statInstance.Word);
                }
            }
            return recognizedWords;
        }
    }

    public List<JapaneseWord> GetCurrentKnownWords()
    {
        if (knownWords != null)
        {
            return knownWords;
        }
        else
        {
            knownWords = new List<JapaneseWord>();
            foreach (var statOverTime in WordStatistics.Values.ToList())
            {
                var statInstance = statOverTime.MostRecent();
                float correctPercentage = statInstance.GetCorrectPercentage();
                if (correctPercentage >= 50f)
                {
                    statInstance.Word.TimesSeen = statInstance.TimesSeen;
                    //stats.Word.
                    knownWords.Add(statInstance.Word);
                }
            }
            return knownWords;
        }
    }

    public void InitializeWordIDs()
    {
        this.WordStatistics.Keys.ToList().ForEach((key) => 
        {
            var wordDataOverTime = WordStatistics[key];
            foreach(var wordDataInstance in wordDataOverTime)
            {
                var word = wordDataInstance.Value.Word;
                if(String.IsNullOrEmpty(word.ID))
                {
                    word.SetID(key);
                }
                else
                {
                    HelperFunctions.Log($"Current word ID: {word.ID} current key:{key}");
                }
            }
            BuildKnownledgeList(wordDataOverTime);

        });
    }

    private void BuildKnownledgeList(StatOverTime<UserStudyData> studyDataOverTime)
    {
        var wordToAdd = studyDataOverTime.MostRecent().Word;
        if (studiedWords == null)
        {
            studiedWords = new List<JapaneseWord>();
        }

        if (!studiedWords.Exists(word => word.ID == wordToAdd.ID))
        {
            studiedWords.Add(wordToAdd);
        }

        float totalSeen = 0f;
        float totalCorrect = 0f;
        float correctPercentage = 0f;

        foreach(var instance in studyDataOverTime)
        {
            totalSeen += instance.Value.TimesSeen;
            totalCorrect += instance.Value.TimesCorrect;
        }

        correctPercentage = (totalCorrect/ totalSeen) * 100;
        
        switch (correctPercentage) 
        {
            case > 98f:
                if (masteredWords == null)
                {
                    masteredWords = new List<JapaneseWord>();
                    masteredWords.Add(wordToAdd);
                    break;
                }

                if (!masteredWords.Exists(word => word.ID == wordToAdd.ID))
                {
                    masteredWords.Add(wordToAdd);
                }
                break;
            case > 69f:
                if (knownWords == null)
                {
                    knownWords = new List<JapaneseWord>();
                    knownWords.Add(wordToAdd);
                    break;
                }

                if(!knownWords.Exists(word => word.ID == wordToAdd.ID))
                {
                    knownWords.Add(wordToAdd);
                }
                break;
            case >= 39f:
                if (recognizedWords == null)
                {
                    recognizedWords = new List<JapaneseWord>();
                    recognizedWords.Add(wordToAdd);
                    break;
                }

                if (!recognizedWords.Exists(word => word.ID == wordToAdd.ID))
                {
                    recognizedWords.Add(wordToAdd);
                }
                break;
            case < 39f:
                if (difficultWords == null)
                {
                    difficultWords = new List<JapaneseWord>();
                    difficultWords.Add(wordToAdd);
                    break;
                }

                if (!difficultWords.Exists(word => word.ID == wordToAdd.ID))
                {
                    difficultWords.Add(wordToAdd);
                }
                break;

        }

    }
}

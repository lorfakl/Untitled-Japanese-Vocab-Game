using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class UserStudyData
{
    [JsonProperty("Word")]
    public JapaneseWord Word;

    [JsonProperty("TimesSeen")]
    public int TimesSeen;

    [JsonProperty("TimesCorrect")]
    public int TimesCorrect;

    [JsonProperty("TotalTimeOnScreen")]
    public double TotalTimeOnScreen;

    [JsonProperty("IsFavorite")]
    public bool IsFavorite;

    [JsonProperty("AnswerSpeed")]
    public double AnswerSpeed;

    /*
    [JsonProperty("SpeedOverTime")]
    public StatOverTime SpeedOverTime;

    [JsonProperty("CorrectPercentageOverTime")]
    public StatOverTime CorrectnessOverTime;*/


    [JsonConstructor]
    public UserStudyData()
    { }

    public UserStudyData(JapaneseWord s)
    {
        Word = new JapaneseWord(s);
        TimesSeen = 0;
        TimesCorrect = 0;
        TotalTimeOnScreen = 0;
        IsFavorite = false;
        AnswerSpeed = 0;
    }

    public UserStudyData(UserStudyData u)
    {
        this.Word = u.Word;
        this.TimesSeen = u.TimesSeen;
        this.TimesCorrect = u.TimesCorrect;
        this.TotalTimeOnScreen = u.TotalTimeOnScreen;
    }

    public float GetCorrectPercentage()
    {
        return ((float)TimesCorrect/(float)TimesSeen)*100;
    }
}



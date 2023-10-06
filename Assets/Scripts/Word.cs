using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Word
{
    public static int maxLeitnerLevel = 6;
    public static int maxPrestigeLevel = 8;
    
    [JsonProperty]
    public string ID
    {
        get;
        protected set;
    }

    [JsonProperty]
    public int LeitnerLevel
    {
        get;
        set;
    }

    [JsonProperty]
    public int PreviousLeitnerLevel
    {
        get;
        set;
    }

    [JsonProperty]
    public int PrestigeLevel
    {
        get;
        set;
    }

    [JsonProperty]
    public int PreviousPrestigeLevel
    {
        get;
        set;
    }

    [JsonProperty]
    public int TimesSeen
    {
        get;
        set;
    }

    [JsonProperty]
    public int TimesAnsweredCorrectly
    {
        get;
        set;
    }

    [JsonProperty]
    public float TotalTimeToAnswer
    {
        get;
        set;
    }

    [JsonProperty]
    public float AverageTime
    {
        get;
        set;
    }

    [JsonProperty]
    public float LongestTime
    {
        get;
        set;
    }
    [JsonProperty]
    public float ShortestTime
    {
        get;
        set;
    }

    public void ModifyProficiencyLevel(bool wasCorrect)
    {
        if(wasCorrect)
        {
            if(LeitnerLevel == maxLeitnerLevel)
            {
                PreviousPrestigeLevel = PrestigeLevel;
                PrestigeLevel++;
            }
            else
            {
                PreviousLeitnerLevel = LeitnerLevel;
                LeitnerLevel++;
            }
        }
        else
        {
            if (LeitnerLevel == maxLeitnerLevel)
            {
                PreviousPrestigeLevel = PrestigeLevel;
                PrestigeLevel--;
            }
            else
            {
                PreviousLeitnerLevel = LeitnerLevel;
                LeitnerLevel = 0;
            }
        }
    }

}

[Serializable]
public class JapaneseWord : Word
{
    [JsonProperty]
    public string Kanji { get; set; }
    [JsonProperty]
    public string Kana { get; set; }
    [JsonProperty]
    public string English { get; set; }

    public JapaneseWord()
    {

    }

    public JapaneseWord(JapaneseWord word)
    {
        Kanji = word.Kanji;
        Kana = word.Kana;
        English = word.English;
    }

    public void SetID(string id)
    {
        this.ID = id;
    }

    public string PrintAnswer()
    {
        if(Kanji == "-1")
        {
            return "Kana: " + Kana +
            "\n" + "Meaning: " + English;
        }
        else
        {
            return "Kanji: " + Kanji + "\n" + "Kana: " + Kana +
            "\n" + "Meaning: " + English;
        }
        
    }
    public override string ToString()
    {
        return "Kanji: " + Kanji + "\n" + "Kana: " + Kana +
            "\n" + "English: " + English +
            "\n" + "Leitner Level: " + LeitnerLevel +
            "\n" + "Prestige Level: " + PrestigeLevel;
    }
}
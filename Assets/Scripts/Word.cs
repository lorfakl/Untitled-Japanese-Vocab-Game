using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Word
{
    public static int maxLeitnerLevel = 6;
    public static int maxPrestigeLevel = 8;
    public string ID
    {
        get;
        protected set;
    }

    public int LeitnerLevel
    {
        get;
        set;
    }

    public int PreviousLeitnerLevel
    {
        get;
        set;
    }

    public int PrestigeLevel
    {
        get;
        set;
    }

    public int PreviousPrestigeLevel
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
    #region Private Variables
    #endregion

    #region Events
    #endregion

    #region Unity Events
    #endregion

    #region Public Methods
    #endregion

    #region Unity Methods
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
#endregion

    #region Private Methods
#endregion
}

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

    public string PrintAnswer()
    {
        return "Kanji: " + Kanji + "\n" + "Kana: " + Kana +
            "\n" + "English: " + English;
    }
    public override string ToString()
    {
        return "Kanji: " + Kanji + "\n" + "Kana: " + Kana +
            "\n" + "English: " + English +
            "\n" + "Leitner Level: " + LeitnerLevel +
            "\n" + "Prestige Level: " + PrestigeLevel;
    }
}
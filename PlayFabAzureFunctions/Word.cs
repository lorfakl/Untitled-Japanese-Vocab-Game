using System.Collections;
using System.Collections.Generic;


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

    public int PrestigeLevel
    {
        get;
        set;
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
    public string Kanji { get; set; }
    public string Kana { get; set; }
    public string English { get; set; }

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
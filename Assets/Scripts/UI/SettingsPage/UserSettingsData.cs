using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Utilities;
using Utilities.PlayFabHelper;

public enum TranslateDirection
{
    
    English2Kana = 14,
    English2Kanji = 12,
    Kana2Kanji = 10,
    Kana2English = 7,
    Kanji2English = 6,
    Kana2Kana = 3,
    Kanji2Kana = 5

}

public enum MaxWordsPerSession
{
    Thirty = 30,
    Forty = 40,
    Fifty = 50
}

public enum StudyTopic
{
    Kanji = 0,
    Kana = 1
}

public enum NewWordsPerSession
{
    
    Five = 5,
    Ten = 10,
    Fifteen = 15
}

public enum AudioTranslateDirection
{
    Japanese2English = 0,
    English2Japanese = 1,
    None = 3
}

[Serializable]
public class UserSettingsData
{
    string _dataOwner;
    
    bool _isAudioStudy = false;

    TranslateDirection _translateDirection;
    MaxWordsPerSession maxWords;
    StudyTopic studyTopic;
    NewWordsPerSession newWords;
    AudioTranslateDirection audioTranslateDirection;

    public string OwnerID
    {
        get { return _dataOwner; }
        set { _dataOwner = value; }
    }

    public bool IsAudioStudy
    {
        get { return _isAudioStudy; }
        set { _isAudioStudy = value; }
    }

    public TranslateDirection TranslationDirection
    {
        get { return _translateDirection; }
        set { _translateDirection = value; }
    }

    public StudyTopic StudyTopic
    {
        get { return studyTopic; }
        set { studyTopic = value; }
    }

    public NewWordsPerSession MaxNewWords
    {
        get { return newWords; }
        set { newWords = value; }
    }

    public MaxWordsPerSession TotalWordsPerSession
    {
        get { return maxWords; }
        set { maxWords = value; }
    }

    public UserSettingsData()
    {

    }

    public UserSettingsData(TranslateDirection TranslateDirection, MaxWordsPerSession maxWords, StudyTopic studyTopic, NewWordsPerSession newWords)
    {

        this._translateDirection = TranslateDirection;
        this.maxWords = maxWords;
        this.studyTopic = studyTopic;
        this.newWords = newWords;
        this.audioTranslateDirection = AudioTranslateDirection.None;
    }

    public UserSettingsData(UserSettingsData otherData)
    {
        _dataOwner = otherData._dataOwner;
        _isAudioStudy = otherData._isAudioStudy;
        this._translateDirection = otherData._translateDirection;
        this.maxWords = otherData.maxWords;
        this.studyTopic = otherData.studyTopic;
        this.newWords = otherData.newWords;
        this.audioTranslateDirection = otherData.audioTranslateDirection;
    }

    public static UserSettingsData CreateDefaultSettings()
    {
        var settings = new UserSettingsData
        {
            IsAudioStudy = false,
            MaxNewWords = NewWordsPerSession.Ten,
            OwnerID = Playfab.PlayFabID,
            StudyTopic = StudyTopic.Kanji,
            TranslationDirection = TranslateDirection.Kanji2Kana,
            TotalWordsPerSession = MaxWordsPerSession.Thirty
        };

        return settings;
    }

    public override string ToString()
    {
        string data = $"Study Topic: {studyTopic} \n" +
            $"Audio Study: {_isAudioStudy} \n" +
            $"Visual Translation Direction: {_translateDirection} \n" +
            $"Max total words per session: {maxWords} \n" +
            $"Max New Words per session: {newWords} \n";
        return data;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Utilities.PlayFabHelper.CurrentUser;
using Utilities.SaveOperations;

public static class StaticUserSettings
{
    static UserSettingsData userSettingsData;

    public static UserSettingsData Settings 
    { get 
        { LoadSettings(); 
          return userSettingsData; 
        } 
    }

    public static bool IsKanjiStudyTopic()
    {
        LoadSettings();
        if(userSettingsData.StudyTopic == StudyTopic.Kanji)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static TranslateDirection GetTranslationDirection()
    {
        LoadSettings();
        return userSettingsData.TranslationDirection;
    }

    public static int GetNewWords()
    {
        LoadSettings();
        return (int)userSettingsData.MaxNewWords;
    }

    public static int GetTotalWords()
    {
        LoadSettings();
        return (int)userSettingsData.TotalWordsPerSession;
    }

    public static float GetMusicVolume()
    {
        LoadSettings();
        float volumeLvl = userSettingsData.AudioVolume / 100.0f;
        return volumeLvl;
    }

    public static void UpdateSettings(UserSettingsData userSettingsData)
    {
        CurrentAuthedPlayer.UserSettings = userSettingsData;
    }

    static void LoadSettings()
    {
        
        if (CurrentAuthedPlayer.UserSettings == null)
        {
            var settings = SaveSystem.Load<UserSettingsData>(DataCategory.Settings);
            if (settings != default)
            {
                userSettingsData = settings;
            }
            else
            {
                userSettingsData = UserSettingsData.CreateDefaultSettings();
                CurrentAuthedPlayer.UserSettings = userSettingsData;
            }
        }
        else
        {
            userSettingsData = CurrentAuthedPlayer.UserSettings;
        }

        HelperFunctions.Log("Loaded Settings: " + userSettingsData);
        
    }
}

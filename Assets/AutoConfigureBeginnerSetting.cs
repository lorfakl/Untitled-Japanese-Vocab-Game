using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Utilities.Events;
using Utilities.PlayFabHelper;

public class AutoConfigureBeginnerSetting : MonoBehaviour
{
    // Start is called before the first frame update
    bool readyToInfoUser = false;
    bool changedUserDefaults = false;
    static bool hasUserAnsweredQuestion = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!hasUserAnsweredQuestion)
        {
            ShouldSettingsAutoConfigure();
        }

        if(changedUserDefaults && readyToInfoUser) 
        {
            MessageBoxFactory.CreateMessageBox("Settings Updated", "Study settings updated for Kana", null, true);
            readyToInfoUser = false;
        }
    }

    private void ShouldSettingsAutoConfigure()
    {
        if(Playfab.WasUserJustCreated && ExplanationManager.IsExplanationCompete)
        {
            hasUserAnsweredQuestion = true;
            MessageBoxFactory.CreateQuestionBox("Welcome to Yokai Chat!", "Now tell me. Do you know all 142 Kana? Both Hiragana and Katakana?",
                () => { hasUserAnsweredQuestion = true; }, AutoConfigureKanaSettings);

        }
    }

    private void AutoConfigureKanaSettings()
    {
        hasUserAnsweredQuestion=true;
        var kanaSettings = UserSettingsData.CreateDefaultSettings();
        kanaSettings.StudyTopic = StudyTopic.Kana;
        kanaSettings.TranslationDirection = TranslateDirection.Kana2English;
        StaticUserSettings.UpdateSettings(kanaSettings);
        readyToInfoUser = true;
        changedUserDefaults = true;
    }

    
}

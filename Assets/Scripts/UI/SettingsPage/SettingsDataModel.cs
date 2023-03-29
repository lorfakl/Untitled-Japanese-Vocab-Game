using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class SettingsDataModel : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    ToggleSlider _studyTopicSlider;

    [SerializeField]
    ArrowSelector _kanaAndEnglish;

    [SerializeField]
    ArrowSelector _kanaAndKanji;

    [SerializeField]
    ArrowSelector _kanaAndKana;

    [SerializeField]
    ArrowSelector _kanjiAndEnglish;

    [SerializeField]
    ArrowSelector _audioTranslationDirection;

    [SerializeField]
    Slider _musicVolume;
    
    [SerializeField]
    ToggleSlider _totalWordsPerSession;

    [SerializeField]
    ToggleSlider _newWordsPerSession;




    void Start()
    {
        _studyTopicSlider.OnValueSelectedChanged.AddListener(UpdateStudyTopic);
        _kanaAndEnglish.OnSelectionChange.AddListener(UpdateTranslationDirection);
        _kanaAndKanji.OnSelectionChange.AddListener(UpdateTranslationDirection);
        _kanaAndKana.OnSelectionChange.AddListener(UpdateTranslationDirection);
        _kanjiAndEnglish.OnSelectionChange.AddListener(UpdateTranslationDirection);
        //_audioTranslationDirection.OnSelectionChange.AddListener(UpdateAudioTranslationDirection);
        _totalWordsPerSession.OnValueSelectedChanged.AddListener(UpdateTotalWords);
        _newWordsPerSession.OnValueSelectedChanged.AddListener(UpdateMaxNewWords);
        _musicVolume.onValueChanged.AddListener(UpdateMusicVolume);
    }

    void UpdateTranslationDirection(string t)
    {   //this overwrites any previously selected direction because only ONE can actually be use at a time
        SettingsPageManager.ModifiedSettings.TranslationDirection = HelperFunctions.ParseEnum<TranslateDirection>(t);
    }

    void UpdateStudyTopic(string t)
    {
        SettingsPageManager.ModifiedSettings.StudyTopic = HelperFunctions.ParseEnum<StudyTopic>(t);
    }

    void UpdateTotalWords(string w)
    {
        SettingsPageManager.ModifiedSettings.TotalWordsPerSession = HelperFunctions.ParseEnum<MaxWordsPerSession>(w);
    }

    void UpdateMaxNewWords(string n)
    {
        SettingsPageManager.ModifiedSettings.MaxNewWords = HelperFunctions.ParseEnum<NewWordsPerSession>(n);
    }

    void UpdateMusicVolume(float value)
    {
        SettingsPageManager.ModifiedSettings.AudioVolume = value;
    }

    void UpdateAudioTranslationDirection(string d)
    {

    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using DG.Tweening;

public class SettingsDisplayController : MonoBehaviour
{
    [SerializeField]
    Transform _scrollContent;

    [SerializeField]
    ToggleSlider _studyTopicSlider;

    [SerializeField]
    Transform _visualUIGroup;

    [SerializeField]
    ArrowSelector _kanaAndEnglish;

    [SerializeField]
    ArrowSelector _kanaAndKanji;

    [SerializeField]
    ArrowSelector _kanaAndKana;

    [SerializeField]
    ArrowSelector _kanjiAndEnglish;

    [SerializeField]
    Button _swapBtn;

    [SerializeField]
    Transform _audioUIGroup;

    [SerializeField]
    ArrowSelector _audioTranslationDirection;

    [SerializeField]
    GameObject _wordSettingsUIGroup;

    [SerializeField]
    ToggleSlider _totalWordsPerSession;

    [SerializeField]
    ToggleSlider _newWordsPerSession;

    [SerializeField]
    VerticalLayoutGroup _visualUILayoutGroup;

    [SerializeField]
    float _tweenTime;

    bool _isUIDriven = false;
    List<ArrowSelector> _swappableSelectors = new List<ArrowSelector>();
    Vector3 _topArrowSelectLocalPosition;
    ArrowSelector _topSelector;
    ArrowSelector _otherSelector;

    (ArrowSelector first, ArrowSelector second) activeSelectors;
    // Start is called before the first frame update
    void Start()
    {
        _audioUIGroup.gameObject.SetActive(false);
        _topArrowSelectLocalPosition = _kanaAndEnglish.GetComponent<RectTransform>().localPosition;
        if (SettingsPageManager.UseDefaultSettings)
        {
            InitializeUserSettings();
            _isUIDriven = true;
        }
        else
        {
            HelperFunctions.Log(SettingsPageManager.CurrentSettings);
            DriveSettingsDisplay(SettingsPageManager.CurrentSettings);
        }
        _swapBtn.onClick.AddListener(SwapSelectors);
        HelperFunctions.Log("Position of Top control " + _topArrowSelectLocalPosition);
        _studyTopicSlider.OnValueSelectedChanged.AddListener(UpdateViewOnTopicChange);

    }

    private void Update()
    {
        if(!_isUIDriven) //this has to be done in Update for some forgotten reason. I believe it was related to Tweening
        {
            _isUIDriven = true;
            DriveSettingsDisplay(SettingsPageManager.CurrentSettings);
        }
    }
    private void InitializeUserSettings()
    {
        _kanaAndEnglish.gameObject.SetActive(false);
        _kanaAndKana.gameObject.SetActive(false);
        SetUpSelectorTuple(_kanjiAndEnglish, _kanaAndKanji);
        activeSelectors.second.gameObject.SetActive(false);
    }

    private void DriveSettingsDisplay(UserSettingsData currentSettings)
    {
        _studyTopicSlider.DriveSlider(currentSettings.StudyTopic.ToString());
        if(currentSettings.StudyTopic == StudyTopic.Kanji)
        {
            _kanaAndEnglish.gameObject.SetActive(false);
            _kanaAndKana.gameObject.SetActive(false);
            SetUpSelectorTuple(_kanjiAndEnglish, _kanaAndKanji);
        }
        else
        {
            _kanaAndKanji.gameObject.SetActive(false);
            _kanjiAndEnglish.gameObject.SetActive(false);
            SetUpSelectorTuple(_kanaAndEnglish, _kanaAndKana);
        }

        DriveToggleSliders(currentSettings);
        DriveArrowSelectors(currentSettings);
        ConfigureSelectors(currentSettings);
    }

    void SetUpSelectorTuple(ArrowSelector a, ArrowSelector b)
    {
        activeSelectors.first = a;
        activeSelectors.second = b;
        _swappableSelectors.Add(a);
        _swappableSelectors.Add(b);
    }

    void ConfigureSelectors(UserSettingsData s)
    {
        if(activeSelectors.first.TranslationDirection == s.TranslationDirection)
        {
            activeSelectors.second.gameObject.SetActive(false);
        }
        else if(activeSelectors.second.TranslationDirection == s.TranslationDirection)
        {
            activeSelectors.first.gameObject.SetActive(false);
        }
        else
        {
            HelperFunctions.Error("YUUUUUGE issue here. One of them should be rqual to the user settings");
        }
    }

    private void DriveToggleSliders(UserSettingsData s)
    {
        int max = (int)s.MaxNewWords;
        int total = (int)s.TotalWordsPerSession;
        _newWordsPerSession.DriveSlider(max.ToString());
        _totalWordsPerSession.DriveSlider(total.ToString());   
    }
    
    private void DriveArrowSelectors(UserSettingsData s)
    {
        foreach(var sec in _swappableSelectors)
        {
            sec.DriveSelector(s.TranslationDirection);
        }
    }

    private void SwapSelectors()
    {
        if(activeSelectors.first.gameObject.activeSelf)
        {
            activeSelectors.first.gameObject.SetActive(false);
            activeSelectors.second.gameObject.SetActive(true);
        }
        else
        {
            activeSelectors.first.gameObject.SetActive(true);
            activeSelectors.second.gameObject.SetActive(false);
        }
    }

    private void UpdateViewOnTopicChange(string topic)
    {
        if (SettingsPageManager.ModifiedSettings.StudyTopic == StudyTopic.Kanji)
        {
            _kanaAndEnglish.gameObject.SetActive(false);
            _kanaAndKana.gameObject.SetActive(false);
            SetUpSelectorTuple(_kanjiAndEnglish, _kanaAndKanji);
            _kanaAndKanji.gameObject.SetActive(true);
            _kanjiAndEnglish.gameObject.SetActive(false);
            SettingsPageManager.ModifiedSettings.TranslationDirection = TranslateDirection.Kanji2Kana;
        }
        else
        {
            _kanaAndKanji.gameObject.SetActive(false);
            _kanjiAndEnglish.gameObject.SetActive(false);
            _kanaAndKana.gameObject.SetActive(false);
            SetUpSelectorTuple(_kanaAndEnglish, _kanaAndKana);
            _kanaAndEnglish.gameObject.SetActive(true); 
            SettingsPageManager.ModifiedSettings.TranslationDirection = TranslateDirection.Kana2English;
        }

        //ConfigureSelectors(SettingsPageManager.ModifiedSettings);
    }
}

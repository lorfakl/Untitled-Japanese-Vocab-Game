using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Events;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System;

public class WordDetailsController : MonoBehaviour
{
    [SerializeField]
    GameEvent wordDetailsClosedEvent;

    [SerializeField]
    Button closeButton;

    [SerializeField]
    Button createGoalButton;

    [SerializeField]
    TMP_Text title;

    [SerializeField]
    TMP_Text seenValue;

    [SerializeField]
    TMP_Text correctValue;

    [SerializeField]
    TMP_Text speedValue;

    [SerializeField]
    TMP_Text kana;

    [SerializeField]
    TMP_Text meaing;

    [SerializeField]
    TMP_Text fastestSpeedValue;

    [SerializeField]
    TMP_Text medianSpeedValue;

    [SerializeField]
    float tweenTime;

    public static Queue<UserStudyData> Data = new Queue<UserStudyData>();

    private UserStudyData _data;
    private void Awake()
    {
        closeButton.onClick.AddListener(() => { wordDetailsClosedEvent.Raise(); });
        createGoalButton.onClick.AddListener(OpenGoalSubMenu);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    private void OpenGoalSubMenu()
    {

    }

    private void DisplayData()
    {
        title.text = (_data.Word.Kanji == "-1") ? _data.Word.Kana : _data.Word.Kanji;
        seenValue.text = $"Times Seen: {_data.TimesSeen}";
        correctValue.text = $"Percent Correct: {_data.GetCorrectPercentage()}";
        speedValue.text = $"Average Answer Speed: {_data.AnswerSpeed}";
        kana.text = $"Kana: {_data.Word.Kana}";
        meaing.text = $"Defintion: {_data.Word.English}";
        //fastestSpeedValue.text = $"Highest Answer Speed: {_data.}";
        medianSpeedValue.text = "Requires API call figure out where it should table place";
    }

    private void OnEnable()
    {
        _data = Data.Dequeue();
        DisplayData();
        transform.DOScale(1f, tweenTime);

    }

    private void OnDisable() 
    {
        transform.DOScale(0.01f, tweenTime);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Utilities.Events;
using Utilities.UserInterfaceAddOns;
using ProjectSpecificGlobals;
using System;


public delegate void GlossaryEntrySelectedEventHandler(JapaneseWord d);
public class GlossaryEntry : MonoBehaviour
{
    [SerializeField]
    TMP_Text wordText;

    [SerializeField]
    TMP_Text seenText;

    [SerializeField]
    TMP_Text correctnessText;

    [SerializeField]
    TMP_Text speedText;

    [SerializeField]
    Button entryButton;

    [SerializeField]
    GameEvent entrySelectedEvent;

    [SerializeField]
    GameEvent entryLongPressEvent;


    public static Queue<JapaneseWord> WordsToDisplay = new Queue<JapaneseWord>(250);
    public event GlossaryEntrySelectedEventHandler entrySelected;
    private JapaneseWord deepCopyToSend;
    private JapaneseWord data;

    public JapaneseWord Data
    {
        get { return data; }
    }

    public void UpdateDisplay(JapaneseWord w)
    {
        data = w;
        DisplayData();
    }

    // Start is called before the first frame update
    private void Awake()
    {
        
        LongPress longPressInstance = entryButton.GetComponent<LongPress>();
        if(longPressInstance != null) 
        {
            //longPressInstance.OnLongPressRegistered.AddListener(LongPressEventHandler);
            //longPressInstance.OnShortPressRegistered.AddListener(EntrySelected);
            entryButton.onClick.AddListener(() => { EntrySelected(); });
        }
        else
        {
            entryButton.onClick.AddListener(EntrySelected);
        }
    }

    void Start()
    {
        data = WordsToDisplay.Dequeue();
        DisplayData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LongPressEventHandler() 
    {
        entryLongPressEvent.Raise(data);
    }

    private void EntrySelected()
    {
        entrySelectedEvent.Raise(deepCopyToSend);

    }

    private void DisplayData()
    {
        deepCopyToSend = new JapaneseWord(data);
        if(data.Kanji == "-1")
        {
            wordText.text = data.Kana;
        }
        else
        {
            wordText.text = data.Kanji;
        }
        if(Globals.UserDataLoaded) 
        { 
            var statInstance = Globals.LoadedStudyRecord.TryGetWordData(data.ID);
            if(statInstance != null)
            {
                seenText.text = $"{statInstance.TimesSeen}";
                float correctPercentage = statInstance.GetCorrectPercentage();
                correctnessText.text = $"{correctPercentage}%";
                deepCopyToSend.TimesSeen = statInstance.TimesSeen;
                deepCopyToSend.CorrectPercent = correctPercentage;
                Data.CorrectPercent = correctPercentage;
                Data.TimesSeen = statInstance.TimesSeen;
                double avgSpeed = Globals.LoadedStudyRecord.GetAverageAnswerSpeedForWord(data.ID);
                if(avgSpeed > 0) 
                {
                    speedText.text = $"{avgSpeed:F2}s";
                    deepCopyToSend.AverageTime = (float)avgSpeed;
                    Data.AverageTime = (float)avgSpeed;
                }
                else
                {
                    speedText.text = $"{statInstance.AnswerSpeed:F2}s";
                    deepCopyToSend.AverageTime = (float)statInstance.AnswerSpeed;
                    Data.AverageTime = (float)statInstance.AnswerSpeed;
                }
                
            }
        }
        else
        {
            seenText.text = data.TimesSeen.ToString();
            deepCopyToSend.TimesSeen = data.TimesSeen;
            
            correctnessText.text = $"{data.CorrectPercentage()}%";
            deepCopyToSend.CorrectPercent = data.CorrectPercentage();

            speedText.text = $"{data.AverageTime}s";
            deepCopyToSend.AverageTime = data.AverageTime;
        }
        
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Utilities.Events;
using Utilities.UserInterfaceAddOns;

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
            longPressInstance.OnLongPressRegistered.AddListener(LongPressEventHandler);
            longPressInstance.OnShortPressRegistered.AddListener(EntrySelected);
            entryButton.onClick.AddListener(() => { });
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
        entrySelectedEvent.Raise(data);

    }

    private void DisplayData()
    {
        if(data.Kanji == "-1")
        {
            wordText.text = data.Kana;
        }
        else
        {
            wordText.text = data.Kanji;
        }
        seenText.text = data.TimesSeen.ToString();
        correctnessText.text = data.CorrectPercentage().ToString();
        speedText.text = data.AverageTime.ToString();
    }


}

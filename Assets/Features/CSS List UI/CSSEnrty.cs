using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Events;
using Utilities.UserInterfaceAddOns;

[RequireComponent(typeof(LongPress))]
public class CSSEnrty : MonoBehaviour
{
    [SerializeField]
    Button cssEntryButton;

    [SerializeField]
    TMP_Text cssName;

    [SerializeField]
    TMP_Text description; 

    [SerializeField]
    TMP_Text wordCount;

    [SerializeField]
    Image usesIcon;

    [SerializeField]
    TMP_Text usesText;

    [SerializeField]
    TMP_Text cssCreatorName;

    [SerializeField]
    GameEvent cssEntryShortPress;

    [SerializeField]
    GameEvent cssEntryLongPress;

    LongPress lpInstance;
    CustomStudySet css;
    
    public static Queue<CustomStudySet> Data = new Queue<CustomStudySet>();

    public CustomStudySet CurrentSet { get; private set; }

    public void UpdateDisplayData(CustomStudySet set)
    {
        cssName.text = set.Name;
        description.text = set.Description;
        wordCount.text = set.WordCount.ToString();
        usesText.text = set.Uses.ToString();
        cssCreatorName.text = set.Creator;
        CurrentSet = set;
    }

    private void Awake()
    {
        lpInstance = GetComponent<LongPress>();
        lpInstance.OnShortPressRegistered.AddListener(() => { cssEntryShortPress.Raise(css); });
        lpInstance.OnLongPressRegistered.AddListener(() => { cssEntryLongPress.Raise(css); });    
    }

    // Start is called before the first frame update
    void Start()
    {
        if(Data.Count > 0) 
        { 
            css = Data.Dequeue();
            DisplayData();
        }

    }

    private void DisplayData()
    {
        cssName.text = css.Name;
        description.text = css.Description;
        wordCount.text = css.WordCount.ToString();
        usesText.text = css.Uses.ToString();
        cssCreatorName.text = css.Creator;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

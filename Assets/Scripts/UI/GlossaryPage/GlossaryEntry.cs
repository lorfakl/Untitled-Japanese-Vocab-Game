using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public struct GlossaryEntryData
{
    public int timesSeen;
    public string correctness;
    public float averageAnswerSpeed;
    public JapaneseWord word;
}

public delegate void GlossaryEntrySelectedEventHandler(GlossaryEntryData d);
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

    public event GlossaryEntrySelectedEventHandler entrySelected;

    private GlossaryEntryData data;

    // Start is called before the first frame update
    private void Awake()
    {
        entryButton.onClick.AddListener(EntrySelected);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void EntrySelected()
    {
        entrySelected.Invoke(data);
    }
}

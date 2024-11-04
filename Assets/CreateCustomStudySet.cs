using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Utilities;
using ProjectSpecificGlobals;
using UnityEditor.IMGUI.Controls;
using System.Linq;

public class CreateCustomStudySet : MonoBehaviour
{
    [SerializeField]
    [Range(0.25f, 1.25f)]
    float tweenTime = 0.75f; 

    [SerializeField]
    Button createNewCSSBtn;

    [SerializeField]
    GameObject glossaryEntryPrefab;

    [SerializeField]
    Image buttonImage;

    [SerializeField]
    TMP_Text defaultCSSText;

    [SerializeField]
    TMP_Text currentWordsInCss;

    [SerializeField]
    TMP_InputField nameInput; 

    [SerializeField] 
    TMP_InputField descriptionInput;
    
    [SerializeField]
    TMP_InputField wordSearchField;

    [SerializeField]
    Sprite downButtonSprite;

    [SerializeField]
    Transform wordSearchSubMenu;

    [SerializeField]
    Transform wordPrefabParent;

    [SerializeField]
    SortButton seenSortButton;

    [SerializeField]
    SortButton correctSortButton;

    [SerializeField]
    SortButton speedSortButton;

    Sprite spriteReference; //stores the sprite not in use 
    
    List<JapaneseWord> wordsInCSS = new List<JapaneseWord>();
    const string wordCountBaseText = "Words in set: ";
    TMP_Dropdown subMenuDropdown; 
    int WordCount { get { return wordsInCSS.Count; } }
    private static Dictionary<int, string> filterOptions = new Dictionary<int, string>();
    private Dictionary<string, List<JapaneseWord>> sortedDict = new Dictionary<string, List<JapaneseWord>>();
    private List<GlossaryEntry> entries = new List<GlossaryEntry>();
    private int childCount = 0;

    //Sequence buttonClickSequence;
    private void Awake()
    {
        subMenuDropdown = gameObject.GetComponentInChildren<TMP_Dropdown>(true);
        subMenuDropdown.onValueChanged.AddListener(FilterDisplay);
        wordSearchField.onValueChanged.AddListener(Search);

        foreach (var opt in subMenuDropdown.options)
        {
            filterOptions.Add(subMenuDropdown.options.IndexOf(opt), opt.text);
        }

        HelperFunctions.Log($"Count in filter options {filterOptions.Count}");
        foreach (var opt in subMenuDropdown.options)
        {
            if (!sortedDict.ContainsKey(opt.text))
            {
                sortedDict.Add(opt.text, new List<JapaneseWord>());
            }
        }

        if (DataPlatform.CheckIfStudyRecordLoaded())
        {
            sortedDict["Known"] = Globals.LoadedStudyRecord.GetCurrentKnownWords();
            sortedDict["Studied"] = Globals.LoadedStudyRecord.GetCurrentStudiedWords();
            sortedDict["Mastered"] = Globals.LoadedStudyRecord.GetCurrentMasteredWords();
            sortedDict["Difficult"] = Globals.LoadedStudyRecord.GetCurrentDifficultWords();
            sortedDict["Recognize"] = Globals.LoadedStudyRecord.GetCurrentRecognizedWords();
        }

        sortedDict["None"] = Globals.AllWords;
        
        //for(int i =0; i < sortedDict["None"])
        //{
        //    GlossaryEntry.WordsToDisplay.Enqueue(w);
        //    var go = GameObject.Instantiate(glossaryEntryPrefab, Vector3.zero, Quaternion.identity, wordPrefabParent);
        //    var entry = go.GetComponent<GlossaryEntry>();
        //}
        

        if (correctSortButton != null)
        {
            seenSortButton.OnSortModeChange += SortBySeen;
            correctSortButton.OnSortModeChange += SortByCorrect;
            speedSortButton.OnSortModeChange += SortBySpeed;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        createNewCSSBtn.onClick.AddListener(ShowCreateMenu);
        currentWordsInCss.text = $"{wordCountBaseText}({WordCount} of {PremiumLimits.WordsPerCustomSetLimit})";
    }

    // Update is called once per frame
    void Update()
    {
        if(wordsInCSS.Count > 0)
        {
            defaultCSSText.enabled = false;
        }
        else
        {
            defaultCSSText.enabled = true;
        }
    }

    private void ShowCreateMenu()
    {
        wordSearchSubMenu.gameObject.SetActive(true);
        OnPressOperations();
        
        
    }

    void OnPressOperations()
    {
        SwapCurrentButtonSprite();
    }

    private void SwapCurrentButtonSprite()
    {
        spriteReference = buttonImage.sprite;
        buttonImage.sprite = downButtonSprite;
    }

    private void AddWordToCSS(JapaneseWord word)
    {
        if(defaultCSSText.enabled)
        {
            defaultCSSText.enabled = false;
        }


        currentWordsInCss.text = $"{wordCountBaseText}({WordCount} of {PremiumLimits.WordsPerCustomSetLimit})";
    }

    private void RemoveWordToCSS(JapaneseWord word)
    {



        if(wordsInCSS.Count == 0)
        {
            defaultCSSText.enabled = true;
        }
        currentWordsInCss.text = $"{wordCountBaseText}({WordCount} of {PremiumLimits.WordsPerCustomSetLimit})";
    }

    private void FilterDisplay(int optionIndex)
    {
        HelperFunctions.Log($"Int Value passed from event: {optionIndex}");
        string filterOption = filterOptions[optionIndex];
        DisplayGlossary(sortedDict[filterOption]);
    }

    private void DisplayGlossary(List<JapaneseWord> words)
    {
        if (words.Count == 0)
        {
            MessageBoxFactory.CreateMessageBox("No Words in this Category", "There are no words to display in this category", null, true);
            return;
        }

        int glossaryDisplayCount = wordPrefabParent.childCount;
        //another option for updating the entries that are already visible
        //is to using the GlossaryEntry update function to check for updates from the Queue

        if (glossaryDisplayCount >= words.Count)
        {
            for (int i = 0; i < words.Count; i++)
            {
                entries[i].UpdateDisplay(words[i]);
                entries[i].transform.SetSiblingIndex(i);
                entries[i].gameObject.SetActive(true);
            }

            for (int i = words.Count; i < glossaryDisplayCount; i++)
            {
                wordPrefabParent.GetChild(i).gameObject.SetActive(false);
            }
        }
        else //words.Count > glossaryDisplayCount
        {
            for (int i = 0; i < glossaryDisplayCount; i++)
            {
                entries[i].UpdateDisplay(words[i]);
                entries[i].transform.SetSiblingIndex(i);
                entries[i].gameObject.SetActive(true);
            }

            int leftovers = words.Count - glossaryDisplayCount;
            for (int i = leftovers; i < words.Count; i++)
            {
                GlossaryEntry.WordsToDisplay.Enqueue(words[i]);
                var entry = GameObject.Instantiate(glossaryEntryPrefab, wordPrefabParent, false);
                entries.Add(entry.GetComponent<GlossaryEntry>());
            }
        }
        HelperFunctions.Log("Updated the Glossary Display");

    }

    private void SortBySeen(SortState s)
    {
        HelperFunctions.Log($"Sorting Seen by {s}");
        var sortedEntries = SortButton.SortBySeen(entries, s);
        DisplayGlossary(sortedEntries);
    }

    private void SortByCorrect(SortState s)
    {
        HelperFunctions.Log($"Sorting Corrrect by {s}");
        var sortedEntries = SortButton.SortByCorrect(entries, s);
        DisplayGlossary(sortedEntries);
    }

    private void SortBySpeed(SortState s)
    {
        HelperFunctions.Log($"Sorting Speed by {s}");
        var sortedEntries = SortButton.SortBySpeed(entries, s); // SortEntries(entries, SortProperty.Speed, s);
        DisplayGlossary(sortedEntries);
    }

    private void Search(string keyword)
    {
        if (!String.IsNullOrEmpty(keyword))
        {
            List<JapaneseWord> searchResults = sortedDict[filterOptions[subMenuDropdown.value]].Where(w => w.English.Contains(keyword) || w.Kana.Contains(keyword) || w.Kanji.Contains(keyword)).ToList();
            if (searchResults.Count > 0)
            {
                for (int i = 0; i < wordPrefabParent.childCount; i++)
                {
                    bool shouldDisable = true;
                    var child = wordPrefabParent.GetChild(i);
                    JapaneseWord displayedWord = child.GetComponent<GlossaryEntry>().Data;
                    foreach (var word in searchResults)
                    {
                        if (word.ID == displayedWord.ID)
                        {
                            shouldDisable = false;
                            break;
                        }
                    }

                    if (shouldDisable)
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < wordPrefabParent.childCount; i++)
            {
                var child = wordPrefabParent.GetChild(i);
                if (!child.gameObject.activeSelf)
                {
                    child.gameObject.SetActive(true);
                }
            }
        }

    }

}

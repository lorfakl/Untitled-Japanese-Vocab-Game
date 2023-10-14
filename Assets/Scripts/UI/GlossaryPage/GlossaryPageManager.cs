using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Utilities;
using Utilities.PlayFabHelper.CurrentUser;
using Newtonsoft.Json;
using ProjectSpecificGlobals;
using System.Linq;

public class GlossaryPageManager : MonoBehaviour
{
    [SerializeField]
    GameObject glossaryEntryPrefab;

    [SerializeField]
    GameObject glossaryContentPanel;

    [SerializeField]
    TMP_InputField searchField;

    [SerializeField]
    TMP_Dropdown filterDropdown;

    [SerializeField]
    SortButton seenSortButton;

    [SerializeField]
    SortButton correctSortButton;

    [SerializeField]
    SortButton speedSortButton;

    private static Dictionary<int, string> filterOptions = new Dictionary<int, string>();
    private Dictionary<string, List<JapaneseWord>> sortedDict = new Dictionary<string, List<JapaneseWord>>();
    private int childCount = 0;

    public void OnGlossaryEntrySelected(object e)
    {
        JapaneseWord selectedEntry = (JapaneseWord)e;
        WordDetailsController.Data.Enqueue(selectedEntry);
        HelperFunctions.Log("Casted the selected Entry");

        SwapActiveStateOfChilderen();
    }

    public void OnWordDetailsClosed()
    {
        SwapActiveStateOfChilderen();
    }

    private void Awake()
    {
        childCount = transform.childCount;
        HelperFunctions.Log("Is this called Glossary Awake");
        if (PlayFabController.IsAuthenticated)
        {
            InitializeGlossaryDefaultView();
        }
        else
        {
            PlayFabController.IsAuthedEvent += InitializeGlossaryDefaultView;
        }

        searchField.onValueChanged.AddListener(Search);
        filterDropdown.onValueChanged.AddListener(FilterDisplay);

        if(filterOptions.Count < filterDropdown.options.Count)
        {
            filterOptions.Clear();
            foreach (var opt in filterDropdown.options)
            {
                filterOptions.Add(filterDropdown.options.IndexOf(opt), opt.text);
            }
        }

        foreach (var opt in filterDropdown.options)
        {
            sortedDict.Add(opt.text, new List<JapaneseWord>());
        }
        
        if(correctSortButton != null) 
        {
            seenSortButton.OnSortModeChange += SortBySeen;
            correctSortButton.OnSortModeChange += SortByCorrect;
            speedSortButton.OnSortModeChange += SortBySpeed;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        FilterDisplay(filterDropdown.value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitializeGlossaryDefaultView()
    {
        //default view is "Studied" index 0
        try
        {
            Dictionary<string, List<JapaneseWord>> playerLeitnerLevels = JsonConvert.DeserializeObject<Dictionary<string, List<JapaneseWord>>>(CurrentAuthedPlayer.CachedUserData[Utilities.PlayFabHelper.UserDataKey.LeitnerLevels]);
            foreach (var singleLevels in playerLeitnerLevels.Values)
            {
                foreach (var word in singleLevels)
                {
                    sortedDict["Studied"].Add(word);
                    GlossaryEntry.WordsToDisplay.Enqueue(word);
                    GameObject.Instantiate(glossaryEntryPrefab, glossaryContentPanel.transform, false);
                }
            }
            HelperFunctions.Log($"Studied List has been loaded with {sortedDict["Studied"].Count} entires");
        }
        catch(Exception ex) 
        { 
            //new user or broken user load all words
        }
    }

    private void Search(string keyword)
    {
        if(!String.IsNullOrEmpty(keyword))
        {
            List<JapaneseWord> searchResults = sortedDict[filterOptions[filterDropdown.value]].Where(w => w.English.Contains(keyword) || w.Kana.Contains(keyword) || w.Kanji.Contains(keyword)).ToList();
            if(searchResults.Count > 0) 
            { 
                for(int i = 0; i < glossaryContentPanel.transform.childCount; i++)
                {
                    bool shouldDisable = true;
                    var child = glossaryContentPanel.transform.GetChild(i);
                    JapaneseWord displayedWord = child.GetComponent<GlossaryEntry>().Data;
                    foreach(var word in searchResults) 
                    { 
                        if(word.ID == displayedWord.ID)
                        {
                            shouldDisable = false;
                            break;
                        }
                    }

                    if(shouldDisable) 
                    { 
                        child.gameObject.SetActive(false);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < glossaryContentPanel.transform.childCount; i++)
            {
                var child = glossaryContentPanel.transform.GetChild(i);
                if(!child.gameObject.activeSelf)
                {
                    child.gameObject.SetActive(true);
                }
            }
        }

    }

    private void FilterDisplay(int optionIndex)
    {
        HelperFunctions.Log($"Int Value passed from event: {optionIndex}");
        string filterOption = filterOptions[optionIndex];
        DisplayGlossary(sortedDict[filterOption]);
    }

    private void DisplayGlossary(List<JapaneseWord> words)
    {
        int glossaryDisplayCount = glossaryContentPanel.transform.childCount;
        if (words.Count == glossaryDisplayCount)
        {
            HelperFunctions.Log("Exiting the Glossary Display, because this is likely the same data");
            return;
        }
        else
        {   //another option for updating the entries that are already visible
            //is to using the GlossaryEntry update function to check for updates from the Queue
            if (glossaryDisplayCount > words.Count) 
            {
                for(int i = 0; i < words.Count; i++)
                {
                    glossaryContentPanel.transform.GetChild(i).GetComponent<GlossaryEntry>().UpdateDisplay(words[i]);
                }
                int leftovers = glossaryDisplayCount - words.Count;
                for (int i = leftovers;i < glossaryDisplayCount; i++)
                {
                    glossaryContentPanel.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            else //words.Count > glossaryDisplayCount
            {
                for (int i = 0; i < glossaryDisplayCount; i++)
                {
                    glossaryContentPanel.transform.GetChild(i).GetComponent<GlossaryEntry>().UpdateDisplay(words[i]);
                }

                int leftovers = words.Count - glossaryDisplayCount;
                for (int i = leftovers; i < words.Count; i++)
                {
                    GlossaryEntry.WordsToDisplay.Enqueue(words[i]);
                    GameObject.Instantiate(glossaryEntryPrefab, glossaryContentPanel.transform, false);
                }
            }
            HelperFunctions.Log("Updated the Glossary Display");
        }
    }

    private void SortBySeen(SortState s)
    {
        //foreach()
    }

    private void SortByCorrect(SortState s)
    {

    }

    private void SortBySpeed(SortState s)
    {

    }

    private void SwapActiveStateOfChilderen()
    {
        for (int i = 1; i < childCount; i++)
        {
            var child = transform.GetChild(i);
            child.gameObject.SetActive(!child.gameObject.activeSelf);
        }
    }
}

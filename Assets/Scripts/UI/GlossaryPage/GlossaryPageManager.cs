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
    private Dictionary<int, List<JapaneseWord>> sortedDict = new Dictionary<int, List<JapaneseWord>>();
    private void Awake()
    {
        HelperFunctions.Log("Is this called Glossary Awake");
        if (PlayFabController.IsAuthenticated)
        {
            InitializeGlossaryDefaultView();
        }
        else
        {
            PlayFabController.IsAuthedEvent += InitializeGlossaryDefaultView;
        }

        searchField.onEndEdit.AddListener(Search);
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
            sortedDict.Add(filterDropdown.options.IndexOf(opt), new List<JapaneseWord>());

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
                    sortedDict[0].Add(word);
                }
            }
            HelperFunctions.Log($"Studied List has been loaded with {sortedDict[0].Count} entires");
        }
        catch(Exception ex) 
        { 
            //new user or broken user load all words
        }
        
    }

    private void Search(string keyword)
    {
        //throw new NotImplementedException();
    }

    private void FilterDisplay(int optionIndex)
    {
        HelperFunctions.Log($"Int Value passed from event: {optionIndex}");
        string filterOption = filterOptions[optionIndex];

    }

    private void SortBySeen(SortState s)
    {
        //foreach()
    }

    private void SortByCorrect(SortState s)
    { }

    private void SortBySpeed(SortState s)
    { }

}

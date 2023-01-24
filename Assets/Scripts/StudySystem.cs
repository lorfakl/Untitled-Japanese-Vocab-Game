using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Utilities.PlayFabHelper;
using Utilities.Events;
using Utilities;
using System.Linq;
using UnityEngine.SceneManagement;

public class StudySystem : MonoBehaviour
{

    #region Public Variables
    #endregion

    #region Private Variables
    List<JapaneseWord> sessionWords;

    
    [SerializeField]
    GameEvent correctAnswerEvent;

    [SerializeField]
    GameEvent incorrectAnswerEvent;

    [SerializeField]
    GameEvent removeWordFromSessionEvent;

    

    Dictionary<ProficiencyLevels, List<JapaneseWord>> sessionWordLeitnerLevel = new Dictionary<ProficiencyLevels, List<JapaneseWord>>();
    Dictionary<ProficiencyLevels, List<JapaneseWord>> sessionWordPrestigeLevel = new Dictionary<ProficiencyLevels, List<JapaneseWord>>();
    enum ProficiencyLevels
    {
        Zero = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8
    }

    #endregion

    #region Events

    #endregion

    #region Unity Events
    #endregion

    #region Public Methods
    public void StudyObjectOnClickHandler(object studyObjectClicked)
    {
        StudyObject studyObjSelected = (StudyObject)studyObjectClicked;
        if (studyObjSelected.Word.Kana == WordBankManager.NextWord.Kana)
        {
            //HelperFunctions.Log("Before Prof Mod: " + studyObjSelected.Word);
            if (correctAnswerEvent != null)
            {
                //HelperFunctions.Log("That answer was correct");
                correctAnswerEvent.Raise(studyObjectClicked);
                correctAnswerEvent.Raise();
                SetLeitnerLevel(true, studyObjSelected);
                JapaneseWord wordTarget = JSONWordLibrary.WordsToStudy.Find(word => word.Kanji == studyObjSelected.Word.Kanji);
                bool wasRemoved = JSONWordLibrary.WordsToStudy.Remove(wordTarget);
                if(wasRemoved)
                {
                    removeWordFromSessionEvent.Raise(JSONWordLibrary.WordsToStudy.Count);
                }
                else
                {
                    HelperFunctions.Warning(wordTarget + " was not successful removed");
                }
                HelperFunctions.Log("Words left to study: " + JSONWordLibrary.WordsToStudy.Count);
            }
        }
        else
        {
            //HelperFunctions.Log("Before Prof Mod: " + studyObjSelected.Word);
            if (incorrectAnswerEvent != null)
            {
                //HelperFunctions.Log("That answer was incorrect");
                incorrectAnswerEvent.Raise();
                SetLeitnerLevel(false, studyObjSelected);
                return; //return from here to avoid Adding 
                //the word to the LeitnerDict
            }
        }

        //HelperFunctions.Log("After Prof Mod: " + studyObjSelected.Word);
        AddWordToLeitnerDict(studyObjSelected.Word);
        if (JSONWordLibrary.WordsToStudy.Count == 0)
        {
            PlayFabController.UpdateUserData(new Dictionary<string, string>
                    {
                        {UserDataKey.LeitnerLevels.ToString(), JsonConvert.SerializeObject(sessionWordLeitnerLevel)}
                    });

            PlayFabController.ExecutionCSFunction(CSFunctionNames.AddNewWords,
                new Dictionary<string, string>
                {
                    { "count", StudySettings.newWordsPerDay.ToString() }
                }, Playfab.ArePlayStreamEventsGenerated);
            
            Dictionary<string, string> parameterDict = new Dictionary<string, string>();
            parameterDict.Add("Id", Playfab.PlayFabID);
            parameterDict.Add("TagName", PlayerTags.HasPlayedThisWeek.ToString());

            PlayFabController.ExecutionCSFunction(CSFunctionNames.UpdateTag, parameterDict, true);

            PlayFabController.ExecutionCSFunction<string, List<CloudScriptStatArgument>>(CSFunctionNames.Record, new Dictionary<string, List<CloudScriptStatArgument>>
            {
                {"Entries", new List<CloudScriptStatArgument>{ new CloudScriptStatArgument(StatisticName.LeagueSP, ScoreEventProcessors.Score),
                                                               new CloudScriptStatArgument(StatisticName.TotalSP, ScoreEventProcessors.Score)} }
            }, true);

           
            SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
        }
    }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        PlayFabController.IsAuthedEvent += PerformOneTimeWordSetUp;
        if(PlayFabController.IsAuthenticated)
        {
            PerformOneTimeWordSetUp();
        }
    }

    void Start()
    {
        for(int i = 0; i < Word.maxLeitnerLevel; i++)
        {
            if(!sessionWordLeitnerLevel.ContainsKey((ProficiencyLevels)i))
            {
                sessionWordLeitnerLevel.Add((ProficiencyLevels)i, new List<JapaneseWord>());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion

    #region Private Methods
    void PerformOneTimeWordSetUp()
    {
        GetCurrentWordSetFromPlayFab();
        HelperFunctions.Log("Authentication confirmed grabbing data");
    }

    void GetCurrentWordSetFromPlayFab()
    {
        if(Playfab.WasUserJustCreated)
        {
            PlayFabController.GetPlayerData(new List<string> { UserDataKey.SessionWords.ToString() }, NewlyCreatedParseReturnData);
        }
        else
        {
            PlayFabController.GetPlayerData(new List<string> { "LeitnerLevels", "PrestigeLevels", "NextSession" }, ParseReturnData);
        }
    }

    void NewlyCreatedParseReturnData(Dictionary<string, string> data)
    {
        //HelperFunctions.Log("Parsing return data");
        string leitnerJson = data[UserDataKey.SessionWords.ToString()];


        List<JapaneseWord> sessionWords = JsonConvert.DeserializeObject<List<JapaneseWord>>(leitnerJson);


        HelperFunctions.Log("Printing return data: ");
        HelperFunctions.LogListContent(sessionWords);
        JSONWordLibrary.SetWordsToStudy(sessionWords);
    }

    void ParseReturnData(Dictionary<string, string> data)
    {
        HelperFunctions.Log("Parsing return data");
        sessionWords = new List<JapaneseWord>();
        string leitnerJson = data[UserDataKey.LeitnerLevels.ToString()];
        string prestigeJson = data[UserDataKey.PrestigeLevels.ToString()];
        string nextSessionJson = data[UserDataKey.NextSession.ToString()];


        Dictionary<string, List<JapaneseWord>> currentLeitnerDict = JsonConvert.DeserializeObject<Dictionary<string, List<JapaneseWord>>>(leitnerJson);
        sessionWordLeitnerLevel = ConvertToSSDict(currentLeitnerDict);

        Dictionary<string, List<JapaneseWord>> currentPrestigeDict = JsonConvert.DeserializeObject<Dictionary<string, List<JapaneseWord>>>(prestigeJson);
        sessionWordPrestigeLevel = ConvertToSSDict(currentPrestigeDict);
        
        List<ProficiencyLevels> sessionKeys = JsonConvert.DeserializeObject<List<ProficiencyLevels>>(nextSessionJson);

        foreach (ProficiencyLevels key in sessionWordLeitnerLevel.Keys)
        {
            if (sessionKeys.Contains(key))
            {
                sessionWords.AddRange(sessionWordLeitnerLevel[key]);
            }
        }

        
        foreach (ProficiencyLevels key in sessionWordPrestigeLevel.Keys)
        {
            if (sessionKeys.Contains(key))
            {
                sessionWords.AddRange(sessionWordPrestigeLevel[key]);
            }
        }
        

        HelperFunctions.Log("Printing return data: ");
        HelperFunctions.LogListContent(sessionWords);
        JSONWordLibrary.SetWordsToStudy(sessionWords);
    }

    void SetLeitnerLevel(bool wasCorrect, StudyObject word)
    {
        word.Word.ModifyProficiencyLevel(wasCorrect);
    }

    void AddWordToLeitnerDict(JapaneseWord word)
    {
        //remove from previous level list
        sessionWordLeitnerLevel[(ProficiencyLevels)word.PreviousLeitnerLevel].Remove(word);
        sessionWordLeitnerLevel[(ProficiencyLevels)word.LeitnerLevel].Add(word);
        //HelperFunctions.LogListContent(sessionWordLeitnerLevel.ToList());
    }

    Dictionary<string, string> ConvertToPlayFabDict(Dictionary<ProficiencyLevels, List<JapaneseWord>> data)
    {
        Dictionary<string, string> convertedDict = new Dictionary<string, string>();
        foreach(ProficiencyLevels key in data.Keys)
        {
            convertedDict.Add(key.ToString(), JsonConvert.SerializeObject(data[key]));
        }

        return convertedDict;
    }

    Dictionary<ProficiencyLevels, List<JapaneseWord>> ConvertToSSDict(Dictionary<string, List<JapaneseWord>> data)
    {
        Dictionary<ProficiencyLevels, List<JapaneseWord>> convertedDict = new Dictionary<ProficiencyLevels, List<JapaneseWord>>();
        foreach (string key in data.Keys)
        {
            convertedDict.Add(HelperFunctions.ParseEnum<ProficiencyLevels>(key), data[key]);
        }

        return convertedDict;
    }
    #endregion
}
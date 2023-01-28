using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Utilities.PlayFabHelper;
using Utilities.Events;
using Utilities;
using System.Linq;

public enum ProficiencyLevels
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

public class StudySystem : MonoBehaviour
{

    List<JapaneseWord> sessionWords;
    
    //[SerializeField]
    //GameEvent correctAnswerEvent;

    //[SerializeField]
    //GameEvent incorrectAnswerEvent;

    [SerializeField]
    GameEvent resetScoreIntervalEvent;

    Dictionary<ProficiencyLevels, List<JapaneseWord>> sessionWordLeitnerLevel = new Dictionary<ProficiencyLevels, List<JapaneseWord>>();
    Dictionary<ProficiencyLevels, List<JapaneseWord>> sessionWordPrestigeLevel = new Dictionary<ProficiencyLevels, List<JapaneseWord>>();
    
    public Dictionary<ProficiencyLevels, List<JapaneseWord>> SessionWordLeitnerLevels
    {
        get { return sessionWordLeitnerLevel; }
    }

    #region Public Methods
    public void OnCompletedSessionWords_Handler() //Save Progress to PlayFab
    {
        SavePlayerDataToPlayFab();
        HelperFunctions.LoadScene(ProjectSpecificGlobals.SceneNames.MenuScene);
    }

    public void OnRemovedFromSessionList_Handler()
    {
        if(JSONWordLibrary.WordsToStudy.Count%5 == 0)
        {
            SavePlayerDataToPlayFab();
            resetScoreIntervalEvent.Raise();
        }
    }

    public void ModifySessionList(JapaneseWord s)
    {
        JapaneseWord wordTarget = sessionWords.Find(word => word.Kanji == s.Kanji);
        AddWordToLeitnerDict(s);
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
        /*for(int i = 0; i < Word.maxLeitnerLevel; i++)
        {
            if(!sessionWordLeitnerLevel.ContainsKey((ProficiencyLevels)i))
            {
                sessionWordLeitnerLevel.Add((ProficiencyLevels)i, new List<JapaneseWord>());
            }
        }*/
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
        JSONWordLibrary.SetWordsToStudy(sessionWords.ToList());
    }

    void ParseReturnData(Dictionary<string, string> data)
    {
        HelperFunctions.Log("Parsing return data");
        sessionWords = new List<JapaneseWord>();
        string leitnerJson = data[UserDataKey.LeitnerLevels.ToString()];
        string prestigeJson = data[UserDataKey.PrestigeLevels.ToString()];
        string nextSessionJson = data[UserDataKey.NextSession.ToString()];

        List<ProficiencyLevels> sessionKeys = JsonConvert.DeserializeObject<List<ProficiencyLevels>>(nextSessionJson);

        sessionWordLeitnerLevel = ParsePlayerProficiencyData(leitnerJson, sessionKeys);
        sessionWordPrestigeLevel = ParsePlayerProficiencyData(prestigeJson, sessionKeys);
        
        HelperFunctions.Log("Printing return data: ");
        HelperFunctions.LogListContent(sessionWords);
        JSONWordLibrary.SetWordsToStudy(sessionWords.ToList());

        foreach(var key in sessionWordLeitnerLevel.Keys)
        {
            sessionWordLeitnerLevel[key].Clear();
        }
    }

    void SavePlayerDataToPlayFab()
    {
        
        PlayFabController.UpdateUserData(new Dictionary<string, string>
                    {
                        {UserDataKey.LeitnerLevels.ToString(), JsonConvert.SerializeObject(sessionWordLeitnerLevel)}
                    });

        /*PlayFabController.ExecutionCSFunction(CSFunctionNames.AddNewWords,
            new Dictionary<string, string>
            {
                { "count", StudySettings.newWordsPerDay.ToString() }
            }, Playfab.ArePlayStreamEventsGenerated);*/

        Dictionary<string, string> parameterDict = new Dictionary<string, string>();
        parameterDict.Add("Id", Playfab.PlayFabID);
        parameterDict.Add("TagName", PlayerTags.HasPlayedThisWeek.ToString());

        PlayFabController.ExecutionCSFunction(CSFunctionNames.UpdateTag, parameterDict, true);

        PlayFabController.ExecutionCSFunction(CSFunctionNames.Record, new Dictionary<string, List<CloudScriptStatArgument>>
            {
                {"Entries", new List<CloudScriptStatArgument>{ new CloudScriptStatArgument(StatisticName.LeagueSP, ScoreEventProcessors.ScoreInterval),
                                                               new CloudScriptStatArgument(StatisticName.MonthlySP, ScoreEventProcessors.ScoreInterval),
                                                               new CloudScriptStatArgument(StatisticName.TotalSP, ScoreEventProcessors.ScoreInterval)
                                                              } 
                }
            }, true);
    }
    
    
    void AddWordToLeitnerDict(JapaneseWord word)
    {
        //remove from previous level list
        //sessionWordLeitnerLevel[(ProficiencyLevels)word.PreviousLeitnerLevel].Remove(word);
        sessionWordLeitnerLevel[(ProficiencyLevels)word.LeitnerLevel].Add(word);
        HelperFunctions.Log("Previous Lienter Level: " + word.PreviousLeitnerLevel);
        HelperFunctions.Log("Current Lienter Level: " + word.LeitnerLevel);
        HelperFunctions.Log(sessionWordLeitnerLevel.Count);
        HelperFunctions.Log(sessionWordLeitnerLevel[(ProficiencyLevels)word.LeitnerLevel].Count);
        //HelperFunctions.LogListContent(sessionWordLeitnerLevel.ToList());

    }
    Dictionary<ProficiencyLevels, List<JapaneseWord>> ParsePlayerProficiencyData(string proficiencyJson, List<ProficiencyLevels> sessionKeys)
    {
        Dictionary<string, List<JapaneseWord>> currentProficiencyDict = JsonConvert.DeserializeObject<Dictionary<string, List<JapaneseWord>>>(proficiencyJson);

        Dictionary<ProficiencyLevels, List<JapaneseWord>> convertedDict = new Dictionary<ProficiencyLevels, List<JapaneseWord>>();
        foreach (string key in currentProficiencyDict.Keys)
        {
            ProficiencyLevels p = HelperFunctions.ParseEnum<ProficiencyLevels>(key);
            if(sessionKeys.Contains(p))
            {
                sessionWords.AddRange(currentProficiencyDict[key]);
            }
            convertedDict.Add(p, currentProficiencyDict[key]);
        }

        return convertedDict;
    }
    #endregion
}
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Utilities.PlayFabHelper;
using Utilities.Events;
using Utilities;
using System.Linq;
using Utilities.PlayFabHelper.CurrentUser;
using Utilities.PlayFabHelper.CSArguments;
using System;

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
    int studyProgress = 0;

    static bool shouldNewWordsBeAdded = true; //we want this value to persist after the StudyScene is unloaded
    
    public Dictionary<ProficiencyLevels, List<JapaneseWord>> SessionWordLeitnerLevels
    {
        get { return sessionWordLeitnerLevel; }
    }

    #region Public Methods
    public void OnCompletedSessionWords_Handler() //Save Progress to PlayFab
    {
        SavePlayerDataToPlayFab();
        //HelperFunctions.LoadScene(ProjectSpecificGlobals.SceneNames.MenuScene);
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
        //JapaneseWord wordTarget = sessionWords.Find(word => word.Kanji == s.Kanji);
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
        for(int i = 0; i <= Word.maxLeitnerLevel; i++)
        {
            if(!sessionWordLeitnerLevel.ContainsKey((ProficiencyLevels)i))
            {
                sessionWordLeitnerLevel.Add((ProficiencyLevels)i, new List<JapaneseWord>());
            }
        }
    }

    private void OnDisable()
    {
        if(shouldNewWordsBeAdded)
        {
            shouldNewWordsBeAdded = false;
            AddNewWords();
        }
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
            if(StaticUserSettings.IsKanjiStudyTopic())
            {
                PlayFabController.GetPlayerData(new List<string> { UserDataKey.SessionWords.ToString() }, NewlyCreatedParseReturnData);
            }
            else
            {
                PlayFabController.GetPlayerData(new List<string> { UserDataKey.SessionKana.ToString() }, NewlyCreatedParseReturnData);
            }
            
            
        }
        else
        {
            PlayFabController.GetPlayerData(new List<string> {
                UserDataKey.LeitnerLevels.ToString() , UserDataKey.PrestigeLevels.ToString() , UserDataKey.NextSession.ToString() , UserDataKey.WordsSeen.ToString() 
            }, ParseReturnData);
        }
    }

    void NewlyCreatedParseReturnData(Dictionary<string, string> data)
    {
        //HelperFunctions.Log("Parsing return data");
        string sessionKey = "";

        if(StaticUserSettings.IsKanjiStudyTopic()) 
        { 
            sessionKey = UserDataKey.SessionWords.ToString();
        }
        else
        {
            sessionKey = UserDataKey.SessionKana.ToString();
        }

        PlayFabController.UpdateUserData(new Dictionary<string, string>
                    {
                        {UserDataKey.LeitnerLevels.ToString(), JsonConvert.SerializeObject(data[sessionKey])}
                    });
        string leitnerJson = data[sessionKey];


        List<JapaneseWord> sessionWords = JsonConvert.DeserializeObject<List<JapaneseWord>>(leitnerJson);
        sessionWordLeitnerLevel[ProficiencyLevels.Zero] = sessionWords.ToList();
        PlayFabController.UpdateUserData(new Dictionary<string, string>
                    {
                        {UserDataKey.LeitnerLevels.ToString(), JsonConvert.SerializeObject(sessionWordLeitnerLevel)}
                    });

        studyProgress = 20;

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

        try
        {
            int parsedProgress = Int32.Parse(data[UserDataKey.WordsSeen.ToString()].ToString());
            studyProgress = parsedProgress;
        }
        catch(Exception e)
        {
            HelperFunctions.CatchException(e);
            HelperFunctions.Error($"Unable to cast WordsSeen value from PF. Returned value: {data[UserDataKey.WordsSeen.ToString()]}");
        }
        

        List<ProficiencyLevels> sessionKeys = JsonConvert.DeserializeObject<List<ProficiencyLevels>>(nextSessionJson);
        sessionWordLeitnerLevel = ParsePlayerProficiencyData(leitnerJson, sessionKeys);
        sessionWordPrestigeLevel = ParsePlayerProficiencyData(prestigeJson, sessionKeys);
        
        HelperFunctions.Log("Printing return data: ");
        HelperFunctions.LogListContent(sessionWords);
        var newList = SetTotalWords(sessionWords);

        if (newList.Count == StaticUserSettings.GetTotalWords() && newList.Count > 0)
        {
            JSONWordLibrary.SetWordsToStudy(newList.ToList());
        }
        else
        {
            JSONWordLibrary.SetWordsToStudy(sessionWords.ToList());
        }

        if(sessionWords.Count == 0)
        {
            MessageBoxFactory.CreateMessageBox("Nothing Left to Study", "There are no more words to study right now. Live your life! Come back in 12 hours",
                () => { HelperFunctions.LoadScene(ProjectSpecificGlobals.SceneNames.MenuScene); });
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
        try
        {
            sessionWordLeitnerLevel[(ProficiencyLevels)word.PreviousLeitnerLevel].Remove(word);
            sessionWordLeitnerLevel[(ProficiencyLevels)word.LeitnerLevel].Add(word);
        }
        catch(KeyNotFoundException e)
        {
            HelperFunctions.CatchException(e);

        }
        //remove from previous level list
        
        /*HelperFunctions.Log("Previous Lienter Level: " + word.PreviousLeitnerLevel);
        HelperFunctions.Log("Current Lienter Level: " + word.LeitnerLevel);
        HelperFunctions.Log(sessionWordLeitnerLevel.Count);
        HelperFunctions.Log(sessionWordLeitnerLevel[(ProficiencyLevels)word.LeitnerLevel].Count);*/
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
    
    void AddNewWords()
    {
        AddWordArgument addwordArg = new AddWordArgument
        {
            IsKanjiStudyTopic = StaticUserSettings.IsKanjiStudyTopic(),
            NumToAdd = StaticUserSettings.GetNewWords(),
            Progress = studyProgress
        };

        PlayFabController.ExecutionCSFunction(CSFunctionNames.AddNewWords, addwordArg);
    }

    /// <summary>
    /// Using the StaticUserSettings to get the total words. This function takes in a list of words, if it is longer than TotalWords
    /// the list is shorted and returned
    /// </summary>
    /// <param name="l"></param>
    /// <returns>Shorted list of words, whose size is equal to StaticUserSettings.TotalWordsPerSession</returns>
    List<JapaneseWord> SetTotalWords(List<JapaneseWord> l)
    {
        int totalWords = StaticUserSettings.GetTotalWords();
        List<JapaneseWord> shortenedList = new List<JapaneseWord>(totalWords);
        if (l.Count > totalWords)
        {
            for(int i = 0; i < shortenedList.Capacity; i++)
            {
                int randIndex = UnityEngine.Random.Range(0, l.Count);
                shortenedList.Add(l[randIndex]);
                l.RemoveAt(randIndex);
            }
            return shortenedList;
        }
        else
        {
            return l;
        }

        
    }
    #endregion
}
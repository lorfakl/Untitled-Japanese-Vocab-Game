using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Events;
using Utilities.PlayFab;
using Utilities;
using PlayFab.ClientModels;
using UnityEditor;

public struct LeaderboardEntry
{
    public string displayName;
    public string rank;
    public string playfabID;
    public string score;
    public string avatarURL;

    public void Print()
    {
        HelperFunctions.Log("DisplayName: " + this.displayName + "\n" +
            "Rank: " + this.rank + "\n" +
            "PlayFabID: " + this.playfabID + "\n" +
            "Score: " + this.score + "\n" +
            "AvatarURL: " + this.avatarURL);
    }
}

public class StatPageManager : MonoBehaviour
{

    #region Public Variables
    #endregion

    #region Private Variables
    [SerializeField]
    GameObject leaderboardPanelContent;

    [SerializeField]
    GameObject leaderboardEntryPrefab;

    List<PlayerLeaderboardEntry> playerLeaderboardEntries;

    static Queue<LeaderboardEntry> entryQueue = new Queue<LeaderboardEntry>();
    #endregion

    #region Events


    #endregion

    #region Events

    #endregion

    #region Unity Events
    #endregion

    #region Public Methods
    public static LeaderboardEntry GetLeaderboardEntry(object caller)
    {
        if(caller.GetType() == typeof(LeaderboardEntryController))
        {
            return entryQueue.Dequeue();
        }
        else
        {
            return new LeaderboardEntry();
        }
    }
    #endregion

    #region Unity Methods
    private void Awake()
    {
            }

    void Start()
    {
        PlayFabController.IsAuthedEvent += InitializeStatsPage;

    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion

    #region Private Methods
    private void InitializeStatsPage()
    {
        PlayFabController.GetPlayerTags(ProcessTags);
    }

    private void ProcessTags(GetPlayerTagsResult playerTags)
    {
        bool isGrouped = false;
        foreach(string tag in playerTags.Tags)
        {
            if(tag.Contains(PlayerTags.InGroup.ToString()))
            {
                isGrouped = true;
            }
        }

        if(isGrouped)
        {

        }
        else
        {
            PlayFabController.GetLeadboard(true, StatisticName.LeagueSP, ConvertToTitlePlayerIDs);
            PlayFabController.CreateGroup(Guid.NewGuid().ToString(), (success) => { });
        }
    }

    private void ConvertToTitlePlayerIDs(List<PlayerLeaderboardEntry> leaderboardData)
    {
        playerLeaderboardEntries = leaderboardData;
        List<string> playFabIDs = new List<string>();
        foreach(PlayerLeaderboardEntry entry in leaderboardData)
        {
            playFabIDs.Add(entry.PlayFabId);
        }

        PlayFabController.GetTitlePlayerAccounts(playFabIDs, BuildRivalGroup);
    }

    private void BuildRivalGroup(Dictionary<string, UniversalEntityKey> convertedIDsDict)
    {
        GenerateLeaderboardView();
    }

    private void GenerateLeaderboardView()
    {
        foreach(PlayerLeaderboardEntry entry in playerLeaderboardEntries)
        {
            entryQueue.Enqueue(new LeaderboardEntry
            {
                avatarURL = entry.Profile.AvatarUrl,
                displayName = entry.DisplayName,
                playfabID = entry.PlayFabId,
                rank = (playerLeaderboardEntries.IndexOf(entry) + 1).ToString(),
                score = entry.StatValue.ToString()
            });
        }

        foreach(PlayerLeaderboardEntry entry in playerLeaderboardEntries)
        {
            GameObject.Instantiate(leaderboardEntryPrefab, leaderboardPanelContent.transform);

        }
    }
    #endregion
}

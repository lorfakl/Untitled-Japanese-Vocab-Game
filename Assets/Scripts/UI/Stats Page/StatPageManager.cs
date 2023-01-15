using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Utilities.PlayFabHelper.CSArguments;
using Utilities.PlayFabHelper.CurrentUser;
using Utilities.PlayFabHelper;
using Utilities;
using PlayFab.ClientModels;
using System.Threading.Tasks;

public class StatPageManager : MonoBehaviour
{

    #region Public Variables
    #endregion

    #region Private Variables
    [SerializeField]
    GameObject leaderboardPanelContent;

    [SerializeField]
    GameObject leaderboardEntryPrefab;

    [SerializeField]
    LocalPlayFabData LocalPlayFabData;

    Leaderboard playerLeaderboardEntries;
    
    bool isGrouped = false;
    bool hasPlayed = false;
    bool hasLoadedFromFile = false;

    #endregion

    #region Events

    #endregion

    #region Unity Events
    #endregion

    #region Public Methods
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if(PlayFabController.IsAuthenticated)
        {
            HelperFunctions.Log("Is now authenticated");
            InitializeStatsPage(true);
        }
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

    private void InitializeStatsPage(bool hasData)
    {
        hasPlayed = hasData;
        PlayFabController.GetPlayerTags(ProcessTags);
    }

    private void ProcessTags(GetPlayerTagsResult playerTags)
    {
        
        foreach(string tag in playerTags.Tags)
        {
            if(tag.Contains(PlayerTags.InGroup.ToString()))
            {
                isGrouped = true;
            }

            if(tag.Contains(PlayerTags.HasPlayedThisWeek.ToString()))
            {
                hasPlayed = true;
            }
        }

        HelperFunctions.Warning("Ranking needs to take place in the Leaderboard Entry Controller");

        if (isGrouped)
        {
            HelperFunctions.Log("Grab playfab group data");
            try
            {
                if (CurrentAuthedPlayer.CurrentUser?.Groups[0]?.MembersList?.Count > 0)
                {
                    HelperFunctions.Log("Skipped CS call using cache");
                    hasLoadedFromFile = true;
                    GenerateLeaderboardView(CurrentAuthedPlayer.CurrentUser?.Groups[0]?.MembersList);
                }
                else
                {
                    PlayFabController.GetGroupMembers(CurrentAuthedPlayer.CurrentUser.Groups[0].EntityKey, GenerateLeaderboardView);
                }
            }
            catch(Exception e)
            {
                HelperFunctions.CatchException(e);
                var eKey = new UniversalEntityKey(LocalPlayFabData.GroupID, EntityTypes.group.ToString());
                HelperFunctions.Log("File reading failed");
                CurrentAuthedPlayer.CurrentUser.UpdateGroup(new PlayFabGroup(eKey, "PrimaryGroup"));
                PlayFabController.GetGroupMembers(eKey, GenerateLeaderboardView);
            }
            
            
            
        }
        else if(hasPlayed)
        {
            HelperFunctions.Log("Creating the group");
            LeaderboardManager.CreateLeaderboard(StatisticName.TotalSP, leaderboardEntryPrefab, leaderboardPanelContent.transform, ConvertToTitlePlayerIDsAsync);
            PlayFabController.CreateGroup(Guid.NewGuid().ToString(), (success) => 
            {
                LocalPlayFabData.GroupID = success.Group.Id;
            });
        }
        else
        {
            HelperFunctions.Log("Go study then come back to get Rivals");
        }
    }

    private async Task ConvertToTitlePlayerIDsAsync(Leaderboard leaderboardData)
    {
        playerLeaderboardEntries = leaderboardData;
        List<string> playFabIDs = new List<string>();
        foreach(LeaderboardEntry entry in leaderboardData.Entries)
        {
            playFabIDs.Add(entry.playfabID);
        }

        PlayFabController.GetTitlePlayerAccounts(playFabIDs, BuildRivalGroup);
        await Task.Delay(1000);
    }

    private void BuildRivalGroup(Dictionary<string, UniversalEntityKey> convertedIDsDict)
    {
        List<string> universalEntities = new List<string>();
        foreach(KeyValuePair<string, UniversalEntityKey> pair in convertedIDsDict)
        {
            universalEntities.Add(pair.Value.ID);
        }
        var parameter = new AddMembersCSArgument
        {
            GroupID = LocalPlayFabData.GroupID,
            MemberKeys = universalEntities
        };
        PlayFabController.ExecutionCSFunction(CSFunctionNames.AddMembers, parameter);
    }


    private void GenerateLeaderboardView(List<BasicProfile> profiles)
    {
        if(!hasLoadedFromFile)
        {
            CurrentAuthedPlayer.CurrentUser.Groups[0].Add(profiles);
        }
        
        List<LeaderboardEntry> leaderboard = new List<LeaderboardEntry>();
        try
        {
            profiles = profiles.OrderBy(x => x.Statistics[StatisticName.LeagueSP].value).ToList();
        }
        catch(KeyNotFoundException e)
        {
            HelperFunctions.CatchException(e);
            foreach(var p in profiles)
            {
                if(p.Statistics.Count < 2)
                {
                    HelperFunctions.Log(p.PlayFabID);
                    HelperFunctions.LogDictContent(p.Statistics);   
                }
            }
        }

        foreach(BasicProfile profile in profiles)
        {
            if(profile.PlayFabID == Playfab.PlayFabID)
            {
                playerLeaderboardEntries.EntryQueue.Enqueue(new LeaderboardEntry
                {
                    displayName = "YOU",
                    playfabID = profile.PlayFabID,
                    score = ScoreEventProcessors.Score,
                    rank = (profiles.IndexOf(profile) + 1)

                });
            }
            else
            {
                playerLeaderboardEntries.EntryQueue.Enqueue(new LeaderboardEntry
                {
                    displayName = profile.DisplayName,
                    playfabID = profile.PlayFabID,
                    score = profile.Statistics[StatisticName.LeagueSP].DecodeStatisticValue(),
                    rank = (profiles.IndexOf(profile) + 1)
                });
            }
            
        }

        foreach (BasicProfile entry in profiles)
        {
            LeaderboardEntryController c = GameObject.Instantiate(leaderboardEntryPrefab, leaderboardPanelContent.transform).GetComponent<LeaderboardEntryController>();
            c.SetLeaderboardHost(playerLeaderboardEntries);

        }

    }
    #endregion
}

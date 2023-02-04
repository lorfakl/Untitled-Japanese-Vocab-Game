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
using TMPro;

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

    [SerializeField]
    TMP_Text averageSpeed;

    [SerializeField]
    TMP_Text wordsKnown;

    [SerializeField]
    TMP_Text longestStreak;

    [SerializeField]
    TMP_Text currentStreak;

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
    public void OnStatReportReady(object r)
    {
        StudyRecord studyRecord = r as StudyRecord;

    }
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
        DisplayRecord();
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
            //LeaderboardManager.CreateLeaderboard(StatisticName.LeagueSP, leaderboardEntryPrefab, leaderboardPanelContent.transform, )
            
            
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
        playerLeaderboardEntries = new Leaderboard();

        foreach (var profile in profiles)
        {
            try
            {
                var le = new LeaderboardEntry
                {
                    displayName = profile.DisplayName,
                    playfabID = profile.PlayFabID,
                    score = Int32.Parse(profile.Statistics[StatisticName.LeagueSP].value),
                    rank = (profiles.IndexOf(profile) + 1)
                };

                le.Print();
                //playerLeaderboardEntries.EntryQueue.Enqueue(le);
                playerLeaderboardEntries.AddEntry(le);
            }
            catch (KeyNotFoundException e)
            {
                HelperFunctions.CatchException(e);
                HelperFunctions.Log("Missing KEY!?!?!!");
                HelperFunctions.Log(profile.PlayFabID);
                HelperFunctions.LogDictContent(profile.Statistics);
            }

            
            //playerLeaderboardEntries.Or

            /*if (profile.PlayFabID == Playfab.PlayFabID)
            {
                playerLeaderboardEntries.EntryQueue.Enqueue(new LeaderboardEntry
                {
                    displayName = "YOU",
                    playfabID = profile.PlayFabID,
                    score = Int32.Parse(profile.Statistics[StatisticName.LeagueSP].value),
                    rank = (profiles.IndexOf(profile) + 1)

                });
            }
            else
            {
                try
                {
                    HelperFunctions.Log("sTATS: " + profile.Statistics[StatisticName.LeagueSP].value);
                    var le = new LeaderboardEntry
                    {
                        displayName = profile.DisplayName,
                        playfabID = profile.PlayFabID,
                        score = Int32.Parse(profile.Statistics[StatisticName.LeagueSP].value),
                        rank = (profiles.IndexOf(profile) + 1)
                    };

                    le.Print();
                    playerLeaderboardEntries.EntryQueue.Enqueue(le);
                    playerLeaderboardEntries.AddEntry(le);
                }
                catch (KeyNotFoundException e)
                {
                    HelperFunctions.CatchException(e);
                    HelperFunctions.Log("Missing KEY!?!?!!");
                    HelperFunctions.Log(profile.PlayFabID);
                    HelperFunctions.LogDictContent(profile.Statistics);
                }

            }*/
        }


        LeaderboardEntry[] ordered = playerLeaderboardEntries.GetOrderArray();

        foreach (var entry in ordered)
        {
            playerLeaderboardEntries.EntryQueue.Enqueue(entry);
            LeaderboardEntryController c = GameObject.Instantiate(leaderboardEntryPrefab, leaderboardPanelContent.transform).GetComponent<LeaderboardEntryController>();
            c.SetLeaderboardHost(playerLeaderboardEntries);

        }

    }

    private void DisplayRecord()
    {
        var r = DataPlatform.GetStudyRecord();
        if(r.Record.Count == 0)
        {
            averageSpeed.text += " " + "N/A";
            wordsKnown.text += " " + "N/A";
            currentStreak.text += " " + "N/A";
            longestStreak.text += " " + "N/A";
            return;
        }

        var report = r.GenerateRecordReport();


        List<string> reportData = new List<string>();

        foreach(string key in report.Keys)
        {
            reportData.Add(report[key]);
        }
        averageSpeed.text += reportData[0];
        wordsKnown.text += " " + reportData[reportData.Count - 3];
        currentStreak.text += " " + reportData[reportData.Count - 2];
        longestStreak.text += " " + reportData[reportData.Count - 1];
    }
    #endregion
}

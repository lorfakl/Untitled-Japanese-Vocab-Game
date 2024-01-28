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
using Utilities.Events;

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

    [SerializeField]
    TMP_Text goStudyText;

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
        GameEvent loadingLeaderboardComplete = ScriptableObject.CreateInstance<GameEvent>();
        HelperFunctions.Warning("Ranking needs to take place in the Leaderboard Entry Controller");
        var leaderboardLoading = MessageBoxFactory.Create(MessageBoxType.Loading, "Grabbing updated Leaderboard Info. Please Wait...", "Loading Leaderboard Info", loadingLeaderboardComplete, averageSpeed.transform.parent);
        leaderboardLoading.DisplayMessageBox(leaderboardLoading.AutoDestroyMessageBox, true);

        if (isGrouped)
        {
            HelperFunctions.Log("Grab playfab group data");
            try
            {
                if (CurrentAuthedPlayer.CurrentUser?.Group?.MembersList?.Count > 0) 
                { //we have group members just grab member data
                    HelperFunctions.Log("Skipped CS call using cache");
                    hasLoadedFromFile = true;
                    

                    GetOtherPlayerStatisticArgument otherStats = new GetOtherPlayerStatisticArgument
                    {
                        playFabIDs = CurrentAuthedPlayer.CurrentUser?.Group.GetMemberIDs(),
                        StatisticName = StatisticName.LeagueSP
                    };
                    HelperFunctions.Log(JsonConvert.SerializeObject(otherStats));
                    PlayFabController.ExecutionCSFunction(CSFunctionNames.GetOtherStatistics, otherStats, 
                        (csResult) => 
                        {
                            List<GetOtherPlayerStatisticsResult> csResponse = JsonConvert.DeserializeObject<List<GetOtherPlayerStatisticsResult>>(csResult.FunctionResult.ToString());
                            List<BasicProfile> members = new List<BasicProfile>();
                            foreach(var r in csResponse)
                            {
                                members.Add(new BasicProfile(r, StatisticName.LeagueSP));
                            }

                            CurrentAuthedPlayer.CurrentUser?.Group.OverWriteMembers(members);
                            GenerateLeaderboardView(CurrentAuthedPlayer.CurrentUser?.Group?.MembersList);
                            loadingLeaderboardComplete.Raise();
                        });
                    
                }
                else
                {//no members just groupID
                    PlayFabController.GetGroupMembers(CurrentAuthedPlayer.CurrentUser.Group.EntityKey, 
                        (result)=> 
                        {
                            GenerateLeaderboardView(result);
                            loadingLeaderboardComplete.Raise();
                        },
                        (error)=>
                        {
                            PlayFabController.ListGroupMembership(
                                (result) =>
                                {
                                    foreach (var g in result)
                                    {
                                        PlayFabController.GetGroupMembers(CurrentAuthedPlayer.CurrentUser.Group.EntityKey,
                                        (result) =>
                                        {
                                            GenerateLeaderboardView(result);
                                            loadingLeaderboardComplete.Raise();
                                        });
                                    }
                                });
                        });
                }
            }
            catch(Exception e)
            {//no group data at all
                HelperFunctions.CatchException(e);
                PlayFabController.ListGroupMembership( //get group membership
                    (groupList) =>
                    {
                        if (groupList.Count > 0)
                        {
                            var g = groupList[0];
                            CurrentAuthedPlayer.CurrentUser.UpdateGroup(g);
                            PlayFabController.GetGroupMembers(g.EntityKey, (result) => //get member data
                            {
                                CurrentAuthedPlayer.CurrentUser.Group.OverWriteMembers(result);
                                GenerateLeaderboardView(result);
                                loadingLeaderboardComplete.Raise();
                            }); 
                        }
                        else
                        {
                            HelperFunctions.Log("Go study then come back to get Rivals");
                            goStudyText.enabled = true;
                            
                            loadingLeaderboardComplete.Raise();
                        }
                    });
            }
            //LeaderboardManager.CreateLeaderboard(StatisticName.LeagueSP, leaderboardEntryPrefab, leaderboardPanelContent.transform, );
        }
        else if(hasPlayed)
        {
            HelperFunctions.Log("Creating the group");
            LeaderboardManager.CreateLeaderboard(StatisticName.LeagueSP, leaderboardEntryPrefab, leaderboardPanelContent.transform, ConvertToTitlePlayerIDsAsync);
            PlayFabController.CreateGroup(Guid.NewGuid().ToString(), (success) => 
            {
                LocalPlayFabData.GroupID = success.Group.Id;
            });
            loadingLeaderboardComplete.Raise();
        }
        else
        {
            loadingLeaderboardComplete.Raise();
            HelperFunctions.Log("Go study then come back to get Rivals");
            goStudyText.enabled = true;
            PlayFabController.DeleteAllGroups();
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
            CurrentAuthedPlayer.CurrentUser.Group.OverWriteMembers(profiles);
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
                    score = profile.Statistics[StatisticName.LeagueSP].InteralValue,
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
        double overallAverage = DataPlatform.GetOverallAverage();
        int wordsKnownCount = DataPlatform.GetWordsKnownCount();
        int currentStreakNumber = DataPlatform.GetCurrentStreak();
        int longestStreakNumber = DataPlatform.GetLongestStreak();
        averageSpeed.text += overallAverage != -1 ? $" {overallAverage}" : " N/A";
        wordsKnown.text += wordsKnownCount != -1 ? $" {wordsKnownCount}" : " N/A";
        currentStreak.text += currentStreakNumber != -1 ? $" {currentStreakNumber}" : " N/A";
        longestStreak.text += longestStreakNumber != -1 ? $" {longestStreakNumber}" : " N/A";
    }
    #endregion
}

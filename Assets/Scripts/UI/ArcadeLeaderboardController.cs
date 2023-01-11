using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.PlayFabHelper;
using Utilities.PlayFabHelper.CSArguments;
using UnityEngine.SceneManagement;
using PlayFab.ClientModels;
using Utilities.Events;
using Utilities;

public class ArcadeLeaderboardController : MonoBehaviour
{
    [SerializeField]
    Transform _content;

    [SerializeField]
    GameObject _leaderboardEntryPrefab;

    [SerializeField]
    GameEvent leaderboardLoadedEvent;

    List<PlayerLeaderboardEntry> playerLeaderboardEntries;
    static Queue<LeaderboardEntry> entryQueue = new Queue<LeaderboardEntry>();

    public static LeaderboardEntry GetLeaderboardEntry(object caller)
    {
        if (caller.GetType() == typeof(LeaderboardEntryController))
        {
            return entryQueue.Dequeue();
        }
        else
        {
            return new LeaderboardEntry();
        }
    }

    public void MoveToOpeningScene()
    {
        SceneManager.LoadScene("ArcadeOpeningScene");
    }

    // Start is called before the first frame update
    void Start()
    {
        if(PlayFabController.IsAuthenticated)
        {
            PlayFabController.GetLeadboard(false, StatisticName.ArcadeScore, ConvertToTitlePlayerIDs);
        }
        else
        {
            PlayFabController.IsAuthedEvent += OnAuthenticationEvent;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnAuthenticationEvent()
    {
        PlayFabController.GetLeadboard(false, StatisticName.ArcadeScore, ConvertToTitlePlayerIDs);
    }

    private void ConvertToTitlePlayerIDs(List<PlayerLeaderboardEntry> leaderboardData)
    {
        playerLeaderboardEntries = leaderboardData;
        List<string> playFabIDs = new List<string>();
        foreach (PlayerLeaderboardEntry entry in leaderboardData)
        {
            playFabIDs.Add(entry.PlayFabId);
        }

        PlayFabController.GetTitlePlayerAccounts(playFabIDs, GetAvatarPhotos);
    }

    void GetAvatarPhotos(Dictionary<string, UniversalEntityKey> convertedIDsDict)
    {
        List<UniversalEntityKey> universalEntities = new List<UniversalEntityKey>();
        foreach (KeyValuePair<string, UniversalEntityKey> pair in convertedIDsDict)
        {
            universalEntities.Add(pair.Value);
        }
        GenerateLeaderboardView();

        var getRivalArg = new GetRivalAvatarsCSArgument
        {
            Rivals = universalEntities
        };
        PlayFabController.ExecutionCSFunction(CSFunctionNames.GetRivalAvatars, getRivalArg, ProcessAvatarImages);
    }

    void ProcessAvatarImages(PlayFab.CloudScriptModels.ExecuteFunctionResult leaderboardEntries)
    {
        HelperFunctions.Log(leaderboardEntries.FunctionResult);
    }

    private void GenerateLeaderboardView()
    {
        foreach (PlayerLeaderboardEntry entry in playerLeaderboardEntries)
        {
            if (entry.PlayFabId == Playfab.PlayFabID)
            {
                entryQueue.Enqueue(new LeaderboardEntry
                {
                    avatarURL = entry.Profile.AvatarUrl,
                    displayName = "YOU",
                    playfabID = entry.PlayFabId,
                    score = ScoreEventProcessors.Score + " **fix",
                    rank = (playerLeaderboardEntries.IndexOf(entry) + 1).ToString()
                });

            }
            else
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

        }

        foreach (PlayerLeaderboardEntry entry in playerLeaderboardEntries)
        {
            GameObject.Instantiate(_leaderboardEntryPrefab, _content);

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Utilities.Events;
using Utilities.PlayFabHelper;
using System.Linq;

public class ArcadeOpening : MonoBehaviour
{
    [SerializeField]
    Button _startBtn;

    [SerializeField]
    TMP_InputField _InputField;

    [SerializeField]
    GameEvent _startStudyEvent;

    [SerializeField]
    Transform _content;

    [SerializeField]
    GameObject _leaderboardEntryPrefab;

    [SerializeField]
    GameEvent leaderboardLoadedEvent;
    // Start is called before the first frame update

    string name = "";
    

    void Start()
    {
        _startBtn.onClick.AddListener(ConfigureNewUser);
        var loadingLeaderboardBox = MessageBoxFactory.Create(MessageBoxType.Loading, "Please Wait", "Loading Arcade Leaderboard", leaderboardLoadedEvent);
        loadingLeaderboardBox.DisplayLoadingMessageBox(loadingLeaderboardBox.AutoDestroyMessageBox);
        DisplayArcadeLeaderboard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ConfigureNewUser()
    {
        if(String.IsNullOrEmpty(_InputField.text))
        {
            var errorBox = MessageBoxFactory.Create(MessageBoxType.Message, "You must enter a display name", "Missing DisplayName");
            errorBox.DisplayMessageBox(errorBox.AutoDestroyMessageBox);
            return;
        }

        name = _InputField.text;
        //PlayFabController.ArcadeLogin(name,UpdateDisplayName);
        ArcadeStudyManager.AddArcadePlayer(name);
        HelperFunctions.Log("Configured New User ");
        HelperFunctions.LoadScene(ProjectSpecificGlobals.SceneNames.EnenraScene);
    }

    void UpdateDisplayName()
    {
        PlayFabController.DisplayName(name);
        
        HelperFunctions.Log("Start Study Event Raised");
    }

    void DisplayArcadeLeaderboard()
    {
        List<ArcadeStudyManager.ArcadePlayer> arcadeLeaderboard = new List<ArcadeStudyManager.ArcadePlayer>();
        foreach (var key in ArcadeStudyManager.ArcadePlayers.Keys)
        {
            arcadeLeaderboard.Add(ArcadeStudyManager.ArcadePlayers[key]);
        }

        Leaderboard arcadeBoard = new Leaderboard();
        List<ArcadeStudyManager.ArcadePlayer> orderedLeaderboard = arcadeLeaderboard.OrderByDescending(entry => entry.score).ToList();
        for (int i = 0; i < orderedLeaderboard.Count; i++)
        {
            orderedLeaderboard[i].rank = i + 1;
            LeaderboardEntry entry = new LeaderboardEntry
            {
                rank = orderedLeaderboard[i].rank,
                displayName = orderedLeaderboard[i].displayName,
                score = orderedLeaderboard[i].score
            };

            arcadeBoard.EntryQueue.Enqueue(entry);

            LeaderboardEntryController c = GameObject.Instantiate(_leaderboardEntryPrefab, _content).GetComponent<LeaderboardEntryController>();
            c.SetLeaderboardHost(arcadeBoard);
        }

        leaderboardLoadedEvent.Raise();
    }

}

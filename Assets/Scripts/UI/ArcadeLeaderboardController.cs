using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.PlayFabHelper;
using Utilities.SaveOperations;
using Utilities.PlayFabHelper.CSArguments;
using PlayFab.ClientModels;
using Utilities.Events;
using Utilities;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Linq;

public class ArcadeLeaderboardController : MonoBehaviour
{
    [SerializeField]
    Transform _content;

    [SerializeField]
    GameObject _leaderboardEntryPrefab;

    [SerializeField]
    GameEvent leaderboardLoadedEvent;

    Leaderboard _playerLeaderboard;
    List<string> playFabIDs = new List<string>();

    public void MoveToOpeningScene()
    {
        HelperFunctions.LoadScene(ProjectSpecificGlobals.SceneNames.ArcadeOpeningScene);
    }

    // Start is called before the first frame update
    void Start()
    {
        var loadingLeaderboardBox = MessageBoxFactory.Create(MessageBoxType.Loading, "Please Wait", "Loading Arcade Leaderboard", leaderboardLoadedEvent);
        loadingLeaderboardBox.DisplayLoadingMessageBox(loadingLeaderboardBox.AutoDestroyMessageBox);
        //DisplayArcadeLeaderboard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    
    
}

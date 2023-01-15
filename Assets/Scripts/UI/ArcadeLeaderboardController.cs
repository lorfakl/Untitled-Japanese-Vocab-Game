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
        if(PlayFabController.IsAuthenticated)
        {
            SetUpLeaderboard();
        }
        else
        {
            PlayFabController.IsAuthedEvent += SetUpLeaderboard;
        }
        HelperFunctions.Log("ArcadeLeaderboardStart");
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private async Task ConvertToTitlePlayerIDs(Leaderboard leaderboardData)
    {
        _playerLeaderboard = leaderboardData;
        foreach (var entry in leaderboardData.Entries)
        {
            playFabIDs.Add(entry.playfabID);
        }

        if(playFabIDs.Count > 0)
        {
            PlayFabController.GetTitlePlayerAccounts(playFabIDs, GetAvatarPhotos);
        }
        
        

    }

    void GetAvatarPhotos(Dictionary<string, UniversalEntityKey> convertedIDsDict)
    {
        List<UniversalEntityKey> universalEntities = new List<UniversalEntityKey>();
        foreach (KeyValuePair<string, UniversalEntityKey> pair in convertedIDsDict)
        {
            universalEntities.Add(pair.Value);
        }

        var getRivalArg = new GetRivalAvatarsCSArgument
        {
            Rivals = universalEntities
        };
        PlayFabController.ExecutionCSFunction(CSFunctionNames.GetRivalAvatars, getRivalArg, ProcessAvatarImages);
    }

    void ProcessAvatarImages(PlayFab.CloudScriptModels.ExecuteFunctionResult leaderboardEntries)
    {
        HelperFunctions.Log(leaderboardEntries.FunctionResult);
        List<GetRivalAvatarResult> rivalAvatarFiles = JsonConvert.DeserializeObject<List<GetRivalAvatarResult>>(leaderboardEntries.FunctionResult.ToString());
        Dictionary<string, Task<Sprite>> IDtoSpriteDict = new Dictionary<string, Task<Sprite>>();
        for(int i = 0; i < rivalAvatarFiles.Count; i++)
        {
            if(i < playFabIDs.Count)
            {
                IDtoSpriteDict.Add(playFabIDs[i], PlayFabController.PerformDownload(rivalAvatarFiles[i].URL, SaveSystem.ConvertBytesToSprite));

            }

        }

        /*Task.WaitAll(IDtoSpriteDict.Values.ToArray());
        foreach(var id in IDtoSpriteDict.Keys)
        {
            _playerLeaderboard[id].AssignSprite(IDtoSpriteDict[id].Result);
        }*/
        
    }

    void GetSpriteFromImageData(byte[] imageData)
    {
        SaveSystem.ConvertBytesToSprite(imageData);
    }

    void SetUpLeaderboard()
    {
        _playerLeaderboard = LeaderboardManager.CreateLeaderboard(StatisticName.ArcadeScore, _leaderboardEntryPrefab, _content, ConvertToTitlePlayerIDs);
    }
    
}

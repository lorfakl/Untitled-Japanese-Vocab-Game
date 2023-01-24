using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utilities;
using Utilities.PlayFabHelper;

public class LeaderboardEntryController : MonoBehaviour
{
    [SerializeField]
    Image rankIcon;
    [SerializeField]
    Sprite[] possibleRankIcons;

    [SerializeField]
    Image avatarPortrait;
    [SerializeField]
    TMP_Text displayName;
    [SerializeField]
    TMP_Text score;
    [SerializeField]
    TMP_Text rankText;

    LeaderboardEntry entry;
    // Start is called before the first frame update
    Leaderboard host;

    public void SetLeaderboardHost(Leaderboard h)
    {
        host = h;
        entry = host.GetLeaderboardEntry(this);
        ConfigureLeaderboardEntry();
    }

    private void Awake()
    {
        /*HelperFunctions.Error("NORMAL STATS PAGE IS BROKEN IMPLEMENT AN ABSTRACT LEADERBOARD HANDLER CLASS");
        entry = ArcadeLeaderboardController.GetLeaderboardEntry(this);
        entry.Print();*/
    }

    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ConfigureLeaderboardEntry()
    {
        if (String.IsNullOrEmpty(entry.displayName))
        {
            displayName.text = entry.playfabID;
        }
        else
        {
            displayName.text = entry.displayName;
        }

        score.text = entry.score.ToString();
        rankText.text = entry.rank.ToString();
        avatarPortrait.sprite = entry.avatarPhotoSprite;
    }
}

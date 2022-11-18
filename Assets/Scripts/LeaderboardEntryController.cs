using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
    private void Awake()
    {
        entry = StatPageManager.GetLeaderboardEntry(this);
        entry.Print();
    }
    void Start()
    {
        if(String.IsNullOrEmpty(entry.displayName))
        {
            displayName.text = entry.playfabID;
        }
        else
        {
            displayName.text = entry.displayName;
        }

        score.text = entry.score;
        rankText.text = entry.rank;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

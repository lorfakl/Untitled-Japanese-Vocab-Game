using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utilities;
using Utilities.PlayFabHelper;

public class LeaderboardEntryController : MonoBehaviour
{
    List<int> ranksWithAnimations = new List<int>{ 1, 2, 3 };
    Dictionary<int, RuntimeAnimatorController> animators = new Dictionary<int, RuntimeAnimatorController>();   
    
    [SerializeField]
    Image rankIcon;

    [SerializeField]
    Image avatarPortrait;
    [SerializeField]
    TMP_Text displayName;
    [SerializeField]
    TMP_Text score;
    [SerializeField]
    TMP_Text rankText;

    [SerializeField]
    Animator animator;

    [SerializeField]
    RuntimeAnimatorController[] runtimeAnimatorControllers;


    LeaderboardEntry entry;
    // BECAUSE THIS OBJ IS DISABLED WHEN INSTANTIATED THE AWKAE AND START CALLS ARE NOT FUN UNTIL THE OBJ/COMPONENT IS ENABLED
    Leaderboard host;

    public void SetLeaderboardHost(Leaderboard h)
    {
        host = h;
        entry = host.GetLeaderboardEntry(this);
        ConfigureLeaderboardEntry();
    }

    private void Awake()
    {
        //HelperFunctions.Error("NORMAL STATS PAGE IS BROKEN IMPLEMENT AN ABSTRACT LEADERBOARD HANDLER CLASS");
        //throw new Exception("What the fuck");
        /*entry = ArcadeLeaderboardController.GetLeaderboardEntry(this);
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
        foreach (int r in ranksWithAnimations)
        {
            animators.Add(r, runtimeAnimatorControllers[ranksWithAnimations.IndexOf(r)]);
        }

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
        if(animators.ContainsKey(entry.rank))
        {
            animator.runtimeAnimatorController = animators[entry.rank];
            rankText.enabled = false;
        }
        else
        {
            rankIcon.enabled = false;
        }
        avatarPortrait.sprite = entry.avatarPhotoSprite;
    }
}

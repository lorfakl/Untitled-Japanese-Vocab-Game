using PlayFab.ProfilesModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.PlayFabHelper
{
    public enum EntityTypes
    {
        title_player_account,
        group,
        character,
        master_player_account,
        title,
    }

    public enum CustomEventNames
    {

    }

    public enum UserDataKey
    {
        LeitnerLevels,
        PrestigeLevels,
        SessionWords,
        LoginCount,
        NextSession,
        WordsSeen
    }

    public enum TitleDataKeys
    {
        StarterWords,
        CommonWords
    }

    public enum StatisticName
    {
        LeagueSP,
        MonthlySP,
        WordsSeen,
        WordsMastered,
        StudyStreak,
        TotalSP
    }

    public enum VirtualCurrency
    {
        SP
    }

    public enum CSFunctionNames
    {
        TestingFunc,
        FirstTimeWordSetup,
        BuildSessionList,
        SetLoginStatus,
        AddNewWords,
        UpdateTag,
        Record,
        GetProfile,
        AddMembers,
        ModifyTag, 
        GetRivalAvatars,
        CompletePurchase
    }

    public enum PlayerTags
    {
        InGroup,
        HasPlayed,
        HasPlayedThisWeek,
        HasPlayedThisMonth
    }

    public struct ProfileStruct
    {
        public string displayName;
        public string rank;
        public string playfabID;
        public Dictionary<string, EntityStatisticValue> statistics;
        public string avatarURL;

        public void Print()
        {
            HelperFunctions.Log("DisplayName: " + this.displayName + "\n" +
                "Rank: " + this.rank + "\n" +
                "PlayFabID: " + this.playfabID + "\n" +
                "Score: " + this.statistics + "\n" +
                "AvatarURL: " + this.avatarURL);
        }
    }

}

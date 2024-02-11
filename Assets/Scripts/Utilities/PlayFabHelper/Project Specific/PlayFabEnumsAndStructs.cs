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
        SessionKana,
        LoginCount,
        NextSession,
        WordsSeen
    }

    public enum TitleDataKeys
    {
        StarterWords,
        StarterKana,
        CommonWords,
        Kana,
        TestWords,
        TestKana,
        ClientConfiguration,

    }

    public enum StatisticName
    {
        LeagueSP,
        MonthlySP,
        WordsSeen,
        WordsMastered,
        StudyStreak,
        TotalSP,
        ArcadeScore
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
        CompletePurchase,
        GetOtherStatistics
    }

    public enum PlayerTags
    {
        InGroup,
        HasPlayed,
        HasPlayedThisWeek,
        HasPlayedThisMonth
    }

    

}

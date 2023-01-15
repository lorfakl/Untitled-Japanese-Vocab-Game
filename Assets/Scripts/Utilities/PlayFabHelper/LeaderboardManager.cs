using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Utilities.PlayFabHelper;

namespace Utilities.PlayFabHelper
{
    public static class LeaderboardManager
    {
        public static Dictionary<StatisticName, Leaderboard> leaderboards = new Dictionary<StatisticName, Leaderboard>();

        public static Leaderboard CreateLeaderboard(StatisticName s, GameObject entryPrefab, Transform parent, Func<Leaderboard, Task> successCallback, bool isAroundUser = false)
        {
            if(GetLeaderboard(s) != default)
            {
                return GetLeaderboard(s);
            }

            Leaderboard leaderboard = new Leaderboard(s, entryPrefab, parent, successCallback, isAroundUser);
            leaderboards.Add(s, leaderboard);
            return leaderboard;
        }

        public static Leaderboard GetLeaderboard(StatisticName name)
        {
            if (leaderboards.ContainsKey(name))
            {
                return leaderboards[name];
            }
            else
            {
                return default;
            }
        }
    }
}

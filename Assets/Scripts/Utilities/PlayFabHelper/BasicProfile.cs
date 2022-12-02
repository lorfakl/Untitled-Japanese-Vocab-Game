using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.PlayFabHelper
{
    [Serializable]
    public class BasicProfile
    {
        [JsonProperty("avatarURL")]
        public string AvatarURL { get; private set; }

        [JsonProperty("playfabID")]
        public string PlayFabID { get; private set; }

        [JsonProperty("statistics")]
        public Dictionary<StatisticName, CloudScriptStatArgument> Statistics { get; private set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; private set; }

        public BasicProfile() { }

        public BasicProfile(string avatarURL, string playfabID, string displayName, Dictionary<StatisticName, CloudScriptStatArgument> stats)
        {
            AvatarURL = avatarURL;
            PlayFabID = playfabID;
            DisplayName = displayName;
            Statistics = stats;
        }

        [JsonConstructor]
        public BasicProfile(string avatarURL, string playfabID, string displayName, List<PlayFab.ClientModels.StatisticModel> stats)
        {
            AvatarURL = avatarURL;
            PlayFabID = playfabID;
            DisplayName = displayName;
            if (stats.Count > 0)
            {
                Statistics = new Dictionary<StatisticName, CloudScriptStatArgument>();
                Dictionary<StatisticName, int> statVersion = new Dictionary<StatisticName, int>();
                try
                {
                    foreach (PlayFab.ClientModels.StatisticModel s in stats)
                    {
                        StatisticName sn = HelperFunctions.ParseEnum<StatisticName>(s.Name);
                        if (statVersion.ContainsKey(sn) && Statistics.ContainsKey(sn))
                        {
                            if (s.Version > statVersion[sn])
                            {
                                statVersion[sn] = s.Version;
                                Statistics[sn].value = s.Value.ToString();

                            }
                        }
                        else
                        {
                            statVersion.Add(sn, s.Version);
                            Statistics.Add(sn, new CloudScriptStatArgument(HelperFunctions.ParseEnum<StatisticName>(s.Name), s.Value));
                        }
                    }
                }
                catch (Exception e)
                {
                    HelperFunctions.CatchException(e);
                }

            }

        }

        public override string ToString()
        {
            return HelperFunctions.PrintObjectProperties(this);
        }
    }
}
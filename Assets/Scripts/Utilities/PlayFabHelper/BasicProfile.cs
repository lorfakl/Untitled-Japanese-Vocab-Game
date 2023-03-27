using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using Utilities.PlayFabHelper.CSArguments;

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

        public BasicProfile(GetOtherPlayerStatisticsResult res, StatisticName s)
        {
            this.PlayFabID = res.ID;
            this.Statistics = new Dictionary<StatisticName, CloudScriptStatArgument>();
            this.Statistics.Add(s, new CloudScriptStatArgument(s, res.Value));
            if(String.IsNullOrEmpty(res.DisplayName))
            {
                this.DisplayName = PlayFabID;
            }
            else
            {
                this.DisplayName = res.DisplayName;
            }
            
        }
        public BasicProfile(PlayFab.ClientModels.PlayerProfileModel p)
        {
            if(p == null)
            {
                PlayFabID = Playfab.PlayFabID;
                DisplayName = Playfab.DisplayName;
                return;
            }

            PlayFabID = p.PlayerId;
            AvatarURL = p.AvatarUrl;
            DisplayName = p.DisplayName;
            ParsePFStatisticModel(p.Statistics);
        }

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
            ParsePFStatisticModel(stats);

        }

        private void ParsePFStatisticModel(List<PlayFab.ClientModels.StatisticModel> stats)
        {
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
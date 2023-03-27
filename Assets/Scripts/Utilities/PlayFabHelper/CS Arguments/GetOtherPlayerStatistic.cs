using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Utilities.PlayFabHelper.CSArguments
{
    public class GetOtherPlayerStatisticArgument
    {
        [JsonProperty("OtherPlayers")]
        public List<string> playFabIDs { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("StatisticName")]
        public StatisticName StatisticName { get; set; }
    }

    public class GetOtherPlayerStatisticsResult
    {
        [JsonProperty("Rank")]
        public int Rank { get; set; }

        [JsonProperty("ID")]
        public string ID { get; set; }

        [JsonProperty("Value")]
        public int Value { get; set; }

        [JsonProperty("DisplayName")]
        public string DisplayName { get; set; }



    }

}


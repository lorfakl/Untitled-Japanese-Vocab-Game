using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Utilities.PlayFabHelper.CSArguments
{
    [Serializable]
    public class CompletePurchaseArg
    {
        [JsonProperty("itemIds")]
        public List<string> ItemIDs { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("vc")]
        public string VC { get; set; }

        public CompletePurchaseArg()
        {

        }
    }
}


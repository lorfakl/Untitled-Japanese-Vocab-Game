using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Utilities.PlayFabHelper.CSArguments
{
    public class ProfileCSArgument
    {
        /// <summary>
        /// C# Class for the argument for the GetProfile CS Function
        /// </summary>
        [JsonProperty("PlayFabIDs")]
        public List<string> PlayFabIDs { get; set; }

        [JsonProperty("ProfileConstraints")]
        public PlayerProfileViewConstraints ProfileConstraints { get; set; }
    }
}

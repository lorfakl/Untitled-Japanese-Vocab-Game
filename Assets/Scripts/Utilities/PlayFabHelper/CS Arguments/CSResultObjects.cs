using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

namespace Utilities.PlayFabHelper.CSArguments
{
    public class GetRivalAvatarResult
    {
        [JsonProperty("DownloadUrl")]
        public string URL { get; set; }

        [JsonProperty("FileName")]
        public string FileName { get; set; }
        
        [JsonProperty("LastModified")]
        public string LastModified { get; set; }

        [JsonProperty("Size")]
        public string Size { get; set; }
    }
}

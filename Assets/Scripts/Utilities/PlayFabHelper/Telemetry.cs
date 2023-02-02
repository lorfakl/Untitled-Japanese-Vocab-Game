using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

namespace Utilities.PlayFabHelper
{
    public class TelemetryWrapper
    {
        [JsonProperty("EventNamespace")]
        public EventNamespace Namespace { get; set; }

        [JsonProperty("Name")]
        public EventName EventName { get; set; }

        [JsonProperty("PayloadJSON")]
        public string PayloadJSON { get; set; }

        [JsonProperty("Entity")]
        public UniversalEntityKey Entity { get; set; }


        public static implicit operator PlayFab.EventsModels.EventContents(TelemetryWrapper k)
        {
            return new PlayFab.EventsModels.EventContents
            { 
                Entity = k.Entity,
                PayloadJSON = k.PayloadJSON,
                EventNamespace = "custom." + k.Namespace.ToString(),
                Name = k.EventName.ToString(),
                OriginalId = k.GetHashCode().ToString(),
                OriginalTimestamp = DateTime.UtcNow
            };
        }

    }
}


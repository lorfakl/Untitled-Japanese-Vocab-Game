using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using Utilities.Logging;

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

        public DateTime TimeStamp { get; private set; }

        public TelemetryWrapper() 
        {
            TimeStamp = DateTime.Now;
        }

        public TelemetryWrapper(EventNamespace @namespace, EventName eventName, string payloadJSON)
        {
            Namespace = @namespace;
            EventName = eventName;
            PayloadJSON = payloadJSON;
            Entity = Playfab.UserEntityKey;
            TimeStamp = DateTime.UtcNow;
        }

        public static implicit operator PlayFab.EventsModels.EventContents(TelemetryWrapper k)
        {
            return new PlayFab.EventsModels.EventContents
            { 
                Entity = k.Entity,
                PayloadJSON = k.PayloadJSON,
                EventNamespace = "custom." + k.Namespace.ToString(),
                Name = k.EventName.ToString(),
                OriginalId = Guid.NewGuid().ToString("N"),
                OriginalTimestamp = k.TimeStamp
            };
        }

    }
}


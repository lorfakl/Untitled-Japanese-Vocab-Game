using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Utilities.PlayFabHelper.CSArguments
{
    public class AddWordArgument
    {
        [JsonProperty("NumToAdd")]
        public int NumToAdd { get; set; }

        //Progress is the number of words that have been added to the user's list so far so if 70 words have been added then the value is 70 
        //their progress through the 1000 words
        [JsonProperty("Progess")]
        public int Progress { get; set; }

        [JsonProperty("IsKanjiStudyTopic")]
        public bool IsKanjiStudyTopic { get; set; }
    }
}


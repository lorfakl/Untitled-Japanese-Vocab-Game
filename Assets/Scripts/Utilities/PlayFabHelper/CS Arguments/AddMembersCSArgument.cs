using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Utilities.PlayFabHelper.CSArguments
{
    public class AddMembersCSArgument
    {
        [JsonProperty("MemberKeys")]
        public List<string> MemberKeys { get; set; }

        [JsonProperty("GroupID")]
        public string GroupID { get; set; }
    }

    public class GetRivalAvatarsCSArgument
    {
        [JsonProperty("Rivals")]
        public List<UniversalEntityKey> Rivals { get; set; }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public enum AvatarItemLocation
{
    Head,
    Top,
    Bottom,
    Skin,
    Face
}

[Serializable]
public class AvatarData
{
    string _avatarJsonString = "";
    Dictionary<AvatarItemLocation, string> _avatarDataMap = new Dictionary<AvatarItemLocation, string>();

    [JsonProperty]
    public string Head{ get; private set; }

    [JsonProperty]
    public string Top { get; private set; }
    
    [JsonProperty]
    public string Bottom { get; private set; }
    
    [JsonProperty]
    public string Face { get; private set; }
    
    [JsonProperty]
    public string Skin { get; private set; }

    public AvatarData(Dictionary<AvatarItemLocation, string> avatarDataMap)
    {
        _avatarDataMap = avatarDataMap;
        Head = avatarDataMap[AvatarItemLocation.Head];
        Top = avatarDataMap[AvatarItemLocation.Top];
        Bottom = avatarDataMap[AvatarItemLocation.Bottom];
        Face = avatarDataMap[AvatarItemLocation.Face];
        Skin = avatarDataMap[AvatarItemLocation.Skin];
        _avatarJsonString = JsonConvert.SerializeObject(_avatarDataMap);
    }

    [JsonConstructor]
    public AvatarData()
    {

    }


}

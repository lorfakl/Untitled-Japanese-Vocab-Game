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
    Base
}

[Serializable]
public class AvatarData
{
    string _avatarJsonString = "";
    Dictionary<AvatarData, string> _avatarDataDict = new Dictionary<AvatarData, string>();

    
}

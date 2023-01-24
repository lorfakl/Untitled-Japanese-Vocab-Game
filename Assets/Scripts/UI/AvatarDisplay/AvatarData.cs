using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Utilities;
using Utilities.SaveOperations;

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
    string _spritePath = "Sprites/Fashion/New_Fashion";

    [NonSerialized]
    Sprite[] _spriteArray;

    [NonSerialized]
    Sprite _img;

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

    public byte[] ImageData { get; set; }

    public Sprite AvatarPhoto
    {
        get { return _img; }
    }

    public AvatarData(Dictionary<AvatarItemLocation, string> avatarDataMap, byte[] avatarPhoto)
    {
        if(avatarDataMap == null)
        {
            HelperFunctions.Log("Data map is null somehow");
        }
        else if(avatarPhoto == null)
        {
            HelperFunctions.Log("AvatarPhoto is null makes more sense");
        }
        _avatarDataMap = avatarDataMap;
        Head = avatarDataMap[AvatarItemLocation.Head];
        Top = avatarDataMap[AvatarItemLocation.Top];
        Bottom = avatarDataMap[AvatarItemLocation.Bottom];
        Face = avatarDataMap[AvatarItemLocation.Face];
        Skin = avatarDataMap[AvatarItemLocation.Skin];
        _avatarJsonString = JsonConvert.SerializeObject(_avatarDataMap);
        Sprite img = SaveSystem.ConvertBytesToSprite(avatarPhoto).GetAwaiter().GetResult();
        _img = img;
    }

    [JsonConstructor]
    public AvatarData()
    {

    }

    public void AssignSpriteValues(SpriteRenderer s, SpriteRenderer h, SpriteRenderer t, SpriteRenderer b, SpriteRenderer f = null)
    {
        HelperFunctions.Warning("Currently Only looks in the Fashion Folder");
        
        HelperFunctions.Log("Proper resources path: " + _spritePath);
        _spriteArray = Resources.LoadAll<Sprite>(_spritePath);

        foreach(Sprite sp in _spriteArray)
        {
            //HelperFunctions.Log(sp.name);
            if(sp.name == _avatarDataMap[AvatarItemLocation.Head])
            {
                h.sprite = sp;
            }
            else if(sp.name == _avatarDataMap[AvatarItemLocation.Top])
            {
                t.sprite = sp;
            }
            else if (sp.name == _avatarDataMap[AvatarItemLocation.Face] && f != null)
            {
                f.sprite = sp;
            }
            else if (sp.name == _avatarDataMap[AvatarItemLocation.Skin])
            {
                s.sprite = sp;
            }
            else if (sp.name == _avatarDataMap[AvatarItemLocation.Bottom])
            {
                b.sprite = sp;
            }
        }

    }


}

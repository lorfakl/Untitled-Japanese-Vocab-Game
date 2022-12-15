using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.PlayFabHelper.CurrentUser
{
    public static class CurrentAuthedPlayer
    {
        public static PlayFabUser CurrentUser
        {
            get;
            private set;
        }

        public static void SetCurrentUser(PlayFabUser u)
        {
            CurrentUser = u;
        }
    }

    [Serializable]
    public class PlayFabUser
    {
        public BasicProfile Profile
        {
            get { return _profile; }
        }
        BasicProfile _profile;
        
        public PlayFabInventory Inventory
        {
            get { return _inventory; }
        }
        PlayFabInventory _inventory;
        
        public UniversalEntityKey EntityKey
        {
            get { return _entityKey;}
        }
        UniversalEntityKey _entityKey;

        public string PlayFabID
        {
            get { return _playFabID; }
        }
        string _playFabID;
        

        public List<PlayFabGroup> Groups
        {
            get { return _groups; }
        }
        List<PlayFabGroup> _groups = new List<PlayFabGroup>();
        

        public List<PlayerTags> Tags
        {
            get { return _tags; }
        }
        List<PlayerTags> _tags = new List<PlayerTags>();

        public PlayFabUser(string id, List<PlayFab.ClientModels.TagModel> t, UniversalEntityKey e, BasicProfile b, PlayFabInventory i)
        {
            _entityKey = e;
            _inventory = i;

            foreach (PlayerTags et in Enum.GetValues(typeof(PlayerTags)))
            {
                foreach (var tag in t)
                {
                    if (tag.TagValue.Contains(et.ToString()))
                    {
                        int tagStartIndex = tag.TagValue.IndexOf(et.ToString()[0]);
                        string tagName = tag.TagValue.Substring(tagStartIndex);
                        _tags.Add(HelperFunctions.ParseEnum<PlayerTags>(tagName));
                    }
                } 
            }
            _profile = b;
            _playFabID = id;

        }
    
        public void UpdateGroup(PlayFabGroup g)
        {
            _groups.Insert(0, g);
        }
    }
}


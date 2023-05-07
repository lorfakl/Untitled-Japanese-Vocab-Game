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

        public static UserSettingsData UserSettings
        {
            get;
            set;
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

        public AvatarData Avatar
        {
            get { return _avatarData; }
        }
        AvatarData _avatarData;
        
        public UserSettingsData UserSettings
        {
            get { return _userSettings; }
        }
        UserSettingsData _userSettings;

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
        

        public PlayFabGroup Group
        {
            get { return _groups; }
        }
        PlayFabGroup _groups = new PlayFabGroup();
        

        public List<PlayerTags> Tags
        {
            get { return _tags; }
        }
        List<PlayerTags> _tags = new List<PlayerTags>();

        public int WordsSeen
        {
            get;
            private set;
        }
        

        public PlayFabUser(string id, List<PlayFab.ClientModels.TagModel> t, UniversalEntityKey e, BasicProfile b, PlayFabInventory i, int wordsSeen)
        {
            _entityKey = e;
            _inventory = i;

            if (t != null)
            {
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
            }
            _profile = b;
            _playFabID = id;
            WordsSeen = wordsSeen;

        }
    
        public void UpdateAvatar(AvatarData data)
        {
            _avatarData = data;
        }

        public void UpdateGroup(PlayFabGroup g)
        {
            _groups = g;
        }

        
    }
}


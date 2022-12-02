using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.PlayFabHelper
{
    [Serializable]
    public class PlayFabGroup
    {
        Dictionary<string, BasicProfile> _members = new Dictionary<string, BasicProfile>();

        public Dictionary<string, BasicProfile> Members
        {
            get { return _members; }
        }

        UniversalEntityKey _entityKey;

        public UniversalEntityKey EntityKey
        {
            get { return _entityKey; }
        }

        string _name;

        public string Name
        {
            get { return _name; }
        }
        
        public PlayFabGroup()
        {

        }

        public PlayFabGroup(List<BasicProfile> members)
        {
            foreach (BasicProfile member in members)
            {
                _members.Add(member.PlayFabID, member);
            }
        }

        public PlayFabGroup(UniversalEntityKey entityKey, string name)
        {
            this._entityKey = entityKey;
            this._name = name;
        }

        public PlayFabGroup(UniversalEntityKey entityKey, List<BasicProfile> members)
        {
            foreach (BasicProfile member in members)
            {
                _members.Add(member.PlayFabID, member);
            }

            this._entityKey = entityKey;
        }
    }
}
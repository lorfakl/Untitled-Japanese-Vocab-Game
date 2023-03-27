using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities.PlayFabHelper.CurrentUser;

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

        public List<BasicProfile> MembersList
        {
            get { return _members.Values.ToList(); }
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
            ModifyMemberDict(members);
        }

        public PlayFabGroup(UniversalEntityKey entityKey, string name)
        {
            this._entityKey = entityKey;
            this._name = name;
        }

        public PlayFabGroup(UniversalEntityKey entityKey, List<BasicProfile> members)
        {
            ModifyMemberDict(members);

            this._entityKey = entityKey;
        }

        public void Add(List<BasicProfile> members)
        {
            this.ModifyMemberDict(members);
        }

        public void OverWriteMembers(List<BasicProfile> members)
        {
            _members.Clear();
            foreach(var b in members)
            {
                _members.Add(b.PlayFabID, b);
            }
        }

        public List<string> GetMemberIDs()
        {
            List<string> IDs = new List<string>();

            foreach (var member in MembersList)
            {
                IDs.Add(member.PlayFabID);
            }

            return IDs;
        }

    
        private void ModifyMemberDict(List<BasicProfile> m)
        {
            foreach (BasicProfile member in m)
            {
                _members.Add(member.PlayFabID, member);
            }
        }
    }
}
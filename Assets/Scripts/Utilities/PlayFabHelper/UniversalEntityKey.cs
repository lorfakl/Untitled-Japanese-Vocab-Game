using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.PlayFabHelper
{
    [Serializable]
    public class UniversalEntityKey
    {
        public string ID
        {
            get;
            set;
        }
        public string Type
        {
            get;
            set;
        }

        public UniversalEntityKey()
        {

        }

        public UniversalEntityKey(string iD, string type)
        {
            ID = iD;
            Type = type;
        }

        public static implicit operator PlayFab.AuthenticationModels.EntityKey(UniversalEntityKey k)
        {
            return new PlayFab.AuthenticationModels.EntityKey
            {
                Id = k.ID,
                Type = k.Type
            };
        }
        public static implicit operator PlayFab.ClientModels.EntityKey(UniversalEntityKey k)
        {
            return new PlayFab.ClientModels.EntityKey
            {
                Id = k.ID,
                Type = k.Type
            };
        }
        public static implicit operator PlayFab.CloudScriptModels.EntityKey(UniversalEntityKey k)
        {
            return new PlayFab.CloudScriptModels.EntityKey
            {
                Id = k.ID,
                Type = k.Type
            };
        }
        public static implicit operator PlayFab.DataModels.EntityKey(UniversalEntityKey k)
        {
            return new PlayFab.DataModels.EntityKey
            {
                Id = k.ID,
                Type = k.Type
            };
        }
        public static implicit operator PlayFab.GroupsModels.EntityKey(UniversalEntityKey k)
        {
            return new PlayFab.GroupsModels.EntityKey
            {
                Id = k.ID,
                Type = k.Type
            };
        }
        public static implicit operator PlayFab.EconomyModels.EntityKey(UniversalEntityKey k)
        {
            return new PlayFab.EconomyModels.EntityKey
            {
                Id = k.ID,
                Type = k.Type
            };
        }
        public static implicit operator PlayFab.EventsModels.EntityKey(UniversalEntityKey k)
        {
            return new PlayFab.EventsModels.EntityKey
            {
                Id = k.ID,
                Type = k.Type
            };
        }
        public static implicit operator PlayFab.ExperimentationModels.EntityKey(UniversalEntityKey k)
        {
            return new PlayFab.ExperimentationModels.EntityKey
            {
                Id = k.ID,
                Type = k.Type
            };
        }
        public static implicit operator PlayFab.MultiplayerModels.EntityKey(UniversalEntityKey k)
        {
            return new PlayFab.MultiplayerModels.EntityKey
            {
                Id = k.ID,
                Type = k.Type
            };
        }
        public static implicit operator PlayFab.ProfilesModels.EntityKey(UniversalEntityKey k)
        {
            return new PlayFab.ProfilesModels.EntityKey
            {
                Id = k.ID,
                Type = k.Type
            };
        }

        public static explicit operator UniversalEntityKey(PlayFab.AuthenticationModels.EntityKey e)
        {
            return new UniversalEntityKey
            {
                ID = e.Id,
                Type = e.Type
            };
        }
        public static explicit operator UniversalEntityKey(PlayFab.ClientModels.EntityKey e)
        {
            return new UniversalEntityKey
            {
                ID = e.Id,
                Type = e.Type
            };
        }
        public static explicit operator UniversalEntityKey(PlayFab.CloudScriptModels.EntityKey e)
        {
            return new UniversalEntityKey
            {
                ID = e.Id,
                Type = e.Type
            };
        }
        public static explicit operator UniversalEntityKey(PlayFab.DataModels.EntityKey e)
        {
            return new UniversalEntityKey
            {
                ID = e.Id,
                Type = e.Type
            };
        }
        public static explicit operator UniversalEntityKey(PlayFab.GroupsModels.EntityKey e)
        {
            return new UniversalEntityKey
            {
                ID = e.Id,
                Type = e.Type
            };
        }
        public static explicit operator UniversalEntityKey(PlayFab.EconomyModels.EntityKey e)
        {
            return new UniversalEntityKey
            {
                ID = e.Id,
                Type = e.Type
            };
        }
        public static explicit operator UniversalEntityKey(PlayFab.EventsModels.EntityKey e)
        {
            return new UniversalEntityKey
            {
                ID = e.Id,
                Type = e.Type
            };
        }
        public static explicit operator UniversalEntityKey(PlayFab.ExperimentationModels.EntityKey e)
        {
            return new UniversalEntityKey
            {
                ID = e.Id,
                Type = e.Type
            };
        }
        public static explicit operator UniversalEntityKey(PlayFab.MultiplayerModels.EntityKey e)
        {
            return new UniversalEntityKey
            {
                ID = e.Id,
                Type = e.Type
            };
        }
        public static explicit operator UniversalEntityKey(PlayFab.ProfilesModels.EntityKey e)
        {
            return new UniversalEntityKey
            {
                ID = e.Id,
                Type = e.Type
            };
        }


    }
}

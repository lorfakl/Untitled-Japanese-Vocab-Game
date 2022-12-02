using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;


namespace Utilities.PlayFabHelper
{
    [Serializable]
    public class PlayFabInventory
    {
        List<PlayFabItem> _inventory = new List<PlayFabItem>();
        public List<PlayFabItem> Inventory
        {
            get { return _inventory; }
        }
        
        string _pfId;
        public string OwningPfId
        {
            get { return _pfId; }
        }

        Dictionary<string, int> _virtualCurrency = new Dictionary<string, int>();
        public Dictionary<string, int> VirtualCurrencies
        {
            get { return _virtualCurrency; }
        }

        public PlayFabInventory(List<PlayFab.ClientModels.ItemInstance> i, string id, Dictionary<string, int> dict)
        {
            _pfId = id;
            _virtualCurrency = dict;
            foreach(PlayFab.ClientModels.ItemInstance item in i)
            {
                _inventory.Add((PlayFabItem)item);
            }
        }
    }
}


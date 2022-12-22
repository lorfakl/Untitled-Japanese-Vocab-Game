using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft;
using Newtonsoft.Json;

namespace Utilities.PlayFabHelper
{
    public enum CustomDataKeys
    {
        Rare
    }

    [Serializable]
    public class PlayFabItem
    {
        string _id;
        public string ID
        {
            get { return _id; }
        }

        string _name;
        public string Name
        {
            get { return _name; }
        }

        string _description;
        public string Description
        {
            get { return _description; }
        }

        string _iconUrl;
        public string IconUrl
        {
            get { return _iconUrl; }
        }
        string _itemClass;
        public string ItemClass
        {
            get { return _itemClass; }
        }

        Dictionary<string, string> _customData = new Dictionary<string, string>();
        public Dictionary<string, string> CustomData
        {
            get { return _customData; }
        }

        List<string> _tags = new List<string>();

        public List<string> Tags
        {
            get { return _tags; }
        }

        Dictionary<string, uint> _pricesDict = new Dictionary<string, uint>();
        public Dictionary<string, uint> Prices
        {
            get { return _pricesDict; }
        }

        bool _isLimited;
        public bool IsLimited
        {
            get { return _isLimited; }
        }

        int _limitCount;
        public int LimitCount
        {
            get { return _limitCount; }
        }

        bool _isTradable;
        public bool IsTradable
        {
            get { return _isTradable; }
        }

        [NonSerialized]
        Sprite _imageReference;
        
        public Sprite Sprite
        {
            get { return _imageReference; }
        }

        string _catalogVersion;
        public string CatalogVersion
        {
            get { return _catalogVersion; }
        }

        public static implicit operator PlayFab.ClientModels.ItemInstance(PlayFabItem i)
        {
            return new PlayFab.ClientModels.ItemInstance
            {
                CatalogVersion = i.CatalogVersion,
                DisplayName = i.Name,
                ItemClass = i.ItemClass,
                ItemId = i.ID,
                CustomData = i.CustomData
            };
        }

        public static implicit operator PlayFab.ClientModels.CatalogItem(PlayFabItem i)
        {
            string c = "";

            if (i.CustomData.Count > 0)
            {
                c = JsonConvert.SerializeObject(i.CustomData);
            }

            return new PlayFab.ClientModels.CatalogItem
            {
                CatalogVersion = i.CatalogVersion,
                DisplayName = i.Name,
                Description = i.Description,
                ItemClass = i.ItemClass,
                ItemId = i.ID,
                IsLimitedEdition = i.IsLimited,
                IsTradable = i.IsTradable,
                InitialLimitedEditionCount = i.LimitCount,
                CustomData = c,
                Tags = i.Tags,

            };
        }

        public static explicit operator PlayFabItem(PlayFab.ClientModels.ItemInstance i)
        {
            return new PlayFabItem
            {
                _catalogVersion = i.CatalogVersion,
                _name = i.DisplayName,
                _itemClass = i.ItemClass,
                _id = i.ItemId,
                _customData = i.CustomData
            };
        }

        public static explicit operator PlayFabItem(PlayFab.ClientModels.CatalogItem i)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();  
            
            if(!String.IsNullOrEmpty(i.CustomData))
            {
                dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(i.CustomData);
            }

            return new PlayFabItem
            {
                _catalogVersion = i.CatalogVersion,
                _description = i.Description,
                _name = i.DisplayName,
                _itemClass = i.ItemClass,
                _id = i.ItemId,
                _isLimited = i.IsLimitedEdition,
                _isTradable = i.IsTradable,
                _limitCount = i.InitialLimitedEditionCount,
                _pricesDict = i.VirtualCurrencyPrices,
                _customData = dic,
                _tags = i.Tags,
                _iconUrl = i.ItemImageUrl
            };
        }

        public void SetSprite(Sprite s)
        {
            _imageReference = s;
        }

        public override string ToString()
        {
            return $"ID: {ID} \n" +
                $"Name: {Name} \n" +
                $"ItemClass: {ItemClass} \n" +
                $"Tags: {HelperFunctions.LogListContent(Tags)} \n" +
                $"Image Location: {_iconUrl} \n";
        }
    }
}

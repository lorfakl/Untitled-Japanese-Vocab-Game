using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.PlayFabHelper
{
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

        string _customData;
        public string CustomData
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
                ItemId = i.ID
            };
        }

        public static implicit operator PlayFab.ClientModels.CatalogItem(PlayFabItem i)
        {
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
                CustomData = i.CustomData,
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
                _id = i.ItemId
            };
        }

        public static explicit operator PlayFabItem(PlayFab.ClientModels.CatalogItem i)
        {
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
                _customData = i.CustomData,
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

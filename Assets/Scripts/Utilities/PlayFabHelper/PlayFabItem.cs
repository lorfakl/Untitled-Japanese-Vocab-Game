using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft;
using Newtonsoft.Json;
using Unity.VisualScripting;
using static UnityEngine.Rendering.DebugUI;

namespace Utilities.PlayFabHelper
{
    public enum CustomDataKeys
    {
        Rare
    }

    public delegate void ItemSelectionStatus(bool isSelected);

    [Serializable]
    public class PlayFabItem : IEquatable<PlayFabItem>
    {
        readonly Guid _internalID;
        readonly string _id;
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

        [field: NonSerialized]
        public event ItemSelectionStatus OnSelectionChangeEvent;

        public PlayFabItem()
        {

        }

        public PlayFabItem(string id, string name, string description, string iconUrl, string itemClass, Dictionary<string, string> customData, List<string> tags, Dictionary<string, uint> pricesDict, bool isLimited, int limitCount, bool isTradable, string catalogVersion)
        {
            _id = id;
            _name = name;
            _description = description;
            _iconUrl = iconUrl;
            _itemClass = itemClass;
            _customData = customData;
            _tags = tags;
            _pricesDict = pricesDict;
            _isLimited = isLimited;
            _limitCount = limitCount;
            _isTradable = isTradable;
            _catalogVersion = catalogVersion;
        }

        public PlayFabItem(string id, string name, string itemClass, Dictionary<string, string> customData, string catalogVersion)
        {
            _id = id;
            _name = name;
            _itemClass = itemClass;
            _customData = customData;
            _catalogVersion = catalogVersion;
            _internalID = Guid.NewGuid();

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
            
            return new PlayFabItem(i.ItemId, i.DisplayName, i.ItemClass, i.CustomData, i.CatalogVersion);
        }

        public static explicit operator PlayFabItem(PlayFab.ClientModels.CatalogItem i)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();  
            
            if(!String.IsNullOrEmpty(i.CustomData))
            {
                dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(i.CustomData);
            }

            return new PlayFabItem(i.ItemId, i.DisplayName, i.Description, i.ItemImageUrl, i.ItemClass, dic, i.Tags, i.VirtualCurrencyPrices, i.IsLimitedEdition, i.InitialLimitedEditionCount, i.IsTradable, i.CatalogVersion);
        }

        public void SetSelectionStatus(bool selectionState)
        {
            OnSelectionChangeEvent?.Invoke(selectionState);
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
                $"Tags: {HelperFunctions.PrintListContent(Tags)} \n" +
                $"Image Location: {_iconUrl} \n";
        }

        public bool Equals(PlayFabItem other)
        {
            if(this.ID == other.ID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Equals(object obj) => Equals(obj as PlayFabItem);
        public override int GetHashCode()
        {
            int hash = 17;
            hash = (hash * 7) + this.ID.GetHashCode();
            //hash = (hash * 7) + this._internalID.GetHashCode();
            return hash;
        }
    }
}

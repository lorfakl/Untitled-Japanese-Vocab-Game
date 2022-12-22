using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.PlayFabHelper;
using Utilities;
using Utilities.Events;

public class GridItemDisplayController
{
    GameObject _itemDisplayContainerPrefab;
  
    Transform _containerContentParent;
   
    GameObject _itemThumbnailPrefab;

    Dictionary<string, Transform> _thumbnailParents = new Dictionary<string, Transform>();

    Dictionary<string, List<PlayFabItem>> _itemCategories = new Dictionary<string, List<PlayFabItem>>();


    public GridItemDisplayController(Dictionary<string, Transform> thumbnailParents, Dictionary<string, List<PlayFabItem>> itemCategories, GameObject containerPrefab, GameObject iconPrefab, Transform parent)
    {
        _thumbnailParents = thumbnailParents;
        _itemCategories = itemCategories;
        _containerContentParent = parent;
        _itemDisplayContainerPrefab = containerPrefab;
        _itemThumbnailPrefab = iconPrefab;
    }   

    public void CreateItemDictionaries(List<PlayFabItem> items)
    {
        foreach (var item in items)
        {
            AddToDictionary(item.ItemClass, item);

            try
            {
                if (item.CustomData.ContainsKey(CustomDataKeys.Rare.ToString()))
                {
                    AddToDictionary(CustomDataKeys.Rare.ToString(), item);
                }
            }
            catch (Exception e)
            {
                HelperFunctions.CatchException(e);
                //HelperFunctions.Log(item);
            }


            if (item.IsLimited)
            {
                AddToDictionary("Limited", item);
            }
        }

    }

    private void ConfigureItems(string key, PlayFabItem i)
    {
        var thumbnail = GameObject.Instantiate(_itemThumbnailPrefab, _thumbnailParents[key]);
        thumbnail.GetComponent<ItemDisplayController>().SetItemInstance(i);
    }

    private void AddToDictionary(string key, PlayFabItem item)
    {
        if (_itemCategories.ContainsKey(key))
        {
            _itemCategories[key].Add(item);
            ConfigureItems(key, item);
        }
        else
        {
            _itemCategories.Add(key, new List<PlayFabItem>());
            _itemCategories[key].Add(item);

            var container = GameObject.Instantiate(_itemDisplayContainerPrefab, _containerContentParent.transform);
            Transform thumbnailParent = container.GetComponent<ItemContainerController>().Content;

            _thumbnailParents.Add(key, thumbnailParent);

            ConfigureItems(key, item);

        }
    }


}

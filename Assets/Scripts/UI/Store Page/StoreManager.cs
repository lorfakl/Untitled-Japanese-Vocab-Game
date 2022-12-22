using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;
using Utilities.Events;
using Utilities.PlayFabHelper;

public class StoreManager : MonoBehaviour
{
    [SerializeField]
    GameObject _itemDisplayContainerPrefab;
    [SerializeField]
    GameObject _containerContentParent;
    [SerializeField]
    GameObject _itemThumbnailPrefab;

    [SerializeField]
    GameEvent _itemCategoriesCreatedEvent;

    Dictionary<string, Transform> _thumbnailParents = new Dictionary<string,Transform>();


    Dictionary<string, List<PlayFabItem>> _itemCategories = new Dictionary<string, List<PlayFabItem>>();

    GridItemDisplayController _gridController;

    public void On_StorePageClicked()
    {
        RaiseItemCategoiesCreatedEvent();
    }

    void Start()
    {
        if(PlayFabController.IsAuthenticated)
        {
            PlayFabController.GetItemCatalog(ParseCatalogItems);
        }
        else
        {
            PlayFabController.IsAuthedEvent += OnAuthenticatedEvent;
        }

        _gridController = new GridItemDisplayController(_thumbnailParents, _itemCategories, _itemDisplayContainerPrefab, _itemThumbnailPrefab, _containerContentParent.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnAuthenticatedEvent()
    {
        PlayFabController.GetItemCatalog(ParseCatalogItems);
    }


    private void ParseCatalogItems(List<PlayFabItem> items)
    {
        _gridController.CreateItemDictionaries(items);
        
    }

    private void ConfigureItems(string key, PlayFabItem i)
    {
        var thumbnail = Instantiate(_itemThumbnailPrefab, _thumbnailParents[key]);
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

    private void RaiseItemCategoiesCreatedEvent()
    {
        _itemCategoriesCreatedEvent.Raise(_thumbnailParents);
    }
}

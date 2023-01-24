using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;
using Utilities.Events;
using Utilities.PlayFabHelper;
using Utilities.PlayFabHelper.CurrentUser;


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

    public void On_PurchaseComplete()
    {
        //CurrentAuthedPlayer.CurrentUser.Inventory.Inventory
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
        List<PlayFabItem> storeItems = new List<PlayFabItem>();
        if(CurrentAuthedPlayer.CurrentUser.Inventory.Inventory != null)
        {
            storeItems = items.Except(CurrentAuthedPlayer.CurrentUser.Inventory.Inventory).ToList();
            HelperFunctions.Log("Inventory Count: " + CurrentAuthedPlayer.CurrentUser.Inventory.Inventory.Count);
            HelperFunctions.Log("Catalog Count: " + items.Count);
            HelperFunctions.Log("StoreItems Count: " + storeItems.Count);
            _gridController.CreateItemDictionaries(storeItems);
        }
    }

    
    private void RaiseItemCategoiesCreatedEvent()
    {
        _itemCategoriesCreatedEvent.Raise(_thumbnailParents);
    }
}

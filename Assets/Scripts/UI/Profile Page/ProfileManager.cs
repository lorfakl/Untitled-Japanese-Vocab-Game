using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Utilities.Events;
using Utilities.PlayFabHelper;
using Utilities.PlayFabHelper.CurrentUser;

public class ProfileManager : MonoBehaviour
{
    [SerializeField]
    GameObject _itemDisplayContainerPrefab;
    [SerializeField]
    GameObject _containerContentParent;
    [SerializeField]
    GameObject _itemThumbnailPrefab;

    [SerializeField]
    GameEvent _itemCategoriesCreatedEvent;

    Dictionary<string, Transform> _thumbnailParents = new Dictionary<string, Transform>();


    Dictionary<string, List<PlayFabItem>> _itemCategories = new Dictionary<string, List<PlayFabItem>>();

    List<PlayFabItem> _inventoryItems = new List<PlayFabItem>();
    GridItemDisplayController _gridController;

    public void On_ProfilePageClicked()
    {
        CheckInventoryForUpdates();
        RaiseItemCategoiesCreatedEvent();
    }


    // Start is called before the first frame update
    void Start()
    {
        if (PlayFabController.IsAuthenticated)
        {
            GrabPlayerInventory();
        }
        else
        {
            PlayFabController.IsAuthedEvent += GrabPlayerInventory;
        }

        _gridController = new GridItemDisplayController(_thumbnailParents, _itemCategories, _itemDisplayContainerPrefab, _itemThumbnailPrefab, _containerContentParent.transform);
    }


    private void GrabPlayerInventory()
    {
        _inventoryItems = CurrentAuthedPlayer.CurrentUser.Inventory.Inventory;
        HelperFunctions.Log("Items in inventory: " + _inventoryItems.Count);
        _gridController.CreateItemDictionaries(_inventoryItems);

        _itemCategoriesCreatedEvent.Raise(_thumbnailParents);
    }

    private void RaiseItemCategoiesCreatedEvent()
    {
        _itemCategoriesCreatedEvent.Raise(_thumbnailParents);
    }

    private void CheckInventoryForUpdates()
    {
        HelperFunctions.Log("Checking for new items");
        List<PlayFabItem> newlyAddedInventoryItems = new List<PlayFabItem>();
        HelperFunctions.Log($"Number of items in the iventory {CurrentAuthedPlayer.CurrentUser.Inventory.Inventory.Count}");
        HelperFunctions.Log($"As compared to {_inventoryItems.Count}");
        foreach(var item in CurrentAuthedPlayer.CurrentUser.Inventory.Inventory)
        {
            if(!_inventoryItems.Contains(item))
            {
                newlyAddedInventoryItems.Add(item);
            }
        }

        if(newlyAddedInventoryItems.Count > 0)
        {
            HelperFunctions.Log(newlyAddedInventoryItems.Count + " new inventory items!");
            _gridController.CreateItemDictionaries(newlyAddedInventoryItems);
        }
    }
}

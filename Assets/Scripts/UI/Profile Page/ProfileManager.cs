using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;
using Utilities.Events;
using Utilities.PlayFabHelper;
using Utilities.PlayFabHelper.CurrentUser;
using static UnityEditor.Progress;

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
            GrabInitialPlayerInventory();
        }
        else
        {
            PlayFabController.IsAuthedEvent += GrabInitialPlayerInventory;
        }

        if(_gridController == null)
        {
            _gridController = new GridItemDisplayController(_thumbnailParents, _itemCategories, _itemDisplayContainerPrefab, _itemThumbnailPrefab, _containerContentParent.transform);
        }
        
    }


    private void GrabInitialPlayerInventory()
    {
        _inventoryItems = CurrentAuthedPlayer.CurrentUser.Inventory.Inventory.ToList();
        //EVERYTHING IS SECRETLY A POINTER!!!!!!!!!!!! ToList() is used to get a NEW LIST but the SAME items
        
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
        
        if(CurrentAuthedPlayer.CurrentUser.Inventory.Inventory.Count != _inventoryItems.Count)
        {
            newlyAddedInventoryItems = CurrentAuthedPlayer.CurrentUser.Inventory.Inventory.Except(_inventoryItems).ToList();
        }

        if(newlyAddedInventoryItems.Count > 0)
        {
            HelperFunctions.Log(newlyAddedInventoryItems.Count + " new inventory items!");
            _gridController.CreateItemDictionaries(newlyAddedInventoryItems);

        }
    }
}

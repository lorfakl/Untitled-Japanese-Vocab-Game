using System.Collections.Generic;
using UnityEngine;
using Utilities.Events;
using Utilities.PlayFabHelper;
using Utilities.PlayFabHelper.CSArguments;
using PlayFab.CloudScriptModels;
using Utilities;
using Utilities.PlayFabHelper.CurrentUser;

public class PurchaseManager : MonoBehaviour
{
    [SerializeField]
    GameEvent _purchaseCompleteEvent;

    List<PlayFabItem> _itemsInCart = new List<PlayFabItem>();

    public void On_ItemRemovedFromCart(object i)
    {
        PlayFabItem item = (PlayFabItem)i;
        bool removed = _itemsInCart.Remove(item);
        if(removed)
        {
            HelperFunctions.Log($"Items in cart: {_itemsInCart.Count}");
        }
        else
        {
            HelperFunctions.Error($"Not sure how this happned but Item: {item} was not removed");
            HelperFunctions.LogListContent(_itemsInCart);
        }
    }

    public void On_PossibleToPurchase(object i)
    {
        PlayFabItem item = (PlayFabItem)i;
        _itemsInCart.Add(item);
        HelperFunctions.Log($"Items in cart: {_itemsInCart.Count}");
    }

    public void On_StartPurchase(object cost)
    {
        uint price = (uint)cost;
        CompletePurchaseArg arg = new CompletePurchaseArg();
        List<string> itemIDs = new List<string>();
        foreach(var item in _itemsInCart)
        {
            itemIDs.Add(item.ID);
        }

        arg.ItemIDs = itemIDs;
        arg.Price = price.ToString();
        arg.VC = VirtualCurrency.SP.ToString();
        PlayFabController.ExecutionCSFunction(CSFunctionNames.CompletePurchase,
            arg, OnCompletePurchaseSuccess);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCompletePurchaseSuccess(ExecuteFunctionResult res)
    {
        HelperFunctions.Log("The Purchase was completed on PF side. GO CHECK!");
        foreach(var item in _itemsInCart)
        {
            //dont need to check if they exist in inventory because all these items are from the 
            CurrentAuthedPlayer.CurrentUser.Inventory.Inventory.Add(item);
            HelperFunctions.Log("Added new purchased item to inventory");
        }
        _itemsInCart.Clear();
        _purchaseCompleteEvent.Raise();
        
    }
}

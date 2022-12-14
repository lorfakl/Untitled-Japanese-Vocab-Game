using System.Collections.Generic;
using UnityEngine;
using Utilities.Events;
using Utilities.PlayFabHelper;
using Utilities.PlayFabHelper.CSArguments;
using PlayFab.CloudScriptModels;
using Utilities;

public class PurchaseManager : MonoBehaviour
{
    [SerializeField]
    GameEvent _purchaseCompleteEvent;

    List<PlayFabItem> _itemsInCart = new List<PlayFabItem>();

    public void On_ItemRemovedFromCart(object i)
    {
        PlayFabItem item = (PlayFabItem)i;
        _itemsInCart.Remove(item);
    }

    public void On_PossibleToPurchase(object i)
    {
        PlayFabItem item = (PlayFabItem)i;
        _itemsInCart.Add(item);
    }

    public void On_StartPurchase(object cost)
    {
        int price = (int)cost;
        CompletePurchaseArg arg = new CompletePurchaseArg();
        List<string> itemIDs = new List<string>();
        foreach(var item in _itemsInCart)
        {
            itemIDs.Add(item.ID);
        }

        arg.ItemIDs = itemIDs;
        arg.Price = price.ToString();
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
    }
}

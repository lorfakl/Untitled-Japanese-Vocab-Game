using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utilities;
using Utilities.Events;
using Utilities.PlayFabHelper;
using Utilities.PlayFabHelper.CurrentUser;

public class FundsDisplayController : MonoBehaviour
{
    [SerializeField]
    TMP_Text _totalCostText;

    [SerializeField]
    TMP_Text _availableFundsText;

    [SerializeField]
    GameEvent _possibleToPurchaseEvent;

    [SerializeField]
    GameEvent _startPurchaseProcessEvent;
    
    uint _availableFunds;
    uint _totalCost;
    Color _defaultColor;
    bool _isAfforable;
    PlayFabUser _user;
    MessageBox _loadingMessageBox;

    public void On_AddedToCart(object i)
    {
        PlayFabItem item = (PlayFabItem)i;
        item.SetSelectionStatus(true);
        uint itemPrice = item.Prices[VirtualCurrency.SP.ToString()];
        _totalCost += itemPrice;
        UpdateText(_totalCostText, _totalCost);
        _isAfforable = true;
        UpdateTextColor(_availableFundsText, _isAfforable);
    }

    public void On_StoreItemSelected(object i)
    {
        PlayFabItem item = (PlayFabItem)i;
        uint itemPrice = item.Prices[VirtualCurrency.SP.ToString()];
        if (_totalCost + itemPrice <= _availableFunds)
        {
            PossibleToPurchaseEvent(item);
            
        }
        else
        {
            _isAfforable = false;
            UpdateTextColor(_availableFundsText, _isAfforable);
        }
    }

    public void On_ItemRemovedFromCart(object i)
    {
        PlayFabItem item = (PlayFabItem)i;
        uint itemPrice = item.Prices[VirtualCurrency.SP.ToString()];
        _totalCost -= itemPrice;
        item.SetSelectionStatus(false);
        UpdateText(_totalCostText, _totalCost);
        _isAfforable = true;
        UpdateTextColor(_availableFundsText, _isAfforable);
    }

    public void On_PlayerCheckOut()
    {
        if(_totalCost <= _availableFunds)
        {
            long remaingFunds = _availableFunds - _totalCost;
            string msg = $"You will have {remaingFunds} SP remaining after this purchase " +
                $"\n Would you like to continue: ";

            MessageBox confirmPurchase = MessageBoxFactory.Create(MessageBoxType.Confirmation, msg, "Confirm Purchase");
            confirmPurchase.DisplayMessageBox(OnRecievedPurchaseConfirmation, confirmPurchase.AutoDestroyMessageBox);
        }
    }

    public void On_PurchaseComplete()
    {
        UpdateText(_totalCostText, 0);
        UpdateText(_availableFundsText, _availableFunds);
        _loadingMessageBox.DestroyMessageBox();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _user = CurrentAuthedPlayer.CurrentUser;
        _availableFunds = (uint)_user.Inventory.VirtualCurrencies[VirtualCurrency.SP];
        _totalCost = 0;

        _availableFundsText.text = _availableFunds.ToString();
        _totalCostText.text = _totalCost.ToString();

        _defaultColor = _availableFundsText.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateTextColor(TMP_Text textObj, bool canBuy)
    {
        if(canBuy)
        {
            textObj.color = _defaultColor;
        }
        else
        {
            textObj.color = Color.red;
        }
    }
    private void UpdateText(TMP_Text textObj, uint value)
    {
        textObj.text = value.ToString();
    }

    private void PossibleToPurchaseEvent(object i)
    {
        _possibleToPurchaseEvent.Raise(i);
    }

    private void StartPurchaseEvent(object c)
    {
        _availableFunds -= _totalCost;
        _startPurchaseProcessEvent.Raise(c);
    }

    private void OnRecievedPurchaseConfirmation()
    {
        StartPurchaseEvent(_totalCost);
        _loadingMessageBox = MessageBoxFactory.Create(MessageBoxType.Loading, "Completing Purchase...", "Purchasing Items...");
        _loadingMessageBox.DisplayMessageBox(_loadingMessageBox.AutoDestroyMessageBox);
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    
    int _availableFunds;
    uint _totalCost;
    Color _defaultColor;
    bool _isAfforable;
    
    public void On_StoreItemSelected(object i)
    {
        PlayFabItem item = (PlayFabItem)i;
        uint itemPrice = item.Prices[VirtualCurrency.SP.ToString()];
        if (_totalCost + itemPrice <= _availableFunds)
        {
            PossibleToPurchaseEvent(item);
            _totalCost += itemPrice;
            UpdateText(_totalCostText, _totalCost);
            _isAfforable = true;
            UpdateTextColor(_availableFundsText, _isAfforable);
        }
        else
        {
            _isAfforable = false;
            UpdateTextColor(_availableFundsText, _isAfforable);
        }
    }

    public void On_ItemRemovedFromCart(object i)
    {

    }

    public void On_PlayerCheckOut()
    {
        
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _availableFunds = CurrentAuthedPlayer.CurrentUser.Inventory.VirtualCurrencies[VirtualCurrency.SP];
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
        _startPurchaseProcessEvent.Raise(c);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.PlayFabHelper;

public class CartController : MonoBehaviour
{
    [SerializeField]
    GameObject _itemInCartPrefab;

    [SerializeField]
    Transform _cartContentParent;

    public void On_PossibleToPurchase(object i)
    {
        PlayFabItem item = (PlayFabItem)i;
        var cartItem = Instantiate(_itemInCartPrefab, _cartContentParent);
        cartItem.GetComponent<ItemInCartController>().SetItemInstance(item);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}

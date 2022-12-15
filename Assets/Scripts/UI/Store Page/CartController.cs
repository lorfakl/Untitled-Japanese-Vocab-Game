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

    public void On_PurchaseComplete()
    {
        for(int i= 0; i < _cartContentParent.childCount; i++)
        {
            var child = _cartContentParent.GetChild(i);
            Abort(child);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Abort(Transform c)
    {
        Destroy(c.gameObject);
    }

}

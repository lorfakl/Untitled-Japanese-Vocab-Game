using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.PlayFabHelper;
using Utilities;
using UnityEngine.UI;
using Utilities.Events;

public class ItemInCartController : MonoBehaviour
{
    [SerializeField]
    Button _itemButton;

    [SerializeField]
    Button _removeFromCartButton;

    [SerializeField]
    Image _itemImage;

    [SerializeField]
    GameEvent _removeItemFromCartEvent;

    PlayFabItem _itemInstance;
    public PlayFabItem Instance
    {
        get { return _itemInstance; }
    }
    public void SetItemInstance(PlayFabItem i)
    {
        if (_itemInstance == null)
        {
            _itemInstance = i;
            HelperFunctions.Log("Item instance set: " + i);
            _itemImage.sprite = i.Sprite;
            //HelperFunctions.Log("I will now, continue the rest of the setup!");
        }
        else
        {
            return;
        }
    }
    private void Awake()
    {
        _removeFromCartButton.onClick.AddListener(RemoveFromCartEvent);
        //_itemButton.onClick.AddListener();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void RemoveFromCartEvent()
    {
        _removeItemFromCartEvent.Raise(Instance);
        this.gameObject.SetActive(false);
    }
}

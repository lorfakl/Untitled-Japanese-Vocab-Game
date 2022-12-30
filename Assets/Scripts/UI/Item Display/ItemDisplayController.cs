using DG.Tweening.Plugins.Core.PathCore;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Utilities.Events;
using Utilities.PlayFabHelper;


public class ItemDisplayController : MonoBehaviour
{
    [SerializeField]
    Image _itemImage;

    [SerializeField]
    Button _itemButton;

    [SerializeField]
    GameEvent _itemIconClicked;

    [SerializeField]
    Sprite _selectedBorder;

    
    Sprite _defaultBorder;

    Image _borderSourceImage; 

    PlayFabItem _itemInstance;
    public PlayFabItem Instance
    {
        get { return _itemInstance; }
    }

    public void SetItemInstance(PlayFabItem i)
    {
        if(_itemInstance == null)
        {
            _itemInstance = i;
            i.OnSelectionChangeEvent += OnSelectionChange_Handler;
            //HelperFunctions.Log("Item instance set: " + i);
            //HelperFunctions.Log("I will now, continue the rest of the setup!");
        }
        else
        {
            return;
        }
    }

    private void Awake()
    {
        var child = this.gameObject.transform.GetChild(0);
        _itemButton = child.GetComponent<Button>();
        _itemImage = child.GetComponent<Image>();
        _borderSourceImage = GetComponent<Image>();
        _defaultBorder = _borderSourceImage.sprite;
    }

    // Start is called before the first frame update
    void Start()
    {
        _itemButton.onClick.AddListener(ItemClickedHandler);
        string path = "Sprites/" + _itemInstance.ItemClass + "/" + _itemInstance.Name;
        //HelperFunctions.Log($"Path to image: {path}");
        //Assets / Resources / Sprites / Professions / farming.png
        //                     Sprites / Professions / farming.png
        Sprite image = Resources.Load<Sprite>(path);
        if(image == null)
        {
            HelperFunctions.Log("The image is in fact null");
            HelperFunctions.Log("Expected path: " + path);
        }
        _itemImage.sprite = image;
        Instance.SetSprite(image);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ItemClickedHandler()
    {
        _itemIconClicked.Raise(Instance);
    }

    private void OnSelectionChange_Handler(bool isSelected)
    {
        if(isSelected)
        {
            _borderSourceImage.sprite = _selectedBorder;
        }
        else
        {
            _borderSourceImage.sprite = _defaultBorder;
        }
    }


}

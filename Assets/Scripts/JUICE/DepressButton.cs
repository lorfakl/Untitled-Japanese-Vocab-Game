using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class DepressButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    [SerializeField]
    Sprite pressedBtn;

    [SerializeField]
    Sprite unPressedBtn;

    Image _imageSpriteContainer;

    [SerializeField] AudioClip _pressClip, _releaseClip;

    [SerializeField] AudioSource _source;

    

    // Start is called before the first frame update
    void Start()
    {
        _imageSpriteContainer = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _imageSpriteContainer.sprite = pressedBtn;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _imageSpriteContainer.sprite = unPressedBtn;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpriteSwapOnClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] bool _returnToOriginal;

    [SerializeField] AudioClip _pressClip, _releaseClip;

    [SerializeField] AudioSource _source;

    [SerializeField] Sprite _pressedSprite, _releasedSprite;

    [SerializeField] Image _spriteContainer;
    Vector3 _originalScale;

    // Start is called before the first frame update
    void Start()
    {
        if(_spriteContainer == null)
        {
            _spriteContainer = GetComponent<Image>();
        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(!_returnToOriginal) 
        {
            if(_spriteContainer.sprite == _pressedSprite) 
            {
                _spriteContainer.sprite = _releasedSprite;
            }
            else
            {
                _spriteContainer.sprite = _pressedSprite;
            }
        }
        else
        {
            _spriteContainer.sprite = _pressedSprite;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(_returnToOriginal)
        {
            _spriteContainer.sprite = _releasedSprite;
        }  
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class DepressButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Range(0.1f, 0.5f)]
    [SerializeField] float pressScaleFactor;

    [SerializeField] AudioClip _pressClip, _releaseClip;

    [SerializeField] AudioSource _source;

    Vector3 _originalScale;

    // Start is called before the first frame update
    void Start()
    {
        _originalScale = transform.localScale;
        if(pressScaleFactor < 0.1f)
        {
            pressScaleFactor = 0.1f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ChangeButtonScale(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ChangeButtonScale(true);
    }

    private void ChangeButtonScale(bool returnToOriginal)
    {
        if(returnToOriginal) 
        { 
            transform.localScale = _originalScale;
        }
        else
        {
            Vector3 pressedScale = (1.0f-pressScaleFactor) * transform.localScale;
            transform.localScale = pressedScale;
        }
    }
}

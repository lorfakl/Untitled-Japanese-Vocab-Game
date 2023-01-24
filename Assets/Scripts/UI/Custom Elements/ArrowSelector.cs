using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using Utilities;

public class ArrowSelector : MonoBehaviour
{
    [SerializeField]
    Image arrowImage;

    [SerializeField]
    Button arrowBtn;

    [SerializeField]
    Button leftBtn;

    [SerializeField]
    Button rightBtn;

    [SerializeField]
    TMP_Text leftText;

    [SerializeField]
    TMP_Text rightText;

    [SerializeField]
    TranslateDirection _tranlateDirection;

    [SerializeField]
    float tweenTime = 0.25f;
    
    float _rotation = 0f;
    private object _owner;
    bool _isRightFacing = true;

    public UnityEvent<string> OnSelectionChange{ get; private set; }

    public TranslateDirection TranslationDirection
    { 
        get { return _tranlateDirection; }
        set { _tranlateDirection = value; }
    }

    public void DriveSelector(TranslateDirection t)
    {
        if(_tranlateDirection == t)
        {
            return;
        }

        if((int)_tranlateDirection == 3)
        {
            return;
        }

        int val = (int)t;
        int currentVal = (int)_tranlateDirection;
        HelperFunctions.Log($"Current Val: {currentVal} Passed val:{val}");
        if(val/2 == currentVal || val*2 == currentVal)
        {
            RotateArrow();
            return;
        }
        else if(currentVal == 0)
        {
            //if default but the default is unknown the button text should be gathered in this case to determine what is left and right
        }

    }

    private void Awake()
    {
        OnSelectionChange = new UnityEvent<string>();
    }
    // Start is called before the first frame update
    void Start()
    {
        arrowBtn.onClick.AddListener(NotifySubscribers);
        leftBtn.onClick.AddListener(OnLeftButtonClick);
        rightBtn.onClick.AddListener(OnRightButtonClick);
        TranslationDirection = _tranlateDirection;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void NotifySubscribers()
    {
        RotateArrow();
        OnSelectionChange?.Invoke(TranslationDirection.ToString());
    }

    void OnLeftButtonClick()
    {
        if(_isRightFacing)
        {
            NotifySubscribers();
            HelperFunctions.Log("Left button clicked");
        }
    }

    void OnRightButtonClick()
    {
        if(!_isRightFacing)
        {
            NotifySubscribers();
            HelperFunctions.Log("Right button clicked");
        }
    }

    void RotateArrow()
    {
        int enumInt = (int)TranslationDirection;
        _isRightFacing = !_isRightFacing; //flips the value of _isRightFacing
        _rotation += 180;
        HelperFunctions.Log("Current Direction: " + TranslationDirection);

        arrowImage.transform.DOLocalRotate(new Vector3(0, 0, _rotation), tweenTime);
        if(enumInt > 9)
        {
            TranslationDirection = (TranslateDirection)(enumInt/2);
            HelperFunctions.Log("New Direction: " + TranslationDirection);
        }
        else if(enumInt != 3)
        {
            TranslationDirection = (TranslateDirection)(enumInt*2);
            HelperFunctions.Log("New Direction: " + TranslationDirection);
        }

        if(_isRightFacing)
        {
            HelperFunctions.Log("Is right facing");
        }
        else
        {
            HelperFunctions.Log("Is left facing");
        }
    }
}

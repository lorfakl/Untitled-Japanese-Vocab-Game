using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Utilities;
using TMPro;
using UnityEngine.Events;

public class ToggleSlider : MonoBehaviour
{
    /// <summary>
    /// The anchors are set to the left edge of the parent and the pivot point is set to the left edge of the slider
    /// Meaning that the slider's local X position cannot exceed sliderWidth * (sliderOptions - 1)
    /// </summary>
    enum ToggleSliderOptions
    {
        Two = 2,
        Three = 3,
        Four = 4
    }
    [SerializeField]
    float _tweenTime;

    [SerializeField]
    Image _toggleBackground;

    [SerializeField]
    Image _toggleForeground;

    [SerializeField]
    TMP_Text textOptions;

    [SerializeField]
    Transform textContainer;

    [SerializeField]
    ToggleSliderOptions _totalTogglePositions;

    [SerializeField]
    Slider _slider;

    [SerializeField]
    string[] _options;

    public UnityEvent<string> OnValueSelectedChanged
    {
        get;
        private set;
    }

    public Slider SliderComponent
    {
        get { return _slider; }
    }

    public string[] Options
    {
        get { return _options; }
    }

    public string CurrentSelection { get; private set; }

    float _mostRecentSliderVal = -1f;
    float _defaultXPosition;
    float _maximumXPosition;

    public void DriveSlider(string opt)
    {
        for(int i = 0; i < Options.Length; i++) 
        {
            if (Options[i] == opt)
            {
                SliderComponent.value = i;
                HelperFunctions.Log("WHERE R MY FUCKING CALLBACKS");
                SliderComponent.onValueChanged.Invoke(SliderComponent.value);
                return;
            }
        }

        HelperFunctions.Error($"Unable to find Option: {opt} in array {Options}");
    }

    private void Awake()
    {
        OnValueSelectedChanged = new UnityEvent<string>();
        if(_options.Length > 4)
        {
            HelperFunctions.Error("A Slider Toggle SHOULD NOT have more than 4 options. AutoResizing");
            string[] newOptionArr = new string[4];
            for(int i = 0; i < 4; i++) 
            {
                newOptionArr[i] = _options[i];
            }

            _options = newOptionArr;
        }
    }

    private void Start()
    {
        _slider.onValueChanged.AddListener(ChangeSliderLocation);
        _slider.onValueChanged.AddListener(ChangeSelectedValue);


        Vector2 sliderHandleSizeDelta = _toggleForeground.GetComponent<RectTransform>().sizeDelta;
        sliderHandleSizeDelta.x = _toggleBackground.GetComponent<RectTransform>().rect.width / (int)_totalTogglePositions;
        
        _toggleForeground.GetComponent<RectTransform>().sizeDelta = sliderHandleSizeDelta;
        _slider.maxValue = (int)_totalTogglePositions - 1;
        _slider.wholeNumbers = true;
        
        _defaultXPosition = _toggleForeground.transform.localPosition.x;
        _maximumXPosition = _defaultXPosition + sliderHandleSizeDelta.x * ((int)_totalTogglePositions - 1);
        LoadOptions();
    }

    private void InitializeRequiredValues()
    {
        Vector2 sliderHandleSizeDelta = _toggleForeground.GetComponent<RectTransform>().sizeDelta;
        sliderHandleSizeDelta.x = _toggleBackground.GetComponent<RectTransform>().rect.width / (int)_totalTogglePositions;
        _defaultXPosition = _toggleForeground.transform.localPosition.x;
        _maximumXPosition = _defaultXPosition + sliderHandleSizeDelta.x * ((int)_totalTogglePositions - 1);

        _mostRecentSliderVal = 0;
    }

    private void LoadOptions()
    {
        //initially all text children are enabled
        int enabledKids = 4;
        for(int i = 0; i < textContainer.childCount; i++)
        {
            var textChild = textContainer.GetChild(i);
            if (i < _options.Length)
            {
                try
                {
                    textChild.GetComponent<TMP_Text>().text = _options[i];
                    RectTransform textChildRect = textChild.GetComponent<RectTransform>();
                    Vector2 currentSize = textChildRect.sizeDelta;
                    Vector2 updatedSize = new Vector2(CalculateTextBoxWidth(_options.Length), currentSize.y);
                    textChildRect.sizeDelta = updatedSize;
                }
                catch(Exception e) 
                { 
                    HelperFunctions.CatchException(e);
                }
                
            }
            else
            {
                textChild.gameObject.SetActive(false);
            }
        }

        _totalTogglePositions = (ToggleSliderOptions)_options.Length;
        CurrentSelection = _options[0];
    }

    private float CalculateTextBoxWidth(int optCount)
    {
        switch(optCount)
        {
            case 2:
                return 300f;
            case 3:
                return 150f;
            case 4:
                return 100f;
            default:
                string errMsg = $"Invalid number of options when calculating the Text box width count given:{optCount}";
                HelperFunctions.Error(errMsg);
                throw new ArgumentException(errMsg);
        }
    }

    private void ChangeSliderLocation(float value)
    {
        HelperFunctions.Log("I hate UI SOOOOOOOOOOOO MUUUUUUUUUUUUUUCH");
        int signOperation = 1;
        int delta = 1;
        
        if(_mostRecentSliderVal < 0)
        {
            _mostRecentSliderVal = 0;
        }

        if(_mostRecentSliderVal == value)
        {
            return;
        }

        Vector2 sliderHandleSizeDelta = _toggleForeground.GetComponent<RectTransform>().sizeDelta;
        Vector3 newVisibleSliderPosition = _toggleForeground.transform.localPosition;
        
        //calculating the delta so the end tween location is accurate
        if(Mathf.Abs(_mostRecentSliderVal - value) > 1 && (_mostRecentSliderVal > -1))
        {
            delta = (int)Mathf.Abs(_mostRecentSliderVal - value);
        }

        //Tracking the directions of the value change
        if (_mostRecentSliderVal < 0f)
        {
            _mostRecentSliderVal = value;
        }
        else if(_mostRecentSliderVal > value)
        {
            _mostRecentSliderVal = value;
            signOperation = -1;
        }
        else if(_mostRecentSliderVal < value)
        {
            _mostRecentSliderVal = value;
        }

        float modifier = delta * signOperation * sliderHandleSizeDelta.x;


        newVisibleSliderPosition.x += modifier;
        newVisibleSliderPosition.x = Mathf.Clamp(newVisibleSliderPosition.x, _defaultXPosition, _maximumXPosition);
        _toggleForeground.transform.DOLocalMove(newVisibleSliderPosition, _tweenTime);
        
    }

    private void ChangeSelectedValue(float value)
    {
        CurrentSelection = _options[(int)value];
        OnValueSelectedChanged?.Invoke(_options[(int)value]);
        HelperFunctions.Log("New Value: " + _options[(int)value]);
    }
}

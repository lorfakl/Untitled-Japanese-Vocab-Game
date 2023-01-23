using System.Collections;
using System.Collections.Generic;
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
        string opt = "";
        for(int i = 0; i < _options.Length; i++)
        {
            if(i == (_options.Length - 1))
            {
                opt += _options[i];
            }
            else
            {
                if(opt.Length == 2)
                {
                    opt += _options[i] + "\t\t";
                }
                else
                {
                    opt += _options[i] + "\t" + "\t";
                }
                
            }
        }

        textOptions.text = opt;
        _totalTogglePositions = (ToggleSliderOptions)_options.Length;
        CurrentSelection = _options[0];
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

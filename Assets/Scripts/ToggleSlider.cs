using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Utilities;

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
    Image _toggleBackground;

    [SerializeField]
    Image _toggleForeground;

    [SerializeField]
    ToggleSliderOptions _totalTogglePositions;

    [SerializeField]
    int _currentTogglePosition;

    private void Awake()
    {
        
    }

    private void Update()
    {
        Vector2 sliderHandleSizeDelta = _toggleForeground.GetComponent<RectTransform>().sizeDelta;
        sliderHandleSizeDelta.x = _toggleBackground.GetComponent<RectTransform>().rect.width / (int)_totalTogglePositions;
        _toggleForeground.GetComponent<RectTransform>().sizeDelta = sliderHandleSizeDelta;
    }

    private void OnMouseUp()
    {
        HelperFunctions.Log(Input.mousePosition);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Utilities;

public class LoadingIndicator : MonoBehaviour
{
    [SerializeField]
    Image _loadingImage;

    [SerializeField]
    float _tweenTime = 1f;

    bool _isTweening = false;
    
    
    public void UpdateIndicatorSettings(float tweenTime, Color color)
    {
        _tweenTime = tweenTime;
        _loadingImage.color = color;
    }

    private void Awake()
    {
        int i = 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TweenFillAmount();
    }

    private void OnDisable()
    {
        DOTween.Clear();
        _isTweening = false;
    }

    void TweenFillAmount()
    {
        if(!_isTweening)
        {
            //HelperFunctions.Log("Tween Started");
            _loadingImage.DOFillAmount(1, _tweenTime).OnComplete(ResetTweenValue);
            _isTweening = true;
        }
    }

    void ResetTweenValue()
    {
        //HelperFunctions.Log("Tween Complete");
        _isTweening = false;
        _loadingImage.fillAmount = 0;
        //HelperFunctions.Log("Tween Reset");
    }
}

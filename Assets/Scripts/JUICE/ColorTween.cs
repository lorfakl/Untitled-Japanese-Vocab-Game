using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using DG.Tweening.Core;

public class ColorTween : MonoBehaviour
{
    [SerializeField]
    TMP_Text thanksforPlaying;
    
    [SerializeField]
    float _tweenTime = 1;
    
    bool _isTweening = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!_isTweening)
        { 
            TweenColor();
        }
    }

    void TweenColor()
    {
        _isTweening = true;
        thanksforPlaying.DOColor(new Color(255, 255, 0), _tweenTime).OnComplete(
        () =>
        {
            thanksforPlaying.DOColor(new Color(0, 255, 0), _tweenTime).OnComplete(
            () => 
            {
                thanksforPlaying.DOColor(new Color(0, 255, 255), _tweenTime).OnComplete(
                () =>
                {
                    thanksforPlaying.DOColor(new Color(0, 0, 255), _tweenTime).OnComplete(
                    () =>
                    {
                        thanksforPlaying.DOColor(new Color(255, 0, 255), _tweenTime).OnComplete(
                       () =>
                       {
                           thanksforPlaying.DOColor(new Color(255, 0, 0), _tweenTime).OnComplete(
                           () =>
                           {
                               _isTweening = false;
                           }
                           );
                       }
                       );
                    }
                    );
                }
                );
            }
            );
        });
    }

    private void OnDisable()
    {
        DOTween.Clear();
    }
}

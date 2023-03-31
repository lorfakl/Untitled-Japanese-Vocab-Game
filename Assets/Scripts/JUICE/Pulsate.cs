using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Pulsate : MonoBehaviour
{
    [SerializeField]
    float _scaleFactor = 2.5f;

    [SerializeField]
    float _tweenTime = .25f;

    object _tweenID;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 targetScale = transform.localScale * _scaleFactor;
        var tween = this.transform.DOScale(targetScale, _tweenTime).SetLoops(-1, LoopType.Yoyo);
        _tweenID = tween.id;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        DOTween.Kill(_tweenID);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace Utilities.Tweening
{
    public class ScaleOnEnable : MonoBehaviour
    {
        [SerializeField]
        Vector3 scaleValueOnEnable;

        [SerializeField]
        Vector3 scalevalueOnDisable;

        [SerializeField]
        Ease easeValue = Ease.Linear;

        [SerializeField]
        [Range(0.25f, 3f)]
        float tweenTime = 0.5f;

        private void OnEnable()
        {
            transform.DOScale(scaleValueOnEnable, tweenTime).SetEase(easeValue);
        }


        private void OnDisable()
        {
            transform.DOScale(scalevalueOnDisable, tweenTime).SetEase(easeValue);
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace Utilities.Tweening
{
    public class LocalMoveOnEnable : MonoBehaviour
    {
        [SerializeField]
        Vector3 positionToMoveOnEnable;

        [SerializeField]
        Vector3 positionToMoveOnDisable;

        [SerializeField]
        Ease easeValue = Ease.Linear;

        [SerializeField]
        [Range(0.25f, 3f)]
        float tweenTime = 0.5f;

        private void OnEnable()
        {
            transform.DOLocalMove(positionToMoveOnEnable, tweenTime).SetEase(easeValue);
        }


        private void OnDisable()
        {
            transform.DOLocalMove(positionToMoveOnDisable, tweenTime).SetEase(easeValue);
        }
    }
}


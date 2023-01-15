using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Utilities;

public class HeartController : MonoBehaviour
{
    [SerializeField]
    List<Image> hearts;

    [SerializeField]
    GridLayoutGroup gridLayoutGroup;

    [SerializeField]
    float _tweenTime = .05f;

    public void HeartLostEvent_Handler()
    {
        gridLayoutGroup.enabled = false;
        Image heart = hearts[hearts.Count - 1];
        Image heartBlackened = heart.gameObject.transform.GetChild(0).GetComponent<Image>();
        Vector2 finalPos = heartBlackened.rectTransform.position;
        finalPos.y += 2000;
        heart.DOColor(Color.black, 0);
        heartBlackened.DOColor(Color.black, _tweenTime);
        heartBlackened.rectTransform.DOAnchorPos(finalPos, _tweenTime).OnComplete(() => { gridLayoutGroup.enabled = true; });

        if(hearts.Remove(heart))
        {
            return;
        }

        HelperFunctions.Error(heart.gameObject.name + "Was not removed successfully");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        DOTween.Clear();
    }
}

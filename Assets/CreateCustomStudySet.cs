using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CreateCustomStudySet : MonoBehaviour
{
    [SerializeField]
    float buttonFlipTweenTime; 

    [SerializeField]
    Button createNewCSSBtn;

    [SerializeField]
    Image buttonImage;

    [SerializeField]
    Transform createCSSUI;

    [SerializeField]
    TMP_InputField nameInput; 

    [SerializeField] 
    TMP_InputField descriptionInput;
    
    [SerializeField]
    TMP_InputField wordSearchField;

    [SerializeField]
    Sprite addButtonSprite;

    [SerializeField]
    Sprite downButtonSprite;

    Sprite currentButtonSprite;
    Sequence buttonClickSequence;
    private void Awake()
    {
        currentButtonSprite = addButtonSprite;

        buttonClickSequence = DOTween.Sequence();
        buttonClickSequence.Append(buttonImage.transform.DOLocalRotate(new Vector3(0, 0, 360), buttonFlipTweenTime, RotateMode.Fast).SetEase(Ease.OutCubic))
            .Insert(0, buttonImage.DOFade(0, buttonFlipTweenTime / 3)
                .OnComplete(() =>
                {
                    buttonImage.sprite = currentButtonSprite;
                    buttonImage.DOFade(255, buttonFlipTweenTime / 5);
                })).OnComplete(OnPressOperations).Pause();
    }

    

    // Start is called before the first frame update
    void Start()
    {
        createNewCSSBtn.onClick.AddListener(ShowCreateMenu);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ShowCreateMenu()
    {
        if(!buttonClickSequence.IsPlaying())
        {
            buttonClickSequence.Restart();
        }
    }

    void OnPressOperations()
    {
        SwapCurrentButtonSprite();
    }

    private void SwapCurrentButtonSprite()
    {
        if(currentButtonSprite == addButtonSprite)
        {
            currentButtonSprite = downButtonSprite;
        }
        else
        {
            currentButtonSprite = addButtonSprite;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum SortState { Ascending, Descending }

public delegate void OnSortModeChangeEventHandler(SortState state);

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class SortButton : MonoBehaviour
{
    [SerializeField]
    float minimizedScaleFactor = 0.5f;

    [SerializeField]
    float tweenTime = 0.3f;

    [SerializeField]
    SpriteRenderer ascendingSprite;

    [SerializeField]
    SpriteRenderer descendingSprite;

    Image _imageSpriteContainer;
    Button _button;
    SortState _state;

    public event OnSortModeChangeEventHandler OnSortModeChange;
    private void Awake()
    {
        _imageSpriteContainer = GetComponent<Image>();
        _button = GetComponent<Button>();
        _button.onClick.AddListener(ChangeSortMode);
    }

    

    // Start is called before the first frame update
    void Start()
    {
        _state = SortState.Ascending;

    }

    private void ChangeButtonState()
    {
        if(_state == SortState.Ascending)
        {
            _state = SortState.Descending;
            SetActiveSprite(descendingSprite, ascendingSprite);
        }
        else
        {
            _state = SortState.Ascending;
            SetActiveSprite(ascendingSprite, descendingSprite);
        }
    }

    private void SetActiveSprite(SpriteRenderer newlyActiveSprite, SpriteRenderer previouslyActiveSprite)
    {
        previouslyActiveSprite.DOColor(Color.gray, tweenTime);
        Vector3 minimizedScale = minimizedScaleFactor * previouslyActiveSprite.transform.localScale;
        previouslyActiveSprite.transform.DOScale(minimizedScale, tweenTime);

        newlyActiveSprite.DOColor(Color.white, tweenTime);
        Vector3 regualrScale = newlyActiveSprite.transform.localScale / minimizedScaleFactor;
        newlyActiveSprite.transform.DOScale(regualrScale, tweenTime);
    }

    private void ChangeSortMode()
    {
        OnSortModeChange?.Invoke(_state);
    }
}

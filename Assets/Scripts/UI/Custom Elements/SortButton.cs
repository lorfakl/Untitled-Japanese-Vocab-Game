using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Utilities;
using System.Linq;
using static UnityEngine.EventSystems.EventTrigger;

public enum SortState { Ascending, Descending }

public delegate void OnSortModeChangeEventHandler(SortState state);

[RequireComponent(typeof(Image))]
public class SortButton : MonoBehaviour
{
    [SerializeField]
    float minimizedScaleFactor = 0.5f;

    [SerializeField]
    float tweenTime = 0.3f;

    [SerializeField]
    Button button;

    [SerializeField]
    SpriteRenderer descendingSprite;

    Image _imageSpriteContainer;
    Button _button;
    SortState _state;

    public event OnSortModeChangeEventHandler OnSortModeChange;

    private void Awake()
    {
        _imageSpriteContainer = GetComponent<Image>();
        _button = button;
        _button.onClick.AddListener(ChangeSortMode);
    }

    // Start is called before the first frame update
    void Start()
    {
        _state = SortState.Descending;

    }

    private void ChangeButtonState()
    {
        if(_state == SortState.Ascending)
        {
            _state = SortState.Descending;
            //SetActiveSprite(descendingSprite, ascendingSprite);
        }
        else
        {
            _state = SortState.Ascending;
            //SetActiveSprite(ascendingSprite, descendingSprite);
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
        HelperFunctions.Log("Click!");
        ChangeButtonState();
        OnSortModeChange?.Invoke(_state);
    }

    public static List<JapaneseWord> SortBySpeed(List<GlossaryEntry> entriesToSort, SortState s)
    {
        List<GlossaryEntry> sortedList = null;
        if(s == SortState.Ascending)
        {
            sortedList = entriesToSort.OrderBy(entry => entry.Data.AverageTime).ToList();
        }
        else
        {
            sortedList = entriesToSort.OrderByDescending(entry => entry.Data.AverageTime).ToList();
        }

        List<JapaneseWord> sortedWordList = new List<JapaneseWord>();    
        for(int i = 0; i < sortedList.Count; i++)
        {
            sortedWordList.Add(sortedList[i].Data);
        }

        return sortedWordList;
    }

    public static List<JapaneseWord> SortBySeen(List<GlossaryEntry> entriesToSort, SortState s)
    {
        List<GlossaryEntry> sortedList = null;
        if (s == SortState.Ascending)
        {
            sortedList = entriesToSort.OrderBy(entry => entry.Data.TimesSeen).ToList();
        }
        else
        {
            sortedList = entriesToSort.OrderByDescending(entry => entry.Data.TimesSeen).ToList();
        }

        List<JapaneseWord> sortedWordList = new List<JapaneseWord>();
        for (int i = 0; i < sortedList.Count; i++)
        {
            sortedWordList.Add(sortedList[i].Data);
        }

        return sortedWordList;
    }

    public static List<JapaneseWord> SortByCorrect(List<GlossaryEntry> entriesToSort, SortState s)
    {
        List<GlossaryEntry> sortedList = null;
        if (s == SortState.Ascending)
        {
            sortedList = entriesToSort.OrderBy(entry => entry.Data.CorrectPercentage()).ToList();
        }
        else
        {
            sortedList = entriesToSort.OrderByDescending(entry => entry.Data.CorrectPercentage()).ToList();
        }

        List<JapaneseWord> sortedWordList = new List<JapaneseWord>();
        for (int i = 0; i < sortedList.Count; i++)
        {
            sortedWordList.Add(sortedList[i].Data);
        }

        return sortedWordList;
    }

}

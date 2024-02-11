using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
//using SqueelLayer
public class CSSController : MonoBehaviour
{
    [SerializeField]
    GameObject cssEntryPrefab;

    [SerializeField]
    Transform cssContentPanel;

    [SerializeField]
    GameObject cssDetails;

    [SerializeField]
    GameObject goalSubMenu;

    [SerializeField]
    TMP_InputField searchField;

    [SerializeField]
    ToggleSlider toggleFilter;

    [SerializeField]
    SortButton newestSortButton;

    [SerializeField]
    SortButton popularSortButton;

    // Start is called before the first frame update
    private List<CustomStudySet> studySets = new List<CustomStudySet>();
    private void Awake()
    {
        /*Check if data is in SqueelLayer if so load it, either way grab data from PF*/
        searchField.onValueChanged.AddListener(Search);
    }

    private void Search(string keyword)
    {
        if (!String.IsNullOrEmpty(keyword))
        {
            List<CustomStudySet> searchResults = studySets.Where(css => css.Info.Contains(keyword)).ToList();
            if (searchResults.Count > 0)
            {
                for (int i = 0; i < cssContentPanel.childCount; i++)
                {
                    bool shouldDisable = true;
                    var child = cssContentPanel.GetChild(i);
                    CustomStudySet currentSet = child.GetComponent<CSSEnrty>().CurrentSet;
                    foreach (var css in searchResults)
                    {
                        if (css.ID == currentSet.ID)
                        {
                            shouldDisable = false;
                            break;
                        }
                    }

                    if (shouldDisable)
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < cssContentPanel.childCount; i++)
            {
                var child = cssContentPanel.GetChild(i);
                if (!child.gameObject.activeSelf)
                {
                    child.gameObject.SetActive(true);
                }
            }
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}

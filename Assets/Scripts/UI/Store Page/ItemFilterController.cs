using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class ItemFilterController : MonoBehaviour
{
    [SerializeField]
    TMP_Dropdown filterDropdown;

    Dictionary<int, string> filterNames = new Dictionary<int, string>();
    Dictionary<string, Transform> itemCategoryContainers = new Dictionary<string, Transform>();
    public void On_ItemCategoriesCreated(object itemCategories)
    {

        itemCategoryContainers = (Dictionary<string, Transform>)itemCategories;
        List<string> categories = new List<string>();
        categories = itemCategoryContainers.Keys.ToList();

        //HelperFunctions.LogListContent(categories);
        filterDropdown.options.Clear();
        string defaultFilterOption = "None";
        
        filterDropdown.options.Add(new TMP_Dropdown.OptionData { text = defaultFilterOption });
        filterNames.Add(0, defaultFilterOption);
        
        foreach(string cat in categories)
        {
            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
            optionData.text = cat;
            filterDropdown.options.Add(optionData);
            filterNames.Add(categories.IndexOf(cat) + 1, cat);
        }

    }


    // Start is called before the first frame update
    void Start()
    {
        filterDropdown.onValueChanged.AddListener(FilterItemDisplay);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FilterItemDisplay(int i)
    {
        HelperFunctions.Log($"I believe this is an index?: {i}");
        HelperFunctions.Log($"Should be showing: {filterNames[i]}");

        if(i == 0)
        {
            HelperFunctions.Log("Should be enabling all");
            foreach(var pair in itemCategoryContainers)
            {
                pair.Value.parent.parent.gameObject.SetActive(true);
            }

            return;
        }

        foreach(var pair in itemCategoryContainers)
        {
            if(pair.Key != filterNames[i])
            {
                pair.Value.parent.parent.gameObject.SetActive(false);
            }
            else
            {
                pair.Value.parent.parent.gameObject.SetActive(true);
            }
        }

    }
}

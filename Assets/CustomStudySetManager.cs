using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class CustomStudySetManager : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    Transform createCSSParent;

    [SerializeField]
    Transform viewCSSParent;

    [SerializeField]
    Button createNewCSSBtn;

    //private List<CustomStudySet> 
    private void Awake()
    {
        createNewCSSBtn.onClick.AddListener(DisplayNewCSSMenu);
    }

    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DisplayNewCSSMenu()
    {
        if (PremiumLimits.CurrentNumberOfCustomSets < PremiumLimits.MaxNumberOfCustomSets)
        {
            viewCSSParent.gameObject.SetActive(false);
            createCSSParent.gameObject.SetActive(true);
        }
        else
        {
            MessageBoxFactory.CreateMessageBox("You have reached the Max", $"You have reached the Max Number of Custom Study Sets ({PremiumLimits.MaxNumberOfCustomSets}). If you think the app is cool please consider a one time purchase to go even further beyond!", null, true);
        }
    }

    
}

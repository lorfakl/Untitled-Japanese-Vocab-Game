using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Utilities.Events;

public class Explanation : MonoBehaviour
{
    [SerializeField]
    bool IsStarterTrigger;

    [SerializeField]
    GameEvent passthroughGameEvent;

    [SerializeField]
    string titleMessage;

    [SerializeField]
    string descriptionMessage;

    [SerializeField]
    Transform target;

    [SerializeField]
    Button button;

    private void Awake()
    {
        if(button != null) 
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(ExplanationClickHandler);
        }
    }

    private void ExplanationClickHandler()
    {
        if (MenuController.PanelChangeEnabled)
        {
            if (ExplanationManager.IsExplanationCompete)
            {
                passthroughGameEvent.Raise();
            }
            else
            {
                ExplanationManager.StartExplanation();
                if(ExplanationManager.IsExplanationCompete) 
                {
                    passthroughGameEvent.Raise();
                }
            }
        }
        
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
        
    }

    public void ShowExplanation()
    {
        MessageBoxFactory.CreateMessageBox(titleMessage, descriptionMessage, OnMessageBoxClose, true);
    }

    private void OnMessageBoxClose()
    {
        HelperFunctions.Log("Closed MessageBox");
        //emphasisObject.SetActive(false);
        ExplanationManager.GoNext();   
    }

}

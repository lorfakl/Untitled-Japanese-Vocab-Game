using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Utilities.Events;
using Utilities.PlayFabHelper;

public class ArcadeOpening : MonoBehaviour
{
    [SerializeField]
    Button _startBtn;

    [SerializeField]
    TMP_InputField _InputField;

    [SerializeField]
    GameEvent _startStudyEvent;
    // Start is called before the first frame update

    new string name = "";
    

    void Start()
    {
        _startBtn.onClick.AddListener(ConfigureNewUser);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ConfigureNewUser()
    {
        name = _InputField.text;
        PlayFabController.ArcadeLogin(name,UpdateDisplayName);
        HelperFunctions.Log("Configured New User");
    }

    void UpdateDisplayName()
    {

        PlayFabController.DisplayName(name);
        _startStudyEvent.Raise();
        HelperFunctions.Log("Start Study Event Raised");
    }


}

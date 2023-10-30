using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Events;
using DG.Tweening;

public class CreateStudyEntityGoalController : MonoBehaviour
{
    [SerializeField]
    GameEvent wordDetailsClosedEvent;

    [SerializeField]
    Button closeButton;

    [SerializeField]
    Button createGoalButton;

    [SerializeField]
    TMP_Text studyEntityName;

    [SerializeField]
    TMP_Text seenValue;

    [SerializeField]
    TMP_Text accuracyValue;

    [SerializeField]
    TMP_Text speedValue;

    [SerializeField]
    TMP_Text entityKanaOrDescription;

    [SerializeField]
    TMP_Text entityEnglishOrCreatedBy;

    [SerializeField]
    TMP_InputField goalNameField;

    [SerializeField]
    TMP_Dropdown targetStatDropdown;

    [SerializeField]
    Slider goalTargetValueSlider;

    [SerializeField]
    Toggle enablePushNotifications;
    
    [SerializeField]
    float tweenTime;

    private void Awake()
    {
        closeButton.onClick.AddListener(() => { wordDetailsClosedEvent.Raise(); });     
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

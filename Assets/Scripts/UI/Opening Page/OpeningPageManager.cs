using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Utilities;
using Utilities.Events;
using Utilities.PlayFabHelper;

public class OpeningPageManager : MonoBehaviour
{
    [SerializeField]
    TMP_InputField displayNameField;

    [SerializeField]
    TMP_Text _lastStudiedText;

    [SerializeField]
    TMP_Text _displayNameLabel;


    GameEvent _finishedLoadingEvent;

    private void Awake()
    {
        _finishedLoadingEvent = ScriptableObject.CreateInstance<GameEvent>();
        MessageBoxFactory.Create(MessageBoxType.Loading, "Please Wait while game data is loaded", "Loading Player Data...", _finishedLoadingEvent).DisplayLoadingMessageBox(); 
        if(PlayFabController.IsAuthenticated)
        {
            OnLoginPageSetUp();
        }
        else
        {
            PlayFabController.IsAuthedEvent += OnLoginPageSetUp;
        }
        displayNameField.onEndEdit.AddListener(UpdateDisplayName);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnLoginPageSetUp()
    {
        if(string.IsNullOrEmpty(Playfab.DisplayName))
        {
            MessageBoxFactory.Create(MessageBoxType.Message, "Please Enter a displayName so people can know you on the leaderboards", "Set DisplayName");
        }
        else
        {
            displayNameField.text = Playfab.DisplayName;
            displayNameField.interactable = false;
        }

        if(Playfab.LastLogin != System.DateTime.MinValue)
        {
            _lastStudiedText.text = "Last Studied: " + Playfab.LastLogin.ToString();
        }

        _finishedLoadingEvent.Raise();
    }

    void UpdateDisplayName(string n)
    {
        GameEvent onUpdateDisplayNameComplete = ScriptableObject.CreateInstance<GameEvent>();
        var confirmBox = MessageBoxFactory.Create(MessageBoxType.Confirmation, $"This will be a permnant change. Would you like to Keep the DisplayName: {n}?", "Confirm Display");
        confirmBox.DisplayMessageBox(() => 
        {
            var loadBox = MessageBoxFactory.Create(MessageBoxType.Loading, "Please Wait while game data is loaded", "Updating DisplayName", onUpdateDisplayNameComplete);
            loadBox.DisplayLoadingMessageBox();
            HelperFunctions.Log("Did this even happen? ");
            PlayFabController.DisplayName(n, () => { onUpdateDisplayNameComplete.Raise(); }); 
        }, 
        () => { confirmBox.DestroyMessageBox(); });   
    }
}

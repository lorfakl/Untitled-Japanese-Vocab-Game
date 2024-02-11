using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Utilities;
using Utilities.Events;
using Utilities.PlayFabHelper;
using PlayFab;

public class OpeningPageManager : MonoBehaviour
{
    [SerializeField]
    TMP_InputField displayNameField;

    [SerializeField]
    TMP_Text _lastStudiedText;

    [SerializeField]
    TMP_Text _displayNameLabel;

    [SerializeField]
    Button _displayNameConfirmation;


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
        _displayNameConfirmation.onClick.AddListener(ValidateDisplayNameSize);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckDisplayName()
    {
        ValidateDisplayNameSize();
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
            _lastStudiedText.text = "Last Studied: " + Playfab.LastLogin.ToLocalTime().ToShortDateString();
        }
        else
        {
            _lastStudiedText.text = "";
        }

        _finishedLoadingEvent.Raise();
    }

    void UpdateDisplayName(string n)
    {
        if (n.Length < 3 || n.Length > 25)
        {
            var errorBox = MessageBoxFactory.Create(MessageBoxType.Message, "DisplayName must be between 3 and 25 characters", "Invalid DisplayName");
            errorBox.DisplayMessageBox(errorBox.AutoDestroyMessageBox);
            return;
        }

        GameEvent onUpdateDisplayNameComplete = ScriptableObject.CreateInstance<GameEvent>();
        var confirmBox = MessageBoxFactory.Create(MessageBoxType.Confirmation, $"This will be a permnant change. Would you like to Keep the DisplayName: {n}?", "Confirm Display");
        confirmBox.DisplayConfirmationBox(() => 
        {
            var loadBox = MessageBoxFactory.Create(MessageBoxType.Loading, "Please Wait while game data is loaded", "Updating DisplayName", onUpdateDisplayNameComplete);
            loadBox.DisplayLoadingMessageBox();
            HelperFunctions.Log("Did this even happen? ");
            PlayFabController.DisplayName(n, 
                () => 
                { 
                    onUpdateDisplayNameComplete.Raise();
                    _displayNameConfirmation.gameObject.SetActive(false);
                    MenuController.PanelChangeEnabled = true;
                }, 
                (error)=> 
                { 
                    onUpdateDisplayNameComplete.Raise();
                    MenuController.PanelChangeEnabled = true;
                    ValidateUniqueDisplayName(error);
                }); 
        }, 
        () => { confirmBox.DestroyMessageBox(); });   
    }

    void ValidateDisplayNameSize()
    {
        string n = displayNameField.text;
        if (string.IsNullOrEmpty(n))
        {
            var errorBox = MessageBoxFactory.Create(MessageBoxType.Message, "You must enter a display name", "Missing DisplayName");
            errorBox.DisplayMessageBox(errorBox.AutoDestroyMessageBox);
        }
        else
        {
            _displayNameConfirmation.gameObject.SetActive(false);
            MenuController.PanelChangeEnabled = true;
        }
    }

    void ValidateUniqueDisplayName(PlayFabError error)
    {
        if(error.Error == PlayFabErrorCode.NameNotAvailable || error.Error == PlayFabErrorCode.ProfaneDisplayName)
        {
            var errorBox = MessageBoxFactory.Create(MessageBoxType.Message, "DisplayName is not available. Either taken or profane", "Invalid DisplayName");
            errorBox.DisplayMessageBox(errorBox.AutoDestroyMessageBox);
            MenuController.PanelChangeEnabled = false;
        }
    }
}

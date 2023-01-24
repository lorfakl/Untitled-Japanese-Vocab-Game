using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities;
using Utilities.Events;

public class MessageBoxInstance : MonoBehaviour
{
    [SerializeField]
    Transform panelParent;

    [SerializeField]
    Button _continueButton;

    [SerializeField]
    Button _denyButton;

    [SerializeField]
    Button _confirmButton;

    [SerializeField]
    Button _panelButton;

    [SerializeField]
    Image _loadingIndicator;

    [SerializeField]
    TMP_Text _title;

    [SerializeField]
    TMP_Text _message;

    [SerializeField]
    GameEventListener _gameEventListener;

    MessageBox _owner;
    MessageBoxType _type;
    Action _OnContinueCallback;
    Action _OnDenyCallback;
    Action _OnConfirmCallback;
    
    
    public void ConfigureMessageBox(MessageBox owner, Action OnAcceptCallBack = null, Action OnDenyCallBack = null)
    {
        InitializeMessageBox(owner);
        if (OnAcceptCallBack != null && OnDenyCallBack != null)
        {
            _OnConfirmCallback = OnAcceptCallBack;
            _confirmButton.onClick.AddListener(Confirm);
            _OnDenyCallback = OnDenyCallBack;
            _denyButton.onClick.AddListener(Deny);
        }

    }

    public void ConfigureMessageBox(MessageBox owner, Action OnOKCallback)
    {
        InitializeMessageBox(owner);
        if(OnOKCallback != null)
        {
            _OnContinueCallback = OnOKCallback;
            
        }
        else
        {
            _OnContinueCallback = owner.AutoDestroyMessageBox;
        }
        _panelButton.onClick.AddListener(Continue);
        _continueButton.onClick.AddListener(Continue);
    }


    public bool DestroyMessageBox(MessageBox m)
    {
        if(_owner == m)
        {
            panelParent.gameObject.SetActive(false);
            Destroy(gameObject);
            return true;
        }

        return false;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDisable()
    {
        
    }

    void Deny()
    {
        _OnDenyCallback();
        DestroyMessageBox(_owner);
    }

    void Confirm()
    {
        _OnConfirmCallback();
        DestroyMessageBox(_owner);
    }

    void Continue()
    {
        _OnContinueCallback();
    }

    void InitializeMessageBox(MessageBox owner)
    {
        gameObject.SetActive(false);
        if(owner.OnCloseEvent != null)
        {
            if(_gameEventListener == null)
            {
                HelperFunctions.Log("game pretty sure i explicitily prevented thisd what the fuck");
            }
            else if(owner.OnCloseEvent == null)
            {
                HelperFunctions.Log("How is that even hosspoible");
            }
            else if(owner == null)
            {
                HelperFunctions.Log("How trhe fuck this happen");
            }
            _gameEventListener.LateRegistration(owner.OnCloseEvent, owner.AutoDestroyMessageBox);
        }
        
        _owner = owner;
        _type = owner.MessageType;
        _title.text = owner.Title;
        _message.text = owner.Message;
        TransformBoxToType();
    }

    void TransformBoxToType()
    {
        switch(_type)
        {
            case MessageBoxType.Loading:
                BuildLoadingType();
                break;

            case MessageBoxType.Error:
                BuildErrorType();
                break;

            case MessageBoxType.Message:
                BuildMessageType();
                break;


            case MessageBoxType.Tooltip:
                BuildTooltipType();
                break;

            case MessageBoxType.Confirmation:
                BuildConfirmationType();
                break;
        }
    }

    private void BuildConfirmationType()
    {
        gameObject.SetActive(true);
        _panelButton.enabled = false;
        _continueButton.gameObject.SetActive(false);
        _loadingIndicator.gameObject.SetActive(false);
    }

    private void BuildTooltipType()
    {
        gameObject.SetActive(true);
        _confirmButton.gameObject.SetActive(false);
        _denyButton.gameObject.SetActive(false);
        _loadingIndicator.gameObject.SetActive(false);
    }

    private void BuildMessageType()
    {
        gameObject.SetActive(true);
        _confirmButton.gameObject.SetActive(false);
        _denyButton.gameObject.SetActive(false);
        _loadingIndicator.gameObject.SetActive(false);
    }

    private void BuildErrorType()
    {
        gameObject.SetActive(true);
        _panelButton.enabled = false;
        _continueButton.gameObject.SetActive(false);
        _confirmButton.GetComponent<TMP_Text>().text = "Report";
        _denyButton.GetComponent<TMP_Text>().text = "Try Again";
        _loadingIndicator.gameObject.SetActive(false);
    }

    private void BuildLoadingType()
    {
        gameObject.SetActive(true);
        _panelButton.enabled = false;
        _continueButton.gameObject.SetActive(false);
        _confirmButton.gameObject.SetActive(false);
        _denyButton.gameObject.SetActive(false);

    }
}

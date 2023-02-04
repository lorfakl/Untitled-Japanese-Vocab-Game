using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using UnityEngine;
using ProjectSpecificGlobals;
using Utilities.Events;

namespace Utilities
{
    public enum MessageBoxType
    {
        Message,
        Error,
        Loading,
        Confirmation,
        Tooltip
    }

    public static class MessageBoxFactory
    {
        public static MessageBox Create(MessageBoxType messageBoxType, string message, string title, GameEvent closingEvent=null, Transform parent=null)
        {
            return new MessageBox(messageBoxType, message, title, closingEvent, parent);
        }

        public static MessageBox CreateLoadingBox(string title, string message, GameEvent closingEvent = null, Action continueCallback=null, Transform parent = null, bool useAltParent = false)
        {
            MessageBox msgBox = new MessageBox(MessageBoxType.Loading, message, title, closingEvent, parent);
            msgBox.DisplayLoadingMessageBox(continueCallback, useAltParent);
            return msgBox;
        }

        public static MessageBox CreateConfirmationBox(string title, string message, GameEvent closingEvent = null, Transform parent = null, bool useAltParent = false)
        {
            MessageBox msgBox = new MessageBox(MessageBoxType.Confirmation, message, title, closingEvent, parent);
            msgBox.DisplayLoadingMessageBox(msgBox.AutoDestroyMessageBox, useAltParent);
            return msgBox;
        }

        public static MessageBox CreateToolTip(string title, string message, GameEvent closingEvent = null, Transform parent = null, bool useAltParent = false)
        {
            throw new NotImplementedException();
            MessageBox msgBox = new MessageBox(MessageBoxType.Confirmation, message, title, closingEvent, parent);
            msgBox.DisplayLoadingMessageBox(msgBox.AutoDestroyMessageBox, useAltParent);
            return msgBox;
        }

        public static MessageBox CreateErrorBox(string title, string message, GameEvent closingEvent = null, Transform parent = null, bool useAltParent = false)
        {
            throw new NotImplementedException();
            MessageBox msgBox = new MessageBox(MessageBoxType.Confirmation, message, title, closingEvent, parent);
            msgBox.DisplayLoadingMessageBox(msgBox.AutoDestroyMessageBox, useAltParent);
            return msgBox;
        }

    }

    public class MessageBox
    {
        private MessageBoxType _messageBoxType;
        private string _message;
        private string _title;
        private GameObject _messageBoxPrefab;
        private MessageBoxInstance _instance;
        private Transform _altParent;
        private Transform _mainCanvas;
        private GameEvent _onCloseEvent;

        public MessageBoxType MessageType
        {
            get { return _messageBoxType; }
        }

        public string Message
        {
            get { return _message; }
        }

        public string Title
        {
            get { return _title; }
        }

        public GameEvent OnCloseEvent
        {
            get { return _onCloseEvent; }
        }

        public bool DestroyMessageBox()
        {
            return _instance.DestroyMessageBox(this);
        }

        public void AutoDestroyMessageBox()
        {
            _instance.DestroyMessageBox(this);
        }

        public void DisplayMessageBox(Action continueCallback, bool useAltParent = false)
        {
            SpawnMessageBoxInstance(useAltParent);
            _instance.ConfigureMessageBox(this, continueCallback);
        }

        public void DisplayMessageBox(Action onConfirmCallback, Action onDenyCallback, bool useAltParent = false)
        {
            SpawnMessageBoxInstance(useAltParent);
            _instance.ConfigureMessageBox(this, onConfirmCallback, onDenyCallback);
        }

        public void DisplayLoadingMessageBox(Action continueCallback = null, bool useAltParent = false)
        {
            SpawnMessageBoxInstance(useAltParent);
            _instance.ConfigureMessageBox(this, continueCallback);
        }

        public MessageBox(MessageBoxType messageBoxType, string message, string title, GameEvent closingEvent = null, Transform parent = null)
        {
            _messageBoxType = messageBoxType;
            _message = message;
            _title = title;
            _messageBoxPrefab = Resources.Load<GameObject>("Prefabs/MessageBox");
            if(parent != null)
            {
                _altParent = parent;
            }

            if(closingEvent != null)
            {
                _onCloseEvent = closingEvent;
            }

            _mainCanvas = GameObject.FindGameObjectWithTag(Tags.MainCanvas.ToString()).transform;

            
        }

        private void SpawnMessageBoxInstance(bool useAltParent)
        {
            Transform p = null;
            if (useAltParent)
            {
                p = _altParent;
            }
            else
            {
                p = _mainCanvas;
            }
            HelperFunctions.Log(this);
            _messageBoxPrefab = GameObject.Instantiate(_messageBoxPrefab, p);
            _instance = _messageBoxPrefab.GetComponent<MessageBoxInstance>();
        }

        public override string ToString()
        {
            return _messageBoxType.ToString() + $" Type Messagebox Created. With Title: {_title} \n and Message: {_message}";
        }
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Events
{
    public static class SystemEvent
    {

        #region Public Variables
        /*public struct SystemEventArgs
        {
            public object data;
            public 
        }*/
        #endregion

        #region Private Variables
        private static readonly List<SystemEventListener> eventListeners =
                new List<SystemEventListener>();
        #endregion

        #region Events
        #endregion

        #region Unity Events
        #endregion

        #region Public Methods
        public static void Raise()
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
            {
                eventListeners[i].OnEventRaised();
            }
        }

        public static void Raise(object eventData)
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
            {
                eventListeners[i].OnEventRaiseWithData(eventData);
            }
        }

        public static void RegisterListener(SystemEventListener listener)
        {
            if (!eventListeners.Contains(listener))
            {
                eventListeners.Add(listener);
            }
        }

        public static void UnregisterListener(SystemEventListener listener)
        {
            if (eventListeners.Contains(listener))
            {
                eventListeners.Remove(listener);
            }
        }
        #endregion

     

        #region Private Methods
        #endregion
    }

    public class SystemEventListener
    {
        [Tooltip("Response to invoke when Event is raised.")]
        public Action Response;

        [Tooltip("Response to invoke with required data when Event is raised.")]
        public Action<object> ResponseWithData;

        public void OnEventRaised()
        {
            Response.Invoke();
            //HelperFunctions.Log( + " was raised");
        }

        public void OnEventRaiseWithData(object eventData)
        {
            ResponseWithData.Invoke(eventData);
        }
    }
}
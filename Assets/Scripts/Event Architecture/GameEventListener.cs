using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utilities.Events;
using Utilities;

namespace Utilities.Events
{
    public class GameEventListener : MonoBehaviour
    {

        #region Public Variables
        [Tooltip("Event to register with.")]
        public GameEvent Event;

        [Tooltip("Response to invoke when Event is raised.")]
        public UnityEvent Response;

        [Tooltip("Response to invoke with required data when Event is raised.")]
        public UnityEvent<object> ResponseWithData;

        #endregion

        #region Private Variables
        #endregion

        #region Events
        #endregion

        #region Unity Events
        #endregion

        #region Public Methods
        public virtual void OnEventRaised()
        {
            Response.Invoke();
            //HelperFunctions.Log(Event.name + " was raised");
        }

        public virtual void OnEventRaiseWithData(object eventData)
        {
            ResponseWithData.Invoke(eventData);
        }

        public void LateRegistration(GameEvent e, UnityAction callback)
        {
            Event = e;
            Event.RegisterListener(this);
            Response.AddListener(callback);
            
        }
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            if(Event != null)
            {
                Event.RegisterListener(this);
            }
                
        }

        private void OnDisable()
        {
            if (Event != null)
            {
                Event.UnregisterListener(this);
            }
            
        }
        #endregion

        #region Private Methods
        #endregion
    }
}
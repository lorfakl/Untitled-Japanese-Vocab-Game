using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Events
{
    [CreateAssetMenu]
    public class GameEvent : ScriptableObject
    {

        #region Public Variables
        #endregion

        #region Private Variables
        private readonly List<GameEventListener> eventListeners =
            new List<GameEventListener>();
        #endregion

        #region Events
        #endregion

        #region Unity Events
        #endregion

        #region Public Methods

        public void Raise()
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
            {
                eventListeners[i].OnEventRaised();
            }
        }

        public void Raise(object eventData)
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
            {
                eventListeners[i].OnEventRaiseWithData(eventData);
                eventListeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(GameEventListener listener)
        {
            if (!eventListeners.Contains(listener))
            {
                eventListeners.Add(listener);
            }
        }

        public void UnregisterListener(GameEventListener listener)
        {
            if (eventListeners.Contains(listener))
            {
                eventListeners.Remove(listener);
            }
        }
        #endregion

        #region Unity Methods
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        #endregion

        #region Private Methods
        #endregion
    }
}
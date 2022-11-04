using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Events;
using Utilities.Events;

public class EventListenerTesting : MonoBehaviour
{

    public EventListener[] eventBundles;

    //#endregion

    #region Private Variables
    #endregion

    #region Events
    #endregion

    #region Unity Events
    #endregion

    #region Public Methods
    /*public void OnEventRaised()
    {
        Response.Invoke();
        //HelperFunctions.Log(Event.name + " was raised");
    }

    public void OnEventRaiseWithData(object eventData)
    {
        ResponseWithData.Invoke(eventData);
    }
    #endregion

    #region Unity Methods
    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }*/
    #endregion

    #region Private Methods
    #endregion
}

public class EventBundle: EventListener
{

}

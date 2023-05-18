using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Events;

public class EventTester : MonoBehaviour
{

    #region Public Variables
    #endregion

    #region Private Variables
    [SerializeField]
    GameEvent _finishedLoading;
#endregion

#region Events
#endregion

#region Unity Events
#endregion

#region Public Methods
#endregion

#region Unity Methods
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            _finishedLoading.Raise();
        }
    }
#endregion

#region Private Methods
#endregion
}
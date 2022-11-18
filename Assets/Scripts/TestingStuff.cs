using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Utilities.Events;

public class TestingStuff : MonoBehaviour
{

    #region Public Variables
    #endregion

    #region Private Variables

    #endregion

    #region Events
    [SerializeField]
    GameEvent testingEvent;
#endregion

#region Unity Events
#endregion

#region Public Methods
    public void TestingStudyObjectSelectedEvent(object studyObj)
    {
        StudyObject so = (StudyObject)studyObj;
        HelperFunctions.Log("Selected Word: " + so.Word.ToString());
    }
#endregion

#region Unity Methods
    void Start()
    {
                
    
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space))
        {
            testingEvent.Raise();
            HelperFunctions.Log("SpaceBAR!");
        }
    }
#endregion

#region Private Methods
#endregion
}
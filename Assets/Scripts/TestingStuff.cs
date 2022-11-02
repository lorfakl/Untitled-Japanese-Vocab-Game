using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;


public class TestingStuff : MonoBehaviour
{

#region Public Variables
#endregion

#region Private Variables
    
#endregion

#region Events
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
        
    }
#endregion

#region Private Methods
#endregion
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class ArcadeStudyManager : MonoBehaviour
{
    public void OnGameOverEvent_Handler()
    {
        HelperFunctions.LoadScene(ProjectSpecificGlobals.SceneNames.ArcadeGameOver);
    }
}

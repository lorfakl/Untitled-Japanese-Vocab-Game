using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{

    #region Public Variables
    public static bool BeatSaberMode
    {
        get;
        set;
    }
    
    public static bool FruitNinjaMode
    {
        get;
        set;
    }


    #endregion

    #region Private Variables
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
        if (FruitNinjaMode)
        {
            BeatSaberMode = false;
        }

        if (BeatSaberMode)
        {
            FruitNinjaMode = false;
        }
    }
#endregion

#region Private Methods
    void DisplayWordBank()
    {

    }
#endregion
}
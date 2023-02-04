using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WordLeftCounterEventProcessor : MonoBehaviour
{

    #region Public Variables
    #endregion

    #region Private Variables
    [SerializeField]
    TMP_Text wordsLeftInSessionCounter;

    int wordsRemoved = 0;
    #endregion

    #region Events
    #endregion

    #region Unity Events
    public void WordRemovedFromSessionEventHandler(object count)
    {
        wordsRemoved++;
        int currentCount = (int)count;
        wordsLeftInSessionCounter.text = wordsRemoved + "/" + (currentCount + wordsRemoved);
    }
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
        
        }
    #endregion

    #region Private Methods
    #endregion
}
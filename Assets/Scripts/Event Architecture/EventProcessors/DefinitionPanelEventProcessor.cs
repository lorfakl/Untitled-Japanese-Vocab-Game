using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DefinitionPanelEventProcessor : MonoBehaviour
{

    #region Public Variables
    
    #endregion

    #region Private Variables
    [SerializeField]
    TMP_Text definitionText;
    #endregion

    #region Events
    #endregion

    #region Unity Events
    public void StudyObjectClickedEventHandler()
    {
        definitionText.text = WordBankManager.NextWord.PrintAnswer();
        definitionText.enabled = true;
    }

    public void NextWordButtonClickedEventHandler()
    {
        definitionText.enabled = false;
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
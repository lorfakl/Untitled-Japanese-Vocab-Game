using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CanvasManager : MonoBehaviour
{

#region Public Variables
#endregion

#region Private Variables
    [SerializeField]
    TMP_Text definitionText;

    [SerializeField]
    TMP_Text targetWord;
#endregion

#region Events
#endregion

#region Unity Events
#endregion

#region Public Methods
    public void OnStudyObjectSelected()
    {

    }
#endregion

#region Unity Methods
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if(targetWord.color != Color.white)
        {
            definitionText.text = WordBankManager.NextWord.ToString();
            definitionText.enabled = true;
        }
        else
        {
            definitionText.enabled = false;
        }*/
    }
#endregion

#region Private Methods
#endregion
}
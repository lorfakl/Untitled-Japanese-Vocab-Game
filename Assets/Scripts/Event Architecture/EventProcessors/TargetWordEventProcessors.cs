using System.Collections;
using System.Collections.Generic;
using Utilities.Events;
using UnityEngine;
using TMPro;

public class TargetWordEventProcessors : MonoBehaviour
{

#region Public Variables
    
#endregion

#region Private Variables
    [SerializeField]
    TMP_Text targetWord;
#endregion


#region Public Methods
    public void CorrectAnswerEventProcessorTargetWordGreen()
    {
         targetWord.color = Color.green;
    }

    public void IncorrectAnswerEventProcessorTargetWordRed()
    {
        targetWord.color = Color.red;
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
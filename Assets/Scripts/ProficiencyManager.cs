using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;
using Utilities.Events;
using Utilities.PlayFabHelper;

public class ProficiencyManager : MonoBehaviour
{
    [SerializeField]
    GameEvent correctAnswerEvent;

    [SerializeField]
    GameEvent incorrectAnswerEvent;

    [SerializeField]
    StudySystem ss;

    public void StudyObjectOnClickHandler(object studyObjectClicked)
    {
        StudyObject studyObjSelected = (StudyObject)studyObjectClicked;
        if (studyObjSelected.Word.Kana == WordBankManager.NextWord.Kana) //player chose correct word
        {
            //HelperFunctions.Log("Before Prof Mod: " + studyObjSelected.Word);
            if (correctAnswerEvent != null)
            {
                //HelperFunctions.Log("That answer was correct");
                SetLeitnerLevel(true, WordBankManager.NextWord);
                correctAnswerEvent.Raise(studyObjectClicked);
                correctAnswerEvent.Raise();
                
                
            }
        }
        else
        {
            //HelperFunctions.Log("Before Prof Mod: " + studyObjSelected.Word);
            if (incorrectAnswerEvent != null)
            {
                //HelperFunctions.Log("That answer was incorrect");
                SetLeitnerLevel(false, WordBankManager.NextWord);
                incorrectAnswerEvent.Raise();
                
                return; //return from here to avoid Adding 
                //the word to the LeitnerDict
            }
        }
        //HelperFunctions.Log("After Prof Mod: " + studyObjSelected.Word);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetLeitnerLevel(bool wasCorrect, JapaneseWord word)
    {
        word.ModifyProficiencyLevel(wasCorrect);
        ss.ModifySessionList(word);
    }
}

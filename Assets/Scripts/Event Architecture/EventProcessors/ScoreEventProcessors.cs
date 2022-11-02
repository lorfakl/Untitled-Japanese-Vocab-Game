using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Utilities.Events;
using TMPro;
public class ScoreEventProcessors : MonoBehaviour
{

    #region Public Variables
    #endregion

    #region Private Variables
    [SerializeField]
    TMP_Text scoreText;

    [SerializeField]
    GameEvent scoreUpdatedEvent;

    float averageFlightTime = 8.8f;
    float baseScore = 10;
    int currentScore = 0;
    int comboCount = 0;
    #endregion

    #region Public Methods
    public void CorrectAnswerEventProcessorCalculateScore(object studyObj)
    {
        comboCount++;
        StudyObject correctAnswer = HelperFunctions.CastObject<StudyObject>(studyObj);
        float gravityMultipler = Mathf.Abs(9.81f / Physics.gravity.y);
        int score = (int)(baseScore * (averageFlightTime / correctAnswer.TimeInFlight) * gravityMultipler * comboCount);
        currentScore += score;
        scoreText.text = currentScore.ToString();
        if(scoreUpdatedEvent != null)
        {
            scoreUpdatedEvent.Raise();
        }
    }

    public void IncorrectAnswerEventHandlerRemoveCombo()
    {
        comboCount = 0;
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
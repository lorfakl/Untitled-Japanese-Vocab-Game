using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Utilities;
using Utilities.Events;
using TMPro;
using Utilities.PlayFabHelper;
using Utilities.PlayFabHelper.CurrentUser;
using Utilities.PlayFabHelper.CSArguments;

public class ScoreEventProcessors : MonoBehaviour
{

    #region Public Variables
    public static int Score
    {
        get
        {
            return currentScore;
        }

        private set
        {

        }
    }

    public static int ScoreInterval { get; private set; }
    #endregion

    #region Private Variables
    [SerializeField]
    TMP_Text scoreText;

    [SerializeField]
    GameEvent scoreUpdatedEvent;

    float averageFlightTime = 8.8f;
    float baseScore = 10;
    static int currentScore = 0;
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
        ScoreInterval += score;
        scoreText.text = currentScore.ToString();
        if(scoreUpdatedEvent != null)
        {
            scoreUpdatedEvent.Raise();
        }
    }

    public void ResetScoreIntervalEvent_Handler()
    {
        ScoreInterval = 0;
    }

    public void IncorrectAnswerEventHandlerRemoveCombo()
    {
        comboCount = 0;
    }

    public void GameOverEvent_Handler()
    {
        SaveScore();
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

    private void SaveScore()
    {
        
        
    }
    #endregion

    #region Private Methods
    #endregion
}
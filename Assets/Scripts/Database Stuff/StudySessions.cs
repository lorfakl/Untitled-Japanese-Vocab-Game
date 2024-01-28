using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YokaiData.Tables
{
    public class StudySessions
    {
        public static readonly string Table = "StudySessions";

        public int ID {  get; set; }  
        public DateTime Timestamp { get; set; }
        public float TimeToComplete { get; set; }
        public int NumberOfWords { get; set; }
        public float PercentCorrect { get; set; }
        public int Score { get; set; }
        public float AverageAnswerSpeed { get; set; }
        public List<string> WordsStudied { get; set; }

        public StudySessions(int iD, DateTime timestamp, float timeToComplete, int numberOfWords, float percentCorrect, int score, float averageAnswerSpeed, List<string> wordsStudied)
        {
            ID = iD;
            Timestamp = timestamp;
            TimeToComplete = timeToComplete;
            NumberOfWords = numberOfWords;
            PercentCorrect = percentCorrect;
            Score = score;
            AverageAnswerSpeed = averageAnswerSpeed;
            WordsStudied = wordsStudied;
        }   
    }
}


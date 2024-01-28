using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YokaiData.Tables
{
    public class WordStats
    {
        public static readonly string Table = "StudySessions";

        public int ID { get; set; }
        public DateTime Timestamp { get; set; }
        public string WordID { get; set; }
        public int TimesSeen { get; set; }
        public int TimesCorrect { get; set; }
        public float PercentCorrect { get; set; }
        public float SlowestAnswerSpeed { get; set; }
        public float FastestAnswerSpeed { get; set; }
        public float AverageAnswerSpeed { get; set; }

        public WordStats(int iD, DateTime timestamp, string wordID, int timesSeen, int timesCorrect, float percentCorrect, float slowestAnswerSpeed, float fastestAnswerSpeed, float averageAnswerSpeed)
        {
            ID = iD;
            Timestamp = timestamp;
            WordID = wordID;
            TimesSeen = timesSeen;
            TimesCorrect = timesCorrect;
            PercentCorrect = percentCorrect;
            SlowestAnswerSpeed = slowestAnswerSpeed;
            FastestAnswerSpeed = fastestAnswerSpeed;
            AverageAnswerSpeed = averageAnswerSpeed;
        }   
    }
}


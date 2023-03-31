using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class ArcadeStudyManager : MonoBehaviour
{
    static ArcadePlayer CurrentPlayer
    {
        get;
        set;
    }

    static Dictionary<Guid, ArcadePlayer> arcadePlayers = new Dictionary<Guid, ArcadePlayer>();
    bool isPlaying;

    public static ArcadePlayer GetArcadePlayer(Guid id)
    {
        if(arcadePlayers.ContainsKey(id))
        {
            return arcadePlayers[id];
        }

        return default;
    }

    public static void AddArcadePlayer(string name)
    {
        ArcadePlayer a = new ArcadePlayer
        {
            displayName = name,
            guid = Guid.NewGuid(),
            rank = 0,
            score = 0,
            totalRunTime = 0,
            totalWordsCorrect = 0,
            totalWordsSeen = 0
        };

        if(!arcadePlayers.ContainsKey(a.guid))
        {
            arcadePlayers.Add(a.guid, a);
            CurrentPlayer = a;
        }

        HelperFunctions.Log(a);
    }

    public static Dictionary<Guid, ArcadePlayer> ArcadePlayers
    {
        get { return arcadePlayers; }
        private set { }
    }

    public void OnGameOverEvent_Handler()
    {
        HelperFunctions.LoadScene(ProjectSpecificGlobals.SceneNames.ArcadeGameOver);
    }

    public void OnStudyObjectSelect_Handler()
    {
        CurrentPlayer.totalWordsSeen++;
        HelperFunctions.Log(CurrentPlayer);
    }

    public void OnCorrectAnswer_Handler()
    {
        CurrentPlayer.totalWordsCorrect++;
    }

    public void OnNextWordButton_Handler()
    {
        isPlaying = true;
    }

    public void OnScoreUpdated_Handler()
    {
        CurrentPlayer.score += ScoreEventProcessors.Score;
    }

    public void OnRemovedFromSessionList_Handler()
    {
        if(JSONWordLibrary.WordsToStudy.Count == 0)
        {
            isPlaying = false;
        }
    }
    private void Start()
    {
        List<JapaneseWord> arcadeStudy = new List<JapaneseWord>();
        for (int i = 0; i < 100; i++)
        {
            arcadeStudy.Add(JSONWordLibrary.WordsFromFile[i]);
        } 
        JSONWordLibrary.SetWordsToStudy(arcadeStudy);
    }

    private void Update()
    {
        if(isPlaying)
        {
            CurrentPlayer.totalRunTime += Time.deltaTime;
        }
        
    }

    private void OnDisable()
    {
        
    }

    [Serializable]
    public class ArcadePlayer
    {
        public Guid guid;
        public string displayName;
        public int score;
        public int rank;
        public float totalRunTime;
        public int totalWordsCorrect;
        public int totalWordsSeen;

        public override string ToString()
        {
            return $"GUID: {guid} " + "\n" +
                $"DisplayName: {displayName} " + "\n" +
                $"Score: {score} " + "\n" +
                $"Rank: {rank} " + "\n" +
                $"Total Run Time: {totalRunTime} " + "\n" +
                $"Total Words Seen: {totalWordsSeen} " + "\n" +
                $"Total Words Correct: {totalWordsCorrect} " + "\n" +
                $"Percentage Correct: {((float)totalWordsCorrect / (float)totalWordsSeen) * 100} ";
        }
    }

    [Serializable]
    public class ArcadeLeaderboard
    {
        List<ArcadePlayer> leaderboard = new List<ArcadePlayer>();

        public ArcadeLeaderboard()
        {

        }

        public ArcadeLeaderboard(List<ArcadePlayer> leaderboard)
        {
            this.leaderboard = leaderboard;
        }

        //public void 
    }
}

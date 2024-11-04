using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectSpecificGlobals
{
    public enum SceneNames { MenuScene, StudyScene, ArcadeLeaderboard, ArcadeOpeningScene, ArcadeStudyScene, ArcadeGameOver }

    public enum Tags { MainCanvas }

    public static class Globals
    {
        private static bool isStudyRecordLoaded = false;
        private static bool areAllWordsLoaded = false;
        public static int MaxStatOverTimeSize { get { return 7; } }
        public static StudyRecord LoadedStudyRecord { get; private set; }
        public static Dictionary<string, JapaneseWord> GlobalWordDict { get; private set; }
        public static bool UserDataLoaded 
        { 
            get { 
                    if(LoadedStudyRecord == null || LoadedStudyRecord == default(StudyRecord))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                } 
        }


        public static List<JapaneseWord> AllWords { get { return LoadAllWords().Values.ToList(); } }

        public static void UpdateGlobalStudyRecord(StudyRecord record)
        {
            if(!isStudyRecordLoaded) 
            { 
                LoadedStudyRecord = record;
                isStudyRecordLoaded = true;
            }
            else
            {
                if(!UserDataLoaded) 
                {
                    throw new System.Exception($"Static user data is not loaded, but isStudyRecordLoaded is reporting true: {isStudyRecordLoaded} while UserDataLoaded is reporting false: {UserDataLoaded} something insane has happened");
                }
                return;
            }
        }

        public static Dictionary<string, JapaneseWord> LoadAllWords()
        {
            if(!areAllWordsLoaded)
            {
                GlobalWordDict = new Dictionary<string, JapaneseWord>();
                TextAsset wordList = Resources.Load<TextAsset>("japaneseWordList");
                string wordFile = wordList.text;
                List<JapaneseWord> wordObjs = JsonConvert.DeserializeObject<List<JapaneseWord>>(wordFile);
                foreach(JapaneseWord wordObj in wordObjs )
                {
                    if(!GlobalWordDict.ContainsKey(wordObj.ID))
                    {
                        GlobalWordDict.Add(wordObj.ID, wordObj);
                    }
                }

                areAllWordsLoaded = true;

            }
            return GlobalWordDict;
        }
    }

}

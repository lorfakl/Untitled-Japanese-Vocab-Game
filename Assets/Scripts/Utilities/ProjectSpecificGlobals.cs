using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSpecificGlobals
{
    public enum SceneNames { MenuScene, StudyScene, ArcadeLeaderboard, ArcadeOpeningScene, ArcadeStudyScene, ArcadeGameOver }

    public enum Tags { MainCanvas }

    public static class Globals
    {
        private static bool isStudyRecordLoaded = false;

        public static int MaxStatOverTimeSize { get { return 7; } }
        public static StudyRecord LoadedStudyRecord { get; private set; }
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
    }

}

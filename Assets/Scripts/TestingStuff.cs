using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Utilities.Events;
using Utilities.PlayFabHelper;
using Utilities.SaveOperations;
using Utilities.PlayFabHelper.CurrentUser;
using Utilities.PlayFabHelper.CSArguments;

public class TestingStuff : MonoBehaviour
{

    #region Public Variables
    #endregion

    #region Private Variables
    bool hasuploadedFile = true;
    #endregion

    #region Events
    [SerializeField]
    GameEvent testingEvent;
#endregion

#region Unity Events
#endregion

#region Public Methods
    public void TestingStudyObjectSelectedEvent(object studyObj)
    {
        StudyObject so = (StudyObject)studyObj;
        HelperFunctions.Log("Selected Word: " + so.Word.ToString());
    }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        
    }
    void Start()
    {
        //TestSaving(); 
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space))
        {
            testingEvent.Raise();
            HelperFunctions.Log("SpaceBAR!");
        }
        if(CurrentAuthedPlayer.CurrentUser != null)
        {
            //RequestFileInfomation(CurrentAuthedPlayer.CurrentUser.EntityKey);
            if (!hasuploadedFile)
            {
                PlayFabController.InitiateFileUploads(CurrentAuthedPlayer.CurrentUser.EntityKey, "avatar");
                hasuploadedFile = true;
            }
        }
        
    }
#endregion

#region Private Methods
    void RequestFileInfomation(UniversalEntityKey e)
    {
        PlayFabController.GetFileInfo(e, PrintFileInfomation);
    }
    
    void PrintFileInfomation(List<PlayFabFileInfo> files)
    {
        HelperFunctions.LogListContent(files);
    }
    
    void TestSaving()
    {
        BasicProfile b = new BasicProfile("yourmon bro", "im the dad that stepped up", "deeznuts", 
            new Dictionary<StatisticName, CloudScriptStatArgument>
            { 
                [StatisticName.TotalSP] = new CloudScriptStatArgument(StatisticName.TotalSP, 874561)
            });

        HelperFunctions.Log(b);
        SaveSystem.Save<BasicProfile>(b, DataCategory.Profile);


        BasicProfile notB = SaveSystem.Load<BasicProfile>(DataCategory.Profile);
        HelperFunctions.Log("These the same: " + b.Equals(notB));
        HelperFunctions.Log("Heeeeres B: " + "\n" + notB.Statistics[StatisticName.TotalSP].DecodeStatisticValue());
    }

#endregion
}
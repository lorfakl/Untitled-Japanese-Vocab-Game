using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Utilities.PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using System;


public delegate void IsAuthenticatedNotification();

#region PlayFabHelperEnumsandStructs
public enum UserDataKey
{
    LeitnerLevels,
    PrestigeLevels,
    SessionWords,
    LoginCount,
    NextSession,
    WordsSeen
}

public enum TitleDataKeys
{
    StarterWords,
    CommonWords
}

public enum StatisticName
{
    LeagueSP,
    MonthlySP,
    WordsSeen,
    WordsMastered,
    StudyStreak
}

public enum CSFunctionNames
{
    TestingFunc,
    FirstTimeWordSetup,
    BuildSessionList,
    SetLoginStatus,
    AddNewWords,
    UpdateTag
}

public enum PlayerTags
{
    InGroup
}

public struct UniversalEntityKey
{
    public string id;
    public string type;
}
#endregion
public class PlayFabController : MonoBehaviour
{

    #region Public Variables
    public static bool IsAuthenticated
    {
        get;
        private set;
    }
    #endregion

    #region Private Variables
    [SerializeField]
    bool useRandomAccounts = false;

    static string emptyLeitnerLevelData = @"{'Zero':[],'One': [],'Two': [],'Three': [],'Four': [],'Five': []}";
    static string emptyProfeincyLevelData = @"{'One': [],'Two': [],'Three': [],'Four': [],'Five': [],'Six': [],'Seven': [],'Eight': []}";
    static TimeSpan twelveHours = new TimeSpan(12, 0, 0);
    static TimeSpan twoMinutes = new TimeSpan(0, 2, 0);

    #endregion

    #region Events
    public static event IsAuthenticatedNotification IsAuthedEvent;

    #endregion

    #region Unity Events
    #endregion

    #region Public Methods
        public static void GetPlayerData(List<string> dataKeys, Action<Dictionary<string,string>> success)
        {
            GetUserDataRequest getUserDataRequest = new GetUserDataRequest()
            {
                PlayFabId = Playfab.PlayFabID,
                Keys = dataKeys
            };

            Playfab.GetUserData(getUserDataRequest, 
                (result)=> 
                {
                    Dictionary<string, string> returnData = new Dictionary<string, string>();
                    foreach(string key in result.Data.Keys)
                    {
                        returnData.Add(key, result.Data[key].Value);
                    }

                    success(returnData);
                }, 
                OnPlayFabError);
        }
    
        public static void UpdateUserData(Dictionary<string, string> data)
        {
            Playfab.UpdateUserData(new UpdateUserDataRequest
            {
                Data = data
            },
            (result) =>
            {
                HelperFunctions.Log("Successfully saved to PF");
            },
            OnPlayFabError);
        }

        public static void ExecutionCSFunction(CSFunctionNames functionName, Dictionary<string, string> parameter, bool shouldGeneratePlaystream, Action<PlayFab.CloudScriptModels.ExecuteFunctionResult> action = null)
        {
            Playfab.ExecuteFunction(new ExecuteFunctionRequest
            {
                FunctionName = functionName.ToString(),
                FunctionParameter = parameter,
                GeneratePlayStreamEvent = shouldGeneratePlaystream
            }, (result)=>
            { 
                if(action != null)
                {
                    action(result);
                }
            }, OnPlayFabError);
        }

        public static void GetPlayerTags(Action<GetPlayerTagsResult> success)
        {
            GetPlayerTagsRequest rq = new GetPlayerTagsRequest
            {
                PlayFabId = Playfab.PlayFabID
            };
            Playfab.GetTags(rq, success, OnPlayFabError);
        }

        public static void GetLeadboard(bool islocal, StatisticName name, Action<List<PlayerLeaderboardEntry>> sucCallback)
        {
            Playfab.GetLeaderboard(islocal, name, sucCallback, OnPlayFabError);
            
        }
    
        public static void GetTitlePlayerAccounts(List<string> playfabIDs, Action<Dictionary<string, UniversalEntityKey>> success)
        {
            Playfab.GetTitlePlayerIDs(playfabIDs, 
                (result) => 
                { 
                    Dictionary<string, UniversalEntityKey> dict = new Dictionary<string, UniversalEntityKey>();
                    foreach(KeyValuePair<string, PlayFab.ProfilesModels.EntityKey> kvp in result.TitlePlayerAccounts)
                    {
                        UniversalEntityKey entityKey = new UniversalEntityKey
                        {
                            id = kvp.Value.Id,
                            type = kvp.Value.Type
                        };
                        dict.Add(kvp.Key, entityKey);
                    }
                    success(dict);

                }, OnPlayFabError);
        }
    
        public static void CreateGroup(string grpName, Action<PlayFab.GroupsModels.CreateGroupResponse> success)
        {
            PlayFab.GroupsModels.CreateGroupRequest rq = new PlayFab.GroupsModels.CreateGroupRequest
            {
                GroupName = grpName
            };
            Playfab.CreateGroup(rq, 
                (result) => 
                { 
                    Dictionary<string , string> parameterDict = new Dictionary<string , string>();
                    parameterDict.Add("Id", Playfab.PlayFabID);
                    parameterDict.Add("TagName", PlayerTags.InGroup.ToString());

                    ExecutionCSFunction(CSFunctionNames.UpdateTag, parameterDict, Playfab.ArePlayStreamEventsGenerated);
                    success(result);
                }, OnPlayFabError);
        }

        
        //public static void ()
        //public static void ()
        //public static void ()
        //public static void ()
        //public static void ()
        //public static void ()
        //public static void ()
        //public static void ()
        //public static void ()
        //public static void ()
        //public static void ()
        //public static void ()
        //public static void ()
        //public static void ()
        //public static void ()
        //public static void ()
        //public static void ()
        //public static void ()
        //public static void ()
        //public static void ()
        //public static void ()
        //public static void ()
        //public static void ()
        //public static void ()
        //public static void ()
        //public static void ()
        //public static void ()
        //public static void ()
        //public static void ()
        //public static void ()
        //public static void ()
    #endregion

    #region Unity Methods
        void Awake()
        {
        
        }
        
        void Start()
        {
            if(useRandomAccounts)
            {
                Playfab.Login(OnSuccessfulLogin, OnPlayFabError, useRandomAccounts);
            }
            else
            {
                Playfab.Login(OnSuccessfulLogin, OnPlayFabError);
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    #endregion

    #region Private Methods
        static void OnSuccessfulLogin(LoginResult result)
        {
            IsAuthenticated = true;
            if(result.NewlyCreated)
            {
                FirstTimeLoginDataTransfer();
                return;
            }
       

            TimeSpan deltaTime = (TimeSpan)(DateTime.UtcNow - result.LastLoginTime);
            HelperFunctions.Log("Delta time since last login" + deltaTime);
            string l = "Detla Hours: " + deltaTime.Hours + "\n" + "Detla Minutes: " + deltaTime.Minutes + "\n" + "Detla Seconds: " + deltaTime.Seconds;
            HelperFunctions.Log(l);
        
            if(deltaTime > twelveHours)
            {
                Playfab.ExecuteFunction(new ExecuteFunctionRequest
                {
                    FunctionName = "SetLoginStatus",
                    GeneratePlayStreamEvent = true
                },
                (result) =>
                {
                    IsAuthedEvent?.Invoke();
                }, OnPlayFabError);

            }
            else
            {
                IsAuthedEvent?.Invoke();
            }

        
        }


        static void FirstTimeLoginDataTransfer()
        {
            GetTitleDataRequest rq = new GetTitleDataRequest
            {
                Keys = new List<string> { TitleDataKeys.StarterWords.ToString() }
            };

            Playfab.GetTitleData(rq, OnSuccessfulFirstTimeTitleData, OnPlayFabError);
        }

        static void OnSuccessfulFirstTimeTitleData(GetTitleDataResult result)
        {
            Playfab.UpdateUserData(new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string> 
                {
                    {UserDataKey.SessionWords.ToString(), result.Data[TitleDataKeys.StarterWords.ToString()]},
                    {UserDataKey.LeitnerLevels.ToString(), emptyLeitnerLevelData},
                    {UserDataKey.PrestigeLevels.ToString(), emptyProfeincyLevelData},
                    {UserDataKey.LoginCount.ToString(), "0" },
                    {UserDataKey.NextSession.ToString(),  ""},
                    {UserDataKey.WordsSeen.ToString(), "20" }
                }
            },
            (result) =>
            {
                IsAuthedEvent?.Invoke();
            },
            OnPlayFabError); 
        }

        static void OnPlayFabError(PlayFab.PlayFabError error)
        {
            //Pass to Retry Engine
            HelperFunctions.Log(Playfab.DisplayPlayFabError(error));
        }
    
    #endregion
}
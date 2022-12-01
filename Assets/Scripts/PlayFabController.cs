using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Utilities.PlayFabHelper;
using Utilities.PlayFabHelper.CSArguments;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using System;
using PlayFab.GroupsModels;
using PlayFab.ProfilesModels;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public delegate void IsAuthenticatedNotification();

public class PlayFabController : MonoBehaviour
{

    #region Public Variables
    public static bool IsAuthenticated
    {
        get;
        private set;
    }

    public LocalPlayFabData LocalPlayFabData
    {
        get { return localData; }
    }

    #endregion

    #region Private Variables
    [SerializeField]
    bool useRandomAccounts = false;

    [SerializeField]
    LocalPlayFabData localData;

    static string emptyLeitnerLevelData = @"{'Zero':[],'One':[],'Two':[],'Three':[],'Four':[],'Five':[]}";
    static string emptyProfeincyLevelData = @"{'One': [],'Two': [],'Three': [],'Four': [],'Five': [],'Six': [],'Seven': [],'Eight': []}";
    static TimeSpan twelveHours = new TimeSpan(12, 0, 0);
    static TimeSpan twoMinutes = new TimeSpan(0, 2, 0);
    private static PlayFabController _instance;
    public PlayFabController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = this;
            }
            return _instance;
        }
    }
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

        public static void ExecutionCSFunction<TKey, TValue>(CSFunctionNames functionName, Dictionary<TKey,TValue> parameter, bool shouldGeneratePlaystream, Action<PlayFab.CloudScriptModels.ExecuteFunctionResult> action = null)
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

        public static void ExecutionCSFunction(CSFunctionNames functionName, object parameter, Action<PlayFab.CloudScriptModels.ExecuteFunctionResult> action = null)
        {
            Playfab.ExecuteFunction(new ExecuteFunctionRequest
            {
                FunctionName = functionName.ToString(),
                FunctionParameter = parameter,
                GeneratePlayStreamEvent = Playfab.ArePlayStreamEventsGenerated
            }, (result) =>
            {
                if (action != null)
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
                            ID = kvp.Value.Id,
                            Type = kvp.Value.Type
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
                    HelperFunctions.Log("Group Entity Key ID: " + result.Group.Id);
                    HelperFunctions.Log("Group Entity Key Type: " + result.Group.Type);
                    success(result);
                }, OnPlayFabError);
        }

        
        public static void AddToGroup(List<UniversalEntityKey> membersToAdd, UniversalEntityKey grpKey = null)
        {
            List <PlayFab.GroupsModels.EntityKey> convertedList = new List<PlayFab.GroupsModels.EntityKey>();
            foreach(UniversalEntityKey memberKey in membersToAdd)
            {
                convertedList.Add(memberKey);
            }

            AddMembersRequest rq = new AddMembersRequest
            {
                Group = Playfab.GroupEntityKey,
                Members = convertedList
            };

            Playfab.AddMembers(rq, OnPlayFabError);
        }

        
        public static void GetGroupMembers(UniversalEntityKey groupKey, Action<List<BasicProfile>> callback)
        {
            
            ListGroupMembersRequest rq = new ListGroupMembersRequest
            {
                Group = new PlayFab.GroupsModels.EntityKey
                {
                    Id = groupKey.ID,
                    Type = groupKey.Type
                }
            };
            Playfab.ListGroupMembers(rq, (result) =>
            {
                List<UniversalEntityKey> memberKeys = new List<UniversalEntityKey>();
            List<string> memberPfIds = new List<string>();
                foreach (EntityMemberRole role in result.Members)
                {
                    foreach (EntityWithLineage member in role.Members)
                    {
                        memberKeys.Add((UniversalEntityKey)member.Key);
                        string id = member.Lineage[EntityTypes.master_player_account.ToString()].Id;
                        if(!memberPfIds.Contains(id))
                        {
                            memberPfIds.Add(id);
                            HelperFunctions.Log(id);
                        }

                    
                        
                    }
                }

                List<PlayFab.ProfilesModels.EntityKey> entityKeys = new List<PlayFab.ProfilesModels.EntityKey>();
                foreach(var mKey in memberKeys)
                {
                    entityKeys.Add(mKey);
                }
                //GetPlayerProfileRequest 
                PlayerProfileViewConstraints profileViewConstraints = new PlayerProfileViewConstraints
                {
                    ShowAvatarUrl = true,
                    ShowDisplayName = true,
                    ShowStatistics = true,
                    ShowTags = true
                };

                ExecuteFunctionRequest getProfilesCS = new ExecuteFunctionRequest
                {
                    FunctionName = CSFunctionNames.GetProfile.ToString(),
                    FunctionParameter = new ProfileCSArgument 
                    { 
                        PlayFabIDs = memberPfIds,
                        ProfileConstraints = profileViewConstraints
                    },
                    GeneratePlayStreamEvent = Playfab.ArePlayStreamEventsGenerated
                };

                

                Playfab.ExecuteFunction(getProfilesCS, (profilesResult) =>
                {
                    List<BasicProfile> profileLeaderboard = new List<BasicProfile>();
                    string functionResultJson = profilesResult.FunctionResult.ToString();
                    profileLeaderboard = ParseGetProfileFunctionResponse(functionResultJson);
                    callback(profileLeaderboard);
                   
                }, OnPlayFabError);
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
    #endregion

    #region Unity Methods
        void Awake()
        {
            if(Instance != this)
            {
                HelperFunctions.Log("Destroying newly created PlayFab Controller object");
                Destroy(this.gameObject);
                return;
            }
            DontDestroyOnLoad(this.gameObject);
            HelperFunctions.Log("Does awake get called twice?");
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
            HelperFunctions.Log(error.ApiEndpoint + " " + error.GenerateErrorReport());
        }

        static List<BasicProfile> ParseGetProfileFunctionResponse(string jsonString)
        {
            HelperFunctions.Log(jsonString);
            List<BasicProfile> profiles = new List<BasicProfile>();
            JArray functionResult = JArray.Parse(jsonString);
            foreach(JObject o in functionResult.Children<JObject>())
            {
                var d = JsonConvert.DeserializeObject<Dictionary<StatisticName, CloudScriptStatArgument>>(o["statistics"].ToString());
                BasicProfile p = new BasicProfile(o["avatarURL"].ToString(), o["playfabID"].ToString(), o["displayName"].ToString(), d);
                if (!profiles.Any(profile => profile.PlayFabID == p.PlayFabID))
                {
                    profiles.Add(p);
                }
            }
            HelperFunctions.Log($"Profiles created: {profiles.Count}");
            return profiles;
        }
    
    #endregion
}




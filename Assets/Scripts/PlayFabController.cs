using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Utilities.PlayFabHelper;
using Utilities.PlayFabHelper.CSArguments;
using Utilities.PlayFabHelper.CurrentUser;
using Utilities.SaveOperations;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using System;
using PlayFab.GroupsModels;
using PlayFab.DataModels;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using PlayFab.EventsModels;
using PlayFab;

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
    bool overrideLogin;


    static bool overrideTimedLogin;

    [SerializeField]
    LocalPlayFabData localData;

    [SerializeField]
    string customID;

    static string emptyLeitnerLevelData = @"{'Zero':[],'One':[],'Two':[],'Three':[],'Four':[],'Five':[]}";
    static string emptyProfeincyLevelData = @"{'One': [],'Two': [],'Three': [],'Four': [],'Five': [],'Six': [],'Seven': [],'Eight': []}";
    static TimeSpan twelveHours = new TimeSpan(5, 0, 0);
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
    public static void DisplayName(string name, Action success = null, Action<PlayFabError> error = null)
    {
        Playfab.UpdateDisplayName(name,
            (failure)=> 
            { 
                if(error != null)
                {
                    error(failure);
                }
                OnPlayFabError(failure); 
            }, success);
    }

    public static void ArcadeLogin(string id, Action success)
    {
        Playfab.ArcadeLogin(id, (result) => 
        { 
            OnSuccessfulLogin(result);
            success();
        }, OnPlayFabError);
    }

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
            FunctionParameter = JsonConvert.SerializeObject(parameter),
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
    
    public static void CreateGroup(string grpName, Action<CreateGroupResponse> success)
    {
        CreateGroupRequest rq = new CreateGroupRequest
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
                CurrentAuthedPlayer.CurrentUser.UpdateGroup(new PlayFabGroup((UniversalEntityKey)result.Group, grpName));
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

        //Playfab.AddMembers(rq, SaveGroupMembers,  OnPlayFabError);
    }

    
    public static void GetGroupMembers(UniversalEntityKey groupKey, Action<List<BasicProfile>> callback, Action<PlayFabError> error = null)
    {
        HelperFunctions.Log("Group ID from CU: " + groupKey.ID);
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
        }, 
        (failure)=> 
        { 
            if(error != null)
            {
                error(failure);
            }
            OnPlayFabError(failure);
        });
            
    }
    public static void InitiateFileUploads(UniversalEntityKey e, string fileName)
    {
        InitiateFileUploadsRequest rq = new InitiateFileUploadsRequest
        {
            Entity = e,
            FileNames = new List<string> { fileName }
        };

        Playfab.InitiateFileUploads(rq, PerformUpload, OnPlayFabError);
    }

    public static void InitiateFileUpload(UniversalEntityKey e, DataCategory d, string fileName)
    {
        InitiateFileUploadsRequest rq = new InitiateFileUploadsRequest
        {
            Entity = e,
            FileNames = new List<string> { fileName }
        };

        Playfab.InitiateFileUploads(rq, (result) => 
        {
            Playfab.UploadEntityFile(result, d, OnHTTPError);
        }, OnPlayFabError);
    }

    public static void UploadAvatarImage()
    {
        InitiateFileUploadsRequest rq = new InitiateFileUploadsRequest
        {
            Entity = Playfab.UserEntityKey,
            FileNames = new List<string> { "AvatarImage" }
        };

        Playfab.InitiateFileUploads(rq,
            (res) =>
            {
                Playfab.UploadEntityFile(res, DataCategory.Avatar, OnHTTPError);
            }, OnPlayFabError);
    }

    public static void PerformUpload(InitiateFileUploadsResponse res)
    {
        Playfab.UploadEntityFile(res, DataCategory.User, OnHTTPError);
    }

    public static void GetAvatarImage()
    {
        GetFileInfo(Playfab.UserEntityKey, GetAvatarImage);
    }

    public static void GetAvatarImage(UniversalEntityKey e)
    {
        //GetFileInfo(e,)
    }

    public static void GetFileInfo(UniversalEntityKey e, Func<List<PlayFabFileInfo>, Task<Sprite>> success)
    {
        GetFilesRequest rq = new GetFilesRequest
        {
            Entity = e
        };

        Playfab.GetFiles(rq, OnPlayFabError, (files) => 
        { 
            success(files);
        });
    }

    public static void GetFileInfo(UniversalEntityKey e, Action<List<PlayFabFileInfo>> success)
    {
        GetFilesRequest rq = new GetFilesRequest
        {
            Entity = e
        };

        Playfab.GetFiles(rq, OnPlayFabError, success);
    }

    public static void PerformDownload(string url, Action<byte[]> success)
    {
        Playfab.DownloadFileFromPlayFab(url, success, OnHTTPError);
    }

    public static Task<Sprite> PerformDownload(string url, Func<byte[], Task<Sprite>> success)
    {
        return Playfab.DownloadandConvertToSprite(url, success, OnHTTPError);
    }

    public static void GetItemCatalog(Action<List<PlayFabItem>> success)
    {
        Playfab.GetCatalogItems(success, OnPlayFabError);
    }
    
    public static void WriteTelemetryEvents(List<TelemetryWrapper> events, Action success = null)
    {
        if(events.Count == 0 || events == null)
        {
            return;
        }

        List<EventContents> eventContents = new List<EventContents>();
        foreach(var e in events)
        {
            eventContents.Add(e);
        }

        WriteEventsRequest rq = new WriteEventsRequest{ Events = eventContents };
        Playfab.WriteTelemetryEvents(rq, OnPlayFabError, success);
    }
    
    public static void ListGroupMembership(Action<List<PlayFabGroup>> success = null)
    {
        Playfab.ListGroupMembership(OnPlayFabError, success);
    }

    public static void DeleteAllGroups()
    {
        Playfab.ListGroupMembership(OnPlayFabError, 
            (groups) => 
            { 
                foreach(var group in groups)
                {
                    DeleteGroup(group);
                }
            });
    }

    public static void DeleteGroup(PlayFabGroup g)
    {
        DeleteGroupRequest rq = new DeleteGroupRequest
        {
            Group = g.EntityKey
        };

        Playfab.DeleteGroup(rq, OnPlayFabError);
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
#if UNITY_EDITOR
        overrideTimedLogin = overrideLogin;
#endif     
    }
        
    void Start()
    {

#if UNITY_EDITOR
        if (useRandomAccounts)
        {
            Playfab.Login(OnSuccessfulLogin, OnPlayFabError, useRandomAccounts);
        }
        else if(!string.IsNullOrEmpty(customID))
        {
            Playfab.Login(customID, OnSuccessfulLogin, OnPlayFabError);
        }
        else
        {
            Playfab.Login(OnSuccessfulLogin, OnPlayFabError);
        }
        return;

#else
        Playfab.Login(OnSuccessfulLogin, OnPlayFabError);
#endif


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    private void OnDisable()
    {
        SaveSystem.Save(CurrentAuthedPlayer.CurrentUser, DataCategory.User);
        SaveSystem.Save(CurrentAuthedPlayer.CurrentUser.Group, DataCategory.Group);
    }

    #endregion

    #region Private Methods

    static Task<Sprite> GetAvatarImage(List<PlayFabFileInfo> files)
    {
        string fileDownldUrl = "";
        foreach(var f in files)
        {
            if(f.FileName.Contains("Image"))
            {
                fileDownldUrl = f.DownloadUrl;
                break;
            }
        }

        return PerformDownload(fileDownldUrl, SaveSystem.ConvertBytesToSprite);
    }
    static void SaveGroupMembers(List<BasicProfile> basicProfiles)
    {
        CurrentAuthedPlayer.CurrentUser.Group.Add(basicProfiles);
    }

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
        //HelperFunctions.Error("PlayFabCOntroller LoginStatis is set every login NOT FOR PROD");
        if(deltaTime > twelveHours || overrideTimedLogin)
        {
            Playfab.ExecuteFunction(new ExecuteFunctionRequest
            {
                FunctionName = CSFunctionNames.SetLoginStatus.ToString(),
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

        if(CurrentAuthedPlayer.CurrentUser == null)
        {
            var user = SaveSystem.Load<PlayFabUser>(DataCategory.User);
            if (user != null)
            {
                HelperFunctions.Log("Actual data was loaded");
                CurrentAuthedPlayer.SetCurrentUser(user);
            }
            else
            {
                HelperFunctions.Log("Default user obj was returned");
                CurrentAuthedPlayer.SetCurrentUser(null);
                HelperFunctions.Error("A CurrentUser was not created via file or on Login. There's an issue here");
            }
        }
    }


    static void FirstTimeLoginDataTransfer()
    {
        //HelperFunctions.Error("Currently Using TestWords on 541 and TestKana on 547");
        string key = "";

        //Debug.LogError("Using TestWords right now");
        if(StaticUserSettings.IsKanjiStudyTopic())
        {
            key = TitleDataKeys.StarterWords.ToString();
        }
        else
        {
            key = TitleDataKeys.StarterKana.ToString();
        }

        GetTitleDataRequest rq = new GetTitleDataRequest
        {
            Keys = new List<string> { TitleDataKeys.StarterKana.ToString(), TitleDataKeys.StarterWords.ToString() }
        };

        Playfab.GetTitleData(rq, 
            (result)=>
            {
                OnSuccessfulFirstTimeTitleData(result, key); 
            }, 
            OnPlayFabError);
    }

    static void OnSuccessfulFirstTimeTitleData(GetTitleDataResult result, string key)
    {
        
        Playfab.UpdateUserData(new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string> 
            {
                {UserDataKey.SessionWords.ToString(), result.Data[TitleDataKeys.StarterWords.ToString()]},
                {UserDataKey.SessionKana.ToString(), result.Data[TitleDataKeys.StarterKana.ToString()]},
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
        Utilities.Logging.PlayFabErrorLog errorLog = new Utilities.Logging.PlayFabErrorLog(error);
        Logger.LogPlayFabError(errorLog);
        HelperFunctions.Log(errorLog);
        //RetrySystem.Retry(error, error.)
    }

    static void OnHTTPError(string error)
    {
        HelperFunctions.Error(error);
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




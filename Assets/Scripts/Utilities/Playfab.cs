using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using PlayFab.ProfilesModels;
using PlayFab.GroupsModels;

namespace Utilities.PlayFab
{
    public class Playfab
    {
        #region PlayFab Custom Event Name Enums
        public enum CustomEventNames
        {
            
        }
        #endregion
        private static Playfab _instance;

        #region Properties
        public static Playfab Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Playfab();
                }
                return _instance;
            }
        }

        public static bool ArePlayStreamEventsGenerated
        {
            get;
            private set;
        }

        public static string TitleID
        {
            get;
            private set;
        }

        public static string PlayFabID
        {
            get;
            private set;
        }

        public static string TitlePlayerID
        {
            get;
            private set;
        }

        public static string EntityToken
        {
            get;
            private set;
        }

        public static UniversalEntityKey GroupEntityKey
        {
            get;
            private set;
        }

        public static string GroupName
        {
            get;
            private set;
        }

        public static string SessionTicket
        {
            get;
            private set;
        }

        public static bool WasUserJustCreated
        {
            get;
            private set;
        }
        #endregion

        public Playfab()
        {
            _instance = this;
            PlayFabSettings.TitleId = TitleID = "79422";
            PlayFabSettings.DisableFocusTimeCollection = true;
            ArePlayStreamEventsGenerated = true;
        }

        public static void WritePlayStreamEvent(WriteClientPlayerEventRequest eventData)
        {
            
        }

        public static void Login(Action<LoginResult> success, Action<PlayFabError> failure, bool useRandom)
        {
            PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
            {
                TitleId = PlayFabSettings.TitleId,
                CustomId = SystemInfo.deviceUniqueIdentifier + Guid.NewGuid().ToString(),
                CreateAccount = true
            },
            (result) =>
            {
                PlayFabID = result.PlayFabId;
                TitlePlayerID = result.EntityToken.Entity.Id;
                SessionTicket = result.SessionTicket;
                EntityToken = result.EntityToken.EntityToken;
                HelperFunctions.Log("Is this a new account: " + result.NewlyCreated);
                if (result.NewlyCreated)
                {
                    WasUserJustCreated = true;
                }
                else
                {
                    WasUserJustCreated = false;
                }
                success(result);

            },
            (error) =>
            {
                HandlePlayFabError(error);
                failure(error);
            });
        }

        public static void Login(Action<LoginResult> success, Action<PlayFabError> failure)
        {
            PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
            {
                TitleId = PlayFabSettings.TitleId,
                CustomId = /*"devtest",*/SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true
            },
            (result) =>
            {
                PlayFabID = result.PlayFabId;
                TitlePlayerID = result.EntityToken.Entity.Id;
                SessionTicket = result.SessionTicket;
                EntityToken = result.EntityToken.EntityToken;
                if(result.NewlyCreated)
                {
                    WasUserJustCreated = true;
                }
                else
                {
                    WasUserJustCreated = false;
                }
                success(result);

            },
            (error) =>
            {
                HandlePlayFabError(error);
                failure(error);
            });
        }
        
        public static void ExecuteFunction(ExecuteFunctionRequest rq, Action<ExecuteFunctionResult> success, Action<PlayFabError> failure)
        {
            PlayFabCloudScriptAPI.ExecuteFunction(rq, success, failure);
        }
        public static void GetUserData(GetUserDataRequest rq, Action<GetUserDataResult> success, Action<PlayFabError> failure)
        {
            PlayFabClientAPI.GetUserData(rq, success, failure);
        }

        public static void GetTitleData(GetTitleDataRequest rq, Action<GetTitleDataResult> success, Action<PlayFabError> failure)
        {
            PlayFabClientAPI.GetTitleData(rq, success, failure);
        }

        public static void UpdateUserData(UpdateUserDataRequest rq, Action<UpdateUserDataResult> success, Action<PlayFabError> failure)
        {
            PlayFabClientAPI.UpdateUserData(rq, success, failure);
        }

        public static void GetTags(GetPlayerTagsRequest rq, Action<GetPlayerTagsResult> success, Action<PlayFabError> error)
        {
            PlayFabClientAPI.GetPlayerTags(rq, success, error); 
        }

        public static void GetLeaderboard(bool isAroundUser, StatisticName name, Action<List<PlayerLeaderboardEntry>> success, Action<PlayFabError> error, int startPos = 1)
        {
            PlayerProfileViewConstraints profileView = new PlayerProfileViewConstraints
            {
                ShowAvatarUrl = true,
                ShowDisplayName = true,
                ShowStatistics = true,
                ShowTags = true
            };

            if (isAroundUser)
            {
                GetLeaderboardAroundPlayerRequest rq = new GetLeaderboardAroundPlayerRequest
                {
                    PlayFabId = PlayFabID,
                    StatisticName = name.ToString(),
                    ProfileConstraints = profileView
                };

                PlayFabClientAPI.GetLeaderboardAroundPlayer(rq,
                    (suc) =>
                    {
                        success(suc.Leaderboard);
                    }, HandlePlayFabError);
            }
            else
            {
                GetLeaderboardRequest rq = new GetLeaderboardRequest
                {
                    StartPosition = startPos,
                    StatisticName = name.ToString(),
                    ProfileConstraints = profileView
                };

                PlayFabClientAPI.GetLeaderboard(rq,
                    (suc) =>
                    {
                        success(suc.Leaderboard);
                    }, HandlePlayFabError);
            }
        }

        public static void GetTitlePlayerIDs(List<string> playfabIDs, Action<GetTitlePlayersFromMasterPlayerAccountIdsResponse> success, Action<PlayFabError> error)
        {
            GetTitlePlayersFromMasterPlayerAccountIdsRequest rq =
                new GetTitlePlayersFromMasterPlayerAccountIdsRequest
                {
                    MasterPlayerAccountIds = playfabIDs,
                    TitleId = TitleID
                };
            PlayFabProfilesAPI.GetTitlePlayersFromMasterPlayerAccountIds(rq, success, error);
        }

        public static void CreateGroup(CreateGroupRequest rq, Action<CreateGroupResponse> success, Action<PlayFabError> error)
        {
            PlayFabGroupsAPI.CreateGroup(rq, 
                (result) => 
                {
                    UniversalEntityKey grpEntityKey = new UniversalEntityKey
                    {
                        id = result.Group.Id,
                        type = result.Group.Type
                    };

                    GroupName = result.GroupName;

                    success(result);
                }, error);
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
        private static void HandlePlayFabError(PlayFabError error)
        {
            string fullErrorDetails = "Error in PlayFab API: " + error.ApiEndpoint + "\n" +
                "Error: " + error.Error.ToString() + "\n" + "Error Message: " + error.ErrorMessage
                + "\n" + "Error Details: " + error.ErrorDetails.ToString();
            HelperFunctions.Log(fullErrorDetails);
        }
        public static string DisplayPlayFabError(PlayFabError error)
        {
            string fullErrorDetails = "Error in PlayFab API: " + error?.GenerateErrorReport() + "\n" +
                "Error: " + error?.Error.ToString() + "\n" + "Error Message: " + error?.ErrorMessage
                + "\n" + "Error Details: " + error?.ErrorDetails.ToString();
            return fullErrorDetails;
        }
    }
}

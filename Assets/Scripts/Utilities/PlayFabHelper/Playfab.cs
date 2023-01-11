using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using PlayFab.ProfilesModels;
using PlayFab.GroupsModels;
using Newtonsoft.Json;
using Unity.VisualScripting;
using Utilities.PlayFabHelper.Caching;
using Utilities.PlayFabHelper.CurrentUser;
using PlayFab.DataModels;
using PlayFab.Internal;
using Utilities.SaveOperations;
using System.Text;

namespace Utilities.PlayFabHelper
{
    public class Playfab
    {
        private static Playfab _instance;

        #region Properties

        public static bool ArePlayStreamEventsGenerated
        {
            get { return true; }
            private set { }
        }

        public static string TitleID
        {
            get;
            private set;
        }

        public static string DisplayName
        {
            get;
            set;
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

        private static DateTime TokenExpirationTime
        {
            get;
            set;
        }

        public static List<string> ActiveFileUploads
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

        public static void WritePlayStreamEvent(WriteClientPlayerEventRequest eventData)
        {
            
        }

        public static void Login(Action<LoginResult> success, Action<PlayFabError> failure, bool useRandom)
        {
            if(TokenExpirationTime == null || DateTime.UtcNow > TokenExpirationTime)
            {
                string customID = SystemInfo.deviceUniqueIdentifier;
                if (useRandom)
                {
                    customID += Guid.NewGuid().ToString();
                }

                PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
                {
                    TitleId = PlayFabSettings.TitleId,
                    CustomId = customID,
                    CreateAccount = true,
                    InfoRequestParameters = GetInfoRequest()

                },
                (result) =>
                {
                    SetAuthenticatedUserDefaults(result);
                    success(result);

                },
                (error) =>
                {
                    HandlePlayFabError(error);
                    failure(error);
                });
            }
            
        }

        public static void Login(Action<LoginResult> success, Action<PlayFabError> failure)
        {
            Login(success, failure, false);
        }
        
        public static void ExecuteFunction(ExecuteFunctionRequest rq, Action<ExecuteFunctionResult> success, Action<PlayFabError> failure)
        {
            //var cacheResult = CacheSystem.GetResponse(rq);
            //if (cacheResult == null)
            //{
                PlayFabCloudScriptAPI.ExecuteFunction(rq, 
                    (suc)=>
                    {
                        CacheSystem.Add(rq, suc);
                        success(suc);
                    }, failure);
            //}
            /*else
            {
                ExecuteFunctionResult cached = (ExecuteFunctionResult)cacheResult;
                success(cached);
            }*/
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

                //var cacheResult = CacheSystem.GetResponse(rq);
                //if (cacheResult == null)
                //{
                    PlayFabClientAPI.GetLeaderboardAroundPlayer(rq,
                    (suc) =>
                    {
                        //CacheSystem.Add(rq, suc);
                        success(suc.Leaderboard);
                    }, HandlePlayFabError);
                //}
                /*else
                {
                    GetLeaderboardAroundPlayerResult cached = (GetLeaderboardAroundPlayerResult)cacheResult;
                    success(cached.Leaderboard);
                }*/
                
            }
            else
            {
                GetLeaderboardRequest rq = new GetLeaderboardRequest
                {
                    StartPosition = startPos,
                    StatisticName = name.ToString(),
                    ProfileConstraints = profileView
                };
               // var cacheResult = CacheSystem.GetResponse(rq);
                //if (cacheResult == null)
                //{
                    PlayFabClientAPI.GetLeaderboard(rq,
                    (suc) =>
                    {

                        success(suc.Leaderboard);
                    }, HandlePlayFabError);
               // }
               /* else
                {
                    GetLeaderboardResult cached = (GetLeaderboardResult)cacheResult;
                    success(cached.Leaderboard);
                }*/
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
                        ID = result.Group.Id,
                        Type = result.Group.Type
                    };

                    GroupName = result.GroupName;
                    GroupEntityKey = (UniversalEntityKey)result.Group;

                    success(result);
                }, error);
        }
        public static void AddMembers(AddMembersRequest rq, Action<PlayFabError> error)
        {
            PlayFabGroupsAPI.AddMembers(rq, (result) => {  }, error);
        }
        
        public static void ListGroupMembers(ListGroupMembersRequest r, Action<ListGroupMembersResponse> success, Action<PlayFabError> error)
        {
            PlayFabGroupsAPI.ListGroupMembers(r, success, error);
        }
        
        public static void GetProfiles(GetEntityProfilesRequest r, Action<GetEntityProfilesResponse> suceess, Action<PlayFabError> error)
        {
            PlayFabProfilesAPI.GetProfiles(r, suceess, error);
        }
        
        public static void GetGroup(Action<GetGroupResponse> success, Action<PlayFabError> error, bool hasOptinalArgs, string grpName = "", UniversalEntityKey grpKey = null)
        {
            GetGroupRequest rq;

            if (hasOptinalArgs)
            {
                rq = new GetGroupRequest
                { 
                    GroupName = grpName,
                    Group = grpKey
                };
            }
            else
            {
                rq = new GetGroupRequest { 
                    GroupName = String.Empty
                };
            }

            PlayFabGroupsAPI.GetGroup(rq, success, error);
        }
        
        public static void GetFiles(GetFilesRequest rq, Action<PlayFabError> error, Action<List<PlayFabFileInfo>> success = null)
        {
            PlayFabDataAPI.GetFiles(rq, 
                (result) => 
                { 
                    List<PlayFabFileInfo> files = new List<PlayFabFileInfo>();  
                    foreach(var pair in result.Metadata)
                    {
                        PlayFabFileInfo f = new PlayFabFileInfo
                        (
                            pair.Value.FileName,
                            pair.Value.DownloadUrl,
                            String.Empty,
                            pair.Value.Size,
                            pair.Value.LastModified
                        );

                        files.Add(f);
                    }
                    success(files);
                }, error);
        }

        public static void DownloadFileFromPlayFab(string dwnldURl, Action<byte[]> success, Action<string> error)
        {
            PlayFabHttp.SimpleGetCall(dwnldURl, success, error);
        }

        public static void InitiateFileUploads(InitiateFileUploadsRequest rq, Action<InitiateFileUploadsResponse> success, Action<PlayFabError> error)
        {
            ActiveFileUploads = rq.FileNames;
            PlayFabDataAPI.InitiateFileUploads(rq, success, error);
        }

        public static void UploadEntityFile(InitiateFileUploadsResponse initialFileInfo, DataCategory c, Action<string> error)
        {
            byte[] fileInBytes = SaveSystem.PrepareFileForUpload(c);
            if(fileInBytes != null)
            {
                PlayFabHttp.SimplePutCall(initialFileInfo.UploadDetails[0].UploadUrl,
                fileInBytes, FinalizeUpload, error);
            }
             
        }
        public static void FinalizeUpload(byte[] somethingIGuess)
        {
            HelperFunctions.Log("Apparantly this array is null ??");
            HelperFunctions.Log(somethingIGuess == null ? "They sent me a null ??" : "Its not null I suppose" 
                + Encoding.UTF8.GetString(somethingIGuess));

            var rq = new FinalizeFileUploadsRequest
            {
                Entity = CurrentAuthedPlayer.CurrentUser.EntityKey,
                FileNames = ActiveFileUploads
            };

            PlayFabDataAPI.FinalizeFileUploads(rq, (result) =>
            {
                HelperFunctions.Log("Successfully Uploaded: ");
                HelperFunctions.LogDictContent(result.Metadata);
            }, LogPlayFabError);
        }
        public static void GetCatalogItems(Action<List<PlayFabItem>> success, Action<PlayFabError> error)
        {
            GetCatalogItemsRequest rq = new GetCatalogItemsRequest();
            PlayFabClientAPI.GetCatalogItems(rq, (result) =>
            {
                List<PlayFabItem> items = new List<PlayFabItem>();
                
                foreach(var i in result.Catalog)
                {
                    items.Add((PlayFabItem)i);
                }
                success(items);
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
        private static GetPlayerCombinedInfoRequestParams GetInfoRequest()
        {
            GetPlayerCombinedInfoRequestParams infoRequest = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true,
                GetUserAccountInfo = true,
                GetUserInventory = true,
                GetUserVirtualCurrency = true,
                ProfileConstraints = new PlayerProfileViewConstraints
                {
                    ShowAvatarUrl = true,
                    ShowDisplayName = true,
                    ShowTags = true,
                    ShowStatistics = true
                }
            };

            return infoRequest;
        }

        private static void SetAuthenticatedUserDefaults(LoginResult result)
        {
            PlayFabID = result.PlayFabId;
            TitlePlayerID = result.EntityToken.Entity.Id;
            SessionTicket = result.SessionTicket;
            EntityToken = result.EntityToken.EntityToken;
            TokenExpirationTime = (DateTime)result.EntityToken.TokenExpiration;
            HelperFunctions.Log("Is this a new account: " + result.NewlyCreated);
            if (result.NewlyCreated)
            {
                WasUserJustCreated = true;
            }
            else
            {
                WasUserJustCreated = false;
            }
            
            BasicProfile b = new BasicProfile(result.InfoResultPayload.PlayerProfile);
            PlayFabInventory pfInventory = new PlayFabInventory(result.InfoResultPayload.UserInventory, PlayFabID, result.InfoResultPayload.UserVirtualCurrency);
            PlayFabUser user = new PlayFabUser(PlayFabID, result.InfoResultPayload.PlayerProfile.Tags,
                (UniversalEntityKey)result.EntityToken.Entity, b, pfInventory);

            CurrentAuthedPlayer.SetCurrentUser(user);
        }
        
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

        public static void LogPlayFabError(PlayFabError error)
        {
            string fullErrorDetails = "Error in PlayFab API: " + error?.GenerateErrorReport() + "\n" +
                "Error: " + error?.Error.ToString() + "\n" + "Error Message: " + error?.ErrorMessage
                + "\n" + "Error Details: " + error?.ErrorDetails.ToString();
            HelperFunctions.Error(fullErrorDetails);
        }
    }

    
}

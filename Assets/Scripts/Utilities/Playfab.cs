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

namespace Utilities.PlayFabHelper
{
    public enum EntityTypes
    {
        title_player_account,
        group,
        character,
        master_player_account,
        title,
    }

    public class UniversalEntityKey
    {
        public string ID
        {
            get;
            set;
        }
        public string Type
        {
            get;
            set;
        }

        public UniversalEntityKey()
        {

        }

        public UniversalEntityKey(string iD, string type)
        {
            ID = iD;
            Type = type;
        }

        public static implicit operator PlayFab.AuthenticationModels.EntityKey(UniversalEntityKey k)
        {
            return new PlayFab.AuthenticationModels.EntityKey
            {
                Id = k.ID,
                Type = k.Type
            };
        }
        public static implicit operator PlayFab.ClientModels.EntityKey(UniversalEntityKey k)
        {
            return new PlayFab.ClientModels.EntityKey
            {
                Id = k.ID,
                Type = k.Type
            };
        }
        public static implicit operator PlayFab.CloudScriptModels.EntityKey(UniversalEntityKey k)
        {
            return new PlayFab.CloudScriptModels.EntityKey
            {
                Id = k.ID,
                Type = k.Type
            };
        }
        public static implicit operator PlayFab.DataModels.EntityKey(UniversalEntityKey k)
        {
            return new PlayFab.DataModels.EntityKey
            {
                Id = k.ID,
                Type = k.Type
            };
        }
        public static implicit operator PlayFab.GroupsModels.EntityKey(UniversalEntityKey k)
        {
            return new PlayFab.GroupsModels.EntityKey
            {
                Id = k.ID,
                Type = k.Type
            };
        }
        public static implicit operator PlayFab.EconomyModels.EntityKey(UniversalEntityKey k)
        {
            return new PlayFab.EconomyModels.EntityKey
            {
                Id = k.ID,
                Type = k.Type
            };
        }
        public static implicit operator PlayFab.EventsModels.EntityKey(UniversalEntityKey k)
        {
            return new PlayFab.EventsModels.EntityKey
            {
                Id = k.ID,
                Type = k.Type
            };
        }
        public static implicit operator PlayFab.ExperimentationModels.EntityKey(UniversalEntityKey k)
        {
            return new PlayFab.ExperimentationModels.EntityKey
            {
                Id = k.ID,
                Type = k.Type
            };
        }
        public static implicit operator PlayFab.MultiplayerModels.EntityKey(UniversalEntityKey k)
        {
            return new PlayFab.MultiplayerModels.EntityKey
            {
                Id = k.ID,
                Type = k.Type
            };
        }
        public static implicit operator PlayFab.ProfilesModels.EntityKey(UniversalEntityKey k)
        {
            return new PlayFab.ProfilesModels.EntityKey
            {
                Id = k.ID,
                Type = k.Type
            };
        }
        
        public static explicit operator UniversalEntityKey(PlayFab.AuthenticationModels.EntityKey e)
        {
            return new UniversalEntityKey
            {
                ID = e.Id,
                Type = e.Type
            };
        }
        public static explicit operator UniversalEntityKey(PlayFab.ClientModels.EntityKey e)
        {
            return new UniversalEntityKey
            {
                ID = e.Id,
                Type = e.Type
            };
        }
        public static explicit operator UniversalEntityKey(PlayFab.CloudScriptModels.EntityKey e)
        {
            return new UniversalEntityKey
            {
                ID = e.Id,
                Type = e.Type
            };
        }
        public static explicit operator UniversalEntityKey(PlayFab.DataModels.EntityKey e)
        {
            return new UniversalEntityKey
            {
                ID = e.Id,
                Type = e.Type
            };
        }
        public static explicit operator UniversalEntityKey(PlayFab.GroupsModels.EntityKey e)
        {
            return new UniversalEntityKey
            {
                ID = e.Id,
                Type = e.Type
            };
        }
        public static explicit operator UniversalEntityKey(PlayFab.EconomyModels.EntityKey e)
        {
            return new UniversalEntityKey
            {
                ID = e.Id,
                Type = e.Type
            };
        }
        public static explicit operator UniversalEntityKey(PlayFab.EventsModels.EntityKey e)
        {
            return new UniversalEntityKey
            {
                ID = e.Id,
                Type = e.Type
            };
        }
        public static explicit operator UniversalEntityKey(PlayFab.ExperimentationModels.EntityKey e)
        {
            return new UniversalEntityKey
            {
                ID = e.Id,
                Type = e.Type
            };
        }
        public static explicit operator UniversalEntityKey(PlayFab.MultiplayerModels.EntityKey e)
        {
            return new UniversalEntityKey
            {
                ID = e.Id,
                Type = e.Type
            };
        }
        public static explicit operator UniversalEntityKey(PlayFab.ProfilesModels.EntityKey e)
        {
            return new UniversalEntityKey
            {
                ID = e.Id,
                Type = e.Type
            };
        }


    }
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
            get { return true; }
            private set { }
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
            PlayFabGroupsAPI.AddMembers(rq, (result) => { }, error);
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

    public class CloudScriptStatArgument
    {
        [JsonProperty("statName")]
        public string statName { get; set; }

        [JsonProperty("value")]
        public string value { get; set; }

        public CloudScriptStatArgument(StatisticName n, int val)
        {
            statName = n.ToString();
            value = EncodeStatisticValue(val);
        }

        [JsonConstructor]
        public CloudScriptStatArgument(string n, int val)
        {
            statName = n;
            value = val.ToString();
        }

        private string EncodeStatisticValue(int value)
        {

            HelperFunctions.Log("Convert to Byte Array: ");
            byte[] intBytes = BitConverter.GetBytes(value);
            string byteString = "";
            foreach (byte b in intBytes)
            {
                Console.Write(b.ToString() + ", ");
                byteString += b.ToString() + ",";
            }

            return byteString;
        }
        private static int DecodeStringValue(string value)
        {
            List<byte> bytes = new List<byte>();

            foreach (string b in value.Split(','))
            {
                if (!String.IsNullOrEmpty(b))
                {
                    bytes.Add(Convert.ToByte(b));
                }
            }

            //Console.WriteLine(");
            foreach (byte b in bytes)
            {
                Console.Write(b.ToString() + ", ");

            }

            byte[] properArray = bytes.ToArray();


            int i = BitConverter.ToInt32(properArray, 0);

            return i;

        }

        public override string ToString()
        {
            return $"Stat Name: {statName} \n" + $"Stat Value: {value}";
        }
    }
}

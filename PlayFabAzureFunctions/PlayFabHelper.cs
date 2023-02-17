using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using PlayFab;
using PlayFab.Samples;
using PlayFabCloudScript;
using PlayFab.ServerModels;
using PlayFab.GroupsModels;
using PlayFab.DataModels;
using PlayFab.AuthenticationModels;
using PlayFabCloudScript.OnLogin;
using Newtonsoft.Json;
using Utilities;
using Newtonsoft.Json.Converters;

public enum UserDataKey
{
    LeitnerLevels,
    PrestigeLevels,
    SessionWords,
    LoginCount,
    NextSession,
    WordsSeen,
    TotalSP,
    WeeklySP
}

public enum TitleDataKeys
{
    StarterWords,
    CommonWords,
    Kana
}

public enum StatisticName
{
    LeagueSP,
    MonthlySP,
    WordsSeen,
    WordsMastered,
    StudyStreak,
    TotalSP
}

public enum EntityTypes
{
    title_player_account,
    group,
    character,
    master_player_account,
    title
}

public class CloudScriptStatArgument
{
    [JsonProperty("statName")]
    public string statName { get; set; }

    [JsonProperty("value")]
    public string value { get; set; }
}

public class BasicProfile
{
    [JsonProperty("avatarURL")]
    public string AvatarURL { get; private set; }
    
    [JsonProperty("playfabID")]
    public string PlayFabID { get; private set; }
    
    [JsonProperty("statistics")]
    public Dictionary<StatisticName, CloudScriptStatArgument> Statistics { get; private set; }

    [JsonProperty("displayName")]
    public string DisplayName { get; private set; }

    [JsonConstructor]
    public BasicProfile(string avatarURL, string playfabID, string displayName, List<StatisticModel> stats)
    {
        AvatarURL = avatarURL;
        PlayFabID = playfabID;
        DisplayName = displayName;
        if(stats.Count > 0)
        {
            Statistics = new Dictionary<StatisticName, CloudScriptStatArgument>();
            Dictionary<StatisticName, int> statVersion = new Dictionary<StatisticName, int>();
            try
            {
                foreach(StatisticModel s in stats)
                {
                    StatisticName sn = HelperFunctions.ParseEnum<StatisticName>(s.Name);
                    if(statVersion.ContainsKey(sn) && Statistics.ContainsKey(sn))
                    {
                        if(s.Version > statVersion[sn])
                        {
                            statVersion[sn] = s.Version;
                            Statistics[sn].value = s.Value.ToString();

                        }
                    }
                    else
                    {
                        statVersion.Add(sn, s.Version);
                        Statistics.Add(sn, new CloudScriptStatArgument
                        {
                            statName = s.Name,
                            value = s.Value.ToString()
                        });
                    }
                }
            }
            catch(Exception e)
            {
                PlayFabHelper.CaptureException(e, GetModifiedProfileData.logger);
            }
            
        }
        
    }
}

public class UniversalEntityKey
    {
        [JsonProperty("ID")]
        public string ID
        {
            get;
            set;
        }

        [JsonProperty("Type")]
        public string Type
        {
            get;
            set;
        }
        
        public UniversalEntityKey(){}

        public UniversalEntityKey(string Id, EntityTypes t)
        {
            ID = Id;
            Type = t.ToString();
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

        public override string ToString()
        {
            return $"ID: {ID} \n Type: {Type} ";
        }

    }

public class PlayFabFileInfo
{
    [JsonProperty("FileName")]
    public readonly string FileName;
    
    [JsonProperty("DownloadUrl")]
    public readonly string DownloadUrl;
    
    [JsonProperty("UploadUrl")]
    public readonly string UploadUrl;
    
    [JsonProperty("Size")]
    public readonly int Size;
    
    [JsonProperty("LastModified")]
    public readonly DateTime LastModified;

    [JsonConstructor]
    public PlayFabFileInfo(string fileName, string dwnUrl, string upldUrl, int s, DateTime lastMod)
    {
        FileName = fileName;
        DownloadUrl = dwnUrl;
        UploadUrl = upldUrl;
        Size = s;
        LastModified = lastMod;
    }

    public override string ToString()
    {
        return $"File Name: {FileName} \n DownloadUrl: {DownloadUrl} \n " +
            $"UploadUrl: {UploadUrl} \n Size: {Size} \n  Last Modified: {LastModified}";
    }
}

public class CompletePurchaseArg
{
    [JsonProperty("itemIds")]
    public List<string> ItemIDs { get; set; }

    [JsonProperty("price")]
    public string Price { get; set; }

    [JsonProperty("vc")]
    public string VC { get; set; }

    public CompletePurchaseArg()
    {

    }
}

public class ModifyTagParameter
{
    [JsonProperty("Operation")]
    public string Operation { get; set; }
    
    [JsonProperty("TagName")]
    public List<string> TagNames { get; set; }
}

public class AddWordArgument
{
    [JsonProperty("NumToAdd")]
    public int NumToAdd { get; set; }

    [JsonProperty("Progess")]
    public int Progress { get; set; }

    [JsonProperty("IsKanjiStudyTopic")]
    public bool IsKanjiStudyTopic { get; set; }
}

public class GetOtherPlayerStatisticArgument
{
    [JsonProperty("OtherPlayers")]
    public List<string> playFabIDs { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty("StatisticName")]
    public StatisticName StatisticName { get; set; }
}

public class GetOtherPlayerStatisticsResult
{
    [JsonProperty("Rank")]
    public int Rank { get; set; }

    [JsonProperty("ID")]
    public string ID { get; set; }

    [JsonProperty("Value")]
    public int Value { get; set; }
}

public static class PlayFabHelper
{
    #region PlayFab Custom Event Name Enums
    public enum CustomEventNames
    {
        
    }
    #endregion

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

    public static string EntityToken
    {
        get;
        private set;
    }

    public static string SessionTicket
    {
        get;
        private set;
    }


    public static void WritePlayStreamEvent(WriteServerPlayerEventRequest eventData)
    {
        
    }

    public static void DeleteGroup(PlayFab.GroupsModels.EntityKey e)
    {
        //await GetEntityToken();
        DeleteGroupRequest rq = new DeleteGroupRequest
        {
            Group = e
        };
        var deleteGpTask = PlayFabGroupsAPI.DeleteGroupAsync(rq);
    }

    public static Task<PlayFabResult<PlayFab.GroupsModels.ListMembershipResponse>> ListMembership(string id, ILogger l)
    {
        UniversalEntityKey key = new UniversalEntityKey{
            ID = id,
            Type = EntityTypes.master_player_account.ToString()
        };

        ListMembershipRequest rq = new ListMembershipRequest
        {
            Entity =key
        };
        var memTask = PlayFabGroupsAPI.ListMembershipAsync(rq);
        l.LogInformation($"List Membership API for ID: {id} ");
        return memTask;
    }

    public static Task<PlayFabResult<PlayFab.GroupsModels.EmptyResponse>> AddMembers(PlayFab.GroupsModels.EntityKey groupKey, List<PlayFab.GroupsModels.EntityKey> members, ILogger log)
    {
        AddMembersRequest rq = new AddMembersRequest
        {
            Group = groupKey,
            Members = members
        };

        var playfabHttpTask = PlayFabGroupsAPI.AddMembersAsync(rq);
        log.LogInformation("Add Group Request Made ");
        return playfabHttpTask;
    }

    public static Task<PlayFabResult<GetPlayerProfileResult>> GetPlayerProfile(string id, PlayerProfileViewConstraints constraint, ILogger log)
    {
        var playfabHttpTask = PlayFabServerAPI.GetPlayerProfileAsync( new GetPlayerProfileRequest {
            PlayFabId = id,
            ProfileConstraints = constraint
        });
        log.LogInformation("Specific request made: " + id + " " + constraint.ToString());
        return playfabHttpTask;
    }
    
    public static Task<PlayFabResult<GetUserDataResult>> GetUserData(string id, List<string> keys)
    {
        var playfabHttpTask = PlayFabServerAPI.GetUserDataAsync( new GetUserDataRequest 
        {
            PlayFabId = id,
            Keys = keys
        });

        return playfabHttpTask;
    }
    
    public static Task<PlayFabResult<GetTitleDataResult>> GetTitleData(List<string> keys, ILogger log)
    {
        log.LogInformation("Starting Request");
        var playfabHttpTask = PlayFabServerAPI.GetTitleDataAsync( new GetTitleDataRequest 
        {
            Keys = keys
        });
        log.LogInformation("Request sent");
        //playfabHttpTask.ContinueWith(ProcessPlayFabRequest);
        return playfabHttpTask;
    }

    public static Task<PlayFabResult<GetTitleDataResult>> GetTitleData(List<string> keys)
    {
        var playfabHttpTask = PlayFabServerAPI.GetTitleDataAsync( new GetTitleDataRequest 
        {
            Keys = keys
        });

        //playfabHttpTask.ContinueWith(ProcessPlayFabRequest);
        return playfabHttpTask;
    }

    public static Task<PlayFabResult<AddPlayerTagResult>> AddPlayerTag(string pfID, string tagName, ILogger log)
    {
        var addTagRq = new AddPlayerTagRequest 
        {
            PlayFabId = pfID,
            TagName = tagName
        };
        var playfabHttpTask = PlayFabServerAPI.AddPlayerTagAsync(addTagRq);
        log.LogInformation( "PlayFabID" + addTagRq.PlayFabId + "TagName" + addTagRq.TagName);
        //playfabHttpTask.ContinueWith(ProcessPlayFabRequest);
        return playfabHttpTask;
    }

    public static Task<PlayFabResult<GetEntityTokenResponse>> GetEntityToken(UniversalEntityKey key = null)
    {
        GetEntityTokenRequest rq = new GetEntityTokenRequest{};
        if(key != null)
        {
            rq.Entity = key;
        }
        
        return PlayFabAuthenticationAPI.GetEntityTokenAsync(rq);
    }

    public static Task<PlayFabResult<RemovePlayerTagResult>> RemovePlayerTag(string pfID, string tagName, ILogger log)
    {
        var removeTagRq = new RemovePlayerTagRequest 
        {
            PlayFabId = pfID,
            TagName = tagName
        };
        try
        {
            var playfabHttpTask = PlayFabServerAPI.RemovePlayerTagAsync(removeTagRq);
            log.LogInformation( "Remove Tag for PlayFabID " + removeTagRq.PlayFabId + " TagName " + removeTagRq.TagName);
            //playfabHttpTask.ContinueWith(ProcessPlayFabRequest);
            return playfabHttpTask;
        }
        catch(Exception e)
        {
            log.LogError(e.Message + "\n" + e.StackTrace + "\n Inner Exception: " + e.InnerException);
            return default(Task<PlayFabResult<RemovePlayerTagResult>>);
        }
        
    }

    public static Task<PlayFabResult<GetPlayerStatisticsResult>> GetPlayerStatisitic(string id, StatisticName s, ILogger log)
    {
        GetPlayerStatisticsRequest rq = new GetPlayerStatisticsRequest
        {
            PlayFabId = id,
            StatisticNames = new List<string> {s.ToString()}
        };

        var statRequest = PlayFabServerAPI.GetPlayerStatisticsAsync(rq);
        log.LogInformation($"Requested {s.ToString()} value for user: {id}");
        return statRequest;
    }
    
    /*public static Task<PlayFabResult<GetPlayersInSegmentResult>> GetPlayersInSegment()
    {
        var segmentRequest = new GetPlayersInSegmentRequest
        {
            SegmentId
        }
    }*/

    public static Task<PlayFabResult<GetCatalogItemsResult>> GetCatalogItem(ILogger log = null, List<string> logList = null)
    {
        GetCatalogItemsRequest rq = new GetCatalogItemsRequest();
        var catalogRgTask = PlayFabServerAPI.GetCatalogItemsAsync(rq);
        return catalogRgTask;
    }

    public static Task<PlayFabResult<GrantItemsToUserResult>> GrantItemsToUser(string id, List<string> itemIds, ILogger log = null, List<string> logList = null)
    {
        GrantItemsToUserRequest rq = new GrantItemsToUserRequest
        {
            PlayFabId = id,
            ItemIds = itemIds
        };

        if(log != null && logList != null)
        {
            LogInfo($"Request Parameters: ID - {id} \n Item IDs: {Utilities.HelperFunctions.PrintListContent(itemIds)}", log, logList);
        }

        var grantTask = PlayFabServerAPI.GrantItemsToUserAsync(rq);
        return grantTask;
    }

    public static Task<PlayFabResult<ModifyUserVirtualCurrencyResult>> SubTractUserVC(int amount, string id, string vc, ILogger log = null, List<string> logList = null)
    {
        SubtractUserVirtualCurrencyRequest rq = new SubtractUserVirtualCurrencyRequest
        {
            PlayFabId = id,
            Amount = amount,
            VirtualCurrency = vc
        };

        if(log != null && logList != null)
        {
            LogInfo($"Request Parameters: ID - {id} \n Amount to Subtract: {amount} \n VC: {vc}", log, logList);
        }

        var subtractTask = PlayFabServerAPI.SubtractUserVirtualCurrencyAsync(rq);
        return subtractTask;
    }

    public static Task<PlayFabResult<GetFilesResponse>> GetEntityFiles(UniversalEntityKey e, ILogger log = null, List<string> logList = null)
    {
        GetFilesRequest rq = new GetFilesRequest
        {
            Entity = e
        };
        var getFilesRequest = PlayFabDataAPI.GetFilesAsync(rq);
        if(log != null && logList != null)
        {
            LogInfo("Request Entity: " + e.ToString(), log, logList);
        }

        return getFilesRequest;
    }

    public static Task<PlayFabResult<UpdatePlayerStatisticsResult>> UpdateUserStatistic(string id, List<CloudScriptStatArgument> updates, ILogger log)
    {
        List<StatisticUpdate> statisticUpdates = new List<StatisticUpdate>();
        foreach(CloudScriptStatArgument st in updates)
        {
            int v = DecodeStringValue(st.value);
            statisticUpdates.Add( new StatisticUpdate 
            {
                StatisticName = st.statName,
                Value = v
            });
        }

        var updateStatRq = new UpdatePlayerStatisticsRequest
        {
            PlayFabId = id,
            Statistics = statisticUpdates
        };

        log.LogInformation("ID: " + id + " " + "Statistic Values " + HelperFunctions.PrintListContent(updates));
        var statisticUpdateTask = PlayFabServerAPI.UpdatePlayerStatisticsAsync(updateStatRq);
        return statisticUpdateTask;
    }

    public static Task<PlayFabResult<UpdateUserDataResult>> UpdateUserData(string id, Dictionary<string, string> dict)
    {
        AddNewWord.logger.LogInformation("Attempting to call GameServer/UpdateUserData on ID: " + id);
        var playfabHttpTask = PlayFabServerAPI.UpdateUserDataAsync( new UpdateUserDataRequest 
        {
            PlayFabId = id,
            Data = dict
        });

        return playfabHttpTask;
    }

    public static void ProcessPlayFabRequest<T>(PlayFabResult<T> playFabResult, Action<PlayFabResult<T>> callback, ILogger log) where T : PlayFab.Internal.PlayFabResultCommon
    {
        if(playFabResult.Error == null)
        {
            callback(playFabResult);
        }
        else
        {
            CapturePlayFabError(playFabResult.Error, log);
        }
    }

    public static bool WasPlayFabCallSuccessful<T>(PlayFabResult<T> playFabResult, ILogger log) where T : PlayFab.Internal.PlayFabResultCommon
    {
        if(playFabResult.Error == null)
        {
            log.LogInformation("API call was successful");
            return true;
        }
        else
        {
            log.LogInformation("An Error has occured");
            CapturePlayFabError(playFabResult.Error, log);
            return false;
        }
    }

    public static bool WasPlayFabCallSuccessful<T>(PlayFabResult<T> playFabResult, ILogger log, List<string> listofLogs) where T : PlayFab.Internal.PlayFabResultCommon
    {
        if(playFabResult.Error == null)
        {
            log.LogInformation("API call was successful");
            return true;
        }
        else
        {
            string val = "An Error has occured";
            log.LogInformation(val);
            listofLogs.Add(val);
            val = CapturePlayFabError(playFabResult.Error, log);
            listofLogs.Add(val);
            return false;
        }
    }

    private static int DecodeStringValue(string value)
    {
        List<byte> bytes = new List<byte>();

        foreach (string b in value.Split(','))
        {
            if(!String.IsNullOrEmpty(b))
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

    public static string CaptureException(Exception ex, ILogger log)
    {
        string err = ex.ToString();
        log.Log(LogLevel.Error, err);
        GetModifiedProfileData.errorStrings.Add(err);
        return err;
    }

    public static string CapturePlayFabError(PlayFabError error, ILogger log)
    {
        string errorDetails = "";
        try
        {
            errorDetails += "Error in PlayFab API: " + error.RequestId;
            errorDetails += "\n" + "Error: " + error.Error.ToString();
            errorDetails += "\n" + "Error Message: " + error.ErrorMessage;
            errorDetails += "\n" + "Error Details: " + error.ErrorDetails.ToString();
            log.Log(LogLevel.Error, errorDetails);
        }
        catch(Exception e)
        {
            CaptureException(e, log);
            log.LogInformation("Error information before exception: " + errorDetails);
        }
        
        return errorDetails;
    }

    public static void LogInfo(string message, ILogger log, List<string> listOfLogs)
    {
        log.LogInformation(message);
        listOfLogs.Add(DateTime.UtcNow + ": " + message);
    }
}
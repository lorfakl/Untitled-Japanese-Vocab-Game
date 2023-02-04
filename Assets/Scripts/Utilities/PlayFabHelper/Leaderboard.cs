using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Utilities.SaveOperations;

namespace Utilities.PlayFabHelper
{
    public struct LeaderboardEntry
    {
        public string displayName;
        public int rank;
        public string playfabID;
        public int score;
        public Image avatarImage;
        public Sprite avatarPhotoSprite;

        public void Print()
        {
            HelperFunctions.Log("DisplayName: " + this.displayName + "\n" +
                "Rank: " + this.rank + "\n" +
                "PlayFabID: " + this.playfabID + "\n" +
                "Score: " + this.score + "\n" +
                "avatarImage: " + this.avatarImage);
        }

        public void AssignSprite(Sprite s)
        { 
            avatarPhotoSprite = s;
        }
    }



    public class Leaderboard
    {
        Queue<LeaderboardEntry> entryQueue = new Queue<LeaderboardEntry>();
        List<LeaderboardEntry> _entries = new List<LeaderboardEntry>();
        Dictionary<string, LeaderboardEntry> _entryDict = new Dictionary<string, LeaderboardEntry>();
        Func<Leaderboard, Task> _leaderboardSetUpCallback;
        GameObject _leaderboardEntryPrefab;
        Transform _leaderboardPanelContent;
        bool _isLocal;

        public int EntryCount
        {
            get { return _entries.Count; }  
        }

        public List<LeaderboardEntry> Entries
        {
            get { return _entries; }   
        }

        public bool IsAroundPlayer
        {
            get { return _isLocal; }
        }

        public Queue<LeaderboardEntry> EntryQueue
        {
            get { return entryQueue; }
        }

        public LeaderboardEntry GetLeaderboardEntry(object caller)
        {
            if (caller.GetType() == typeof(LeaderboardEntryController))
            {
                return entryQueue.Dequeue();
            }
            else
            {
                return new LeaderboardEntry();
            }
        }

        public LeaderboardEntry this[string id]
        {
            get => _entryDict[id];
            set => _entryDict[id] = value;
        }

        public void AddEntry(LeaderboardEntry e)
        {
            if(!_entryDict.ContainsKey(e.playfabID))
            {
                _entryDict.Add(e.playfabID, e);
                _entries.Add(e);
            }
            OrderListByScoreDescending();
            
        }

        public LeaderboardEntry GetLeaderboardEntry(string ID)
        {
            LeaderboardEntry e;
            foreach(var entry in _entries)
            {
                if(entry.playfabID == ID)
                {
                    e = entry;
                    return e;
                }
            }

            return default;
        }
        public Leaderboard(StatisticName s, GameObject entryPrefab, Transform prefabParent, Func<Leaderboard, Task> callBack, bool isLocal = false)
        {
            _leaderboardEntryPrefab = entryPrefab;
            _leaderboardPanelContent = prefabParent;
            _isLocal = isLocal;
            _leaderboardSetUpCallback = callBack;
            PlayFabController.GetLeadboard(isLocal, s, GenerateLeaderboardView);
        }

        public Leaderboard()
        { }

        async void GenerateLeaderboardView(List<PlayerLeaderboardEntry> pfLb)
        {

            foreach (PlayerLeaderboardEntry entry in pfLb)
            {
                LeaderboardEntry e;
                if (entry.PlayFabId == Playfab.PlayFabID)
                {
                    e = new LeaderboardEntry
                    {
                        displayName = "YOU",
                        playfabID = entry.PlayFabId,
                        score = ScoreEventProcessors.Score,
                        rank = (pfLb.IndexOf(entry) + 1)
                    };
                    PlayFabController.GetAvatarImage();
                    

                }
                else
                {
                    e = new LeaderboardEntry
                    {
                        displayName = entry.DisplayName,
                        playfabID = entry.PlayFabId,
                        rank = (pfLb.IndexOf(entry) + 1),
                        score = entry.StatValue
                    };
                }
                try
                {
                    e.avatarPhotoSprite = await SaveSystem.ConvertBytesToSprite(System.IO.File.ReadAllBytes(Application.persistentDataPath + "/" + e.playfabID + ".png"));
                }
                catch(Exception ex)
                {
                    HelperFunctions.CatchException(ex);
                }
                entryQueue.Enqueue(e);
                AddEntry(e);
                
            }
            Task callBackTask = _leaderboardSetUpCallback(this);


            foreach (PlayerLeaderboardEntry entry in pfLb)
            {
                LeaderboardEntryController c = GameObject.Instantiate(_leaderboardEntryPrefab, _leaderboardPanelContent).GetComponent<LeaderboardEntryController>();
                c.SetLeaderboardHost(this);
            }
        }

        public LeaderboardEntry[] GetOrderArray()
        {
            List<LeaderboardEntry> ordered = _entries.OrderByDescending(entry => entry.score).ToList();
            LeaderboardEntry[] orderedArray = ordered.ToArray();
            for (int i = 0; i < orderedArray.Length; i++)
            {
                orderedArray[i].rank = i + 1;
            }
            _entries = orderedArray.ToList();
            return orderedArray;
        }

        private void OrderListByScoreDescending()
        {
            List<LeaderboardEntry> ordered = _entries.OrderByDescending(entry => entry.score).ToList();
            LeaderboardEntry[] orderedArray = ordered.ToArray();
            for (int i = 0; i < orderedArray.Length; i++)
            {
                orderedArray[i].rank = i + 1;
            }
        }

        async Task GetAvatarPhotos(LeaderboardEntry e)
        {

        }
    }
}

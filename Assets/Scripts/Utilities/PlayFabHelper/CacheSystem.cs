using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using PlayFab.SharedModels;
using System;

namespace Utilities.PlayFabHelper.Caching
{
    public static class CacheSystem
    {
        static Dictionary<CachedPlayFabRequest, CachedPlayFabResponse> cacheData = new Dictionary<CachedPlayFabRequest, CachedPlayFabResponse>();

        static TimeSpan lifeSpan = new TimeSpan(0, 15, 0);

        private static bool CheckIfCached(CachedPlayFabRequest rq)
        {
            if (cacheData.ContainsKey(rq))
            {
                CachedPlayFabResponse res = cacheData[rq];
                TimeSpan timeInCache = DateTime.UtcNow - res.TimeGenerated;
                if (timeInCache > lifeSpan)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false; 
        }

        public static void Add(PlayFabRequestCommon rq, PlayFabResultCommon res)
        {
            try
            {
                CachedPlayFabRequest cacheRq = new CachedPlayFabRequest(rq);
                cacheData.Remove(cacheRq);
                cacheData.Add(cacheRq, new CachedPlayFabResponse(res));
            }
            catch(Exception e)
            {
                HelperFunctions.CatchException(e);
            }
            
        }

        public static PlayFabResultCommon GetResponse(PlayFabRequestCommon rq)
        {
            CachedPlayFabRequest key = new CachedPlayFabRequest(rq);
            if(CheckIfCached(key))
            {
                CachedPlayFabResponse res = cacheData[key];
                return res.Response;
            }
            else
            {
                return null;
            }
        }
    }
}


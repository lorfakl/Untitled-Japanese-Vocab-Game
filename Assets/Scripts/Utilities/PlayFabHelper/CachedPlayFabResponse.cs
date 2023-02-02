using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.SharedModels;

namespace Utilities.PlayFabHelper.Caching
{
    public class CachedPlayFabResponse 
    {
        public PlayFabResultCommon Response
        {
            get;
            private set;
        }
        
        public DateTime TimeGenerated
        {
            get;
            private set;
        }

        public CachedPlayFabResponse(PlayFabResultCommon response)
        {
            Response = response;
            TimeGenerated = DateTime.UtcNow;
        }


    }

    public class CachedPlayFabRequest : IEquatable<CachedPlayFabRequest>
    {
        PlayFabRequestCommon _request;

        public PlayFabRequestCommon Request
        {
            get { return _request; }
        }

        public CachedPlayFabRequest(PlayFabRequestCommon rq)
        {
            _request = rq;
        }

        public bool Equals(CachedPlayFabRequest other)
        {
            if(other != null)
            {
                if (this.Request.GetType() == other.Request.GetType())
                {
                    string thisObjProperties = HelperFunctions.PrintObjectFields(this.Request);
                    string otherObjProperties = HelperFunctions.PrintObjectFields(other.Request);
                    
                    if(thisObjProperties.Equals(otherObjProperties))
                    {
                        return true;
                    }
                }
            }

            return false;
            
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CachedPlayFabRequest);
        }

        public override int GetHashCode()
        {
            int hashcode = UnityEngine.Random.Range(0, 546964562);
            //HelperFunctions.Error("Implement a proper override for gethashcode");
            return hashcode;
        }
    }



}


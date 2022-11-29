using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.PlayFabHelper;

[CreateAssetMenu]
public class LocalPlayFabData : ScriptableObject
{
    [SerializeField]
    public string GroupID = "CE5B1436828B0898";

    [SerializeField]
    public string Score
    {
        get;
        set;
    }
    
}

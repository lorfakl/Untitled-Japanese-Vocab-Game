using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
public static class GlobalClientData
{
    public static bool IsGlossaryLoaded { get; private set; }

    private static Dictionary<string, JapaneseWord> EntireWordDictionary { get; set; }

    
}

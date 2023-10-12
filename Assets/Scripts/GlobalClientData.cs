using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
public static class GlobalClientData
{
    public static bool IsGlossaryLoaded { get; private set; }

    private static Dictionary<string, JapaneseWord> EntireWordDictionary { get; set; }

    public static JapaneseWord GetWord(string id)
    {
        if (IsGlossaryLoaded)
        {
            return EntireWordDictionary[id];
        }
        else
        {
            LoadWordDictionary(JSONWordLibrary.LoadWordsFromJSON());
            return EntireWordDictionary[id];
        }
    }

    public static void LoadWordDictionary(List<JapaneseWord> words)
    {
        if (!IsGlossaryLoaded)
        {
            foreach (JapaneseWord word in words)
            {
                EntireWordDictionary.Add(word.ID, word);
            }
            IsGlossaryLoaded = true;
        }
    }
}

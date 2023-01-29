using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Utilities;

public class WordBankManager : MonoBehaviour
{

    #region Public Variables
    public static JapaneseWord NextWord
    {
        get;
        private set;
    }

    public static Queue<JapaneseWord> WordBank = new Queue<JapaneseWord>();
    #endregion

    #region Private Variables
    [SerializeField]
    TMP_Text currentWordTarget;
    #endregion

    #region Events
    #endregion

    #region Unity Events
    #endregion

    #region Public Methods
    public List<JapaneseWord> SetUpWordBank()
    {
        GetNewTargetWord();
        HelperFunctions.Log(NextWord);
        currentWordTarget.color = Color.white;
        return SendNewWordBank();
    }
    #endregion

    #region Unity Methods
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region Private Methods
    
    void GetNewTargetWord()
    {
        NextWord = JSONWordLibrary.GetNewTargetWord();

        if(NextWord.Kanji == "-1")
        {
            currentWordTarget.text = NextWord.English;
        }
        else
        {
            currentWordTarget.text = NextWord.Kanji;
        }
        
    }

    List<JapaneseWord> SendNewWordBank()
    {
        List<JapaneseWord> wordBank = JSONWordLibrary.GetWordBank();
        string overlap = GetKanaOverlap(NextWord);
        foreach (var w in wordBank)
        {
            JapaneseWord word = new JapaneseWord(w);
            if(word.Kanji != NextWord.Kanji)
            {
                word.Kana += overlap;
            }
            WordBank.Enqueue(word);
        }
        return wordBank;
    }

    string GetKanaOverlap(JapaneseWord word)
    {
        if(word.Kanji != "-1")
        {
            string overlap = "";
            //HelperFunctions.Log("Finding overlap for: " + word);
            for (int i = word.Kanji.Length - 1; i > 0; i--)
            {
                if (word.Kanji[i] == word.Kana[word.Kana.Length - 1])
                {
                    overlap += word.Kanji[i];
                }
            }

            HelperFunctions.Log("Kana Overlap: " + overlap);
            return overlap;
        }
        else
        {
            return string.Empty;
        }
        
    }

    #endregion
}
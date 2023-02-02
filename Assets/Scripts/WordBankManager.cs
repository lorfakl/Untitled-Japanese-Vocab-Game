using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Utilities;
using System.Linq;
using System;

public struct WordBankObject
{
    public string DisplayText;
    public JapaneseWord Word;
}

public class WordBankManager : MonoBehaviour
{

    #region Public Variables
    public static JapaneseWord NextWord
    {
        get;
        private set;
    }

    public static Queue<WordBankObject> WordBank = new Queue<WordBankObject>();
    #endregion

    #region Private Variables
    [SerializeField]
    TMP_Text currentWordTarget;

    WordBankSettingsInterpreter _settingsInterpreter;
    #endregion

    #region Events
    #endregion

    #region Unity Events
    #endregion

    #region Public Methods
    public List<JapaneseWord> SetUpWordBank()
    {
        GetNewTargetWord();
        //HelperFunctions.Log(NextWord);
        currentWordTarget.color = Color.white;
        return SendNewWordBank();
    }
    #endregion

    #region Unity Methods
    void Start()
    {
        _settingsInterpreter = new WordBankSettingsInterpreter(StaticUserSettings.GetTranslationDirection());
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
        currentWordTarget.text = _settingsInterpreter.GetTargetText(NextWord); 
    }

    List<JapaneseWord> SendNewWordBank()
    {
        List<JapaneseWord> wordBank = JSONWordLibrary.GetWordBank();
        string overlap = GetKanaOverlap(NextWord);

        if(_settingsInterpreter.TranslateDirection == TranslateDirection.Kanji2Kana)
        {
            overlap = GetKanaOverlap(NextWord);
        }
        else
        {
            overlap = "";
        }

        foreach (var w in wordBank)
        {
            JapaneseWord word = new JapaneseWord(w);
            if(word.Kanji != NextWord.Kanji)
            {
                word.Kana += overlap;
            }
            WordBankObject entry = new WordBankObject
            {
                DisplayText = _settingsInterpreter.GetOptionText(word),
                Word = word
            };

            WordBank.Enqueue(entry);
        }
        return wordBank;
    }

    string GetKanaOverlap(JapaneseWord word)
    {
        if (word.Kanji == "-1")
        {
            return string.Empty;
        }
        else
        {
            string overlap = "";
            int decrementCount = 0;
            //HelperFunctions.Log("Finding overlap for: " + word);
            for (int i = word.Kanji.Length - 1; i > 0; i--) //In order to track all exposed kana in the kanji the kanaIndex needs to keep pace with i
            {
                int kanaIndex = word.Kana.Length - (1 + decrementCount);
                if (word.Kanji[i] == word.Kana[kanaIndex])
                {
                    overlap += word.Kanji[i];
                }
                decrementCount++;   
            }

            char[] overlapChar = overlap.ToCharArray();
            Array.Reverse(overlapChar);
            overlap = new string(overlapChar);
            return overlap;
        }
    }

    #endregion

    private class WordBankSettingsInterpreter
    {
        TranslationDirectionTransformer _wordTransformer;

        public TranslateDirection TranslateDirection
        {
            get;
            private set;
        }

        public WordBankSettingsInterpreter(TranslateDirection currentSettings)
        {
            _wordTransformer = new TranslationDirectionTransformer(currentSettings);
            TranslateDirection = currentSettings;
        }

        public string GetTargetText(JapaneseWord w)
        {
            _wordTransformer.SetDirection(w);
            return _wordTransformer.Target;
        }

        public string GetOptionText(JapaneseWord w)
        {
            _wordTransformer.SetDirection(w);
            if(10 % (int)_wordTransformer.Direction == 0)
            {
                
            }
            return _wordTransformer.Option;
        }

        private class TranslationDirectionTransformer
        {
            TranslateDirection _direction;
            
            public string Target { get; private set; }
            public string Option { get; private set; }
            public TranslateDirection Direction { get { return _direction; } }

            public TranslationDirectionTransformer(TranslateDirection t)
            {
                _direction = t; 
            }

            public void SetDirection(JapaneseWord w)
            {
                switch(_direction)
                {
                    case TranslateDirection.English2Kana:
                        Target = w.English;
                        Option = w.Kana;
                        break;

                    case TranslateDirection.English2Kanji:
                        Target = w.English;
                        Option = w.Kanji;
                        break;

                    case TranslateDirection.Kana2Kanji:
                        Target = w.Kana;
                        Option = w.Kanji;
                        break;

                    case TranslateDirection.Kana2English:
                        Target = w.Kana;
                        Option = w.English;
                        break;

                    case TranslateDirection.Kanji2English:
                        Target = w.Kanji;
                        Option = w.English;
                        break;

                    case TranslateDirection.Kanji2Kana:
                        Target = w.Kanji;
                        Option = w.Kana;
                        break;

                    case TranslateDirection.Kana2Kana:
                        Target = w.Kana;
                        Option = w.Kana;
                        break;

                    default:
                        break;
                }
                ReplaceNullKanji(w);
            }

            void ReplaceNullKanji(JapaneseWord w)
            {
                string[] wordComponents = { w.Kanji, w.Kana, w.English };
                if (Option == "-1" || Target == "-1")
                {
                    string replacement = "";
                    for (int i = 0; i < wordComponents.Length; i++)
                    {
                        if (wordComponents[i] != Option && wordComponents[i] != Target)
                        {
                            replacement = wordComponents[i];
                        }
                    }

                    if (Option == "-1")
                        Option = replacement;
                    if (Target == "-1")
                        Target = replacement;
                }
            }

            
        }
    }
}
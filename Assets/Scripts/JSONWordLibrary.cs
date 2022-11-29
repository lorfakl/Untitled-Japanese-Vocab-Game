using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Utilities;
using System.IO;
using System.Linq;

public class JSONWordLibrary : MonoBehaviour
{

    #region Public Variables
    public static List<JapaneseWord> WordsToStudy
    {
        get { return wordsToStudy; }
        private set { wordsToStudy = value; }
    }
    #endregion

    #region Private Variables
    static JapaneseWord CurrentWord
    {
        get;
        set;
    }

    List<string> linesInFile;
    static List<JapaneseWord> jsonWords;
    static List<JapaneseWord> wordsToStudy;
    #endregion

#region Events
#endregion

#region Unity Events
#endregion

#region Public Methods

    public static JapaneseWord GetNewTargetWord()
    {
        CurrentWord = GetNewRandomWord();
        return CurrentWord;
    }

    public static JapaneseWord GetNewRandomWord()
    {
        int randomIndex = Random.Range(0, wordsToStudy.Count);
        JapaneseWord nextWord = wordsToStudy[randomIndex];
        return nextWord;
    }

    public static List<JapaneseWord> GetWordBank()
    {
        int wordBankCount = Random.Range(4, 9);
        List<JapaneseWord> wordBank = new List<JapaneseWord>();
        for(int i = 0; i < wordBankCount; i++)
        {
            wordBank.Add(AddWordToWordBank(CurrentWord));
        }
        wordBank.Add(CurrentWord);
        HelperFunctions.Shuffle(wordBank);
        return wordBank;
    }

    public static void SetWordsToStudy(List<JapaneseWord> b)
    {
        WordsToStudy = b;
    }

#endregion

#region Unity Methods
    void Awake()
    {
        LoadWordFromJSON();
        //wordsToStudy = jsonWords.ToList();
        //HelperFunctions.LogListContent(wordsToStudy);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
#endregion

#region Private Methods

    void LoadWordFromJSON()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("japaneseWords");
        string jsonFile = jsonText.text;
        jsonWords = JsonConvert.DeserializeObject<List<JapaneseWord>>(jsonFile);
        //HelperFunctions.PrintListContent<JapaneseWord>(jsonWords);
    }

    static JapaneseWord AddWordToWordBank(JapaneseWord currentWord)
    {
        int randomIndex = Random.Range(0, jsonWords.Count);
        JapaneseWord nextWord = jsonWords[randomIndex];
        return nextWord;
    }


    void ReadTextFile()
    {
        string path = Application.dataPath + "//japaneseWords.txt";
        HelperFunctions.Log(path);
        List<string> fileLines = new List<string>();
        string[] lines = File.ReadAllLines(path);

        HelperFunctions.Log(lines.Length.ToString());

        linesInFile = lines.ToList();
    }

    void ParseFileLines()
    {
        jsonWords = new List<JapaneseWord>();

        foreach(string l in linesInFile)
        {
            int kanaStartIndex = l.IndexOf('[');
            int englishStartIndex = l.IndexOf(':');
            string kanji = "";
            string english = "";
            string kana = "";
            if (kanaStartIndex == -1)
            {
                //this line has no kanji
                string pureKana = l.Substring(0, englishStartIndex);
                //print(pureKana);
                kana = pureKana;
                kanji = "-1";
            }
            else
            {
                int kanaEndIndex = l.IndexOf(':');
                int totalKanaLength = (kanaEndIndex - kanaStartIndex) - 2;
                string kanaString = l.Substring(kanaStartIndex + 1, totalKanaLength);
                string kanjiString = l.Substring(0, kanaStartIndex);
                //print(kanjiString);
                //print(kanaString);
                kana = kanaString;
                kanji = kanjiString;
            }

            
            english = l.Substring(englishStartIndex+1);

            JapaneseWord word = new JapaneseWord
            {
                Kanji = kanji,
                Kana = kana,
                English = english
            };

            string jsonWordString = JsonConvert.SerializeObject(word);
            print(jsonWordString);
            jsonWords.Add(word);
        }
    }

    void WriteJsonFile()
    {
        string allJson = JsonConvert.SerializeObject(jsonWords);
        File.WriteAllText(Application.dataPath + "//japaneseWords.json", allJson);
    }

#endregion
}


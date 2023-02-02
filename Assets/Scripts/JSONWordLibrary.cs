using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Utilities;
using System.IO;
using System.Linq;
using Utilities.Events;

public class JSONWordLibrary : MonoBehaviour
{

    [SerializeField]
    GameEvent removeWordFromSessionEvent;

    [SerializeField]
    GameEvent completedSessionListEvent;

    [SerializeField]
    TextAsset kanjiJsonFile;

    [SerializeField]
    TextAsset kanaJsonFile;

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
    static List<JapaneseWord> kanjiJsonObjs;
    static List<JapaneseWord> kanaJsonObjs;
    static List<JapaneseWord> workingWordBankList;
    static List<JapaneseWord> wordsToStudy;
    #endregion

    List<JapaneseWord> removedWords = new List<JapaneseWord>();
    static Dictionary<JapaneseWord, bool> wasAnsweredDict = new Dictionary<JapaneseWord, bool>();
    #region Public Methods

    public void OnCorrectAnswerEvent_Handler(object s)
    {
        StudyObject studyObject = (StudyObject)s;
        JapaneseWord wordTarget = JSONWordLibrary.WordsToStudy.Find(word => word.Kanji == studyObject.Word.Kanji);
        wasAnsweredDict.Add(wordTarget, true);
        HelperFunctions.Log($"Correctly Translated: {studyObject.Word}");
        bool wasRemoved = WordsToStudy.Remove(wordTarget);
        if (wasRemoved)
        {
            removeWordFromSessionEvent.Raise(WordsToStudy.Count);
            removedWords.Add(wordTarget);

            if(WordsToStudy.Count == 0)
            {
                completedSessionListEvent.Raise();

            }
        }
        else
        {
            HelperFunctions.Warning(wordTarget + " was not successful removed");
        }
        HelperFunctions.Log("Words left to study: " + WordsToStudy.Count);
        HelperFunctions.LogListContent(wordsToStudy);
    }

    public static JapaneseWord GetNewTargetWord()
    {
        CurrentWord = GetNewRandomWord();
        return CurrentWord;
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
        RemoveCopies(WordsToStudy);
    }

    #endregion

    #region Unity Methods
    void Awake()
    {
        LoadWordFromJSON();
        //wordsToStudy = kanjiJsonObjs.ToList();
        //HelperFunctions.LogListContent(wordsToStudy);
    }

    void Start()
    {
        /*ReadTextFile();
        ParseFileLines();
        WriteJsonFile();*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region Private Methods

    void LoadWordFromJSON()
    {
        string kanjiFile = kanjiJsonFile.text;
        kanjiJsonObjs = JsonConvert.DeserializeObject<List<JapaneseWord>>(kanjiFile);
        //HelperFunctions.PrintListContent<JapaneseWord>(kanjiJsonObjs);
        string kanaFile = kanaJsonFile.text;
        kanaJsonObjs = JsonConvert.DeserializeObject<List<JapaneseWord>>(kanaFile);

        if(StaticUserSettings.IsKanjiStudyTopic())
        {
            workingWordBankList = kanjiJsonObjs;
        }
        else
        {
            workingWordBankList = kanaJsonObjs;
        }
    }

    static JapaneseWord AddWordToWordBank(JapaneseWord currentWord)
    {
        int randomIndex = Random.Range(0, workingWordBankList.Count);
        JapaneseWord nextWord = workingWordBankList[randomIndex];
        return nextWord;
    }

    private static JapaneseWord GetNewRandomWord()
    {
        int randomIndex = Random.Range(0, WordsToStudy.Count);
        JapaneseWord nextWord = WordsToStudy[randomIndex];
        if(wasAnsweredDict.ContainsKey(nextWord))
        {
            throw new System.Exception($"{nextWord} was not properly removed kinda sucks");
        }
        return nextWord;
    }

    private static void RemoveCopies(List<JapaneseWord> words)
    {
        List<JapaneseWord> copies = new List<JapaneseWord>();
        for(int i = 0; i < words.Count; i++)
        {
            for(int j = i+1; j < words.Count; j++)
            {
                if(words[j].Kana == words[i].Kana)
                {
                    HelperFunctions.Log("Found some copies");
                    copies.Add(words[i]);
                    HelperFunctions.LogListContent(copies);
                }
            }
        }

        HelperFunctions.Log("Found some copies");
        HelperFunctions.LogListContent(copies);
    }

    void ReadTextFile()
    {
        string path = Application.dataPath + "//kanaFile.txt";
        HelperFunctions.Log(path);
        List<string> fileLines = new List<string>();
        string[] lines = File.ReadAllLines(path);

        HelperFunctions.Log(lines.Length.ToString());

        linesInFile = lines.ToList();
    }

    void ParseFileLines()
    {
        kanjiJsonObjs = new List<JapaneseWord>();

        foreach(string l in linesInFile)
        {
            int spaceIndex = l.IndexOf(" ");
            string english = l.Substring(spaceIndex);
            string kana = l.Substring(0, spaceIndex);

            JapaneseWord word = new JapaneseWord
            {
                Kanji = "-1",
                Kana = kana,
                English = english
            };

            string jsonWordString = JsonConvert.SerializeObject(word);
            print(jsonWordString);
            kanjiJsonObjs.Add(word);
        }
    }

    void WriteJsonFile()
    {
        string allJson = JsonConvert.SerializeObject(kanjiJsonObjs);
        File.WriteAllText(Application.dataPath + "//kana.json", allJson);
    }

    #endregion
}


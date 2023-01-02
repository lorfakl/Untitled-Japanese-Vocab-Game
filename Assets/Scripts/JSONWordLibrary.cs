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
        int randomIndex = Random.Range(0, WordsToStudy.Count);
        JapaneseWord nextWord = WordsToStudy[randomIndex];
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
        string path = Application.dataPath + "//kanaFile.txt";
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
            jsonWords.Add(word);
        }
    }

    void WriteJsonFile()
    {
        string allJson = JsonConvert.SerializeObject(jsonWords);
        File.WriteAllText(Application.dataPath + "//kana.json", allJson);
    }

#endregion
}


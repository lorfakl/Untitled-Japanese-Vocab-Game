using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Utilities;

public class DefinitionPanelEventProcessor : MonoBehaviour
{

    #region Public Variables

    #endregion

    #region Private Variables
    [SerializeField]
    StudySceneManager sceneManager;

    [SerializeField] 
    RectTransform definitionTextPanel;


    [SerializeField]
    TMP_Text kanjiText;
    [SerializeField]
    TMP_Text kanaText;
    [SerializeField]
    TMP_Text meaningText;

    [SerializeField]
    TMP_Text reportText;

    [SerializeField]
    Image displayUI;

    [SerializeField]
    Sprite statsBox;

    [SerializeField]
    Button NextWordButton;

    [SerializeField]
    TMP_Text nextWordButtonText;

    bool _wasSessionComplete = false;
    Dictionary<string, string> _statsReport = new Dictionary<string, string>();
    #endregion

    #region Events
    #endregion

    #region Unity Events
    public void OnSessionComplete(object r)
    {
        _wasSessionComplete = true; 
        
        StudySession session = r as StudySession;
        _statsReport = session.GenerateRecordReport();


        
        definitionTextPanel.gameObject.SetActive(true);
        nextWordButtonText.text = "Show Stats";
        NextWordButton.onClick.RemoveAllListeners();
        NextWordButton.onClick.AddListener(DisplayStatSummary);
    }
    public void StudyObjectClickedEventHandler()
    {
        if(_wasSessionComplete)
        {
            return;
        }
        ShowDefintion();
        definitionTextPanel.gameObject.SetActive(true);
    }

    public void NextWordButtonClickedEventHandler()
    {
        if (_wasSessionComplete)
        {
            return;
        }
        definitionTextPanel.gameObject.SetActive(false);
    }
    #endregion

    #region Unity Methods
    void Start()
    {
        HelperFunctions.Log("DefinitionPanelEventProcessor Start Function");
        //definitionTextPanel = GameObject.FindGameObjectWithTag("def").GetComponent<RectTransform>();
        HelperFunctions.Log($"There are {definitionTextPanel.childCount} children");
        displayUI.sprite = sceneManager.GetDefinitionPanelImage(StudySceneManager.PanelUI.Meaning);

    }
    #endregion

    #region Private Methods

    void ShowDefintion()
    {
        JapaneseWord def = WordBankManager.NextWord;
        if(def.Kanji == "-1")
        {
            kanjiText.text = "None";
        }
        else
        {
            kanjiText.text = def.Kanji;
        }

        kanaText.text = def.Kana;
        meaningText.text = def.English;
        definitionTextPanel.gameObject.SetActive(true);
    }

    void DisplayStatSummary()
    {
        kanaText.gameObject.SetActive(false);
        kanjiText.gameObject.SetActive(false);
        meaningText.gameObject.SetActive(false);
        
        displayUI.sprite = sceneManager.GetDefinitionPanelImage(StudySceneManager.PanelUI.Stats);
        foreach (var key in _statsReport.Keys)
        {
            reportText.text += key + ": " + _statsReport[key] + "\n";
        }
        reportText.text += "Total Score Gained: " + ScoreEventProcessors.Score + "\n";
        definitionTextPanel.gameObject.SetActive(true);
        
        for(int i = 0; i < definitionTextPanel.transform.childCount; i++)
        {
            definitionTextPanel.GetChild(i).gameObject.SetActive(false);
        }
        reportText.enabled = true;
        reportText.gameObject.SetActive(true);
        NextWordButton.onClick.RemoveAllListeners();
        NextWordButton.onClick.AddListener(ReturnToMainMenu);
        nextWordButtonText.text = "Return Home";
    }

    void ReturnToMainMenu()
    {
        HelperFunctions.LoadScene(ProjectSpecificGlobals.SceneNames.MenuScene);
    }
    #endregion
}
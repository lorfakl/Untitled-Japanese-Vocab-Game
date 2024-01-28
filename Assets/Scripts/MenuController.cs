using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using Utilities;
using Utilities.Events;
using ProjectSpecificGlobals;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    GameObject statsPanel;
    [SerializeField]
    GameObject shopPanel;
    [SerializeField]
    GameObject settingsPanel;
    [SerializeField]
    GameObject profilePanel;
    [SerializeField]
    GameObject openingPanel;
    [SerializeField]
    GameObject glossaryPanel;
    [SerializeField]
    GameObject studySetPanel;

    [SerializeField]
    GameObject[] panels;

    [SerializeField]
    GameEvent _profilePageActiveEvent;

    [SerializeField]
    GameEvent _shopPageActiveEvent;

    [SerializeField]
    OpeningPageManager openingPage;

    GameObject activePanel;

    Dictionary<GameObject, Vector3> stowPositionDict = new Dictionary<GameObject, Vector3>();


    public static bool PanelChangeEnabled { get; set; }
    #region Public Methods
    public void StudyButtonClickHandler()
    {
        if (!PanelChangeEnabled)
        {
            openingPage.CheckDisplayName();
            return;
        }
        AsyncSceneLoader.StartSceneLoadProcess(SceneNames.StudyScene);
    }

    public void StatsButtonClickHandler()
    {
        CheckActivePanel(statsPanel);
    }

    public void SettingButtonClickHandler()
    {
        CheckActivePanel(settingsPanel);
    }

    public void ProfileButtonClickHandler()
    {
        CheckActivePanel(profilePanel);
        _profilePageActiveEvent.Raise();
    }

    public void ShopButtonClickHandler()
    {
        CheckActivePanel(shopPanel); 
        _shopPageActiveEvent.Raise();
    }

    public void CustomStudySetButtonClickHandler()
    {
        CheckActivePanel(studySetPanel);
    }

    public void GlossaryButtonClickHandler()
    {
        CheckActivePanel(glossaryPanel);
    }
    #endregion

    #region Unity Methods

    private void Awake()
    {
        foreach(GameObject p in panels)
        {
            if(p.transform.localPosition == Vector3.zero)
            {
                activePanel = p;
            }
            p.SetActive(false);
            stowPositionDict.Add(p, p.transform.localPosition);
            //HelperFunctions.Log("Local Position: " + p.gameObject.transform.localPosition);
            //HelperFunctions.Log("Position: " + p.gameObject.transform.position);
        }

        if(activePanel != null)
        {
            activePanel.SetActive(true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    private void CheckActivePanel(GameObject panel)
    {
        if(!PanelChangeEnabled)
        {
            openingPage.CheckDisplayName();
            return;
        }

        if (!panel.activeInHierarchy)
        {
            ChangeActivePanel(panel);
        }
        else
        {
            panel.GetComponent<RectTransform>().localPosition = Vector3.zero;
            activePanel = panel;
        }
    }

    private void ChangeActivePanel(GameObject selectedPanel)
    {
        if(activePanel != null)
        {
            Vector3 stowPosition = stowPositionDict[activePanel];
            activePanel?.GetComponent<RectTransform>().DOLocalMove(stowPosition, .25f);

        }
        
        for (int i = 0; i < panels.Length; i++)
        {
            if(panels[i] != selectedPanel)
            {
                panels[i].SetActive(false);
            }
        }

        selectedPanel.SetActive(true);
        activePanel = selectedPanel;
        selectedPanel.GetComponent<RectTransform>().DOLocalMove(Vector3.zero, .25f);
    }


}

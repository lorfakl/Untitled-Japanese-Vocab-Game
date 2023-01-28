using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using Utilities;
using Utilities.Events;

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
    GameObject[] panels;

    [SerializeField]
    GameEvent _profilePageActiveEvent;
    [SerializeField]
    GameEvent _shopPageActiveEvent;

    GameObject activePanel;

    Dictionary<GameObject, Vector3> stowPositionDict = new Dictionary<GameObject, Vector3>();

    #region Public Methods
    public void StudyButtonClickHandler()
    {
        SceneManager.LoadScene("StudyScene", LoadSceneMode.Single);

    }

    public void StatsButtonClickHandler()
    {

        if (!statsPanel.activeInHierarchy)
        {
            ChangeActivePanel(statsPanel);
        }
        else
        {
            statsPanel.GetComponent<RectTransform>().localPosition = Vector3.zero;
            activePanel = statsPanel;
        }
    }

    public void SettingButtonClickHandler()
    {
        if (!settingsPanel.activeInHierarchy)
        {
            ChangeActivePanel(settingsPanel);
        }
        else
        {
            settingsPanel.GetComponent<RectTransform>().localPosition = Vector3.zero;
            activePanel = settingsPanel;
        }
    }

    public void ProfileButtonClickHandler()
    {
        if (!profilePanel.activeInHierarchy)
        {
            ChangeActivePanel(profilePanel);
        }
        else
        {
            profilePanel.GetComponent<RectTransform>().localPosition = Vector3.zero;
            activePanel = profilePanel;
        }

        _profilePageActiveEvent.Raise();
    }

    public void ShopButtonClickHandler()
    {
        if (!shopPanel.activeInHierarchy)
        {
            ChangeActivePanel(shopPanel);
        }
        else
        {
            shopPanel.GetComponent<RectTransform>().localPosition = Vector3.zero;
            activePanel = shopPanel;
        }

        _shopPageActiveEvent.Raise();
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

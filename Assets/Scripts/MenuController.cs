using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

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

    GameObject[] panels;
    GameObject activePanel;
    Vector3 stowedPosition = new Vector3(-1080, 0, 0);
    public void StudyButtonClickHandler()
    {
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);

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
    }
    // Start is called before the first frame update
    void Start()
    {
        panels = new GameObject[] {profilePanel, settingsPanel, shopPanel, statsPanel};
        foreach(GameObject p in panels)
        {
            p.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ChangeActivePanel(GameObject panel)
    {
        activePanel?.GetComponent<RectTransform>().DOLocalMove(stowedPosition, .25f);

        for (int i = 0; i < panels.Length; i++)
        {
            if(panels[i] != panel)
            {
                panels[i].SetActive(false);
            }
        }

        panel.SetActive(true);
        activePanel = panel;
        panel.GetComponent<RectTransform>().DOLocalMove(Vector3.zero, .25f);

    }


}

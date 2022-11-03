using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StatsMenuEventHandler : MonoBehaviour
{
    public void BackButtonClickEventHandler()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void StatsButtonClickEventHandler()
    {
        SceneManager.LoadScene("StatsScene", LoadSceneMode.Single);
    }

    public void SettingsButtonClickEventHandler()
    {
        SceneManager.LoadScene("SettingsScene", LoadSceneMode.Single);
    }
}

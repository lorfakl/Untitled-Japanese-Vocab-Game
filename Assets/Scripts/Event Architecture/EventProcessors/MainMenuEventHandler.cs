using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuEventHandler : MonoBehaviour
{
    public void StudyButtonClickEventHandler()
    {
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
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

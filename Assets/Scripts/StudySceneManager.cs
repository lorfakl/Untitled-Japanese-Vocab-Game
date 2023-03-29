using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudySceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    AudioSource mainTheme;


    private void Awake()
    {
        if(PlayFabController.IsAuthenticated)
        {

        }
    }

    void Start()
    {
        mainTheme.volume = StaticUserSettings.GetMusicVolume();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

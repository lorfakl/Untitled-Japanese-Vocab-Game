using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainThemeController : MonoBehaviour
{
    [SerializeField]
    AudioSource mainTheme;

    [SerializeField]
    AudioClip menuTheme;

    private void Awake()
    {
        mainTheme.clip = menuTheme;

        mainTheme.Play();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        mainTheme.volume = StaticUserSettings.GetMusicVolume();
    }

    // Update is called once per frame
    void Update()
    {
        //mainTheme.volume = StaticUserSettings.GetMusicVolume();
    }
}

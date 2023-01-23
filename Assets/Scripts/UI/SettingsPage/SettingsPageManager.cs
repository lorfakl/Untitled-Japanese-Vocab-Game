using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Utilities.Events;
using Utilities.PlayFabHelper;
using Utilities.PlayFabHelper.CurrentUser;
using Utilities.SaveOperations;

public class SettingsPageManager : MonoBehaviour
{
    [SerializeField]
    Button saveSettingsBtn;

    [SerializeField]
    GameEvent settingsLoadedEvent;

    public static UserSettingsData CurrentSettings { get; private set; }
    public static UserSettingsData ModifiedSettings { get; private set; }
    public static bool UseDefaultSettings { get; private set; }

    private void Awake()
    {
        ModifiedSettings = new UserSettingsData();
    }
    // Start is called before the first frame update
    void Start()
    {
        if(PlayFabController.IsAuthenticated)
        {
            OnAuthenticated();
        }
        else
        {
            PlayFabController.IsAuthedEvent += OnAuthenticated;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnAuthenticated()
    {        
        if(CurrentSettings == null)
        {
            CurrentSettings = SaveSystem.Load<UserSettingsData>(DataCategory.Settings);
            if(CurrentSettings == default)
            {
                LoadSettingsFromPlayFab();
            }
            else
            {
                ModifiedSettings = new UserSettingsData(CurrentSettings);
            }
            CurrentAuthedPlayer.UserSettings = CurrentSettings;
        }
        else
        {
            ModifiedSettings = new UserSettingsData(CurrentSettings);
        }
    }

    public void OnSettingsSaved()
    {
        SaveSystem.Save<UserSettingsData>(ModifiedSettings, DataCategory.Settings);
        PlayFabController.InitiateFileUpload(Playfab.UserEntityKey,DataCategory.Settings, "Settings.bruh");
        HelperFunctions.Log(ModifiedSettings);
        CurrentAuthedPlayer.UserSettings = ModifiedSettings;
    }

    void LoadSettingsFromPlayFab()
    {
        PlayFabController.GetFileInfo(Playfab.UserEntityKey, DownloadSettingsFile);
        MessageBox loadingSettings = MessageBoxFactory.Create(MessageBoxType.Loading, "Local settings not found. Loading Settings from Cloud", "Downloading Settings...", settingsLoadedEvent);
        loadingSettings.DisplayLoadingMessageBox();
    }

    void DownloadSettingsFile(List<PlayFabFileInfo> files)
    {
        string settingsFileUrl = "";

        foreach(var file in files)
        {
            if(file.FileName.Contains("Set"))
            {
                settingsFileUrl = file.DownloadUrl;
                break;
            }
        }

        if(string.IsNullOrEmpty(settingsFileUrl))
        {
            HelperFunctions.Log("Settings file not found");
            settingsLoadedEvent.Raise();
            MessageBoxFactory.Create(MessageBoxType.Message, "Settings File Not found", "Unable to Get Settings").DisplayMessageBox(null);
            UseDefaultSettings = true;
            return;
        }
        else
        {
            PlayFabController.PerformDownload(settingsFileUrl, ConvertDownloadBytesToUserSettings);
        }
    }

    void ConvertDownloadBytesToUserSettings(byte[] settingsFileContent)
    {
        settingsLoadedEvent.Raise();
        UserSettingsData settings = SaveSystem.ConvertToObject<UserSettingsData>(settingsFileContent);
        if(settings != default)
        {
            CurrentSettings = settings;
        }
    }
}

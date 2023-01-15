using System.Collections;
using System.Collections.Generic;
using Utilities.SaveOperations;
using Utilities.PlayFabHelper;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Utilities;
using Utilities.PlayFabHelper.CurrentUser;
using System;

public class AvatarDisplayController : MonoBehaviour
{
    [SerializeField]
    Button _headBack;

    [SerializeField]
    Button _headNext; 
    
    [SerializeField]
    Button _topBack;

    [SerializeField]
    Button _topNext;

    [SerializeField]
    Button _botBack;

    [SerializeField]
    Button _botNext;

    [SerializeField]
    Button _skinBack;

    [SerializeField]
    Button _skinNext;

    

    [SerializeField]
    Sprite[] _tops;

    [SerializeField]
    Sprite[] _heads;

    [SerializeField]
    Sprite[] _skins;

    [SerializeField]
    Sprite[] _bottoms;

    [SerializeField]
    Image _skinImage;

    [SerializeField]
    Image _headImage;
    
    [SerializeField]
    Image _topImage;

    [SerializeField]
    Image _botImage;

    [SerializeField]
    SpriteRenderer _skinSR;

    [SerializeField]
    SpriteRenderer _headSR;

    [SerializeField]
    SpriteRenderer _faceSR;

    [SerializeField]
    SpriteRenderer _topSR;

    [SerializeField]
    SpriteRenderer _botSR;

    int _currentHeadIndex = 0;
    int _currenttopIndex = 0;
    int _currentbotIndex = 0;
    int _currentSkinIndex = 1;
    AvatarData _avatarData;
    Dictionary<AvatarItemLocation, string> ava = new Dictionary<AvatarItemLocation, string>();
    // Start is called before the first frame update
    void Start()
    {
        _headBack.onClick.AddListener(ShowPrevHead);   
        _headNext.onClick.AddListener(ShowNextHead);
        _topBack.onClick.AddListener(ShowPrevTop);
        _topNext.onClick.AddListener(ShowNextTop);
        _skinBack.onClick.AddListener(ShowPrevSkin);
        _skinNext.onClick.AddListener(ShowNextSkin);
        _botBack.onClick.AddListener(ShowPrevBot);
        _botNext.onClick.AddListener(ShowNextBot);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartStudy()
    {
        try
        {
            ava.Add(AvatarItemLocation.Head, _headImage.sprite.name);
            HelperFunctions.Log("Head Sprite Name: " + _headImage.sprite.name);
        }
        catch (UnassignedReferenceException e)
        {
            HelperFunctions.CatchException(e);
            ava.Add(AvatarItemLocation.Head, "None");
            HelperFunctions.Log("Head Sprite Name: " + "Naked");
        }

        try
        {
            ava.Add(AvatarItemLocation.Top, _topImage.sprite.name);
            HelperFunctions.Log("Top Sprite Name: " + _topImage.sprite.name);
        }
        catch (UnassignedReferenceException e)
        {
            HelperFunctions.CatchException(e);
            ava.Add(AvatarItemLocation.Top, "None");
            HelperFunctions.Log("Top Sprite Name: " + "Naked");
        }

        try
        {
            ava.Add(AvatarItemLocation.Bottom, _botImage.sprite.name);
            HelperFunctions.Log("Bottom Sprite Name: " + _botImage.sprite.name);
        }
        catch(UnassignedReferenceException e)
        {
            HelperFunctions.CatchException(e);
            ava.Add(AvatarItemLocation.Bottom, "None");
            HelperFunctions.Log("Bottom Sprite Name: " + "Naked");
        }

        ava.Add(AvatarItemLocation.Skin, _skinImage.sprite.name);
        HelperFunctions.Log("Skin Sprite Name: " + _skinImage.sprite.name);
        /*ava.Add(AvatarItemLocation.Bottom, _botImage.sprite.name);
        HelperFunctions.Log("Bottom Sprite Name: " + _botImage.sprite.name);*/
        ava.Add(AvatarItemLocation.Face, "None");



        //HelperFunctions.Warning("Avatar Data is not saved");
        byte[] imageBytes = ScreenShoter.TakeScreenShot(200, 200);
        

        

    }

    public void CreateAvatarData(byte[] pngData)
    {
        _avatarData = new AvatarData(ava, pngData);
        _avatarData.ImageData = pngData;

        SaveSystem.Save(_avatarData, DataCategory.Avatar);
        CurrentAuthedPlayer.CurrentUser.UpdateAvatar(_avatarData);
        PlayFabController.UploadAvatarImage();
        SceneManager.LoadScene("ArcadeStudyScene", LoadSceneMode.Single);
    }

    Sprite ShowNextOption(ref int index, bool nxt, Sprite[] spirteOptions)
    {
        if(nxt)
        {
            if(index + 1 == spirteOptions.Length)
            {
                index = 0;
            }
            else
            {
                index++;
            }
        }
        else
        {
            if (index - 1 < 0)
            {
                index = spirteOptions.Length - 1;
            }
            else
            {
                index--;
            }
        }

        return spirteOptions[index];
    }

    void DisplaySprite(Sprite s, Image i)
    {
        i.sprite = s;
        Color c = i.color;
        c.a = 1;
        i.color = c;
    }

    void DisplaySprite(Sprite s, SpriteRenderer i)
    {
        i.sprite = s;
    }

    void ShowNextSkin()
    {
        Sprite s = ShowNextOption(ref _currentSkinIndex, true, _skins);
        DisplaySprite(s, _skinImage);
        DisplaySprite(s, _skinSR);
    }

    void ShowPrevSkin()
    {
        Sprite s = ShowNextOption(ref _currentSkinIndex, false, _skins);
        DisplaySprite(s, _skinImage);
        DisplaySprite(s, _skinSR);
    }

    void ShowNextHead()
    {
        Sprite s = ShowNextOption( ref _currentHeadIndex, true, _heads);
        DisplaySprite(s, _headImage);
        DisplaySprite(s, _headSR);
    }

    void ShowPrevHead()
    {
        Sprite s = ShowNextOption( ref _currentHeadIndex, false, _heads);
        DisplaySprite(s, _headImage);
        DisplaySprite(s, _headSR);
    }

    void ShowNextTop()
    {
        Sprite s = ShowNextOption( ref _currenttopIndex, true, _tops);
        DisplaySprite(s, _topImage);
        DisplaySprite(s, _topSR);
    }

    void ShowPrevTop()
    {
        Sprite s = ShowNextOption( ref _currenttopIndex, false, _tops);
        DisplaySprite(s, _topImage);
        DisplaySprite(s, _topSR);
    }

    void ShowNextBot()
    {
        Sprite s = ShowNextOption( ref _currentbotIndex, true, _bottoms);
        DisplaySprite(s, _botImage);
        DisplaySprite(s, _botSR);
    }

    void ShowPrevBot()
    {
        Sprite s = ShowNextOption( ref _currentbotIndex, false, _bottoms);
        DisplaySprite(s, _botImage);
        DisplaySprite(s, _botSR);
    }


    
}

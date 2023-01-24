using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Utilities.PlayFabHelper.CurrentUser;
using Utilities.SaveOperations;

public class AvatarBuilder : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer _avatarHead;

    [SerializeField]
    SpriteRenderer _avatarFace;

    [SerializeField]
    SpriteRenderer _avatarTop;

    [SerializeField]
    SpriteRenderer _avatarBot;

    [SerializeField]
    SpriteRenderer _avatarSkin;


    AvatarData _avatarData;
    // Start is called before the first frame update
    void Start()
    {
        if(PlayFabController.IsAuthenticated)
        {
            LoadCurrentUserAvatar();
        }
        else
        {
            PlayFabController.IsAuthedEvent += OnAuthenticatedHandler;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnAuthenticatedHandler()
    {
        LoadCurrentUserAvatar();
    }

    void LoadAvatarData()
    {
        if(CurrentAuthedPlayer.CurrentUser == null)
        {
            _avatarData = SaveSystem.Load<AvatarData>(DataCategory.Avatar);
            if(_avatarData == default)
            {
                HelperFunctions.Error("Unable to load AvatarData");
                return;
            }
            return;
        }
    }

    void LoadCurrentUserAvatar()
    {
        if(CurrentAuthedPlayer.CurrentUser == null || CurrentAuthedPlayer.CurrentUser.Avatar == null)
        {
            LoadAvatarData();
        }
        else
        {
            _avatarData = CurrentAuthedPlayer.CurrentUser.Avatar;
        }

        _avatarData.AssignSpriteValues(_avatarSkin, _avatarHead, _avatarTop, _avatarBot);
    }

}

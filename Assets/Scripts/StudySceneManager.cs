using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;
using Utilities.PlayFabHelper.CurrentUser;
using ProjectSpecificGlobals;

public class StudySceneManager : MonoBehaviour
{
    public enum PanelUI { Stats, Meaning}

    //Boss Music Themes
    [SerializeField]
    AudioSource mainTheme;

    [SerializeField]
    AudioClip[] bossThemes;

    //Boss Animations
    [SerializeField]
    Animator animator;

    [SerializeField]
    RuntimeAnimatorController[] runtimeAnimatorControllers;

    //Background Image
    [SerializeField]
    Image bgImage;

    [SerializeField]
    Sprite[] bossBackgrounds;

    //Definition Panel
    [SerializeField]
    Sprite[] definitionDisplays;

    [SerializeField]
    Sprite[] statDisplays;

    //Next Button UI
    [SerializeField]
    Image nextWordBtnImage;

    [SerializeField]
    Sprite[] nextWordBtnImages;

    //Score Readout UI
    [SerializeField]
    Image scoreDisplayImage;

    [SerializeField]
    Sprite[] scoreDisplaySprites;

    //Target Word UI
    [SerializeField]
    Image targetWordImage;

    [SerializeField]
    Sprite[] targetWordSprites;

    [SerializeField]
    Button exitBtn;

    

    public float debugProgressValue = 0;

    float progressValue = 0;

    int[] wordsSeenThresholds = {200, 400, 600, 800 };
    float[] bossChangeChances = { .5f, .3333f, .25f, .2f };
    
    int currentProgressIndex = 0; //this index is used to retrieve the values in these arrays
    float bossChangeChance = 0;
    int bossInfoIndex = 0;
    public void OnQuitSelection()
    {
        MessageBoxFactory.CreateConfirmationBox("Are you sure you want to quit?", "Your progress will not be saved and you will return to the title screen",
            () => 
            {
                SceneManager.LoadScene(SceneNames.MenuScene.ToString(), LoadSceneMode.Single);    
            });
    }

    public Sprite GetDefinitionPanelImage(PanelUI selection)
    {
        switch(selection) 
        {
            case PanelUI.Meaning:
                return definitionDisplays[bossInfoIndex];
            case PanelUI.Stats: 
                return statDisplays[bossInfoIndex];
            default:
                return null;
        }
    }

    private void Awake()
    {

        int wordsSeen = 0;
        if (CurrentAuthedPlayer.CurrentUser == null)
        {
            wordsSeen = 20;
        }
        else
        {
            wordsSeen = CurrentAuthedPlayer.CurrentUser.WordsSeen;
        }

        exitBtn.onClick.AddListener(OnQuitSelection);

        //1000 words 5 bosses 200 words per boss
        //as the wordsSeen approaches 200 the probably of boss changing approaches 0.5
        //there are 4 thresholds 200 400 600 800 1000 by the time you get to 1000 words it should be 
        //loop through thresholds if wordsSeen is less than i use i as threshold 
        //take fraction of wordsSeen and threshold this fraction determines the probability of the next boss
        //ex ws=60 thres = 200 60/200  30% change of a 50%
        for (int i = 0; i < wordsSeenThresholds.Length; i++)
        {
            if(wordsSeen < wordsSeenThresholds[i])
            {
                HelperFunctions.Log($"Words Seen: {wordsSeen} Current Threshold {wordsSeenThresholds[i]}");
                currentProgressIndex = i;
                HelperFunctions.Log($"Progress Index {currentProgressIndex}");
                progressValue = (float)wordsSeen/(float)wordsSeenThresholds[i];
                HelperFunctions.Log($"Progress Value: {progressValue}");
                break;
            }
            else
            {
                currentProgressIndex = i;
                HelperFunctions.Log($"Progress Index {currentProgressIndex}");
            }
        }

            
        HelperFunctions.Log($"Progress Index {currentProgressIndex}. Prod Boss Change Chance: {progressValue * bossChangeChances[currentProgressIndex]}");
            
        bossChangeChance = progressValue * bossChangeChances[currentProgressIndex];


#if UNITY_EDITOR

        bossChangeChance = debugProgressValue * bossChangeChances[currentProgressIndex];
#endif

        HelperFunctions.Log($"Chance of Boss change {bossChangeChance}");
        bossInfoIndex = currentProgressIndex;

        if (bossChangeChance > Random.Range(0f, 1f))
        {
            bossInfoIndex = Random.Range(0, currentProgressIndex + 2);
        }

        mainTheme.clip = bossThemes[bossInfoIndex];
        mainTheme.Play();
            
        animator.runtimeAnimatorController = runtimeAnimatorControllers[bossInfoIndex];
        animator.Play("Base Layer.Entry");

        bgImage.sprite = bossBackgrounds[bossInfoIndex];

        if (bossInfoIndex != 0) 
        {
            targetWordImage.sprite = targetWordSprites[bossInfoIndex];
            scoreDisplayImage.sprite = scoreDisplaySprites[bossInfoIndex];
        }

        nextWordBtnImage.sprite = nextWordBtnImages[bossInfoIndex];

            
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

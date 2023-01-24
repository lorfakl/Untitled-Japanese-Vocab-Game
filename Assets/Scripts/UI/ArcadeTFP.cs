using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class ArcadeTFP : MonoBehaviour
{
    [SerializeField]
    Button startOver;

    // Start is called before the first frame update
    void Start()
    {
        startOver.onClick.AddListener(LoadLeaderboard);    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadLeaderboard()
    {
        HelperFunctions.LoadScene(ProjectSpecificGlobals.SceneNames.ArcadeLeaderboard);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Utilities.Events;

public class FriendSystem : MonoBehaviour
{
    [SerializeField]
    GameEvent _heartLostEvent;

    [SerializeField]
    GameEvent _gameOverEvent;

    int _totalPoints = 6;

    public void OnIncorrectAnswer()
    {
        _heartLostEvent.Raise();
        _totalPoints--;
        if(_totalPoints == 0)
        {
            _gameOverEvent.Raise();
            HelperFunctions.Log("game over");
            HelperFunctions.LoadScene(ProjectSpecificGlobals.SceneNames.ArcadeGameOver);
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

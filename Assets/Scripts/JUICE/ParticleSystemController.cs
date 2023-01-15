using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemController : MonoBehaviour
{

    [SerializeField]
    private ParticleSystem _particleSystem;

    [SerializeField]
    Sprite _correct;

    [SerializeField]
    Sprite _wrong1;

    [SerializeField]
    Sprite _wrong2;

    List<Sprite> wrongs = new List<Sprite>();

    public void OnCorrectAnswerEvent_Handler()
    {
        _particleSystem.textureSheetAnimation.SetSprite(0, _correct);
        _particleSystem.Play();
    }

    public void OnIncorrectAnswerEvent_Handler()
    {
        Sprite setSprite = null;
        if(Random.Range(0f, 1.0f) > .5f)
        {
            setSprite = wrongs[1];
        }
        else
        {
            setSprite = wrongs[0];
        }
        _particleSystem.textureSheetAnimation.SetSprite(0, setSprite);
        _particleSystem.Play();
    }
    // Start is called before the first frame update
    void Start()
    {
        wrongs.Add(_wrong1);
        wrongs.Add(_wrong2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

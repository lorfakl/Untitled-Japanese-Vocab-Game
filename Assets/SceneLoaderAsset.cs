using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Utilities.Events;

public class SceneLoaderAsset : MonoBehaviour
{

    [SerializeField]
    GameEvent _loadingComplete;

    [SerializeField]
    Image _image;

    private static SceneLoaderAsset _instance;

    
    public static SceneLoaderAsset StaticInstance
    {
        get { return _instance; }
    }

    public Image Image
    {
        get { return this._image; }
    }

    public GameEvent LoadingCompleteEvent
    {
        get { return this._loadingComplete; }   
    }

    public void ChangeVisibility(bool isTransparent)
    {
        _image.enabled = isTransparent; 
    }

    private SceneLoaderAsset Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = this;
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (Instance != this && Instance != null)
        {
            HelperFunctions.Log("Destroying newly created LoadingScreen object");
            Destroy(this);
            return;
        }

        _instance = this;
        _image.enabled = true;
        DontDestroyOnLoad(this.gameObject);
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

using ProjectSpecificGlobals;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Utilities;
using Utilities.Events;

namespace Utilities
{
    public class AsyncSceneLoader : MonoBehaviour
    {
        
        private static AsyncSceneLoader _instance;

        private AsyncSceneLoader Instance
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
            DontDestroyOnLoad(this.gameObject);
            //loadingImage = _loadingScreenPrefab.GetComponent<Image>();
        }

        
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public static void StartSceneLoadProcess(SceneNames s)
        {
            _instance.ConfigureLoadingScreen(s);
        }

        private void ConfigureLoadingScreen(SceneNames s)
        {

            SceneLoaderAsset.StaticInstance.ChangeVisibility(true);
            SceneLoaderAsset.StaticInstance.LoadingCompleteEvent.Raise();
            MessageBoxFactory.CreateLoadingBox("Loading Study Session", "Get Ready to Tap some words...quickly",
                SceneLoaderAsset.StaticInstance.LoadingCompleteEvent, null, SceneLoaderAsset.StaticInstance.transform, true);

            StartCoroutine(LoadScene(s));
        }

        private IEnumerator LoadScene(SceneNames s)
        {
            AsyncOperation loadSceneOp = SceneManager.LoadSceneAsync(SceneNames.StudyScene.ToString(), LoadSceneMode.Single);
            loadSceneOp.allowSceneActivation = false;

            while (!loadSceneOp.isDone)
            {
                if(loadSceneOp.progress >= 0.9f)
                {
                    loadSceneOp.allowSceneActivation = true;
                }
                yield return null;
            }


            SceneLoaderAsset.StaticInstance.LoadingCompleteEvent.Raise();

        }

        

    }
}

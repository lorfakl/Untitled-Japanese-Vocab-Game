using System.Collections;

using UnityEngine;
using UnityEngine.Events;
using Utilities;
using Utilities.Events;
using Utilities.PlayFabHelper;

public class ScreenShoter : MonoBehaviour
{
    [SerializeField]
    GameEvent _;

    private static ScreenShoter _instance;
    private Camera _camera;
    private bool takeScreenshotOnNextFrame;
    private static byte[] _pngSaveData;
    public static Texture2D PngTexture
    { 
        get;
        private set;
    }

    public UnityEvent<byte[]> PngDataCreated;

    private void Awake()
    {
        _instance = this;
        _camera = gameObject.GetComponent<Camera>();
    }

    public static byte[] TakeScreenShot(int width, int height)
    {
        _instance.InitializeScreenShot(width, height);
        return _pngSaveData;
    }

    private void InitializeScreenShot(int w, int h)
    {
        takeScreenshotOnNextFrame = true;
        _camera.targetTexture = RenderTexture.GetTemporary(w, h, 16);
    }

    private void OnPostRender()
    {
        if(takeScreenshotOnNextFrame)
        {
            takeScreenshotOnNextFrame = false;
            RenderTexture renderTexture = _camera.targetTexture;
            int w = renderTexture.width;
            int h = renderTexture.height;
            PngTexture = new Texture2D(w, h);
            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);
            PngTexture = renderResult;
            _pngSaveData = renderResult.EncodeToPNG();
            PngDataCreated.Invoke(_pngSaveData);
            string path = Application.persistentDataPath + "/" + Playfab.PlayFabID + ".png";
            System.IO.File.WriteAllBytes(path, _pngSaveData);
            HelperFunctions.Log("Image saved at path: " + path);
            RenderTexture.ReleaseTemporary(renderTexture);
            _camera.targetTexture = null;
        }
    }

   

        
}



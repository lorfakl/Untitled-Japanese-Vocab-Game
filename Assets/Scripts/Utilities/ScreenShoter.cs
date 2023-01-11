using System.Collections;

using UnityEngine;
using Utilities;
using Utilities.Events;

public class ScreenShoter : MonoBehaviour
{
    [SerializeField]
    GameEvent _;

    private static ScreenShoter _instance;
    private Camera _camera;
    private bool takeScreenshotOnNextFrame;

    private void Awake()
    {
        _instance = this;
        _camera = gameObject.GetComponent<Camera>();
    }

    public static void TakeScreenShot(int width, int height)
    {
        _instance.InitializeScreenShot(width, height);
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

            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);

            byte[] pngByteArray = renderResult.EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + "/AvatarImg.png", pngByteArray);
            HelperFunctions.Log("Image saved");

            RenderTexture.ReleaseTemporary(renderTexture);
            _camera.targetTexture = null;
        }
    }

   

        
}



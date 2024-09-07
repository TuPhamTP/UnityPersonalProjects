using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class Screenshot : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void DebugPictureJS(string base64Image);

    public RectTransform panelToCapture; 
    public Image ResultImg;

    private void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer) UnityEngine.WebGLInput.captureAllKeyboardInput = true;
    }

    public void CaptureScreenshot()
    {
        StartCoroutine(CaptureScreenshotCoroutine());
    }

    private IEnumerator CaptureScreenshotCoroutine()
    {
        yield return new WaitForEndOfFrame();

        Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
        screenshot.ReadPixels(screenRect, 0, 0);
        screenshot.Apply();

        byte[] imageBytes = screenshot.EncodeToPNG();
        string base64String = System.Convert.ToBase64String(imageBytes);

#if UNITY_WEBGL && !UNITY_EDITOR
        DebugPictureJS(base64String);
#endif



        Vector2 panelPosition = panelToCapture.transform.position;
        int panelWidth = (int)panelToCapture.rect.width;
        int panelHeight = (int)panelToCapture.rect.height;

        int startX = Mathf.Clamp((int)(panelPosition.x - panelWidth / 2), 0, Screen.width);
        int startY = Mathf.Clamp((int)(panelPosition.y - panelHeight / 2), 0, Screen.height);
        int width = Mathf.Clamp(panelWidth, 0, Screen.width - startX);
        int height = Mathf.Clamp(panelHeight, 0, Screen.height - startY);

        Texture2D panelScreenshot = new Texture2D(width, height);
        panelScreenshot.SetPixels(screenshot.GetPixels(startX, startY, width, height));
        panelScreenshot.Apply();

        Sprite newSprite = Sprite.Create(panelScreenshot, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
        ResultImg.sprite = newSprite;


    }
}


/*
screenshot world

public Camera uiCamera;
    public RectTransform panelToCapture;  
    public Image ResultImg; 

    public void CaptureScreenshot()
    {
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        uiCamera.targetTexture = renderTexture;
        uiCamera.Render();

        Texture2D screenshot = new Texture2D((int)panelToCapture.rect.width, (int)panelToCapture.rect.height, TextureFormat.RGB24, false);

        Rect rect = new Rect(0, 0, screenshot.width, screenshot.height);
        RenderTexture.active = renderTexture;
        screenshot.ReadPixels(rect, 0, 0);
        screenshot.Apply();

        Sprite newSprite = Sprite.Create(screenshot, rect, new Vector2(0.5f, 0.5f));
        ResultImg.sprite = newSprite;

        uiCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);
    }

*/
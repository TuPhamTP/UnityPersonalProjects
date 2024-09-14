using UnityEngine;
using UnityEngine.UI;

public class PinchZoom : MonoBehaviour
{
    public Camera mainCamera;
    public Text _txt;

    private float zoomSpeed = 0.0001f;  
    private float minZoom = 1f;     
    private float maxZoom = 5f;

    private bool isZooming = false;
    private float initialTouchDistance;
    private float zoomThreshold = 0.1f;

    private void Start()
    {
        _txt.text = mainCamera.orthographicSize.ToString();
    }
    /*
    void Update()
    {
        if (Input.touchCount == 2)
        {
            
        }
        else 
        {
            isZooming = false;
        }
    }
    */
    public void SetIsZoomFalse() { isZooming = false; }

    public void Zoom()
    {
        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        float currentTouchDistance = (touchZero.position - touchOne.position).magnitude;

        if (!isZooming)
        {
            initialTouchDistance = currentTouchDistance;
            isZooming = true;
        }
        else
        {
            float distanceDifference = Mathf.Abs(currentTouchDistance - initialTouchDistance);

            if (distanceDifference > zoomThreshold)
            {
                float deltaMagnitudeDiff = initialTouchDistance - currentTouchDistance;

                mainCamera.orthographicSize += deltaMagnitudeDiff * zoomSpeed;

                mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, minZoom, maxZoom);
            }
        }

        _txt.text = mainCamera.orthographicSize.ToString() + "-" + (Input.touchCount == 2).ToString();

        if (touchZero.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Ended ||
            touchZero.phase == TouchPhase.Canceled || touchOne.phase == TouchPhase.Canceled)
        {
            
        }
    }


}

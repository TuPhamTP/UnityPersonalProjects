using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
    [DllImport("__Internal")] private static extern void OpenWebsite();
    [DllImport("__Internal")] private static extern int GetDeviceType();

    //[SerializeField] private PinchZoom _pinchZoom;
    [SerializeField] private SpriteRenderer[] _roadSRs;
    [SerializeField] private SpriteRenderer[] _dotSRs;
    [SerializeField] private SpriteRenderer[] _txtStationSRs;
    [SerializeField] private Sprite[] _txtStationViewMoreBlurs, _txtStationBlurs;//*
    [SerializeField] private Sprite[] _txtStationViewMores, _txtStations;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Transform _positionChecker;
    [SerializeField] private Transform _background;
    [SerializeField] private GameObject _arrow, _tapIcon;
    [SerializeField] private Transform _tapIconStart, _tapIconEnd;
    [SerializeField] private GameObject _panelMap;
    [SerializeField] private GameObject _txtStart;
    public Text _txtDevice;

    private List<Sprite> _currentTxtStations = new List<Sprite>();
    private List<Sprite> _currentTxtStationBlurs = new List<Sprite>();
    private Color _transparentColor = new Color(1, 1, 1, 0);
    private Vector3 _tapIconDirection;
    private Vector3 _startCameraPos = new Vector3(0.73f, -9, -10);
    private Vector3 _dragOrigin;
    private Vector2 _minBgBound, _maxBgBound;
    private int _currentID = -1;
    private bool _showStation6First;
    private bool _playArrow, _playTapIcon;
    private bool _openedWebsite;
    private bool _canDrag;
    private bool _isWebGL;

    float camHeight;
    float camWidth;
    SpriteRenderer backgroundSprite;
    Vector2 backgroundSize;

    private void CalculateBound()
    {
        // Get the camera's size based on its orthographic size
        camHeight = _mainCamera.orthographicSize * 2;
        camWidth = camHeight * _mainCamera.aspect;

        // Calculate the minimum and maximum boundaries the camera can move
        _minBgBound = (Vector2)_background.position - backgroundSize / 2 + new Vector2(camWidth / 2, camHeight / 2);
        _maxBgBound = (Vector2)_background.position + backgroundSize / 2 - new Vector2(camWidth / 2, camHeight / 2);
    }

    void Start()
    {
        _isWebGL = Application.platform == RuntimePlatform.WebGLPlayer;
        if (_isWebGL)
        {
            bool _isPhone = (GetDeviceType() == 1) ? true : false;
            _txtDevice.text = _isPhone ? "Phone" : "Computers";
        }

        DOVirtual.DelayedCall(0.2f, () =>
        {
            // Calculate the background bounds based on the SpriteRenderer size
            backgroundSprite = _background.GetComponent<SpriteRenderer>();
            backgroundSize = backgroundSprite.bounds.size;
        });
        

        CalculateBound();

        //_mainCamera.transform.position = _startCameraPos;

        for (int i = 0; i < _txtStationViewMoreBlurs.Length; i++)
        {
            _currentTxtStationBlurs.Add(_txtStationViewMoreBlurs[i]);
            _currentTxtStations.Add(_txtStationViewMores[i]);

            _txtStationSRs[i].sprite = _currentTxtStationBlurs[i];
        }

        for (int i = 0; i < _roadSRs.Length; i++)
        {
            _roadSRs[i].color = _transparentColor;
            _dotSRs[i].transform.localScale = Vector3.zero;
        }

        _arrow.SetActive(false);
        _tapIcon.SetActive(false);
        _tapIconDirection = (_tapIconEnd.position - _tapIconStart.position).normalized;
        _tapIcon.transform.position = _tapIconStart.position;

        _txt.text = mainCamera.orthographicSize.ToString();
    }

    public void ClickSeeDetails()
    {
        DOVirtual.DelayedCall(0.1f, () =>
        {
            _panelMap.SetActive(false);
            _canDrag = true;
            _txtStart.transform.localScale = Vector3.zero;
            _txtStart.transform.DOScale(0.39768f, 1f);
        });
        
    }

    public void CliclReadMore()
    { 
        if (_currentID == _txtStationSRs.Length - 1)
        {
            if (_isWebGL) { OpenWebsite(); }
            else { Debug.Log("open web"); }
            _playTapIcon = false;
            _tapIcon.SetActive(false);
            _openedWebsite = true;
        }
    }

    public void ClickViewMore(int indexStation)
    {
        if (_currentID == indexStation)
        {
            _currentTxtStations[indexStation] = _txtStations[indexStation];
            _currentTxtStationBlurs[indexStation] = _txtStationBlurs[indexStation];

            _txtStationSRs[indexStation].sprite = _currentTxtStations[indexStation];
        }
    }

    void Update()
    {
        if (_canDrag)
        {
            CheckInput();
            CheckPosition();

            if (_playArrow)
            {
                if (_arrow.transform.localPosition.x < 2)
                {
                    _arrow.transform.localPosition += Vector3.right * Time.deltaTime;
                }
                else { _arrow.transform.localPosition = new Vector3(0, 0.48f, 10); }
            }

            if (_playTapIcon)
            {
                if (_tapIcon.transform.position.y > _tapIconEnd.position.y)
                {
                    _tapIcon.transform.position += _tapIconDirection * Time.deltaTime * 0.35f;
                }
                else { _tapIcon.transform.position = _tapIconStart.position; }
            }
        }
        
    }

    Vector3 _worldPositionChecker;
    Vector3 _textWorldPos;
    private void CheckPosition()
    {
        for (int i = 0; i < _txtStationSRs.Length; i++)
        {
            _worldPositionChecker = _positionChecker.position;
            _textWorldPos = _txtStationSRs[i].transform.position;
            if (Mathf.Abs(_worldPositionChecker.x - _textWorldPos.x) < 10f
                && _worldPositionChecker.y - _textWorldPos.y > -0.2f
                && i != _currentID)
            {
                _currentID = i;
                UpdateSprites(i);
            }
        }
    }

    void UpdateSprites(int newIndex)
    {
        for (int i = 0; i < _txtStationSRs.Length; i++)
        {
            _txtStationSRs[i].sprite = _currentTxtStationBlurs[i];
        }

        _txtStationSRs[newIndex].sprite = _currentTxtStations[newIndex];

        if (_roadSRs[newIndex].color.a == 0)
        {
            _roadSRs[newIndex].DOFade(1, 0.6f);
            _dotSRs[newIndex].transform.DOScale(0.2f, 0.6f);
        }

        //Debug.Log("Index ========== " + newIndex);

        if (newIndex == 4 && !_showStation6First)
        {
            _arrow.SetActive(true);
            _playArrow = true;
        }

        if (newIndex == 5 && !_showStation6First)
        {
            _showStation6First = true;
            _arrow.SetActive(false);
        }

        if (newIndex == 8)
        {
            if (!_openedWebsite) 
            {
                _tapIcon.SetActive(true);
                _playTapIcon = true;
            }
        }
        else { _tapIcon.SetActive(false); }
    }

    private Vector3 _mousePos;
    private Vector3 _originalDistance;
    private void CheckInput()
    {
        //_txt2.text = (Input.touchCount == 1).ToString() + "--" + ()
        if ((Input.touchCount == 1))// || Input.GetMouseButton(0)
        {
            if (_canTouch1)
            {
                isZooming = false;
                CalculateBound();
                // Start dragging
                if (Input.GetMouseButtonDown(0))
                {
                    _dragOrigin = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                }

                // Continue dragging
                if (Input.GetMouseButton(0))
                {
                    Vector3 difference = _dragOrigin - _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                    _mainCamera.transform.position = ClampCamera(_mainCamera.transform.position + difference);
                }
            }
            
        }
        else if (Input.touchCount == 2)
        {
            Zoom();
        }
        else { isZooming = false; }
            
    }

    public Camera mainCamera;
    public Text _txt;

    private float zoomSpeed = 0.0001f;
    private float minZoom = 1f;
    private float maxZoom = 5f;

    private bool isZooming = false;
    private float initialTouchDistance;
    private float zoomThreshold = 0.1f;

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
            isZooming = false;
            _canTouch1 = false;
            Invoke(nameof(SetTouch1), 0.2f);
        }
    }

    bool _canTouch1 = true;
    private void SetTouch1() { _canTouch1 = true; }

    // Clamp the camera position to the background bounds
    float _clampedX, _clampedY;
    float _maxClampedY = 5;
    private Vector3 ClampCamera(Vector3 targetPosition)
    {
        _clampedX = Mathf.Clamp(targetPosition.x, _minBgBound.x, _maxBgBound.x);
        if (!_showStation6First)
        {
            if (_mainCamera.transform.position.x > 1)
            {
                _maxClampedY = 6;
            }
            _clampedY = Mathf.Clamp(targetPosition.y, _minBgBound.y, _maxClampedY);
        }
        else
        {
            _clampedY = Mathf.Clamp(targetPosition.y, _minBgBound.y, _maxBgBound.y);
        }
        
        return new Vector3(_clampedX, _clampedY, targetPosition.z);
    }
}

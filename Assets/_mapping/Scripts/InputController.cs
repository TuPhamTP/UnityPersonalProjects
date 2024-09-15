using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
    [DllImport("__Internal")] private static extern void OpenWebsite();
    [DllImport("__Internal")] private static extern int GetDeviceType();

    [SerializeField] private GameObject _startStation;
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

    [SerializeField] private Text _txtDevice, _txtOrthographicsSize;

    private List<Sprite> _currentTxtStations = new List<Sprite>();
    private List<Sprite> _currentTxtStationBlurs = new List<Sprite>();
    private Color _transparentColor = new Color(1, 1, 1, 0);
    private Vector3 _tapIconDirection;
    private Vector3 _startCameraPos = new Vector3(0.73f, -9, -10);
    private Vector3 _dragOrigin;
    private Vector2 _minBgBound, _maxBgBound;
    private int _currentID = -1;
    private bool _showStation6First;
    private bool _canInteractMap;
    private bool _playArrow, _playTapIcon;
    private bool _openedWebsite;
    private bool _isWebGL, _isPhone;

    #region CalculateBgBound
    private float _camHeight;
    private float _camWidth;
    private Vector2 _backgroundSize;

    private void CalculateBgBound()
    {
        // Get the camera's size based on its orthographic size
        _camHeight = _mainCamera.orthographicSize * 2;
        _camWidth = _camHeight * _mainCamera.aspect;

        // Calculate the minimum and maximum boundaries the camera can move
        _minBgBound = (Vector2)_background.position - _backgroundSize / 2 + new Vector2(_camWidth / 2, _camHeight / 2);
        _maxBgBound = (Vector2)_background.position + _backgroundSize / 2 - new Vector2(_camWidth / 2, _camHeight / 2);
    }
    #endregion

    void Start()
    {
        _isWebGL = Application.platform == RuntimePlatform.WebGLPlayer;
        if (_isWebGL)
        {
            _isPhone = (GetDeviceType() == 1) ? true : false;
            _txtDevice.text = _isPhone ? "Phone" : "Computers";
        }

        DOVirtual.DelayedCall(0.2f, () =>
        {
            // Calculate the background bounds based on the SpriteRenderer size
            _backgroundSize = _background.GetComponent<SpriteRenderer>().bounds.size;
            CalculateBgBound();
        });

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

        _txtOrthographicsSize.text = _mainCamera.orthographicSize.ToString();
    }

    #region ClickBtn
    public void ClickSeeDetails()
    {
        DOVirtual.DelayedCall(0.1f, () =>
        {
            _panelMap.SetActive(false);
            _canInteractMap = true;
            _startStation.transform.localScale = Vector3.zero;
            _startStation.transform.DOScale(0.39768f, 1f);
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
    #endregion

    void Update()
    {
        if (_canInteractMap)
        {
            CalculateBgBound();
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

    #region CheckPositionToBlur_ShowText
    private Vector3 _worldPositionChecker;
    private Vector3 _textWorldPos;
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
    #endregion

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

    private void CheckInput()
    {
        if (_isPhone)
        {
            if (Input.touchCount == 1)
            {
                if (_canTouchOne) 
                { 
                    _isZoomingByFingers = false; 
                    Drag(); 
                }
            }
            else if (Input.touchCount == 2)
            {
                ZoomWithFingers();
            }
            else { _isZoomingByFingers = false; }
        }
        else
        {
            Drag();
            ZoomWithMouse();
        }
          
    }

    #region Drag
    private Vector3 _dragDifference;

    private void Drag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _dragOrigin = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(0))
        {
            _dragDifference = _dragOrigin - _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            _mainCamera.transform.position = ClampCamera(_mainCamera.transform.position + _dragDifference);
        }
    }
    #endregion

    private float _minZoom = 1f;
    private float _maxZoom = 5f;

    #region ZoomWithFingers
    private float _zoomSpeedByFingers = 0.0001f;
    private bool _isZoomingByFingers = false;
    private float _initialTouchDistance;
    private float _currentTouchDistance;
    private float _distanceDifference;
    private float _deltaMagnitudeDiff;
    private float _zoomThreshold = 0.1f;
    private Touch _touchZero;
    private Touch _touchOne;

    public void ZoomWithFingers()
    {
        _touchZero = Input.GetTouch(0);
        _touchOne = Input.GetTouch(1);
        _currentTouchDistance = (_touchZero.position - _touchOne.position).magnitude;

        if (!_isZoomingByFingers)
        {
            _initialTouchDistance = _currentTouchDistance;
            _isZoomingByFingers = true;
        }
        else
        {
            _distanceDifference = Mathf.Abs(_currentTouchDistance - _initialTouchDistance);

            if (_distanceDifference > _zoomThreshold)
            {
                _deltaMagnitudeDiff = _initialTouchDistance - _currentTouchDistance;

                _mainCamera.orthographicSize += _deltaMagnitudeDiff * _zoomSpeedByFingers;

                _mainCamera.orthographicSize = Mathf.Clamp(_mainCamera.orthographicSize, _minZoom, _maxZoom);
            }
        }

        if (_touchZero.phase == TouchPhase.Ended || _touchOne.phase == TouchPhase.Ended ||
            _touchZero.phase == TouchPhase.Canceled || _touchOne.phase == TouchPhase.Canceled)
        {
            _isZoomingByFingers = false;
            _canTouchOne = false;
            Invoke(nameof(SetTouchOne), 0.2f);
        }

        _txtOrthographicsSize.text = _mainCamera.orthographicSize.ToString() + "-" + (Input.touchCount == 2).ToString();
    }

    private bool _canTouchOne = true;
    private void SetTouchOne() { _canTouchOne = true; }
    #endregion

    #region ZoomWithMouse
    private float _zoomSpeedByMouse = 0.25f;
    private float _scrollDelta;

    public void ZoomWithMouse()
    {
        _scrollDelta = Input.mouseScrollDelta.y;

        if (_scrollDelta != 0)
        {
            _mainCamera.orthographicSize -= _scrollDelta * _zoomSpeedByMouse;

            _mainCamera.orthographicSize = Mathf.Clamp(_mainCamera.orthographicSize, _minZoom, _maxZoom);
        }

        _txtOrthographicsSize.text = _mainCamera.orthographicSize.ToString();
    }
    #endregion

    #region ClampCamera
    // Clamp the camera position to the background bounds
    float _clampedX, _clampedY;
    float _maxClampedY = 5;

    private Vector3 ClampCamera(Vector3 targetPosition)
    {
        _clampedX = Mathf.Clamp(targetPosition.x, _minBgBound.x, _maxBgBound.x);
        /*
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
        */
        _clampedY = Mathf.Clamp(targetPosition.y, _minBgBound.y, _maxBgBound.y);

        return new Vector3(_clampedX, _clampedY, targetPosition.z);
    }
    #endregion


}

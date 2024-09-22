using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InputController : MonoBehaviour
{
    [DllImport("__Internal")] private static extern void OpenWebsite();
    [DllImport("__Internal")] private static extern void ClickBtnCount(int index);

    [SerializeField] private PopUpText _popUpText;
    [SerializeField] private GameObject _bgRef;
    [SerializeField] private SpriteRenderer[] _roadSRs;
    [SerializeField] private SpriteRenderer[] _dotSRs;
    [SerializeField] private SpriteRenderer[] _txtStationSRs;
    [SerializeField] private Sprite[] _txtStationViewMoreBlursEN;
    [SerializeField] private Sprite[] _txtStationViewMoresEN;
    [SerializeField] private Sprite[] _txtStationViewMoreBlursVN;
    [SerializeField] private Sprite[] _txtStationViewMoresVN;
    [SerializeField] private Sprite _btnVN, _btnEN;
    [SerializeField] private Image _btnLanguage;
    [SerializeField] private GameObject[] _viewMoreBtns;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Transform _positionChecker;
    [SerializeField] private Transform _background;
    [SerializeField] private GameObject _arrow, _tapIcon;
    [SerializeField] private Transform _tapIconStart, _tapIconEnd;
    [SerializeField] private GameObject _panelMap;
    [SerializeField] private GameObject _txtStart;
    

    [SerializeField] private Sprite _bgRefEN, _bgRefVN;
    [SerializeField] private Image _panelBg;
    [SerializeField] private Sprite _clickStartEN, _clickStartVN;
    [SerializeField] private Image _clickStart;
    [SerializeField] private Sprite _titleEN, _titleVN;
    [SerializeField] private SpriteRenderer _title;
    [SerializeField] private Sprite _txtStartEN, _txtStartVN;
    [SerializeField] private SpriteRenderer _txtStartSR;

    [SerializeField] private GameObject _handClick;
    private bool _playHandClick = true;
    private int _handClickDir = 1;

    [SerializeField] private GameObject _handDrag;
    [SerializeField] private Transform _handDragStart, _handDragEnd;
    private bool _playHandDrag;

    private Sprite[] _txtStationViewMoreBlursLoad = new Sprite[9];
    private Sprite[] _txtStationViewMoresLoad = new Sprite[9];

    private List<Sprite> _currentTxtStations = new List<Sprite>();
    private List<Sprite> _currentTxtStationBlurs = new List<Sprite>();
    private Color _transparentColor = new Color(1, 1, 1, 0);
    private Vector3 _tapIconDirection;
    private Vector3 _startCameraPos = new Vector3(0, 0, -10);//new Vector3(0.73f, -9, -10);
    private Vector3 _dragOrigin;
    private Vector2 _minBgBound, _maxBgBound;
    private int _currentID = -1;
    private bool _showStation6First;
    private bool _playArrow, _playTapIcon;
    private bool _openedWebsite;
    private bool _canDrag;

    private void CalculateBgBound()
    {
        // Calculate the background bounds based on the SpriteRenderer size
        SpriteRenderer backgroundSprite = _background.GetComponent<SpriteRenderer>();
        Vector2 backgroundSize = backgroundSprite.bounds.size;

        // Get the camera's size based on its orthographic size
        float camHeight = _mainCamera.orthographicSize * 2;
        float camWidth = camHeight * _mainCamera.aspect;

        // Calculate the minimum and maximum boundaries the camera can move
        _minBgBound = (Vector2)_background.position - backgroundSize / 2 + new Vector2(camWidth / 2, camHeight / 2);
        _maxBgBound = (Vector2)_background.position + backgroundSize / 2 - new Vector2(camWidth / 2, camHeight / 2);
    }

    void Start()
    {
        CalculateBgBound();
        _mainCamera.transform.position = _startCameraPos;

        ClickLangueBtn();

        for (int i = 0; i < _roadSRs.Length; i++)
        {
            _roadSRs[i].color = _transparentColor;
            _dotSRs[i].transform.localScale = Vector3.zero;
        }

        _handDrag.SetActive(false);
        _arrow.SetActive(false);
        _tapIcon.SetActive(false);
        _tapIconDirection = (_tapIconEnd.position - _tapIconStart.position).normalized;
        _tapIcon.transform.position = _tapIconStart.position;
    }

    private void SetUpCurrentTxtStationSprite()
    {
        _currentTxtStationBlurs.Clear();
        _currentTxtStations.Clear();
        for (int i = 0; i < _txtStationViewMoreBlursLoad.Length; i++)
        {
            _currentTxtStationBlurs.Add(_txtStationViewMoreBlursLoad[i]);
            _currentTxtStations.Add(_txtStationViewMoresLoad[i]);

            _txtStationSRs[i].sprite = _currentTxtStationBlurs[i];
        }
    }

    private void PlayHandClick()
    {
        if (_handClick.transform.localScale.x > 1.3f) { _handClickDir = -1; }
        else if (_handClick.transform.localScale.x < 0.8f) { _handClickDir = 1; }
        _handClick.transform.localScale += Vector3.one * Time.deltaTime * _handClickDir * 0.75f;
    }

    #region ClickBtn

    private bool _isVNLanguage = true;
    public void ClickLangueBtn()
    {
        _isVNLanguage = !_isVNLanguage;
        _popUpText.SetUpPopUpText(!_isVNLanguage);
        if (_isVNLanguage)
        {
            _btnLanguage.sprite = _btnVN;
            for (int i = 0; i < 9; i++)
            {
                _txtStationViewMoreBlursLoad[i] = _txtStationViewMoreBlursVN[i];
                _txtStationViewMoresLoad[i] = _txtStationViewMoresVN[i];
            }
            _panelBg.sprite = _bgRefVN;
            _clickStart.sprite = _clickStartVN;
            _title.sprite = _titleVN;
            _txtStartSR.sprite = _txtStartVN;
        }
        else
        {
            _btnLanguage.sprite = _btnEN;
            for (int i = 0; i < 9; i++)
            {
                _txtStationViewMoreBlursLoad[i] = _txtStationViewMoreBlursEN[i];
                _txtStationViewMoresLoad[i] = _txtStationViewMoresEN[i];
            }
            _panelBg.sprite = _bgRefEN;
            _clickStart.sprite = _clickStartEN;
            _title.sprite = _titleEN;
            _txtStartSR.sprite = _txtStartEN;
        }
        SetUpCurrentTxtStationSprite();
    }


    public void ClickSeeDetails()//Click to Start
    {
        _bgRef.transform.DOScale(1.25f, 2f);
        _mainCamera.transform.DOMove(new Vector3(0.73f, -9, -10), 2f);
        _panelMap.SetActive(false);
        _txtStart.transform.localScale = Vector3.zero;
        _playHandClick = false;

        DOVirtual.DelayedCall(2.1f, () =>
        {
            CalculateBgBound();
            _canDrag = true;
            _txtStart.transform.localScale = Vector3.zero;
            _txtStart.transform.DOScale(0.39768f, 1f).OnComplete(()=> 
            {
                _handDrag.transform.position = _handDragStart.position;
                _handDrag.SetActive(true);
                _playHandDrag = true; 
            });
        });
    }

    public void CliclReadMore()
    {
        if (_currentID >= 7)//_currentID == _txtStationSRs.Length - 1
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer) 
            { 
                OpenWebsite();
                ClickBtnCount(8);
            }
            else { Debug.Log("open web"); }
            _playTapIcon = false;
            _tapIcon.SetActive(false);
            _openedWebsite = true;
        }
    }

    public void ClickViewMore(int indexStation)
    {
        if (_currentID == indexStation || (_currentID >= indexStation && indexStation >= 7))
        {
            //Show Pop Up
            _popUpText.ShowPopUpText(indexStation);

            Debug.Log("click view more " + indexStation);

            if (Application.platform == RuntimePlatform.WebGLPlayer) { ClickBtnCount(indexStation); }
            else { Debug.Log("indexStation = " + indexStation); }
        }
    }
    #endregion

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

            if (_playHandDrag)
            {
                if (_handDrag.transform.position.y > _handDragEnd.position.y)
                {
                    _handDrag.transform.position += Vector3.down * Time.deltaTime * 1.2f;
                }
                else { _handDrag.transform.position = _handDragStart.position; }
            }
        }
        if (_playHandClick) { PlayHandClick(); }
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
        if (newIndex == 8) { return; }

        _playHandDrag = false;
        _handDrag.SetActive(false);

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

        if (newIndex == 7)
        {
            _txtStationSRs[8].sprite = _currentTxtStations[8];
            if (_roadSRs[8].color.a == 0)
            {
                _roadSRs[8].DOFade(1, 0.6f);
                _dotSRs[8].transform.DOScale(0.2f, 0.6f);
            }
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
        if (Input.GetMouseButtonDown(0))
        {
            _dragOrigin = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 difference = _dragOrigin - _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            _mainCamera.transform.position = ClampCamera(_mainCamera.transform.position + difference);
        }
    }

    float _clampedX, _clampedY;
    float _maxClampedY = 5.2f;
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

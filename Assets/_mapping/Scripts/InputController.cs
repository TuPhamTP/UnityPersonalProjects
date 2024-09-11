using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InputController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] _roadSRs;
    [SerializeField] private SpriteRenderer[] _dots;
    [SerializeField] private SpriteRenderer[] _textNoSRs;
    [SerializeField] private SpriteRenderer[] _textViewMoreSRs;
    [SerializeField] private Sprite[] _textNoNormals;
    [SerializeField] private Sprite[] _textNoBlurs;
    [SerializeField] private Sprite _textViewMoreNormal;
    [SerializeField] private Sprite _textViewMoreBlur;
    public Camera mainCamera;
    public Transform _positionChecker;
    public Transform background; // The background containing the sprite renderer

    private Vector3 dragOrigin;
    private Vector2 minBounds;
    private Vector2 maxBounds;
    private Vector3 _startCameraPos = new Vector3(0.73f, -9, -10);
    private Vector3 _mousePosition;

    private Color _transparentColor = new Color(1, 1, 1, 0);

    private float _clickViewMoreRadius = 0.5f;
    private int _currentID = -1;

    void Start()
    {
        // Calculate the background bounds based on the SpriteRenderer size
        SpriteRenderer backgroundSprite = background.GetComponent<SpriteRenderer>();
        Vector2 backgroundSize = backgroundSprite.bounds.size;

        // Get the camera's size based on its orthographic size
        float camHeight = mainCamera.orthographicSize * 2;
        float camWidth = camHeight * mainCamera.aspect;

        // Calculate the minimum and maximum boundaries the camera can move
        minBounds = (Vector2)background.position - backgroundSize / 2 + new Vector2(camWidth / 2, camHeight / 2);
        maxBounds = (Vector2)background.position + backgroundSize / 2 - new Vector2(camWidth / 2, camHeight / 2);

        mainCamera.transform.position = _startCameraPos;

        for (int i = 0; i < _textNoSRs.Length; i++)
        {
            _textNoSRs[i].sprite = _textNoBlurs[i];
            _textViewMoreSRs[i].sprite = _textViewMoreBlur;
        }

        for (int i = 0; i < _roadSRs.Length; i++)
        {
            _roadSRs[i].color = _transparentColor;
            _dots[i].transform.localScale = Vector3.zero;
        }
    }

    void Update()
    {
        CheckInput();
        CheckPosition();

        if (Input.GetMouseButtonDown(0))
        {
            _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _mousePosition.z = 0;
            Debug.Log("mouse pos ========== " + _mousePosition);
            for (int i = 0; i < _textViewMoreSRs.Length; i++)
            {
                if (i == _currentID && Vector3.Distance(_textViewMoreSRs[i].transform.position, _mousePosition) < _clickViewMoreRadius)
                {
                    _textNoSRs[i].maskInteraction = SpriteMaskInteraction.None;
                    _textViewMoreSRs[i].gameObject.SetActive(false);
                }
            }
        }            
    }

    Vector3 worldPositionChecker;
    Vector3 textWorldPos;

    private void CheckPosition()
    {
        
        for (int i = 0; i < _textNoSRs.Length; i++)
        {
            worldPositionChecker = _positionChecker.position;
            textWorldPos = _textNoSRs[i].transform.position;
            if (Mathf.Abs(worldPositionChecker.x - textWorldPos.x) < 10f
                && worldPositionChecker.y - textWorldPos.y > -0.2f
                && i != _currentID)
            {
                _currentID = i;
                UpdateSprites(i);
            }
        }
    }

    void UpdateSprites(int newIndex)
    {
        for (int i = 0; i < _textNoSRs.Length; i++)
        {
            _textNoSRs[i].sprite = _textNoBlurs[i];
            _textViewMoreSRs[i].sprite = _textViewMoreBlur;
        }

        _textNoSRs[newIndex].sprite = _textNoNormals[newIndex];
        _textViewMoreSRs[newIndex].sprite = _textViewMoreNormal;

        if (_roadSRs[newIndex].color.a == 0)
        {
            _roadSRs[newIndex].DOFade(1, 0.6f);
            _dots[newIndex].transform.DOScale(0.2f, 0.6f);
        }

    }

    private void CheckInput()
    {
        // Start dragging
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        // Continue dragging
        if (Input.GetMouseButton(0))
        {
            Vector3 difference = dragOrigin - mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mainCamera.transform.position = ClampCamera(mainCamera.transform.position + difference);
        }
    }

    // Clamp the camera position to the background bounds
    private Vector3 ClampCamera(Vector3 targetPosition)
    {
        float clampedX = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
        return new Vector3(clampedX, clampedY, targetPosition.z);
    }
}

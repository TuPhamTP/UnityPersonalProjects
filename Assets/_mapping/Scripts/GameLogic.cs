using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] _textNoSRs;
    [SerializeField] private SpriteRenderer[] _textViewMoreSRs;
    [SerializeField] private Sprite[] _textNoNormals;
    [SerializeField] private Sprite[] _textNoBlurs;
    [SerializeField] private Sprite _textViewMoreNormal;
    [SerializeField] private Sprite _textViewMoreBlur;
    [SerializeField] private Transform _hand;

    private IEnumerator _showNextTextAuto;
    private int _startIndex = 0;
    private int _currentIndex = -1;
    private Vector3 _startHandPos = new Vector3(1.82f, -2.75f, 0);
    private Vector3 _dragStartPos;
    private float _dragDistance;
    private float _thresholdDistance = 2f;
    private float _clickViewMoreRadius = 0.5f;
    private bool _canShowViewMore, _calledRoutine;
    private bool _playInstruction = true;

    private void Start()
    {
        for (int i = 0; i < _textNoSRs.Length; i++)
        {
            _textNoSRs[i].sprite = _textNoBlurs[i];
            _textViewMoreSRs[i].sprite = _textViewMoreBlur;
        }
        _showNextTextAuto = ShowNextTextAuto();
    }

    private IEnumerator ShowNextTextAuto()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            _calledRoutine = true;
            _startIndex = _currentIndex;
            int index = Mathf.Clamp(_startIndex + 1, 0, _textNoSRs.Length - 1);

            UpdateSprites(index);

            if (_currentIndex == _textNoSRs.Length - 1) 
            {
                yield break;
            }
        }
    }

    void Update()
    {
        HandleDrag();

        if (_playInstruction)
        {
            if (_hand.position.y < -1.5f)
            {
                _hand.position += Vector3.up * Time.deltaTime * 1f;
            }
            else
            {
                _hand.position = _startHandPos;
            }
        }
    }

    void HandleDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StopCoroutine(_showNextTextAuto);
            if (_calledRoutine) { _startIndex = _currentIndex; }
            _dragStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _dragStartPos.z = 0;

            for (int i = 0; i < _textViewMoreSRs.Length; i++)
            {
                if (_canShowViewMore && i == _startIndex && Vector3.Distance(_textViewMoreSRs[i].transform.position, _dragStartPos) < _clickViewMoreRadius)
                {
                    Debug.Log("_canShowViewMore ===== " + _canShowViewMore);
                    _textNoSRs[i].maskInteraction = SpriteMaskInteraction.None;
                    _textViewMoreSRs[i].gameObject.SetActive(false);
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _dragDistance = currentMousePos.y - _dragStartPos.y; 

            int index = Mathf.Clamp(Mathf.FloorToInt(_startIndex + _dragDistance / _thresholdDistance), 0, _textNoSRs.Length - 1);

            if (index != _currentIndex)
            {
                UpdateSprites(index);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _startIndex = _currentIndex;
            _showNextTextAuto = ShowNextTextAuto();
            StartCoroutine(_showNextTextAuto);
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

        _currentIndex = newIndex;
        Debug.Log("currentIndex ===== " + _currentIndex);
        _canShowViewMore = true;

        _playInstruction = false;
        _hand.gameObject.SetActive(false);
    }
}

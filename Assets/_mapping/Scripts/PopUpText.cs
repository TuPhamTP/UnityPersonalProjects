using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopUpText : MonoBehaviour
{
    [SerializeField] private GameObject _panelPopUp;
    [SerializeField] private Transform _popUp;
    [SerializeField] private Sprite[] _popUpENs;
    [SerializeField] private Sprite[] _popUpVNs;
    [SerializeField] private Image _popUpImg;
    //[SerializeField] private Text _txtBtnBack;

    private Sprite[] _currentPopUpSprites = new Sprite[8];


    void Start()
    {
        SetUpPopUpText(true);
        _panelPopUp.SetActive(false);
    }

    public void SetUpPopUpText(bool isEN)
    {
        if (isEN)
        {
            for (int i = 0; i < 8; i++)
            {
                _currentPopUpSprites[i] = _popUpENs[i];
            }
            //_txtBtnBack.text = "Back";
        }
        else
        {
            for (int i = 0; i < 8; i++)
            {
                _currentPopUpSprites[i] = _popUpVNs[i];
            }
            //_txtBtnBack.text = "Quay lại";
        }
    }

    public void ShowPopUpText(int stationIndex)
    {
        _popUpImg.sprite = _currentPopUpSprites[stationIndex];

        _popUp.localScale = Vector3.zero;
        _popUp.DOScale(1, 0.7f);
        _panelPopUp.SetActive(true);
    }

}

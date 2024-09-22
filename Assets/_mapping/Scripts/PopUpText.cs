using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopUpText : MonoBehaviour
{
    [SerializeField] private GameObject _panelPopUp;
    [SerializeField] private Transform _popUp;
    [SerializeField] private Text _txtBtnBack;

    [SerializeField] private Text _txtNumber;
    [SerializeField] private Text _txtTitle;
    [SerializeField] private Text _txtContent;

    [SerializeField] private string[] _txtTitleENs, _txtTitleVNs;
    [SerializeField] private string[] _txtContentENs, _txtContentVNs;

    private string[] _currentTxtNumbers = { "01", "02", "03", "04", "05", "06", "07", "08" };
    private string[] _currentTxtTitles = new string[8];
    private string[] _currentTxtContents = new string[8];

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
                _currentTxtTitles[i] = _txtTitleENs[i];
                _currentTxtContents[i] = _txtContentENs[i];
            }
            _txtBtnBack.text = "Back";
        }
        else
        {
            for (int i = 0; i < 8; i++)
            {
                _currentTxtTitles[i] = _txtTitleVNs[i];
                _currentTxtContents[i] = _txtContentVNs[i];
            }
            _txtBtnBack.text = "Quay lại";
        }
    }

    public void ShowPopUpText(int stationIndex)
    {
        _txtNumber.text = _currentTxtNumbers[stationIndex];
        _txtTitle.text = _currentTxtTitles[stationIndex];
        _txtContent.text = _currentTxtContents[stationIndex];

        _popUp.localScale = Vector3.zero;
        _popUp.DOScale(1, 0.7f);
        _panelPopUp.SetActive(true);
    }

}

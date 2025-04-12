using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Intial UI")]
    public GameObject Loadbtn;

    [Header("Try Again UI")]
    public GameObject TryAgainPanel;
    public TextMeshProUGUI MessageText;
    public GameObject tryAgainbtn;

    [Header("Loading UI")]
    public GameObject Loadingpanel;
    public TextMeshProUGUI LoadingMessageText;

    [Header("HUD UI")]
    public GameObject HUDPanel;
    public TextMeshProUGUI ModelNametext;


    void Start()
    {
        if (instance == null) instance = this;
    }
    
    public void TryAgainUI(string message)
    {
        Loadingpanel.SetActive(false);
        TryAgainPanel.SetActive(true);
        MessageText.text = message;
    }

    public void LoadingDataUI()
    {
        Loadbtn.SetActive(false);
        tryAgainbtn.SetActive(false);
        Loadingpanel.SetActive(true);
        LoadingMessageText.text = "Fetching data from the server...";
    }
    
    public void UpdateLoadingText(string message)
    {
        LoadingMessageText.text = message;
    }

    public void LoadingDone()
    {
        Loadingpanel.SetActive(false);
        HUDPanel.SetActive(true);
    }

    public void HudUIUpdate(string name)
    {
        ModelNametext.text = name;
    }

    public void HudPanelToggle()
    {
        HUDPanel.SetActive(!HUDPanel.activeSelf);
    }
}

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuitGamePopup : MonoBehaviour
{

    [Header("PopUp Items")]
    public Button noBtn;
    public Button closeBtn;
    public Button yesBtn;

    void Start()
    {

        noBtn.onClick.AddListener(()=> ClosePopUpDelegate());
        yesBtn.onClick.AddListener(()=> QuitGame());
        closeBtn.onClick.AddListener(()=> ClosePopUpDelegate());
    }

    public void QuitGame()
    {
        if (SoundManager.instance != null) SoundManager.instance.ButtonPressPlayer(true);

        if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Multiplayer && GameManager.instace.NakamaConnection.Socket.IsConnected)
        {
            Debug.Log(gameObject.name + " detect error: ln: 28");
            GameManager.instace.OnRequestQuitMatch.Invoke();
        }
        else
        {
            GridManager.ResetStaticFields();
            SceneManager.LoadScene(Global.UIScene);
        }
    }

    private void ClosePopUpDelegate()
    {
        if (SoundManager.instance != null) SoundManager.instance.ButtonPressPlayer(true);

        GamePlayUIPanel.OpenClosePopUp(gameObject, false, false);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinnerScreen : MonoBehaviour
{
    public Image Status;
    public Image Avatar;
    public Text DomiCoinStatus;
    public Text DomiCoins;

    public Image WinBorder;
    public Image LostBorder;

    public Button continueBtn;

    void Start()
    {
        continueBtn.onClick.AddListener(() => ExitTheGame());
    }

    public void ExitTheGame()
    {
        if (SoundManager.instance != null) SoundManager.instance.ButtonPressPlayer(true);

        if (GameRulesManager.currentSelectedGame_GameType == GameRulesManager.GameType.Tournament)
        {//dispach event on tournament match ending
            //JS_Hook.instance.OnTournamentMatchEnded();
        }

        if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Multiplayer && GameManager.instace.NakamaConnection.Socket.IsConnected)
        {

            Debug.Log(gameObject.name + " detect error: ln: 36");
            GameManager.instace.OnRequestQuitMatch.Invoke();
        }
        else
        {
            GridManager.ResetStaticFields();
            SceneManager.LoadScene(Global.UIScene);
        }
    }
}
    
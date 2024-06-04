using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NoOfPlayersScreen : MonoBehaviour
{
    public Button backBtn;
    public Button closeBtn;

    public Button twoPlayerBtn;
    public Button threePlayerBtn;
    public Button fourPlayerBtn;


    void Start()
    {
        backBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.noOfPlayers.gameObject, false));
        closeBtn.onClick.AddListener(() => Close());

      twoPlayerBtn.onClick.AddListener(() =>StartGame(2));
       threePlayerBtn.onClick.AddListener(() =>StartGame(3));
       fourPlayerBtn.onClick.AddListener(() =>StartGame(4));


    }
    
    void StartGame(int i)
    {
        GameRulesManager.noOfPlayers = i;
       
        if (GameRulesManager.isPrivateRoom)
        {
            UI_Manager.instance.ChangeScreen(UI_Manager.instance.invitePlayersScreen.gameObject, true);
        }
        else
        {
           // GameRulesManager.currentSelectedGame_MatchType = GameRulesManager.MatchType.Bot;
            SceneManager.LoadScene(Global.GameScene);
        }

    }
    void Close()
    {
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.coinsScreen.gameObject, false);
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.gameModeScreen.gameObject, false);
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.noOfPlayers.gameObject, false);
    }


    public void ResetBtns()
    {
          twoPlayerBtn.interactable = false;
        threePlayerBtn.interactable = false;
         fourPlayerBtn.interactable = false;
    }
}

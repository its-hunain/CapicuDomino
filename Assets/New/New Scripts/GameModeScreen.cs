using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeScreen : MonoBehaviour
{
    public Button closeBtn;

    public Button blocking; //replace with gameMode type object
    public Button capicu; //replace with gameMode type object
    public Button nines; //replace with gameMode type object


    void Start()
    {
        closeBtn.onClick.AddListener(() => BackBtnCallBack());

        capicu.onClick.AddListener(() =>    GameModeSelection(GameRulesManager.GameRules.GameMode5, capicu,false));
        blocking.onClick.AddListener(() =>GameModeSelection(GameRulesManager.GameRules.GameMode4, blocking,false));
        nines.onClick.AddListener(() => GameModeSelection(GameRulesManager.GameRules.GameMode5, nines,true));
    }
    void GameModeSelection(GameRulesManager.GameRules gameRules,Button btn,bool isNines)
    {
        GameRulesManager.currentSelectedGame_Rule = gameRules;
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.noOfPlayers.gameObject, true);
        ChangeNoOfPlayers(btn) ;
        Rule5.isNines = isNines;
    }

    void ChangeNoOfPlayers(Button btn)
    {
        UI_Manager.instance.noOfPlayers.ResetBtns();
        if (btn == blocking)
        {
           // UI_Manager.instance.ChangeScreen(UI_Manager.instance.scoreToWinScreen.gameObject, true);
            UI_Manager.instance.noOfPlayers.twoPlayerBtn.interactable = true;
        }
        else if (btn == capicu)
        {
            UI_Manager.instance.noOfPlayers.twoPlayerBtn.interactable = true;
            UI_Manager.instance.noOfPlayers.fourPlayerBtn.interactable = true;

        }
        else
        {
            UI_Manager.instance.noOfPlayers.threePlayerBtn.interactable = true;

        }
    }

    void BackBtnCallBack() 
    {    
        if (GameRulesManager.isPrivateRoom)
        {
            UI_Manager.instance.ChangeScreen(UI_Manager.instance.createJoinRoomButtonPanel.gameObject, true);
        }
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.gameModeScreen.gameObject, false);
    }

}

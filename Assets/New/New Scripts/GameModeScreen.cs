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
        closeBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.gameModeScreen.gameObject, false));

        blocking.onClick.AddListener(() =>GameModeSelection(GameRulesManager.GameRules.GameMode4, blocking));
        capicu.onClick.AddListener(() =>    GameModeSelection(GameRulesManager.GameRules.GameMode5, capicu));
        nines.onClick.AddListener(() => GameModeSelection(GameRulesManager.GameRules.GameMode5, nines));
    }
    void GameModeSelection(GameRulesManager.GameRules gameRules,Button btn)
    {
        GameRulesManager.currentSelectedGame_Rule = gameRules;
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.coinsScreen.gameObject, true);
        ChangeNoOfPlayers(btn) ;
    }

    void ChangeNoOfPlayers(Button btn)
    {
        UI_Manager.instance.noOfPlayers.ResetBtns();
        if (btn == blocking)
        {
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

}

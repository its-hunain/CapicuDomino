using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeScreen : MonoBehaviour
{
    public Button closeBtn;

    public Button blocking; //replace with gameMode type object
    public Button draw; //replace with gameMode type object
    public Button muggins; //replace with gameMode type object


    void Start()
    {
        closeBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.gameModeScreen.gameObject, false));

        blocking.onClick.AddListener(() =>GameModeSelection(GameRulesManager.GameRules.GameMode4));
        draw.onClick.AddListener(() =>    GameModeSelection(GameRulesManager.GameRules.GameMode5));
        muggins.onClick.AddListener(() => GameModeSelection(GameRulesManager.GameRules.GameMode5));
    }
    void GameModeSelection(GameRulesManager.GameRules gameRules)
    {
        GameRulesManager.currentSelectedGame_Rule = gameRules;
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.coinsScreen.gameObject, true);
    }
}

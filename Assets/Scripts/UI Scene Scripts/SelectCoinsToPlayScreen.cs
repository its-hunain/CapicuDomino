using System;
using System.Collections;
using System.Collections.Generic;
using Dominos;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SelectCoinsToPlayScreen : MonoBehaviour
{
    [Header("PopUp Items")]
    public Button backBtn;


    void Start()
    {
        backBtn.onClick.AddListener(() => backButtonDelegate());
    }


    public void MoveToNextScreen(int coinsToPlay)
    {

        SoundManager.instance.SelectModePlayer(true);

        Debug.Log("coinsToPlay: " + coinsToPlay);
        Debug.Log("playerDomiCoins: " + PlayerPersonalData.playerDomiCoins);

        if (PlayerPersonalData.playerDomiCoins < coinsToPlay)
        {
            MesgBar.instance.show("You don't have enough coins to play");
            return;
        }
        GameRulesManager.currentSelectedGame_CoinsToPlay = coinsToPlay;
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.noOfPlayers.gameObject, true);
    }


    private void backButtonDelegate()
    {
        SoundManager.instance.ButtonPressPlayer(true);
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.selectCoinsToPlayScreen.gameObject, false);
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.gameModeScreen.gameObject, true);
    }
}
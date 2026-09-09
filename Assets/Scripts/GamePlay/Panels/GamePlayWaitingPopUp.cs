using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayWaitingPopUp : MonoBehaviour
{
    public static GamePlayWaitingPopUp instance;

    public Text msgText;

    public Text TimerText;

    public List<GameObject> profilePic = new();



    private void Awake()
    {
        instance = this;
        if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot)
        {
            EnableDisable(false);
        }
        else
        {
            for (int i = 0; i < GameRulesManager.noOfPlayers; i++)
            {
                profilePic[i].SetActive(true);
                profilePic[i].transform.GetChild(0).gameObject.SetActive(false);
            }

            SetImage(0);
        }
        

        //if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot) //No need of waiting in bot game
        //{
        //}
        //else
        //{
        //    if (GameRulesManager.currentSelectedGame_GameType == GameRulesManager.GameType.SingleMatch)
        //    {
        //        SetData("Waiting for the Opponents...");
        //    }
        //}


    }

    public void SetData(string msg, bool showTimer = false)
    {
        msgText.text = msg;
        TimerText.gameObject.SetActive(showTimer);
    }

    public void SetImage(int index) 
    {
        Debug.Log("Set Image");
        var temp = Sprite.Create(PlayerPersonalData.playerTexture, new Rect(0.0f, 0.0f, PlayerPersonalData.playerTexture.width, PlayerPersonalData.playerTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
        GameObject maskParent = profilePic[index].transform.GetChild(0).gameObject;
        maskParent.SetActive(true);
        maskParent.transform.GetChild(0).GetComponent<Image>().sprite = temp;

    }

    public void EnableDisable(bool state)
    {
        Debug.Log("EnableDisable...");
        if (!state)
        { SetData(""); }

        gameObject.SetActive(state);

    }
}

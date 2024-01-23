using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dominos;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatesPopUp : MonoBehaviour
{

    [Header("Profile Details")]
    public Image    profilePicBorder;
    public Image    profilePic;
    public Text     playerName;
    public Text     playerCoins;

    [Header("Game States")]
    public Text     gamesPlayed;
    public Text     gamesWon;
    public Text     gamesWonPercentage;

    [Header("Tournament States")]
    public Text     tournamentPlayed;
    public Text     tournamentWon;
    public Text     tournamentLost;
    public Text     championShipWon;
    
    [Header("Other States")]
    public Image playerFlagIcon;
    public string  flagName;
    public Image playerClassIcon;
    public Text  className;

    [Header("Other Items")]
    public Button closeBtn;


    void Start()
    {
        closeBtn.onClick.AddListener(() => GamePlayUIPanel.OpenClosePopUp(gameObject, false , false));
    }

    public void EnableProfilePanel(ProfileStatesPopUpData profileStatesPopUpData)
    {
        profilePic.sprite       = profileStatesPopUpData.profilePic;
        playerName.text         = profileStatesPopUpData.playerName;
        playerCoins.text        = profileStatesPopUpData.playerCoins;

        gamesPlayed.text        = profileStatesPopUpData.gamesPlayed;
        gamesWon.text           = profileStatesPopUpData.gamesWon;
        gamesWonPercentage.text = float.Parse(profileStatesPopUpData.gamesWonPercentage).ToString("F2");

        tournamentPlayed.text   = profileStatesPopUpData.tournamentPlayed;
        tournamentWon.text      = profileStatesPopUpData.tournamentWon;
        tournamentLost.text     = profileStatesPopUpData.tournamentLost;
        championShipWon.text    = profileStatesPopUpData.championShipWon;

        playerFlagIcon.sprite   = profileStatesPopUpData.playerFlagIcon;
        flagName                = profileStatesPopUpData.flagName;
        playerClassIcon.sprite  = profileStatesPopUpData.playerClassIcon;
        className.text          = profileStatesPopUpData.className;

        profilePicBorder.color = GetBorderColor(profileStatesPopUpData.userGameModeInfo);

        GamePlayUIPanel.OpenClosePopUp(gameObject, true, true);
    }

    private Color GetBorderColor(UserGameModeInfo userGameModeInfo)
    {
        UserGameModeScoreInfo userGameModeScoreInfo = userGameModeInfo.userGameModeScoreInfos.ToList().Find(x => x.GameMode.Equals(GameRulesManager.currentSelectedGame_Rule));
        switch (userGameModeScoreInfo.medalRank)
        {
            case 1:
                return Color.white;
            case 2:
                return Color.yellow;
            case 3:
                return Color.blue;
            case 4:
                return new Color(1, 0.3774137f,0,1);
            //return Color.red;
            case 5:
                return new Color(0.3254902f, 0.7960785f, 0.1686275f, 1);
                //return Color.green;
            default:
                return Color.white;
        }
    }
}

public class ProfileStatesPopUpData
{
    [Header("Profile Details")]
    public Sprite profilePic;
    public string playerName;
    public string playerCoins;

    [Header("Game States")]
    public string gamesPlayed;
    public string gamesWon;
    public string gamesWonPercentage;

    [Header("Tournament States")]
    public string tournamentPlayed;
    public string tournamentWon;
    public string tournamentLost;
    public string championShipWon;

    [Header("Other States")]
    public Sprite playerFlagIcon;
    public string flagName;
    public Sprite playerClassIcon;
    public string className;


    [Header("UserGameModeInfo")]
    public UserGameModeInfo userGameModeInfo;
}

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

    public Button blockingInfo; //replace with gameMode type object
    public Button capicuInfo; //replace with gameMode type object
    public Button ninesInfo; //replace with gameMode type object
    
    void Start()
    {
        closeBtn.onClick.AddListener(() => BackBtnCallBack());

        capicu.onClick.AddListener(() =>    GameModeSelection(GameRulesManager.GameRules.GameMode5, capicu,false));
        blocking.onClick.AddListener(() =>GameModeSelection(GameRulesManager.GameRules.GameMode4, blocking,false));
        nines.onClick.AddListener(() => GameModeSelection(GameRulesManager.GameRules.GameMode5, nines,true));

        capicuInfo.onClick.AddListener(() =>     AboutBtnCallBack(capicuInfo));
        blockingInfo.onClick.AddListener(() =>   AboutBtnCallBack(blockingInfo));
        ninesInfo.onClick.AddListener(() =>      AboutBtnCallBack(ninesInfo));
    }

    void GameModeSelection(GameRulesManager.GameRules gameRules,Button btn,bool isNines)
    {
        GameRulesManager.currentSelectedGame_Rule = gameRules;
        if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Multiplayer)
        {
            UI_Manager.instance.ChangeScreen(UI_Manager.instance.selectCoinsToPlayScreen.gameObject, true);
        }
        else
        {
            UI_Manager.instance.ChangeScreen(UI_Manager.instance.noOfPlayers.gameObject, true);
        }
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
    void AboutBtnCallBack(Button btn)
    {
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.aboutScreen.gameObject, true);

        if (btn== blockingInfo)
            UI_Manager.instance.aboutScreen.UpdateUIText("ABOUT US", "Born from the passion of two seasoned domino gamers, Capicú has quickly become the go-to platform for players seeking the authentic thrill of competitive dominoes. Designed by people who live and breathe the game, Capicú brings a fresh, immersive, and fast-paced experience to domino enthusiasts everywhere. Whether you’ve been playing dominoes for years or are just discovering the game, Capicú offers a unique way to enjoy this timeless pastime, combining strategy, camaraderie, and excitement right at your fingertips. With a global community of players, Capicú provides the pure joy of dominoes anytime, anywhere.\r\n\r\nCapicú isn’t just another domino game—it’s a reflection of the passion and dedication of its creators. Built by individuals who’ve spent years honing their skills in competitive dominoes, Capicú delivers an experience that captures the essence of the game’s rich traditions and thrilling dynamics. Here’s why <b>Capicú stands out:</b>\r\n\r\n<b>Authenticity at Its Core:</b> Capicú’s creators have ensured the game stays true to its roots. The rules, gameplay, and interface are designed to mimic the feeling of playing a real-life game of dominoes with friends.\r\n\r\n<b>Fast-Paced Gameplay:</b> Dominoes is a game of both strategy and speed. Capicú reflects this by offering a fast-paced environment that keeps players on their toes while maintaining the integrity of the game.\r\n\r\n<b>Global Community:</b> Capicú brings together players worldwide, fostering a sense of community and competition. Whether you’re looking to test your skills against seasoned players or make new friends, Capicú is the perfect platform for social interaction and competitive fun.\r\n");

        if (btn== capicuInfo)
            UI_Manager.instance.aboutScreen.UpdateUIText("ABOUT US", "Born from the passion of two seasoned domino gamers, Capicú has quickly become the go-to platform for players seeking the authentic thrill of competitive dominoes. Designed by people who live and breathe the game, Capicú brings a fresh, immersive, and fast-paced experience to domino enthusiasts everywhere. Whether you’ve been playing dominoes for years or are just discovering the game, Capicú offers a unique way to enjoy this timeless pastime, combining strategy, camaraderie, and excitement right at your fingertips. With a global community of players, Capicú provides the pure joy of dominoes anytime, anywhere.\r\n\r\nCapicú isn’t just another domino game—it’s a reflection of the passion and dedication of its creators. Built by individuals who’ve spent years honing their skills in competitive dominoes, Capicú delivers an experience that captures the essence of the game’s rich traditions and thrilling dynamics. Here’s why <b>Capicú stands out:</b>\r\n\r\n<b>Authenticity at Its Core:</b> Capicú’s creators have ensured the game stays true to its roots. The rules, gameplay, and interface are designed to mimic the feeling of playing a real-life game of dominoes with friends.\r\n\r\n<b>Fast-Paced Gameplay:</b> Dominoes is a game of both strategy and speed. Capicú reflects this by offering a fast-paced environment that keeps players on their toes while maintaining the integrity of the game.\r\n\r\n<b>Global Community:</b> Capicú brings together players worldwide, fostering a sense of community and competition. Whether you’re looking to test your skills against seasoned players or make new friends, Capicú is the perfect platform for social interaction and competitive fun.\r\n");

        if (btn== ninesInfo)
            UI_Manager.instance.aboutScreen.UpdateUIText("ABOUT US", "Born from the passion of two seasoned domino gamers, Capicú has quickly become the go-to platform for players seeking the authentic thrill of competitive dominoes. Designed by people who live and breathe the game, Capicú brings a fresh, immersive, and fast-paced experience to domino enthusiasts everywhere. Whether you’ve been playing dominoes for years or are just discovering the game, Capicú offers a unique way to enjoy this timeless pastime, combining strategy, camaraderie, and excitement right at your fingertips. With a global community of players, Capicú provides the pure joy of dominoes anytime, anywhere.\r\n\r\nCapicú isn’t just another domino game—it’s a reflection of the passion and dedication of its creators. Built by individuals who’ve spent years honing their skills in competitive dominoes, Capicú delivers an experience that captures the essence of the game’s rich traditions and thrilling dynamics. Here’s why <b>Capicú stands out:</b>\r\n\r\n<b>Authenticity at Its Core:</b> Capicú’s creators have ensured the game stays true to its roots. The rules, gameplay, and interface are designed to mimic the feeling of playing a real-life game of dominoes with friends.\r\n\r\n<b>Fast-Paced Gameplay:</b> Dominoes is a game of both strategy and speed. Capicú reflects this by offering a fast-paced environment that keeps players on their toes while maintaining the integrity of the game.\r\n\r\n<b>Global Community:</b> Capicú brings together players worldwide, fostering a sense of community and competition. Whether you’re looking to test your skills against seasoned players or make new friends, Capicú is the perfect platform for social interaction and competitive fun.\r\n");


    }
}

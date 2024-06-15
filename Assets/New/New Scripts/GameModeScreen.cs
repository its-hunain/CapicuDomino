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
            UI_Manager.instance.aboutScreen.UpdateUIText("ABOUT BLOCKING GAME", "Lorem ipsum dolor sit amet consectetur adipiscing elit odio, mattis quam tortor taciti aenean luctus nullam enim, dui praesent ad dapibus tempus natoque a. Rhoncus praesent massa torquent malesuada maecenas arcu curae, porta pulvinar potenti at mus sem, vel purus proin eleifend nisi dictum. Egestas tortor blandit vestibulum tempus dignissim cras placerat, ligula ridiculus sollicitudin interdum quisque facilisis, suscipit tempor justo tristique et mattis. Nisl imperdiet donec nascetur feugiat massa vehicula elementum nullam purus morbi, sagittis et penatibus taciti vitae lobortis facilisis maecenas gravida, venenatis sed pellentesque suspendisse sociis magna class nibh volutpat. Sodales leo arcu ornare eget torquent dictumst, id morbi fringilla ultricies suscipit, nulla sapien a aliquet tempor. Tristique non eros a felis quam convallis nascetur montes auctor hendrerit, mollis metus sodales ligula magnis condimentum et arcu nam. id morbi fringilla ultricies suscipit, nulla sapien a aliquet tempor. Tristique non eros a felis quam convallis nascetur montes auctor hendrerit, mollis metus sodales ligula magnis condimentum et arcu nam. Lorem ipsum dolor sit amet consectetur adipiscing elit odio, mattis quam tortor taciti aenean luctus nullam enim, dui praesent ad dapibus tempus natoque a. Rhoncus praesent massa torquent malesuada maecenas arcu curae, porta pulvinar potenti at mus sem, vel purus proin eleifend nisi dictum Tristique non eros a felis quam convallis nascetur montes auctor hendrerit, mollis metus sodales ligula magnis condimentum et arcu nam. Lorem ipsum dolnam");

        if (btn== capicuInfo) 
            UI_Manager.instance.aboutScreen.UpdateUIText("ABOUT CAPICU GAME", "Lorem ipsum dolor sit amet consectetur adipiscing elit odio, mattis quam tortor taciti aenean luctus nullam enim, dui praesent ad dapibus tempus natoque a. Rhoncus praesent massa torquent malesuada maecenas arcu curae, porta pulvinar potenti at mus sem, vel purus proin eleifend nisi dictum. Egestas tortor blandit vestibulum tempus dignissim cras placerat, ligula ridiculus sollicitudin interdum quisque facilisis, suscipit tempor justo tristique et mattis. Nisl imperdiet donec nascetur feugiat massa vehicula elementum nullam purus morbi, sagittis et penatibus taciti vitae lobortis facilisis maecenas gravida, venenatis sed pellentesque suspendisse sociis magna class nibh volutpat. Sodales leo arcu ornare eget torquent dictumst, id morbi fringilla ultricies suscipit, nulla sapien a aliquet tempor. Tristique non eros a felis quam convallis nascetur montes auctor hendrerit, mollis metus sodales ligula magnis condimentum et arcu nam. id morbi fringilla ultricies suscipit, nulla sapien a aliquet tempor. Tristique non eros a felis quam convallis nascetur montes auctor hendrerit, mollis metus sodales ligula magnis condimentum et arcu nam. Lorem ipsum dolor sit amet consectetur adipiscing elit odio, mattis quam tortor taciti aenean luctus nullam enim, dui praesent ad dapibus tempus natoque a. Rhoncus praesent massa torquent malesuada maecenas arcu curae, porta pulvinar potenti at mus sem, vel purus proin eleifend nisi dictum Tristique non eros a felis quam convallis nascetur montes auctor hendrerit, mollis metus sodales ligula magnis condimentum et arcu nam. Lorem ipsum dolnam");

        if (btn== ninesInfo) 
            UI_Manager.instance.aboutScreen.UpdateUIText("ABOUT NINES GAME", "Lorem ipsum dolor sit amet consectetur adipiscing elit odio, mattis quam tortor taciti aenean luctus nullam enim, dui praesent ad dapibus tempus natoque a. Rhoncus praesent massa torquent malesuada maecenas arcu curae, porta pulvinar potenti at mus sem, vel purus proin eleifend nisi dictum. Egestas tortor blandit vestibulum tempus dignissim cras placerat, ligula ridiculus sollicitudin interdum quisque facilisis, suscipit tempor justo tristique et mattis. Nisl imperdiet donec nascetur feugiat massa vehicula elementum nullam purus morbi, sagittis et penatibus taciti vitae lobortis facilisis maecenas gravida, venenatis sed pellentesque suspendisse sociis magna class nibh volutpat. Sodales leo arcu ornare eget torquent dictumst, id morbi fringilla ultricies suscipit, nulla sapien a aliquet tempor. Tristique non eros a felis quam convallis nascetur montes auctor hendrerit, mollis metus sodales ligula magnis condimentum et arcu nam. id morbi fringilla ultricies suscipit, nulla sapien a aliquet tempor. Tristique non eros a felis quam convallis nascetur montes auctor hendrerit, mollis metus sodales ligula magnis condimentum et arcu nam. Lorem ipsum dolor sit amet consectetur adipiscing elit odio, mattis quam tortor taciti aenean luctus nullam enim, dui praesent ad dapibus tempus natoque a. Rhoncus praesent massa torquent malesuada maecenas arcu curae, porta pulvinar potenti at mus sem, vel purus proin eleifend nisi dictum Tristique non eros a felis quam convallis nascetur montes auctor hendrerit, mollis metus sodales ligula magnis condimentum et arcu nam. Lorem ipsum dolnam");


    }
}

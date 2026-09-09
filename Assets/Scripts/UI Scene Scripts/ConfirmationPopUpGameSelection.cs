using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConfirmationPopUpGameSelection : MonoBehaviour
{
    [Header("PopUp Items")]
    public Button noBtn;
    public Button yesBtn;
    public Button closeBtn;

    [Header("PopUp Items")]
    public GameObject coinsToPlayRow;

    public Text coinsToPlay;
    public Text noOfPlayers;
    public Text selectedGameRule;
    Animation anim;
    AnimationClip animClip;

    private void Awake()
    {
        anim        = GetComponent<Animation>();
        animClip    = anim.GetClip("ConfirmationPopUpGameSelection");
    }

    public void OnEnable()
    {
        anim[animClip.name].speed = 1;
        anim.Play(animClip.name);

        SetData();
    }

    public void OnReset()
    {
        anim[animClip.name].speed = -1;
        anim[animClip.name].time = anim[animClip.name].length;
        anim.Play(animClip.name);
    }

    void SetData()
    {
        coinsToPlayRow.gameObject.SetActive(GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Multiplayer);
        coinsToPlay.text = GameRulesManager.currentSelectedGame_CoinsToPlay.ToString();
        noOfPlayers.text = GameRulesManager.noOfPlayers.ToString() + " Players";
        selectedGameRule.text = GameRulesManager.currentSelectedGame_Rule.ToString();
    }

    void Start()
    {
        noBtn.onClick.AddListener(      ()=> backButtonDelegate());
        closeBtn.onClick.AddListener(   ()=> backButtonDelegate());
        yesBtn.onClick.AddListener(     ()=> MoveToNextScreen());
    }

    private void MoveToNextScreen()
    {
        SoundManager.instance.ButtonPressPlayer(true);

        OnReset();
        StartCoroutine(UI_ScreenManager._ChangeScreen(UI_ScreenManager.instance.uiScene_LoadingScreen.gameObject, gameObject, true));
    }

    private void backButtonDelegate()
    {
        SoundManager.instance.ButtonPressPlayer(true);

        OnReset();
        GameObject nextScreen = GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Multiplayer ? UI_ScreenManager.instance.selectCoinsToPlayScreen.gameObject : UI_ScreenManager.instance.no_Of_Player_SelectionScreen.gameObject;
        StartCoroutine(UI_ScreenManager._ChangeScreen(nextScreen, gameObject, false));
    }
}
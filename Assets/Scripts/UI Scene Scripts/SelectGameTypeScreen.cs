using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectGameTypeScreen : MonoBehaviour
{
    [Header("PopUp Items")]
    public Button backBtn;
    public Button miltiplayerBtn;
    public Button practiceModeBtn;

    Animation anim;
    AnimationClip animClip;

    private void Awake()
    {
        anim = GetComponent<Animation>();
        animClip = anim.GetClip("SelectGameTypeScreen");
    }
    public void OnEnable()
    {
        anim[animClip.name].speed = 1;
        //anim[animClip.name].time = anim[animClip.name].length;
        anim.Play(animClip.name);
        ChangeSelectable();
    }

    public void OnReset()
    {
        anim[animClip.name].speed = -2;
        anim[animClip.name].time = anim[animClip.name].length;
        anim.Play(animClip.name);
    }

    void Start()
    {
        backBtn.onClick.AddListener(() => backButtonDelegate());
        practiceModeBtn.onClick.AddListener(() => MoveToNextScreen(practiceModeBtn));
        miltiplayerBtn.onClick.AddListener(() => MoveToNextScreen(miltiplayerBtn));
    }

    private void MoveToNextScreen(Button selectedBtn)
    {
        SoundManager.instance.SelectModePlayer(true);
        OnReset();
        ChangeSelectable(selectedBtn);
        if (selectedBtn == practiceModeBtn) GameRulesManager.currentSelectedGame_MatchType = GameRulesManager.MatchType.Bot;
        else GameRulesManager.currentSelectedGame_MatchType = GameRulesManager.MatchType.Multiplayer;
        StartCoroutine(UI_ScreenManager._ChangeScreen(UI_ScreenManager.instance.no_Of_Player_SelectionScreen.gameObject, gameObject, true));
    }

    private void ChangeSelectable(Button selectedBtn = null)
    {
        miltiplayerBtn.transform.GetChild(3).gameObject.SetActive(false);
        practiceModeBtn.transform.GetChild(3).gameObject.SetActive(false);

        if(selectedBtn!=null) selectedBtn.transform.GetChild(3).gameObject.SetActive(true);
    }

    private void backButtonDelegate()
    {
        SoundManager.instance.ButtonPressPlayer(true);
        OnReset();
        ChangeSelectable();
        StartCoroutine(UI_ScreenManager._ChangeGlobeScreen(UI_ScreenManager.instance.ruleSelectionScreen.gameObject, gameObject, false));
    }
}
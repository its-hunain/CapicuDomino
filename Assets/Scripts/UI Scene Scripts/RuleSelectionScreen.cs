using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RuleSelectionScreen : MonoBehaviour
{
    public Button disablePopUpsCommonBtn;
    public RuleSelectionButton[] ruleSelectionButtons;

    [Header("PopUp Items")]
    public Button backBtn;

    Animation anim;
    AnimationClip animClip;

    private void Awake()
    {
        anim        = GetComponent<Animation>();
        animClip    = anim.GetClip("RuleSelectionScreen");
    }
    public void OnEnable()
    {
        Debug.Log("GameCenterSelectionScreen.selectedGameCenterID: " + GameCenterSelectionScreen.selectedGameCenterID);
        anim[animClip.name].speed = 1;
        anim.Play(animClip.name);
        Invoke("EnableBG", anim.clip.length);
        ChangeSelectable();
        ResetAllButtonState();

        if(GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Multiplayer)
            LoadGameCenterData();
        
    }

    private void LoadGameCenterData()
    {
        GameCenterDetails gameCenterDetails = UI_ScreenManager.instance.gameCenterSelectionScreen.gameCenters.data.Find(x => x._id == GameCenterSelectionScreen.selectedGameCenterID);
        foreach (var item in ruleSelectionButtons)
        {
            //Debug.Log("+ item: " + item);
            if (gameCenterDetails.gameCenterGames.Find(RuleDetail => RuleDetail.Rules[0].gameRulesName.Equals(item.gameRule)) == null)
            {
                //Debug.Log("+ + item: " + item);
                item.ChangeRuleButtonState(false);
            }
        }
    }

    void EnableBG()
    {
        UI_ScreenManager.instance.uiScene_Background.SetActive(true);
    }

    public void OnReset()
    {
        Debug.Log("OnReset" , gameObject);
        anim[animClip.name].speed = -2;
        anim[animClip.name].time = anim[animClip.name].length;
        anim.Play(animClip.name);
    }

    void Start()
    {
        backBtn.onClick.AddListener(()  => backButtonDelegate());
        disablePopUpsCommonBtn.onClick.AddListener(() => ChangeSelectable());

        foreach (var item in ruleSelectionButtons)
            item.thisBtn.onClick.AddListener(() => MoveToNextScreen(item, item.gameRule));
    }

    private void MoveToNextScreen(RuleSelectionButton selectedBtn, GameRulesManager.GameRules selectedGameRule)
    {
        SoundManager.instance.ButtonPressPlayer(true);
        GameRulesManager.currentSelectedGame_Rule = selectedGameRule;
        ChangeSelectable(selectedBtn);
    }

    private void ChangeSelectable(RuleSelectionButton selectedBtn = null)
    {
        foreach (var item in ruleSelectionButtons)
            item.ChangeSelectable(false);

        if (selectedBtn != null)
            selectedBtn.ChangeSelectable(true);
    }


    private void ResetAllButtonState()
    {
        foreach (var item in ruleSelectionButtons)
            item.ChangeRuleButtonState(true);
    }

    private void backButtonDelegate()
    {
        OnReset();
        GameObject screen = GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot ? UI_ScreenManager.instance.playGameScreen.gameObject : UI_ScreenManager.instance.gameCenterSelectionScreen.gameObject;
        screen.SetActive(true);
        this.gameObject.SetActive(false);
    }
}

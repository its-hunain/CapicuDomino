using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class No_Of_Player_SelectionScreen : MonoBehaviour
{
    [Header("PopUp Items")]
    public Button backBtn;
    public Button twoPlayerBtn;
    public Button threePlayerBtn;
    public Button fourPlayerBtn;

    Animation anim;
    AnimationClip animClip;

    private void Awake()
    {
        anim = GetComponent<Animation>();
        animClip = anim.GetClip("No_Of_Player_SelectionScreen");
    }
    public void OnEnable()
    {
        anim[animClip.name].speed = 1;
        //anim[animClip.name].time = anim[animClip.name].length;
        anim.Play(animClip.name);
        ChangeSelectable();
        ResetAllButtonState();

        if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Multiplayer)
            LoadGameCenterData();

    }

    private void LoadGameCenterData()
    {
        GameCenterDetails gameCenterDetail = UI_ScreenManager.instance.gameCenterSelectionScreen.gameCenters.data.Find(x => x._id == GameCenterSelectionScreen.selectedGameCenterID);
        RuleDetails ruleDetail = gameCenterDetail.gameCenterGames.Find(ruleDetail => ruleDetail.Rules[0].gameRulesName == GameRulesManager.currentSelectedGame_Rule);
        UpdateButtonState(ruleDetail);
    }

    public void OnReset()
    {
        anim[animClip.name].speed = -1;
        anim[animClip.name].time = anim[animClip.name].length;
        anim.Play(animClip.name);
    }

    void Start()
    {
        backBtn.onClick.AddListener(() => backButtonDelegate());
        twoPlayerBtn.onClick.AddListener(() => MoveToNextScreen(twoPlayerBtn, 2));
        threePlayerBtn.onClick.AddListener(() => MoveToNextScreen(threePlayerBtn, 3));
        fourPlayerBtn.onClick.AddListener(() => MoveToNextScreen(fourPlayerBtn, 4));
    }

    private void MoveToNextScreen(Button selectedBtn, int noOfPlayers)
    {
        SoundManager.instance.SelectModePlayer(true);
        GameRulesManager.noOfPlayers = noOfPlayers;
        OnReset();
        ChangeSelectable(selectedBtn);
        GameObject nextScreen = GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Multiplayer ? UI_ScreenManager.instance.selectCoinsToPlayScreen.gameObject : UI_ScreenManager.instance.confirmationPopUpGameSelection.gameObject;
        StartCoroutine(UI_ScreenManager._ChangeScreen(nextScreen, gameObject, true));
    }

    private void ChangeSelectable(Button selectedBtn = null)
    {
        twoPlayerBtn.transform.GetChild(0).gameObject.SetActive(false);
        threePlayerBtn.transform.GetChild(0).gameObject.SetActive(false);
        fourPlayerBtn.transform.GetChild(0).gameObject.SetActive(false);

        if(selectedBtn!=null) selectedBtn.transform.GetChild(0).gameObject.SetActive(true);
    }

    private void backButtonDelegate()
    {
        SoundManager.instance.ButtonPressPlayer(true);
        OnReset();
        ChangeSelectable();
        StartCoroutine(UI_ScreenManager._ChangeGlobeScreen(UI_ScreenManager.instance.ruleSelectionScreen.gameObject, gameObject, false));
    }


    private void ResetAllButtonState()
    {
        //enable disable hover script
        //enable disable button interactable
        twoPlayerBtn.GetComponent<HoverGreen>().enabled     =   twoPlayerBtn.interactable   = true;
        threePlayerBtn.GetComponent<HoverGreen>().enabled   =   threePlayerBtn.interactable = true;
        fourPlayerBtn.GetComponent<HoverGreen>().enabled    =   fourPlayerBtn.interactable  = true;

        Image image = twoPlayerBtn.transform.GetChild(1).GetComponent<Image>();
        Color color = image.color; color.a = 1; image.color = color;

        image = threePlayerBtn.transform.GetChild(1).GetComponent<Image>();
        color = image.color; color.a = 1; image.color = color;

        image = fourPlayerBtn.transform.GetChild(1).GetComponent<Image>();
        color = image.color; color.a = 1; image.color = color;

    }


    private void UpdateButtonState(RuleDetails ruleDetail)
    {
        //enable disable hover script
        //enable disable button interactable
        twoPlayerBtn.GetComponent<HoverGreen>().enabled     =   twoPlayerBtn.interactable       =     ruleDetail.Rules[0].noOfPlayers.Contains(2);
        threePlayerBtn.GetComponent<HoverGreen>().enabled   =   threePlayerBtn.interactable     =     ruleDetail.Rules[0].noOfPlayers.Contains(3);
        fourPlayerBtn.GetComponent<HoverGreen>().enabled    =   fourPlayerBtn.interactable      =     ruleDetail.Rules[0].noOfPlayers.Contains(4);


        Image image = twoPlayerBtn.transform.GetChild(1).GetComponent<Image>();
        Color color = image.color;
        color.a = twoPlayerBtn.interactable == false ? .5f : 1;
        image.color = color;

        image = threePlayerBtn.transform.GetChild(1).GetComponent<Image>();
        color = image.color;
        color.a = threePlayerBtn.interactable == false ? .5f : 1;
        image.color = color;

        image = fourPlayerBtn.transform.GetChild(1).GetComponent<Image>();
        color = image.color;
        color.a = fourPlayerBtn.interactable == false ? .5f : 1;
        image.color = color;
    }

}

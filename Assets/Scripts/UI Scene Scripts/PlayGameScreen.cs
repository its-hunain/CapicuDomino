using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayGameScreen : MonoBehaviour
{
    public Button playGameBtn;
    public Button practiceGameBtn;
    public Button settingBtn;

    Animation anim;
    AnimationClip animClip;

    private void Awake()
    {
        anim = GetComponent<Animation>();
        animClip = anim.GetClip("PlayGameScreen");
    }
    public void OnEnable()
    {
        AudioListener.volume = 1;
        anim[animClip.name].speed = 1;
        anim.Play(animClip.name);
    }


    public void OnReset()
    {
        Debug.Log("OnReset", gameObject);
        anim[animClip.name].speed = -2;
        anim[animClip.name].time = anim[animClip.name].length;
        anim.Play(animClip.name);
    }


    private IEnumerator Start()
    {
        playGameBtn.onClick.AddListener(() => PlayGameButtonDelegate(GameRulesManager.MatchType.Multiplayer));
        practiceGameBtn.onClick.AddListener(() => PlayGameButtonDelegate(GameRulesManager.MatchType.Bot));
        settingBtn.onClick.AddListener(() => SettingButtonDelegate());

        yield return new WaitUntil(() => !string.IsNullOrEmpty(Global.GetBearerToken) && !string.IsNullOrEmpty(WebServiceManager.baseURL));
        WebServiceManager.instance.APIRequest(WebServiceManager.instance.getGameCenter, Method.GET, null, null, UI_ScreenManager.instance.gameCenterSelectionScreen.OnSuccessfullyDataDownloaded, OnFail, CACHEABLE.NULL, true, null);
    }


    private void OnFail(string reason)
    {
        Debug.LogError("Fail To GetGameCenter: " + reason);
    }

    private void SettingButtonDelegate()
    {
        SoundManager.instance.ButtonPressPlayer(true);
        UI_ScreenManager.OpenClosePopUp(UI_ScreenManager.instance.settingPopUp.gameObject, true, true);
    }

    private void PlayGameButtonDelegate(GameRulesManager.MatchType matchType)
    {
        GameRulesManager.currentSelectedGame_MatchType = matchType;

        SoundManager.instance.ButtonPressPlayer(true);

        if (matchType == GameRulesManager.MatchType.Multiplayer)
        {
            if (PlayerPersonalData.playerDomiCoins <= 0)
            {
                UI_ScreenManager.instance.errorPopUpScreen.OpenCloseWarning(true, "Not enough DomiCoins, please buy to proceed");
                return;
            }

            //#if !UNITY_EDITOR
            //            if (!UI_ScreenManager.CheckAllNFTsExist())
            //            {
            //                UI_ScreenManager.instance.errorPopUpScreen.OpenCloseWarning(true, "Your default AVATAR has been reset, please set it to proceed");
            //                return;
            //            }
            //#endif
            //        }
        }
        Invoke("SimpleScreenChange", anim[animClip.name].time/2);
        OnReset();
    }
    public void SimpleScreenChange()
    {
        GameObject screen = (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot) ? UI_ScreenManager.instance.ruleSelectionScreen.gameObject : UI_ScreenManager.instance.  gameCenterSelectionScreen.gameObject;
        screen.SetActive(true);
        gameObject.SetActive(false);
    }
}

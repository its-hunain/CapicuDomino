using System;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_ScreenManager : MonoBehaviour
{

    [Header("Black Image")]
    public GameObject showBlackScreen;

    public PlayGameScreen playGameScreen;
    public SettingScreen settingPopUp;
    public RuleSelectionScreen ruleSelectionScreen;
    public SelectGameTypeScreen selectGameTypeScreen;
    public No_Of_Player_SelectionScreen no_Of_Player_SelectionScreen;
    public SelectCoinsToPlayScreen selectCoinsToPlayScreen;
    public ConfirmationPopUpGameSelection confirmationPopUpGameSelection;
    public GameCenterSelectionScreen gameCenterSelectionScreen;
    public UIScene_LoadingScreen uiScene_LoadingScreen;
    public ErrorPopUpScreen errorPopUpScreen;

    public GameObject uiScene_Background;

    public static UI_ScreenManager instance;


    public GameObject userNotFoundPopUpScreen;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == Global.SplashScene) return;

        if (SoundManager.instance != null) SoundManager.instance.MenuBGPlayer(true);


        //Hitting again when scene reload after finish one game.
        WebServiceManager.instance.APIRequest(WebServiceManager.instance.getPlayerProfile, Method.GET, null, null, PlayerPersonalData.OnSuccessfullyProfileDownload, PlayerPersonalData.OnFailDownload, CACHEABLE.NULL, true, null);
    }

      public static void OpenClosePopUp(GameObject panel, bool doOpen, bool showBlackBG)
    {
        //Debug.Log("OpenClosePopUp" + panel,panel);
        if (doOpen)
        {
            if (!panel.activeInHierarchy)
            {
                panel.transform.localScale = Vector3.zero;
                panel.SetActive(true);
                instance.showBlackScreen.SetActive(showBlackBG);
                LeanTween.scale(panel, Vector3.one, .5f).setEaseOutBack();
            }
        }
        else
        {
            if(panel.activeInHierarchy) LeanTween.scale(panel, Vector3.zero, .5f).setEaseInBack().setOnComplete(() => OnComplete(panel, showBlackBG));
        }
    }

    static void OnComplete(GameObject panel, bool showBlackBG)
    {
        panel.SetActive(false);
        instance.showBlackScreen.SetActive(showBlackBG);
    }

    public static IEnumerator _ChangeScreen(GameObject nextScreen,GameObject currentScreen, bool isPush)
    {
        Vector3 offset  = new Vector3(2000, 0, 0);
        Vector3 reset   = Vector3.zero;

        yield return new WaitForSeconds(isPush? 0.1f : 0);
        Vector3 moveValue = isPush ? offset * -1 : offset;

        nextScreen.SetActive(true);
        LeanTween.moveLocal(currentScreen, moveValue, 0.5f).setEaseInSine().setOnComplete(()=> currentScreen.SetActive(false));
        LeanTween.moveLocal(nextScreen, reset, 0.5f).setEaseOutSine();
    }


    public static IEnumerator _ChangeGlobeScreen(GameObject nextScreen,GameObject currentScreen, bool isPush)
    {
        Vector3 offset = Vector3.one;
        Vector3 reset   = Vector3.zero;

        yield return new WaitForSeconds(isPush? 0.1f : 0);
        //Vector3 moveValue = isPush ? offset * -1 : offset;

        nextScreen.SetActive(true);
        //yield return new WaitForSeconds(0.5f);

        LeanTween.scale(currentScreen, reset, 0.5f).setEaseInSine().setOnComplete(()=> currentScreen.SetActive(false));
        LeanTween.scale(nextScreen, offset, .1f).setEaseOutSine();

    }


}
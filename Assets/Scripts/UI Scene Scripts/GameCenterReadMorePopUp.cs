using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCenterReadMorePopUp : MonoBehaviour
{
    public Button disablePopUpBtn;

    [Header("PopUp Items")]
    public string gameCenterID;
    public Text ruleHeading;
    public Text ruleDescription;
    public Button playNowButton;

    void Start()
    {
        disablePopUpBtn.onClick.AddListener(() => ShowHideReadMorePopup(false));
        playNowButton.onClick.AddListener(() => MoveToNextScreen());
    }

    private void MoveToNextScreen()
    {
        GameCenterSelectionScreen.selectedGameCenterID = gameCenterID;
        SoundManager.instance.ButtonPressPlayer(true);
        Invoke("Delay", .2f);
    }

    private void Delay()
    {
        UI_ScreenManager.OpenClosePopUp(this.gameObject, false, false);

        //UI_ScreenManager.instance.StartCoroutine(UI_ScreenManager._ChangeScreen(UI_ScreenManager.instance.ruleSelectionScreen.gameObject, UI_ScreenManager.instance.gameCenterSelectionScreen.gameObject, true));

        UI_ScreenManager.instance.ruleSelectionScreen.gameObject.SetActive(true);
        UI_ScreenManager.instance.gameCenterSelectionScreen.gameObject.SetActive(false);

    }

    public void ShowHideReadMorePopup(bool state)
    {
        SoundManager.instance.ButtonPressPlayer(true);
        UI_ScreenManager.OpenClosePopUp(gameObject, state, state);
    }
}

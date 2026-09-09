using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCenter_ItemRow : MonoBehaviour
{
    public string gameCenterID;
    public string gameDescription;
    public Image gameCenterImage;
    public Text gameCenterName;
    public Text gameCenterDescription;

    public Button readMoreBtn;
    public Button playNowBtn;
    
    // Start is called before the first frame update
    void Start()
    {
        readMoreBtn.onClick.AddListener(()=> ReadMoreBtnDelegate());
        playNowBtn.onClick.AddListener(()=> PlayBtnDelegate());
    }

    private void ReadMoreBtnDelegate()
    {
        GameCenterReadMorePopUp gameCenterReadMorePopUp = GetComponentInParent<GameCenterSelectionScreen>().gameCenterReadMorePopUp;
        gameCenterReadMorePopUp.gameCenterID = gameCenterID;
        gameCenterReadMorePopUp.ruleDescription.text = gameDescription;
        gameCenterReadMorePopUp.ruleHeading.text = gameCenterName.text; 
        gameCenterReadMorePopUp.ShowHideReadMorePopup(true);
    }

    private void PlayBtnDelegate()
    {
        GameCenterSelectionScreen.selectedGameCenterID = gameCenterID;
        SoundManager.instance.ButtonPressPlayer(true);
        UI_ScreenManager.instance.gameCenterSelectionScreen.gameObject.SetActive(false);
        UI_ScreenManager.instance.ruleSelectionScreen.gameObject.SetActive(true);

        //Invoke("Delay", .6f);
    }

    private void Delay()
    {

        //UI_ScreenManager.instance.StartCoroutine(UI_ScreenManager._ChangeScreen(UI_ScreenManager.instance.ruleSelectionScreen.gameObject, UI_ScreenManager.instance.gameCenterSelectionScreen.gameObject, true));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

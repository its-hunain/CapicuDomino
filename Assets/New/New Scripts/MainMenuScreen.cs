using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour
{
    public Button profileBtn;
    public Button shopBtn;
    public Button coinsBtn;
    public Button settingsBtn;


    public Button botGameBtn;
    public Button randomGameBtn;
    public Button friendsGameBtn;

    public Button partnerGameBtn;
    public Button leaderBoardBtn;

    void Start()
    {

        //  shopBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance..gameObject, true));
        profileBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.profileScreen.gameObject, true));
        coinsBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.coinScreen.gameObject, true));
        shopBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.shopScreen.gameObject, true));
        settingsBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.settingScreen.gameObject, true));

        botGameBtn.onClick.AddListener(()     => UI_Manager.instance.ChangeScreen(UI_Manager.instance.gameModeScreen.gameObject, true));
        randomGameBtn.onClick.AddListener(()  => UI_Manager.instance.ChangeScreen(UI_Manager.instance.gameModeScreen.gameObject, true));
        friendsGameBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.gameModeScreen.gameObject, true));


        partnerGameBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.partnerScreen.gameObject, true));
      //  leaderBoardBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance..gameObject, true));

    }

}

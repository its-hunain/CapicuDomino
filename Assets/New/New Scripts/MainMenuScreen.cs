using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour
{
    public Button profileBtn;
    public Text name;
    public Text country;

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
        UpdateUI();

        //  shopBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance..gameObject, true));
        profileBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.profileScreen.gameObject, true));
        coinsBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.coinScreen.gameObject, true));
        shopBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.shopScreen.gameObject, true));
        settingsBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.settingScreen.gameObject, true));

        botGameBtn.onClick.AddListener(()     => GameModeSelection(GameRulesManager.MatchType.Bot));
        randomGameBtn.onClick.AddListener(()  => GameModeSelection(GameRulesManager.MatchType.Multiplayer));
        friendsGameBtn.onClick.AddListener(() => GameModeSelection(GameRulesManager.MatchType.Multiplayer));


        partnerGameBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.partnerScreen.gameObject, true));
      //  leaderBoardBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance..gameObject, true));

    }
    public void UpdateUI()
    {
        name.text = PlayerPersonalData.playerName;
        //country.text = UI_Manager.instance.userCountry;

    }

    public void GameModeSelection(GameRulesManager.MatchType matchType)
    {
        GameRulesManager.currentSelectedGame_MatchType = matchType;
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.gameModeScreen.gameObject, true);
    }
}

using Dominos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : MonoBehaviour //new
{
    public static UI_Manager instance;
    public SplashScreen splashScreen;
    public LoginScreen loginScreen;
    public MainMenuScreen menuScreen;
    public ProfilePanelScreen profileScreen;
    public EditProfileScreen editProfileScreen;
   // public GameObject coinScreen;
    public ShopScreen shopScreen;
    public SettingsScreen settingScreen;
    public AboutScreen aboutScreen;
    public CoinsScreen coinsScreen;
    public GameModeScreen gameModeScreen;
    public SelectCoinsToPlayScreen selectCoinsToPlayScreen;
    public NoOfPlayersScreen noOfPlayers;
    public CreateJoinRoomButtonPanel createJoinRoomButtonPanel;
    public InvitePlayersScreen invitePlayersScreen;
    public JoinRoomScreen joinRoomScreen;
    public PartnerScreen partnerScreen;
    public ScoreToWinScreen scoreToWinScreen;
    public LeaderBoardScript leaderBoardScreen;
    public ErrorPopUpScreen errorPopUpScreen;
    public GameObject purchaseSuccessPanel;
    public GameObject purchaseFailPanel;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        // StartCoroutine(UpdateUI());

        if (SoundManager.instance != null) SoundManager.instance.MenuBGPlayer(true);

    }


    public void UpdateUI()
    {
        Debug.Log("UpdateUI");
        //yield return new WaitForSeconds(2f);
        menuScreen.UpdateUI();
        profileScreen.UpdateUI();
        editProfileScreen.UpdateUI();
    }
    //public void SaveUserData(string name, string country, string age, string gender)
    //{
    //    PlayerPrefs.SetString("Name", name);
    //    PlayerPrefs.SetString("Country", country);
    //    PlayerPrefs.SetString("Age", age);
    //    PlayerPrefs.SetString("Gender", gender);
    //    UI_Manager.instance.profileScreen.UpdateUI();
    //    UI_Manager.instance.menuScreen.UpdateUI();
    //}
    public void ChangeScreen(GameObject obj,bool on)
    {
        if (SoundManager.instance != null) SoundManager.instance.PlacetileAudioPlayer(true);
        obj.SetActive(on);
    }


}

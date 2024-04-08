using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;
    public LoginScreen loginScreen;
    public MainMenuScreen menuScreen;
    public ProfilePanelScreen profileScreen;
    public EditProfileScreen editProfileScreen;
    public GameObject coinScreen;
    public ShopScreen shopScreen;
    public SettingsScreen settingScreen;
    public AboutScreen aboutScreen;
    public GameModeScreen gameModeScreen;
    public CoinsScreen coinsScreen;
    public NoOfPlayersScreen noOfPlayers;
    public FriendsPlayScreen friendsPlayScreen;
    public InvitePlayersScreen invitePlayersScreen;
    public PartnerScreen partnerScreen;
    public ScoreToWinScreen scoreToWinScreen;



    // Start is called before the first frame update
    void Start()
    {
        instance = this;
      // StartCoroutine(UpdateUI());
    }

    public IEnumerator UpdateUI()
    {
        Debug.LogError("UpdateUI");
        yield return new WaitForSeconds(2f);
        menuScreen.UpdateUI();
        profileScreen.UpdateUI();
        editProfileScreen.UpdateUI();
    }
    public void SaveUserData(string name, string country, string age, string gender)
    {
        PlayerPrefs.SetString("Name", name);
        PlayerPrefs.SetString("Country", country);
        PlayerPrefs.SetString("Age", age);
        PlayerPrefs.SetString("Gender", gender);
        UI_Manager.instance.profileScreen.UpdateUI();
        UI_Manager.instance.menuScreen.UpdateUI();
    }
    public void ChangeScreen(GameObject obj,bool on)
    {
        obj.SetActive(on);
    }
}

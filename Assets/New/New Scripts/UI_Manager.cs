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


    public string userName = null;
    public string userCountry;
    public string userAge;
    public string userGender;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        GetUserDataFromSave();
    }
    public void GetUserDataFromSave()
    {
        userName = PlayerPrefs.GetString("Name");
        userCountry = PlayerPrefs.GetString("Country");
        userAge = PlayerPrefs.GetString("Age");
        userGender = PlayerPrefs.GetString("Gender");
    }
    public void UpdateUserData(string name, string country, string age, string gender)
    {
        name = userName;
        country = userCountry;
        age = userAge;
        gender = userGender;
    }
    public void SaveUserData(string name, string country, string age, string gender)
    {
        PlayerPrefs.SetString("Name", name);
        PlayerPrefs.SetString("Country", country);
        PlayerPrefs.SetString("Age", age);
        PlayerPrefs.SetString("Gender", gender);
        GetUserDataFromSave();
    }
    public void ChangeScreen(GameObject obj,bool on)
    {
        obj.SetActive(on);
    }
}

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


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

 
    public void ChangeScreen(GameObject obj,bool on)
    {
        obj.SetActive(on);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfilePanelScreen : MonoBehaviour
{
    public Image profileImage;
    public Image flag;

    public Text name;
    public Text country;
    public Text email;
    public Text age;
    public Text gender;

    public Text totalWinning;
    public Text totalPlay;
    public Text totalCoins;

    public Button editProfileBtn;
    public Button fbConnectBtn;
    public Button appleConnectBtn;
    public Button closeBtn;

    void Start()
    {
        UpdateUI();

        editProfileBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.editProfileScreen.gameObject, true));
     //   fbConnectBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance..gameObject, true));
     //   appleConnectBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance..gameObject, true));


        closeBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.profileScreen.gameObject, false));
    }
    public void UpdateUI()
    {
        name.text = PlayerPersonalData.playerName;
        var temp = Sprite.Create(PlayerPersonalData.playerTexture,new Rect(0.0f, 0.0f, PlayerPersonalData.playerTexture.width, PlayerPersonalData.playerTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
        profileImage.sprite = temp;

        country.text = PlayerPersonalData.country;
        email.text = PlayerPersonalData.playerEmail;
        age.text = PlayerPersonalData.age.ToString();
        gender.text = PlayerPersonalData.gender;


        totalWinning.text = PlayerPersonalData.playerStates.gamesWon;
        totalPlay.text = PlayerPersonalData.playerStates.gamesPlayed;
        totalCoins.text = PlayerPersonalData.playerDomiCoins.ToString();

        flag.sprite = PlayerPersonalData.playerStates.flagSprite;
    }
}

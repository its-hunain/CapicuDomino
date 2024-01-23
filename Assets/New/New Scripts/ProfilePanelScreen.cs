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
        editProfileBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.editProfileScreen.gameObject, true));
     //   fbConnectBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance..gameObject, true));
     //   appleConnectBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance..gameObject, true));


        closeBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.profileScreen.gameObject, false));
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginScreen : MonoBehaviour
{
    public Button fbLoginBtn;
    public Button appleLoginBtn;
    public Button guestLoginBtn;
    public Button googleLoginBtn;
    public GameObject mainmenu;

    private void Awake()
    {
        if (PlayerPrefs.HasKey(Global.AuthProvider))
        {
            gameObject.SetActive(false);
            mainmenu.SetActive(true);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            PlayerPersonalData.Player_OS = "Android";
            appleLoginBtn.gameObject.SetActive(false);
            if (googleLoginBtn != null) googleLoginBtn.gameObject.SetActive(true);
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            PlayerPersonalData.Player_OS = "iOS";
            appleLoginBtn.gameObject.SetActive(true);
            if (googleLoginBtn != null) googleLoginBtn.gameObject.SetActive(false);
        }
        else
        {
            PlayerPersonalData.Player_OS = "Editor";
            appleLoginBtn.gameObject.SetActive(false);
            if (googleLoginBtn != null) googleLoginBtn.gameObject.SetActive(true);
        }

        //fbLoginBtn.onClick.AddListener(()    =>UI_Manager.instance.ChangeScreen(UI_Manager.instance.menuScreen.gameObject,true));
        //appleLoginBtn.onClick.AddListener(() =>UI_Manager.instance.ChangeScreen(UI_Manager.instance.menuScreen.gameObject,true));
        //   guestLoginBtn.onClick.AddListener(() =>UI_Manager.instance.ChangeScreen(UI_Manager.instance.menuScreen.gameObject,true));
    }

   void LoginWithFB()
    {
        //Logic for FB Login
    }
    void LoginWithApple()
    {
        //Logic for Apple Login
    }
    void GuestLogin()
    {
        //Logic for Guest Login
    }

}

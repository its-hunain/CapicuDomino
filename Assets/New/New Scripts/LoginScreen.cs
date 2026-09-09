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
        ConfigureLoginButtons();

        //fbLoginBtn.onClick.AddListener(()    =>UI_Manager.instance.ChangeScreen(UI_Manager.instance.menuScreen.gameObject,true));
        //appleLoginBtn.onClick.AddListener(() =>UI_Manager.instance.ChangeScreen(UI_Manager.instance.menuScreen.gameObject,true));
        //   guestLoginBtn.onClick.AddListener(() =>UI_Manager.instance.ChangeScreen(UI_Manager.instance.menuScreen.gameObject,true));
    }

    void ConfigureLoginButtons()
    {
#if UNITY_EDITOR
#if UNITY_EDITOR_OSX
        // Apple Editor - Show Apple login, hide Google login
        PlayerPersonalData.Player_OS = "Editor_Mac";
        if (appleLoginBtn != null) appleLoginBtn.gameObject.SetActive(true);
        if (googleLoginBtn != null) googleLoginBtn.gameObject.SetActive(false);
#else
        // Windows Editor - Show Google login, hide Apple login
        PlayerPersonalData.Player_OS = "Editor_Windows";
        if (appleLoginBtn != null) appleLoginBtn.gameObject.SetActive(false);
        if (googleLoginBtn != null) googleLoginBtn.gameObject.SetActive(true);
#endif
#elif UNITY_IOS
        // iOS Device - Show Apple login, hide Google login
        PlayerPersonalData.Player_OS = "iOS";
        if (appleLoginBtn != null) appleLoginBtn.gameObject.SetActive(true);
        if (googleLoginBtn != null) googleLoginBtn.gameObject.SetActive(false);
#elif UNITY_ANDROID
        // Android Device - Show Google login, hide Apple login
        PlayerPersonalData.Player_OS = "Android";
        if (appleLoginBtn != null) appleLoginBtn.gameObject.SetActive(false);
        if (googleLoginBtn != null) googleLoginBtn.gameObject.SetActive(true);
#else
        // Fallback for other platforms
        PlayerPersonalData.Player_OS = "Other";
        if (appleLoginBtn != null) appleLoginBtn.gameObject.SetActive(false);
        if (googleLoginBtn != null) googleLoginBtn.gameObject.SetActive(true);
#endif
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

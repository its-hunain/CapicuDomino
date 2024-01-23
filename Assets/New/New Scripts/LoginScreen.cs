using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginScreen : MonoBehaviour
{
    public Button fbLoginBtn;
    public Button appleLoginBtn;
    public Button guestLoginBtn;

    // Start is called before the first frame update
    void Start()
    {
       fbLoginBtn.onClick.AddListener(()    =>UI_Manager.instance.ChangeScreen(UI_Manager.instance.menuScreen.gameObject,true));
       appleLoginBtn.onClick.AddListener(() =>UI_Manager.instance.ChangeScreen(UI_Manager.instance.menuScreen.gameObject,true));
       guestLoginBtn.onClick.AddListener(() =>UI_Manager.instance.ChangeScreen(UI_Manager.instance.menuScreen.gameObject,true));
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

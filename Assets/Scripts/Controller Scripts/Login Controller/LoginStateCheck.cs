using UnityEngine;
using UnityEngine.UI;

public class LoginStateCheck : MonoBehaviour
{

    public Button AppleLoginBtn;
    public Button GoogleLoginBtn;

    void Start()
    {
        if(Application.platform == RuntimePlatform.Android)
        {
            PlayerPersonalData.Player_OS = "Android";
            AppleLoginBtn.gameObject.SetActive(false);
            if (GoogleLoginBtn != null) GoogleLoginBtn.gameObject.SetActive(true);
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            PlayerPersonalData.Player_OS = "iOS";
            AppleLoginBtn.gameObject.SetActive(true);
            if (GoogleLoginBtn != null) GoogleLoginBtn.gameObject.SetActive(false);
        }
        else
        {
            PlayerPersonalData.Player_OS = "Editor";
            AppleLoginBtn.gameObject.SetActive(false);
            if (GoogleLoginBtn != null) GoogleLoginBtn.gameObject.SetActive(false);
        }

        //if (PlayerPrefs.HasKey(ConstantVariables.AuthProvider))
        //{
        //    Debug.Log("%%%%%%%%%%%" + gameObject.name);
        //    UIEvents.ShowPanel(Panel.TabPanels);
        //    UIEvents.HidePanel(Panel.SignupPanel);
        //}
    }
}
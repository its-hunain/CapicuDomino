using System.Collections.Generic;
using Dominos;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RegistrationScreen : MonoBehaviour
{
    public InputField usernameInput;
    public InputField passwordInput;
    public InputField emailInput;
    public Button registerButton;

    [Header("Login Assets")]
    public InputField emailInputLogin;
    public InputField passwordInputLogin;
    public Button loginButton;

    private void Start()
    {
        // Attach a listener to the registerButton
        registerButton.onClick.AddListener(()=> signup());
        loginButton.onClick.AddListener(()=> login());

        if (PlayerPrefs.GetString(Global.AuthProvider) == Global.Custom)
        {
            Debug.Log("ByPass Login");
            ByPassLogin();
        }
    }

    void loadDataFromPRefs()
    {

        PlayerPersonalData.playerUserID = PlayerPrefs.GetString(Global.UserID);
        PlayerPersonalData.playerName = PlayerPrefs.GetString(Global.UserName);
        PlayerPersonalData.playerEmail = PlayerPrefs.GetString(Global.UserEmail);
        PlayerPersonalData.playerPassword = PlayerPrefs.GetString(Global.UserPassword);
    }

    private void ByPassLogin()
    {
        loadDataFromPRefs();

        Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();

        string email = PlayerPersonalData.playerEmail;
        string password = PlayerPersonalData.playerPassword;
        keyValuePairs.Add("Email", email);
        keyValuePairs.Add("Password", password);

        WebServiceManager.instance.APIRequest(WebServiceManager.instance.customLoginFunction, Method.POST, null, keyValuePairs, PlayerPersonalData.OnSuccessfullyProfileDownload, PlayerPersonalData.OnFailDownload, CACHEABLE.NULL, true, null);
    }


    private void login()
    {
        //string guestUserID = GuestLoginGenerator.GenerateUniqueUserId();
        //string username = usernameInput.text;
        string password = passwordInputLogin.text.Trim();
        string email = emailInputLogin.text.Trim();



        // Perform basic validation (you should implement more robust validation)
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
        {
            MesgBar.instance.show("All fields are required.", true);
            return;
        }


        // Email Validation
        if (!CheckValidation.EmailValidaton(email))
        {
            MesgBar.instance.show("Invalid Email.", true);
            return;
        }


        PlayerPersonalData.authProvider = Global.Custom;

        // Use the generated email and password for guest login
        if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
        {
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            //keyValuePairs.Add("UserID", guestUserID);
            //keyValuePairs.Add("FullName", username);
            keyValuePairs.Add("Email", email);
            keyValuePairs.Add("Password", password);
            //keyValuePairs.Add("AuthProvider", PlayerProfile.authProvider);

            WebServiceManager.instance.APIRequest(WebServiceManager.instance.customLoginFunction, Method.POST, null, keyValuePairs, PlayerPersonalData.OnSuccessfullyProfileDownload, PlayerPersonalData.OnFailDownload, CACHEABLE.NULL, true, null);
        }
    }

    private void signup()
    {
        //string guestUserID = GuestLoginGenerator.GenerateUniqueUserId();
        string username = usernameInput.text;
        string password = passwordInput.text;
        string email = emailInput.text;



        // Perform basic validation (you should implement more robust validation)
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email)|| string.IsNullOrEmpty(username))
        {
            MesgBar.instance.show("Please fill in all fields.", true);
            return;
        }


        // Email Validation
        if (!CheckValidation.EmailValidaton(email))
        {
            MesgBar.instance.show("Invalid Email.", true);
            return;
        }


        

        PlayerPersonalData.authProvider = Global.Custom;

        // Use the generated email and password for guest login
        if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(username))
        {
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            //keyValuePairs.Add("UserID", guestUserID);
            keyValuePairs.Add("FullName", username);
            keyValuePairs.Add("Email", email);
            keyValuePairs.Add("Password", password);
            keyValuePairs.Add("AuthProvider", PlayerPersonalData.authProvider);
            //keyValuePairs.Add("AuthProvider", PlayerProfile.authProvider);

            WebServiceManager.instance.APIRequest(WebServiceManager.instance.signUpFunction, Method.POST, null, keyValuePairs, PlayerPersonalData.OnSuccessfullyProfileDownload, PlayerPersonalData.OnFailDownload, CACHEABLE.NULL, true, null);
        }
    }


    public void BackFromRegistration()
    {
        //UIEvents.ShowPanel(Panel.SignupPanel);
        //UIEvents.HidePanel(Panel.RegistrationPanel);
        //UIEvents.HidePanel(Panel.CustomLoginPanel);
    }
}
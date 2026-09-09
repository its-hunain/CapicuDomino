using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_ANDROID
using Google;
#endif

public class GoogleManager : MonoBehaviour
{
    public Button GoogleButton;

#if UNITY_ANDROID
    private Task<GoogleSignInUser> _pendingSignInTask;
#endif

    private void Awake()
    {
#if !UNITY_ANDROID && !UNITY_EDITOR
        if (GoogleButton != null)
            GoogleButton.gameObject.SetActive(false);
#endif
    }

    private void Start()
    {
#if UNITY_ANDROID
        GoogleSignIn.Configuration = new GoogleSignInConfiguration
        {
            RequestIdToken = false,
            RequestEmail   = true,
        };

        CheckIfUserIsAlreadyLogin();

        if (GoogleButton != null)
            GoogleButton.onClick.AddListener(SignIn);
#endif
    }

#if UNITY_ANDROID
    private void Update()
    {
        if (_pendingSignInTask != null && _pendingSignInTask.IsCompleted)
        {
            var task = _pendingSignInTask;
            _pendingSignInTask = null;
            HandleSignInResult(task);
        }
    }

    private void HandleSignInResult(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (var enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    var error = enumerator.Current as GoogleSignIn.SignInException;
                    if (error != null)
                        Debug.LogError("Google Sign-In Error: " + error.Status + " - " + error.Message);
                    else
                        Debug.LogError("Google Sign-In Exception: " + enumerator.Current);
                }
            }
            return;
        }

        if (task.IsCanceled)
        {
            Debug.LogWarning("Google Sign-In cancelled by user.");
            return;
        }

        GoogleSignInUser user = task.Result;

        string userId      = user.UserId;
        string email       = string.IsNullOrEmpty(user.Email)
                                 ? GuestLoginGenerator.GenerateUniqueEmail()
                                 : user.Email;
        string displayName = string.IsNullOrEmpty(user.DisplayName) ? "Guest" : user.DisplayName;

        Debug.Log("Google UserId: "      + userId);
        Debug.Log("Google Email: "       + email);
        Debug.Log("Google DisplayName: " + displayName);

        PlayerPersonalData.playerUserID   = userId;
        PlayerPersonalData.playerName     = displayName;
        PlayerPersonalData.playerEmail    = email;
        PlayerPersonalData.authProvider   = Global.Google;
        PlayerPersonalData.playerPassword = "GooglePass";

        PlayerPrefs.SetString(Global.UserID,       userId);
        PlayerPrefs.SetString(Global.UserName,     displayName);
        PlayerPrefs.SetString(Global.UserEmail,    email);
        PlayerPrefs.SetString(Global.AuthProvider, Global.Google);
        PlayerPrefs.Save();

        SendDataToDataBase();
    }
#endif

    public void SignIn()
    {
#if UNITY_ANDROID
        Debug.Log("GoogleManager: Starting Google Sign-In...");
        _pendingSignInTask = GoogleSignIn.DefaultInstance.SignIn();
#endif
    }

    public void SignOut()
    {
#if UNITY_ANDROID
        GoogleSignIn.DefaultInstance.SignOut();
        Debug.Log("Google_SignOut.");
#endif
    }

    private void CheckIfUserIsAlreadyLogin()
    {
        if (PlayerPrefs.HasKey(Global.UserID) &&
            PlayerPrefs.HasKey(Global.AuthProvider) &&
            PlayerPrefs.GetString(Global.AuthProvider) == Global.Google)
        {
            Debug.Log("Google user already logged in — restoring session.");
            // Wait one frame so all managers (UI_Manager, WebServiceManager) finish their Awake/Start
            StartCoroutine(RestoreSessionNextFrame());
        }
    }

    private IEnumerator RestoreSessionNextFrame()
    {
        yield return null;

        PlayerPersonalData.playerUserID   = PlayerPrefs.GetString(Global.UserID);
        PlayerPersonalData.playerName     = PlayerPrefs.GetString(Global.UserName);
        PlayerPersonalData.playerEmail    = PlayerPrefs.GetString(Global.UserEmail);
        PlayerPersonalData.authProvider   = Global.Google;
        PlayerPersonalData.playerPassword = "GooglePass";

        // LoginScreen.Awake() already showed the main menu — just reload profile from backend
        CallAPI();
    }

    private void SendDataToDataBase()
    {
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.menuScreen.gameObject, true);
        CallAPI();
    }

    private void CallAPI()
    {
        Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
        keyValuePairs.Add("userId",       PlayerPersonalData.playerUserID);
        keyValuePairs.Add("displayName",  PlayerPersonalData.playerName);
        keyValuePairs.Add("email",        PlayerPersonalData.playerEmail);
        keyValuePairs.Add("userName",     PlayerPersonalData.playerName);
        keyValuePairs.Add("authProvider", PlayerPersonalData.authProvider);

        WebServiceManager.instance.APIRequest(
            WebServiceManager.instance.signUpFunction,
            Method.POST,
            null,
            keyValuePairs,
            PlayerPersonalData.OnSuccessfullyProfileDownload,
            PlayerPersonalData.OnFailDownload,
            CACHEABLE.NULL,
            true,
            null);
    }
}

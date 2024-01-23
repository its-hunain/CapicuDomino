using System.Text;
using UnityEngine;
using UnityEngine.UI;
using AppleAuth;
using AppleAuth.Native;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using System.Collections;
using System;
using System.IO;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Dominos;

public class AppleManager : MonoBehaviour
, ILoginWithAppleIdResponse
{
    public bool Success => true;

    public IAppleError Error => throw new System.NotImplementedException();

    public IAppleIDCredential AppleIDCredential => throw new System.NotImplementedException();

    public IPasswordCredential PasswordCredential => throw new System.NotImplementedException();

    public object ConstantVariables { get; private set; }

    public IAppleAuthManager appleAuthManager;

    public Button AppleButton;

    public Texture2D appleTexture;
    public List<Texture2D> AvatarsList;

    [Space]
    [Header("User Details")]
    public string userid = "";
    public string email = "";
    public string fullname = "";
    public string identitytoken = "";
    public string f_name = "";
    public string l_name = "";

    public Text errorText;

    private void Awake()
    {
        // Check if the current platform is Android
        if (Application.platform == RuntimePlatform.Android)
        {
            // Hide the Apple login button
            if (AppleButton != null)
            {
                AppleButton.interactable = false;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        if (AppleAuthManager.IsCurrentPlatformSupported)
        {
            var deserializer = new PayloadDeserializer();
            this.appleAuthManager = new AppleAuthManager(deserializer);
        }
        if (this.appleAuthManager == null)
        {
            Debug.Log("Null AuthManager");
            var deserializer = new PayloadDeserializer();
            this.appleAuthManager = new AppleAuthManager(deserializer);
        }

        CheckIfUserisAlreadyLogin();
        if (AppleButton != null && Application.platform != RuntimePlatform.Android) AppleButton.onClick.AddListener(() => PerformSigninWithApple());

    }

    // Update is called once per frame
    void Update()
    {
        AppleManagerUpdate();
    }


    public void SignOutFromApple()
    {
        Debug.Log("Apple_LogOut.");
        Signout();
    }
    void AppleManagerUpdate()
    {
        if (AppleAuthManager.IsCurrentPlatformSupported)
        {
            if (this.appleAuthManager == null)
            {
                Debug.Log("Null AuthManager");
                var deserializer = new PayloadDeserializer();
                this.appleAuthManager = new AppleAuthManager(deserializer);
            }

            if (this.appleAuthManager != null)
            {

                appleAuthManager.Update();
            }
        }
    }


    /// <summary>
    /// Perfrom sign in with apple
    /// opens the login menu On Iphones
    /// Gets all the data like name email etc
    /// sets apple Id PlayerPrefs
    /// sends data to PlayerProfile to use against
    /// photon server or anywhere in the project
    /// use it On apple button click
    /// Either call this method on apple button or Drop apple button in above variable
    /// </summary>
    ///
    void PerformSigninWithApple()
    {
        var login = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);
        Debug.Log("login line 138 ");
        if (this.appleAuthManager != null)
        {

            Debug.Log("PerformSigninWithApple() =>  this.appleAuthManager != null ");

            this.appleAuthManager.LoginWithAppleId(login, credential =>
            {

                Debug.Log(credential + "Credentials");

                var appleIdCredential = credential as IAppleIDCredential;
                if (appleIdCredential != null)
                {

                    // Sets Credentials from IApplecredentials
                    Debug.Log(appleIdCredential + "above if");

                    if (appleIdCredential != null)
                    {
                        Debug.Log(appleIdCredential + "not null");
                        if (appleIdCredential.User != null || appleIdCredential.User != "" || !string.IsNullOrEmpty(appleIdCredential.User))
                        {
                            Debug.Log("appleIdCredential.User=> " + appleIdCredential.User.ToString());
                            userid = appleIdCredential.User.ToString();
                        }
                        else
                        {
                            int val = Random.Range(0, 9999);
                            userid = "GuestUser" + val + (val * 10);
                        }

                        try //email
                        {
                            if (appleIdCredential.Email is null || appleIdCredential.Email == null)
                            {
                                email = "GuestUser@Poker.com";
                                Debug.Log(email);
                            }
                            else
                            {

                                //Debug.Log("appleIdCredential.Email=> " + appleIdCredential.Email);

                                email = appleIdCredential.Email.ToString();

                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.Log("appleIdCredential.Email=> " + ex);
                            email = "GuestUser@Poker.com";
                        }

                        try //f name
                        {
                            if (appleIdCredential.FullName.GivenName is null || appleIdCredential.FullName.GivenName == null)
                            {
                                int val = Random.Range(0, 9999);
                                f_name = "Guest" + val;
                                Debug.Log(f_name);

                            }
                            else
                            {
                                f_name = appleIdCredential.FullName.GivenName.ToString();
                            }

                        }
                        catch (Exception ex)
                        {
                            int val = Random.Range(0, 9999);

                            Debug.Log("GivenName ex=> " + ex);
                            f_name = "Guest" + val;
                        }

                        try //l name
                        {
                            if (appleIdCredential.FullName.FamilyName is null || appleIdCredential.FullName.FamilyName == null)
                            {
                                l_name = " ";
                                Debug.Log(l_name);

                            }
                            else
                            {
                                l_name = appleIdCredential.FullName.FamilyName.ToString();
                            }

                        }
                        catch (Exception ex)
                        {
                            Debug.Log("FamilyName ex=> " + ex);
                            l_name = " ";
                        }


                        fullname = f_name + " " + l_name;


                        Debug.LogError(userid + " <=Apple User ID");
                        Debug.LogError(email + " <=Apple  email Id");
                    }
                    else
                    {
                        Debug.Log("apple credentials are null" + appleIdCredential);
                    }
                    Debug.Log("Data Fetch Complete Apple");

                    identitytoken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken,
                        0,
                        appleIdCredential.IdentityToken.Length);

                    var authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode,
                        0,
                        appleIdCredential.AuthorizationCode.Length);

                    int avatarIndex = Random.Range(0, AvatarsList.Count);
                    appleTexture = AvatarsList[avatarIndex];
                    TextureConverter.Texture2DToBase64(appleTexture);

                    PlayerPrefs.SetString(Global.UserID, userid);
                    PlayerPrefs.SetString(Global.UserName, fullname);
                    PlayerPrefs.SetString(Global.UserEmail, email);
                    PlayerPrefs.SetString(Global.AuthProvider, Global.Apple);
                    PlayerPrefs.Save();

                    SendDataToPlayerProfile();
                    SendDataToDataBase();
                }
                else
                {
                    AddToInformation("Credentials are null");
                }
            },
            error =>
            {
                Debug.Log("Credential Error");
                var authoriztionErrorCode = error.GetAuthorizationErrorCode();
                AddToInformation(authoriztionErrorCode.ToString());
            });
        }
        else
        {
            AddToInformation("this apple auth manager is null");
            //            this.appleAuthManager.Update();
            AddToInformation("Updating");
            if (this.appleAuthManager != null)
            {
                AddToInformation("Updated");
            }
        }
    }

    private void SendDataToDataBase()
    {
        PlayerPersonalData.authProvider = Global.Apple;

        Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
        keyValuePairs.Add("UserID", PlayerPersonalData.playerUserID);
        keyValuePairs.Add("FullName", PlayerPersonalData.playerName);
        keyValuePairs.Add("Email", PlayerPersonalData.playerEmail);
        keyValuePairs.Add("Password", PlayerPersonalData.playerPassword);
        keyValuePairs.Add("AuthProvider", PlayerPersonalData.authProvider);
        keyValuePairs.Add("Image", TextureConverter.Get_Base64Image());

        WebServiceManager.instance.APIRequest(WebServiceManager.instance.signUpFunction, Method.POST, null, keyValuePairs, PlayerPersonalData.OnSuccessfullyProfileDownload, PlayerPersonalData.OnFailDownload, CACHEABLE.NULL, true, null);
    }


    /// <summary>
    /// Check inside Playerprefs and bypass it
    /// </summary>
    void CheckIfUserisAlreadyLogin()
    {
        if (PlayerPrefs.HasKey(Global.UserID) &&
            PlayerPrefs.HasKey(Global.AuthProvider) &&
            PlayerPrefs.GetString(Global.AuthProvider) == Global.Apple)
        {
            Debug.Log("Has Key");

            RestoreCredentials(PlayerPrefs.GetString(Global.UserID));
        }
    }


    /// <summary>
    /// Restore from credentials if apple id is found authorized else it will delete that apple id
    /// </summary>
    /// <param name="AppleUserID"></param>
    void RestoreCredentials(string AppleUserID)
    {
        Debug.LogError("Apple ID Exists now restoring data");

        this.appleAuthManager.GetCredentialState(
            AppleUserID,
            state =>
            {
                switch (state)
                {
                    case CredentialState.Authorized:

                        RetrieveData();
                        break;
                    case CredentialState.NotFound:
                        //this.appleAuthManager.SetCredentialsRevokedCallback(null);
                        PlayerPrefs.DeleteKey(AppleUserID);
                        PerformSigninWithApple();
                        break;
                    case CredentialState.Revoked:
                        //this.appleAuthManager.SetCredentialsRevokedCallback(null);
                        PlayerPrefs.DeleteKey(AppleUserID);
                        PerformSigninWithApple();
                        break;
                }
            },
            error =>
            {
                Debug.Log("Error Fetching Data");
            });
    }

    /// <summary>
    /// Get data from PlayerPrefs
    /// </summary>
    void RetrieveData()
    {
        Debug.Log("Retrieving Data");

        userid = PlayerPrefs.GetString(Global.UserID);
        fullname = PlayerPrefs.GetString(Global.UserName);
        email = PlayerPrefs.GetString(Global.UserEmail);
        var appleTexture = TextureConverter.Base64ToTexture2D(TextureConverter.Get_Base64Image());
        PlayerPersonalData.playerTexture = appleTexture;
        SendDataToPlayerProfile();


        SendDataToDataBase();
    }



    /// <summary>
    /// saves data in player profile
    /// </summary>
    void SendDataToPlayerProfile()
    {
        Debug.Log("Sending Data");

        PlayerPersonalData.playerEmail = email;
        PlayerPersonalData.playerName = fullname;
        PlayerPersonalData.playerUserID = userid;
        PlayerPersonalData.authProvider = Global.Apple;
        PlayerPersonalData.playerPassword = "ApplePass";
        AddToInformation("fullname: " + fullname);
        AddToInformation("userid: " + userid);
        AddToInformation("email: " + email);

        //PhotonConnectionController.Instance.ConnectingToPhoton();
    }



    public void Signout()
    {
        this.appleAuthManager.SetCredentialsRevokedCallback(null);
    }

    public void Revoked()
    {

    }

    public void AddToInformation(string str)
    {
        Debug.LogError("\n" + str);
    }

}


using Dominos;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static NativeGallery;
using Newtonsoft.Json.Linq;

public class EditProfileScreen : MonoBehaviour
{
    public Image profileImage;
    public Texture2D playerTexture;
    public Button uploadImageBtn;

    public InputField name;
    public InputField country;
    public InputField age;

    public Button gender;
    public Button backBtn;
    public Button saveBtn;

    [Header("ImgBB Configuration")]
    [Tooltip("Get your free API key from https://api.imgbb.com/")]
    public string imgbbApiKey = "eeb0c71d4a137a26a29abf55cc8aae0f";

    string genderValue = "male";

    private void OnEnable()
    {
        UpdateUI();
    }
    void Start()
    {
        backBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.editProfileScreen.gameObject, false));
        uploadImageBtn.onClick.AddListener(() => PickImage(512));
        saveBtn.onClick.AddListener(() => UploadImage());

        gender.onClick.AddListener(() => ChangeGender());
    }

    private void ChangeGender()
    {
        genderValue = genderValue == "male" ? "female" : "male";
        UpdateGender();
    }

    private void PictureFetch(Texture2D texture2D)
    {
        profileImage.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(texture2D.width / 2, texture2D.height / 2));
        playerTexture = texture2D;
        PlayerPersonalData.playerTexture = texture2D;

    }

    private void PickImage(int maxSize)
    {
        Permission permission = GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Create Texture from selected image
                Texture2D texture = LoadImageAtPath(path, 256 , false , false, false);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                PictureFetch(texture);
            }
        });

        Debug.Log("Permission result: " + permission);
    }

    public void UploadImage()
    {
        // ImgBB API configuration
        string imgbbUrl = "https://api.imgbb.com/1/upload?key=" + imgbbApiKey;

        // Convert image to base64
        byte[] imageBytes = playerTexture.EncodeToJPG();
        string base64Image = System.Convert.ToBase64String(imageBytes);

        // Prepare form data for ImgBB
        Dictionary<string, object> postData = new Dictionary<string, object>();
        postData.Add("image", base64Image);

        Debug.Log("Uploading image to ImgBB...");

        // Upload image to ImgBB
        WebServiceManager.instance.UploadT0Bucket(imgbbUrl, Method.POST, null, postData, OnImageUploadSuccess, OnFail, CACHEABLE.NULL, true, null);
    }

    private void OnImageUploadSuccess(string response, long statusCode)
    {
        Debug.Log("ImgBB upload response: " + response);

        try
        {
            JObject jsonResponse = JObject.Parse(response);

            // ImgBB response format: {"data": {"url": "https://i.ibb.co/..."}, "success": true}
            string fileUrl = jsonResponse["data"]["url"].ToString();

            Debug.Log("Profile image URL from ImgBB: " + fileUrl);

            // Now save the player info with the profile URL
            SavePlayerInfo(fileUrl);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to parse ImgBB response: " + ex.Message);
            Debug.LogError("Response was: " + response);
            OnFail("Failed to parse upload response");
        }
    }

    private void OnFail(string obj)
    {
        Debug.LogError("Error: " + obj);

    }

    public void SavePlayerInfo(string profileUrl = null)
    {
        Dictionary<string, object> postData = new Dictionary<string, object>();

        string userName = name.text.ToString();
        string country = this.country.text.ToString();
        string age = this.age.text.ToString();
        string gender = genderValue;

        postData.Add("userName", userName);
        postData.Add("age", age);
        postData.Add("gender", gender);

        // Add profile URL if provided
        if (!string.IsNullOrEmpty(profileUrl))
        {
            postData.Add("profilePicUrl", profileUrl);
        }

        WebServiceManager.instance.APIRequest(WebServiceManager.instance.getPlayerProfile, Method.POST, null, postData, PlayerPersonalData.OnSuccessfullyProfileUpdated, PlayerPersonalData.OnFailDownload, CACHEABLE.NULL, true, null);
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.editProfileScreen.gameObject, false);
    }


    public void UpdateUI()
    {
        name.text = PlayerPersonalData.playerName;
        var temp = Sprite.Create(PlayerPersonalData.playerTexture, new Rect(0.0f, 0.0f, PlayerPersonalData.playerTexture.width, PlayerPersonalData.playerTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
        profileImage.sprite = temp;
        playerTexture = PlayerPersonalData.playerTexture;
        country.text = PlayerPersonalData.country;
        age.text =     PlayerPersonalData.age.ToString();
        genderValue = PlayerPersonalData.gender;
        UpdateGender();
    }

    public void UpdateGender()
    {
        gender.transform.rotation = new Quaternion(0, 0, (genderValue == "male") ? 0 : 180 , 0);
        Debug.Log("genderValue: " + genderValue);
    }
}

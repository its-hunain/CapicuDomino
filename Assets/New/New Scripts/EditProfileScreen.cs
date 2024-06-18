
using Dominos;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static NativeGallery;

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


    }

    private void PickImage(int maxSize)
    {
        Permission permission = GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Create Texture from selected image
                Texture2D texture = LoadImageAtPath(path, 512 , false);
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
        string route = "https://lady-luck-api-dev.cubestagearea.xyz/api/upload/image";

        Dictionary<string, object> postData = new Dictionary<string, object>();
        FileUplaod fileUplaod = new FileUplaod();
        fileUplaod.key = "file";
        fileUplaod.mimeType = ".jpg";
        fileUplaod.name = "profilePic";
        fileUplaod.data = playerTexture.EncodeToPNG();

        WebServiceManager.instance.UploadT0Bucket(route, Method.POST,null,null,SavePlayerInfo , OnFail , CACHEABLE.NULL,true,fileUplaod );
    }

    private void OnFail(string obj)
    {
        Debug.LogError("Error: " + obj);

    }

    public void SavePlayerInfo(string keyValuePairs, long code)
    {
        //UI_Manager.instance.SaveUserData(name.text.ToString(), country.text.ToString(), age.text.ToString(), gender.text.ToString());

        if (!ResponseStatus.Check(code))
        {
            Debug.LogError("Error: " + keyValuePairs.ToString());

            
            return;
        }

        Debug.LogError("image data: "+ keyValuePairs.ToString());
        ImageUpload fileData = ImageUpload.FromJson(keyValuePairs.ToString());



        Dictionary<string, object> postData = new Dictionary<string, object>();


        string userName = name.text.ToString();
        string country = this.country.text.ToString();
        string age = this.age.text.ToString();
        string gender = genderValue;

        postData.Add("displayName", userName);
        Debug.Log("fileData.file_url: " + fileData.data.file_url);
        postData.Add("profilePicUrl", fileData.data.file_url);
        postData.Add("age", age);
        postData.Add("gender", gender);

        WebServiceManager.instance.APIRequest(WebServiceManager.instance.getPlayerProfile, Method.POST, null, postData,PlayerPersonalData.OnSuccessfullyProfileUpdated, PlayerPersonalData.OnFailDownload, CACHEABLE.NULL, true, null);
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

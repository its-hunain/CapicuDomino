
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static NativeGallery;

public class EditProfileScreen : MonoBehaviour
{
    public Image profileImage;
    public Button uploadImageBtn;

    public InputField name;
    public InputField country;
    public InputField age;

    public Button gender;
    public Button backBtn;
    public Button saveBtn;

    bool isMale = true;


    void Start()
    {
        backBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.editProfileScreen.gameObject, false));
        uploadImageBtn.onClick.AddListener(() => PickImage(512));
        saveBtn.onClick.AddListener(() => SavePlayerInfo());

        gender.onClick.AddListener(() => ChangeGender());
    }

 
    private void PictureFetch(Texture2D texture2D)
    {
        profileImage.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(texture2D.width / 2, texture2D.height / 2));

        
    }

    private void PickImage(int maxSize)
    {
        Permission permission = GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Create Texture from selected image
                Texture2D texture = LoadImageAtPath(path, maxSize);
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


    public void SavePlayerInfo()
    {
        //UI_Manager.instance.SaveUserData(name.text.ToString(), country.text.ToString(), age.text.ToString(), gender.text.ToString());


        Dictionary<string, object> postData = new Dictionary<string, object>();

        string userName = name.text.ToString();
        string country = this.country.text.ToString();
        string age = this.age.text.ToString();
        string gender =isMale==true? "male":"female";
        postData.Add("userName", userName);
       // postData.Add("country", country);
        postData.Add("age", age);
        postData.Add("gender", gender);

        WebServiceManager.instance.APIRequest(WebServiceManager.instance.getPlayerProfile, Method.POST, null, postData,PlayerPersonalData.OnSuccessfullyProfileUpdated, PlayerPersonalData.OnFailDownload, CACHEABLE.NULL, true, null);
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.editProfileScreen.gameObject, false);
    }
    public void UpdateUISuccess()
    {

    }
    public void UpdateUI()
    {
        name.text = PlayerPersonalData.playerName;
        var temp = Sprite.Create(PlayerPersonalData.playerTexture, new Rect(0.0f, 0.0f, PlayerPersonalData.playerTexture.width, PlayerPersonalData.playerTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
        profileImage.sprite = temp;
         country.text = PlayerPersonalData.country;
         age.text =     PlayerPersonalData.age.ToString();
         ChangeGender(PlayerPersonalData.gender);


    }

    public void ChangeGender(string str=null)
    {
            isMale = !isMale;

        if (str !=null)
        {
            isMale = str == "male" ? true : false;
            if (isMale)
                return;
        }
        //else

            gender.transform.Rotate(0, 0, 180);
        Debug.LogError("male? "+isMale);

    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditProfileScreen : MonoBehaviour
{
    public Image profileImage;
    public Button uploadImageBtn;

    public InputField name;
    public InputField country;
    public InputField age;
    public InputField gender;

    public Button backBtn;
    public Button saveBtn;

    void Start()
    {
        backBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.editProfileScreen.gameObject, false));
        saveBtn.onClick.AddListener(() => SavePlayerInfo());
    }
    public void SavePlayerInfo()
    {
        //UI_Manager.instance.SaveUserData(name.text.ToString(), country.text.ToString(), age.text.ToString(), gender.text.ToString());


        Dictionary<string, object> postData = new Dictionary<string, object>();

        string userName = name.text.ToString();
        string country = this.country.text.ToString();
        string age = this.age.text.ToString();
        string gender = this.gender.text.ToString();
        postData.Add("userName", userName);
        postData.Add("country", country);
        postData.Add("age", age);
        postData.Add("gender", gender);

        WebServiceManager.instance.APIRequest(WebServiceManager.instance.getPlayerProfile, Method.POST, null, postData,PlayerPersonalData.OnSuccessfullyProfileDownload, PlayerPersonalData.OnFailDownload, CACHEABLE.NULL, true, null);
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
         gender.text = PlayerPersonalData.gender;


    }
}

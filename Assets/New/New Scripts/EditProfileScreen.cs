
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
            name.text = UI_Manager.instance.userName;
         country.text = UI_Manager.instance.userCountry;
             age.text = UI_Manager.instance.userAge;
        gender.text =  UI_Manager.instance.userGender;

        backBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.editProfileScreen.gameObject, false));
        saveBtn.onClick.AddListener(() => SavePlayerInfo());
    }
    void SavePlayerInfo()
    {

        UI_Manager.instance.SaveUserData(name.text.ToString(), country.text.ToString(), age.text.ToString(), gender.text.ToString());
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.editProfileScreen.gameObject, false);
    }
}

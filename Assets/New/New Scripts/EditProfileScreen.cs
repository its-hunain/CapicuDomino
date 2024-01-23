
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditProfileScreen : MonoBehaviour
{
    public Image profileImage;
    public Button uploadImageBtn;

    public Text name;
    public Text country;
    public Text age;
    public Text gender;

    public Button backBtn;
    public Button saveBtn;

    void Start()
    {
        backBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.editProfileScreen.gameObject, false));
        saveBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.editProfileScreen.gameObject, false));
    }

}

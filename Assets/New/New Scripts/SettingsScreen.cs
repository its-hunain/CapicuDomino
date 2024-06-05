using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsScreen : MonoBehaviour
{
    public Button soundBtn;
    public Button musicBtn;
    public Button logoutBtn;
    public Button aboutBtn;
    public Button termsBtn;
    public Button policyBtn;
    public Button closeBtn;

    public bool soundOn = true;
    public bool musicOn = true;


    private void Start()
    {
        GetSoundSettings();

        CustomToggler(musicBtn, musicOn);
        CustomToggler(soundBtn, soundOn);

        musicBtn.onClick.AddListener(() => MusicToggle());
        soundBtn.onClick.AddListener(() => SoundToggle());

        closeBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.settingScreen.gameObject, false));

        aboutBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.aboutScreen.gameObject, true));
        termsBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.aboutScreen.gameObject, true));
        policyBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.aboutScreen.gameObject, true));
        logoutBtn.onClick.AddListener(() => LogOut());
    }

    public void LogOut()
    {
        //UI_Manager.instance.ChangeScreen(UI_Manager.instance.settingScreen.gameObject, false);
        //UI_Manager.instance.ChangeScreen(UI_Manager.instance.menuScreen.gameObject, false);
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(Global.UIScene);
    }
    public void MusicToggle()
    {
        musicOn = !musicOn;
        CustomToggler(musicBtn, musicOn);

        int val = musicOn == true ? 1 : 0;
        PlayerPrefs.SetInt("musicOn", val);

        SoundManager.instance.MusicControllor(val);

    }
    public void SoundToggle()
    {
        soundOn = !soundOn;
        CustomToggler(soundBtn, soundOn);

        int val = soundOn == true ? 1 : 0;
        PlayerPrefs.SetInt("soundOn", val);

        SoundManager.instance.SFXControllor(val);

    }

    public void CustomToggler(Button btn,bool value)
    {
        btn.transform.GetChild(2).gameObject.SetActive(value);
        btn.transform.GetChild(3).gameObject.SetActive(!value);
    }

    public void GetSoundSettings()
    {
        soundOn = PlayerPrefs.GetInt("soundOn") == 1 ? true : false;

        SoundManager.instance.SFXControllor(PlayerPrefs.GetInt("soundOn"));

        musicOn = PlayerPrefs.GetInt("musicOn") == 1 ? true : false;

        SoundManager.instance.MusicControllor(PlayerPrefs.GetInt("musicOn"));

    }
}

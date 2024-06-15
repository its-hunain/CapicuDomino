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

        aboutBtn.onClick.AddListener(()  => AboutBtnCallBack(aboutBtn));
        termsBtn.onClick.AddListener(()  => AboutBtnCallBack(termsBtn));
        policyBtn.onClick.AddListener(() => AboutBtnCallBack(policyBtn));


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

    public void CustomToggler(Button btn, bool value)
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
    void AboutBtnCallBack(Button btn)
    {
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.aboutScreen.gameObject, true);

        if (btn == aboutBtn)
            UI_Manager.instance.aboutScreen.UpdateUIText("ABOUT CAPICU GAME", "Lorem ipsum dolor sit amet consectetur adipiscing elit odio, mattis quam tortor taciti aenean luctus nullam enim, dui praesent ad dapibus tempus natoque a. Rhoncus praesent massa torquent malesuada maecenas arcu curae, porta pulvinar potenti at mus sem, vel purus proin eleifend nisi dictum. Egestas tortor blandit vestibulum tempus dignissim cras placerat, ligula ridiculus sollicitudin interdum quisque facilisis, suscipit tempor justo tristique et mattis. Nisl imperdiet donec nascetur feugiat massa vehicula elementum nullam purus morbi, sagittis et penatibus taciti vitae lobortis facilisis maecenas gravida, venenatis sed pellentesque suspendisse sociis magna class nibh volutpat. Sodales leo arcu ornare eget torquent dictumst, id morbi fringilla ultricies suscipit, nulla sapien a aliquet tempor. Tristique non eros a felis quam convallis nascetur montes auctor hendrerit, mollis metus sodales ligula magnis condimentum et arcu nam. id morbi fringilla ultricies suscipit, nulla sapien a aliquet tempor. Tristique non eros a felis quam convallis nascetur montes auctor hendrerit, mollis metus sodales ligula magnis condimentum et arcu nam. Lorem ipsum dolor sit amet consectetur adipiscing elit odio, mattis quam tortor taciti aenean luctus nullam enim, dui praesent ad dapibus tempus natoque a. Rhoncus praesent massa torquent malesuada maecenas arcu curae, porta pulvinar potenti at mus sem, vel purus proin eleifend nisi dictum Tristique non eros a felis quam convallis nascetur montes auctor hendrerit, mollis metus sodales ligula magnis condimentum et arcu nam. Lorem ipsum dolnam");

        else if (btn == termsBtn)
            UI_Manager.instance.aboutScreen.UpdateUIText("TERMS AND CONDITIONS", "Lorem ipsum dolor sit amet consectetur adipiscing elit odio, mattis quam tortor taciti aenean luctus nullam enim, dui praesent ad dapibus tempus natoque a. Rhoncus praesent massa torquent malesuada maecenas arcu curae, porta pulvinar potenti at mus sem, vel purus proin eleifend nisi dictum. Egestas tortor blandit vestibulum tempus dignissim cras placerat, ligula ridiculus sollicitudin interdum quisque facilisis, suscipit tempor justo tristique et mattis. Nisl imperdiet donec nascetur feugiat massa vehicula elementum nullam purus morbi, sagittis et penatibus taciti vitae lobortis facilisis maecenas gravida, venenatis sed pellentesque suspendisse sociis magna class nibh volutpat. Sodales leo arcu ornare eget torquent dictumst, id morbi fringilla ultricies suscipit, nulla sapien a aliquet tempor. Tristique non eros a felis quam convallis nascetur montes auctor hendrerit, mollis metus sodales ligula magnis condimentum et arcu nam. id morbi fringilla ultricies suscipit, nulla sapien a aliquet tempor. Tristique non eros a felis quam convallis nascetur montes auctor hendrerit, mollis metus sodales ligula magnis condimentum et arcu nam. Lorem ipsum dolor sit amet consectetur adipiscing elit odio, mattis quam tortor taciti aenean luctus nullam enim, dui praesent ad dapibus tempus natoque a. Rhoncus praesent massa torquent malesuada maecenas arcu curae, porta pulvinar potenti at mus sem, vel purus proin eleifend nisi dictum Tristique non eros a felis quam convallis nascetur montes auctor hendrerit, mollis metus sodales ligula magnis condimentum et arcu nam. Lorem ipsum dolnam");

        else if (btn == policyBtn)
            UI_Manager.instance.aboutScreen.UpdateUIText("PRIVACY AND POLICY", "Lorem ipsum dolor sit amet consectetur adipiscing elit odio, mattis quam tortor taciti aenean luctus nullam enim, dui praesent ad dapibus tempus natoque a. Rhoncus praesent massa torquent malesuada maecenas arcu curae, porta pulvinar potenti at mus sem, vel purus proin eleifend nisi dictum. Egestas tortor blandit vestibulum tempus dignissim cras placerat, ligula ridiculus sollicitudin interdum quisque facilisis, suscipit tempor justo tristique et mattis. Nisl imperdiet donec nascetur feugiat massa vehicula elementum nullam purus morbi, sagittis et penatibus taciti vitae lobortis facilisis maecenas gravida, venenatis sed pellentesque suspendisse sociis magna class nibh volutpat. Sodales leo arcu ornare eget torquent dictumst, id morbi fringilla ultricies suscipit, nulla sapien a aliquet tempor. Tristique non eros a felis quam convallis nascetur montes auctor hendrerit, mollis metus sodales ligula magnis condimentum et arcu nam. id morbi fringilla ultricies suscipit, nulla sapien a aliquet tempor. Tristique non eros a felis quam convallis nascetur montes auctor hendrerit, mollis metus sodales ligula magnis condimentum et arcu nam. Lorem ipsum dolor sit amet consectetur adipiscing elit odio, mattis quam tortor taciti aenean luctus nullam enim, dui praesent ad dapibus tempus natoque a. Rhoncus praesent massa torquent malesuada maecenas arcu curae, porta pulvinar potenti at mus sem, vel purus proin eleifend nisi dictum Tristique non eros a felis quam convallis nascetur montes auctor hendrerit, mollis metus sodales ligula magnis condimentum et arcu nam. Lorem ipsum dolnam");

    }
}

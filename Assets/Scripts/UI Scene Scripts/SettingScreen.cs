using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingScreen : MonoBehaviour
{

    public Button closeBtn;
    public Button doneBtn;
    public Button soundBtn;
    public Button musicBtn;



    public Button songLeftBtn;
    public Button songRightBtn;
    public Text musicNameTxt;
    public Color greenColor;
    public Color whiteColor;

    public static int songIndex = 1;

    void Start()
    {
        songLeftBtn.onClick.AddListener(() => SongLeftButtonDelegate());
        songRightBtn.onClick.AddListener(() => SongRightButtonDelegate());
        closeBtn.onClick.AddListener(() => backButtonDelegate());
        doneBtn.onClick.AddListener(() => backButtonDelegate());
        soundBtn.onClick.AddListener(() => SoundButtonDelegate());
        musicBtn.onClick.AddListener(() => MusicButtonDelegate());
    }
    private void OnEnable()
    {
        musicNameTxt.text = SoundManager.instance.menuMusicSource.clip.name;
        ChangeColor();

        UpdateImage(soundBtn, SoundManager._soundIsOn);
        UpdateImage(musicBtn, SoundManager._musicIsOn);
    }

    private void SoundButtonDelegate()
    {
        SoundManager.instance.ButtonPressPlayer(true);
        UpdateSoundAndMusicDelegate(soundBtn);
    }

    private void UpdateSoundAndMusicDelegate(Button Btn)
    {
        if (Btn == soundBtn)
        {
            UpdateImage(Btn, !SoundManager._soundIsOn);
            SoundManager.instance.SFXControllor(SoundManager._soundIsOn ? 0 : 1);
            SoundManager._soundIsOn = !SoundManager._soundIsOn;
        }
        else
        {
            UpdateImage(Btn, !SoundManager._musicIsOn);
            SoundManager.instance.MusicControllor(SoundManager._musicIsOn ? 0 : 1);
            SoundManager._musicIsOn = !SoundManager._musicIsOn;
        }
    }

    private void MusicButtonDelegate()
    {
        SoundManager.instance.ButtonPressPlayer(true);
        UpdateSoundAndMusicDelegate(musicBtn);
    }

    private void backButtonDelegate()
    {
        SoundManager.instance.ButtonPressPlayer(true);
        UI_ScreenManager.OpenClosePopUp(gameObject, false, false);
    }

    private void SongLeftButtonDelegate()
    {
        SoundManager.instance.ButtonPressPlayer(true);

        if (songIndex > 1)
        {
            songIndex--;
            ChangeClip();
        }
    }

    private void SongRightButtonDelegate()
    {
        SoundManager.instance.ButtonPressPlayer(true);

        if (songIndex < 5)
        {
            songIndex++;
            ChangeClip();
        }
    }

    public void ChangeClip()
    {
        AudioClip audioClip;
        switch (songIndex)
        {
            case 1:
                audioClip = SoundManager.instance.DistantVoices;
                break;
            case 2:
                audioClip = SoundManager.instance.El_Inicio;
                break;
            case 3:
                audioClip = SoundManager.instance.En_Seguida;
                break;
            case 4:
                audioClip = SoundManager.instance.La_Partida;
                break;
            case 5:
                audioClip = SoundManager.instance.Venezolano;
                break;
            default:
                audioClip = SoundManager.instance.DistantVoices;
                break;
        }
        Debug.Log("audioClip song index : " + songIndex+  " and name = "  + audioClip.name);
        SoundManager.instance.menuMusicSource.clip = audioClip;
        musicNameTxt.text = SoundManager.instance.menuMusicSource.clip.name;
        SoundManager.instance.MenuBGPlayer(true);

        ChangeColor();
    }

    void ChangeColor()
    {
        if (songIndex <= 1)
        {
            songLeftBtn.GetComponent<Image>().color = whiteColor;
        }
        else
        {
            songLeftBtn.GetComponent<Image>().color = greenColor;
        }

        if (songIndex >= 5)
        {
            songRightBtn.GetComponent<Image>().color = whiteColor;
        }
        else
        {
            songRightBtn.GetComponent<Image>().color = greenColor;
        }
    }

    public void UpdateImage(Button selectedBtn, bool isOn)
    {
        var anim = selectedBtn.GetComponent<Animation>();
        var animClip = anim.clip;
        anim[animClip.name].speed = isOn ? 1 : - 1;
        anim[animClip.name].time = anim[animClip.name].length;
        anim.Play(animClip.name);
        selectedBtn.transform.GetChild(1).GetComponent<Text>().color = isOn ? Color.white : Color.black;
        selectedBtn.transform.GetChild(2).GetComponent<Text>().color = isOn ? Color.black : Color.white;
    }
}
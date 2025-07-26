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
    public bool isGameScene=false;

    private void Start()
    {

        if (!isGameScene)
        {
            closeBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.settingScreen.gameObject, false));
            aboutBtn.onClick.AddListener(() => AboutBtnCallBack(aboutBtn));
            termsBtn.onClick.AddListener(() => AboutBtnCallBack(termsBtn));
            policyBtn.onClick.AddListener(() => AboutBtnCallBack(policyBtn));
            logoutBtn.onClick.AddListener(() => LogOut());
        }

        GetSoundSettings();

        CustomToggler(musicBtn, musicOn);
        CustomToggler(soundBtn, soundOn);

        musicBtn.onClick.AddListener(() => MusicToggle());
        soundBtn.onClick.AddListener(() => SoundToggle());

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
            UI_Manager.instance.aboutScreen.UpdateUIText("ABOUT US", "Born from the passion of two seasoned domino gamers, Capicu has quickly become the go-to platform for players seeking the authentic thrill of competitive dominoes. Designed by people who live and breathe the game, Capicu brings a fresh, immersive, and fast-paced experience to domino enthusiasts everywhere. Whether you’ve been playing dominoes for years or are just discovering the game, Capicu offers a unique way to enjoy this timeless pastime, combining strategy, camaraderie, and excitement right at your fingertips. With a global community of players, Capicu provides the pure joy of dominoes anytime, anywhere.\r\n\r\nCapicu isn’t just another domino game—it’s a reflection of the passion and dedication of its creators. Built by individuals who’ve spent years honing their skills in competitive dominoes, Capicu delivers an experience that captures the essence of the game’s rich traditions and thrilling dynamics. Here’s why <b>Capicu stands out:</b>\r\n\r\n<b>Authenticity at Its Core:</b> Capicu’s creators have ensured the game stays true to its roots. The rules, gameplay, and interface are designed to mimic the feeling of playing a real-life game of dominoes with friends.\r\n\r\n<b>Fast-Paced Gameplay:</b> Dominoes is a game of both strategy and speed. Capicu reflects this by offering a fast-paced environment that keeps players on their toes while maintaining the integrity of the game.\r\n\r\n<b>Global Community:</b> Capicu brings together players worldwide, fostering a sense of community and competition. Whether you’re looking to test your skills against seasoned players or make new friends, Capicu is the perfect platform for social interaction and competitive fun.\r\n");

        else if (btn == termsBtn)
            UI_Manager.instance.aboutScreen.UpdateUIText("TERMS AND CONDITIONS", "Welcome to Capicu! Before using our mobile application, please take a moment to review the following terms and conditions. By accessing or using our services, you agree to comply with these terms and conditions, which govern your use of the Capicu mobile application.\r\n\r\n<b>1. Acceptance of Terms</b>\r\nBy accessing or using the Capicu mobile application, you agree to be bound by these terms and conditions. If you do not agree to these terms, please refrain from using our services.\r\n\r\n<b>2. Use of Services</b>\r\nYou may use the Capicu mobile application for personal, non-commercial purposes only. You agree not to use our services for any illegal or unauthorized purpose, or in any way that violates these terms and conditions.\r\n\r\n<b>3. Intellectual Property</b>\r\nAll content, including but not limited to text, graphics, logos, images, and software, is the property of Capicu and is protected by copyright and other intellectual property laws. You may not use, reproduce, modify, or distribute any content from our mobile application without prior written consent.\r\n\r\n<b>4. User Accounts</b>\r\nTo access certain features of the Capicu app, you may be required to create a user account. You are responsible for maintaining the confidentiality of your account credentials and for any activity that occurs under your account.\r\n\r\n<b>5. Privacy Policy</b>\r\nYour privacy is important to us. Please review our Privacy Policy to understand how we collect, use, and disclose your personal information when you use our mobile application.\r\n\r\n<b>6. Limitation of Liability</b>\r\nCapicu and its affiliates shall not be liable for any direct, indirect, incidental, special, or consequential damages arising out of or in any way connected with your use of our services.\r\n\r\n<b>7. Governing Law</b>\r\nThese terms and conditions shall be governed by and construed in accordance with the laws of USA, without regard to its conflict of law provisions.\r\n\r\n<b>8. Changes to Terms</b>\r\nCapicu reserves the right to update or modify these terms and conditions at any time without prior notice. Your continued use of our services after any changes to these terms constitutes your acceptance of the revised terms.\r\n\r\nThank you for reviewing our terms and conditions. If you have any questions or concerns, please contact us.\r\n");

        else if (btn == policyBtn)
            UI_Manager.instance.aboutScreen.UpdateUIText("PRIVACY AND POLICY", "At Capicu, we are committed to protecting your privacy and ensuring the security of your personal information. This Privacy Policy outlines how we collect, use, disclose, and protect the information you provide when using our app.\r\n\r\n<b>Information We Collect</b>\r\nWhen you use Capicu, we may collect personal information such as your name, email address, and profile picture. We may also collect non-personal information such as device information, usage data, and cookies.\r\n\r\n<b>How We Use Your Information</b>\r\nWe use your personal information to create and manage your account, provide customer support, personalize your experience, and communicate with you about updates, promotions, and new features. We may also use non-personal information for analytics, research, and improving our services.\r\n\r\n<b>Sharing Your Information</b>\r\nWe do not sell, trade, or rent your personal information to third parties. However, we may share your information with trusted third-party service providers who assist us in operating our app, conducting business, or servicing you.\r\n\r\n<b>Data Security</b>\r\nWe take appropriate measures to protect your personal information from unauthorized access, alteration, disclosure, or destruction. We use industry-standard security technologies and procedures to safeguard your data.\r\n\r\n<b>Your Choices</b>\r\nYou have the right to access, update, or delete your personal information at any time. You can also opt-out of receiving promotional emails from us by following the instructions provided in the email.\r\n\r\n<b>Changes to this Policy</b>\r\nWe reserve the right to update or change this Privacy Policy at any time. We will notify you of any changes by posting the new Privacy Policy on this page.\r\n\r\n<b>Contact Us</b>\r\nIf you have any questions or concerns about our Privacy Policy, please contact us.\r\n\r\nBy using Capicu, you consent to the terms of this Privacy Policy.\r\nThank you for trusting us with your information.\r\n");

    }
}

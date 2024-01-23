using AvatarBuilder;
using UnityEngine;
using UnityEngine.UI;

public class AvatarNamePopUp : MonoBehaviour
{
    [Header("PopUp Items")]
    public InputField avatarNameField;
    public Button yesBtn;
    public Button noBtn;

    private void OnEnable()
    {
        avatarNameField.text = "";
        yesBtn.interactable = false;
    }

    void Start()
    {
        avatarNameField.onValueChanged.AddListener(delegate {
            OnValueChangeListener();
        });
        yesBtn.onClick.AddListener(() => SaveName());
        noBtn.onClick.AddListener(() => ClosePopUpDelegate());
    }

    private void OnValueChangeListener()
    {
        yesBtn.interactable = avatarNameField.text.Length > 0;
    }

    public void SaveName()
    {
        AvatarParent_FbxHolder.instance.cachedSelecteditem.avatarName = avatarNameField.text;
        AvatarScreenManager.OpenClosePopUp(AvatarScreenManager.instance.MintAvatarPopUpScreen.gameObject, true);
        gameObject.SetActive(false);
    }

    public void ClosePopUpDelegate()
    {
        if (SoundManager.instance != null) SoundManager.instance.ButtonPressPlayer(true);

        AvatarScreenManager.OpenClosePopUp(gameObject, false);
    }
}
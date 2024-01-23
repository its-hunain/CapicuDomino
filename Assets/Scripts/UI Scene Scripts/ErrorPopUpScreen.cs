using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorPopUpScreen : MonoBehaviour
{
    public Button okBtn;
    public Text warningText;

    private System.Action callback; 

    // Start is called before the first frame update
    void Start()
    {
        okBtn.onClick.AddListener(()=> {
            Close();

            if (callback != null)
            {
                callback();
            }
       });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetWarning(string warning)
    {
        warningText.text = warning;
    }

    public void OpenCloseWarning(bool state , string warning = "" , bool hideOkButton = false, System.Action action = null)
    {
        callback = action;
        okBtn.gameObject.SetActive(!hideOkButton);

        SetWarning(warning);

        UI_ScreenManager.OpenClosePopUp(UI_ScreenManager.instance.errorPopUpScreen.gameObject, state, state);
    }

    public void Close()
    {
        UI_ScreenManager.OpenClosePopUp(UI_ScreenManager.instance.errorPopUpScreen.gameObject, false, false);
    }
}

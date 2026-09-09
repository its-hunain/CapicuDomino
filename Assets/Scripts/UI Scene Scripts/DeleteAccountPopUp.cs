using UnityEngine;
using UnityEngine.UI;

public class DeleteAccountPopUp : MonoBehaviour
{
    public Button okBtn;
    public Text warningText;

    private System.Action callback;

    // Start is called before the first frame update
    void Start()
    {
        okBtn.onClick.AddListener(() => {
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

    public void OpenCloseWarning(bool state, string warning = "", bool hideOkButton = false, System.Action action = null)
    {
        callback = action;
        okBtn.gameObject.SetActive(!hideOkButton);
        SetWarning(warning);
        gameObject.SetActive(state);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}

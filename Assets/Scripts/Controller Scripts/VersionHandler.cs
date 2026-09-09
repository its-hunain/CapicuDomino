using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersionHandler : MonoBehaviour
{
    public Text versionText;
    // Start is called before the first frame update
    void Start()
    {
        versionText.gameObject.SetActive(true);
        versionText.text = "V-" + Application.version;
        Invoke("HideVersion", 5f);
    }

    void HideVersion()
    {
        versionText.gameObject.SetActive(false);
    }
}
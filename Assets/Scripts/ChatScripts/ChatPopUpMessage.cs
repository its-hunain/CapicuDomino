using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatPopUpMessage : MonoBehaviour
{
    public Image emoteImage;
    public Text msgTxt;

    public GameObject emoteImageBG;
    public GameObject msgTxtBG;

    void Start()
    {
        Invoke("DisableObject",2f);
        LeanTween.scale(gameObject, gameObject.transform.localScale * 1.2f, 0.2f);
    }

    void DisableObject()
    {
        LeanTween.scale(gameObject, gameObject.transform.localScale * 0f, .5f).setEaseInBack().setOnComplete(() => {
            Destroy(gameObject, 1f);
        });
    }
}

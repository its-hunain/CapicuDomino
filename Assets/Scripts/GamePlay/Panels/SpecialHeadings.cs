using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialHeadings : MonoBehaviour
{

    public List<SpecialHeadingsData> specialHeadingsDatas = new List<SpecialHeadingsData>();
    public Image image;



    public void ShowData(string headingName)
    {
        image.transform.localScale = Vector3.zero;
        gameObject.SetActive(true);
        image.sprite = specialHeadingsDatas.Find(heading => heading.headingName.Trim().Equals(headingName.Trim())).sprite;
        LeanTween.scale(image.gameObject, Vector3.one , 1f).setEaseInBounce();
        Invoke(nameof(hide),2f);
    }

    void hide() { gameObject.SetActive(false); }
}

[Serializable]
public class SpecialHeadingsData 
{
    public string headingName;
    public Sprite sprite;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIScene_LoadingScreen : MonoBehaviour
{
    public string[] loadingMsgs = { "Setting up the table. . ." , "Loading Environment. . ."};

    public Image loadingImage;
    public Text loadingText;

    private void OnEnable()
    {
        loadingImage.fillAmount = 0;
        StartCoroutine(_Load());
        StartCoroutine(_ShowingText());
    }


    private IEnumerator _ShowingText()
    {
        for (int i = 0; i < loadingMsgs.Length; i++)
        {
            loadingText.text = "";
            int j = 0;
            while (j<=loadingMsgs[i].Length-1)
            {
                loadingText.text += loadingMsgs[i][j];
                yield return new WaitForSeconds(0.1f);
                j++;
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator _Load()
    {
        yield return new WaitForSeconds(0.5f);
        while (loadingImage.fillAmount < 1)
        {
            loadingImage.fillAmount += 0.001f;
            yield return null;
        }
        GridManager.ResetStaticFields();
        SceneManager.LoadScene(Global.GameScene);
    }
}
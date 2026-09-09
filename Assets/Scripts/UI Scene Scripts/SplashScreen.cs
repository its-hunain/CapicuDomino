using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetBuilder;
using AvatarBuilder;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using static GameRulesManager;

public class SplashScreen : MonoBehaviour
{
    public bool isAnimating = true;
    public float waitTime;
    public Image loadingImage;
    public Text loadingTxt;
    public static SplashScreen instance;
    public static bool FirstLoading = true;

    private void OnEnable()
    {
        isAnimating = true;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        if (!FirstLoading)
            gameObject.SetActive(false);
        FirstLoading = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAnimating && loadingImage != null)
        {
            //fill in waitTime seconds
            loadingImage.fillAmount += 1.0f / waitTime * Time.deltaTime;
            loadingTxt.text = ((int)(loadingImage.fillAmount * 100)).ToString()+"%";
            if (loadingImage.fillAmount >=1.0f)
            {
                isAnimating = false;
                gameObject.SetActive(false); 
            }
        }
    }


    //public void OnLoadingCompleted()
    //{
    //    StartCoroutine(_OnLoadingCompleted());
    //}
    //public IEnumerator _OnLoadingCompleted()
    //{
    //    yield return new WaitUntil(()=> !isAnimating);

    //    string SceneName = currentSelectedGame_GameType.Equals(GameType.Tournament) ? Global.GameScene : Global.UIScene;
    //    Debug.Log("Scene Loading: " + SceneName);
    //    SceneManager.LoadScene(SceneName);
    //}
}
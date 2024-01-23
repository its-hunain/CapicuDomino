using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace AvatarBuilder
{
    public class AvatarConfigurator : MonoBehaviour
    {
        public const string dummyJsonBaseURL = "AvatarBuilder/Json";
        public Gender currentSelectedGender = Gender.male;
        public static AvatarConfigurator instance;
        
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);

            }
            else
            {
                Destroy(gameObject);
            }
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => !string.IsNullOrEmpty(Global.GetBearerToken) && !string.IsNullOrEmpty(WebServiceManager.baseURL));
            WebServiceManager.instance.APIRequest(WebServiceManager.instance.getSetUserAvatarSpecs, Method.GET, null, null, AvatarScreenManager.instance.OnSuccessGetAvatarSpecs, AvatarScreenManager.instance.OnFailGetAvatarSpecs, CACHEABLE.NULL, true, null);
        }

        #region Download Categories 
        public static void CategoriesDownloaderAsynchrously(string url = null)
        {
            //Load Dummy Data From Json
            if (url == null)
            {
                var loaded_text_file = Resources.Load(dummyJsonBaseURL + "/" + instance.currentSelectedGender.ToString() + "Categories") as TextAsset;
                var getCatagories = GetCatagories.FromJson(loaded_text_file.text);
                AvatarScreenManager.instance.SpawnCategories(getCatagories.Data);
            }
            else
            {
                Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
                keyValuePairs.Add("gender", (int)instance.currentSelectedGender);
                WebServiceManager.instance.APIRequest(url, Method.GET, null, keyValuePairs, instance.OnSuccessfullyCategoriesDownload, instance.OnFailDownload, CACHEABLE.NULL, true, null);
                //instance.StartCoroutine(_ItemsDownloaderAsynchrously(Callback , dummyJsonBaseURL+"categories"));
            }
        }

        public void OnSuccessfullyCategoriesDownload(Newtonsoft.Json.Linq.JObject keyValuePairs, long OnSuccess)
        {
            var getCatagories = GetCatagories.FromJson(keyValuePairs.ToString());
            if (getCatagories.Data != null)
            {
                AvatarScreenManager.instance.SpawnCategories(getCatagories.Data);
            }
            else
            {
                Debug.LogError("Data is Null: " + getCatagories.ToString());
            }
        }
        #endregion Categories End

        #region Download  Items 
        public static void ItemsDownloaderAsynchrously(Action<ItemDataJson> Callback = null, string categoryName = null, string url = null)
        {
            if (url == null)
            {
                var loaded_text_file = Resources.Load(dummyJsonBaseURL + "/" + categoryName) as TextAsset;
                var getItems = ItemThumbnailJson.FromJson(loaded_text_file.text);
                Callback(getItems.Data.Data);
            }
            else
            {
                WebServiceManager.instance.APIRequest(url, Method.GET, null, null, instance.OnSuccessfullyItemsDownload, instance.OnFailDownload, CACHEABLE.NULL, true, null);
            }
        }

        public void OnSuccessfullyItemsDownload(Newtonsoft.Json.Linq.JObject keyValuePairs, long OnSuccess)
        {
            Debug.Log("ONSUCCESS: " + OnSuccess);
            var getItems = ItemThumbnailJson.FromJson(keyValuePairs.ToString());
            AvatarScreenManager.instance.categorySelectionPanel.currentSelectedCategoryThumbnail.FillCategoryItems(getItems.Data.Data);
        }

        public void OnFailDownload(string OnFail)
        {
            Debug.LogError("OnFail: " + OnFail);
        }

        #endregion Items End

        #region Thumbnail Downloader
        public static void ThumbnailDownloaderAsynchrously(Action<object> Callback, string url = null)
        {
            if (!url.Contains("http"))
            {
                Debug.Log("URL: " + url);
                var sprite = Resources.Load<Sprite>(url);
                Callback(sprite);
            }
            else
            {
                instance.StartCoroutine(_ThumbnailDownloaderAsynchrously(Callback, url));
            }
        }

        public static IEnumerator _ThumbnailDownloaderAsynchrously(Action<object> Callback, string url)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            Debug.Log("Downloading Icon. . . ");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture2D texture2D = ((DownloadHandlerTexture)www.downloadHandler).texture;
                Sprite sprite = Sprite.Create(texture2D, new Rect(0.0f, 0.0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100.0f);

                if (sprite != null) Callback(sprite);
                else
                    Debug.Log("Some Error in Downloading Thumbnail. . . \n URL: " + url);

                Debug.Log("Downloaded. . . ");
            }
        }
        #endregion
    }

    public enum Gender
    {
        female,
        male
    }

    public enum ItemType
    {
        blend,
        mesh,
        texture,
        color
    }

    public enum CategoryType
    {
        skin,                   //color
        faceCut,                //blend
        eyeBrows,               //texture and color
        eyes,                   //blend and color
        hair,                   //meshindex and color
        nose,                   //blend
        lips,                   //blend and color
        facialHair,             //texture and color
        none
    }
}
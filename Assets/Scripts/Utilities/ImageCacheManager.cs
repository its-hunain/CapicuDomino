using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ImageCacheManager : MonoBehaviour
{
    public static ImageCacheManager instance;
    public List<Sprite> randomAvatars; 
    private string CACHE_IMAGE_FOLDER = "/CacheImages";
    private string fileStorage;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        fileStorage = Application.persistentDataPath;
    }

    public void CheckOrDownloadImage(string url, Image currentImage = null, Action<Texture2D> onCallBack = null)
    {


        ////Jugar
        //if (url.Contains("graph.facebook.com") || url.Contains("googleapis.com"))
        //{
        //    StartCoroutine(DownloadImage(url, currentImage));
        //    return;
        //}
        //else if (!url.Contains("http"))
        //{
        //    //      url = GamePlayManager.instance.config.baseUrl + url;
        //}

        //--
          if (!IsCacheImageExist(url))
          {
              //Debug.Log("DownLoad Image " + url);
              StartCoroutine(DownloadImage(url, currentImage, onCallBack));
                //StartCoroutine(DownloadImage(url, currentImage, onCallBack));//will remove this line


          }
          else
          {
            //Debug.Log("Load Image " + url);
            if (currentImage != null)
            {
                currentImage.sprite = SpriteReturnFromLink(url);
            }


            if (onCallBack != null)
            {
                  onCallBack.Invoke((Texture2D)SpriteReturnFromLink(url).texture);
            }
          }
        //to here


    }

    public Sprite SpriteReturnFromLink(string url)
    {
        //if (!string.IsNullOrEmpty(url))
        //{
            Texture2D texture2D = loadImageFromCache(url);
            return Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0, 0));
        //}
        //else
        //    return null;
    }

    IEnumerator DownloadImage(string url,Image image = null, Action<Texture2D> onCallBack = null)
    {
        //WWW www = new WWW(imageURL);
        if (!string.IsNullOrEmpty(url))
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
           
            //Debug.Log("Download Image Started" + url);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("DownloadFailed");

                if (onCallBack != null)
                    onCallBack.Invoke(null);
            }
            else
            {
                //Debug.Log("ImageDownloaded");
                Texture2D tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
                if (image != null)
                    image.sprite = sprite;
                CacheImage(www, url);

                if (onCallBack != null)
                    onCallBack.Invoke(tex);

            }
        }

    }



    public void CacheImage(UnityWebRequest www, string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            if (!Directory.Exists(fileStorage + CACHE_IMAGE_FOLDER))
            {
                Directory.CreateDirectory(Application.persistentDataPath + CACHE_IMAGE_FOLDER);
            }

            string[] urlArray = url.Split('/');
            string fileName = urlArray[urlArray.Length - 1];

            string path = fileStorage + CACHE_IMAGE_FOLDER + Path.DirectorySeparatorChar + fileName;
            byte[] downloadedBytes = www.downloadHandler.data;
            System.IO.File.WriteAllBytes(path, downloadedBytes);
        }
    }
   public void DeleteImageCache(string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            string[] urlArray = url.Split('/');
            string fileName = urlArray[urlArray.Length - 1];
            //print("File Path " + Application.persistentDataPath + CACHE_IMAGE_FOLDER + "/" + fileName);
            if (IsCacheImageExist(fileName))
                File.Delete(Application.persistentDataPath + CACHE_IMAGE_FOLDER + "/" + fileName);
            else
                print("Image Not Found" + fileName);
        }
    }
  


   /// <summary>
   /// Naem Overload for Native Gallery
   /// </summary>
   /// <param name="imagePickerBytes"></param>
   /// <param name="url"></param>
    public void CacheImage(byte[] imagePickerBytes, string url)
    {
        if (!Directory.Exists(fileStorage + CACHE_IMAGE_FOLDER))
        {
            Directory.CreateDirectory(Application.persistentDataPath + CACHE_IMAGE_FOLDER);
        }

        string[] urlArray = url.Split('/');
        string fileName = urlArray[urlArray.Length - 1];

        string path = fileStorage + CACHE_IMAGE_FOLDER + Path.DirectorySeparatorChar + fileName;
        System.IO.File.WriteAllBytes(path, imagePickerBytes);
    }

    public Texture2D loadImageFromCache(string url)
    {
        string[] urlArray = url.Split('/');
        
        string fileName = urlArray[urlArray.Length - 1];
       
        string path = fileStorage + CACHE_IMAGE_FOLDER + Path.DirectorySeparatorChar + fileName;
        //print("File Name " + path);
        byte[] bytes = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(100, 100, TextureFormat.RGBA32, false);
        texture.LoadImage(bytes);

        return texture;
    }

    public bool IsCacheImageExist(string url)
    {
        if (url != null)
        {
            string[] urlArray = url.Split('/');
            string fileName = urlArray[urlArray.Length - 1];
            FileInfo info = new FileInfo(fileStorage + CACHE_IMAGE_FOLDER + Path.DirectorySeparatorChar + fileName);
            //print("FilePath: " + fileStorage + CACHE_IMAGE_FOLDER + Path.DirectorySeparatorChar + fileName +"Exist"+ info.Exists);
            return info.Exists;
        }
        else
        {
            return false;
        }
    }
    public void DeleteImageCacheFolder()
    {
        try
        {
            //print("FilePath: " + fileStorage + CACHE_IMAGE_FOLDER);
            Directory.Delete(fileStorage + CACHE_IMAGE_FOLDER, true);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }


    public void DownloadPlayerImage(string url,string shortcode)
    {
        StartCoroutine(DownloadAvatarImage(url,shortcode));
    }
    IEnumerator DownloadAvatarImage(string url,string shortcode)
    {
        //WWW www = new WWW(imageURL);
        if (!string.IsNullOrEmpty(url))
        {
         //   if (!url.Contains("http"))
         //       url = GamePlayManager.instance.config.baseUrl + url;

            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);

            Debug.Log("Download Image Started" + url);
            yield return www.SendWebRequest();
            Debug.LogError("Image  Url "+url);
            

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("DownloadFailed");
            }
            else
            {
                Debug.Log("ImageDownloaded");
                Texture2D tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
               
            }
        }

    }
}

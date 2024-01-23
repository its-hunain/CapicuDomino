using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingLoader : MonoBehaviour
{
    public static WaitingLoader instance;
    public Transform loaderTransform;
    public Transform assetLoader;
    private Vector3 rotationEuler;
    public bool onShot;

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
        if (assetLoader) assetLoader.parent.gameObject.SetActive(false);
        loaderTransform.parent.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (loaderTransform.gameObject.activeInHierarchy)
        {
            rotationEuler -= Vector3.forward * 180 * Time.deltaTime; //increment 30 degrees every second
            loaderTransform.rotation = Quaternion.Euler(rotationEuler);
        }
        else
        {
            rotationEuler -= Vector3.forward * 180 * Time.deltaTime; //increment 30 degrees every second
            if (assetLoader) assetLoader.rotation = Quaternion.Euler(rotationEuler);
        }
    }

    internal void ShowHide(bool doShow = false)
    {
        if(!doShow) Debug.Log("Hide Loader");
        if (onShot == false)
        {
            gameObject.SetActive(doShow);
            loaderTransform.parent.gameObject.SetActive(doShow);
            assetLoader.parent.gameObject.SetActive(false);
        }
    }

    internal void ShowHideAssetsLoader(bool doShow = false)
    {
        if (onShot == false)
        {   
            gameObject.SetActive(doShow);
            if (assetLoader) assetLoader.parent.gameObject.SetActive(doShow);
        }
    }

    internal void ShowHideTillTextureUpdate(bool doShow = false)
    {
            print("Calling Default Texture");
            onShot = doShow;
            gameObject.SetActive(doShow);
            loaderTransform.parent.gameObject.SetActive(doShow);
    }


}
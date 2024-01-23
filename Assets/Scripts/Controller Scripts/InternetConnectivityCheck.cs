using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class InternetConnectivityCheck : MonoBehaviour
{
    public GameObject waitingLoader;
    public Transform rotator;

    private Vector3 rotationEuler;

    void Awake()
    {
        waitingLoader.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckInternetConnection());
    }

    // Update is called once per frame
    void Update()
    {
        if (waitingLoader.gameObject.activeInHierarchy)
        {
            rotationEuler -= Vector3.forward * 180 * Time.deltaTime; //increment 30 degrees every second
            rotator.rotation = Quaternion.Euler(rotationEuler);
        }
    }

    IEnumerator CheckInternetConnection()
    {
        yield return new WaitForSeconds(2f);
        //WebServiceManager.instance.APIRequest(WebServiceManager.instance.checkInternetConnectivity, Method.GET, null, null, OnSuccess, OnFail, CACHEABLE.NULL, false, null);

        UnityWebRequest request = UnityWebRequest.Get(WebServiceManager.baseURL +  WebServiceManager.instance.checkInternetConnectivity);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Internet connection available");
            OnSuccess();
        }
        else
        {
            Debug.Log("No internet connection");
            OnFail();
        }

    }



    private void OnFail()
    {
        waitingLoader.SetActive(true);
        Start();
    }

    private void OnSuccess()
    {
        waitingLoader.SetActive(false);
        Start();
    }
}

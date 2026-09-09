using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class InternetConnectivityCheck : MonoBehaviour
{
    public GameObject waitingLoader;
    public Transform rotator;

    private Vector3 rotationEuler;
    public static InternetConnectivityCheck instance;

    private bool hasLoadedScene = false; // ✅ Flag to prevent multiple scene loads

    void Awake()
    {
        waitingLoader.SetActive(false);
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

    void Start()
    {
        StartCoroutine(CheckInternetConnection());
    }

    void Update()
    {
        if (waitingLoader.gameObject.activeInHierarchy)
        {
            rotationEuler -= Vector3.forward * 180 * Time.deltaTime;
            rotator.rotation = Quaternion.Euler(rotationEuler);
        }
    }

    IEnumerator CheckInternetConnection()
    {
        yield return new WaitForSeconds(2f);

        UnityWebRequest request = UnityWebRequest.Get(WebServiceManager.baseURL + WebServiceManager.instance.checkInternetConnectivity);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
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
        StartCoroutine(CheckInternetConnection());
    }

    private void OnSuccess()
    {
        waitingLoader.SetActive(false);

        if (!hasLoadedScene)
        {
            hasLoadedScene = true; // ✅ Set flag to prevent future loads
            SceneManager.LoadScene("UI_Scene");
        }
    }
}

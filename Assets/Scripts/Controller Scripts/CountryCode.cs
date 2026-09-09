using UnityEngine;
using System.Collections;
using System.Net;

public class CountryCode : MonoBehaviour
{
    public static string code;

    private void Awake()
    {        
        StartCoroutine(GetCountryCode());
    }

    IEnumerator GetCountryCode()
    {
        // Make a web request to ipinfo.io to get geolocation information
        using (WebClient client = new WebClient())
        {
            string response = client.DownloadString("https://ipinfo.io/country");

            code = response;
            // The response should contain the country code
            Debug.Log("Country Code: " + response);
        }

        yield return null;
    }
}

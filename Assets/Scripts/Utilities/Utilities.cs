using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class Utilities
{
    public static string DecodeState(byte[] state)
    {
        return Encoding.UTF8.GetString(state);
    }
    public static float GetTimeDiffInSeconds(double currentActionTime)
    {
        var timeDiffInMilliseconds = Epoch.Now - currentActionTime;
        return (float)timeDiffInMilliseconds / 1000;
    }
    public static string Base64Encode(string text)
    {
        byte[] bytesToEncode = Encoding.UTF8.GetBytes(text);
        return Convert.ToBase64String(bytesToEncode);
    }

    public static string Base64Decode(string encodedText)
    {
        byte[] decodedBytes = Convert.FromBase64String(encodedText);
        return Encoding.UTF8.GetString(decodedBytes);
    }
    public static string randomKey(double v)
    {
        string k = "";
        double x = Math.Floor(v);

        double j = 1;
        double a = 4;
        double r = 9;

        for (int i = 0; i < 6; i++)
        {
            x = ((a * x) + j) % r;

            k = k + Math.Floor(x);
        }

        return k;
    }

    /// <summary>
    /// Converts a string into a SHA-256 Hash Vaue
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string GetSHA256(string text)
    {
        byte[] textToBytes = Encoding.UTF8.GetBytes(text);

        SHA256Managed mySHA256 = new SHA256Managed();

        byte[] hashValue = mySHA256.ComputeHash(textToBytes);

        return GetHexStringFromHash(hashValue);
    }
    public static string GetHexStringFromHash(byte[] hashValue)
    {
        string hexString = String.Empty;

        foreach (byte b in hashValue)
        {
            hexString += b.ToString("x2");
        }

        return hexString;
    }
    public static string calculateHashKey()
    {
        int mi = 48;
        int mx = 122;
        int x = 1;
        int a = 4;
        int j = 1;
        int c = 32;

        int r = (mx - mi);

        string k = "";

        for (int i = 0; i < c; i++)
        {
            x = ((((a * x) + j) ^ 10) % r) + mi;
            k = k + (char)x;
        }

        return k;
    }
}

[System.Serializable]
public class HashedData
{
    public string data;
    public string hash;
    public string timeStamp;

    public HashedData(string jsonData, double dateTime, string hashEncodedKey, string matchId)
    {
        string encodedData = jsonData; // Utilities.Base64Encode(jsonData);
        string hashKey = Utilities.Base64Decode(hashEncodedKey);

        string randomKey = Utilities.randomKey(dateTime);

        this.data = encodedData;
        this.hash = Utilities.GetSHA256(hashKey + matchId + encodedData + randomKey);

        this.timeStamp = dateTime.ToString();
    }

    public string decodeData()
    {
        return this.data; // Utilities.Base64Decode(this.data);
    }

    public bool isValid(string hashEncodedKey, double dateTime, string matchId)
    {
        //string hashKey = Utilities.Base64Decode(hashEncodedKey);

        string randomKey = Utilities.randomKey(dateTime);
        string cHash = Utilities.GetSHA256(hashEncodedKey + matchId + this.data + randomKey);

        return cHash == hash;
    }
}
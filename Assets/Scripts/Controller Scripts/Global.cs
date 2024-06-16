using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class Global 
{
    private static string authToken;
    private static string bearerToken = "";

    //public static string testToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI2M2JkNGNiZDczNDVjYmU1YzZmZjNhNTEiLCJpYXQiOjE2ODMwOTk4Nzd9.vR2MRnyCzylyma7XMj8vTx9LsF1OfAoMUwrey8mBzv0"; //PreProd
    //public static string testToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI2M2JkNGNiZDczNDVjYmU1YzZmZjNhNTEiLCJpYXQiOjE2ODMwOTk4Nzd9.vR2MRnyCzylyma7XMj8vTx9LsF1OfAoMUwrey8mBzv0"; //Dev  
    //public static string testToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI2M2FkNGFmOThkMWNjZmM5OWRiMGNjYjgiLCJpYXQiOjE2OTEwNTc5NDd9.oQS7LpIARieBruAsbyIh4U-3lNCf6RUpg3P5-FVh040"; //Pre Ali Hammad  
    public static string testToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI2M2JkNGNiZDczNDVjYmU1YzZmZjNhNTEiLCJpYXQiOjE2ODcyNjkwMTZ9.xbRogv37Y9XPZnYtdoP1BDk0woR3tpSZzzLEAIlb_hY"; //Pre
    public static string tournamentID = "";//63fb4a5e587ccc9ed151e5cd //From Base URL
    public static string gameType = "";

    public const string AvatarBuilderScene = "AvatarConfigurator";
    public const string SplashScene = "SplashScene";
    public const string AssetBuilderScene = "AssetBuilder";
    public const string UIScene = "UI_Scene";
    public const string GameScene = "BotScene";
    public const string CustomizeAvatarScene = "InventoryBuilder";
    public static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore,
        MissingMemberHandling = MissingMemberHandling.Ignore
    };


    public static JsonLoadSettings jsonLoadSettings = new JsonLoadSettings
    {
        //DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Ignore,
        LineInfoHandling = LineInfoHandling.Ignore,
        CommentHandling = CommentHandling.Ignore
    };


    
    public static void panelTransition(GameObject go,Action onComplete = null)
    {
        go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y + 150, go.transform.localPosition.z);
        LeanTween.moveLocalY(go, 0, 0.5f).setOnComplete(onComplete).setEase(LeanTweenType.easeOutBack);
    }


    public static void popupTransition(GameObject go, Action onComplete = null)
    {
        go.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        LeanTween.scale(go, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBack).setOnComplete(onComplete);
    }


    public static string GetAuthToken
    {
        get
        {
            return authToken;
        }
        set
        {
            authToken = value;
        }
    }

    public static string GetBearerToken
    {
        get
        {
            return bearerToken;
        }
        set
        {
            bearerToken = value;
        }
    }



    public const string UserID = "UserId";
    public const string UserEmail = "UserEmail";
    public const string UserPassword = "UserPassword";
    public const string UserName = "UserName";
    public const string AuthProvider = "AuthProvider";


    public const string Apple       = "Apple";
    public const string Custom      = "Custom";
    public const string Facebook    = "Facebook";
    public const string Guest       = "Guest";


    public const string hasSessionKey = "hasSession";
}


namespace TileTheme
{
    public static class ColorUtility
    {
        public static Color HexToColor(string hex)
        {
            // Remove the '#' character if present
            if (hex.StartsWith("#"))
            {
                hex = hex.Substring(1);
            }

            // Ensure the hex string is valid (6 or 8 characters long)
            if (hex.Length != 6 && hex.Length != 8)
            {
                Debug.LogError("Invalid hex string length. Must be 6 or 8 characters long.");
                return Color.white; // Return a default color
            }

            // Parse the hex string into RGB(A) components
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            byte a = 255; // Default to opaque if no alpha value is provided

            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            }

            // Convert to a Color object
            return new Color32(r, g, b, a);
        }


        public static Color UpdateTileTheme(string theme)
        {
            Color color = Color.white;

            if (theme == "TileTheme1")
            {
                color = Color.white;
            }

            else if (theme == "TileTheme2")
            {
                //color = new Color(235, 95, 165, 255);
                color = HexToColor("#EB5FA5FF");
            }
            else if (theme == "TileTheme3")
            {
                color = HexToColor("#7F8EF3FF");

                //color = Color.yellow;// new Color(127, 142, 243, 255);
            }
            else if (theme == "TileTheme4")
            {
                color = HexToColor("#9D33EEFF");

                //color = new Color(157, 51, 238, 255);
            }
            else
            {
                color = Color.white;
            }

            Debug.Log("theme: " + theme);
            return color;
        }
    }
}
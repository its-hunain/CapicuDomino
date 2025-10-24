using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdMobManager : MonoBehaviour
{
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern int AppTrackingTransparencyStatus();

    [DllImport("__Internal")]
    private static extern void RequestAppTrackingTransparency();
#endif

    public static AdMobManager instance;

    [Header("Ad Unit IDs - Android")]
    [SerializeField] private string androidInterstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712"; // Test ID
    [SerializeField] private string androidRewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917"; // Test ID

    [Header("Ad Unit IDs - iOS")]
    [SerializeField] private string iosInterstitialAdUnitId = "ca-app-pub-3940256099942544/4411468910"; // Test ID
    [SerializeField] private string iosRewardedAdUnitId = "ca-app-pub-3940256099942544/1712485313"; // Test ID

    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    private Action onRewardedAdCompleted;
    private Action onRewardedAdFailed;

    private bool isInterstitialLoaded = false;
    private bool isRewardedLoaded = false;

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
            return;
        }
    }

    void Start()
    {
        StartCoroutine(RequestTrackingPermissionAndInitialize());
    }

    private IEnumerator RequestTrackingPermissionAndInitialize()
    {
#if UNITY_IOS && !UNITY_EDITOR
        // Wait for ATT dialog on iOS 14.5+
        int status = AppTrackingTransparencyStatus();
        if (status == 0) // Not determined
        {
            Debug.Log("Requesting App Tracking Transparency permission...");
            RequestAppTrackingTransparency();

            // Wait for user response
            yield return new WaitForSeconds(2f);
            status = AppTrackingTransparencyStatus();
        }

        Debug.Log($"ATT Status: {status} (0=NotDetermined, 1=Restricted, 2=Denied, 3=Authorized)");
#endif
        yield return null;
        InitializeAdMob();
    }

    private void InitializeAdMob()
    {
        Debug.Log("Initializing Google Mobile Ads SDK...");
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("Google Mobile Ads SDK initialized successfully.");
            LoadInterstitialAd();
            LoadRewardedAd();
        });
    }

    #region Interstitial Ad

    private string GetInterstitialAdUnitId()
    {
#if UNITY_ANDROID
        return androidInterstitialAdUnitId;
#elif UNITY_IOS
        return iosInterstitialAdUnitId;
#else
        return "unexpected_platform";
#endif
    }

    public void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        Debug.Log("Loading interstitial ad...");

        var adRequest = new AdRequest();

        InterstitialAd.Load(GetInterstitialAdUnitId(), adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Interstitial ad failed to load: " + error);
                isInterstitialLoaded = false;
                return;
            }

            Debug.Log("Interstitial ad loaded successfully.");
            interstitialAd = ad;
            isInterstitialLoaded = true;

            // Register for ad events
            RegisterInterstitialAdEvents(ad);
        });
    }

    private void RegisterInterstitialAdEvents(InterstitialAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");
            isInterstitialLoaded = false;
            LoadInterstitialAd(); // Load next ad
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to show: " + error);
            isInterstitialLoaded = false;
            LoadInterstitialAd(); // Load next ad
        };
    }

    public void ShowInterstitialAd()
    {
        if (interstitialAd != null && isInterstitialLoaded)
        {
            Debug.Log("Showing interstitial ad.");
            interstitialAd.Show();
        }
        else
        {
            Debug.LogWarning("Interstitial ad is not ready yet.");
            LoadInterstitialAd(); // Try to load if not ready
        }
    }

    public bool IsInterstitialAdReady()
    {
        return interstitialAd != null && isInterstitialLoaded;
    }

    #endregion

    #region Rewarded Ad

    private string GetRewardedAdUnitId()
    {
#if UNITY_ANDROID
        return androidRewardedAdUnitId;
#elif UNITY_IOS
        return iosRewardedAdUnitId;
#else
        return "unexpected_platform";
#endif
    }

    public void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        Debug.Log("Loading rewarded ad...");

        var adRequest = new AdRequest();

        RewardedAd.Load(GetRewardedAdUnitId(), adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Rewarded ad failed to load: " + error);
                isRewardedLoaded = false;
                return;
            }

            Debug.Log("Rewarded ad loaded successfully.");
            rewardedAd = ad;
            isRewardedLoaded = true;

            // Register for ad events
            RegisterRewardedAdEvents(ad);
        });
    }

    private void RegisterRewardedAdEvents(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
            isRewardedLoaded = false;
            LoadRewardedAd(); // Load next ad
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to show: " + error);
            isRewardedLoaded = false;
            onRewardedAdFailed?.Invoke();
            LoadRewardedAd(); // Load next ad
        };
    }

    public void ShowRewardedAd(Action onCompleted, Action onFailed = null)
    {
        if (rewardedAd != null && isRewardedLoaded)
        {
            onRewardedAdCompleted = onCompleted;
            onRewardedAdFailed = onFailed;

            Debug.Log("Showing rewarded ad.");
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"Rewarded ad granted reward: {reward.Amount} {reward.Type}");
                onRewardedAdCompleted?.Invoke();
            });
        }
        else
        {
            Debug.LogWarning("Rewarded ad is not ready yet.");
            onFailed?.Invoke();
            LoadRewardedAd(); // Try to load if not ready
        }
    }

    public bool IsRewardedAdReady()
    {
        return rewardedAd != null && isRewardedLoaded;
    }

    #endregion

    private void OnDestroy()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
        }
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
        }
    }
}

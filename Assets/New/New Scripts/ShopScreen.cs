using Dominos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShopScreen : MonoBehaviour
{
    public Button coinsBtn;
    public Button tilesBtn;
    public Button tablesBtn;
    public Button backBtn;
    public Button settingsBtn;
    public Button watchAdForCoinsBtn; // New button for watching ad to earn 50 coins

    public Text coinsText;

    public GameObject coinsScroll;
    public GameObject tilesScroll;
    public GameObject tablesScroll;

    public Image selectionImage;

    public List<ShopItemBtn> shopBtns;


    [SerializeField]
    public FetchOwnShopItem ownedProducts;// = new List<BuyShopItem>();

    private System.DateTime? lastAdWatchDateTime = null; // Stores actual DateTime instead of Time.time
    private const float AD_COOLDOWN = 30f; // 5 minutes cooldown between ads
    private const string LAST_AD_WATCH_TIME_KEY = "ShopAdLastWatchTime";

    // Start is called before the first frame update
    void Start()
    {
        coinsText.text= PlayerPersonalData.playerDomiCoins.ToString();

        // Load the last ad watch time from PlayerPrefs
        LoadLastAdWatchTime();

        GetOwnedProds();
        coinsBtn.onClick.AddListener(() => SwitchPanels(coinsBtn,coinsScroll));
        tilesBtn.onClick.AddListener(() => SwitchPanels(tilesBtn,tilesScroll));
        tablesBtn.onClick.AddListener(() => SwitchPanels(tablesBtn,tablesScroll));

        backBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.shopScreen.gameObject, false));
        settingsBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.settingScreen.gameObject, true));

        // Setup watch ad button
        if (watchAdForCoinsBtn != null)
        {
            watchAdForCoinsBtn.onClick.AddListener(() => WatchAdForCoins());
            UpdateWatchAdButton();
        }

        foreach (var item in shopBtns)
        {
            if (ownedProducts.myProducts.Exists(x => x.productId == item.productId))
                item.isBought = true;
        }
    }

    private void LoadLastAdWatchTime()
    {
        Debug.Log("=====================================");
        Debug.Log($"[LoadAdTime] Checking PlayerPrefs key: {LAST_AD_WATCH_TIME_KEY}");
        bool hasKey = PlayerPrefs.HasKey(LAST_AD_WATCH_TIME_KEY);
        Debug.Log($"[LoadAdTime] Key exists: {hasKey}");

        if (hasKey)
        {
            string savedTimeStr = PlayerPrefs.GetString(LAST_AD_WATCH_TIME_KEY);
            Debug.Log($"[LoadAdTime] Retrieved saved time string: {savedTimeStr}");

            long savedTimeTicks;
            if (long.TryParse(savedTimeStr, out savedTimeTicks))
            {
                lastAdWatchDateTime = new System.DateTime(savedTimeTicks);
                System.DateTime currentTime = System.DateTime.Now;

                Debug.Log($"[LoadAdTime] Saved DateTime: {lastAdWatchDateTime}");
                Debug.Log($"[LoadAdTime] Current DateTime: {currentTime}");

                // Calculate how much time has passed since the ad was watched
                System.TimeSpan timePassed = currentTime - lastAdWatchDateTime.Value;
                float secondsPassed = (float)timePassed.TotalSeconds;

                Debug.Log($"[LoadAdTime] Seconds passed: {secondsPassed}");
                Debug.Log($"[LoadAdTime] Cooldown duration: {AD_COOLDOWN}");

                if (secondsPassed < AD_COOLDOWN)
                {
                    float remainingCooldown = AD_COOLDOWN - secondsPassed;
                    Debug.Log($"[LoadAdTime] *** COOLDOWN ACTIVE *** {remainingCooldown} seconds remaining");
                }
                else
                {
                    Debug.Log("[LoadAdTime] Cooldown expired, ready to watch");
                }
            }
            else
            {
                Debug.LogError($"[LoadAdTime] Failed to parse ticks: {savedTimeStr}");
                lastAdWatchDateTime = null;
            }
        }
        else
        {
            lastAdWatchDateTime = null;
            Debug.LogWarning("[LoadAdTime] *** NO PREVIOUS AD WATCH TIME FOUND ***");
            Debug.LogWarning("[LoadAdTime] This means either it's the first time or PlayerPrefs was cleared");
        }

        Debug.Log("=====================================");
    }

    [ContextMenu("Test - Print All PlayerPrefs")]
    void TestPrintPlayerPrefs()
    {
        Debug.Log("=== TESTING PLAYERPREFS ===");
        Debug.Log($"Has Key '{LAST_AD_WATCH_TIME_KEY}': {PlayerPrefs.HasKey(LAST_AD_WATCH_TIME_KEY)}");
        if (PlayerPrefs.HasKey(LAST_AD_WATCH_TIME_KEY))
        {
            Debug.Log($"Value: {PlayerPrefs.GetString(LAST_AD_WATCH_TIME_KEY)}");
        }
    }

    [ContextMenu("Test - Force Save Test Time")]
    void TestForceSaveTime()
    {
        Debug.Log("=== FORCING TEST SAVE ===");
        SaveLastAdWatchTime();
        Debug.Log("Test save complete. Check logs above.");
    }

    [ContextMenu("Test - Clear Ad Cooldown")]
    void TestClearAdCooldown()
    {
        Debug.Log("=== CLEARING AD COOLDOWN ===");
        PlayerPrefs.DeleteKey(LAST_AD_WATCH_TIME_KEY);
        PlayerPrefs.Save();
        lastAdWatchDateTime = null;
        UpdateWatchAdButton();
        Debug.Log("Ad cooldown cleared!");
    }

    private void SaveLastAdWatchTime()
    {
        // Save the current real-world time
        System.DateTime currentTime = System.DateTime.Now;
        string ticksStr = currentTime.Ticks.ToString();
        PlayerPrefs.SetString(LAST_AD_WATCH_TIME_KEY, ticksStr);
        PlayerPrefs.Save();

        Debug.Log($"[SaveAdTime] *** SAVING AD WATCH TIME ***");
        Debug.Log($"[SaveAdTime] DateTime: {currentTime}");
        Debug.Log($"[SaveAdTime] Ticks: {ticksStr}");
        Debug.Log($"[SaveAdTime] Key: {LAST_AD_WATCH_TIME_KEY}");
        Debug.Log($"[SaveAdTime] PlayerPrefs.Save() called");

        // Verify it was saved
        if (PlayerPrefs.HasKey(LAST_AD_WATCH_TIME_KEY))
        {
            string verifyValue = PlayerPrefs.GetString(LAST_AD_WATCH_TIME_KEY);
            Debug.Log($"[SaveAdTime] Verification - Value saved: {verifyValue}");
        }
        else
        {
            Debug.LogError("[SaveAdTime] ERROR: Key not found after save!");
        }
    }

    void Update()
    {
        // Update the watch ad button state periodically
        if (watchAdForCoinsBtn != null && Time.frameCount % 60 == 0) // Check every ~1 second
        {
            UpdateWatchAdButton();
        }
    }

    void SwitchPanels(Button btn, GameObject panel)
    {
        //print("karwa");
        coinsScroll.SetActive(false);
        tilesScroll.SetActive(false);
        tablesScroll.SetActive(false);

        panel.SetActive(true);
        selectionImage.transform.position = btn.transform.position;
    }

    public void ChangeSelectable()
    {
        foreach (var item in shopBtns)
        {
            if (item.selectionFrame)
            {
                item.selectionFrame.gameObject.SetActive(false);
            }
        }
    }
    [ContextMenu("Try")]
    void GetOwnedProds()
    {
        WebServiceManager.instance.APIRequest(WebServiceManager.instance.getProducts, Method.GET, null, null, OnSuccess, OnFail);
    }
    void OnSuccess(string keyValuePairs, long successCode)
    {
        Debug.Log("OnSuccessfullyGetProducts: " + keyValuePairs.ToString());
        ownedProducts = FetchOwnShopItem.FromJson(keyValuePairs.ToString());


        foreach (var item in shopBtns)
        {
            if (ownedProducts.myProducts.Exists(x => x.productId == item.productId))
                item.isBought = true;
        }


    }
    void OnFail(string msg)
    {
        Debug.Log("OnFailGetProducts: " + msg);

    }

    private void WatchAdForCoins()
    {
        // Check cooldown (only if ad was watched before)
        if (lastAdWatchDateTime.HasValue)
        {
            System.TimeSpan timeSinceLastAd = System.DateTime.Now - lastAdWatchDateTime.Value;
            float secondsSinceLastAd = (float)timeSinceLastAd.TotalSeconds;

            if (secondsSinceLastAd < AD_COOLDOWN)
            {
                float remainingTime = AD_COOLDOWN - secondsSinceLastAd;
                int minutes = Mathf.FloorToInt(remainingTime / 60f);
                int seconds = Mathf.FloorToInt(remainingTime % 60f);

                if (MesgBar.instance != null)
                {
                    MesgBar.instance.show($"Please wait {minutes}m {seconds}s before watching another ad.");
                }
                return;
            }
        }

        // Check if ad is ready
        if (AdMobManager.instance != null && AdMobManager.instance.IsRewardedAdReady())
        {
            int rewardAmount = 50; // 50 coins for watching ad in store

            AdMobManager.instance.ShowRewardedAd(
                onCompleted: () =>
                {
                    Debug.Log($"Store: Rewarding player with {rewardAmount} coins");
                    lastAdWatchDateTime = System.DateTime.Now;
                    SaveLastAdWatchTime(); // Save to PlayerPrefs for persistence
                    RewardCoins(rewardAmount);
                    UpdateWatchAdButton();
                },
                onFailed: () =>
                {
                    Debug.LogError("Store: Reward ad failed to show");
                    if (MesgBar.instance != null)
                    {
                        MesgBar.instance.show("Ad is not available right now. Please try again later.");
                    }
                }
            );
        }
        else
        {
            Debug.LogError("Store: Reward ad is not ready");
            if (MesgBar.instance != null)
            {
                MesgBar.instance.show("Ad is not available right now. Please try again later.");
            }
        }
    }

    private void RewardCoins(int amount)
    {
        // Show success message
        if (MesgBar.instance != null)
        {
            MesgBar.instance.show($"You earned {amount} bonus coins" , false);
        }

        // Sync with server (server will add the amount and return new total)
        SyncCoinsWithServer(amount);
    }

    private void SyncCoinsWithServer(int amount)
    {
        // Send the amount to ADD (not the total)
        Dictionary<string, object> postData = new Dictionary<string, object>();
        postData.Add("coins", amount);

        WebServiceManager.instance.APIRequest(
            WebServiceManager.instance.getPlayerProfile,
            Method.POST,
            null,
            postData,
            (data, code) =>
            {
                // Update local coins with server response
                User user = User.FromJson(data.ToString());
                WebServiceManager.instance.playerPersonalData.Data.User.Domicoins = PlayerPersonalData.playerDomiCoins = user.Domicoins;

                // Update UI
                if (coinsText != null)
                {
                    coinsText.text = PlayerPersonalData.playerDomiCoins.ToString();
                }

                // Update menu screen coin text if available
                if (UI_Manager.instance != null && UI_Manager.instance.menuScreen != null)
                {
                    UI_Manager.instance.menuScreen.coinTxt.text = PlayerPersonalData.playerDomiCoins.ToString();
                }
            },
            (msg) => { Debug.LogWarning($"ShopScreen: Failed to sync coins with server: {msg}"); }
        );
    }

    private void UpdateWatchAdButton()
    {
        if (watchAdForCoinsBtn == null) return;

        // Check if ad is ready and cooldown has passed
        bool isAdReady = AdMobManager.instance != null && AdMobManager.instance.IsRewardedAdReady();

        float secondsSinceLastAd = AD_COOLDOWN; // Default to cooldown expired
        bool isCooldownOver = true;

        if (lastAdWatchDateTime.HasValue)
        {
            System.TimeSpan timeSinceLastAd = System.DateTime.Now - lastAdWatchDateTime.Value;
            secondsSinceLastAd = (float)timeSinceLastAd.TotalSeconds;
            isCooldownOver = secondsSinceLastAd >= AD_COOLDOWN;
        }

        //Debug.Log($"[UpdateButton] isAdReady={isAdReady}, lastAdWatchDateTime={lastAdWatchDateTime}, secondsSinceLastAd={secondsSinceLastAd}, isCooldownOver={isCooldownOver}");

        watchAdForCoinsBtn.interactable = isAdReady && isCooldownOver;

        // Update button text
        Text btnText = watchAdForCoinsBtn.GetComponentInChildren<Text>();
        if (btnText != null)
        {
            if (!isCooldownOver)
            {
                float remainingTime = AD_COOLDOWN - secondsSinceLastAd;
                int minutes = Mathf.FloorToInt(remainingTime / 60f);
                int seconds = Mathf.FloorToInt(remainingTime % 60f);
                btnText.text = $"Wait {minutes}:{seconds:00}";
            }
            else if (!isAdReady)
            {
                btnText.text = "Loading Ad...";
            }
            else
            {
                btnText.text = "Watch Ad (+50 Coins)";
            }
        }
    }


}

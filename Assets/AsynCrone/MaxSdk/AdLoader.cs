using System;
using System.Collections;
using AsCrone;
using UnityEngine;

public enum AdsType
{
    Interstial,
    Rewarded,
    Banner
}

public class AdLoader : MonoBehaviour
{
    public bool useBanner;

    void Awake()
    {

        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {
            // Initialize the Adjust SDK inside the AppLovin SDK's initialization callback
        };

        MaxSdk.SetSdkKey("8B6RctQc1Tn2dJ_nXy04fNahqbbZxfSTLCVIFQtf71pqTTqB4LUKSTxB1zX3vbDMQvuIv2_Wcs8eIlJQA_QWqi");
        MaxSdk.SetUserId("USER_ID");
        MaxSdk.InitializeSdk();

        InitializeInterstitialAds();
        InitializeRewardedAds();
        if (useBanner)
        {
            CallBanner();
        }

        StartCoroutine(WaitAndTry());
    }

    IEnumerator WaitAndTry()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            PrepareInterstitial();
        }
    }


    #region INTERSTITIAL

    [SerializeField] string UnitId_Interstitial = "1c6df78ee060781c";
    [SerializeField] int retryAttempInterstitialtLimit = 6;
    int retryAttemptInter;


    public void InitializeInterstitialAds()
    {
        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;

        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;

        // Load the first interstitial
        LoadInterstitial();
    }

    private void OnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        double revenue = adInfo.Revenue;
        Debug.Log($"[Revenu ID :  {adUnitId}]");

        if (adUnitId == UnitId_Interstitial)
        {
            Debug.Log($"[REVENUE INTERSTITIAL ==> {revenue}]");

        }else if (adUnitId == UnitId_Rewarded)
        {
            Debug.Log($"[REVENUE REWARDED ==> {revenue}]");
        }else
        {
            Debug.Log($"[REVENUE REWARDED ==> {revenue}]");

        }

        // Miscellaneous data
        //string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
        //string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        //string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        //string placement = adInfo.Placement; // The placement this ad's postbacks are tied to
        //string networkPlacement = adInfo.NetworkPlacement; // The placement ID from the network that showed the ad
    }



    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(UnitId_Interstitial);
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'
        Debug.Log($"[LOAD SUCCESS!]");
        CroneAPI.OnDataStarted(AdsType.Interstial);
        // Reset retry attempt
        retryAttemptInter = 0;
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)
        //TODO:FAIL
        Debug.Log($"[LOAD FAILED! Network]");

        retryAttemptInter++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttemptInter));

        Invoke("LoadInterstitial", (float)retryDelay);
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
        CroneAPI.OnDataNotReady(AdsType.Interstial);
        Debug.Log($"[LOAD FAILED Display!]");

        LoadInterstitial();
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad.
        LoadInterstitial();


    }


#if UNITY_EDITOR

    /// <summary>
    /// Call interstitial ads on editor
    /// </summary>
    public void CallInterstitial()
    {
        if (MaxSdk.IsInterstitialReady(UnitId_Interstitial))
        {
            MaxSdk.ShowInterstitial(UnitId_Interstitial);
            PrepareInterstitialParalels();
        }
        else
        {
            CroneAPI.OnDataNotReady(AdsType.Interstial);
        }
    }

#elif UNITY_ANDROID

    public void CallInterstitial()
    {
        if (MaxSdkAndroid.IsInterstitialReady(UnitId_Interstitial))
        { 
            MaxSdkAndroid.ShowInterstitial(UnitId_Interstitial);
            PrepareInterstitialParalels();
        }
        else
        {
            AnalyticsTool.BuildDataNotReady(AdsType.Interstial);
        }
    }
#elif UNITY_IPHONE || UNITY_IOS

  public void CallInterstitial()
    {
        if (MaxSdkiOS.IsInterstitialReady(UnitId_Interstitial))
        {
            MaxSdkiOS.ShowInterstitial(UnitId_Interstitial);
            PrepareInterstitialParalels();
        }
        else
        {
            AnalyticsTool.BuildDataNotReady(AdsType.Interstial);
        }
    }

#endif

    /// <summary>
    /// Preparation of ads for instance loading
    /// </summary>
    public void PrepareInterstitialParalels()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            StartCoroutine(ParalelFetch(PrepareInterstitial, retryAttempInterstitialtLimit));
        }
    }

#if UNITY_EDITOR

    private bool PrepareInterstitial()
    {
        if (MaxSdk.IsInterstitialReady(UnitId_Interstitial))
        {
            CallInterstitial();
            return true;
        }
        else
        {
            return false;
        }
    }

#elif UNITY_ANDROID

    private bool PrepareInterstitial()
    {
        if (MaxSdkAndroid.IsInterstitialReady(UnitId_Interstitial))
        {
            CallInterstitial();
            return true;
        }
        else
        {
            return false;
        }
    }
#elif  UNITY_IPHONE || UNITY_IOS

     private bool PrepareInterstitial()
    {
        if (MaxSdkiOS.IsInterstitialReady(UnitId_Interstitial))
        {
            CallInterstitial();
            return true;
        }
        else
        {
            return false;
        }
    }

#endif



    #endregion


    #region REWARDED

    [SerializeField] string UnitId_Rewarded = "643f65da1176a300";
    [SerializeField] int retryAttemptRewardedLimit = 6;
    int retryAttemptRewarded;

    public void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        // Load the first rewarded ad
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(UnitId_Rewarded);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.

        // Reset retry attempt
        retryAttemptRewarded = 0;
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

        retryAttemptRewarded++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttemptRewarded));

        Invoke("LoadRewardedAd", (float)retryDelay);
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        CroneAPI.OnDataNotReady(AdsType.Rewarded);
        LoadRewardedAd();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        LoadRewardedAd();
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // The rewarded ad displayed and the user should receive the reward.
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Ad revenue paid. Use this callback to track user revenue.
        CroneAPI.OnDataSuccess(AdsType.Rewarded);

    }




#if UNITY_EDITOR

    /// <summary>
    /// Call rewarded ads
    /// </summary>
    public void CallRewarded()
    {
        if (MaxSdk.IsRewardedAdReady(UnitId_Rewarded))
        {
            MaxSdk.ShowRewardedAd(UnitId_Rewarded);
            CroneAPI.OnDataStarted(AdsType.Rewarded);
            PrepareRewardedOnParallels();
        }
        else
        {
            CroneAPI.OnDataNotReady(AdsType.Rewarded);
        }
    }

#elif UNITY_ANDROID

    public void CallRewarded()
    {
        if (MaxSdkAndroid.IsRewardedAdReady(UnitId_Rewarded))
        {
            MaxSdkAndroid.ShowRewardedAd(UnitId_Rewarded);
           AnalyticsTool.BuildDataStarted(AdsType.Rewarded);
            PrepareRewardedOnParallels();
        }
        else
        {
            AnalyticsTool.BuildDataNotReady(AdsType.Rewarded);
        }
    }
#elif UNITY_IPHONE || UNITY_IOS

    public void CallRewarded()
    {
        if (MaxSdkiOS.IsRewardedAdReady(UnitId_Rewarded))
        {
            MaxSdkiOS.ShowRewardedAd(UnitId_Rewarded);
           AnalyticsTool.BuildDataStarted(AdsType.Rewarded);
            PrepareRewardedOnParallels();
        }
        else
        {
            AnalyticsTool.BuildDataNotReady(AdsType.Rewarded);
        }
    }

#endif


    /// <summary>
    /// Preparation of ads for instance loading
    /// </summary>
    public void PrepareRewardedOnParallels()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            StartCoroutine(ParalelFetch(PrepareRewarded, retryAttemptRewardedLimit));
        }
    }

#if UNITY_EDITOR

    private bool PrepareRewarded()
    {
        if (MaxSdk.IsInterstitialReady(UnitId_Rewarded))
        {
            CallRewarded();
            return true;
        }
        else
        {
            return false;
        }
    }

#elif UNITY_ANDROID

    private bool PrepareRewarded()
    {
        if (MaxSdkAndroid.IsInterstitialReady(UnitId_Rewarded))
        {
            CallRewarded();
            return true;
        }
        else
        {
            return false;
        }
    }
#elif UNITY_IPHONE || UNITY_IOS

  private bool PrepareRewarded()
    {
        if (MaxSdkiOS.IsInterstitialReady(UnitId_Rewarded))
        {
            CallRewarded();
            return true;
        }
        else
        {
            return false;
        }
    }

#endif


    #endregion


    #region BANNER

    [SerializeField] string UnitId_bannerAd = "dfcdf9c8232f4ca3"; // Retrieve the ID from your account

    public void InitializeBannerAds()
    {
        // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
        // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
        MaxSdk.CreateBanner(UnitId_bannerAd, MaxSdkBase.BannerPosition.BottomCenter);

        // Set background or background color for banners to be fully functional
        MaxSdk.SetBannerBackgroundColor(UnitId_bannerAd, Color.black);
    }
    /// <summary>
    /// Call basic banner ads
    /// </summary>
    public void CallBanner()
    {
        InitializeBannerAds();
        MaxSdk.ShowBanner(UnitId_bannerAd);
        CroneAPI.OnDataSuccess(AdsType.Banner);
    }
    /// <summary>
    /// Hide banner ads
    /// </summary>
    public void HideBanner()
    {
        MaxSdk.HideBanner(UnitId_bannerAd);
    }
    #endregion


    IEnumerator ParalelFetch(Func<bool> func, int attempt = 4)
    {
        int logN = 0;
        double retryDelay = Math.Min(attempt, logN);
        while (true)
        {
            yield return new WaitForSeconds((float)retryDelay);

            if (func())
            {
                break;
            }
            logN++;
        }
    }

}

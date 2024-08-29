using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AdsManager : MonoBehaviour
{
    public UnityEvent onRewardedVideoCompleted;

    void Start()
    {
        Gley.MobileAds.API.Initialize();
        Gley.MobileAds.Events.onInitialized += OnInitialized;
    }

    public void ShowBanner()
    {
        Gley.MobileAds.API.ShowBanner(Gley.MobileAds.BannerPosition.Top, Gley.MobileAds.BannerType.BannerShort);
    }

    public void ShowInterstitial()
    {
        if (Gley.MobileAds.API.IsInterstitialAvailable())
        {
            Gley.MobileAds.API.ShowInterstitial();
        }
        else
        {
            Debug.LogWarning("Interstitial ad is not available.");
        }
    }

    public void ShowAppOpen()
    {
        if (Gley.MobileAds.API.IsAppOpenAvailable())
        {
            Gley.MobileAds.API.ShowAppOpen();
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        }
        else
        {
            Debug.LogWarning("App Open ad is not available.");
        }
    }

    public void ShowRewardedVideo()
    {
        if (Gley.MobileAds.API.IsRewardedVideoAvailable())
        {
            Gley.MobileAds.API.ShowRewardedVideo(CompleteMethod);
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        }
        else
        {
            Debug.LogWarning("Rewarded video ad is not available.");
        }
    }

    private void CompleteMethod(bool completed)
    {
        if (completed)
        {
            onRewardedVideoCompleted.Invoke();
        }
    }

    public void ShowMRec()
    {
        Gley.MobileAds.API.ShowMRec(Gley.MobileAds.BannerPosition.BottomRight);
        MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
    }

    public void HideMRec()
    {
        Gley.MobileAds.API.HideMRec();
    }

    private void OnInitialized()
    {
        Gley.MobileAds.Events.onInterstitialLoadSucces += onInterstitialLoadSucces;
        Gley.MobileAds.Events.onBannerLoadSucces += OnBannerAdsShow;
    }

    private void onInterstitialLoadSucces()
    {
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
    }

    private void OnBannerAdsShow()
    {
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
    }

    private void OnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo impressionData)
    {
        double revenue = impressionData.Revenue;
        var impressionParameters = new[]
        {
            new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
            new Firebase.Analytics.Parameter("ad_source", impressionData.NetworkName),
            new Firebase.Analytics.Parameter("ad_unit_name", impressionData.AdUnitIdentifier),
            new Firebase.Analytics.Parameter("ad_format", impressionData.AdFormat),
            new Firebase.Analytics.Parameter("value", revenue),
            new Firebase.Analytics.Parameter("currency", "USD"),
        };
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AdsManager : MonoBehaviour
{
    // Start is called before the first frame update


    public UnityEvent onRewardedVideoCompleted;
    void Start()
    {
    Gley.MobileAds.API.Initialize();
        Gley.MobileAds.Events.onInitialized += OnInitialized;
    }


    public void ShowBanner()
    {
        Gley.MobileAds.API.ShowBanner(Gley.MobileAds.BannerPosition.Top, Gley.MobileAds.BannerType.Adaptive);
        
        
    }

    public void ShowInterstitial()
    {
        Gley.MobileAds.API.ShowInterstitial();
        
    }

    public void ShowAppOpen()
    {
        Gley.MobileAds.API.ShowAppOpen();
       // MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
    }

      public void ShowRewardedVideo()
    {
        Gley.MobileAds.API.ShowRewardedVideo(CompleteMethod);
       // MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
    }

    private void CompleteMethod(bool completed)
    {
        if (completed)
        {
            onRewardedVideoCompleted.Invoke();


        }
    }

    private void OnInitialized()
    {
       
        Gley.MobileAds.Events.onInterstitialLoadSucces += onInterstitialLoadSucces;
        Gley.MobileAds.Events.onBannerLoadSucces += OnBannerAdsShow;
    }

    private void onInterstitialLoadSucces()
    {
        //do something
       // MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
    }
    private void OnBannerAdsShow()
    {
        //do something
       // MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
    }

    


    //MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
    //    MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
    //    MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
    //    MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
    //private void OnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo impressionData)
    //{
    //    double revenue = impressionData.Revenue;
    //    var impressionParameters = new[] {
    //  new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
    //  new Firebase.Analytics.Parameter("ad_source", impressionData.NetworkName),
    //  new Firebase.Analytics.Parameter("ad_unit_name", impressionData.AdUnitIdentifier),
    //  new Firebase.Analytics.Parameter("ad_format", impressionData.AdFormat),
    //  new Firebase.Analytics.Parameter("value", revenue),
    //  new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
    //};
    //    Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
    //}

}

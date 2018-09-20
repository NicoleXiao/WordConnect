using UnityEngine;
using System;


public class AdmobController : MonoBehaviour
{
   

    public static AdmobController instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (!CUtils.IsBuyItem() && !CUtils.IsAdsRemoved())
        {
            RequestBanner();
            RequestInterstitial();
        }

        InitRewardedVideo();
        RequestRewardBasedVideo();
    }

    private void InitRewardedVideo()
    {
        
    }

    public void RequestBanner()
    {
        // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = ConfigController.Config.admob.androidBanner.Trim();
#elif UNITY_IPHONE
        string adUnitId = ConfigController.Config.admob.iosBanner.Trim();
#else
        string adUnitId = "unexpected_platform";
#endif

        
    }

    public void RequestInterstitial()
    {
        // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = ConfigController.Config.admob.androidInterstitial.Trim();
#elif UNITY_IPHONE
        string adUnitId = ConfigController.Config.admob.iosInterstitial.Trim();
#else
        string adUnitId = "unexpected_platform";
#endif

  
    }

    public void RequestRewardBasedVideo()
    {
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = ConfigController.Config.admob.androidRewarded.Trim();
#elif UNITY_IPHONE
        string adUnitId = ConfigController.Config.admob.iosRewarded.Trim();
#else
        string adUnitId = "unexpected_platform";
#endif

       
    }


    //public void ShowInterstitial()
    //{
        
    //}

    public void ShowBanner()
    {
        if (CUtils.IsBuyItem()) return;
        //if (bannerView != null)
        //{
        //    bannerView.Show();
        //}
    }

    public void HideBanner()
    {
        
    }

    public bool ShowInterstitial(bool video = false)
    {
        
        return false;
    }

    public void ShowRewardBasedVideo()
    {
        
    }

    #region Banner callback handlers

    public void HandleAdLoaded(object sender, EventArgs args)
    {
        print("HandleAdLoaded event received.");
    }

    public void HandleAdFailedToLoad(object sender, EventArgs args)
    {
        print("HandleFailedToReceiveAd event received with message: ");
    }

    public void HandleAdOpened(object sender, EventArgs args)
    {
        print("HandleAdOpened event received");
    }

    public void HandleAdClosed(object sender, EventArgs args)
    {
        print("HandleAdClosed event received");
    }

    public void HandleAdLeftApplication(object sender, EventArgs args)
    {
        print("HandleAdLeftApplication event received");
    }

    #endregion

    #region Interstitial callback handlers

    public void HandleInterstitialLoaded(object sender, EventArgs args)
    {
        print("HandleInterstitialLoaded event received.");
    }

    public void HandleInterstitialFailedToLoad(object sender, EventArgs args)
    {
        print("HandleInterstitialFailedToLoad event received with message: " );
    }

    public void HandleInterstitialOpened(object sender, EventArgs args)
    {
        print("HandleInterstitialOpened event received");
    }

    public void HandleInterstitialClosed(object sender, EventArgs args)
    {
        print("HandleInterstitialClosed event received");
        RequestInterstitial();
    }

    public void HandleInterstitialLeftApplication(object sender, EventArgs args)
    {
        print("HandleInterstitialLeftApplication event received");
    }

    #endregion

    #region RewardBasedVideo callback handlers

    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoLoaded event received");
    }

    public void HandleRewardBasedVideoFailedToLoad(object sender, EventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardBasedVideoFailedToLoad event received with message: " );
    }

    public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoOpened event received");
    }

    public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoStarted event received");
    }

    public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
        RequestRewardBasedVideo();
        MonoBehaviour.print("HandleRewardBasedVideoClosed event received");
    }

    public void HandleRewardBasedVideoRewarded(object sender, EventArgs args)
    {
        //string type = args.Type;
        //double amount = args.Amount;
        //MonoBehaviour.print(
        //    "HandleRewardBasedVideoRewarded event received for " + amount.ToString() + " " + type);
    }

    public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoLeftApplication event received");
    }

    #endregion
}

﻿using UnityEngine;
using System;

[System.Serializable]
public class GameConfig
{
    public Admob admob;

    [Header("")]
    public int bannerPeriod;
    public int intertitialPeriod;
    public int rewardedVideoPeriod;
    public int rewardedVideoAmount;
    public string androidPackageID;
    public string iosAppID;
    public string macAppID;
    public string facebookPageID;
    public bool enableFacebookFeatures;

    [Header("")]
    public int fontSizeInDiskSelectLevel;
    public int fontSizeInDiskMainScene;
    public int fontSizeInCellMainScene;
    [Header("广告测试")]
    public bool adtest;
    public bool showad;
    public bool showpurchase;
   
}

[System.Serializable]
public class Admob
{
    [Header("Interstitial")]
    public string androidInterstitial;
    public string iosInterstitial;
    [Header("Banner")]
    public string androidBanner;
    public string iosBanner;
    [Header("RewardedVideo")]
    public string androidRewarded;
    public string iosRewarded;
}

﻿//using GoogleMobileAds.Api;
using UnityEngine;

public class RewardedVideoCallBack : MonoBehaviour {

    private void Start()
    {
        Timer.Schedule(this, 0.1f, AddEvents);
    }

    private void AddEvents()
    {
#if UNITY_ANDROID || UNITY_IOS
        //if (AdmobController.instance.rewardBasedVideo != null)
        //{
        //    AdmobController.instance.rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
        //}
#endif
    }

    private const string ACTION_NAME = "rewarded_video";
    //public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    //{
    //    var dialog = (RewardedVideoDialog)DialogController.instance.GetDialog(DialogType.RewardedVideo);
    //    dialog.SetAmount(ConfigController.Config.rewardedVideoAmount);
    //    DialogController.instance.ShowDialog(dialog);

    //    CUtils.SetActionTime(ACTION_NAME);
    //}

    private void OnDestroy()
    {
#if UNITY_ANDROID || UNITY_IOS
        //if (AdmobController.instance.rewardBasedVideo != null)
        //{
        //    AdmobController.instance.rewardBasedVideo.OnAdRewarded -= HandleRewardBasedVideoRewarded;
        //}
#endif
    }
}

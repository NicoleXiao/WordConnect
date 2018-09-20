//using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RewardedButton : MonoBehaviour
{
    public GameObject content;
   // public GameObject adAvailableTextHolder;
   //public TimerText timerText;
    public UnityEvent countDownEvent;
    public UnityEvent buttonHide;
    private const string ACTION_NAME = "rewarded_video";
    private bool isEventAttached;

    private void Start()
    {
        MonoBehaviour.print ("on Start");
        // if ( timerText != null ) timerText.onCountDownComplete += OnCountDownComplete;

#if UNITY_ANDROID || UNITY_IOS
        Timer.Schedule (this,0.1f,AddEvents);
        if (!IsAvailableToShow())
        {
            content.SetActive (false);
            //if ( IsAdAvailable () && !IsActionAvailable () )
            //{
            //    int remainTime = ( int ) ( ConfigController.Config.rewardedVideoPeriod - CUtils.GetActionDeltaTime (ACTION_NAME) );
            //    ShowTimerText (remainTime);
            //}
        }

        InvokeRepeating ("IUpdate", 1, 1);
#else
        content.SetActive(false);
#endif
    }

    private void AddEvents()
    {
        AdController.instance.OnAdRewarded += HandleRewardBasedVideoRewarded;
        AdController.instance.OnAdLoaded += HandleRewardVideoLoaded;

    }

    private void HandleRewardVideoLoaded( object sender, EventArgs e )
    {
        MonoBehaviour.print ("rewardVideo loaded");
    }
    private void HandleRewardBasedVideoRewarded( object sender, EventArgs e )
    {
        MonoBehaviour.print ("rewardVideo rewarded");

        if (((AdController.RewardTypeEventArgs)e).rewardType == null) {

            var dialog = (RewardedVideoDialog)DialogController.instance.GetDialog (DialogType.RewardedVideo);
            dialog.SetAmount (ConfigController.Config.rewardedVideoAmount);
            DialogController.instance.ShowDialog (dialog);
            CUtils.SetActionTime (ACTION_NAME);
            content.SetActive (false);
            if (buttonHide != null) {
                buttonHide.Invoke ();
            }
        }
        // ShowTimerText (ConfigController.Config.rewardedVideoPeriod);
    }

    private void IUpdate()
    {
        //MonoBehaviour.print("IUpdate");
        content.SetActive (IsAvailableToShow ());
        if ( !IsAvailableToShow () )
        {
            if ( buttonHide != null )
            {
                buttonHide.Invoke ();
            }
        }
        else
        {
            if ( countDownEvent != null )
            {
                countDownEvent.Invoke ();
            }
        }
    }

    public void OnClick()
    {
        GameState.playVideoCount++;
        Prefs.playVideoCount = GameState.playVideoCount;
        Debug.Log ("点击视频的次数 :  " + GameState.playVideoCount);
        AdController.instance.ShowRewardedVideo ();
        Sound.instance.PlayButton ();//测试用

    }

    //private void ShowTimerText( int time )
    //{
    //    if ( adAvailableTextHolder != null )
    //    {
    //        adAvailableTextHolder.SetActive (true);
    //        timerText.SetTime (time);
    //        timerText.Run ();
    //    }
    //}

    // public void HandleRewardBasedVideoRewarded( object sender, Reward args )
    //{
    //    content.SetActive (false);
    //    ShowTimerText (ConfigController.Config.rewardedVideoPeriod);
    //}

    //private void OnCountDownComplete()
    //{
    //    adAvailableTextHolder.SetActive (false);
    //    if ( IsAdAvailable () )
    //    {
    //        content.SetActive (true);
    //        if ( countDownEvent != null )
    //        {
    //            countDownEvent.Invoke ();
    //        }
    //    }
    //}

    public bool IsAvailableToShow()
    {
      //  Debug.Log ("ActionAvailabgle -----------------"+ IsActionAvailable ());
        return IsActionAvailable() && IsAdAvailable() && GameState.playVideoCount<GameState.totalPlayVideo && Prefs.unlockedLevel>=16;
    }

    private bool IsActionAvailable()
    {
        return CUtils.IsActionAvailable (ACTION_NAME, ConfigController.Config.rewardedVideoPeriod);
    }

    private bool IsAdAvailable()
    {
        bool isLoaded = AdController.instance.IsRewardedVideoLoaded;
        return isLoaded;
    }

    private void OnDestroy()
    {
#if UNITY_ANDROID || UNITY_IOS
        //if (AdmobController.instance.rewardBasedVideo != null)
        //{
        //    AdmobController.instance.rewardBasedVideo.OnAdRewarded -= HandleRewardBasedVideoRewarded;
        //}
        AdController.instance.OnAdRewarded -= HandleRewardBasedVideoRewarded;
        AdController.instance.OnAdLoaded -= HandleRewardVideoLoaded;
#endif
    }

    private void OnApplicationPause( bool pause )
    {
        if ( !pause )
        {
            //MonoBehaviour.print ("adAvailableTextHolder.activeSelf :" + adAvailableTextHolder.activeSelf);
            //if ( adAvailableTextHolder.activeSelf )
            //{
            //    int remainTime = ( int ) ( ConfigController.Config.rewardedVideoPeriod - CUtils.GetActionDeltaTime (ACTION_NAME) );
            //    ShowTimerText (remainTime);
            //}
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DailyWinDialog : Dialog {
    public Button bgClose;//背景就是关闭按钮
    public Button rewardBtn;
    public Text rewardAmout;
    private int amount = 20;
    protected override void Awake() {
        base.Awake ();
        AddEvents ();
        rewardAmout.text = "X" + amount;
        bgClose.onClick.AddListener (() => {
            Sound.instance.PlayButton ();
            ShowGems ();
        });
        if (AdController.instance.IsRewardedVideoLoaded) {
            rewardBtn.gameObject.SetActive (true);
        } else {
            rewardBtn.gameObject.SetActive (false);
        }
        rewardBtn.onClick.AddListener (() => {
            Sound.instance.PlayButton ();
            //没有广告出现就直接飞奖励
            AdController.instance.ShowRewardedVideo ("dailywinreward");


        });
    }
    protected override void Start() {
        base.Start ();
    }
    private void HandleRewardVideoLoaded(object sender, EventArgs e) {
        Debug.Log ("rewardVideo loaded");
    }
    private void HandleRewardBasedVideoRewarded(object sender, EventArgs e) {
        Debug.Log ("rewardVideo rewarded");
        if (((AdController.RewardTypeEventArgs)e).rewardType != null) {
            amount += amount;
            ShowGems ();
        }
    }
    private void AddEvents() {
        AdController.instance.OnAdRewarded += HandleRewardBasedVideoRewarded;
        AdController.instance.OnAdLoaded += HandleRewardVideoLoaded;

    }
    private void ShowGems() {
        GemsEffet gems = EffectManager.Instance.LoadGems (GameObject.Find ("Canvas").transform, rewardBtn.transform.position, amount, true);
        gems.OnEffectComplete += () => {
            CurrencyController.CreditBalance (amount);
        };
        gems.OnEffectShow += () => {
            Close ();

        };
    }
    public override void Close() {
        base.Close ();
        AdController.instance.OnAdRewarded -= HandleRewardBasedVideoRewarded;
        AdController.instance.OnAdLoaded -= HandleRewardVideoLoaded;

    }
}

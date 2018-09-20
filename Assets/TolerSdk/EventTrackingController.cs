using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrackingController : MonoBehaviour
{

    private AndroidJavaObject mFirebaseController;
    private AndroidJavaObject mGameAnalyticsController;
    public static EventTrackingController instance;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        //获取context
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        mFirebaseController = new AndroidJavaClass("com.firebase.FirebaseController").CallStatic<AndroidJavaObject>("getInstance", currentActivity);
        mGameAnalyticsController = new AndroidJavaClass("com.gameanalytics.GameAnalyticsController").CallStatic<AndroidJavaObject>("getInstance", currentActivity);
    }

    public void LogHintClickEvent(int count)
    {
        LogFirebaseEvent(EventTrackingConst.HintEvent, count);
        string[] append={ GameState.currentLevel.ToString(),CurrencyController.GetBalance().ToString()};
        LogGameAnalyticsEvent(EventTrackingConst.HintEvent, append,
        count);
    }

    public void LogExtraWordClickEvent(int count)
    {
        LogFirebaseEvent(EventTrackingConst.ExtralWordEvent, count);
        string[] append={ GameState.currentLevel.ToString(),CurrencyController.GetBalance().ToString()};
        LogGameAnalyticsEvent(EventTrackingConst.ExtralWordEvent, append,
        count);
    }

    /*
     * 签到进入事件
     */
    public void LogSignClickEvent()
    {
        LogFirebaseEvent(EventTrackingConst.SignEnterEvent);
        string[] append={ GameState.currentLevel.ToString(),CurrencyController.GetBalance().ToString()};
        LogGameAnalyticsEvent(EventTrackingConst.SignEnterEvent, append);
    }
    /*
     * 签到完成事件
     */
    public void LogSignDoneEvent()
    {
        LogFirebaseEvent(EventTrackingConst.SignDoneEvent);
        string[] append={ GameState.currentLevel.ToString(),CurrencyController.GetBalance().ToString()};
        LogGameAnalyticsEvent(EventTrackingConst.SignDoneEvent, append);
    }
    /*
     * 大章节奖励事件
     */
    public void LogChapterRewardEvent()
    {
        LogFirebaseEvent(EventTrackingConst.ChapterReward);
        string[] append={ GameState.currentLevel.ToString(),CurrencyController.GetBalance().ToString()};
        LogGameAnalyticsEvent(EventTrackingConst.ChapterReward, append);
    }
   
    /*
     * 关卡奖励事件(存钱罐)
     */
    public void LogLevelReward()
    {
        LogFirebaseEvent(EventTrackingConst.LevelReward);
        string[] append={ GameState.currentLevel.ToString(),CurrencyController.GetBalance().ToString()};
        LogGameAnalyticsEvent(EventTrackingConst.LevelReward, append);
    }

    /**
     * 记录购买事件 
     * amount 金额 99 表示 0.99元
     * itemId= purchase IAPItem productId
     * cartType 标示购买来源
     * receipt IAPItem 购买收据
     * signature IAPItem 购买签名
     */
    public void LogPurchaseEvent(float amount, string itemId, string cartType = "shopdialog", string receipt = "", string signature = "")
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            LogFirebaseEvent(EventTrackingConst.PurchaseEvent,amount/100);
            string[] append={ GameState.currentLevel.ToString(),CurrencyController.GetBalance().ToString()};
            LogGameAnalyticsEvent(EventTrackingConst.PurchaseEvent, append, amount/100);
            mGameAnalyticsController.Call("logPurchaseEvent", "USD", amount, itemId, cartType, receipt, signature);
        }
    }
    /**
     * 记录gems增加事件
     * amount 增加数量
     * type :IAP(内购) chapterreward(大章节奖励) subsectionreward(小章节奖励) levelreward(关卡奖励/存钱罐奖励) signinreward(签到奖励) slotmachinesreward(老虎机奖励)
     * itemId IAPItem productId
     */
    public void LogGemsAdd(int amount, string type, string itemId = "")
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            LogFirebaseEvent(EventTrackingConst.GemsAdd,amount);
            string[] append={ GameState.currentLevel.ToString(),CurrencyController.GetBalance().ToString()};
            LogGameAnalyticsEvent(EventTrackingConst.GemsAdd, append, amount);
            mGameAnalyticsController.Call("logGemsAdd", amount, type, itemId);
        }
    }
    /**
     * 记录gems减少事件
     * amount 减少数量
     * type :hint (通过提示消费钻石)
     * itemId IAPItem productId
     */
    public void LogGemsSink(int amount, string type, string itemId = "")
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            LogFirebaseEvent(EventTrackingConst.GemsSink,amount);
            string[] append={ GameState.currentLevel.ToString(),CurrencyController.GetBalance().ToString()};
            LogGameAnalyticsEvent(EventTrackingConst.GemsSink, append,amount);
            mGameAnalyticsController.Call("logGemsSink", amount, type, itemId);
        }
    }
    public void LogLevelProcess(bool enterNewLevel, String chapter, String subsection, String level,double score=0.0f) {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (!enterNewLevel) {
                LogFirebaseEvent(EventTrackingConst.LevelProgress, Convert.ToInt32(level));
                string[] append={ GameState.currentLevel.ToString(),CurrencyController.GetBalance().ToString()};
                LogGameAnalyticsEvent(EventTrackingConst.LevelProgress, append);
            }
            
            mGameAnalyticsController.Call("logLevelProcess",enterNewLevel, chapter, subsection, level,score);
        }
    }


    public void LogFirebaseEvent(string eventName, float value = -1f) {
        if (Application.platform == RuntimePlatform.Android) {
            AndroidJavaObject map = new AndroidJavaObject("java.util.HashMap");
            map.Call<string>("put", "level", Convert.ToString(GameState.currentLevel));
            if (value != -1) {
                map.Call<string>("put", "count", value.ToString());
            }
            mFirebaseController.Call("logEvent", eventName, map);
        }
    }

    public void LogGameAnalyticsEvent(string eventName,string[] appendEvent, float value = -1f)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (appendEvent != null && appendEvent.Length > 0) {
                foreach (string append in appendEvent) {
                    eventName += ":" + append;
                }
            }
            if (value == -1f)
            {
                mGameAnalyticsController.Call("logEvent", eventName,GameState.currentLevel);
            }
            else
            {
                mGameAnalyticsController.Call("logEvent", eventName, value);
            }
        }
    }




  public static AndroidJavaObject DicToHashMap(Dictionary<string, string> dictionary)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaObject map = new AndroidJavaObject("java.util.HashMap");
            if (dictionary != null && dictionary.Count > 0)
            {
                foreach (KeyValuePair<string, string> pair in dictionary)
                {
                    map.Call<AndroidJavaObject>("put", pair.Key, pair.Value);
                }
            }
            map.Call<string>("put", "level", Convert.ToString(GameState.currentLevel));
            return map;
        }
        else
        {
            return null;
        }


    }
    // Update is called once per frame
    void Update()
    {

    }
}

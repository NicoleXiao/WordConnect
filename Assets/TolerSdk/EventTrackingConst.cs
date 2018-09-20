using System;

public class EventTrackingConst
{
    private const string rewardVideoEvent = "rewardvideo";
    private const string hintEvent = "hint";
    private const string extralWordEvent = "extralword";
    private const string signEnterEvent = "signenter";
    private const string signDoneEvent = "signdone";
    private const string levelReward = "levelreward";
    private const string chapterReward = "chapterreward";
    private const string subsectionReward = "titleReward";
    private const string purchaseEvent="purchase";
    private const string gemsAdd="gemsadd";
    private const string gemsSink="gemssink";
    private const string levelProgress="levelprogress";

    public static string RewardVideoEvent
    {
        get
        {
            return rewardVideoEvent;
        }
    }

    public static string HintEvent
    {
        get
        {
            return hintEvent;
        }
    }

    public static string ExtralWordEvent
    {
        get
        {
            return extralWordEvent;
        }
    }

    public static string SignEnterEvent
    {
        get
        {
            return signEnterEvent;
        }
    }

    public static string SignDoneEvent
    {
        get
        {
            return signDoneEvent;
        }
    }

    public static string LevelReward
    {
        get
        {
            return levelReward;
        }
    }

    public static string ChapterReward
    {
        get
        {
            return chapterReward;
        }
    }

    public static string SubsectionReward
    {
        get
        {
            return subsectionReward;
        }
    }

    public static string PurchaseEvent { get {
            return purchaseEvent;
        } }

    public static string GemsAdd { get {
            return gemsAdd;
        } }

    public static string GemsSink { get {
            return gemsSink;
        } }

    public static string LevelProgress
    {
        get
        {
            return levelProgress;
        }
    }
}

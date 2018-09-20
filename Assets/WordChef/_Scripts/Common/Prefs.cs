using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Prefs {
   
    public static int unlockedWorld
    {
        get
        {
            if (GameState.unlockedWorld == -1)
            {
                int value = CPlayerPrefs.GetInt("unlocked_world",0);
                GameState.unlockedWorld = value;
            }
            return GameState.unlockedWorld;
        }
        set { CPlayerPrefs.SetInt("unlocked_world", value); GameState.unlockedWorld = value; }
    }

    public static int unlockedSubWorld
    {
        get
        {
            if (GameState.unlockedSubWord == -1)
            {
                int value = CPlayerPrefs.GetInt("unlocked_sub_world",-1);
                GameState.unlockedSubWord = value;
            }
            return GameState.unlockedSubWord;
        }
        set { CPlayerPrefs.SetInt("unlocked_sub_world", value); GameState.unlockedSubWord = value; }
    }

    public static int unlockedLevel
    {
        get
        {
            if (GameState.unlockedLevel == -1)
            {
                int value = CPlayerPrefs.GetInt("unlocked_level", 1);
                GameState.unlockedLevel = value;
            }
            return GameState.unlockedLevel;
        }
        set { CPlayerPrefs.SetInt("unlocked_level", value); GameState.unlockedLevel = value; }
    }

    public static List<int> GetPanWordIndexes(int world, int subWorld, int level)
    {
        string data = PlayerPrefs.GetString("pan_word_indexes_v2_" + world + "_" + subWorld + "_" + level);
        return CUtils.BuildListFromString<int>(data);
    }

    public static void SetPanWordIndexes(int world, int subWorld, int level, int[] indexes)
    {
        string data = CUtils.BuildStringFromCollection(indexes);
        PlayerPrefs.SetString("pan_word_indexes_v2_" + world + "_" + subWorld + "_" + level, data);
    }
    public static bool IsLastLevel() {
        
        return GameState.currentLevel == unlockedLevel
              && ((GameState.currentLevel % GameState.chapterLevelNum==0)
              || (GameState.currentLevel == CSVReadManager.Instance.GetWordDataCount ()));

    }

    public static void SetExtraWords(int world, int subWorld, int level, string[] extraWords)
    {
        CryptoPlayerPrefsX.SetStringArray("extra_words_" + world + "_" + subWorld + "_" + level, extraWords);
    }

    public static string[] GetExtraWords(int world, int subWorld, int level)
    {
        return CryptoPlayerPrefsX.GetStringArray("extra_words_" + world + "_" + subWorld + "_" + level);
    }

    public static int extraProgress
    {
        get { return CPlayerPrefs.GetInt("extra_progress", 0); }
        set { CPlayerPrefs.SetInt("extra_progress", value); }
    }
    public static int moneyProgress {
        get {
            return CPlayerPrefs.GetInt ("money_progress", 0);
        }
        set {
            CPlayerPrefs.SetInt ("money_progress", value);
        }
    }

    public static int extraTarget {
        get {
            return CPlayerPrefs.GetInt ("extra_target", 5);
        }
        set {
            CPlayerPrefs.SetInt ("extra_target", value);
        }
    }

    public static int totalExtraAdded
    {
        get { return CPlayerPrefs.GetInt("total_extra_added", 0); }
        set { CPlayerPrefs.SetInt("total_extra_added", value); }
    }
    public static string[] crossyProgress {
        get {
            return CryptoPlayerPrefsX.GetStringArray ("crossy_progress");
        }
        set {
            CryptoPlayerPrefsX.SetStringArray ("crossy_progress", value);
        }
    }
    public static string[] pictureProgress {
        get {
            return CryptoPlayerPrefsX.GetStringArray ("picture_progress");
        }
        set {
            CryptoPlayerPrefsX.SetStringArray ("picture_progress", value);
        }
    }
    public static string[] levelProgress
    {
        get { return CryptoPlayerPrefsX.GetStringArray("level_progress" + GameState.currentLevel); }
        set { CryptoPlayerPrefsX.SetStringArray("level_progress" + GameState.currentLevel, value); }
    }
    public static bool isNoti1Enabled
    {
        get { return PlayerPrefs.GetInt("is_noti_1_enabled") == 1; }
        set { PlayerPrefs.SetInt("is_noti_1_enabled", value ? 1 : 0); }
    }

    public static bool isNoti2Enabled
    {
        get { return PlayerPrefs.GetInt("is_noti_2_enabled") == 1; }
        set { PlayerPrefs.SetInt("is_noti_2_enabled", value ? 1 : 0); }
    }

    public static int noti3Ruby
    {
        get { return PlayerPrefs.GetInt("noti_3_ruby"); }
        set { PlayerPrefs.SetInt("noti_3_ruby", value); }
    }

    public static int noti4Ruby
    {
        get { return PlayerPrefs.GetInt("noti_4_ruby"); }
        set { PlayerPrefs.SetInt("noti_4_ruby", value); }
    }

    public static int noti5Ruby
    {
        get { return PlayerPrefs.GetInt("noti_5_ruby"); }
        set { PlayerPrefs.SetInt("noti_5_ruby", value); }
    }

    public static int noti6Ruby
    {
        get { return PlayerPrefs.GetInt("noti_6_ruby"); }
        set { PlayerPrefs.SetInt("noti_6_ruby", value); }
    }

    public static int noti7Ruby
    {
        get { return PlayerPrefs.GetInt("noti_7_ruby"); }
        set { PlayerPrefs.SetInt("noti_7_ruby", value); }
    }
    public static int playVideoCount
    {
        get { return PlayerPrefs.GetInt ("play_Video_Count",0); }
        set { PlayerPrefs.SetInt ("play_Video_Count", value); }
    }
    public static int FirstEnterGame {
        get { return PlayerPrefs.GetInt ("first_enter", 0); }
        set { PlayerPrefs.SetInt ("first_enter", value); }
    }
    //小红花个数
    public static int StarCount {
        get {
            return PlayerPrefs.GetInt ("star_count", 0);
        }
        set {
            PlayerPrefs.SetInt ("star_count", value);
        }
    }
    public static int RewardStarCount {
        get {
            return PlayerPrefs.GetInt ("reward_star_count", 0);
        }
        set {
            PlayerPrefs.SetInt ("reward_star_count", value);
        }
    }

    //签到天数
    public static int SignDay {
        get {  return PlayerPrefs.GetInt ("sign_day", 0);  }
        set {PlayerPrefs.SetInt ("sign_day", value); }
    }
    //是否已经签到，0是没有，1是有
    public static int Signed {
        get {
            return PlayerPrefs.GetInt ("sign", 0);
        }
        set {
            PlayerPrefs.SetInt ("sign", value);
        }
    }
    //有没有弹签到出来.
    public static int SignPop {
        get {
            return PlayerPrefs.GetInt ("sign_pop", 0);
        }
        set {
            PlayerPrefs.SetInt ("sign_pop", value);
        }
    }
    //刮刮奖的id
    public static int ScratchCardID {
        get {
            return PlayerPrefs.GetInt ("scratch_card_id", 0);
        }
        set {
            PlayerPrefs.SetInt ("scratch_card_id", value);
        }
    }
    public static int UnLockNewGrahp {
        get {
            return PlayerPrefs.GetInt ("unlock_new_graph", 0);
        }
        set {
            PlayerPrefs.SetInt ("unlock_new_graph", value);
        }
    }
    //刮刮奖随机的count
    public static int ScratchCardCount {
        get {
            return PlayerPrefs.GetInt ("scratch_card_count", 0);
        }
        set {
            PlayerPrefs.SetInt ("scratch_card_count", value);
        }
    }
    //是否完成刮卡的任务了 0 没有，1 完成了 ，2 领取了奖励
    public static int ScratchCardTaskFinish {
        get {
            return PlayerPrefs.GetInt ("scratch_card_finish", 0);
        }
        set {
            PlayerPrefs.SetInt ("scratch_card_finish", value);
        }
    }
    //是否是任务
    public static int IsTask {
        get {
            return PlayerPrefs.GetInt ("is_task", 0);
        }
        set {
            PlayerPrefs.SetInt ("is_task", value);
        }
    }
  
    //游戏中为了完成任务已经达到数量
    public static int ScratchCardGameCount {
        get {
            return PlayerPrefs.GetInt ("scratch_card_game_count", 0);
        }
        set {
            PlayerPrefs.SetInt ("scratch_card_game_count", value);
        }
    }
    public static int PassLevelCount{
        get {
            return PlayerPrefs.GetInt ("pass_level_count", 0);
        }
        set {
            PlayerPrefs.SetInt ("pass_level_count", value);
        }
    }
    public static int FirstLoginFacebook{
        get {
            return PlayerPrefs.GetInt ("first_login_facebook", 0);
        }
        set {
            PlayerPrefs.SetInt ("first_login_facebook", value);
        }
    }
    public static int HintNum {
        get {
            return PlayerPrefs.GetInt ("hint_num", 0);
        }
        set {
            PlayerPrefs.SetInt ("hint_num", value);
        }
    }
    public static void AddToNumHint(int world, int subWorld, int level)
    {
        int numHint = GetNumHint(world, subWorld, level);
        PlayerPrefs.SetInt("numhint_used_" + world + "_" + subWorld + "_" + level, numHint + 1);
    }

    public static int GetNumHint(int world, int subWorld, int level)
    {
        return PlayerPrefs.GetInt("numhint_used_" + world + "_" + subWorld + "_" + level);
    }
  
}

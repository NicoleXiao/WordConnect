using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlayerDataManager : MonoBehaviourSingleton<PlayerDataManager> {
    public PlayerData playerData;
    public Vector2 contentPos = Vector2.zero;
    public bool loginDay = false;
    public bool continuousLoginDay = false;
    private bool firstRead = false;
    private int crossyTotalCount = 10;
    private int searchTotalCount = 7;
    private int puzzleTotalCount = 3;
    private int pictureTotalCount = 10;

    private void Awake() {
        m_instance = this;
        ReadPlayerProgress ();
    }

    public void SavePlayerProgress() {
        TextManager.Instance.SaveProgress<PlayerData> ("PlayerData.json", playerData);

    }
    //删除昨天保存的数据
    private void DeleteData() {
        PlayerPrefs.DeleteKey ("crossy_progress");
        TextManager.Instance.DeleteFile ("PuzzleProgress.json");
        TextManager.Instance.DeleteFile ("PictureProgress.json");
        TextManager.Instance.DeleteFile ("SearchProgress.json");
    }

    public void ReadPlayerProgress() {
        playerData = TextManager.Instance.ReadProgress<PlayerData> ("PlayerData.json");
        if (playerData == null) {
            firstRead = true;
            playerData = new PlayerData ();
        }
        playerData.passLevel = Prefs.unlockedLevel - 1;
        playerData.accumulativeStar = Prefs.StarCount;
        SetGameData ();
    }

    private void SetGameData() {
        GameState.crossyLevel = playerData.crossyLevelID;
        GameState.searchLevel = playerData.searchLevelID;
        GameState.pictureLevel = playerData.pictureLevelID;
        GameState.puzzleLevel = playerData.puzzleLevelID;
    }

    public void JudeReachAchieve(int id, int currentNum) {
        List<AchievementData> data = CSVReadManager.Instance.achievement_data[id];
        for (int i = 0; i < data.Count; i++) {
            if (currentNum == data[i].num) {
                if (id != 2) {
                    Compliment.Instance.ShowAchievement ();
                } else {
                    StartCoroutine (Compliment.Instance.DelayShowAchieve());
                }
                break;
            }
        }
        SavePlayerProgress ();
    }

    public void NewDay() {
        if (!firstRead) {
            playerData.dailyChangeBgIndex++;
            if (playerData.dailyChangeBgIndex > 7) {
                playerData.dailyChangeBgIndex = 1;
            }

            playerData.isDailyFinish = false;
            if (playerData.isCrossyFinish) {
                playerData.isCrossyFinish = false;
                playerData.crossyLevelID = (playerData.crossyLevelID + 1) <= crossyTotalCount ? (playerData.crossyLevelID + 1) : 1;
            }
            if (playerData.isSearchFinish) {
                playerData.isSearchFinish = false;
                playerData.searchLevelID = (playerData.searchLevelID + 1) <= searchTotalCount ? (playerData.searchLevelID + 1) : 1;
            }
            if (playerData.isPuzzleFinish) {
                playerData.isPuzzleFinish = false;
                playerData.puzzleLevelID = (playerData.puzzleLevelID + 1) <= puzzleTotalCount ? (playerData.puzzleLevelID + 1) : 1;
            }
            if (playerData.isPictureFinish) {
                playerData.isPictureFinish = false;
                playerData.pictureLevelID = (playerData.pictureLevelID + 1) <= pictureTotalCount ? (playerData.pictureLevelID + 1) : 1;
            }
            SetGameData ();
            SavePlayerProgress ();
            DeleteData ();
        }
    }
}
[System.Serializable]
public class PlayerData {

    public bool FirstEnterMap = true;

    //四个关卡挑战进度
    public int crossyLevelID = 1;
    public int searchLevelID = 1;
    public int pictureLevelID = 1;
    public int puzzleLevelID = 1;
    //当天的关卡是否挑战完毕了
    public bool isCrossyFinish = false;
    public bool isSearchFinish = false;
    public bool isPictureFinish = false;
    public bool isPuzzleFinish = false;
    public bool isDailyFinish = false;

    public int dailyChangeBgIndex = 1;

    //成就相关
    public int passLevel = 0;
    public int fantasticCount = 0;
    public int loginDayCount = 0;
    public int continuousLoginCount = 0;
    public int accumulativeLinkWord = 0;
    public int accumulativeChallenge = 0;
    public int accumulativeStar = 0;
    public int accumulativeCollectStar = 0;
    public int accumulativeExtraWord = 0;
    public int searchCount = 0;
    public int crossyCount = 0;
    public int pictureCount = 0;
    public int puzzleCount = 0;
    public int slotBounsCount = 0;
    public int wordCount = 0;
    public int connectCount = 0;

}

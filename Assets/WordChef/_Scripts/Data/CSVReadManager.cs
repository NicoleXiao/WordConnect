using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Superpow;
using LitJson;
public enum DataType {
    None,
    Sign,//签到类
    Card,//刮刮卡
    Item,//道具
    StarReward,//星座的奖励
    SlotChange,// 老虎机概率
    WorldData,//游戏配置
    LevelOnReward, //地图上的奖励
    Picture,
    Crossy,
    Search,
    Puzzle,
    Achievement
}
public class CSVReadManager : MonoBehaviourSingleton<CSVReadManager> {

    [HideInInspector]
    public List<PictureData> pictureData = new List<PictureData> ();
    [HideInInspector]
    public List<int> rewardLevel = new List<int> ();//需要给奖励的关卡
    [HideInInspector]
    public System.Action ReadCompelte;
    public System.Action ReadError;
    [HideInInspector]
    public CrossyData crossyData = new CrossyData ();
    [HideInInspector]
    public SearchData searchData = new SearchData ();
    [HideInInspector]
    public PuzzleData puzzleData = new PuzzleData ();
    [HideInInspector]
    public Dictionary<int, List<AchievementData>> achievement_data = new Dictionary<int, List<AchievementData>> ();
    private Dictionary<int, List<RewardData>> sign_data = new Dictionary<int, List<RewardData>> ();
    private Dictionary<int, StarReward> star_reward = new Dictionary<int, StarReward> ();
    private Dictionary<int, List<SlotChance>> slot_chance = new Dictionary<int, List<SlotChance>> ();
    private Dictionary<int, List<RewardData>> level_on_Reward = new Dictionary<int, List<RewardData>> ();


    private List<ItemData> item_data = new List<ItemData> ();
    private List<CardData> card_data = new List<CardData> ();
    private List<SlotChance> chance1 = new List<SlotChance> ();
    private List<SlotChance> chance2 = new List<SlotChance> ();
    private List<SlotChance> chance3 = new List<SlotChance> ();
    public List<WorldData> world_data = new List<WorldData> ();

    void Awake() {
        m_instance = this;
    }

    void Start() {
        if (world_data.Count == 0) {
            StartCoroutine (GetCSVData (SetPath ("World_Data.csv"), DataType.WorldData));
        }
        if (sign_data.Count == 0) {
            StartCoroutine (GetCSVData (SetPath ("Sign_Reward.csv"), DataType.Sign));
        }
        if (item_data.Count == 0) {
            StartCoroutine (GetCSVData (SetPath ("Item.csv"), DataType.Item));
        }
        if (item_data.Count == 0) {
            StartCoroutine (GetCSVData (SetPath ("Level_on_Reward.csv"), DataType.LevelOnReward));
        }
        if (card_data.Count == 0) {
            StartCoroutine (GetCSVData (SetPath ("Card_Reward.csv"), DataType.Card));
        }
        if (star_reward.Count == 0) {
            StartCoroutine (GetCSVData (SetPath ("Star_Des.csv"), DataType.StarReward));
        }
        if (slot_chance.Count == 0) {
            StartCoroutine (GetCSVData (SetPath ("Slot_Chance.csv"), DataType.SlotChange));
        }
        if (achievement_data.Count == 0) {
            StartCoroutine (GetCSVData (SetPath ("Achievement.csv"), DataType.Achievement));
        }

    }

    private string SetPath(string filename) {
        return Application.streamingAssetsPath + "/Data/" + filename;
    }

    public IEnumerator GetCSVData(string path, DataType type) {

        WWW www = new WWW (path);
        yield return www;

        if (www.error != null) {
            Debug.Log ("Error : " + www.error);
            if (ReadError != null) {
                ReadError ();
            }
        }
        if (!string.IsNullOrEmpty (www.text)) {
            SetData (www.text, type);
        }
        //www.Dispose ();
    }

    //把数据存储到数据中
    private void SetData(string info, DataType type) {

        string[] data = info.Split ('\r');
        switch (type) {
            case DataType.None:
                break;
            case DataType.Sign:
                SetSignData (data);
                break;
            case DataType.Card:
                SetCardData (data);
                break;
            case DataType.Item:
                SetItemData (data);
                break;
            case DataType.LevelOnReward:
                ReadLevelOnRewardData (data);
                break;
            case DataType.StarReward:
                SetWorldReward (data);
                break;
            case DataType.SlotChange:
                SetSlotChance (data);
                break;
            case DataType.WorldData:
                ReadWorldData (data);
                break;
            case DataType.Picture:
                ReadPictureData (data);
                break;
            case DataType.Crossy:
                ReadCrossyData (info);
                break;
            case DataType.Search:
                ReadSearchData (info);
                break;
            case DataType.Puzzle:
                ReadPuzzleData (info);
                break;
            case DataType.Achievement:
                ReadAchievementData (data);
                break;

        }
    }

    //地图上的星星奖励
    private void ReadLevelOnRewardData(string[] data) {
        for (int i = 1; i < data.Length; i++) {
            string[] info = data[i].Split (',');
            if (info.Length > 2) {
                int levelId = 0;
                int.TryParse (info[0], out levelId);
                List<RewardData> list = new List<RewardData> ();
                RewardData data1 = new RewardData ();
                int.TryParse (info[1], out data1.rewardId);
                int.TryParse (info[2], out data1.rewardNum);
                list.Add (data1);
                rewardLevel.Add (levelId);
                level_on_Reward.Add (levelId, list);
            }
        }
    }


    #region SlotChange
    private void SetSlotChance(string[] data) {
        for (int i = 1; i < data.Length; i++) {

            string[] info = data[i].Split (',');
            if (info.Length > 2) {
                SlotChance sc = new SlotChance ();
                int.TryParse (info[0], out sc.num);
                int.TryParse (info[1], out sc.chance);
                chance1.Add (sc);
                SlotChance sc1 = new SlotChance ();
                int.TryParse (info[2], out sc1.num);
                int.TryParse (info[3], out sc1.chance);
                chance2.Add (sc1);
                SlotChance sc2 = new SlotChance ();
                int.TryParse (info[4], out sc2.num);
                int.TryParse (info[5], out sc2.chance);
                chance3.Add (sc2);
            }
        }
        slot_chance.Add (1, chance1);
        slot_chance.Add (2, chance2);
        slot_chance.Add (3, chance3);
    }
    //这里的num代表的是第几个老虎机数字。老虎机现在只有三个
    public List<SlotChance> GetSlotChange(int num) {
        return slot_chance[num];
    }
    #endregion

    #region WorldReward
    private void SetWorldReward(string[] data) {
        for (int i = 1; i < data.Length; i++) {
            StarReward starReward = new StarReward ();
            string[] info = data[i].Split (',');
            if (info.Length > 2) {
                int mapId = 0;
                int.TryParse (info[0], out mapId);

                if (!string.IsNullOrEmpty (info[1]) && !string.IsNullOrEmpty (info[2])) {
                    RewardData da = new RewardData ();
                    int.TryParse (info[1], out da.rewardId);
                    int.TryParse (info[2], out da.rewardNum);
                    starReward.reward.Add (da);
                }
                if (!string.IsNullOrEmpty (info[3]) && !string.IsNullOrEmpty (info[4])) {
                    RewardData da = new RewardData ();
                    int.TryParse (info[3], out da.rewardId);
                    int.TryParse (info[4], out da.rewardNum);
                    starReward.reward.Add (da);
                }
                starReward.des = info[5];
                //因为字符太多要读取新的列
                if (info.Length > 6) {
                    for (int j = 6; j < info.Length; j++) {
                        if (!string.IsNullOrEmpty (info[j])) {
                            starReward.des = starReward.des + info[j];
                        }
                    }
                }
                star_reward.Add (mapId, starReward);
            }
        }

    }
    public List<RewardData> GetWorldRewardData(int index) {
        if (!star_reward.ContainsKey (index + 1)) {
            Debug.Log ("World  :" + index + 1 + "  data is Lost");
            return null;
        }
        return star_reward[index + 1].reward;
    }
    public string GetStarDes(int index) {
        if (!star_reward.ContainsKey (index + 1)) {
            Debug.Log ("World  :" + index + 1 + "  data is Lost");
            return null;
        }
        return star_reward[index + 1].des;
    }
    #endregion

    #region CardData
    private void SetCardData(string[] data) {
        for (int i = 1; i < data.Length; i++) {
            string[] info = data[i].Split (',');
            if (info.Length < 2) {
                break;
            }
            CardData card = new CardData ();
            int.TryParse (info[1], out card.type);
            card.task = info[2];
            int.TryParse (info[3], out card.minNum);
            int.TryParse (info[4], out card.maxNum);
            int.TryParse (info[5], out card.rewardID);
            int.TryParse (info[6], out card.rewardNum);
            int.TryParse (info[7], out card.chance);
            card_data.Add (card);
        }
    }
    public List<CardData> GetCardData() {
        if (card_data.Count == 0) {
            return null;
        }
        return card_data;
    }
    #endregion

    #region ItemData
    private void SetItemData(string[] data) {
        for (int i = 1; i < data.Length; i++) {
            string[] info = data[i].Split (',');
            if (info.Length < 2) {
                break;
            }
            ItemData item = new ItemData ();

            int.TryParse (info[0], out item.itemID);
            item.ItemName = info[1];
            item_data.Add (item);
        }
    }
    public string ItemName(int id) {
        string name = "";
        for (int i = 0; i < item_data.Count; i++) {
            if (item_data[i].itemID == id) {
                name = item_data[i].ItemName;
                break;
            }
        }
        return name;
    }
    #endregion

    #region SignData
    private void SetSignData(string[] data) {
        for (int i = 1; i < data.Length; i++) {
            List<RewardData> dataList = new List<RewardData> ();
            string[] info = data[i].Split (',');
            if (info.Length > 2) {
                int signId = 0;
                int.TryParse (info[0], out signId);

                for (int j = 1; j < info.Length; j = j + 2) {
                    if (!string.IsNullOrEmpty (info[j])) {
                        RewardData da = new RewardData ();
                        da.rewardId = int.Parse (info[j]);
                        da.rewardNum = int.Parse (info[j + 1]);
                        dataList.Add (da);
                    }
                }
                sign_data.Add (signId, dataList);
            }
        }
    }
    public List<RewardData> GetSignData(int day) {
        if (!sign_data.ContainsKey (day)) {
            Debug.Log ("Day  :" + day + "  data is Lost");
            return null;
        }
        return sign_data[day];
    }
    public int GetSignDataLength() {
        return sign_data.Count;
    }
    #endregion

    #region WorldData
  
    private void ReadWorldData(string[] data) {
        for (int i = 1; i < data.Length; i++) {
            string[] info = data[i].Split (',');
            if (info.Length > 2) {
                WorldData wd = new WorldData ();
                int.TryParse (info[0], out wd.level);
                int.TryParse (info[1], out wd.mapId);
                Vector2 pos = Vector2.one;
                float.TryParse (info[2], out pos.x);
                float.TryParse (info[3], out pos.y);
                wd.pointPos = pos;
                wd.levelData.word = info[4];
                wd.levelData.answers = info[5];
                wd.levelData.validWords = info[6];
                world_data.Add (wd);
            }
        }
        int chapterCount = world_data.Count / GameState.chapterLevelNum;
        int remainder = world_data.Count % GameState.chapterLevelNum;
        if (remainder > 0) {
            chapterCount++;
        }
        GameState.chapterCount = chapterCount;
        TextManager.Instance.ReadWorldRewardFile ();
       // CheckAnswer ();
    }

    //检查策划配置数据
    private void CheckAnswer() {
        for (int i = 0; i < world_data.Count; i++) {
            GameLevel level = world_data[i].levelData;
            string word = level.word;
            string[] answerInfo = level.answers.Split ('|');
            for (int j = 0; j < answerInfo.Length;j++) {
                char[] info = answerInfo[j].ToCharArray ();
                for(int k = 0; k < info.Length; k++) {
                    if (!word.Contains (info[k].ToString())) {
                        Debug.Log (string.Format("第{0}个关卡，答案存在误差 ： " ,(i+1)));
                    }
                }
            }
        }
    }


    public WorldData GetWorldData(int id) {
        return world_data[id - 1];
    }
    public int GetWordDataCount() {
        return world_data.Count;
    }
   
    //获取到当前点的坐标
    public Vector2 GetPointPos(int id) {
        return world_data[id].pointPos;
    }



    #endregion

    #region PictureData
    public void LoadPictureData() {
        StartCoroutine (GetCSVData (SetPath ("Picture.csv"), DataType.Picture));
    }
    /// <summary>
    /// 如果策划这里只需要一关的话，不需要全部读完，读指定行就可以了
    /// </summary>
    /// <param name="data"></param>
    public void ReadPictureData(string[] data) {
        for (int i = 1; i < data.Length; i++) {
            string[] info = data[i].Split (',');
            if (info.Length >= 5) {
                PictureData pic = new PictureData ();

                int.TryParse (info[0], out pic.id);
                pic.pictureName.Add (info[1]);
                pic.pictureName.Add (info[2]);
                pic.pictureName.Add (info[3]);
                pic.pictureName.Add (info[4]);
                pic.word = info[5];
                pictureData.Add (pic);
            }
        }
        if (ReadCompelte != null) {
            ReadCompelte ();
        }
    }
    public void LoadDailyData(string path, DataType type) {
        StartCoroutine (GetCSVData (path, type));
    }
    public void ReadCrossyData(string info) {

        crossyData = JsonMapper.ToObject<CrossyData> (info);
        if (ReadCompelte != null) {
            ReadCompelte ();
        }
    }

    public void ReadSearchData(string info) {
        searchData = JsonMapper.ToObject<SearchData> (info);
        //  Debug.Log ("Crossy Count "+);
        if (ReadCompelte != null) {
            ReadCompelte ();
        }
    }
    public void ReadPuzzleData(string info) {
        puzzleData = JsonMapper.ToObject<PuzzleData> (info);
        //  Debug.Log ("Crossy Count "+);
        if (ReadCompelte != null) {
            ReadCompelte ();
        }
    }
    #endregion

    #region AchievementData
    public void ReadAchievementData(string[] data) {
        
        for (int i = 1; i < data.Length; i++) {
            List<AchievementData> achieveData = new List<AchievementData> ();
            AchievementData achieve = new AchievementData ();
            string[] info = data[i].Split (',');
            if (info.Length >= 6) {
                int id = 0;
                int.TryParse (info[0], out id);
                achieve.describe = info[1];
                int.TryParse (info[2], out achieve.num);
                int.TryParse (info[3], out achieve.rewardType);
                int.TryParse (info[4], out achieve.rewardId);
                int.TryParse (info[5], out achieve.rewardNum);
                if (!achievement_data.ContainsKey (id)) {
                    achieveData.Add (achieve);
                    achievement_data.Add (id,achieveData);
                } else {
                    achieveData = achievement_data[id];
                    achieveData.Add (achieve);
                    achievement_data[id] = achieveData;
                }
            }
        }
    }

    //public void LoadAchieveData() {
    //    if (achievement_data.Count == 0) {
    //        StartCoroutine (GetCSVData (SetPath ("Achievement.csv"), DataType.Achievement));
    //    } else {
    //        if (ReadCompelte != null) {
    //            ReadCompelte ();
    //        }
    //    }
    //}
    #endregion

}
//章节奖励
public class StarReward {
    public List<RewardData> reward = new List<RewardData> ();
    public string des;//星座的描述
}
//签到
public class RewardData {
    public int rewardId;
    public int rewardNum;
}
//道具
public class ItemData {
    public int itemID;
    public string ItemName;
}
//刮刮卡
public class CardData {
    public string task;
    public int minNum;
    public int maxNum;
    public int rewardID;
    public int rewardNum;
    public int type;//card 刮奖的种类 0没有任何奖，1直接获取奖励，2 任务
    public int chance;// 概率
}
//老虎机
public class SlotChance {
    public int num;//数字
    public int chance;//概率
}
//主玩法数据
public class WorldData {
    public int level;//关卡id，现在基本有1000多关的样子
    public GameLevel levelData = new GameLevel ();
   
    public int mapId;//星座的id,新地图弃用了
    public Vector2 pointPos = new Vector2 ();//点在星座里面的位置,新地图弃用了
   
}

public class GameLevel {
    public string word;
    public string answers;
    public string validWords;
}
//每日挑战图片玩法
public class PictureData {
    public int id;
    public List<string> pictureName = new List<string> ();
    public string word;
}
//成就
public class AchievementData {
    public string describe;
    public int num;
    public int rewardType;
    public int rewardId;
    public int rewardNum;

}

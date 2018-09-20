using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Superpow;
using LitJson;

public class TextManager :MonoBehaviourSingleton<TextManager> {
    private string worldRewardPath = "CollectReward.text";
    private List<int> world_reward = new List<int> ();
    
    private void Awake() {
        m_instance = this;
    }

    #region 章节奖励相关

    public void ReadWorldRewardFile() {
      worldRewardPath=  SetPath (worldRewardPath);
        if (!File.Exists (worldRewardPath)) {

            FirstWriteWorldReward ();
        } else {
            ReadWorldReward ();
        }
    }

    private void FirstWriteWorldReward() {
        FileStream file = File.Create (worldRewardPath);
        StreamWriter sw = new StreamWriter (file);
        Prefs.unlockedWorld = Prefs.unlockedLevel / GameState.chapterLevelNum;
        PlayerDataManager.Instance.playerData.accumulativeCollectStar = Prefs.unlockedWorld;
        PlayerDataManager.Instance.SavePlayerProgress ();
        for (int i = 0; i < GameState.chapterCount; i++) {

            if (i < Prefs.unlockedWorld) {
                sw.Write (1);
                world_reward.Add (1);
            } else {
                sw.Write (0);
                world_reward.Add (0);
            }
            if (i < GameState.chapterCount - 1) {
                sw.Write (",");
            }

        }
          sw.Close ();
    }

    private void WriteWorldReward() {
        if (File.Exists (worldRewardPath)) {
            File.Delete (worldRewardPath);
        }
        FileStream file = File.Create (worldRewardPath);
        StreamWriter sw = new StreamWriter (file);
        for (int i = 0; i < world_reward.Count; i++) {
            sw.Write (world_reward[i]);
            if (i < world_reward.Count - 1) {
                sw.Write (",");
            }
        }
        sw.Close ();
    }

    private void ReadWorldReward() {
        if (File.Exists (worldRewardPath)) {
            FileStream file = new FileStream (worldRewardPath, FileMode.Open);
            StreamReader sr = new StreamReader (file);
            string strLine = sr.ReadLine ();
            string[] info = strLine.Split (',');
            for (int i = 0; i < info.Length; i++) {
                world_reward.Add (int.Parse (info[i]));
            }
            sr.Close ();
        }
    }

    public int ReturnWorldRewardState(int id) {
        return world_reward[id];
    }

    public void SetWorldRewardState(int id, int state) {
        world_reward[id] = state;
        WriteWorldReward ();
    }

    #endregion


    #region 保存每日挑战进度
   
    public void SaveProgress<T>(string path, T data) {
        path = SetPath (path);
        if (File.Exists (path)) {
            File.Delete (path);
        }
        FileStream file = File.Create (path);
        StreamWriter sw = new StreamWriter (file);
        string jsonData = JsonMapper.ToJson (data);
        sw.WriteLine (jsonData);
        sw.Close ();
    }

    public T ReadProgress<T>(string path) {
        path = SetPath (path);
        if (File.Exists (path)) {
            FileStream file = File.Open (path, FileMode.Open);
            StreamReader sr = new StreamReader (file);
            string line = sr.ReadLine ();
            sr.Close ();
            return JsonMapper.ToObject<T> (line);
        
        }
        return default(T);
    }

    public void  DeleteFile(string path) {
        path = SetPath (path);
        if (File.Exists (path)) {
            File.Delete (path);
        }
    }

    #endregion

    #region 成就
    public List<int> ReadAchievementRewardState(string path) {
        List<int> reward = new List<int> ();
        FileStream file = new FileStream (path, FileMode.Open);
        StreamReader sr = new StreamReader (file);
        string strLine = sr.ReadLine ();
        string[] info = strLine.Split (',');
        for (int i = 0; i < info.Length; i++) {
            reward.Add (int.Parse (info[i]));
        }
        return reward;
    }

    public void WriteAchievementRewardState(string path,List<int> reward) {
        if (File.Exists (path)) {
            File.Delete (path);
        }
        FileStream file = File.Create (path);
        StreamWriter sw = new StreamWriter (file);
        for (int i = 0; i < reward.Count; i++) {
            sw.Write (reward[i]);
            if (i < reward.Count - 1) {
                sw.Write (",");
            }
        }
        sw.Close ();
    }
    #endregion


   private string SetPath(string path) {
        //安卓平台
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
           path = Application.persistentDataPath +"/"+ path;

        }
        //windows编辑器
        else if (Application.platform == RuntimePlatform.WindowsEditor) {
            path = Application.streamingAssetsPath + "/" + path;
        }
        return path;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager :MonoBehaviourSingleton<CardManager> {
    [HideInInspector]
    public int card_gems;
    [HideInInspector]
    public int card_hint;
    [HideInInspector]
    public bool isTask=false;
    void Awake() {
        m_instance = this;
    }
    /// <summary>
    /// 随机设置刮刮卡的信息
    /// </summary>
    /// <param name="text"></param>
    public void SetCardInfo(Text text) {
        List<CardData> card_data = CSVReadManager.Instance.GetCardData ();
        if (card_data != null) {
            int num = Random.Range(0, 100);
            int currentRate = 0;
            int nextRate = 0;
            int id = 0;
            for (int i = 0; i < card_data.Count; i++) {
                if (i != 0) {
                    currentRate += card_data[i - 1].chance;
                }
                nextRate += card_data[i].chance;
                if (num >= currentRate && num < nextRate) {
                    id = i;
                    break;
                }
            }
            GetCount (card_data[id], text);
            Prefs.ScratchCardID = id;
        }
    }
   /// <summary>
   /// 随机获取任务个数
   /// </summary>
   /// <param name="data"></param>
   /// <param name="text"></param>
    void GetCount(CardData data, Text text) {
        int count = Random.Range (data.minNum, data.maxNum);
        Prefs.ScratchCardCount = count;
        if (data.type == 2) {
            text.text = data.task;
        } else if (data.type == 0) {
            isTask = true;
            text.text = string.Format (data.task, count);
        } else if (data.type == 1) {
            if (data.rewardID == 10001) {
                text.text = string.Format (data.task, count);
                card_gems = count * data.rewardNum;
            } else if (data.rewardID == 10002) {
                text.text = string.Format (data.task, count);
                card_hint = count * data.rewardNum;
            }
        }
    }
 
    public void CheckTask(int index) {
        int taskId = Prefs.ScratchCardID;
        if (Prefs.ScratchCardTaskFinish == 0 && index == taskId) {
            int totalCount = Prefs.ScratchCardCount;
            int count = Prefs.ScratchCardGameCount;
            count++;
            Prefs.ScratchCardGameCount = count;
            TaskManager._instance.UpdateText ();
            if (count == totalCount) {
                Prefs.ScratchCardTaskFinish = 1;
                TaskManager._instance.Hide ();
            }
        }
    }
}
   



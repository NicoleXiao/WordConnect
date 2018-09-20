using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SignRewardItem : MonoBehaviour {
    //   public ScrollRect rect;
    public Text title;
    public Image check;
    public Sprite currentSign;
    public Sprite done;
    public GameObject line;
    public Transform rewardGroup;
    private RectTransform lineTrans;
    private float currentLength = 0;//线段不断变化的长度
    private bool showLine = false;
    public void SetItemInfo(int day, int current_day, List<RewardData> data) {
        lineTrans = line.GetComponent<RectTransform> ();
        title.text = string.Format (title.text, day);
        bool isCurrentDay = day == current_day ? true : false;
        if (day < current_day) {
            line.SetActive (true);
        } else if (day == current_day) {
            //还没有签到
            if (Prefs.Signed == 0 || current_day == Prefs.SignDay + 1) {
                lineTrans.sizeDelta = new Vector2 (5, 0);
                check.sprite = currentSign;
            }
            if (current_day == Prefs.SignDay + 1) {
                SignRewardController._instance.tomorrowTag.SetActive (true);
            }
        } else {
            check.gameObject.SetActive (false);
        }
        //} else {
        //    if (Prefs.Signed == 1 && current_day + 1 == day) {
        //        check.sprite = currentSign;
        //    } else {
        //        check.gameObject.SetActive (false);
        //    }
        //}

        for (int i = 0; i < data.Count; i++) {
            if (data[i].rewardId == 10001) {
                if (isCurrentDay) {
                    SignRewardController._instance.gemsNum = SignRewardController._instance.gemsNum + data[i].rewardNum;
                }
            }
            if (data[i].rewardId == 10002) {
                if (isCurrentDay) {
                    SignRewardController._instance.hintNum = SignRewardController._instance.hintNum + data[i].rewardNum;
                }
            }
            rewardGroup.GetChild (i).GetComponent<RewardItem> ().SetInfo (CSVReadManager.Instance.ItemName (data[i].rewardId), data[i].rewardNum);

        }
    }
    public void SetLine() {
        SetDone ();
        line.SetActive (true);
        showLine = true;
    }
    public void SetDone() {
        check.sprite = done;
    }
    public void SetCheck() {
        check.sprite = currentSign;
        check.gameObject.SetActive (true);
    }
    private void Update() {
        if (showLine) {
            currentLength += 116 / 60f;
            if (currentLength <= 116) {
                lineTrans.sizeDelta = new Vector2 (lineTrans.sizeDelta.x, currentLength);
            } else if (currentLength > 116) {
                showLine = false;
                currentLength = 0;
                lineTrans.sizeDelta = new Vector2 (lineTrans.sizeDelta.x, 116);
                SignRewardController._instance.SetCheck (transform.GetSiblingIndex()+1);
                
            }
        }
    }

}



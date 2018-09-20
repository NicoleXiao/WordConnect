using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class AchievementItem : MonoBehaviour {
    public int id;
    public Image image;
    public Image box;
    public Sprite boxDark;
    public Sprite boxLight;

    //textbg先不处理，以后有需求再处理
    public RectTransform textBg;

    public Text describle;
    public Text progress;
    public Button rewardClick;
    public int completeIndex;
    public int showIndex; //要显示成就的下标
    public int currentNum;
    //金银铜摆放
    public List<Sprite> rewardSp = new List<Sprite> ();
    private List<int> rewardState = new List<int> ();
    private List<AchievementData> data = new List<AchievementData> ();
    private void Awake() {
        rewardClick.onClick.AddListener (()=> {
            Sound.instance.PlayButton ();
            SetRewardList ();
            SetBoxState ();
            WriteRewardState ();
        });
    }

    private void SetRewardList() {
        List<RewardData> rewardList = new List<RewardData> ();
        for(int i = 0; i < rewardState.Count; i++) {
            if (rewardState[i] == 1) {
                RewardData re = new RewardData ();
                re.rewardId = data[i].rewardId;
                re.rewardNum = data[i].rewardNum;
                rewardList.Add (re);
                rewardState[i] = 2;
            }
        }
        AchievementController.instance.ShowReward (rewardList);

    }
    public void SetInfo(int id,int completeIndex, List<AchievementData> data,int currentNum) {
        this.id =id;
        this.data = data;
        this.completeIndex = completeIndex;
        this.currentNum = currentNum;
        showIndex = completeIndex + 1 < data.Count  ? completeIndex + 1 : completeIndex;
        ShowAchieveInfo (showIndex);
        ReadRewardState ();
        SetBoxState ();
        ChangeInfoState (true);
    }
    private void ShowAchieveInfo(int index) {
        if (data[index].rewardType == 1) {
            image.sprite = rewardSp[0];
        } else if (data[index].rewardType == 2) {
            image.sprite = rewardSp[1];
        } else if (data[index].rewardType == 3) {
            image.sprite = rewardSp[2];
        }
        describle.text = string.Format (data[index].describe, data[index].num.ToString ());
        progress.text = string.Format ("{0}/{1}", currentNum, data[index].num);
      
    }

    private void ChangeInfoState(bool show) {
        box.gameObject.SetActive (show);
        textBg.gameObject.SetActive (show);
        progress.gameObject.SetActive (show);
        describle.gameObject.SetActive (show);
    }
    

    private void ReadRewardState() {
        string path = SetPath ("Achievement_"+id.ToString()+".txt");
        string folderPath = SetFolderPath ();
        if (!Directory.Exists (folderPath)) {
            Directory.CreateDirectory (folderPath);
        }
        if (File.Exists (path)) {
            rewardState = TextManager.Instance.ReadAchievementRewardState (path);
            //如果有数据变化就要做更新
            for(int i = 0; i < data.Count; i++) {
                if (i <= completeIndex) {
                    if (rewardState[i] == 0) {
                        rewardState[i] = 1;
                    }
                } else {
                    break;
                }
            }
        } else {
            //第一次保存数据
            for(int i = 0; i < data.Count; i++) {
                if (i <= completeIndex) {
                    rewardState.Add (1);
                } else {
                    rewardState.Add (0);
                }
            }
        }
    }

    private void SetBoxState() {
        if (rewardState.Contains (1)) {
            box.sprite = boxLight;
            rewardClick.enabled = true;
        } else {
            rewardClick.enabled = false;
            box.sprite = boxDark;
            //已经全部领取完毕
            if (!rewardState.Contains(0)) {
                box.gameObject.SetActive (false);
            }
        }
    }

    private void WriteRewardState() {
        string path = SetPath ("Achievement_" + id.ToString () + ".txt");
        TextManager.Instance.WriteAchievementRewardState (path, rewardState);
    }

    public void ShowOwn() {
        if (completeIndex != -1) {
            ChangeInfoState (false);
            ShowAchieveInfo (completeIndex);
            AchievementController.instance.ownCount++;
        } else {
            this.gameObject.SetActive (false);
        }
    }

    //回到总的成就界面
    public void BackAchieve() {
        ShowAchieveInfo (showIndex);
        ChangeInfoState (true);
        this.gameObject.SetActive (true);
    }

    private string SetPath(string path) {
        //安卓平台
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
            path = Application.persistentDataPath + "/Achieve/" + path;
        }
        //windows编辑器
        else if (Application.platform == RuntimePlatform.WindowsEditor) {
            path = Application.streamingAssetsPath + "/Achieve/" + path;
        }
        return path;
    }
    private string SetFolderPath() {
        string path = "";
        //安卓平台
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
            path = Application.persistentDataPath + "/Achieve";
        }
        //windows编辑器
        else if (Application.platform == RuntimePlatform.WindowsEditor) {
            path = Application.streamingAssetsPath + "/Achieve";
        }
        return path;

    }
}

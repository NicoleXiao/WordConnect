using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SelectWorldItem : MonoBehaviour {
    //public Text name;
    //public Image reward;
    //public int id;//当前章节的id
    //private Button reward_btn;
    //public List<Sprite> reward_sps = new List<Sprite> ();//有三种状态 0- 不可领取 1-可以领取 2-领取完
    //private void Awake() {
    //    reward.GetComponent<Animator> ().enabled = false;

    //}
    //private void Start() {

    //    reward_btn = reward.GetComponent<Button> ();
    //    reward_btn.onClick.AddListener (() => {
    //        ClickReward ();
    //    });
    //    SetRewardState ();
    //}
    //void SetRewardState() {
    //    reward.GetComponent<Button> ().enabled = false;
    //    int type = TextManager.Instance.ReturnWorldRewardState (id - 1);
    //    if (type == 0) {
    //        reward.sprite = reward_sps[0];
    //        reward_btn.enabled = false;
    //    }
    //    if (type == 1) {
    //        reward.sprite = reward_sps[1];
    //        reward_btn.enabled = true;

    //    }
    //    if (type == 2) {
    //        reward.sprite = reward_sps[2];
    //        reward_btn.enabled = false;
    //    }
    //}
    //void ClickReward() {
    //    SelectWorldSliderBarManager._instance.rubyTrans.parent.transform.SetAsLastSibling ();
    //    SelectWorldSliderBarManager._instance.itemGroup.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0,634);
    //    Sound.instance.PlayButton ();
    //    TextManager.Instance.SetWorldRewardState (id - 1, 2);
    //    reward_btn.enabled = false;
    //    reward.sprite = reward_sps[2];
    //    List<RewardData> data = CSVReadManager.Instance.GetWorldRewardData (id);
    //    for (int i = 0; i < data.Count; i++) {
    //        if (data[i].rewardId == 10001) {
                
    //            LoadGems (data[i].rewardNum);
    //        }
    //        if (data[i].rewardId == 10002) {
    //            Prefs.HintNum = Prefs.HintNum + data[i].rewardNum;
    //        }
    //    }
    //   // EventTrackingController.instance.LogChapterRewardEvent ();
    //}
    //void LoadGems(int gemsNum) {
    //    GemsEffet effet = EffectManager.Instance.LoadGems (GameObject.Find ("Header").transform, reward.transform.position, gemsNum);
    //    effet.OnEffectComplete += () => {
    //        CurrencyController.CreditBalance (gemsNum);
    //        SelectWorldSliderBarManager._instance.rubyTrans.parent.transform.SetAsFirstSibling ();
    //        SelectWorldSliderBarManager._instance.itemGroup.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, 741);
    //        Destroy (effet.gameObject);
    //    };


    //}

    //IEnumerator ShowRewardShake() {
    //    yield return new WaitForSeconds (1f);
    //    reward.GetComponent<Animator> ().enabled = true;

    //}

}

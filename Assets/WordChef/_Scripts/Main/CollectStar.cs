using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Superpow;
using DG.Tweening;

public class CollectStar : MonoBehaviour {
    public int id;
    public Text starName;

    public Image graph;//放在下面的图形，透明度根据点亮的星星个数来调整
    public GameObject notUnlockBg;
    public GameObject unLockBg;
    public Button box;
    public Image boxImage;
    public Image boxLight;
    public Sprite dark;

    private void Start() {
        box.onClick.AddListener (() => {
            ClickReward ();
        });
    }
    public void SetTotalCount(int id, Sprite bg, bool isPass, string name) {
        if (!isPass) {
            unLockBg.SetActive (false);
            notUnlockBg.SetActive (true);
            return;
        } else {
            graph.sprite = bg;
        }
        starName.text = name;
        this.id = id;
        int state = TextManager.Instance.ReturnWorldRewardState (id);
        if (state == 2) {
            box.gameObject.SetActive (false);
        } else if (state == 0) {
            box.enabled = false;
            boxImage.sprite = dark;
        } else {
            boxLight.DOFade (1f,0.8f).SetLoops(-1,LoopType.Yoyo);
        }

    }

    /// <summary>
    /// 已经解锁的关卡,不会再显示button
    /// 参数是是否是正在解锁的关卡
    /// </summary>
    /// <param name="unlock"></param>
    public void SetCollectStarButtonState() {
        ShowInfo();
        if (id == 0) {
            FirstGraph ();
        } else {
            CalculateAlpha ();
        }
        
    }
    private void CalculateAlpha() {
        float progress = (float)(GameState.starLightCount) / (float)GameState.starTotalCount;
        float alpha = progress > 0 ? progress : 0;
        alpha = progress * progress;
        graph.color = new Color (1, 1, 1, alpha);
       
    }

    private void FirstGraph() {
        float alpha = 0f;
        if (GameState.starLightCount <= 0) {
            alpha = 0.3f;
        } else if (GameState.starLightCount <= 3) {
            alpha = 0.3f + GameState.starLightCount * 0.2f;
        } else {
            alpha = 0.9f; 
            alpha += 0.1f / (float)(GameState.starTotalCount - 1) * (GameState.starLightCount - 1);
        }
            graph.color = new Color (1, 1, 1, alpha);
        
    }



    public void ShowInfo() {
        StarCollectDialog._instance.selectBg.gameObject.SetActive (true);
        StarCollectDialog._instance.selectBg.SetParent(this.transform,true);
        StarCollectDialog._instance.selectBg.localPosition = Vector3.one;
        StarCollectDialog._instance.SetStarInfo (starName.text, id);
    }
    void ClickReward() {
        Sound.instance.PlayButton ();
        List<RewardData> data = new List<RewardData> ();
        data = CSVReadManager.Instance.GetWorldRewardData (StarCollectDialog._instance.GetMapId (id));
        TextManager.Instance.SetWorldRewardState (id, 2);
        for (int i = 0; i < data.Count; i++) {
            if (data[i].rewardId == 10001) {
                LoadGems (data[i].rewardNum);
            }
            if (data[i].rewardId == 10002) {
                Prefs.HintNum = Prefs.HintNum + data[i].rewardNum;
            }
        }
        box.enabled = false;
        EventTrackingController.instance.LogChapterRewardEvent ();
    }
    
    void LoadGems(int number) {
        GemsEffet effet = EffectManager.Instance.LoadGems (StarCollectDialog._instance.transform, box.transform.position,number);
        effet.OnEffectComplete += () => {
            CurrencyController.CreditBalance (number);
            EventTrackingController.instance.LogGemsAdd (number, "chapterreward");
            box.gameObject.SetActive (false);
            Destroy (effet.gameObject);
        };


    }
}

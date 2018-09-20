using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using Superpow;

public class WorldGroupManager : MonoBehaviour {
    public RectTransform content;
    public Transform group;
    public MapScrollRect rect;
    //首尾的两个物体
    public RectTransform startRect;
    public RectTransform endRect;

    public GameObject opeBtnGroup;
    public GameObject cardAnim;
    public GameObject mask;

    public Image progress;
    public Image bookLight;
    public Text starName;
    public Text percentText;
    public int currentLevelIndex = 1;
    public List<Sprite> graphBg = new List<Sprite> ();
    public List<StarManager> chapter = new List<StarManager> ();
    [HideInInspector]
    public Transform rewardFly;//当前关卡
    [HideInInspector]
    public float alpha = 0;//图形的alpha
    [HideInInspector]
    public Image graph;
    private float contentOffset = 0;
    private float startOffset = 0;//开头有个start的图片
    private int posIndex = 0;//当前在屏幕中间的章节下标
    private bool slideDown = false;//下滑了
    private bool slideUp = false;//上滑了                           
    public static WorldGroupManager _instance;

    private void Awake() {
        _instance = this;
        contentOffset = content.anchoredPosition.y;
        startOffset = startRect.sizeDelta.y;

    }

    private void Start() {
        ShowMap ();
        rect.onValueChanged.AddListener (OnValueChangedEvent);
        if (Prefs.UnLockNewGrahp == 1) {
            bookLight.DOFade (1f, 0.5f).SetLoops (-1, LoopType.Yoyo);
        }

    }

    /// <summary>
    ///现在没有表现效果了。玩家滑到哪里就是哪里
    ///先把首尾的图形克隆出来。选择不摧毁
    ///放置物体的时候，start永远在第一个物体 下标为0。end永远在最下面。
    /// </summary>
    void ShowMap() {
        int currentIndex = (GameState.currentLevel - 1) / GameState.chapterLevelNum;
        posIndex = currentIndex;
        RectTransform start = Instantiate (startRect, group);
        start.anchoredPosition = new Vector2 (-1, -1090);
        //第一个图形要单独处理
        if (currentIndex == 0) {
            CloneGrah (0);
            CloneGrah (1);
        } else if (currentIndex == GameState.chapterCount - 1) {//最后一个图形

            currentLevelIndex = (currentIndex - 1) * GameState.chapterLevelNum + 1;
            CloneGrah (currentIndex - 1);
            CloneGrah (currentIndex);

        } else {
            currentLevelIndex = (currentIndex - 1) * GameState.chapterLevelNum + 1;
            CloneGrah (currentIndex - 1);
            CloneGrah (currentIndex);
            CloneGrah (currentIndex + 1);
        }

        RectTransform end = Instantiate (endRect, group);
        end.anchoredPosition = new Vector2 (-1, 67216);

        //如果解锁了新的图形，那么就先显示之前关卡图形的变化，然后移动到当前的
        if (PlayerDataManager.Instance.playerData.FirstEnterMap) {
            PlayerDataManager.Instance.playerData.FirstEnterMap = false;
            PlayerDataManager.Instance.SavePlayerProgress ();
        } else if (GameState.firstEnterMap) {
            GameState.firstEnterMap = false;
            SetContentPos ();

        } else {
            if (PlayerDataManager.Instance.contentPos != Vector2.zero) {
                content.anchoredPosition = PlayerDataManager.Instance.contentPos;
            }
        }
        if (GameState.unLockNewGrahp) {
            PlayerDataManager.Instance.playerData.accumulativeCollectStar++;
            PlayerDataManager.Instance.JudeReachAchieve (8, PlayerDataManager.Instance.playerData.accumulativeCollectStar);
            GameState.unLockNewGrahp = false;
            mask.SetActive (true);
            cardAnim.SetActive (true);
        } else {
            ShowMapEffect ();
        }
        GameState.unlockNewLevel = false;
    }
    public void ShowMapEffect() {
        cardAnim.SetActive (false);
        mask.SetActive (false);
        ChangeGraphAlpha ();
        ShowGems ();

    }

    public void ClickBook() {
        Sound.instance.PlayButton ();
        bookLight.DOKill ();
        bookLight.gameObject.SetActive (false);
        Prefs.UnLockNewGrahp = 0;
        DialogController.instance.ShowDialog (DialogType.StarCollection, DialogShow.REPLACE_CURRENT);
    }

    private void OnValueChangedEvent(Vector2 value) {
        CalculateContentAndAddChild ();
    }

    /// <summary>
    /// 重新计算两个的位置
    /// contetn向下滑是 -640去减-1280，group里面的物体位置是加1280
    /// content初始位置是contentOffset
    /// 第一个物体的初始值是0.
    /// </summary>
    void CalculateContentAndAddChild() {
        int currentPosIndex = 0;
        float currentLevelPos = CalculateContentY (posIndex);
        float offset =  content.anchoredPosition.y-currentLevelPos;
        //上滑  并且到达图片的1/3处的时候。判断到了这个图片
        if (Math.Abs (offset) > 640) {
            if (offset < 0) {
                slideUp = true;
                currentPosIndex = posIndex + 1;
            } else {
                slideDown=true;
                currentPosIndex = posIndex - 1;
            }
        }

        if (slideUp) {
            SliderUpClone (currentPosIndex);
        }
        if (slideDown) {
            SliderDownClone (currentPosIndex);
        }
    }

    /// <summary>
    /// 向下滑，pos小于0没有可以滑的了不作处理，pos大于0,可以滑
    /// 因为end要发放在最后。所以新克隆的图形要放在end的上一层
    /// 要保证除去start和end，还有3个物体存在。
    /// </summary>
    /// <param name="currentPosIndex"></param>
    private void SliderDownClone(int currentPosIndex) {
        slideDown = false;
        int pos =currentPosIndex - 1;
        if (pos >= 0) {
            posIndex = currentPosIndex;
            currentLevelIndex = pos * GameState.chapterLevelNum + 1;
            CloneGrah ( pos).SetSiblingIndex (1);
            if (group.childCount == 6) {
                Destroy (group.GetChild (group.childCount - 2).gameObject);
            }
        }

    }

    /// <summary>
    /// 向上滑，不超过图形的最大值
    /// 因为end要发放在最后。所以新克隆的图形要放在end的上一层
    /// 要保证除去start和end，还有3个物体存在。
    /// </summary>
    /// <param name="currentPosIndex"></param>
    private void SliderUpClone(int currentPosIndex) {
        slideUp = false;
        int pos = currentPosIndex + 1;
        if (pos <= GameState.chapterCount - 1) {
            posIndex = currentPosIndex;
            currentLevelIndex = pos * GameState.chapterLevelNum + 1;
            CloneGrah (pos).SetSiblingIndex (group.childCount - 2);
            if (group.childCount == 6) {
                Destroy (group.GetChild (1).gameObject);
            }
        }
    }

    private int ReturnIndex(float currentY) {
        return Mathf.Abs ((int)(currentY - contentOffset + startOffset) / 1280);
    }

    private float CalculateContentY(int index) {
        return contentOffset - index * 1280 - startOffset;
    }


    private string GetBgUrl(int index) {
        return Utils.ChapterBgUrl (index);
    }


    /// <summary>
    /// pos位置坐标
    /// index只的是克隆的第几个物体
    /// newLevel判断是否是新关卡
    /// </summary>
    /// <param name="index"></param>
    /// <param name="newLevel"></param>
    /// <param name="up"></param>
    Transform CloneGrah(int pos) {
        StarManager clone = Instantiate (chapter[pos % chapter.Count], group);
        clone.transform.localScale = Vector3.one;
        clone.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, pos * 1280 + (contentOffset + startOffset));
        clone.GetComponent<StarManager> ().SetStarBg (GetBgUrl (pos % chapter.Count), graphBg[pos % graphBg.Count]);
        clone.GetComponent<StarManager> ().SetButtonState (pos == Prefs.unlockedWorld);
        return clone.transform;
    }

    public void SetContentPos() {
       content.anchoredPosition = new Vector2 (content.anchoredPosition.x, CalculateContentY (posIndex));
    }


    public void ChangeGraphAlpha() {
       // group.GetChild (0).GetComponent<StarManager> ().graph.DOFade (alpha, 0.5f);
        if (graph != null) {
            graph.DOFade (alpha, 0.5f).SetDelay(1f);
        }

    }

    public void SetSlider() {
        float percent = (float)GameState.starLightCount / (float)GameState.starTotalCount;
        progress.fillAmount = percent;
        percentText.text = ((int)(percent * 100)).ToString () + "%";
    }

    public void ShowGems() {
        if (rewardFly == null) {
            return;
        }
        Transform rubyBalance = GameObject.FindWithTag ("RubyBalance").transform;
        rewardFly.SetParent (GameObject.Find("SelectWorld").transform);
        rewardFly.SetAsLastSibling ();
        rewardFly.DOMove (rubyBalance.position, 0.8f).SetDelay(1f).OnComplete (() => {
            rewardFly.DOScale (0, 0.01f).OnComplete (() => {
                Destroy (rewardFly.gameObject);
            });
            CurrencyController.CreditBalance (1);
            EventTrackingController.instance.LogGemsAdd (1, "levelcrosslinereward");

        });
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AchievementController : BaseController {
    public Transform achievementGroup;

    public RectTransform rewardGroup;
    public RewardItem rewardItem;
    public GameObject mask;
    public Text page;
    public ScrollRect rect;
    public Button preBtn;
    public Button nextBtn;
    public Button ownBtn;
    public Button backBtn;
    public int  ownCount;

    private float rewardGroupStartY;
    private int currentNum = 0;
    private int currentPage = 1;
    private int totalPage = 3;
    private float startPosX;
    private float pageWidth = 640;
    private RectTransform achieveRect;
    public static AchievementController instance;
    protected override void Awake() {
        base.Awake ();
        instance = this;
        rewardGroupStartY = rewardGroup.anchoredPosition.y;
        achieveRect = achievementGroup.GetComponent<RectTransform> ();
        preBtn.onClick.AddListener (() => {
            ClickPre ();
        });
        nextBtn.onClick.AddListener (() => {
            ClickNext ();
        });
        ownBtn.onClick.AddListener (() => {
            achieveRect.anchoredPosition = new Vector2 (startPosX, achieveRect.anchoredPosition.y);
            currentPage = 1;
            backBtn.gameObject.SetActive (true);
            ownBtn.gameObject.SetActive (false);
            ShowOwnAndBack (false);
            SetPage ();
        });
        backBtn.onClick.AddListener (() => {
            achieveRect.anchoredPosition = new Vector2 (startPosX, achieveRect.anchoredPosition.y);
            currentPage = 1;
            ownBtn.gameObject.SetActive (true);
            backBtn.gameObject.SetActive (false);
            ShowOwnAndBack (true);
            SetPage ();
        });
        startPosX = achieveRect.anchoredPosition.x;
        rect.onValueChanged.AddListener (OnValueChangedEvent);
        SetRewardCard ();
    }

    private void SetPage() {
        if (currentPage != 1) {
            preBtn.transform.parent.gameObject.SetActive (true);
        } else {
            preBtn.transform.parent.gameObject.SetActive (false);
        }
        if (currentPage != totalPage) {
            nextBtn.transform.parent.gameObject.SetActive (true);
        } else {
            nextBtn.transform.parent.gameObject.SetActive (false);
        }
        page.text = string.Format ("{0}/{1}", currentPage, totalPage);
    }

    private void OnValueChangedEvent(Vector2 value) {
        if (achieveRect.anchoredPosition.x - startPosX < 0 && achieveRect.anchoredPosition.x> startPosX-(totalPage-1)*pageWidth) {
            currentPage = Mathf.Abs (Mathf.RoundToInt ((achieveRect.anchoredPosition.x - startPosX) / pageWidth)) + 1;
            SetPage ();
        }
    }

    //显示已经获得的奖励
    private void ShowOwnAndBack(bool back) {
        Sound.instance.PlayButton ();
        ownCount = 0;
        for (int i = 0; i < achievementGroup.childCount; i++) {
            if (back) {
                achievementGroup.GetChild (i).GetComponent<AchievementItem> ().BackAchieve ();
            } else {
                achievementGroup.GetChild (i).GetComponent<AchievementItem> ().ShowOwn ();
            }
        }
        float x = 1900;
        if (!back) {
            totalPage = ((ownCount - 1) / 6 + 1);
            x = totalPage * pageWidth;
        } else {
            totalPage = 3;
        }
        achieveRect.sizeDelta = new Vector2 (x, achieveRect.sizeDelta.y);
    }

 

    //前一页
    public void ClickPre() {
        Sound.instance.PlayButton ();
        if (currentPage == 1) {
            return;
        }
        rect.enabled = false;
        currentPage--;
        SetPage ();
        float currentX  = -pageWidth*(currentPage-1)+startPosX;
        achieveRect.DOAnchorPosX (currentX, 0.5f).OnComplete (() => {
            rect.enabled = true;
        });
    }

    public void ClickNext() {
        Sound.instance.PlayButton ();
        if (currentPage == totalPage) {
            return;
        }
        rect.enabled = false;
        currentPage++;
        SetPage ();
        float currentX = -pageWidth * (currentPage - 1) + startPosX;
        achieveRect.DOAnchorPosX (currentX, 0.5f).OnComplete (() => {
            rect.enabled = true;
        });
    }

    public void ShowReward(List<RewardData> rewardList) {
        if (rewardList.Count != 0) {
            rewardGroup.gameObject.SetActive (true);
            mask.SetActive (true);
        }
        for (int i = 0; i < rewardList.Count; i++) {
            if (i < rewardGroup.childCount) {
                rewardGroup.GetChild (i).GetComponent<RewardItem> ().SetInfo (CSVReadManager.Instance.ItemName (rewardList[i].rewardId), rewardList[i].rewardNum);
                rewardGroup.GetChild (i).GetComponent<RewardItem> ().Show ();
            } else {
                RewardItem item = Instantiate (rewardItem, rewardGroup.transform);
                item.SetInfo (CSVReadManager.Instance.ItemName (rewardList[i].rewardId), rewardList[i].rewardNum);
            }
        }
        rewardGroup.DOAnchorPosY (320, 1f);
        StartCoroutine (Fade ());
    }

    IEnumerator Fade() {
        yield return new WaitForSeconds (1f);
        for (int i = 0; i < rewardGroup.transform.childCount; i++) {
            rewardGroup.transform.GetChild (i).GetComponent<RewardItem> ().DOFade (0.5f);
            yield return new WaitForSeconds (0.5f);
            rewardGroup.DOKill ();
            rewardGroup.DOAnchorPosY (120+ rewardGroup.anchoredPosition.y, 0.5f);
        }
        yield return new WaitForSeconds (0.5f);
        rewardGroup.gameObject.SetActive (false);
        rewardGroup.anchoredPosition = new Vector2 (rewardGroup.anchoredPosition.x,rewardGroupStartY);
        mask.SetActive (false);
    }

    public void SetRewardCard() {
        for (int i = 1; i <= CSVReadManager.Instance.achievement_data.Count; i++) {
            currentNum = 0;
            List<AchievementData> data = CSVReadManager.Instance.achievement_data[i];
            if (i <= achievementGroup.childCount) {
                AchievementItem item = achievementGroup.GetChild (i - 1).GetComponent<AchievementItem> ();
                int levelIndex = JudgeAchievement (i, data);
                item.SetInfo (i, levelIndex, data, currentNum);
            }

        }
    }




    /// <summary>
    /// id 为 1 通过关数
    /// id 为 2 获得fantastic个数
    /// id 为 3  累计登录天数
    /// id 为 4  连续登录天数
    /// id 为 5  累计连接单词个数
    /// id 为 6  累计完成每日挑战次数
    /// id 为 7  累计获得小星星
    /// id 为 8 累计获得星座
    /// id 为 9 累计找到额外单词个数
    /// id 为 10 完成search次数
    /// id 为 11 完成picture次数
    /// id 为 12 完成crossy次数
    /// id 为 13 完成puzzle次数
    /// id 为 14 slot的bouns次数
    /// id 为 15 累计找到word单词
    /// id 为 16 累计找到connect的个数
    /// </summary>
    private int JudgeAchievement(int id, List<AchievementData> data) {
        currentNum = 0;
        switch (id) {
            case 1:
                currentNum = PlayerDataManager.Instance.playerData.passLevel;
                break;
            case 2:
                currentNum = PlayerDataManager.Instance.playerData.fantasticCount;
                break;
            case 3:
                currentNum = PlayerDataManager.Instance.playerData.loginDayCount;
                break;
            case 4:
                currentNum = PlayerDataManager.Instance.playerData.continuousLoginCount;
                break;
            case 5:
                currentNum = PlayerDataManager.Instance.playerData.accumulativeLinkWord;
                break;
            case 6:
                currentNum = PlayerDataManager.Instance.playerData.accumulativeChallenge;
                break;
            case 7:
                currentNum = PlayerDataManager.Instance.playerData.accumulativeStar;
                break;
            case 8:
                currentNum = PlayerDataManager.Instance.playerData.accumulativeCollectStar;
                break;
            case 9:
                currentNum = PlayerDataManager.Instance.playerData.accumulativeExtraWord;
                break;
            case 10:
                currentNum = PlayerDataManager.Instance.playerData.searchCount;
                break;
            case 11:
                currentNum = PlayerDataManager.Instance.playerData.pictureCount;
                break;
            case 12:
                currentNum = PlayerDataManager.Instance.playerData.crossyCount;
                break;
            case 13:
                currentNum = PlayerDataManager.Instance.playerData.puzzleCount;
                break;
            case 14:
                currentNum = PlayerDataManager.Instance.playerData.slotBounsCount;
                break;
            case 15:
                currentNum = PlayerDataManager.Instance.playerData.wordCount;
                break;
            case 16:
                currentNum = PlayerDataManager.Instance.playerData.connectCount;
                break;

        }
        return JudgeAchievementLevel (data, currentNum);
    }

    //判断到达哪个等级了 不是金银铜，而是配置表里面第几个阶段
    private int JudgeAchievementLevel(List<AchievementData> data, int num) {
        int index = -1;
        for (int i = 0; i < data.Count; i++) {
            if (num >= data[i].num) {
                if (i + 1 < data.Count) {
                    if (num < data[i + 1].num) {
                        index = i;
                        break;
                    }
                } else {
                    index = i;
                    break;
                }
            }
        }
        if (index == 0) {
            ownBtn.gameObject.SetActive (true);
        }
        return index;
    }
}

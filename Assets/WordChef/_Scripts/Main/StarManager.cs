using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StarManager : MonoBehaviour {
    public Image bg;
    public Image graph;//放在下面的图形，透明度根据点亮的星星个数来调整
    public int totalCount;//图形里面btn的数量
    public List<LevelItem> levelItems = new List<LevelItem> ();

    private int passCount;//点亮的数量

    public void SetStarBg(string bgurl,Sprite graphSp) {
        ResManager.Instance.LoadSpriteFromStreamingAsset (bgurl, delegate (Sprite sp) {
            bg.sprite = sp;
        }, new Vector2 (0.5f, 0.5f), false);
        graph.sprite = graphSp;
        graph.SetNativeSize ();
        graph.gameObject.SetActive (true);
    }
    /// <summary>
    /// 参数是是否是正在解锁的关卡  1060 是现在策划配置的最大关卡数
    /// </summary>
    /// <param name="unlock"></param>
    public void SetButtonState(bool isUnLock) {
        for (int i = 0; i < levelItems.Count; i++) {
            int levelId = WorldGroupManager._instance.currentLevelIndex;
            if (levelId <= 1060) {
                levelItems[i].gameObject.SetActive (true);
                bool isPass = false;
                if (levelId < Prefs.unlockedLevel) {
                    isPass = true;
                    passCount++;
                }
                levelItems[i].SetLevelInfo (levelId, isPass);
                if (CSVReadManager.Instance.rewardLevel.Contains (levelId)) {
                    if (levelId >= Prefs.unlockedLevel) {
                        levelItems[i].reward.gameObject.SetActive (true);
                    } else if (levelId == Prefs.unlockedLevel-1 && GameState.unlockNewLevel) {
                        levelItems[i].reward.gameObject.SetActive (true);
                        WorldGroupManager._instance.rewardFly = levelItems[i].reward;
                    }
                }
                WorldGroupManager._instance.currentLevelIndex++;
            }
        }
        SetAlpha (isUnLock);

    }
    /// <summary>
    /// 显示图形的alpha值。第一个章节的图形要单独处理，因为想要显示alpha的速度快一点
    /// </summary>
    /// <param name="isUnlock"></param>
    private void SetAlpha(bool isUnlock) {
        if (isUnlock) {
            GameState.starLightCount = passCount;
            GameState.starTotalCount = totalCount;
            float nextProgress = (float)passCount / (float)totalCount;
            if (GameState.unlockNewLevel) {
                WorldGroupManager._instance.graph = graph;
                if (Prefs.unlockedWorld == 0) {
                    FirstGraph (passCount - 1, false);
                    FirstGraph (passCount, true);
                } else {
                    float preProgress = (float)(passCount - 1) / (float)totalCount;
                    CalculateAlpha (preProgress, false);
                    CalculateAlpha (nextProgress, true);
                }
            } else {
                if (Prefs.unlockedWorld == 0) {
                    FirstGraph (passCount, false);
                } else {
                    CalculateAlpha (nextProgress, false);
                }
            }
            WorldGroupManager._instance.SetSlider ();
        } else {
            if (totalCount == passCount) {
                graph.color = new Color (1, 1, 1, 1);
            }
        }
    }

    private void CalculateAlpha(float progress, bool change) {
        float alpha = progress > 0 ? progress : 0;
        alpha = progress * progress ;
        if (!change) {
            graph.color = new Color (1, 1, 1, alpha);
        } else {
            WorldGroupManager._instance.alpha = alpha;
        }
    }

    private void FirstGraph(int count, bool change) {
        float alpha = 0f;
        if (count <= 0) {
            alpha = 0.3f;
        } else if (count <= 3) {
            alpha = 0.3f+count*0.2f;
        } else {
            alpha = 0.9f;
            alpha += 0.1f / (float)(totalCount - 1) * (count - 1);
        }
        if (change) {
            WorldGroupManager._instance.alpha = alpha;
        } else {
            graph.color = new Color (1, 1, 1, alpha);
        }
    }

 

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LevelItem : MonoBehaviour {
    public Image head;//facebook的头像
    public Image levelBg;
    public Sprite currentSelectSp;
    public Sprite notPassSp;
    public Sprite passedSp;
    public Text levelNum;
    public Transform reward;
    public int levelId;
    public Button click;
    private void Awake() {
       click.onClick.AddListener (() => {
           //记录玩家滑动位置
           PlayerDataManager.Instance.contentPos = WorldGroupManager._instance.content.anchoredPosition;
            GameState.currentLevel = levelId;
            CUtils.LoadScene (Const.SCENE_MAINGAME, true);
            Sound.instance.PlayButton ();
        });
    }
    public void SetLevelInfo(int level,bool isPass) {
        levelId = level;
        levelNum.text = level.ToString ();
        if (isPass && levelId != Prefs.unlockedLevel) {
            levelBg.sprite = passedSp;
            levelBg.raycastTarget = true;
        } else if(levelId == Prefs.unlockedLevel) {
            levelBg.raycastTarget = true;
            levelBg.sprite = currentSelectSp;
        } else {
            levelBg.raycastTarget = false;
            levelBg.sprite = notPassSp;
        }
    }
    //public void ShowUnlockAnim(System.Action animEnd) {
    //    levelBg.DOFade (0f,0.3f).OnComplete(()=> {
    //        levelBg.sprite = passedSp;
    //        levelBg.DOFade (1f,0.3f).OnComplete(()=> {
    //            if (animEnd != null) {
    //                animEnd ();
    //            }
    //        });
    //    });
    //}
}

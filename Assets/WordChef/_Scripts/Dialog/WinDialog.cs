using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class WinDialog : Dialog {
    public Button nextClick;
    public Animator titleAnim;
    public Image lightImage;
    public Transform progressGroup;
    public Text starText;
    private int numLevels;
    private bool isLastLevel;
    private int subWorld, level;
    private int currentWorld, currentLevel;
    public static WinDialog instance;
    protected override void Awake() {
        base.Awake ();
        instance = this;
        nextClick.enabled = false;
    }
    protected override void Start() {
        base.Start ();
        int mOldScore = Prefs.StarCount;
        int newScore = Prefs.StarCount + MainController.instance.starCount;
        Prefs.StarCount = MainController.instance.starCount + Prefs.StarCount;
        Sequence mScoreSequence = DOTween.Sequence ();
        mScoreSequence.Append (DOTween.To (delegate (float value) {
            var temp = Mathf.Floor (value);
            starText.text = temp + "";
        }, mOldScore, newScore, 0.4f));
        CheckUnlock ();
        SetProgress ();
    }
    void SetProgress() {
        for (int i = 0; i < progressGroup.childCount; i++) {
            float index = GameState.passLevelCount - 1 >= 0 ? GameState.passLevelCount - 1 : 0;

            if (i < index) {
                progressGroup.GetChild (i).gameObject.SetActive (true);
            } else {
                progressGroup.GetChild (i).gameObject.SetActive (false);
            }
        }
        StartCoroutine (ShowProgress ());
    }
    IEnumerator ShowProgress() {
        yield return new WaitForSeconds (0.5f);
        progressGroup.GetChild (GameState.passLevelCount - 1).gameObject.SetActive (true);
        if (GameState.passLevelCount == 10) {
            GameState.passLevelCount = 0;
            Prefs.PassLevelCount = 0;
            lightImage.DOFade (1f, 0.5f).SetLoops (2, LoopType.Yoyo).OnComplete (() => {
                DialogController.instance.ShowDialog (DialogType.Slot);
            });
        } else {
            nextClick.enabled = true;
        }
    }
    private void CheckUnlock() {
        if (Prefs.unlockedLevel >= 16) {
            if (GameState.passLevelCount != 10 && CUtils.IsInterstitialReady ()) {
                Timer.Schedule (this, 0.3f, () => {
                    CUtils.ShowInterstitialAd ();
                });

            }
        }
        level = GameState.currentLevel;
        currentLevel = level;
        currentWorld = Prefs.unlockedWorld;
        //中间来处理事件
        if (Prefs.IsLastLevel ()) {
            TextManager.Instance.SetWorldRewardState (Prefs.unlockedWorld, 1);
            if (Prefs.unlockedLevel <= CSVReadManager.Instance.GetWordDataCount ()) {
                Prefs.unlockedWorld = Prefs.unlockedWorld + 1;
                Prefs.UnLockNewGrahp = 1;
                GameState.unLockNewGrahp = true;
            }
        }
        level++;
        if (level > Prefs.unlockedLevel && Prefs.unlockedLevel < CSVReadManager.Instance.GetWordDataCount ()) {
            GameState.unlockNewLevel = true;
            Prefs.unlockedLevel = level;
            GameState.currentLevel = level;
        } else {
            nextClick.enabled = true;
        }


    }
    public void ClickMenu() {
        GameState.unlockNewLevel = false;
        CUtils.LoadScene (Const.SCENE_MAP, true);
        Close ();
    }


    /// <summary>
    ///策划新的需求不管有没有开启新的关卡，都要让玩家进入到地图里面去。
    /// </summary>
    /// <param name="isAddScene"></param>
    public void NextClick(bool isAddScene = true) {
        Close ();
        Sound.instance.PlayButton ();
        if (Prefs.ScratchCardTaskFinish == 0) {
            CardManager.Instance.CheckTask (6);
        }

        //if (GameState.unlockNewLevel) {
        //    //删除数据
        //    // CUtils.LoadSceneAsy (this, 1, true);
        //    CUtils.LoadScene (Const.SCENE_MAP, true);
        //} else {
        //    //删除数据
        //    GameState.currentLevel = level;
        //    CUtils.LoadScene (Const.SCENE_MAINGAME, true);
        //}
        CUtils.LoadScene (Const.SCENE_MAP, true);
        if (GameState.currentLevel == Prefs.unlockedLevel) {
            EventTrackingController.instance.LogLevelProcess (false, currentWorld.ToString (), Prefs.unlockedSubWorld.ToString (), currentLevel.ToString ());
        }

    }

    public void LeaderboardClick() {
        Sound.instance.PlayButton ();

        int score = Superpow.Utils.GetLeaderboardScore ();
        GPGSController.instance.ReportScoreAndShowLeaderboard (score);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;

public class SlotMachineDialog : Dialog {
    public Button rocker;
    public SlotNumManager slot;
    public Transform content;
    public Transform ligthGroup;
    private bool speedUp = true;
    private bool speedDown = true;
    //public Animation num;
    protected override void Start() {
        base.Start ();
        slot.onSlotComplete += Close;
        rocker.onClick.AddListener (() => {
            Sound.instance.PlayButton ();
            rocker.enabled = false;
            this.GetComponent<Animator> ().Play ("slotNum");
            StartCoroutine (EndSlot());
            EventTrackingController.instance.LogLevelReward ();
        });
    }
    public void EndSlotAnim() {
        speedDown = false;
    }
    //动画进行到一半的时候
    public void HalfSlotAnim() {
        speedUp = false;
    }
    IEnumerator EndSlot() {
        float delayTime = 0.15f;
        int index = 0;
        while (speedUp) {
            if (index > 0) {
                ligthGroup.GetChild ((index - 1) % ligthGroup.childCount).GetComponent<LoopAnimation> ().SetDark ();
            }
            ligthGroup.GetChild (index % ligthGroup.childCount).GetComponent<LoopAnimation> ().SetLight ();
            float time = delayTime -0.004f;
            if (time <0.03f) {
                delayTime = 0.03f;
            } else {
                delayTime = time;
            }
            index++;
            yield return new WaitForSeconds (delayTime);

        }
        while (speedDown) {
            if (index > 0) {
                ligthGroup.GetChild ((index - 1) % ligthGroup.childCount).GetComponent<LoopAnimation> ().SetDark ();
            }
            ligthGroup.GetChild (index % ligthGroup.childCount).GetComponent<LoopAnimation> ().SetLight ();
            float time = delayTime +0.004f;
            if (time >0.15) {
                delayTime = 0.15f;
            } else {
                delayTime = time;
            }
            index++;
            yield return new WaitForSeconds (delayTime);

        }
        ligthGroup.GetChild ((index-1) % ligthGroup.childCount).GetComponent<LoopAnimation> ().SetDark ();
        yield return new WaitForSeconds (0.5f);
        foreach (Transform item in ligthGroup) {
            item.GetComponent<LoopAnimation> ().Shine ();
        }
        yield return new WaitForSeconds (1.5f);
        slot.CheckReward ();

    }
    public override void Show() {
     
        content.DOScale (1f, 0.5f);
    }
    public override void Close() {
        base.Close ();
        onDialogClosed (this);
        content.DOScale (0f, 0.5f).OnComplete (() => {
            GotoNext ();
            Sound.instance.Play (Sound.Others.Win);

        });
    }
    void GotoNext() {
        if (Prefs.ScratchCardTaskFinish == 0) {
            CardManager.Instance.CheckTask (6);
        }
        if (GameState.unlockNewLevel) {
            //删除数据
            CPlayerPrefs.DeleteKey ("level_progress" + (GameState.currentLevel - 1).ToString ());
            CUtils.LoadScene (Const.SCENE_MAP, true);
        } else {
            //删除数据
            CPlayerPrefs.DeleteKey ("level_progress" + GameState.currentLevel);
            GameState.currentLevel = GameState.currentLevel+1;
            CUtils.LoadScene (Const.SCENE_MAINGAME, true);
        }
    }
  
}

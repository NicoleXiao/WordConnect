using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintManager : MonoBehaviour {
    public Text hint_text;
    public string info;
    private Button hintBtn;
    public System.Action<bool> HintEvent;

    private int mHintClickCount;
    private int mNotFreeHintClickCount;
    private void Awake() {
        hintBtn = this.GetComponent<Button> ();
        hintBtn.onClick.AddListener (()=> {
            ClickHint ();
        });
    }
    public void ClickHint() {
        int hint_count = Const.HINT_COST;
        mHintClickCount++;
        int ballance = CurrencyController.GetBalance ();
        //如果有道具先使用道具
        if (Prefs.HintNum > 0) {
            Prefs.HintNum = Prefs.HintNum - 1;
            if (HintEvent != null) {
                SetHint ();
                HintEvent (true);
            }
        } else {
            if (ballance >= hint_count) {
                mNotFreeHintClickCount++;
                if (HintEvent != null) {
                    SetHint ();
                    HintEvent (false);
                }
            } else {
                if (ConfigController.instance.config.showpurchase) {
                    DialogController.instance.ShowDialog (DialogType.Shop);
                }
            }
        }
        Sound.instance.PlayButton ();
    }

    public void SetHint() {
        if (Prefs.HintNum > 0) {
            hint_text.text = Prefs.HintNum.ToString () + " x hints";
        } else {
            hint_text.text = Const.HINT_COST.ToString () + " gems";
        }
    }
    //上传hint点击事件
    public void HintData() {
        hintBtn.enabled = false;
        if (EventTrackingController.instance != null) {
            if (mHintClickCount > 0) {
                EventTrackingController.instance.LogHintClickEvent (mHintClickCount);
                EventTrackingController.instance.LogGemsSink (mNotFreeHintClickCount * Const.HINT_COST, info);
            }
        }
        mHintClickCount = 0;
        mNotFreeHintClickCount = 0;
    }
}

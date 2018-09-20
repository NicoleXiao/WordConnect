using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RewardItem : MonoBehaviour {
    public Image bg;
    public Image rewardIcon;
    public Text numText;
    private int num;
    public void SetInfo(string path,int num) {
        this.num = num;
        this.gameObject.SetActive (true);
        rewardIcon.sprite= Resources.Load<Sprite> (path);
        rewardIcon.SetNativeSize ();
        numText.text = "X" + num.ToString ();
    }
    //奖励消失
    public void DOFade(float time) {
        if (bg != null) {
            bg.DOFade (0f,time);
        }
        if (rewardIcon != null) {
            rewardIcon.DOFade (0f, time).OnComplete(()=> {
                CurrencyController.CreditBalance (num);
            });
        }
        if (numText != null) {
            numText.DOFade (0f, time);
        }
    }
    public void Show() {
        if (bg != null) {
            bg.color = Color.white;
        }
        if (rewardIcon != null) {
            rewardIcon.color = Color.white;
        }
        if (numText != null) {
            numText.color = Color.white;
        }
    }
}

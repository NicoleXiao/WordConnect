
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SignRewardController : BaseController {
    //一个奖励的宽度
    public Transform group;
    public Transform currentBg;
    public Sprite claimDown;
    public Button claim;
    [HideInInspector]
    public int gemsNum = 0;
    [HideInInspector]
    public int hintNum = 0;
    [HideInInspector]
    public int currentSign = 0;//记录当前应该签到天数
    public GameObject tomorrowTag;
    private const int BG_MOVE = -116;//当前bg移动的距离
    public static SignRewardController _instance;
    protected override void Awake() {
        base.Awake ();
        _instance = this;
        Prefs.SignPop = 1;
        EventTrackingController.instance.LogSignClickEvent ();
    }
    protected override void Start() {
        base.Start ();
        claim.onClick.AddListener (() => {
            claim.enabled = false;
            ClickClaim ();
        });
        SetSignRewardGroup ();

    }
    /// <summary>
    /// 每次显示7天的东西，从1-7 8-14 循环
    /// </summary>

    void SetSignRewardGroup() {
        if (Prefs.Signed == 1) {
            SetClaimUnEnable ();
            currentSign = Prefs.SignDay + 1 >28 ? 1 :Prefs.SignDay+1;
        } else {
            currentSign = Prefs.SignDay;
        }

        int currentIndex = ((currentSign-1) / 7) * 7 + 1;
        Debug.Log ("Sign Current Index  : " +currentIndex + " Prefs.signDay: "+ Prefs.SignDay);
        for (int i = 0; i < group.childCount; i++) {
            SignRewardItem currentItem = group.GetChild (i).GetComponent<SignRewardItem> (); ;
            List<RewardData> data = CSVReadManager.Instance.GetSignData (currentIndex);
            currentItem.SetItemInfo (currentIndex, currentSign,data);
            currentIndex++;
        }
        SetCurrentBgPos ();
    }
    public void SetCurrentBgPos() {
        int index = (currentSign - 1) % 7;
        currentBg.localPosition = new Vector3 (currentBg.localPosition.x, index * BG_MOVE, 1);

    }
    public void ClickClaim() {
        if (gemsNum != 0) {
            LoadGems ();
        } else {
            SetClaimUnEnable ();
        }
        if (hintNum != 0) {
            Prefs.HintNum = Prefs.HintNum + hintNum;
        }
        Prefs.Signed = 1;
        EventTrackingController.instance.LogSignDoneEvent ();
        
    }
   void SetClaimUnEnable() {
        claim.enabled = false;
        claim.GetComponent<Image> ().sprite = claimDown;
        claim.GetComponent<Image> ().SetNativeSize ();
    }

    void LoadGems() {
        Transform parent = GameObject.Find ("SignRewardGroup").transform;
        Vector3 pos= claim.transform.parent.transform.TransformPoint (claim.transform.localPosition);
        GemsEffet gems= EffectManager.Instance.LoadGems (parent,pos,gemsNum);
        gems.OnEffectComplete += () => {
            EventTrackingController.instance.LogGemsAdd (gemsNum,"signreward");
            CurrencyController.CreditBalance (gemsNum);
            SetClaimUnEnable ();
            int index = (currentSign) % 7;
            if (index != 0) {
                currentBg.DOLocalMoveY ((index) * BG_MOVE, 1f);
                group.GetChild ((currentSign - 1) % 7).GetComponent<SignRewardItem> ().SetLine ();
            } else {
                group.GetChild ((currentSign - 1) % 7).GetComponent<SignRewardItem> ().SetDone ();
                DialogController.instance.ShowDialog (DialogType.Card);
            }
            Destroy (gems.gameObject);
        };
    }
    public void SetCheck(int index) {
        group.GetChild (index).GetComponent<SignRewardItem> ().SetCheck ();
        DialogController.instance.ShowDialog (DialogType.Card);
    }

}

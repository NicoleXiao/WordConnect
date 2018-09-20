using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WorldReward : MonoBehaviour {
    public Sprite canClick;//能够点击
    public Sprite closed;//已经打开了
    public Sprite unEnable;//没有激活的状态
    public List<Image> rewardGroup = new List<Image> ();
    private void Start() {
        SetRewardButtonState ();
    }
    // Use this for initialization
    void SetRewardButtonState() {
        for(int i = 0; i < rewardGroup.Count; i++) {
            int type = TextManager.Instance.ReturnWorldRewardState (i);
            if (type==0) {
                rewardGroup[i].sprite = unEnable;
                rewardGroup[i].GetComponent<Button> ().enabled = false;
            }
            if (type == 1) {
                rewardGroup[i].sprite = canClick;
                rewardGroup[i].GetComponent<Button> ().enabled = true;
                rewardGroup[i].transform.parent.GetChild (0).gameObject.SetActive (true);
            }
            if (type == 2) {
                rewardGroup[i].sprite = closed;
                rewardGroup[i].GetComponent<Button> ().enabled = false;
            }
            
        }
    }
    public void ClickReward(int id) {
        rewardGroup[id].sprite = closed;
        rewardGroup[id].GetComponent<Button> ().enabled = false;
        Sound.instance.PlayButton ();
        TextManager.Instance.SetWorldRewardState (id, 2);
        rewardGroup[id].transform.parent.GetChild (0).gameObject.SetActive (false);
        List<RewardData> data = CSVReadManager.Instance.GetWorldRewardData (id+1);
        for (int i = 0; i < data.Count; i++) {
            if (data[i].rewardId==10001) {

                //Toast.instance.ShowMessage ("You've getted " + data[i].rewardNum + " gems");
                
                CurrencyController.CreditBalance (data[i].rewardNum-10);
                StartCoroutine (ClaimEffect (rewardGroup[id].transform));
            }
            if (data[i].rewardId == 10002) {
                //Toast.instance.ShowMessage ("You've getted " + data[i].rewardNum + " hints");
                Prefs.HintNum = Prefs.HintNum + data[i].rewardNum;
            }
        }
    }
    private IEnumerator ClaimEffect(Transform claimTr) {
        Transform rubyBalance = GameObject.FindWithTag ("RubyBalance").transform;
        var middlePoint = CUtils.GetMiddlePoint (claimTr.position, rubyBalance.position, -0.4f);
        Vector3[] waypoints = { claimTr.position, middlePoint, rubyBalance.position };
        for (int i = 0; i < 10; i++) {
            GameObject gameObj = Instantiate (MonoUtils.instance.rubyFly);
            gameObj.transform.position = waypoints[0];
            gameObj.transform.localScale = 0.5f * Vector3.one;
            iTween.MoveTo (gameObj, iTween.Hash ("path", waypoints, "speed", 30, "oncomplete", "OnMoveComplete"));
            iTween.ScaleTo (gameObj, iTween.Hash ("scale", 0.7f * Vector3.one, "time", 0.3f));
            yield return new WaitForSeconds (0.1f);
        }

    }
}

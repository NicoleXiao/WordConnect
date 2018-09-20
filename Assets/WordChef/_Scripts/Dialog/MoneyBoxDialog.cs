using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//存钱罐功能暂时屏蔽
public class MoneyBoxDialog :Dialog{
    //public Image fill;
    //public Text text;
    //public Button claim;
    //public Transform claimTr;
    //private int numWords, claimQuantity;

    //protected override void Start() {
    //    base.Start ();
    //    onDialogCompleteClosed += MainController.instance.HideOpenLight;
    //    fill.fillAmount = (float)Prefs.moneyProgress / (float)10;
    //    text.text = string.Format (text.text,Prefs.moneyProgress);
    //    if (Prefs.moneyProgress == 10) {
    //        claim.enabled = true;
    //    } else {
    //        claim.enabled = false;
    //        claim.GetComponent<Image> ().color = Color.gray;
    //        claim.transform.GetChild (0).GetComponent<Text> ().color = Color.gray;

    //    }
    //    claim.onClick.AddListener (()=> {
    //        Sound.instance.PlayButton ();
    //        Claim ();

    //    });
    //}
    //public void Claim() {
    //    claimQuantity = Random.Range(25,50) ;
    //    text.text = "0/10";

    //    StartCoroutine (ClaimEffect ());
    //    Prefs.moneyProgress = 0;
    //    claim.enabled = false;
    //    claim.GetComponent<Image> ().color = Color.gray;
    //    claim.transform.GetChild (0).GetComponent<Text> ().color = Color.gray;

    //}

    //private IEnumerator ClaimEffect() {
    //    int ab = claimQuantity - 10;
    //    Transform rubyBalance = GameObject.FindWithTag ("RubyBalance").transform;
    //    var middlePoint = CUtils.GetMiddlePoint (claimTr.position, rubyBalance.position, -0.4f);
    //    Vector3[] waypoints = { claimTr.position, middlePoint, rubyBalance.position };
    //    CurrencyController.CreditBalance (ab);
    //    for (int i = 0; i < 10; i++) {
    //        GameObject gameObj = Instantiate (MonoUtils.instance.rubyFly);
    //        gameObj.transform.position = waypoints[0];
    //        gameObj.transform.localScale = 0.5f * Vector3.one;

    //        iTween.MoveTo (gameObj, iTween.Hash ("path", waypoints, "speed", 30, "oncomplete", "OnMoveComplete"));
    //        iTween.ScaleTo (gameObj, iTween.Hash ("scale", 0.7f * Vector3.one, "time", 0.3f));
    //        yield return new WaitForSeconds (0.1f);
    //    }
        
    //}
}

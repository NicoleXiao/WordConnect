using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExtraWordDialog : Dialog
{
    public Transform claimTr;
   // public ExtraProgress extraProgress;
    public GameObject claimButton;
  //  public Text progressText;
    public GameObject closeButton;
    public Transform progressGroup;
    private int numWords;

    protected override void Start()
    {
        base.Start();
        SetProgress ();
        if (Prefs.extraProgress < Prefs.extraTarget) {
            closeButton.SetActive (true);
            claimButton.SetActive (false);
        }
    }
    void SetProgress() {
       // progressText.text = Prefs.extraProgress.ToString () + " / " + Prefs.extraTarget.ToString ();
        for(int i = 0; i < progressGroup.childCount; i++) {
            if (i < Prefs.extraProgress) {
                progressGroup.GetChild (i).gameObject.SetActive (true);
            } else {
                progressGroup.GetChild (i).gameObject.SetActive (false);
            }
        }
    }

    public void Claim()
    {
     
        Prefs.extraProgress = 0;
        SetProgress ();
        claimButton.GetComponent<Button> ().enabled = false;
        GemsEffet gems = EffectManager.Instance.LoadGems (transform, claimTr.transform.position + new Vector3 (0, 0.25f, 0), 2);
        gems.OnEffectComplete += () => {
            CurrencyController.CreditBalance (2);
            EventTrackingController.instance.LogGemsAdd (2, "extraword");
            Close ();
            Destroy (gems.gameObject);
        };
        ExtraWord.instance.OnClaimed();

    }
  

    //private IEnumerator ClaimEffect()
    //{

    //    Transform rubyBalance = GameObject.FindWithTag("RubyBalance").transform;
    //    var middlePoint = CUtils.GetMiddlePoint(claimTr.position, rubyBalance.position, -0.4f);
    //    Vector3[] waypoints = { claimTr.position, middlePoint, rubyBalance.position };

    //    for (int i = 0; i < claimQuantity; i++)
    //    {
    //        GameObject gameObj = Instantiate(MonoUtils.instance.rubyFly);
    //        gameObj.transform.position = waypoints[0];
    //        gameObj.transform.localScale = 0.5f * Vector3.one;

    //        iTween.MoveTo(gameObj, iTween.Hash("path", waypoints, "speed", 30, "oncomplete", "OnMoveComplete"));
    //        iTween.ScaleTo(gameObj, iTween.Hash("scale", 0.7f * Vector3.one, "time", 0.3f));
    //        yield return new WaitForSeconds(0.1f);
    //    }
    //    claimButton.SetActive (false);
    //    Close ();
    //}


}

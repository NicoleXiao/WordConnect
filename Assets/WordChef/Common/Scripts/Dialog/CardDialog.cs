using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDialog :Dialog {
    public PaintCheck paint;
    public Transform paintTrans;
    public Text text;
    public Transform rubyTrans;
    public GameObject close;
    public GameObject go;
    protected override void Start() {
        base.Start ();
        paint.canDraw = true;
        CardManager.Instance.SetCardInfo (text);
        paint.OnPaintComplete += PaintComplete;
    }
    void PaintComplete() {
        Prefs.IsTask = CardManager.Instance.isTask ? 1 : 0;
        if (Prefs.IsTask == 1) {
            go.SetActive (true);
            close.SetActive (false);
        }
        paint.gameObject.SetActive (false);
        if (CardManager.Instance.card_gems != 0) {
            int num = CardManager.Instance.card_gems;
            GemsEffet effet = EffectManager.Instance.LoadGems (transform, paintTrans.position, num);
            effet.OnEffectComplete += () => {
                EventTrackingController.instance.LogGemsAdd (num, "scratchcardreward");
                CurrencyController.CreditBalance (num);
                Close ();
                Destroy (effet.gameObject);
            };
        }
        if (CardManager.Instance.card_hint != 0) {
            Prefs.HintNum = Prefs.HintNum + CardManager.Instance.card_hint;
        }
    }
  
}

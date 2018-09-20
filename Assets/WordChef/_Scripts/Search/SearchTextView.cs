using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SearchTextView : MonoBehaviour {
    public Image bg;
    public Text view;

    public void ShowText(string text) {
        if (string.IsNullOrEmpty (view.text)) {
            bg.DOKill ();
            view.DOKill ();
            bg.DOFade (1f, 0.5f);
            view.DOFade (1f, 0.5f);
        }
        view.text = text;
        RectTransform bgRT = bg.GetComponent<RectTransform> ();
        bgRT.sizeDelta = new Vector2 (view.preferredWidth + 60, bgRT.sizeDelta.y);
        
       

    }
    public void HideText() {
        bg.DOKill ();
        view.DOKill ();
        bg.DOFade (0f, 0.5f);
        view.DOFade (0f,0.5f);
        view.text = "";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour {
    public Transform gems;
    public bool isShowGems;
    public Text letterText;
    public string letter;
    public bool isShown;
    private bool isShowing;//正在显示
    private Vector3 originLetterScale;

    public void Animate()
    {
        if (isShowing) {
            return;
        }
        Vector3 beginPosition = TextPreview.instance.transform.position;
        originLetterScale = letterText.transform.localScale;
        Vector3 middlePoint = CUtils.GetMiddlePoint(beginPosition, transform.position, -0.3f);
        Vector3[] waypoint = { beginPosition, middlePoint, transform.position };

        ShowText();
        letterText.transform.position = beginPosition;
        letterText.transform.localScale = TextPreview.instance.text.transform.localScale;
        letterText.transform.SetParent(MonoUtils.instance.textFlyTransform);
        iTween.MoveTo(letterText.gameObject, iTween.Hash("path", waypoint, "time", 0.2f, "oncomplete", "OnMoveToComplete", "oncompletetarget", gameObject));
        iTween.ScaleTo(letterText.gameObject, iTween.Hash("scale", originLetterScale, "time", 0.2f));
    }

    private void OnMoveToComplete()
    {
        if (isShowGems) {
            isShowGems = false;
            FlyGems ();
        }
        letterText.transform.SetParent(transform);
        iTween.ScaleTo(letterText.gameObject, iTween.Hash("scale", originLetterScale * 1.3f, "time", 0.15f, "oncomplete", "OnScaleUpComplete", "oncompletetarget", gameObject));
    }

    private void OnScaleUpComplete()
    {
        iTween.ScaleTo(letterText.gameObject, iTween.Hash("scale", originLetterScale, "time", 0.15f));
    }

    public void ShowHint()
    {
        originLetterScale = letterText.transform.localScale;
        ShowText();
        OnMoveToComplete();
    }

    public void ShowText()
    {
        isShown = true;
        isShowing = true;
        letterText.text = letter;
    }
    public void ScaleTextAndGems(float scale) {
        if (gems != null) {
            gems.localScale = new Vector3 (scale, scale, 1);
        }
        letterText.transform.localScale = new Vector3 (scale, scale, 1);
    }
    public void ShowGems() {
        if (!isShown) {
            isShowGems = true;
            if (gems != null) {
                gems.gameObject.SetActive (true);
            }
        }
    }
    void FlyGems() {
        Transform rubyTrans = GameObject.FindWithTag ("RubyBalance").transform;
        var middlePoint = CUtils.GetMiddlePoint (gems.position, rubyTrans.position, -0.4f);
        Vector3[] waypoints = { gems.position, middlePoint, rubyTrans.position };
        iTween.MoveTo (gems.gameObject, iTween.Hash ("path", waypoints, "speed", 30, "oncomplete", "OneMove"));
        iTween.ScaleTo (gems.gameObject, iTween.Hash ("scale", 0.7f * Vector3.one, "time", 0.3f));
    }
}

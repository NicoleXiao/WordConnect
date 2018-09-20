using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SelectWorldSliderBarManager : MonoBehaviour {
    public Button showBtn;
    public Button hideBtn;
    public Transform itemGroup;
    public Image scrollView;
    private float hideX = 254;
    private float showX = -253f;
    // private bool isShow = false;
    public Transform rubyTrans;
    public static SelectWorldSliderBarManager _instance;
    // Use this for initialization
    private void Awake() {
        _instance = this;
    }
    void Start() {

       showBtn.onClick.AddListener (() => {
           scrollView.raycastTarget = true;
           Sound.instance.PlayButton ();
            showBtn.gameObject.SetActive (false);
            hideBtn.gameObject.SetActive (true);
            this.GetComponent<RectTransform> ().DOKill ();
            this.GetComponent<RectTransform> ().DOAnchorPosX (showX, 1f);

        });
        hideBtn.onClick.AddListener (() => {
            scrollView.raycastTarget = false;
            this.GetComponent<RectTransform> ().DOKill ();
            this.GetComponent<RectTransform> ().DOAnchorPosX (hideX, 1f).OnComplete (() => {
                showBtn.gameObject.SetActive (true);
                hideBtn.gameObject.SetActive (false);
            });
        });
    }


}

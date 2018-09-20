using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class LoopAnimation : MonoBehaviour {
    private GameObject dark;
    private GameObject lightObj;
    private void Awake() {
        dark = transform.GetChild (0).gameObject;
        lightObj = transform.GetChild (1).gameObject;
    }
    private void Start() {
        lightObj.SetActive (false);
    }
    public void SetLight() {
        dark.SetActive (false);
        lightObj.SetActive (true);
    }
    public void SetDark() {
        dark.SetActive (true);
        lightObj.SetActive (false);
    }
    //闪两下
    public void Shine() {
        dark.SetActive (true);
        lightObj.GetComponent<Image> ().color = new Color (1,1,1,0);
        lightObj.SetActive (true);
        lightObj.GetComponent<Image> ().DOFade (1f,0.3f).SetLoops(4,LoopType.Yoyo).OnComplete(()=> {
            lightObj.GetComponent<Image> ().DOFade (0f, 0.3f);
            
        });
    }

    //public List<GameObject> anim = new List<GameObject>();
    //private int index;
    //private float time = 0f;
    //private void Start() {
    //    index = 0;
    //    anim[index].SetActive (true);
    //}
    //private void Update() {
    //    time += 0.05f;
    // //   Debug.Log ("Time.DeltaTime-------"+Time.deltaTime);
    //    if (time >1f) {
    //        time = 0f;
    //        index++;

    //        if (index >= anim.Count) {
    //            index = 0;
    //        }
    //        Debug.Log ("anim index --------------" + index);
    //        for (int i = 0; i < anim.Count; i++) {
    //            if (i == index) {
    //                anim[index].SetActive (true);
    //            } else {
    //                anim[i].SetActive (false);
    //            }
    //        }
    //    }
    //}
}

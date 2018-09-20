 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Superpow;


public class WorldController : BaseController {

    public Button achieveBtn;

    protected override void Start()
    {
        base.Start();
        achieveBtn.onClick.AddListener (()=> {
            //achieveBtn.enabled = false;
            //CSVReadManager.Instance.ReadCompelte += LoadAchiveve;
            Sound.instance.PlayButton ();
            //CSVReadManager.Instance.LoadAchieveData ();
            CUtils.LoadScene (Const.SCENE_ACHIEVE, true);
        });
    }
    //private void LoadAchiveve() {
    //    CUtils.LoadScene (Const.SCENE_ACHIEVE, true);
    //}
    //private void OnDisable() {
    //    if (CSVReadManager.Instance.ReadCompelte!=null) {
    //        CSVReadManager.Instance.ReadCompelte -= LoadAchiveve;
    //       }
    //}




}

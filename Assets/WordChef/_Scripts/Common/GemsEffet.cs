using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.IO;

public class GemsEffet : MonoBehaviour {
    public System.Action OnEffectComplete;//特效结束
    public System.Action OnEffectShow;//特效开始
    public Transform flyGroup;
    private float delyTime;
    private Transform showGroup;
    private Transform rubyTrans;
    private int showNum = 0;

    //以后要删除的，现在只是为了测试数据
    //private string loadPath = "";
    //private int sceneIndex = 4;

    public void Show(int num, bool showPath = false) {
        showGroup = flyGroup;

        if (num >= 1 && num < 100) {
            delyTime = 0.2f;
            showNum = 3;
        } else if (num >= 100 && num < 1000) {
            delyTime = 0.15f;
            showNum = 6;
        } else if (num >= 1000) {
            delyTime = 0.1f;
            showNum = 9;
        }
        if (showPath) {
            StartCoroutine (ShowDialyFlyPath ());
        } else {
            StartCoroutine (ShowFlyEffet ());
        }


    }
    IEnumerator ShowFlyEffet() {

        rubyTrans = GameObject.FindWithTag ("RubyBalance").transform;

        for (int i = 0; i < showNum; i++) {
            showGroup.GetChild (i).gameObject.SetActive (true);
            var middlePoint = CUtils.GetMiddlePoint (showGroup.GetChild (i).position, rubyTrans.position, -0.4f);
            Vector3[] waypoints = { showGroup.GetChild (i).position, middlePoint, rubyTrans.position };
            iTween.MoveTo (showGroup.GetChild (i).gameObject, iTween.Hash ("path", waypoints, "speed", 30, "oncomplete", "OnMoveComplete"));
            iTween.ScaleTo (showGroup.GetChild (i).gameObject, iTween.Hash ("scale", 0.7f * Vector3.one, "time", 0.3f));
            yield return new WaitForSeconds (delyTime);
        }
        if (OnEffectComplete != null) {
            OnEffectComplete ();
        }
    }
    //显示每日挑战的路径
    IEnumerator ShowDialyFlyPath() {
        rubyTrans = GameObject.FindWithTag ("RubyBalance").transform;
        float delay = 0f;
        float time = 1f;
        for (int i = 0; i < showNum; i++) {
            if (i == showNum - 1) {
                if (OnEffectShow != null) {
                    OnEffectShow ();
                }
            }
            showGroup.GetChild (i).gameObject.SetActive (true);
            var middlePoint = CUtils.GetMiddlePoint (showGroup.GetChild (i).position, rubyTrans.position, -0.4f);
            Vector3[] waypoints = { showGroup.GetChild (i).position, middlePoint, rubyTrans.position };
            iTween.MoveTo (showGroup.GetChild (i).gameObject, iTween.Hash ("path", waypoints, "time", time, "oncomplete", "OnMoveComplete"));
            iTween.ScaleTo (showGroup.GetChild (i).gameObject, iTween.Hash ("scale", 0.7f * Vector3.one, "time", 0.3f));
            yield return new WaitForSeconds (delay);
            delay += 0.1f;
            time -= 0.1f;
        }
        yield return new WaitForSeconds (time);
        if (OnEffectComplete != null) {
            OnEffectComplete ();
            Timer.Schedule (this, 1, () => {
                CUtils.LoadScene (4, true);
                Destroy (this.gameObject);
            });
            //下面这个函数是测试版本，策划要求测试版本一关一关的过。
            // LoadNextScene ();
        }
    }
    #region 测试版本
    //void LoadNextScene() {
    //    CSVReadManager.Instance.ReadCompelte += ReadComplete;
    //    CSVReadManager.Instance.ReadError += ReadError;
    //    switch (GameState.dialyType) {
    //        case DailyType.Crossy:
    //            SetPath ("CrossyData/crossy_" + GameState.crossyLevel.ToString ());
    //            sceneIndex = 5;
    //            CSVReadManager.Instance.LoadDailyData (loadPath, DataType.Crossy);
    //            PlayerDataManager.Instance.playerData.isCrossyFinish = true;

    //            break;
    //        case DailyType.Search:
    //            SetPath ("SearchData/search_" + GameState.searchLevel.ToString ());
    //            sceneIndex = 6;
    //            CSVReadManager.Instance.LoadDailyData (loadPath, DataType.Search);
    //            PlayerDataManager.Instance.playerData.isSearchFinish = true;
    //            break;
    //        case DailyType.Puzzle:
    //            SetPath ("PuzzleData/puzzle_" + GameState.puzzleLevel.ToString ());
    //            sceneIndex = 8;
    //            CSVReadManager.Instance.LoadDailyData (loadPath, DataType.Puzzle);
    //            PlayerDataManager.Instance.playerData.isPuzzleFinish = true;
    //            break;
    //        case DailyType.Picutre:
    //            if (GameState.pictureLevel <= CSVReadManager.Instance.pictureData.Count) {
    //                CUtils.LoadScene (Const.SCENE_PICTURE, true);
    //            } else {
    //                CUtils.LoadScene (Const.SCENE_DAILY, true);
    //                PlayerDataManager.Instance.playerData.isPictureFinish = true;
    //            }

    //            break;
    //    }
    //}
    //private void SetPath(string path) {
    //    loadPath = Application.streamingAssetsPath + "/DailyData/" + path + ".json";

    //}
    //private void OnDisable() {
    //    CSVReadManager.Instance.ReadError -= ReadError;
    //    CSVReadManager.Instance.ReadCompelte -= ReadComplete;
    //}
    //private void ReadComplete() {
    //    CUtils.LoadScene (sceneIndex, true);
    //    Destroy (this.gameObject);
    //}
    //private void ReadError() {
    //    switch (GameState.dialyType) {
    //        case DailyType.Crossy:
    //            PlayerDataManager.Instance.playerData.crossyLevelID = 1;
    //            GameState.crossyLevel = 1;
    //            break;
    //        case DailyType.Search:
    //            PlayerDataManager.Instance.playerData.searchLevelID = 1;
    //            GameState.searchLevel = 1;
    //            break;
    //        case DailyType.Puzzle:
    //            PlayerDataManager.Instance.playerData.puzzleLevelID = 1;
    //            GameState.puzzleLevel = 1;
    //            break;
    //        case DailyType.Picutre:
    //            if (GameState.pictureLevel <= CSVReadManager.Instance.pictureData.Count) {
    //                PlayerDataManager.Instance.playerData.pictureLevelID= 1;
    //                GameState.pictureLevel = 1;
    //                CUtils.LoadScene (Const.SCENE_PICTURE, true);
    //            } else {
    //                CUtils.LoadScene (Const.SCENE_DAILY, true);
    //                PlayerDataManager.Instance.playerData.isPictureFinish = true;
    //            }

    //            break;
    //    }
    //    PlayerDataManager.Instance.SavePlayerProgress ();
    //    CUtils.LoadScene (Const.SCENE_DAILY, true);
    //    Destroy (this.gameObject);
    //}
    #endregion
}

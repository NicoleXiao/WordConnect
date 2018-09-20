
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public enum DailyType {
    None,
    Crossy,
    Search,
    Picutre,
    Puzzle
}
public class DailyChangeController : BaseController {
    private string LoadPath = "";
    private int sceneIndex = 0;
    public GameObject crossyDone;
    public GameObject searchDone;
    public GameObject pictureDone;
    public GameObject puzzleDone;
    public Text month;
    public Text day;
    public Image bg;
    protected override void Awake() {
        base.Awake ();
        CSVReadManager.Instance.ReadCompelte = ReadComplete;
        DateTime now = DateTime.Now;
        day.text = now.Day.ToString ();
        month.text= now.ToString ("MMMM", new System.Globalization.CultureInfo ("en-us")).Substring (0, 3).ToUpper();
        if (PlayerDataManager.Instance.playerData.isCrossyFinish) {
            crossyDone.SetActive (true);
            crossyDone.transform.parent.GetComponent<Button> ().enabled = false;
        }
        if (PlayerDataManager.Instance.playerData.isSearchFinish) {
            searchDone.SetActive (true);
            searchDone.transform.parent.GetComponent<Button> ().enabled = false;
        }
        if (PlayerDataManager.Instance.playerData.isPuzzleFinish) {
            puzzleDone.SetActive (true);
            puzzleDone.transform.parent.GetComponent<Button> ().enabled = false;
        }
        if (PlayerDataManager.Instance.playerData.isPictureFinish) {
            pictureDone.SetActive (true);
            pictureDone.transform.parent.GetComponent<Button> ().enabled = false;
        }
        if (PlayerDataManager.Instance.playerData.isCrossyFinish && PlayerDataManager.Instance.playerData.isSearchFinish
            && PlayerDataManager.Instance.playerData.isPuzzleFinish && PlayerDataManager.Instance.playerData.isPictureFinish &&
           　!PlayerDataManager.Instance.playerData.isDailyFinish) {
            PlayerDataManager.Instance.playerData.isDailyFinish = true;
            PlayerDataManager.Instance.playerData.dailyChangeBgIndex++;
            PlayerDataManager.Instance.playerData.accumulativeChallenge++;
            PlayerDataManager.Instance.JudeReachAchieve (6, PlayerDataManager.Instance.playerData.accumulativeChallenge);


        }
    }
    protected override void Start() {
        base.Start ();
        string bgUrl = "DailyChallengeBg/dailybg_" + PlayerDataManager.Instance.playerData.dailyChangeBgIndex + ".png";
        ResManager.Instance.LoadSpriteFromStreamingAsset (bgUrl, delegate (Sprite sp) {
            bg.sprite = sp;
        }, new Vector2 (0.5f, 0.5f), false);
    }
    private void ReadComplete() {
        if (sceneIndex == 7 && GameState.pictureLevel > CSVReadManager.Instance.pictureData.Count) {
            return;
        }
        CUtils.LoadScene (sceneIndex, true);
    }
    public void ClickCrossy() {
        Sound.instance.PlayButton ();
        sceneIndex = 5;
        GameState.dialyType = DailyType.Crossy;
        CombineJsonPath ("CrossyData/crossy_" + GameState.crossyLevel.ToString ());
        if (CSVReadManager.Instance.crossyData.wordInfo.Count == 0) {
            CSVReadManager.Instance.LoadDailyData (LoadPath, DataType.Crossy);
        } else {
            CUtils.LoadScene (sceneIndex, true);
        }
    }
    public void ClickSearch() {
        Sound.instance.PlayButton ();
        sceneIndex = 6;
        GameState.dialyType = DailyType.Search;
        CombineJsonPath("SearchData/search_" + GameState.searchLevel.ToString ());
        if (CSVReadManager.Instance.searchData.wordPositon.Count == 0) {
            CSVReadManager.Instance.LoadDailyData (LoadPath, DataType.Search);
        } else {
            CUtils.LoadScene (sceneIndex, true);
        }
    }
    public void ClickPicture() {
        Sound.instance.PlayButton ();
        sceneIndex = 7;
        GameState.dialyType = DailyType.Picutre;
        if (CSVReadManager.Instance.pictureData.Count == 0) {
            CSVReadManager.Instance.LoadPictureData ();
        } else {
            if (GameState.pictureLevel <= CSVReadManager.Instance.pictureData.Count) {
                CUtils.LoadScene (sceneIndex, true);
            }
        }
    }
    public void ClickPuzzle() {
        Sound.instance.PlayButton ();
        sceneIndex = 8;
        GameState.dialyType = DailyType.Puzzle;
        CombineJsonPath ("PuzzleData/puzzle_" + GameState.puzzleLevel.ToString());
        if (CSVReadManager.Instance.puzzleData.puzzleInfo.Count == 0) {
            CSVReadManager.Instance.LoadDailyData (LoadPath, DataType.Puzzle);
        } else {
            CUtils.LoadScene (sceneIndex, true);
        }
    }

    private void CombineJsonPath(string path) {
        LoadPath = Application.streamingAssetsPath + "/DailyData/"+ path + ".json";

    }
    private void OnDisable() {
        CSVReadManager.Instance.ReadCompelte -= ReadComplete;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using Superpow;

public class CrossyWordManager : MonoBehaviour {
    public RectTransform cell;
    public RectTransform areaGroup;
    public RectTransform wordGroup;
    public TextPreview textPreview;
   // public Compliment compliment;
    public Text hint_text;
    public HintManager hint;
    private float currentWidth;
    private float currentHeight;
    private float areaWidth;
    private float areaHeight;
    private float min = 0;
    private float scale = 1f;
    private int comboCount = 0;
    private string[] levelProgress;//保存进度
    private int progressIndex = 0;
    private List<string> answer = new List<string> ();
    private List<List<Cell>> cellGroup = new List<List<Cell>> ();
    private Dictionary<string, Cell> cellDic = new Dictionary<string, Cell> ();//为了找到相同的cell
    public static CrossyWordManager instance;

    private void Awake() {
        instance = this;
    }

    private void Start() {

        hint.info = "crossyhintcost";
        hint.HintEvent += ShowHint;
        hint.SetHint ();
    }

    public void SetItem(CrossyData data) {
        levelProgress = GetLevelProgress ();
        Calculate (data.row, data.col);
        for (int i = 0; i < data.wordInfo.Count; i++) {
            WordInfo info = data.wordInfo[i];
            answer.Add (info.validWord);
            CloneCell (info.pos.x, info.pos.y, info.validWord, info.direction == WordDirection.Right);
        }
        float moveX = areaWidth - data.col * min;
        float moveY = areaHeight - data.row * min;
        wordGroup.anchoredPosition = new Vector2 (wordGroup.anchoredPosition.x + moveX / 2, wordGroup.anchoredPosition.y - moveY / 2);
        if (data.gemsPos.Count > 0) {
            for (int j = 0; j < data.gemsPos.Count; j++) {
                Position pos = data.gemsPos[j];
                string name = cell.name + pos.x.ToString () + pos.y.ToString ();
                if (cellDic.ContainsKey (name)) {
                    cellDic[name].ShowGems ();
                }
            }
        }
        CheckGameComplete ();
    }

    //克隆物体，只有两个方向，向右和向下
    private void CloneCell(int row, int col, string word, bool right) {
        List<Cell> cells = new List<Cell> ();
        int index = 0;
        int begin = 0;
        if (right) {
            begin = col;
        } else {
            begin = row;
        }
        for (int i = begin; i < word.Length+begin; i++) {
            string name = "";
            Vector2 pos = Vector2.zero;
            if (right) {
                name = cell.name + row.ToString () + i.ToString ();
                pos = new Vector2 (cell.anchoredPosition.x + i * min, cell.anchoredPosition.y - row * min);
            } else {
                name = cell.name + i.ToString () + col.ToString ();
                pos = new Vector2 (cell.anchoredPosition.x + col * min, cell.anchoredPosition.y - i * min);
            }
            if (!cellDic.ContainsKey (name)) {
                RectTransform item = Instantiate (cell, cell.transform.parent);
                cellDic.Add (name, item.GetComponent<Cell> ());
                item.anchoredPosition = pos;
                item.GetComponent<Cell> ().ScaleTextAndGems (scale);
                item.GetComponent<Cell> ().letter = word.Substring (index, 1);
                item.gameObject.SetActive (true);
                if (levelProgress.Length > 0) {
                    if (levelProgress[0].Substring (progressIndex, 1) == "1") {
                        item.GetComponent<Cell> ().ShowText ();
                    }
                    progressIndex++;
                }
                cells.Add (item.GetComponent<Cell> ());
            } else {
                cells.Add (cellDic[name]);
            }
            index++;
        }
        cellGroup.Add (cells);
    }

    private void Calculate(int row, int col) {
        areaHeight = areaGroup.sizeDelta.y;
        areaWidth = areaGroup.sizeDelta.x;
        currentWidth = areaGroup.sizeDelta.x / (float)col;
        currentHeight = areaGroup.sizeDelta.y / (float)row;
        min = currentWidth < currentHeight ? currentWidth : currentHeight;
        scale = min / cell.sizeDelta.x;
        float x = min / 2 - areaWidth / 2;
        float y = areaHeight / 2 - min / 2;
        cell.anchoredPosition = new Vector2 (x, y);
        cell.sizeDelta = new Vector2 (min, min);

    }

    public void CheckAnswer(string word) {
        if (word.Length == 1) {
            return;
        }
        bool isRight = false;
        for (int i = 0; i < answer.Count; i++) {
            if (answer[i].ToUpper () == word.ToUpper ()) {
                isRight = true;
                List<Cell> group = cellGroup[i];
                answer.Remove (answer[i]);
                cellGroup.Remove (group);
                if (word.ToLower () == "word") {
                    PlayerDataManager.Instance.playerData.wordCount++;
                    PlayerDataManager.Instance.JudeReachAchieve (15, PlayerDataManager.Instance.playerData.wordCount);
                }
                if (word.ToLower () == "connect") {
                    PlayerDataManager.Instance.playerData.connectCount++;
                    PlayerDataManager.Instance.JudeReachAchieve (16, PlayerDataManager.Instance.playerData.connectCount);
                }
                PlayerDataManager.Instance.playerData.accumulativeLinkWord++;
                PlayerDataManager.Instance.JudeReachAchieve (5, PlayerDataManager.Instance.playerData.accumulativeLinkWord);
                Sound.instance.Play (Sound.Others.Match);
                StartCoroutine (IEShowAnswer (group));
                break;
            }
        }
        if (isRight) {
            comboCount++;
            Compliment.Instance.ShowRandom (comboCount);
        } else {
            comboCount = 0;
        }
        textPreview.FadeOut ();
    }

    private void CheckGameComplete() {
        SaveProcess ();
        bool isFinish = true;
        Transform group = wordGroup.transform;
        for (int i = 1; i < group.childCount; i++) {
            Cell cell = group.GetChild (i).GetComponent<Cell> ();
            if (cell.isShown == false) {
                isFinish = false;
                break;
            }
        }
        if (isFinish) {
            hint.HintData ();
            //处理数据
            CSVReadManager.Instance.crossyData = new CrossyData ();
            GameState.crossyLevel++;
            PlayerDataManager.Instance.playerData.crossyLevelID = GameState.crossyLevel;
            PlayerDataManager.Instance.playerData.isCrossyFinish = true;
            PlayerDataManager.Instance.playerData.crossyCount++;
            PlayerDataManager.Instance.JudeReachAchieve (12, PlayerDataManager.Instance.playerData.crossyCount);
            PlayerPrefs.DeleteKey ("crossy_progress");
            Timer.Schedule (this, 1f, () => {
                DialogController.instance.ShowDialog (DialogType.DailyWin);
            });
            hint.HintData ();
        }
    }

    public IEnumerator IEShowAnswer(List<Cell> cells) {
        foreach (var cell in cells) {
            // cell.isShown = true;
            cell.Animate ();
            yield return new WaitForSeconds (0.1f);
        }
        CheckGameComplete ();

    }

    private void ShowHint(bool isFree) {
        if (answer.Count > 0) {
            List<Cell> cells = cellGroup[0];
            var cell = cells.Find (x => !x.isShown);
            if (cell != null) {
                cell.ShowHint ();
                if (!isFree) {
                    CurrencyController.DebitBalance (Const.HINT_COST);
                }
            }
            cell = cells.Find (x => !x.isShown);
            if (cell == null) {
                answer.Remove (answer[0]);
                cellGroup.Remove (cells);

            }
            CheckGameComplete ();
        }
    }

    //保存进度
    private void SaveProcess() {
        List<string> results = new List<string> ();
        StringBuilder sb = new StringBuilder ();
        Transform group = wordGroup.transform;
        for (int i = 1; i < group.childCount; i++) {
            Cell cell = group.GetChild (i).GetComponent<Cell> ();
            sb.Append (cell.isShown ? "1" : "0");
        }
        results.Add (sb.ToString ());
        Prefs.crossyProgress = results.ToArray ();
    }

    public string[] GetLevelProgress() {
       return Prefs.crossyProgress;
    }

}

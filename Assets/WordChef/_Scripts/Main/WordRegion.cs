using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using UnityEngine.UI;

public class WordRegion : MonoBehaviour {
    public HintManager hint;
    public TextPreview textPreview;
    //public Compliment compliment;

    private List<LineWord> lines = new List<LineWord> ();
    private List<string> validWords = new List<string> ();

    private GameLevel gameLevel;
    private int numWords, numCol, numRow;
    private float cellSize, startFirstColX;
    private bool hasLongLine;

    private RectTransform rt;
    public static WordRegion instance;
    private int mExtraWordClickCount;


    private void Awake() {
        instance = this;
        rt = GetComponent<RectTransform> ();
    }
    private void Start() {
        hint.info = "hintcost";
        hint.HintEvent = HintClick;
        hint.SetHint ();
    }

    public void Load(GameLevel gameLevel) {
        this.gameLevel = gameLevel;

        var wordList = CUtils.BuildListFromString<string> (this.gameLevel.answers);
        validWords = CUtils.BuildListFromString<string> (this.gameLevel.validWords);
        numWords = wordList.Count;

        numCol = numWords <= 4 ? 1 :
                     numWords <= 10 ? 2 : 3;

        numRow = (int)Mathf.Ceil (numWords / (float)numCol);

        int maxCellInWidth = 0;

        for (int i = numRow; i <= numWords; i += numRow) {
            maxCellInWidth += wordList[i - 1].Length;
        }

        if (numWords % numCol != 0) maxCellInWidth += wordList[numWords - 1].Length;

        if (numCol > 1) {
            float coef = (maxCellInWidth + (maxCellInWidth - numCol) * Const.CELL_GAP_COEF + (numCol - 1) * Const.COL_GAP_COEF);
            cellSize = rt.rect.width / coef;
            float maxSize = rt.rect.height / (numRow + (numRow + 1) * Const.CELL_GAP_COEF);
            if (maxSize < cellSize) {
                cellSize = maxSize;
                startFirstColX = (rt.rect.width - cellSize * coef) / 2f;
            }
        } else {
            cellSize = rt.rect.height / (numRow + (numRow - 1) * Const.CELL_GAP_COEF + 0.8f);
            float maxSize = rt.rect.width / (maxCellInWidth + (maxCellInWidth - 1) * Const.CELL_GAP_COEF);

            if (maxSize < cellSize) {
                hasLongLine = true;
                cellSize = maxSize;
            }
        }

        string[] levelProgress = GetLevelProgress ();

        int lineIndex = 0;
        //int levelProgressIndex = 0;
        foreach (var word in wordList) {
            LineWord line = Instantiate (MonoUtils.instance.lineWord);
            line.answer = word.ToUpper ();
            line.cellSize = cellSize;
            line.SetLineWidth ();
            line.Build ();

            if (levelProgress.Length > 0) {
                line.SetProgress (levelProgress[lineIndex]);
                //levelProgressIndex++;
            }

            line.transform.SetParent (transform);
            line.transform.localScale = Vector3.one;
            line.transform.localPosition = Vector3.zero;

            lines.Add (line);
            lineIndex++;
        }

        CheckGameComplete ();
        SetLinesPosition ();
    }

    private void SetLinesPosition() {
        if (numCol >= 2) {
            float[] startX = new float[numCol];
            startX[0] = startFirstColX;

            for (int i = 1; i < numCol; i++) {
                startX[i] = startX[i - 1] + lines[numRow * i - 1].lineWidth + cellSize * Const.COL_GAP_COEF;
            }

            for (int i = 0; i < lines.Count; i++) {
                int lineX = i / numRow;
                int lineY = numRow - 1 - i % numRow;

                float x = startX[lineX];
                float gapY = (rt.rect.height - cellSize * numRow) / (numRow + 1);
                float y = (lineY + 1) * gapY + lineY * cellSize;

                lines[i].transform.localPosition = new Vector2 (x, y);
            }
        } else {
            for (int i = 0; i < lines.Count; i++) {
                float x = rt.rect.width / 2 - lines[i].lineWidth / 2;
                float y;
                if (hasLongLine) {
                    float gapY = (rt.rect.height - numRow * cellSize) / (numRow + 1);
                    y = gapY + (cellSize + gapY) * (numRow - i - 1);
                } else {
                    y = 0.4f * cellSize + (cellSize + cellSize * Const.CELL_GAP_COEF) * (numRow - i - 1);
                }
                lines[i].transform.localPosition = new Vector2 (x, y);
            }
        }
    }

    public void CheckAnswer(string checkWord) {
        LineWord line = lines.Find (x => x.answer == checkWord);
        if (line != null) {
            if (!line.isShown) {
                SetCombo (1);
                textPreview.SetAnswerColor ();
                line.ShowAnswer ();
                CheckGameComplete ();
                Compliment.Instance.ShowRandom (MainController.instance.comboCount);
                //if (lines.Last () == line) {
                //    compliment.ShowRandom ();
                //}
                if (checkWord.ToLower () == "word") {
                    PlayerDataManager.Instance.playerData.wordCount++;
                    PlayerDataManager.Instance.JudeReachAchieve (15, PlayerDataManager.Instance.playerData.wordCount);
                }
                if (checkWord.ToLower () == "connect") {
                    PlayerDataManager.Instance.playerData.connectCount++;
                    PlayerDataManager.Instance.JudeReachAchieve (16, PlayerDataManager.Instance.playerData.connectCount);
                }
                PlayerDataManager.Instance.playerData.accumulativeLinkWord++;
                PlayerDataManager.Instance.JudeReachAchieve (5, PlayerDataManager.Instance.playerData.accumulativeLinkWord);
                Sound.instance.Play (Sound.Others.Match);
            } else {
                textPreview.SetExistColor ();
            }
        } else if (validWords.Contains (checkWord.ToLower ())) {
            ExtraWord.instance.ProcessWorld (checkWord);
        } else {
            if (textPreview.text.text.Length != 1) {
                SetCombo (0);
            }
            textPreview.SetWrongColor ();
        }

        textPreview.FadeOut ();
    }
    /// <summary>
    /// 只有新关卡才有奖励
    /// 当combo数值到达1时候，不给予玩家任何鼓励提示；给予1朵小红花；
    /// 当combo数值到达2时候，给予玩家GoodJob提示；给予3朵小红花；
    ///当combo数值到达3时候，给予玩家welldone提示；给予5朵小红花
    ///当combo数值到达4时候，给予玩家excellent提示；给予8朵小红花
    ///当combo数值到达5时候，给予玩家amazing提示；给予12朵小红花
    ///当combo数值大于等于6时候，给予玩家fantastic提示；给予15朵小红花
    /// </summary>
    /// <param name="value"></param>
    void SetCombo(int value) {
        if (GameState.currentLevel < Prefs.unlockedLevel) {
            return;
        }
        int addCount = 0;
        if (value == 0) {
            MainController.instance.comboCount = 0;
        } else {
            MainController.instance.comboCount++;
            if (MainController.instance.comboCount < 4) {
                addCount = (1 + 2 * (MainController.instance.comboCount - 1));
            } else if (MainController.instance.comboCount == 4) {
                addCount = 8;
            } else if (MainController.instance.comboCount == 5) {
                addCount = 12;
            } else if (MainController.instance.comboCount >= 6) {
                addCount = 15;
            }
            MainController.instance.SetStarProgress (addCount);
        }

    }

    private void CheckGameComplete() {
        SaveLevelProgress ();
        ////forteset
        //string isNotShown = null;
        var isNotShown = lines.Find (x => !x.isShown);
        if (isNotShown == null) {
            ClearLevelProgress ();

            MainController.instance.OnComplete ();

            //统计提示点击次数
            if (EventTrackingController.instance != null) {
                hint.HintData ();
                if (mExtraWordClickCount > 0) {
                    EventTrackingController.instance.LogExtraWordClickEvent (mExtraWordClickCount);
                }
            }
            mExtraWordClickCount = 0;
            //if (lines.Count >= 6) {
            //    compliment.ShowRandom ();
            //}
        }
    }

    public void ExtraWordClick() {
        mExtraWordClickCount++;
    }

    public void HintClick(bool isFree) {
        var line = lines.Find (x => !x.isShown);
        if (line != null) {
            if (!isFree) {
                CurrencyController.DebitBalance (Const.HINT_COST);
            }
            line.ShowHint ();
            CheckGameComplete ();
            Prefs.AddToNumHint (GameState.currentWorld, GameState.currentSubWorld, GameState.currentLevel);
        }
    }

    public void SaveLevelProgress() {
        // if (!Prefs.IsLastLevel ()) return;

        List<string> results = new List<string> ();
        foreach (var line in lines) {
            StringBuilder sb = new StringBuilder ();
            foreach (var cell in line.cells) {
                sb.Append (cell.isShown ? "1" : "0");
            }
            results.Add (sb.ToString ());
        }

        Prefs.levelProgress = results.ToArray ();
    }

    public string[] GetLevelProgress() {
        // if (!Prefs.IsLastLevel ()) return new string[0];
        return Prefs.levelProgress;
    }

    public void ClearLevelProgress() {
        if (!Prefs.IsLastLevel ()) return;
        CPlayerPrefs.DeleteKey ("level_progress");
    }

    private void OnApplicationPause(bool pause) {
        if (!pause) {
            Timer.Schedule (this, 0.5f, () => {
                UpdateBoard ();
            });
        }
    }

    private void UpdateBoard() {
        string[] progress = GetLevelProgress ();
        if (progress.Length == 0) return;

        int i = 0;
        foreach (var line in lines) {
            line.SetProgress (progress[i]);
            i++;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SearchWordManager : MonoBehaviour {
    public RectTransform cell;
    public RectTransform wordGroup;
    public HintManager hint;
    public List<SearchWord> word=new List<SearchWord>();
  //  public Compliment compliment;

    private int wordRow;
    private int wordCol;
    private int combCount = 0;
    private float areaHeight;
    private float areaWidth;
    private float min;
    private float scale;
    private int rightCount = 0;//答对单词个数
    private string path = "";
    private List<LineData> lined = new List<LineData> ();
    private List<int> hintInital = new List<int> ();
    private List<SearchWord> answer = new List<SearchWord> ();
    private List<SearchCell> inital = new List<SearchCell> ();//存放答案的首字母
    private List<List<string>> wordPositon = new List<List<string>> ();
    private List<Color> colors = new List<Color> ();

    private void Awake() {
        hint.info = "searchhintcost";
        hint.SetHint ();
        hint.HintEvent += Hint;
        path = "SearchProgress.json";
        SearchSaveData saveData = TextManager.Instance.ReadProgress<SearchSaveData> (path);
        if (saveData != null) {
            lined = saveData.lineData;
            rightCount = lined.Count;
            hintInital = saveData.hintInital;
        }
        
        
    }

    public void LoadWord(SearchData data) {
        InitColor ();
        wordPositon = data.wordPositon;
        wordRow = wordPositon.Count;
        wordCol = wordPositon[0].Count;
        Calculate (wordRow, wordCol);

        for (int i = 0; i < wordRow; i++) {
            List<string> word = wordPositon[i];
            for (int j = 0; j < wordCol; j++) {
                RectTransform item;
                if (i == 0 && j == 0) {
                    item = cell;
                } else {
                    item = Instantiate (cell, wordGroup.transform);
                    item.SetSiblingIndex (i * wordCol + j);
                }
                SearchCell search_cell = item.GetComponent<SearchCell> ();
                search_cell.SetText (word[j].Substring (0, 1), scale, i, j);
                item.name = "Search" + i.ToString () + j.ToString ();
                item.anchoredPosition = new Vector3 (cell.anchoredPosition.x + j * min, cell.anchoredPosition.y - i * min);
                item.gameObject.SetActive (true);
                if (word[j].Length > 1) {
                    if (hintInital.Count != 0 && hintInital.Contains (i * wordCol + j)) {
                        search_cell.ChangeBgColor (Color.red);
                    } else {
                        inital.Add (search_cell);
                    }
                }
            }

        }
        float moveX = areaWidth - wordCol * min;
        float moveY = areaHeight - wordRow * min;
        wordGroup.anchoredPosition = new Vector2 (wordGroup.anchoredPosition.x + moveX / 2, wordGroup.anchoredPosition.y - moveY / 2);

        SearchController.instance.drawer.SetColor (colors[rightCount]);
        SearchController.instance.drawer.SetWidth (min);
        ShowAnswerWord (data.answer);
    }

    private void ShowAnswerWord(List<string> answerWord) {
        for (int i = 0; i < answerWord.Count; i++) {
            SearchWord search = new SearchWord ();
            search = word[i];
            //从保存数据判断是否是已经完成的
            LineData data = null;
            if (lined.Count != 0) {
                for (int j = 0; j < lined.Count; j++) {
                    if (lined[j].word.ToLower () == answerWord[i].ToLower ()) {
                        data = lined[j];
                        break;
                    }
                }
            }
            search.gameObject.SetActive (true);
            search.SetTextAndDoneWidth (answerWord[i]);
            if (data != null) {
                int beginIndex = data.begin.x * wordCol + data.begin.y;
                Vector2 begin = wordGroup.transform.GetChild (beginIndex).GetComponent<RectTransform> ().anchoredPosition;
                if(hintInital.Contains(beginIndex)) {
                    wordGroup.transform.GetChild (beginIndex).GetComponent<SearchCell> ().ChangeBgColor (Color.white);
                }
                Vector2 end = wordGroup.transform.GetChild (data.end.x * wordCol + data.end.y).GetComponent<RectTransform> ().anchoredPosition;
                SearchController.instance.drawer.SetRightLine (begin, end, colors[data.ColorIndex]);
                search.Done (colors[data.ColorIndex]);
            } else {
                answer.Add (search);
            }

        }
    }

    private void Calculate(int row, int col) {
        areaHeight = this.GetComponent<RectTransform> ().sizeDelta.y;
        areaWidth = this.GetComponent<RectTransform> ().sizeDelta.x;
        float currentWidth = wordGroup.sizeDelta.x / (float)row;
        float currentHeight = wordGroup.sizeDelta.y / (float)col;
        min = currentWidth < currentHeight ? currentWidth : currentHeight;
        scale = min / cell.sizeDelta.x;
        float x = min / 2 - areaWidth / 2;
        float y = areaHeight / 2 - min / 2;
        cell.anchoredPosition = new Vector2 (x, y);
        cell.sizeDelta = new Vector2 (min, min);

    }

    public bool CheckAnswer(SearchCell begin, SearchCell end) {
        if (begin == end) {
            return false;
        }
        bool isRight = false;
        string word = SetAnswer (begin.pos, end.pos);
        for (int i = 0; i < answer.Count; i++) {
            if (answer[i].word.text.ToLower () == word.ToLower ()) {
                AddLinePos (begin.pos, end.pos, word.ToLower ());
                isRight = true;
                answer[i].Done (colors[rightCount]);
                answer.RemoveAt (i);
                SearchCell sc = SearchController.instance.drawer.beginCell;
                if (inital.Contains (sc)) {
                    inital.Remove (sc);
                }
                sc.ChangeBgColor (Color.white);
                rightCount++;
                SearchController.instance.drawer.SetColor (colors[rightCount]);
                CheckGameComplete ();
                break;
            }
        }
        if (isRight) {
            combCount++;
            Compliment.Instance.ShowRandom (combCount);
        } else {
            combCount = 0;
        }
        return isRight;
    }

    public string SetAnswer(Position begin, Position end) {
        string word = "";
        //在同一行的情况
        if (begin.x == end.x) {
            if (begin.y < end.y) {
                for (int i = begin.y; i <= end.y; i++) {
                    word += wordPositon[begin.x][i].Substring (0, 1);
                }
            } else {
                for (int i = begin.y; i >= end.y; i--) {
                    word += wordPositon[begin.x][i].Substring (0, 1);
                }
            }
        } else if (begin.y == end.y) {//在同一列的情况
            if (begin.x < end.x) {
                for (int j = begin.x; j <= end.x; j++) {
                    word += wordPositon[j][begin.y].Substring (0, 1);
                }
            } else {
                for (int j = begin.x; j >= end.x; j--) {
                    word += wordPositon[j][begin.y].Substring (0, 1);
                }
            }
        } else {//不在同一列和同一行的情况
            int colNum = 1;
            int rowNum = 1;
            if (begin.y > end.y) {
                colNum = -1;
            }
            if (begin.x > end.x) {
                rowNum = -1;
            }
            int col = begin.y;
            int row = begin.x;
            int count = 0;
            while (count <= Mathf.Abs (begin.x - end.x)) {
                word += wordPositon[row][col].Substring (0, 1);
                row += rowNum;
                col += colNum;
                count++;
            }

        }

        return word;
    }

    private void CheckGameComplete() {
        SaveProgress ();
        if (answer.Count <= 0) {
            hint.HintData ();
            //处理数据
            CSVReadManager.Instance.searchData = new SearchData ();
            GameState.searchLevel++;
            PlayerDataManager.Instance.playerData.searchLevelID= GameState.searchLevel;
            PlayerDataManager.Instance.playerData.isSearchFinish= true;
            TextManager.Instance.DeleteFile (path);

            PlayerDataManager.Instance.playerData.searchCount++;
            PlayerDataManager.Instance.JudeReachAchieve (10, PlayerDataManager.Instance.playerData.searchCount);
            Timer.Schedule (this, 1, () => {
                DialogController.instance.ShowDialog (DialogType.DailyWin);
            });
        }
    }

    private void Hint(bool isFree) {
        if (inital.Count > 0) {

            if (!isFree) {
                CurrencyController.DebitBalance (Const.HINT_COST);
            }
            inital[0].ChangeBgColor (Color.red);
            hintInital.Add (inital[0].pos.x * wordCol + inital[0].pos.y);
            inital.RemoveAt (0);
            SaveProgress ();
        }
        
    }

    public void AddLinePos(Position begin, Position end, string word) {
        LineData line = new LineData ();
        line.begin = begin;
        line.end = end;
        line.word = word;
        line.ColorIndex = rightCount;
        lined.Add (line);
    }

    private void SaveProgress() {
        SearchSaveData data = new SearchSaveData ();
        data.lineData = lined;
        data.hintInital = hintInital;
        TextManager.Instance.SaveProgress (path, data);
    }

    private void InitColor() {
        colors.Add (new Color (126f / 255f, 108f / 255f, 220f / 255f));
        colors.Add (new Color (43f / 255f, 146f / 255f, 135f / 255f));
        colors.Add (new Color (179f / 255f, 148f / 255f, 163f / 255f));
        colors.Add (new Color (238f / 255f, 206f / 255f, 110f / 255f));
        colors.Add (new Color (121f / 255f, 69f / 255f, 69f / 255f));
        colors.Add (new Color (251f / 255f, 148f / 255f, 255f / 255f));
        colors.Add (new Color (255f / 255f, 170f / 255f, 170f / 255f));
        colors.Add (new Color (197f / 255f, 209f / 255f, 177f / 255f));

        colors.Add (new Color (104f / 255f, 245f / 255f, 247f / 255f));
        colors.Add (new Color (83f / 255f, 126f / 255f, 167f / 255f));
        colors.Add (new Color (255f / 255f, 255f / 255f, 255f / 255f));
        colors.Add (new Color (250f / 255f, 196f / 255f, 132f / 255f));
        colors.Add (new Color (157f / 255f, 223f / 255f, 180f / 255f));
        colors.Add (new Color (162f / 255f, 53f / 255f, 89f / 255f));
        colors.Add (new Color (237f / 255f, 114f / 255f, 189f / 255f));
        colors.Add (new Color (1f / 255f, 204f / 255f, 255f / 255f));

        colors.Add (new Color (250f / 255f, 102f / 255f, 68f / 255f));
        colors.Add (new Color (240f / 255f, 156f / 255f, 67f / 255f));
        colors.Add (new Color (191f / 255f, 253f / 255f, 153f / 255f));
        colors.Add (new Color (180f / 255f, 101f / 255f, 141f / 255f));
    }


}

public class SearchSaveData {
    public List<int> hintInital = new List<int> ();//已经提示了的首字母
    public List<LineData> lineData = new List<LineData> ();
}

public class LineData {
    public Position begin = new Position ();
    public Position end = new Position ();
    public int ColorIndex;
    public string word;
}
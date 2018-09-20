using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Linq;

public class PuzzleController : BaseController {
    public Transform wordGroup;
    public Transform blockGroup;
    public Transform hintGroup;

    public Text tipRow;
    public Text tipCol;
    public RectTransform rowContent;
    public RectTransform colContent;
    public GameObject hintText;

    public PuzzleLetterCell letterCell;
    public PuzzleBlock block;
    public GameObject blockParent;
    public HintManager hint;

    public int levelId;

    private float minSize = 0f;
    private float hintMin = 30;//当row为7的时候为30

    private int wordRow;
    private int wordCol;
    private PuzzleData data;
    private Vector2 blockOri;
    private Vector2 cellOri;

    private PuzzleSaveData saveData;

    private List<PuzzleLetterCell> cellGroup = new List<PuzzleLetterCell> ();//存放所有的格子信息
    private List<List<string>> wordInfo = new List<List<string>> ();
    private List<BlockParentManager> notPlaceRightBlock = new List<BlockParentManager> ();//还没有放置正确的方块
    private List<BlockParentManager> blocks = new List<BlockParentManager> ();//所有的方块 主要是用于保存功能
    private List<BlockParentManager> place = new List<BlockParentManager> ();//已经放置了的方块
    [HideInInspector]
    public int totalPlaceCount = 0;//总共需要放置的方块数量
    private List<Color> colors = new List<Color> ();
    public static PuzzleController instance;
    protected override void Awake() {
        base.Awake ();
        instance = this;
        blockOri = block.GetComponent<RectTransform> ().anchoredPosition;
        cellOri = letterCell.GetComponent<RectTransform> ().anchoredPosition;
        saveData = TextManager.Instance.ReadProgress<PuzzleSaveData> ("PuzzleProgress.json");
        hint.info = "puzzlehintcost";
        hint.SetHint ();
        hint.HintEvent += ClickHint;
        InitColor ();
        SetReadData ();
    }

    private void SetReadData() {
        data = CSVReadManager.Instance.puzzleData;
        if (data != null) {
            wordInfo = data.wordInfo;
            SetWordGrid ();
            SetHintInfo (tipRow, data.tipsRow, rowContent, true);
            SetHintInfo (tipCol, data.tipsCol, colContent, false);
            SetBlockInfo ();
        }
    }

    //字母格子，默认区域是480 * 480 ,策划说的格子永远是正方形，所以row和col永远是相等的。
    private void Calculate(int row, int col) {
        float rowGap = (row - 1) * 2 + Mathf.Abs (cellOri.x * 2);
        float rowWidth = (480 - rowGap) / row;
        minSize = rowWidth;
        block.GetComponent<RectTransform> ().sizeDelta = new Vector2 (minSize, minSize);
        letterCell.GetComponent<RectTransform> ().sizeDelta = new Vector2 (minSize, minSize);
        hintMin = (7 - row) * 5 + 30;
        hintText.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (24 + (7 - row) * 2.5f, -24 - (7 - row) * 2.5f);
    }

    //点击格子显示Tips
    public void ShowHint(PuzzleLetterCell letter) {
        string rowHint = "";
        string colHint = "";
        Position pos = letter.pos;
        List<PuzzleLetterCell> rowCell = new List<PuzzleLetterCell> ();
        List<PuzzleLetterCell> colCell = new List<PuzzleLetterCell> ();
        foreach (PuzzleLetterCell cell in cellGroup) {
            cell.ClearHintBg ();
        }
        //row先向左遍历   然后再向右row的hint 都在最左边
        if (pos.y == 0) {
            rowHint = data.hintInfo[pos.x][pos.y];
        } else {
            for (int i = pos.y - 1; i >= 0; i--) {
                if (!string.IsNullOrEmpty (wordInfo[pos.x][i])) {
                    rowCell.Add (cellGroup[pos.x * wordCol + i]);
                    rowHint = data.hintInfo[pos.x][i];
                } else {
                    break;
                }
            }
        }
        for (int j = pos.y + 1; j < wordCol; j++) {
            if (!string.IsNullOrEmpty (wordInfo[pos.x][j])) {
                rowCell.Add (cellGroup[pos.x * wordCol + j]);
            } else {
                break;
            }
        }

        //向下的遍历  先向上
        if (pos.x == 0) {
            colHint = data.hintInfo[pos.x][pos.y];
        } else {
            for (int m = pos.x - 1; m >= 0; m--) {
                if (!string.IsNullOrEmpty (wordInfo[m][pos.y])) {
                    colCell.Add (cellGroup[m * wordCol + pos.y]);
                    colHint = data.hintInfo[m][pos.y];
                } else {
                    break;
                }
            }
        }
        for (int n = pos.x + 1; n < wordRow; n++) {
            if (!string.IsNullOrEmpty (wordInfo[n][pos.y])) {
                colCell.Add (cellGroup[n * wordCol + pos.y]);
            } else {
                break;
            }
        }
        //设置相关区域背景
        letter.SetHintBg (2);
        foreach (PuzzleLetterCell cell in rowCell) {
            cell.SetHintBg (0);
        }
        foreach (PuzzleLetterCell cell in colCell) {
            cell.SetHintBg (1);
        }
            foreach (Transform item in rowContent) {
                if (item.GetComponent<TipItem> ().tipIndex == rowHint) {
                    item.GetComponent<TipItem> ().ClickButtonOn();
                } else {
                    item.GetComponent<TipItem> ().ClickButtonOff ();
                }
            }
 
            foreach (Transform item in colContent) {
                if (item.GetComponent<TipItem> ().tipIndex == colHint) {
                    item.GetComponent<TipItem> ().ClickButtonOn ();
                } else {
                    item.GetComponent<TipItem> ().ClickButtonOff ();
                }
            }

    }

    //点击Tips区域显示格子里面的提示
    public void ShowCellTips(string tip) {
        for (int i = 0; i < data.hintInfo.Count; i++) {
            for (int j = 0; j < data.hintInfo[i].Count; j++) {
                if (data.hintInfo[i][j] == tip) {
                    ShowHint (cellGroup[i * wordCol + j]);
                    break;
                }
            }
        }
    }

    private void SetWordGrid() {
        wordRow = wordInfo.Count;
        wordCol = wordInfo[0].Count;
        Calculate (wordRow, wordCol);
        for (int i = 0; i < wordInfo.Count; i++) {
            List<string> letters = wordInfo[i];
            for (int j = 0; j < letters.Count; j++) {
                if (i == 0 && j == 0) {
                    letterCell.pos.x = i;
                    letterCell.pos.y = j;
                    letterCell.SetSize (minSize);
                    letterCell.GetComponent<RectTransform> ().anchoredPosition = CalculateWordPos (j, i);
                    if (!string.IsNullOrEmpty (wordInfo[i][j])) {
                        letterCell.gameObject.SetActive (true);
                        if (data.hintInfo.Count != 0 && !string.IsNullOrEmpty (data.hintInfo[i][j])) {
                            hintText.GetComponent<RectTransform> ().sizeDelta = new Vector2 (hintMin, hintMin);
                            hintText.GetComponentInChildren<Text> ().text = data.hintInfo[i][j];
                        }
                        if (saveData != null) {
                            letterCell.isShowing = saveData.letterShow[i][j];
                        }
                    } else {
                        letterCell.isShowing = true;
                    }
                    cellGroup.Add (letterCell);
                } else {
                    PuzzleLetterCell cell = Instantiate (letterCell, wordGroup);
                    cell.pos.x = i;
                    cell.pos.y = j;
                    cell.SetSize (minSize);
                    cell.isShowing = false;
                    cell.gameObject.SetActive (false);
                    cell.GetComponent<RectTransform> ().anchoredPosition = CalculateWordPos (j, i);
                    cell.name = "letterCell" + i.ToString () + j.ToString ();
                    cellGroup.Add (cell);
                    if (!string.IsNullOrEmpty (wordInfo[i][j])) {
                        cell.gameObject.SetActive (true);
                        if (saveData != null) {
                            cell.isShowing = saveData.letterShow[i][j];
                        }
                        if (data.hintInfo.Count != 0 && !string.IsNullOrEmpty (data.hintInfo[i][j])) {
                            Vector2 cellSize = cell.GetComponent<RectTransform> ().sizeDelta;
                            GameObject hintClone = Instantiate (hintText, hintGroup);
                            hintClone.GetComponentInChildren<Text> ().text = data.hintInfo[i][j];
                            hintClone.GetComponent<RectTransform> ().sizeDelta = new Vector2 (hintMin, hintMin);
                            hintClone.GetComponent<RectTransform> ().anchoredPosition = new Vector2 ((minSize + 2) * j, (-minSize - 2) * i) + hintText.GetComponent<RectTransform> ().anchoredPosition;
                        }
                    } else {
                        cell.isShowing = true;
                    }
                }
            }
        }
    }

    private void SetHintInfo(Text tip, List<string> tipTexts, RectTransform content, bool isRowTips) {
        float height = 0;
        float x = 0;
        float y = 0;
        for (int i = 0; i < tipTexts.Count; i++) {
            Text currentText = tip;
            if (i == 0) {
            } else {
                Text clone = Instantiate (tip, content);
                clone.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (x, y - height);
                currentText = clone;
            }
            currentText.text = tipTexts[i];
            string tipId = currentText.text.Substring (0, 1);
            currentText.GetComponent<TipItem> ().tipIndex = tipId;
            currentText.GetComponent<TipItem> ().isRowTips = isRowTips;
            height = currentText.preferredHeight;
            x = currentText.GetComponent<RectTransform> ().anchoredPosition.x;
            y = currentText.GetComponent<RectTransform> ().anchoredPosition.y;
            content.sizeDelta = new Vector2 (content.sizeDelta.x, content.sizeDelta.x + height);

        }
    }

    private void SetBlockInfo() {
        totalPlaceCount = data.puzzleInfo.Count;
        for (int i = 0; i < data.puzzleInfo.Count; i++) {
            BlockInfo info = data.puzzleInfo[i];
            GameObject parent = Instantiate (blockParent, blockParent.transform.parent);
            parent.name =  info.blockNum.ToString ();
            //设置方块的父物体
            Vector2 min = CalculateBlockPos (info.blockRect.min.y, info.blockRect.min.x);
            Vector2 max = CalculateBlockPos (info.blockRect.max.y, info.blockRect.max.x);
            parent.GetComponent<RectTransform> ().anchoredPosition = CalculateCenter (min, max);


            //计算对应单词的中心点
            Vector2 wordMin = CalculateWordPos (info.wordRect.min.y, info.wordRect.min.x);
            Vector2 wordMax = CalculateWordPos (info.wordRect.max.y, info.wordRect.max.x);
            Vector2 wordCenter = CalculateCenter (wordMin, wordMax);
            //设置父物体的信息
            int row = info.blockRect.max.x - info.blockRect.min.x;
            int col = info.blockRect.max.y - info.blockRect.min.y;
            float width = Mathf.Abs ((max.x - min.x) / 2) == 0 ? 0 : Mathf.Abs ((max.x - min.x) / 2);
            float height = Mathf.Abs ((max.y - min.y) / 2) == 0 ? 0 : Mathf.Abs ((max.y - min.y) / 2);
            BlockParentManager manager = parent.GetComponent<BlockParentManager> ();
            manager.blockNum = info.blockNum;
            manager.SetBlockInfo (wordCenter, width+minSize/2, height+minSize/2, row, col, info.blockRect.min);
            manager.SetRightCells (AddCorrectCells (info.wordPos));
            for (int j = 0; j < info.blockPos.Count; j++) {
                int x = info.blockPos[j].pos.x;
                int y = info.blockPos[j].pos.y;
                PuzzleBlock cell = Instantiate (block, blockGroup);
                cell.GetComponent<RectTransform> ().anchoredPosition = CalculateBlockPos (y, x);
                cell.pos.x = x;
                cell.pos.y = y;
                cell.SetSize (minSize);
                cell.SetBlockInfo (info.blockPos[j].letter, colors[info.blockNum], parent.transform);
                cell.gameObject.SetActive (true);
                manager.SetPos (x, y);
                manager.AddBlock (cell);
            }
            //读取进度
            if (saveData != null) {
                PuzzleBlockPosition pos = saveData.blockPosition[i];
                parent.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (float.Parse (pos.x), float.Parse (pos.y));
                if (saveData.HintBlock.Contains (info.blockNum)) {
                    notPlaceRightBlock.Add (manager);
                }
                for (int m = 0; m < saveData.blockNum.Count; m++) {
                    if (saveData.blockNum[m] == info.blockNum) {
                        List<PuzzleLetterCell> letters = new List<PuzzleLetterCell> ();
                        for (int n = 0; n < saveData.blockPlaceCell[m].Count; n++) {
                            letters.Add (cellGroup[saveData.blockPlaceCell[m][n]]);
                        }
                        manager.cells = letters;
                        saveData.blockNum.RemoveAt (m);
                        break;
                    }
                }
                for (int k = 0; k < saveData.placeBlock.Count; k++) {
                    if (saveData.placeBlock[k] == info.blockNum) {
                        place.Add (manager);
                        place.RemoveAt (k);
                        break;
                    }
                }
            } else {
                notPlaceRightBlock.Add (manager);
            }
            blocks.Add (manager);
        }
    }

    public List<PuzzleLetterCell> AddCorrectCells(List<CellInfo> wordInfo) {
        List<PuzzleLetterCell> cells = new List<PuzzleLetterCell> ();
        for (int i = 0; i < wordInfo.Count; i++) {
            int index = wordInfo[i].pos.x * wordCol + wordInfo[i].pos.y;
         //   Debug.Log ("Index :" + index + "  ChildCout ：" + wordGroup.childCount);
            cells.Add (wordGroup.GetChild (index).GetComponent<PuzzleLetterCell> ());
        }
        return cells;
    }
    //计算方块在父物体的位置
    private Vector2 CalculateBlockPos(int x, int y) {
        return new Vector2 (blockOri.x + (minSize + 3) * x, blockOri.y - (minSize + 3) * y);
    }

    private Vector2 CalculateWordPos(int x, int y) {
        return new Vector2 (cellOri.x + (minSize + 2) * x, cellOri.y - (minSize + 2) * y);
    }

    private Vector2 CalculateCenter(Vector2 min, Vector2 max) {
        max = new Vector2 (max.x+minSize,max.y-minSize);//max的坐标要固定在右下角
        Vector2 center = Vector2.one;
        if (min.x == max.x) {
            center.x = min.x;
        } else {
            center.x = max.x - Mathf.Abs ((max.x - min.x) / 2);
        }
        if (min.y == max.y) {
            center.y = min.y;
        } else {
            center.y = max.y + Mathf.Abs ((max.y - min.y) / 2);
        }

        return center;
    }

    public void CalculateBlockLocation(BlockParentManager manager, Vector2 pos, int row, int col) {
        //这个代表的是在第几列
        int indexX = Mathf.RoundToInt ((pos.x) / minSize) ;
        if (indexX == -1 && pos.x > cellOri.x - minSize / 2f) {
            indexX = 0;
        }
        if(indexX== wordCol && pos.x < cellOri.x + minSize * wordCol+2* (wordCol-1)-10) {
            indexX = wordCol-1;
        }
       //  Debug.Log ("移动的横坐标是 ： " + indexX);
        if (indexX < 0 || indexX >= wordCol || indexX + col >= wordCol) {
            manager.Back ();
            return;
        }
        //这个代表的是在第几行
        int indexY = Mathf.RoundToInt ((-pos.y) / minSize);
        if (indexY == -1 && pos.y < cellOri.y + minSize-10f) {
            indexY = 0;
        }
       // Debug.Log ("移动的纵坐标是 ： " + indexY);
        if (indexY < 0 || indexY >= wordRow || indexY + row >= wordRow) {
            manager.Back ();
            return;
        }
        int m = indexX;
        int k = indexY;
        List<PuzzleLetterCell> cells = new List<PuzzleLetterCell> ();
        for (int i = 0; i < manager.pos.GetLength (0); i++) {
            for (int j = 0; j < manager.pos.GetLength (1); j++) {
                PuzzleLetterCell cell = wordGroup.GetChild (k * wordCol + m).GetComponent<PuzzleLetterCell> ();
                if (manager.pos[i, j] == 1) {
                    if (cell.isShowing) {
                        manager.Back ();
                        return;
                    } else {
                        cells.Add (cell);
                    }
                }
                m++;
            }
            m = indexX;
            k++;
        }
        PlaceBlock (manager, true);
        manager.SetBlockPosition (CalculateWordPos (indexX, indexY), cells);


    }

    private void ClickHint(bool isFree) {
        if (notPlaceRightBlock.Count > 0) {
            if (!isFree) {
                CurrencyController.DebitBalance (Const.HINT_COST);
            }
            PlaceBlock (notPlaceRightBlock[0], true);
            notPlaceRightBlock[0].ShowHint ();
            notPlaceRightBlock[0].isPlaceRight = true;
            Correct (notPlaceRightBlock[0]);
            CheckGameComplete ();
        }

    }
    //移除阻挡hint的方块。
    public void RemoveHintBlock(List<PuzzleLetterCell> right,BlockParentManager current) {
        for(int i = 0; i < blocks.Count; i++) {
            if (blocks[i].cells.Count != 0 && current!=blocks[i]) {
                if(blocks[i].cells.Any (v => right.Contains (v))) {
                    blocks[i].SetCellState (false);
                    NotCorrect (blocks[i]);
                    blocks[i].Back ();
                }
            }
        }
    }

    public void CheckGameComplete() {
        Timer.Schedule (this, 0.6f, () => {
            SaveProcess ();
        });
        if (notPlaceRightBlock.Count == 0) {
            hint.HintData ();
            CSVReadManager.Instance.puzzleData = new PuzzleData ();
            GameState.puzzleLevel++;
            PlayerDataManager.Instance.playerData.puzzleLevelID = GameState.puzzleLevel;
            PlayerDataManager.Instance.playerData.isPuzzleFinish = true;
            PlayerDataManager.Instance.playerData.puzzleCount++;
            PlayerDataManager.Instance.JudeReachAchieve (13, PlayerDataManager.Instance.playerData.puzzleCount);
            Timer.Schedule (this, 1, () => {
                TextManager.Instance.DeleteFile ("PuzzleProgress.json");
                DialogController.instance.ShowDialog (DialogType.DailyWin);
            });
        } else if (place.Count == totalPlaceCount) {
            foreach (BlockParentManager item in place) {
                item.ShowBlocksWrong ();
            }
        }
    }
    //方块到了正确的位置
    public void Correct(BlockParentManager manager) {
        if (notPlaceRightBlock.Contains (manager)) {
            manager.isPlaceRight = true;
            notPlaceRightBlock.Remove (manager);
        }
    }
    //方块的位置不正确
    public void NotCorrect(BlockParentManager manager) {
        if (!notPlaceRightBlock.Contains (manager)) {
            notPlaceRightBlock.Add (manager);

        }
    }

    public void PlaceBlock(BlockParentManager manager, bool isPlace) {
        if (isPlace) {
            if (!place.Contains (manager)) {
                place.Add (manager);
            }
        } else {
            if (place.Contains (manager)) {
                place.Remove (manager);
            }
        }
    }
    //保存进度
    private void SaveProcess() {
        PuzzleSaveData data = new PuzzleSaveData ();
        for (int i = 0; i < wordRow; i++) {
            List<bool> show = new List<bool> ();
            for (int j = 0; j < wordCol; j++) {
                show.Add (cellGroup[i * wordCol + j].isShowing);
            }
            data.letterShow.Add (show);
        }
        for (int j = 0; j < notPlaceRightBlock.Count; j++) {
            data.HintBlock.Add (notPlaceRightBlock[j].blockNum);
        }
        for (int w = 0; w < place.Count; w++) {
            data.HintBlock.Add (place[w].blockNum);
        }

        for (int m = 0; m < blocks.Count; m++) {
            PuzzleBlockPosition pos = new PuzzleBlockPosition ();
            float x = blocks[m].GetComponent<RectTransform> ().anchoredPosition.x;
            float y = blocks[m].GetComponent<RectTransform> ().anchoredPosition.y;
            pos.x = x.ToString ();
            pos.y = y.ToString ();
            data.blockPosition.Add (pos);

            if (blocks[m].cells.Count != 0) {
                data.blockNum.Add (blocks[m].blockNum);
                List<int> index = new List<int> ();
                for (int n = 0; n < blocks[m].cells.Count; n++) {
                    index.Add (blocks[m].cells[n].transform.GetSiblingIndex ());
                }
                data.blockPlaceCell.Add (index);
            }
        }
        TextManager.Instance.SaveProgress<PuzzleSaveData> ("PuzzleProgress.json", data);

    }

    public void InitColor() {
        colors.Add (new Color (90f / 255f, 138f / 255f, 187f / 255f));
        colors.Add (new Color (30f / 255f, 103f / 255f, 203f / 255f));
        colors.Add (new Color (0f / 255f, 174f / 255f, 255f / 255f));
        colors.Add (new Color (224f / 255f, 158f / 255f, 158f / 255f));
        colors.Add (new Color (255f / 255f, 125f / 255f, 165f / 255f));
        colors.Add (new Color (136f / 255f, 202f / 255f, 152f / 255f));
        colors.Add (new Color (0f / 255f, 237f / 255f, 173f / 255f));
        colors.Add (new Color (112f / 255f, 231f / 255f, 59f / 255f));


        colors.Add (new Color (209f / 255f, 202f / 255f, 152f / 255f));
        colors.Add (new Color (255f / 255f, 188f / 255f, 92f / 255f));
        colors.Add (new Color (187f / 255f, 90f / 255f, 90f / 255f));
        colors.Add (new Color (172f / 255f, 132f / 255f, 193f / 255f));
        colors.Add (new Color (134f / 255f, 78f / 255f, 237f / 255f));
        colors.Add (new Color (234f / 255f, 121f / 255f, 255f / 255f));
        colors.Add (new Color (219f / 255f, 197f / 255f, 255f / 255f));
        colors.Add (new Color (120f / 255f, 225f / 255f, 255f / 255f));

        colors.Add (new Color (63f / 255f, 169f / 255f, 95f / 255f));
        colors.Add (new Color (255f / 255f, 136f / 255f, 83f / 255f));
        colors.Add (new Color (255f / 255f, 217f / 255f, 0f / 255f));
        colors.Add (new Color (255f / 255f, 120f / 255f, 0f / 255f));
    }
}
public class PuzzleSaveData {
    public List<List<bool>> letterShow = new List<List<bool>> ();//格子里面的显示情况
    public List<int> HintBlock = new List<int> ();//还可以提示的Block，也就是还没有摆放正确的方块
    public List<int> placeBlock = new List<int> ();//已经放置的方块
    public List<PuzzleBlockPosition> blockPosition = new List<PuzzleBlockPosition> ();
    public List<int> blockNum = new List<int> ();//存放，方块有放置cell的下标
    public List<List<int>> blockPlaceCell = new List<List<int>> ();//方块放置的cell
}
public class PuzzleBlockPosition {
    public string x = "";
    public string y = "";
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.IO;
using LitJson;
public class PuzzleAreaManager : MonoBehaviour {
    public Transform wordGroup;
    public Transform blockGroup;
    public PuzzleCell wordCell;
    public BlockLetter block;
    public GameObject letterGroup;
    public GameObject colorGroup;
    public GameObject opGroup;
    public Text levelText;
    public InputField levelId;
    public Text hint;
    public InputField hintField;
    public Transform hintGroup;

    public Button startSetBlock;
    public Button modifierWord;//修改单词
    public Button saveBtn;
    public List<InputField> rowInput = new List<InputField> ();
    public List<InputField> colInput=new List<InputField>();
    public List<PuzzleColor> puzzleColor = new List<PuzzleColor> ();
   
    [HideInInspector]
    public string letter;//字母
    [HideInInspector]
    public PuzzleCell currentItem;
    [HideInInspector]
    public PuzzleCell downItem;
    [HideInInspector]
    public BlockLetter currentBlock;
    [HideInInspector]
    public int currentColor = 0;//当前选中的颜色
    [HideInInspector]
    public bool clickColor = true;

    private float min = 0f;
    private float hintMin=30;//当row为7的时候为30

    private PuzzleLoadData loadData = new PuzzleLoadData ();
    private PuzzleData data;
    private int row = 7;
    private int col = 7;//默认为7 和 7

    private bool setColor = false;
    private bool isStart = false;

    private Vector2 cellOri = Vector2.one;
    private Vector2 blockOri = Vector2.one;
    private Dictionary<int, List<CellInfo>> wordPosDic = new Dictionary<int, List<CellInfo>> ();
    private Dictionary<int, List<CellInfo>> blockPosDic = new Dictionary<int, List<CellInfo>> ();

    private List<Color> colors = new List<Color> ();
    
    public static PuzzleAreaManager instance;

    private void Awake() {
        instance = this;
        levelText.text = string.Format (levelText.text,"Puzzle");
        cellOri = wordCell.GetComponent<RectTransform> ().anchoredPosition;
        min = wordCell.GetComponent<RectTransform> ().sizeDelta.x;
        blockOri = block.GetComponent<RectTransform> ().anchoredPosition;
        InitColor ();
        SetColor ();
        saveBtn.onClick.AddListener (() => {
            Save ();
            modifierWord.gameObject.SetActive (false);
            startSetBlock.gameObject.SetActive (false);
        });
        modifierWord.onClick.AddListener (() => {
            currentBlock = null;
            currentItem = null;
            downItem = null;
            //blockText = "";
            blockGroup.gameObject.SetActive (false);
            wordGroup.gameObject.SetActive (true);
            opGroup.SetActive (true);
            letterGroup.SetActive (true);
            colorGroup.SetActive (false);
            modifierWord.gameObject.SetActive (false);
            startSetBlock.gameObject.SetActive (true);
            saveBtn.gameObject.SetActive (false);
            setColor = false;
        });
        startSetBlock.onClick.AddListener (() => {
            modifierWord.gameObject.SetActive (true);
            blockGroup.gameObject.SetActive (true);
            SetHintCell ();
            SetBlockGrid ();
            setColor = true;
            opGroup.SetActive (false);
            letterGroup.SetActive (false);
            colorGroup.SetActive (true);
            currentItem = null;
            letter = "";
            startSetBlock.gameObject.SetActive (false);
            saveBtn.gameObject.SetActive (true);
        });
    }

    //加载旧关卡
    public void LoadData(string path,string levelID) {
        levelId.text = levelID;
        loadData = ReadPuzzleData (path);
        row = loadData.wordInfo.Count;
        col = loadData.wordInfo[0].Count;
        SetWordGrid ();
        //tipsRow.text = loadData.tipsRow;
        //tipsCol.text = loadData.tipsCol;
        for(int i = 0; i < loadData.tipsRow.Count; i++) {
            rowInput[i].text = loadData.tipsRow[i];
        }
        for (int j = 0; j < loadData.tipsCol.Count; j++) {
           colInput[j].text = loadData.tipsCol[j];
        }

    }

    //加载新关卡
    public  void LoadNew() {
        blockGroup.gameObject.SetActive (false);
        SetWordGrid ();
        
    }

    //设置方块的放置格子
    private void SetBlockGrid() {
        if (blockGroup.childCount > 1) {
           // SetBlockSizeAndPos ();
            blockGroup.gameObject.SetActive (true);
        } else {
            float offset = 2;
            for (int i = 0; i < 10; i++) {
                for (int j = 0; j < 10; j++) {
                        if (j >= col || i>=row) {
                            offset = 3;
                        }
                    if (i == 0 && j == 0) {

                    } else {
                        BlockLetter cell = CloneBlock (i, j, offset);
                        if (loadData.blockInfo.Count != 0) {
                            if (loadData.blockColorIndex[i][j] != -1) {
                                int colorIndex = loadData.blockColorIndex[i][j];
                                cell.SetColor (colorIndex, colors[colorIndex]);
                                cell.letter.text = loadData.blockInfo[i][j];
                            }
                        }
                    }
                }
            }
        }
    }
    private void SetBlockSizeAndPos() {
        int index = 0;
        for (int i = 0; i < 10; i++) {
            for (int j = 0; j < 10; j++) {
                RectTransform cell = blockGroup.GetChild (index).GetComponent<RectTransform> ();
                cell.gameObject.SetActive (false);
                if (j >= col || i >= row) {
                    cell.sizeDelta = new Vector2 (min,min);
                    cell.GetComponent<BlockLetter>().pos.x= i;
                    cell.GetComponent<BlockLetter> ().pos.y= j;
                    cell.anchoredPosition = new Vector2 (blockOri.x + (min + 3) * j, blockOri.y - (min + 3) * i);
                    if (cell.anchoredPosition.x + min < 720 && cell.anchoredPosition.y - min > -720) {
                        cell.gameObject.SetActive (true);
                    }
                }
                index++;
             
            }
        }
    }

    //设置单词格子
    private void SetWordGrid() {
        Calculate ();
        int index = 0;
        for (int i = 0; i < row; i++) {
            for (int j = 0; j < col; j++) {
                if (i == 0 && j == 0) {
                    wordCell.ClearColor ();
                    wordCell.pos.x = i;
                    wordCell.pos.y = j;
                    wordCell.gameObject.SetActive (true);
                    wordCell.SetSize (min);
                    if (loadData.wordInfo.Count != 0 && loadData.wordColorIndex[i][j] != -1) {
                        wordCell.SetText (loadData.wordInfo[i][j]);
                        wordCell.SetColor (loadData.wordColorIndex[i][j],colors[loadData.wordColorIndex[i][j]]);
                    }
 
                } else {
                    PuzzleCell cell = CloneCell (i,j,index);
                   
                    if (loadData.wordInfo.Count != 0 && loadData.wordColorIndex[i][j]!=-1) {
                        cell.SetText (loadData.wordInfo[i][j]);
                        cell.SetColor (loadData.wordColorIndex[i][j], colors[loadData.wordColorIndex[i][j]]);
                    }
                }
                index++;
            }
        }
        isStart = true;
    }
    private void SetHintCell() {
        if (hintGroup.childCount > 1) {
            return;
        }
        for (int i = 0; i < row; i++) {
            for (int j = 0; j < col; j++) {
                if(i==0 && j == 0) {
                    if (loadData.hintInfo.Count != 0 && !string.IsNullOrEmpty (loadData.hintInfo[i][j])) {
                        hintField.text = loadData.hintInfo[i][j];
                    }
                    hintField.gameObject.SetActive (true);
                    hintField.GetComponent<RectTransform>().sizeDelta= new Vector2 (hintMin, hintMin);
                } else {
                    InputField hintClone = SetHintPosition ( i, j); 
                    if (loadData.hintInfo.Count != 0 && !string.IsNullOrEmpty (loadData.hintInfo[i][j])) {
                        hintClone.text = loadData.hintInfo[i][j];
                    }
                }
            }
        }
    }

    //字母格子，默认区域是480 * 480 ,策划说的格子永远是正方形，所以row和col永远是相等的。
    private void Calculate() {
        float rowGap = (row - 1) * 2+Mathf.Abs( cellOri.x*2);
        float rowWidth = (480 - rowGap) / row;
        min = rowWidth;
        block.GetComponent<RectTransform> ().sizeDelta = new Vector2 (min, min);
        wordCell.GetComponent<RectTransform> ().sizeDelta = new Vector2 (min,min);
        hintMin = (7 - row) * 5 + 30;
        hintField.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (24+ (7 - row) * 2.5f,-24- (7 - row) * 2.5f);
    }
    
    //克隆方格
    private PuzzleCell CloneCell(int i,int j,int index) {
        PuzzleCell cell = Instantiate (wordCell, wordGroup);
        cell.name = "word" + i.ToString () + j.ToString ();
        cell.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (cellOri.x + (min + 2) * j, cellOri.y - (min + 2) * i);
        cell.pos.x = i;
        cell.pos.y = j;
        cell.SetText ("");
        cell.ClearColor();
        cell.SetSize (min);
        cell.transform.SetSiblingIndex (index);
        cell.gameObject.SetActive (true);
     
        return cell;
        
    }

    private BlockLetter CloneBlock(int i,int j,float offset) {
        BlockLetter cell = Instantiate (block, blockGroup);
        cell.name = "block" + i.ToString () + j.ToString ();
        cell.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (blockOri.x + (min + offset) * j, blockOri.y - (min + offset) * i);
        cell.pos.x = i;
        cell.pos.y = j;
        cell.ClearColor ();
        cell.SetSize (min);
        if ((i < row && j >= col) || i >= row) {
            Vector2 pos = cell.GetComponent<RectTransform> ().anchoredPosition;
            if (pos.x + min < 720 && pos.y - min > -720) {
                cell.gameObject.SetActive (true);
            }
        }
        return cell;
    }
    //设置提示的位置
    private InputField SetHintPosition(int i, int j) {
        InputField hint = Instantiate (hintField, hintGroup);
        hint.name = "hint" + i.ToString () + j.ToString ();
        hint.text = "";
        hint.GetComponent<RectTransform> ().sizeDelta = new Vector2 (hintMin, hintMin);
        hint.GetComponent<RectTransform> ().anchoredPosition = new Vector2 ((min + 2) * j, (-min - 2) * i) + hintField.GetComponent<RectTransform> ().anchoredPosition;

        return hint;
    }

    private void SetColor() {
        for (int i = 0; i < colors.Count; i++) {
            puzzleColor[i].SetColor (i, colors[i]);
        }
    }

    void Update() {
        if (Input.GetMouseButtonDown (0) && isStart) {
            if (setColor) {

                if (currentItem != null) {
                    
                   // blockText = currentItem.letter.text;
                    currentItem. SetColor (currentColor,colors[currentColor]);
                }
                if (currentBlock != null && downItem!=null ) {
                    if (downItem.blockIndex != -1) {
                        currentBlock.SetText (downItem.letter.text);
                        currentBlock.SetColor (downItem.blockIndex, colors[downItem.blockIndex]);
                        downItem = null;
                    }
                }
                
            } else {
                if (currentItem != null) {
                    currentItem.SetText (letter);
                }
            }
        }
        if (Input.GetMouseButtonDown (1)) {
            if (setColor) {
                if (currentBlock != null) {
                    currentBlock.ClearColor ();
                }
                if (currentItem != null) {
                    currentItem.ClearColor ();
                }
            } else {
                if (currentItem != null) {
                    currentItem.SetText ("");
                }
            }
        }

    }

    //删减操作
    public void AddCol() {
        if (col >= 7) {
            return;
        }
        col++;
        row++;
        Calculate ();
        int index = 0;
        for (int i = 0; i < row; i++) {
            for (int j = 0; j < col; j++) {
                if (j == col - 1 || i == row - 1) {
                    CloneCell (i, j,index);
                   // SetHintPosition (null, i, j, true);
                } else {
                    wordGroup.GetChild (index).GetComponent<RectTransform> ().sizeDelta = new Vector2 (min, min);
                    wordGroup.GetChild (index).GetComponent<RectTransform> ().anchoredPosition = new Vector2 (cellOri.x + (min + 2) * j, cellOri.y - (min + 2) * i);
                    wordGroup.GetChild (index).GetComponent<PuzzleCell> ().SetSize (min);
                   // float preHintScale = hintGroup.GetChild (index).localScale.x;
                  //  SetHintPosition (hintGroup.GetChild (index).GetComponent<InputField> (), i, j,false);
                   
                }
                index++;
            }
        }

    }

    public void DeleteCol() {
        if (col <= 3) {
            return;
        }
        col--;
        row--;
        Calculate ();
        int index = 0;
        for (int i = 0; i < row+1; i++) {
            for (int j = 0; j < col+1; j++) {
                if (j == col || i==row) {
                    DestroyImmediate (wordGroup.GetChild (index).gameObject);
                   // DestroyImmediate (hintGroup.GetChild (index).gameObject);
                   // Destroy (wordGroup.GetChild (index));
                } else {
                   // GameObject obj = wordGroup.GetChild (index).gameObject;
                   // float preHintScale = hintGroup.GetChild (index).localScale.x;
                   // SetHintPosition (hintGroup.GetChild (index).GetComponent<InputField> (), i, j,false);
                    wordGroup.GetChild (index).GetComponent<RectTransform> ().sizeDelta = new Vector2 (min, min);
                    wordGroup.GetChild (index).GetComponent<RectTransform> ().anchoredPosition = new Vector2 (cellOri.x + (min + 2) * j, cellOri.y - (min + 2) * i);
                    wordGroup.GetChild (index).GetComponent<PuzzleCell> ().SetSize (min);
                    index++;
                }
              
            }
        }
    }

    //保存操作
    public void Save() {
        if (string.IsNullOrEmpty (levelId.text)) {
            HintShow ("请输入关卡ID ！ ",true);
            return;
        }
        data = new PuzzleData ();
        loadData = null;
        loadData = new PuzzleLoadData ();
        for(int i = 0; i < rowInput.Count; i++) {
            if (!string.IsNullOrEmpty (rowInput[i].text)) {
                data.tipsRow.Add (rowInput[i].text);
            }
        }
        loadData.tipsRow = data.tipsRow;
        for (int i = 0; i < colInput.Count; i++) {
            if (!string.IsNullOrEmpty (colInput[i].text)) {
                data.tipsCol.Add (colInput[i].text);
            }
        }
        loadData.tipsCol = data.tipsCol;


        CalculateWord ();
        CalculateBlock ();
        CalculateCenter ();
        string path = Application.streamingAssetsPath + "/DailyData/PuzzleData/puzzle_" + levelId.text + ".json";
        string loadPath= Application.dataPath + "/WordChef/PuzzlePos/puzzlePos_" +levelId.text + ".json";
        WritePuzzleData (path,data);
        WritePuzzleLoadData (loadPath,loadData);
        HintShow ("保存成功");
        saveBtn.gameObject.SetActive (false);
        wordPosDic.Clear ();
        blockPosDic.Clear ();
    }

    private void CalculateWord() {
        int index = 0;
        for(int i = 0; i < row; i++) {
            List<string> letters = new List<string> ();
            List<string> hintInfo = new List<string> ();
            List<int> loadColor = new List<int> ();
            for(int j = 0; j < col; j++) {
                List<CellInfo> list = new List<CellInfo> ();
                CellInfo info = new CellInfo ();
                PuzzleCell cell = wordGroup.GetChild (index).GetComponent<PuzzleCell> ();
                letters.Add (cell.letter.text);
                hintInfo.Add (hintGroup.GetChild(index).GetComponent<InputField>().text);
                //loadData相关

                loadColor.Add (cell.blockIndex);
                //end

                if (cell.blockIndex!=-1 && !string.IsNullOrEmpty(cell.letter.text)) {
                    info.pos = cell.pos;
                    info.letter = cell.letter.text;
                    if (wordPosDic.ContainsKey (cell.blockIndex)) {
                        list = wordPosDic[cell.blockIndex];
                        list.Add (info);
                        wordPosDic[cell.blockIndex] = list;
                    } else {
                        list.Add (info);
                        wordPosDic.Add (cell.blockIndex,list);
                    }
                }
                index++;
            }
            data.wordInfo.Add (letters);
            data.hintInfo.Add (hintInfo);
            loadData.wordInfo.Add (letters);
            loadData.hintInfo.Add (hintInfo);
            loadData.wordColorIndex.Add (loadColor);
        }

    }

    private void CalculateBlock() {
        int index = 0;
        for (int i = 0; i < 10; i++) {
            List<string> block_word = new List<string> ();
            List<int> block_color = new List<int> ();
            for (int j = 0; j < 10; j++) {
                List<CellInfo> list = new List<CellInfo> ();
                CellInfo info = new CellInfo ();
                BlockLetter cell = blockGroup.GetChild (index).GetComponent<BlockLetter> ();
                block_word.Add (cell.letter.text);
                block_color.Add (cell.blockIndex);
                if (cell.blockIndex != -1 && !string.IsNullOrEmpty (cell.letter.text)) {
                    info.pos = cell.pos;
                    info.letter = cell.letter.text;
                    if (blockPosDic.ContainsKey (cell.blockIndex)) {
                        list = blockPosDic[cell.blockIndex];
                        list.Add (info);
                        blockPosDic[cell.blockIndex] = list;
                    } else {
                        list.Add (info);
                        blockPosDic.Add (cell.blockIndex, list);
                    }
                }
                index++;
            }
            loadData.blockInfo.Add (block_word);
            loadData.blockColorIndex.Add (block_color);
        }
    }

    private void CalculateCenter() {
        //两个字典的数量一定是相等的
        List<BlockInfo> blockInfos = new List<BlockInfo> ();
       foreach(int key in wordPosDic.Keys) {
            BlockInfo info = new BlockInfo ();
            info.blockNum = key;
            
            List<CellInfo> word = new List<CellInfo> ();
            List<CellInfo> block= new List<CellInfo> ();
            word = wordPosDic[key];
            block = blockPosDic[key];
            info.wordPos = word;
            info.blockPos = block;
            info.wordRect = CalculateRectangle (word, wordGroup);
            info.blockRect = CalculateRectangle (block,blockGroup);
            blockInfos.Add (info);
        }
        data.puzzleInfo = blockInfos;
    }


    // 找到两个角，是左上角和右下角顶点
    private Rectangle CalculateRectangle(List<CellInfo> word,Transform group) {
        Rectangle rect = new Rectangle ();
        int minX = 10000;
        int minY = 10000;
        int maxX = - 100000;
        int maxY = -10000;
        for(int i = 0; i < word.Count; i++) {
            if (minX > word[i].pos.x) {
                minX = word[i].pos.x;
            }
            if (minY > word[i].pos.y) {
                minY = word[i].pos.y;
            }
            if (maxX < word[i].pos.x) {
                maxX = word[i].pos.x;
            }
            if (maxY < word[i].pos.y) {
                maxY = word[i].pos.y;
            }
        }
        // int min = minX * col + minY;
        rect.min.x = minX;
        rect.min.y = minY;
        rect.max.x = maxX;
        rect.max.y = maxY;

        return rect;
    }

    //提示
    private void HintShow(string text, bool fade = true) {
        hint.color = new Color (1, 0, 0, 1);
        hint.text = text;
        if (fade) {
            hint.DOKill ();
            hint.DOFade (0f, 1f).SetDelay (1f);
        }
    }

    //初始化颜色
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

    //读写操作
    public void WritePuzzleData(string path, PuzzleData data) {
        if (File.Exists (path)) {
            File.Delete (path);
        }
        FileStream file = File.Create (path);
        StreamWriter sw = new StreamWriter (file);
        string jsonData = JsonMapper.ToJson (data);
        sw.WriteLine (jsonData);
        sw.Close ();
    }

    public void WritePuzzleLoadData(string path, PuzzleLoadData data) {
        if (File.Exists (path)) {
            File.Delete (path);
        }
        FileStream file = File.Create (path);
        StreamWriter sw = new StreamWriter (file);
        string jsonData = JsonMapper.ToJson (data);
        sw.WriteLine (jsonData);
        sw.Close ();
    }

    public PuzzleLoadData ReadPuzzleData(string path) {
        if (File.Exists (path)) {

            FileStream file = new FileStream (path, FileMode.Open);
            StreamReader sr = new StreamReader (file);
            string strLine = sr.ReadLine ();
            return JsonMapper.ToObject<PuzzleLoadData> (strLine);
        }
        return null;
    }

}
public class PuzzleLoadData {
    public List<List<string>> wordInfo = new List<List<string>> ();//单词在格子里面的信息
    public List<List<int>> wordColorIndex = new List<List<int>> ();//每个方块的颜色值

    public List<List<string>> blockInfo = new List<List<string>> ();//单词在格子里面的信息
    public List<List<int>> blockColorIndex = new List<List<int>> ();//每个方块的颜色值

    public List<List<string>> hintInfo = new List<List<string>> ();
    public List< string> tipsRow = new List<string>();
    public List<string> tipsCol= new List<string> ();

}

public class PuzzleData {
    public List<string> tipsRow = new List<string> ();
    public List<string> tipsCol = new List<string> ();
    public List<List<string>> hintInfo = new List<List<string>> ();
    public List<List<string>> wordInfo = new List<List<string>> ();//单词在格子里面的信息
    public List<BlockInfo> puzzleInfo = new List<BlockInfo> ();
}

//每个方块里面包含的信息
public class BlockInfo {
    public int blockNum;//第几个小方块
    public List<CellInfo> wordPos = new List<CellInfo> ();//单词小块的位置
    public List<CellInfo> blockPos = new List<CellInfo> ();//方块小块的位置
    public  Rectangle wordRect = new Rectangle ();//方便以后计算距离中心
    public Rectangle blockRect  = new Rectangle ();
}

public class CellInfo {
    public Position pos = new Position ();
    public string letter;
}

public class Rectangle {
    public Position min = new Position ();
    public Position max = new Position ();
}

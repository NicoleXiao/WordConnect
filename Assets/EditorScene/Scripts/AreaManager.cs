using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.IO;
using LitJson;
public enum WordDirection {
    Right,
    Down
}
public enum EditorType {
    None,
    Crossy,
    Search
}
public class AreaManager : MonoBehaviour {

    public RectTransform editorItem;
    public InputField levelField;
    public InputField loadId;
    public InputField wordField;
    public Text hint;
    public GameObject loadGroup;
    public GameObject newGroup;
    public GameObject opBtn;
    public GameObject areaGroup;
    public GameObject letterGroup;
    public GameObject wordGroup;
    public Button nextLevel;
    public Text typeText;
    [HideInInspector]
    public EditorItem currentItem;
    [HideInInspector]
    public string letter;//字母
    private CrossyData data;
    private SearchData searchData;
    private string[,] wordPos;

    private int row = 0;//行
    private int col = 0;//列
    private string level = "";//关卡id

    private float areaWidth;
    private float areaHeight;// Use this for initialization
    private float currentWidth;
    private float currentHeight;

    private bool isStart = false;

    private string path = "";
    private string wordPosPath = "";
    private EditorType type = EditorType.None;
    public static AreaManager instance;
    float min = 0;
    private void Awake() {
        instance = this;
    }
    void Start() {
        nextLevel.onClick.AddListener (() => {
            EditorManager.instance.Crossy ();
            Destroy (this.gameObject);
        });
        areaHeight = areaGroup.GetComponent<RectTransform> ().sizeDelta.y;
        areaWidth = areaGroup.GetComponent<RectTransform> ().sizeDelta.x;
    }
    public void StartLoadOld() {
        if (string.IsNullOrEmpty (loadId.text)) {
            HintShow ("请输入加载关卡的ID ");
            return;
        }
        if (type == EditorType.Crossy && !File.Exists (path + loadId.text + ".json")) {
            HintShow ("CrossyData不存在ID为 ：" + loadId.text + "的关卡,请重新输入关卡");
            return;
        } else if (type != EditorType.Search) {
            LoadCrossyInfo ();
        } else if (type == EditorType.Search && !File.Exists (path + loadId.text + ".json")) {
            HintShow ("Search不存在ID为 ：" + loadId.text + "的关卡,请重新输入关卡");
            return;
        } else {
            LoadSearchInfo ();

        }
        levelField.text = loadId.text;
        level = loadId.text;
        Begin ();
        Calculate ();
        LoadItem ();
        opBtn.SetActive (true);
    }

    public void SetCrossyType() {
        type = EditorType.Crossy;
        typeText.text = string.Format (typeText.text, "Crossy");
        SetPath ();

    }
    public void SetSearchType() {
        type = EditorType.Search;
        typeText.text = string.Format (typeText.text, "Search");
        SetPath ();
    }
    public void NewLevel() {
        loadGroup.SetActive (false);
        newGroup.SetActive (false);
        StartSet ();

    }
    public void StartSet() {
        if (type == EditorType.Search) {
            row = 14;
            col = 14;
        } else {
            row = 3;
            col = 3;
        }
        Begin ();
        Calculate ();
        SetItem ();

    }
    void Begin() {
        letterGroup.SetActive (true);
        isStart = true;
        areaGroup.SetActive (true);
        opBtn.SetActive (true);
        loadGroup.SetActive (false);
        newGroup.SetActive (false);
        hint.text = "";

    }
    void SetPath() {
        if (type == EditorType.Crossy) {

            path = Application.streamingAssetsPath + "/DailyData/CrossyData/crossy_";
            wordPosPath = Application.dataPath + "/WordChef/WordPos/crossypos_";
        } else {
            path = Application.streamingAssetsPath + "/DailyData/SearchData/search_";
        }
    }


    void SetItem() {

        for (int i = 0; i < row; i++) {
            for (int j = 0; j < col; j++) {
                if (!(i == 0 && j == 0)) {
                    RectTransform item = Instantiate (editorItem, editorItem.transform.parent);
                    item.anchoredPosition = new Vector2 (editorItem.anchoredPosition.x + j * min, editorItem.anchoredPosition.y - i * min);
                    item.name = editorItem.name + i.ToString ();
                }
            }
        }
    }
    void LoadItem() {
        for (int i = 0; i < row; i++) {
            for (int j = 0; j < col; j++) {
                if (!(i == 0 && j == 0)) {
                    RectTransform item = Instantiate (editorItem, editorItem.transform.parent);
                    item.anchoredPosition = new Vector2 (editorItem.anchoredPosition.x + j * min, editorItem.anchoredPosition.y - i * min);
                    item.name = editorItem.name + i.ToString ();
                    item.GetComponent<EditorItem> ().SetText ("");
                    item.GetComponent<EditorItem> ().SetText (wordPos[i, j],true);
                    item.GetComponent<EditorItem> ().input.GetComponent<RectTransform> ().sizeDelta = new Vector2 (min, min);
                } else {
                    editorItem.GetComponent<EditorItem> ().SetText (wordPos[i, j],true);
                }
            }
        }
    }

    #region 四个按钮的操作
    public void AddRow() {
        row++;
        if (row > 15) {
            return;
        }
        Calculate ();
        int index = 0;
        for (int i = 0; i < row; i++) {
            for (int j = 0; j < col; j++) {
                if (index >= areaGroup.transform.childCount) {
                    RectTransform item = Instantiate (editorItem, editorItem.transform.parent);
                    item.GetComponent<EditorItem> ().SetText ("");
                    item.anchoredPosition = new Vector2 (editorItem.anchoredPosition.x + j * min, editorItem.anchoredPosition.y - i * min);
                    item.GetComponent<EditorItem> ().input.GetComponent<RectTransform> ().sizeDelta = new Vector2 (min, min);
                } else {
                    RectTransform rectTrans = areaGroup.transform.GetChild (index).GetComponent<RectTransform> ();
                    rectTrans.sizeDelta = new Vector2 (min, min);
                    rectTrans.anchoredPosition = new Vector2 (editorItem.anchoredPosition.x + j * min, editorItem.anchoredPosition.y - i * min);
                    areaGroup.transform.GetChild (index).GetComponent<EditorItem> ().input.GetComponent<RectTransform> ().sizeDelta = new Vector2 (min, min);
                }
                index++;
            }
        }
    }
    public void DeleteRow() {
        int currentRow = row - 1;
        if (currentRow < 3) {
            return;
        }
        Calculate ();
        int index = 0;
        for (int i = 0; i < row; i++) {
            for (int j = 0; j < col; j++) {
                if (i >= currentRow) {
                    Destroy (areaGroup.transform.GetChild (index).gameObject);
                } else {
                    RectTransform rectTrans = areaGroup.transform.GetChild (index).GetComponent<RectTransform> ();
                    rectTrans.sizeDelta = new Vector2 (min, min);
                    areaGroup.transform.GetChild (index).GetComponent<EditorItem> ().input.GetComponent<RectTransform> ().sizeDelta = new Vector2 (min, min);
                }
                index++;
            }
        }
        row--;
    }
    public void DeleteCol() {
        int currentCol = col - 1;
        if (currentCol < 3) {
            return;
        }
        Calculate ();
        int index = 0;
        for (int i = 0; i < row; i++) {
            for (int j = 0; j < col; j++) {
                if (j >= currentCol) {
                    Destroy (areaGroup.transform.GetChild (index).gameObject);
                } else {
                    RectTransform rectTrans = areaGroup.transform.GetChild (index).GetComponent<RectTransform> ();
                    rectTrans.sizeDelta = new Vector2 (min, min);
                    areaGroup.transform.GetChild (index).GetComponent<EditorItem> ().input.GetComponent<RectTransform> ().sizeDelta = new Vector2 (min, min);
                }
                index++;
            }
        }
        col--;
    }
    public void AddCol() {
        int preCol = col;
        col++;
        if (col > 15)
            return;
        Calculate ();
        int index = 0;
        for (int i = 0; i < row; i++) {
            for (int j = 0; j < col; j++) {
                if (j >= preCol) {
                    RectTransform item = Instantiate (editorItem, editorItem.transform.parent);
                    item.GetComponent<EditorItem> ().SetText ("");
                    item.anchoredPosition = new Vector2 (editorItem.anchoredPosition.x + j * min, editorItem.anchoredPosition.y - i * min);
                    item.transform.SetSiblingIndex (index);
                    item.GetComponent<EditorItem> ().input.GetComponent<RectTransform> ().sizeDelta = new Vector2 (min, min);
                } else {
                    RectTransform rectTrans = areaGroup.transform.GetChild (index).GetComponent<RectTransform> ();
                    rectTrans.sizeDelta = new Vector2 (min, min);
                    rectTrans.anchoredPosition = new Vector2 (editorItem.anchoredPosition.x + j * min, editorItem.anchoredPosition.y - i * min);
                    areaGroup.transform.GetChild (index).GetComponent<EditorItem> ().input.GetComponent<RectTransform> ().sizeDelta = new Vector2 (min, min);
                }
                index++;
            }
        }
    }
    #endregion

    void Calculate() {
        currentWidth = areaWidth / (float)col;
        currentHeight = areaHeight / (float)row;
        min = currentWidth < currentHeight ? currentWidth : currentHeight;
        editorItem.sizeDelta = new Vector2 (min, min);
        editorItem.GetComponent<EditorItem> ().input.GetComponent<RectTransform> ().sizeDelta = new Vector2 (min, min);
    }
    public void Save() {
        if (string.IsNullOrEmpty (levelField.text)) {
            HintShow ("请填入关卡ID ！！  ");
            return;
        }
        level = levelField.text;
        if (string.IsNullOrEmpty (wordField.text)) {
            HintShow ("请填入Word ！ ");
            return;
        }
        wordPos = new string[row, col];
        int index = 0;
        for (int i = 0; i < row; i++) {
            for (int j = 0; j < col; j++) {
                string info = areaGroup.transform.GetChild (index).GetComponent<EditorItem> ().input.text;
                wordPos[i, j] = info;
                index++;
            }
        }
        if (type == EditorType.Crossy) {
            SaveCrossy ();
        }
        if (type == EditorType.Search) {
            SaveSearch ();
        }
        nextLevel.gameObject.SetActive (true);
        HintShow ("保存成功 ！");
    }
    #region SearchData
    void LoadSearchInfo() {
        searchData = ReadSearchData (path + loadId.text + ".json");
        for (int i = 0; i < searchData.answer.Count; i++) {
            wordField.text += searchData.answer[i];
            if (i != searchData.answer.Count - 1) {
                wordField.text += ",";
            }
        }
        row = searchData.wordPositon.Count;
        col = searchData.wordPositon[0].Count;
        wordPos = new string[row, col];
        for (int j = 0; j < searchData.wordPositon.Count; j++) {
            List<string> wordCol = searchData.wordPositon[j];
            for (int k = 0; k < wordCol.Count; k++) {
                wordPos[j, k] = wordCol[k];
            }
        }

    }
    void SaveSearch() {
        searchData = new SearchData ();
        for (int i = 0; i < wordPos.GetLength (0); i++) {
            List<string> wordRow = new List<string> ();
            for (int j = 0; j < wordPos.GetLength (1); j++) {
                wordRow.Add (wordPos[i, j]);
            }
            searchData.wordPositon.Add (wordRow);
        }
        string[] answerGroup = wordField.text.Split (',');
        for (int k = 0; k < answerGroup.Length; k++) {
            if (!string.IsNullOrEmpty (answerGroup[k])) {
                searchData.answer.Add (answerGroup[k]);
            }
        }
        WriteSearchData (path + level + ".json", searchData);
    }
    #endregion

    #region AboutCrossyData
    void LoadCrossyInfo() {
        data = ReadCrossyData (path + loadId.text + ".json");
        wordPos = ReadLineData (wordPosPath + loadId.text + ".txt");
        wordField.text = data.word;
        row = data.row;
        col = data.col;
    }
    void SaveCrossy() {
        isStart = false;
        data = new CrossyData ();
        data.row = row;
        data.col = col;
        data.word = wordField.text;
        SetWordInfo ();

        WriteCrossyData (path + level + ".json", data);
        WritePos (wordPosPath + level + ".txt", wordPos);
    }
    void SetWordInfo() {
        for (int i = 0; i < wordPos.GetLength (0); i++) {
            for (int j = 0; j < wordPos.GetLength (1); j++) {
                if (!string.IsNullOrEmpty (wordPos[i, j])) {
                    
                    if (wordPos[i, j].Length > 1) {
                        for (int k = 1; k < wordPos[i, j].Length; k++) {
                            if (wordPos[i, j][k].ToString () == "1") {//向右有单词
                                WordInfo info = new WordInfo {
                                    pos = new Position ()
                                };
                                info.pos.x = i;
                                info.pos.y = j;
                                info.direction = WordDirection.Right;
                                Right (i, j, info);
                                data.wordInfo.Add (info);
                            } else if (wordPos[i, j][k].ToString () == "2") {
                                WordInfo info = new WordInfo {
                                    pos = new Position ()
                                };
                                info.pos.x = i;
                                info.pos.y = j;
                                info.direction = WordDirection.Down;
                                Down (i, j, info);
                                data.wordInfo.Add (info);
                            } else if (wordPos[i, j][k].ToString () == "0") {
                                WordInfo info = new WordInfo {
                                    pos = new Position ()
                                };
                                info.pos.x = i;
                                info.pos.y = j;
                                if (!data.gemsPos.Contains (info.pos)) {
                                    data.gemsPos.Add (info.pos);
                                }
                            }

                        }
                    }

                }
            }
        }
    }
    void Right(int i, int j, WordInfo info) {
        info.validWord = "";
        for (int k = j; k < wordPos.GetLength (1); k++) {
            if (!string.IsNullOrEmpty (wordPos[i, k])) {
                string text = wordPos[i, k].Substring (0, 1);
                info.validWord += text;
            } else {
                break;
            }
        }

    }
    void Down(int i, int j, WordInfo info) {
        info.validWord = "";
        for (int k = i; k < wordPos.GetLength (0); k++) {
            if (!string.IsNullOrEmpty (wordPos[k, j])) {
                string text = wordPos[k, j].Substring(0,1);
                info.validWord += text;
            } else {
                break;
            }
        }

    }
    #endregion

    void HintShow(string text, bool fade = true) {
        hint.color = new Color (1, 0, 0, 1);
        hint.text = text;
        if (fade) {
            hint.DOKill ();
            hint.DOFade (0f, 1f).SetDelay (1f);
        }
    }
    private void Update() {
        if (Input.GetMouseButtonDown (0) && isStart) {
            if (currentItem != null) {
                currentItem.SetText (letter);
            }
        }
        if (Input.GetMouseButtonDown (1)) {
            if (currentItem != null) {
                currentItem.SetText ("");
            }
        }
        if (Input.GetKeyDown (KeyCode.Escape)) {

            EditorManager.instance.Show ();
            Destroy (this.gameObject);

        }
    }
    //Search
    public void WriteSearchData(string path, SearchData data) {
        if (File.Exists (path)) {
            File.Delete (path);
        }
        FileStream file = File.Create (path);
        StreamWriter sw = new StreamWriter (file);
        string jsonData = JsonMapper.ToJson (data);
        sw.WriteLine (jsonData);
        sw.Close ();
    }
    public SearchData ReadSearchData(string path) {
        if (File.Exists (path)) {
            FileStream file = new FileStream (path, FileMode.Open);
            StreamReader sr = new StreamReader (file);
            string strLine = sr.ReadLine ();
            return JsonMapper.ToObject<SearchData> (strLine);
        }
        return null;
    }

    //Crossy
    public void WriteCrossyData(string path, CrossyData data) {
        if (File.Exists (path)) {
            File.Delete (path);
        }
        FileStream file = File.Create (path);
        StreamWriter sw = new StreamWriter (file);
        string jsonData = JsonMapper.ToJson (data);
        sw.WriteLine (jsonData);
        sw.Close ();
    }
    public CrossyData ReadCrossyData(string path) {
        if (File.Exists (path)) {

            FileStream file = new FileStream (path, FileMode.Open);
            StreamReader sr = new StreamReader (file);
            string strLine = sr.ReadLine ();
            return JsonMapper.ToObject<CrossyData> (strLine);
        }
        return null;
    }
    public string[,] ReadLineData(string path) {

        List<List<string>> data = new List<List<string>> ();
        if (File.Exists (path)) {
            FileStream file = new FileStream (path, FileMode.Open);
            StreamReader sr = new StreamReader (file);
            string strLine = null;
            while ((strLine = sr.ReadLine ()) != null) {
                string[] info = strLine.Split (',');
                List<string> list = new List<string> ();
                for (int i = 0; i < info.Length; i++) {
                    list.Add (info[i]);
                }

                data.Add (list);
            }
        }
        string[,] wordPos = new string[data.Count, data[0].Count];
        for (int i = 0; i < data.Count; i++) {
            for (int j = 0; j < data[i].Count; j++) {
                wordPos[i, j] = data[i][j];
            }
        }
        return wordPos;
    }
    public void WritePos(string path, string[,] data) {
        if (File.Exists (path)) {
            File.Delete (path);
        }
        FileStream file = File.Create (path);
        StreamWriter sw = new StreamWriter (file);
        for (int i = 0; i < data.GetLength (0); i++) {
            for (int j = 0; j < data.GetLength (1); j++) {
                sw.Write (data[i, j]);
                if (j == data.GetLength (1) - 1) {
                    sw.Write ("\r\n");

                } else {
                    sw.Write (",");
                }

            }
        }
        sw.Close ();

    }
}

public class CrossyData {
    public int row;
    public int col;
    public string word = "";//单词
    public List<WordInfo> wordInfo = new List<WordInfo> ();
    public List<Position> gemsPos = new List<Position> ();//钻石的位置
}
public class WordInfo {
    public string validWord;//单词
    public Position pos;//第一个单词占的位置
    public WordDirection direction;//方向
}
public class Position {
    public int x;
    public int y;
}
public class SearchData {
    public List<List<string>> wordPositon = new List<List<string>> ();
    public List<string> answer = new List<string> ();
}


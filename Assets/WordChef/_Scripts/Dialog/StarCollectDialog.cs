using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Superpow;

public class StarCollectDialog : Dialog {
    public Transform selectBg;
    public Transform rubyTrans;
    public RectTransform content;
    public ScrollRect rect;
    public CollectStar star;
    public Text starName;
    public Text des;
    public List<Sprite> bg = new List<Sprite> ();
    private float groupoffset = 200;
    private float contentOffset = 0;
    private float itemWidth = 380;//加了一个20的间隔
    private int posIndex = 0;//当前关卡的位置下标                    
    public static StarCollectDialog _instance;
    protected override void Awake() {
        base.Awake ();
        WorldGroupManager._instance.gameObject.SetActive (false);
        _instance = this;
        contentOffset = content.anchoredPosition.x;
        content.sizeDelta = new Vector2 (GameState.chapterCount * itemWidth + 100, content.sizeDelta.y);
    }

    protected override void Start() {
        base.Start ();
        ShowMap ();
        rect.onValueChanged.AddListener (OnValueChangedEvent);
    }
    public override void Close() {
        base.Close ();
        WorldGroupManager._instance.gameObject.SetActive (true);
    }
    void OnValueChangedEvent(Vector2 value) {
        CalculateContentAndAddChild ();

    }
    /// <summary>
    /// 重新计算两个的位置.
    /// curretnPosIndex 越大代表关卡越大
    /// </summary>
    void CalculateContentAndAddChild() {
        int currentPosIndex = Mathf.Abs ((int)(content.anchoredPosition.x - contentOffset) / (int)itemWidth);
        if (currentPosIndex < posIndex || currentPosIndex > posIndex) {
            posIndex = currentPosIndex;
            ShowItem ();
        }
    }

    /// <summary>
    /// 不用再计算了，因为level关卡里面已经计算好了
    /// </summary>
    void ShowMap() {
        //全部克隆
        for (int i = 0; i < GameState.chapterCount; i++) {

            CloneGrah (i, i <= Prefs.unlockedWorld);
        }
        SetContentPos ();
        posIndex = Prefs.unlockedWorld;
        ShowItem ();
    }
    void ShowItem() {

        for (int i = 0; i < content.childCount; i++) {
            if (posIndex == 0 && i <= 4 && posIndex + 4 < content.childCount) {
                content.GetChild (i).gameObject.SetActive (true);
            } else if (posIndex == content.childCount - 1 && (i > posIndex - 4)) {
                content.GetChild (i).gameObject.SetActive (true);
            } else if (i == posIndex - 1 || (i <= posIndex + 2 && i > posIndex) || i == posIndex) {
                content.GetChild (i).gameObject.SetActive (true);
            } else {
                content.GetChild (i).gameObject.SetActive (false);
            }
        }
    }


    /// <summary>
    /// pos位置坐标
    /// index只的是克隆的第几个物体
    /// newLevel判断是否是正在解锁的关卡
    /// </summary>
    /// <param name="index"></param>
    /// <param name="newLevel"></param>
    /// <param name="up"></param>
    void CloneGrah(int pos, bool pass, bool up = false) {
        CollectStar clone = Instantiate (star, content);
        clone.name = "Star" + pos.ToString ();
        clone.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (groupoffset + pos * itemWidth, 0);
        clone.SetTotalCount (pos, bg[pos % bg.Count], pass, Utils.StarName (pos % bg.Count));
        if (pos == Prefs.unlockedWorld) {
            clone.SetCollectStarButtonState ();
        }
        if (pos < Prefs.unlockedWorld) {
            clone.graph.color = new Color (1, 1, 1, 1);
        }
        if (up) {
            clone.transform.SetAsFirstSibling ();
        }
    }


    public void SetContentPos() {
        if (Prefs.unlockedWorld == GameState.chapterCount - 1) {
            content.anchoredPosition = new Vector2 (-Prefs.unlockedWorld * itemWidth - itemWidth / 2, content.anchoredPosition.y);
        } else if (Prefs.unlockedWorld == 0) {
            content.anchoredPosition = new Vector2 (-(Prefs.unlockedWorld + 1) * itemWidth, content.anchoredPosition.y);
        } else {//居中显示
            content.anchoredPosition = new Vector2 (-(Prefs.unlockedWorld + 1) * itemWidth + itemWidth / 2.5f - 10f, content.anchoredPosition.y);
        }

    }
    public void SetStarInfo(string name, int id) {
        starName.text = name;
        des.text = CSVReadManager.Instance.GetStarDes (id % bg.Count);
    }
    public int GetMapId(int id) {
        return id / bg.Count;
    }
}

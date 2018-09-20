using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class PictureController :BaseController{
    public HintManager hint;
    public Transform answerTrans;

    public List<Image> pictures = new List<Image> ();
    public List<PictureLetter> letterGroup = new List<PictureLetter> ();
    public List<PictureLetter> answerGroup = new List<PictureLetter> ();//总共的答案格子，没有用到的就隐藏
    private List<PictureLetter> select = new List<PictureLetter> ();//游戏里面要用到的答案格子
   
    private int randomCount = 0;
    private List<bool> selectShowing = new List<bool> ();
    private List<string> alreadyShowAnswer = new List<string> ();//已经在答案框显示的字母
    public List<int> answer_select = new List<int> ();

    private List<string> selectLetter = new List<string> ();
    private List<string> letter=new List<string> () { "A","B", "C" , "D" , "E" , "F" , "G" , "H" , "I" , "J",
    "K","L", "M", "N" , "O" , "P" , "Q" , "R", "S", "T","U","V","W","X","Y","Z"};//26个字母
    private List<PictureData> picData = new List<PictureData> ();
    private Dictionary<int, PictureLetter> answerDic = new Dictionary<int, PictureLetter> ();
    public static PictureController _instance;
    protected override void Awake() {
        base.Awake ();
        _instance = this;
        PictureSaveData saveData = TextManager.Instance.ReadProgress<PictureSaveData> ("PictureProgress.json");
        if (saveData != null) {
            selectLetter = saveData.select;
            selectShowing = saveData.selectShowing;
            alreadyShowAnswer = saveData.answer;
            answer_select = saveData.answer_select;
        }
        hint.info = "picturehintcost";
        hint.SetHint ();
        hint.HintEvent += ClickHint;
      
        
    }
    protected override void Start() {
        base.Start ();
        Init ();
    }
    void Init() {
        picData = CSVReadManager.Instance.pictureData;
        SetPicture (picData[GameState.pictureLevel - 1].pictureName);
        if (selectLetter.Count ==0) {
            SetAnswer (picData[GameState.pictureLevel - 1].word);
        } else {
            SetProgressAnswer (picData[GameState.pictureLevel - 1].word);
        }
        SetLetter ();
    }

    private void SetPicture(List<string> picName) {
        ResManager.Instance.LoadSpriteFromStreamingAsset ("Picture/" + picName[0] + ".png", delegate (Sprite sp) {
            pictures[0].sprite = sp;
            pictures[0].SetNativeSize ();
            pictures[0].enabled = true;
        }, new Vector2 (0.5f, 0.5f), false);
        ResManager.Instance.LoadSpriteFromStreamingAsset ("Picture/" + picName[1] + ".png", delegate (Sprite sp) {
            pictures[1].sprite = sp;
            pictures[1].SetNativeSize ();
            pictures[1].enabled = true;
        }, new Vector2 (0.5f, 0.5f), false);
        ResManager.Instance.LoadSpriteFromStreamingAsset ("Picture/" + picName[2] + ".png", delegate (Sprite sp) {
            pictures[2].sprite = sp;
            pictures[2].SetNativeSize ();
            pictures[2].enabled = true;
        }, new Vector2 (0.5f, 0.5f), false);
        ResManager.Instance.LoadSpriteFromStreamingAsset ("Picture/" + picName[3] + ".png", delegate (Sprite sp) {
            pictures[3].sprite = sp;
            pictures[3].SetNativeSize ();
            pictures[3].enabled = true;
        }, new Vector2 (0.5f, 0.5f), false);
    }

    private void SetProgressAnswer(string word) {
        int length = word.Trim ().Length;
        for (int i = 0; i < answerGroup.Count; i++) {
            if (i < length) {
                char gameChar = char.ToUpper (word[i]);
                answerGroup[i].Hide ();
                answerGroup[i].SetText (gameChar.ToString (), true);
                if (!string.IsNullOrEmpty (alreadyShowAnswer[i])) {
                    
                    answerGroup[i].letter.text = alreadyShowAnswer[i];
                    answerGroup[i].Show ();
                }
                select.Add (answerGroup[i]);
                answerDic.Add (i, letterGroup[answer_select[i]]);
             
            } else {
                answerGroup[i].gameObject.SetActive (false);
            }
        }
        Vector3 pos = answerTrans.localPosition;
        answerTrans.localPosition = new Vector3 (pos.x + (answerGroup.Count - length) * 49, pos.y, pos.z);
    }

    private void SetAnswer(string word) {
        int length = word.Trim ().Length;
        randomCount = letterGroup.Count - length;
        for (int i = 0; i < answerGroup.Count; i++) {
            if (i < length) {
                char gameChar = char.ToUpper (word[i]);
                answerGroup[i].Hide ();
                answerGroup[i].SetText (gameChar.ToString(),true);
             //   answerLetter.Add (gameChar.ToString());
                //字典初始化
                selectLetter.Add (gameChar.ToString ());
                select.Add (answerGroup[i]);
                answerDic.Add (i, letterGroup[i]);
                if (letter.Contains (gameChar.ToString ())) {
                    letter.Remove (gameChar.ToString ());
                }
            } else {
                answerGroup[i].gameObject.SetActive (false);
            }
        }
        Vector3 pos =answerTrans.localPosition;
        answerTrans.localPosition = new Vector3 (pos.x+(answerGroup.Count- length)*49,pos.y,pos.z);
        
    }

    private void SetLetter() {
        if (selectShowing.Count != 0) {
            for(int j = 0; j < selectLetter.Count;j++) {
                letterGroup[j].SetText (selectLetter[j],false);
                if (selectShowing[j] == true) {
                    letterGroup[j].Show ();
                } else {
                    letterGroup[j].Hide();
                }
            }
            return;
        }
        int count = 0;
        while (count <randomCount) {
            int random = Random.Range (0,letter.Count);
            selectLetter.Add (letter[random]);
            letter.RemoveAt (random);
            count++;
        }
        int i = 0;
        while (selectLetter.Count >0) {
            int random = Random.Range (0, selectLetter.Count);
            letterGroup[i].SetText (selectLetter[random],false);
            selectLetter.RemoveAt (random);
            i++;
        }
    }

    private void OnDisable() {
        CSVReadManager.Instance.ReadCompelte -= Init;
    }

    public void SetAnswerCellText(string text,PictureLetter letter) {
        for (int i = 0; i < select.Count; i++) {
            if (select[i].isShowing == false) {
                select[i].Back (text);
                answerDic[i] = letter;
                letter.Hide ();
                break;
            }
        }

        CheckGameComplete ();
    }

    public void CheckGameComplete() {
        SaveProcess ();
        PictureLetter pic = select.Find (x => x.isShowing == false);
        if (pic == null) {
            PictureLetter notRight = select.Find (x => x.rightLetter != x.letter.text);
            if (notRight == null) {
                hint.HintData ();
                for (int j = 0; j < select.Count; j++) {
                    select[j].ShowColorChange (new Color(115f / 255f,251f / 255f, 205f/255f),false);
                }
                //处理数据
                GameState.pictureLevel++;
                PlayerDataManager.Instance.playerData.pictureLevelID = GameState.pictureLevel;
                TextManager.Instance.DeleteFile ("PictureProgress.json");
                PlayerDataManager.Instance.playerData.isPictureFinish = true;
                PlayerDataManager.Instance.playerData.pictureCount++;
                PlayerDataManager.Instance.JudeReachAchieve (11, PlayerDataManager.Instance.playerData.pictureCount);
               
                Timer.Schedule (this, 2, () => {
                    DialogController.instance.ShowDialog (DialogType.DailyWin);
                });
            } else {
                for (int j = 0; j < select.Count; j++) {
                    select[j].ShowColorChange (Color.red);
                }
            }
        }
        
    }

    public void HideAnswer(int index) {
        select[index].Hide ();
        answerDic[index].Show ();
    }

    public void ClickHint(bool isFree) {
        PictureLetter pic = select.Find (x => x.isShowing == false);
        if (pic != null) {
            List<int> notShowIndex = new List<int> ();
            if (!isFree) {
                CurrencyController.DebitBalance (Const.HINT_COST);
            }
            for (int i = 0; i < select.Count; i++) {
                if (select[i].isShowing == false) {
                    notShowIndex.Add (i);
                }
            }
            //随机出现hint的提示
            int index = Random.Range (0,notShowIndex.Count);
            select[notShowIndex[index]].Show (true);
            PictureLetter picletter = letterGroup.Find (x => x.letter.text == select[notShowIndex[index]].rightLetter);
            if (picletter != null) {
                answerDic[notShowIndex[index]] = picletter;
                picletter.Hide ();
            }
            CheckGameComplete ();
        }
    }

    private void SaveProcess() {
        PictureSaveData data = new PictureSaveData ();
       for(int i = 0; i < answerGroup.Count; i++) {
            data.answer.Add (answerGroup[i].letter.text);
        }
       for(int j = 0; j < letterGroup.Count;j++) {
            data.select.Add (letterGroup[j].letter.text);
            if (letterGroup[j].isShowing) {
                data.selectShowing.Add (true);
            } else {
                data.selectShowing.Add (false);
            }
        }
        for (int k = 0; k < answerDic.Count; k++) {
            data.answer_select.Add (answerDic[k].transform.GetSiblingIndex ());
        }
        TextManager.Instance.SaveProgress<PictureSaveData> ("PictureProgress.json", data);
    }
   

}
public class PictureSaveData {
    public List<string> answer = new List<string> ();
    public List<string> select = new List<string> ();   //选择的12个单词
    public List<bool> selectShowing = new List<bool> ();//选择的12个单词显示情况
    public List<int> answer_select =new List<int>();//answer 和 select的对应关系，answer默认下标从0开始。这里记录的select的下标

}

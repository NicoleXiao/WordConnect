using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleLetterCell : MonoBehaviour {
    //  public Text letter;
    //  public Image nullBg;
    //  public Image colorBg;
    public Image bg;
    public Button click;
    public bool isShowing;
    public GameObject colBg;
    public GameObject rowBg;
    public GameObject clickBg;
    public Position pos=new Position();
    private void Awake() {
        click.onClick.AddListener (()=> {
            Sound.instance.PlayButton ();
            PuzzleController.instance.ShowHint (this);
        });
    }
    /// <summary>
    /// 0 代表行 1代表列 2 代表当前点击的
    /// </summary>
    /// <param name="index"></param>
    public void SetHintBg(int index) {
        if (index==0) {
            rowBg.SetActive (true);
            colBg.SetActive (false);
            clickBg.SetActive (false);
        } else if(index==1) {
            colBg.SetActive (true);
            clickBg.SetActive (false);
            rowBg.SetActive (false);
        } else {
            colBg.SetActive (false);
            clickBg.SetActive (true);
            rowBg.SetActive (false);
        }
    }
    public void ClearHintBg() {
        colBg.SetActive (false);
        clickBg.SetActive (false);
        rowBg.SetActive (false);
    }
    public void SetText(string text,Color bgColor) {
       // letter.text = text;
        bg.color = bgColor;
    }
    
    public void SetSize(float min) {
        bg.GetComponent<RectTransform> ().sizeDelta = new Vector2 (min, min);
        min = min + 8;
        rowBg.GetComponent<RectTransform> ().sizeDelta = new Vector2 (min, min);
        colBg.GetComponent<RectTransform> ().sizeDelta = new Vector2 (min, min);
        clickBg.GetComponent<RectTransform> ().sizeDelta = new Vector2 (min, min);

        //  letter.GetComponent<RectTransform> ().sizeDelta = new Vector2 (min, min);
        // colorBg.GetComponent<RectTransform> ().sizeDelta = new Vector2 (min, min);
    }
    //策划要求直接隐藏，功能先屏蔽
    //public void NotNull() {
    //    bg.gameObject.SetActive (true);
    //    nullBg.gameObject.SetActive (false);
    //    isShowing = false;
    //    letter.text = "";
    //}
    //public void SetNull() {
    //    bg.gameObject.SetActive (false);
    //    nullBg.gameObject.SetActive (true);
    //    isShowing = true;
    //    letter.text = "";
    //}
   
    
}

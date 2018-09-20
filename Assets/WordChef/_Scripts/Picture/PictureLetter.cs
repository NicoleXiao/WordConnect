using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PictureLetter : MonoBehaviour {
    public Text letter;
    public GameObject bg;
    public bool isShowing=true;
    public string rightLetter;//正确答案
    private bool isAnswer;
    private Button click;
    private void Awake() {
        click = bg.GetComponent<Button> ();
        click.onClick.AddListener (()=> {
           
            if (isAnswer) {
                Hide ();
                PictureController._instance.HideAnswer (transform.GetSiblingIndex());
            } else {
                PictureController._instance.SetAnswerCellText (letter.text,this);
            }
        });
    }
    public void SetText(string text,bool isAnswer) {
        if (isAnswer) {
            rightLetter = text;
        } else {
            letter.text = text;
        }
        this.isAnswer = isAnswer;
        
    }
    public void Back(string text) {
        letter.text = text;
        Show ();
    }
    public void Show(bool isShowRight=false) {
        if (isShowRight) {
            letter.text = rightLetter;
        }
        isShowing = true;
        letter.gameObject.SetActive (true);
        bg.SetActive (true);
    }
    public void Hide() {
        isShowing = false;
        letter.gameObject.SetActive (false);
        bg.SetActive (false);
    }
    public void ShowColorChange(Color color,bool enable=true) {
        click.enabled = false;
        if (isAnswer) {
            Sequence sq = DOTween.Sequence ();
            sq.Append (bg.GetComponent<Image>().DOColor (color, 0.8f))
                .Append (bg.GetComponent<Image> ().DOColor (Color.white, 1f)).SetDelay (0.2f).OnComplete(()=> {
                    click.enabled = enable;
                });


        }
    }
  
}

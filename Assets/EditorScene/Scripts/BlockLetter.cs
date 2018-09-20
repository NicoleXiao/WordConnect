using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockLetter : MonoBehaviour {

    public Text letter;
    public Image colorBg;
    [HideInInspector]
    public int blockIndex=-1;//在哪个形状的方块
    public Position pos = new Position ();//在父物体里面的位置
    void Start() {
        //blockIndex = -1;
        //colorBg.gameObject.SetActive (false);
    }
    public void SetText(string text) {
        letter.text = text;
    }
    public void SetSize(float min) {
        letter.GetComponent<RectTransform> ().sizeDelta = new Vector2 (min, min);
        colorBg.GetComponent<RectTransform> ().sizeDelta = new Vector2 (min, min);
    }
    public void SetColor(int index, Color color) {
        colorBg.gameObject.SetActive (true);
        colorBg.color = color;
        blockIndex = index;
    }
    public void ClearColor() {
        colorBg.color = Color.white;
        colorBg.gameObject.SetActive (false);
        blockIndex = -1;
        letter.text = "";

    }
    public void OnPointEnter() {
        //  Debug.Log ("Enter  : " + this.name);
        PuzzleAreaManager.instance.currentBlock = this;
    }
    public void OnPointUp() {
        //  Debug.Log ("Escape  : "+this.name);
        PuzzleAreaManager.instance.currentBlock = null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SearchCell : MonoBehaviour {
    public Text letterText;
    public Transform area;
    public Position pos = new Position ();
    public void OnPointEnter() {
        SearchController.instance.drawer.AddPos (this);
    }
    public void OnPointerDown() {
        SearchController.instance.drawer.AddPos (this, true);
    }
    public void SetText(string text, float scale, int x, int y) {
        pos.x = x;
        pos.y = y;
        letterText.text = text;
        area.localScale = new Vector3 (scale, scale, 1);
        letterText.transform.localScale = new Vector3 (scale, scale, 1);
    }
    public void ChangeBgColor(Color color) {
        if (color == Color.red) {
            this.GetComponent<Image> ().enabled = true;
        } else {
            this.GetComponent<Image> ().enabled =false;
        }
        this.GetComponent<Image> ().DOColor (color, 0.5f); ;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipItem : MonoBehaviour {
    public Outline outline;
    public Color color;
    public string tipIndex;
    public bool isRowTips;
    private Button button;
    private void Awake() {
        button = this.GetComponent<Button> ();
        button.onClick.AddListener (() => {
            Sound.instance.PlayButton ();
            Click ();
        });
      
    }
    public void ClickButtonOn() {
        outline.enabled = true;
        this.GetComponent<Text> ().color = color;
    }
    public void ClickButtonOff() {
        Exit ();
    }
    public void Click() {
        PuzzleController.instance.ShowCellTips (tipIndex);
    }
    public void Exit() {
        outline.enabled = false;
        this.GetComponent<Text> ().color = Color.white;
    }
}

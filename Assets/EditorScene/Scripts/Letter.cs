using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Letter : MonoBehaviour{
    private Toggle letter;
    public bool isPuzzle;
    public bool IsRight;
    public bool IsLeft;
	// Use this for initialization
	void Start () {
        letter = this.GetComponent<Toggle> ();
        if (letter.isOn) {
            if (isPuzzle) {
                PuzzleAreaManager.instance.letter = this.GetComponentInChildren<Text> ().text;
            } else {
                if (IsLeft) {
                    AreaManager.instance.letter = "2";
                } else if (IsRight) {
                    AreaManager.instance.letter = "1";
                } else {
                    AreaManager.instance.letter = this.GetComponentInChildren<Text> ().text;
                }
            }
        }
        letter.onValueChanged.AddListener ((value)=> {
            if (value) {
                if (isPuzzle) {
                    PuzzleAreaManager.instance.letter = this.GetComponentInChildren<Text> ().text;
                } else {
                    if (IsLeft) {
                        AreaManager.instance.letter = "2";
                    } else if (IsRight) {
                        AreaManager.instance.letter = "1";
                    } else {
                        AreaManager.instance.letter = this.GetComponentInChildren<Text> ().text;
                    }
                }
            }
        });
	}
	
	//// Update is called once per frame
	//void Update () {
 //       if (Input.GetMouseButtonDown (1)) {
 //           if (pointName== this.name) {
 //               letter.isOn = false;
 //           }
 //       }
	//}
 //   public void OnPointerEnter() {
 //       pointName = this.name;
 //   }
}

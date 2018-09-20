using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorItem : MonoBehaviour {
    public InputField input;
    private void Awake() {
        input.onValueChanged.AddListener ((string text)=> {
           // Debug.Log ("Input Text ：" + text);
            SetLetterColor (text);
        });
    }
    public void OnPointEnter() {
        AreaManager.instance.currentItem = this;
    }
    public void OnPointUp() {
        AreaManager.instance.currentItem = null;

    }
    public void SetText(string text,bool load=false) {
        if (string.IsNullOrEmpty (text) || load) {
            SetLetterColor (text);
            input.text = text;
            return;
        }
        if (input.text.Length >= 2 && input.text.Length < 4) {//只能是一个字母加0
            if (text == "2" && !input.text.Contains ("2")) {
                input.text += text;

            } else if (text == "1" && !input.text.Contains ("1")) {

                input.text += text;
            }
        } else if (input.text.Length == 1) {
            if (text == "0" || text == "1" || text == "2") {
                input.text += text;
            }
        } else {
            if (text != "0") {
                input.text = text;
            }
        }
            SetLetterColor (input.text);
    }
    public void SetLetterColor(string text) {
        if (string.IsNullOrEmpty (text) || text.Length==1) {
            input.textComponent.color = Color.white;
        }else if(text.Contains("0") && !text.Contains ("1") && !text.Contains ("2")) {
            input.textComponent.color = Color.red;
        } else if (text.Contains ("1") && !text.Contains ("2") && !text.Contains ("0")) {
            input.textComponent.color = Color.green;
        } else if (text.Contains ("2") && !text.Contains ("1") && !text.Contains ("0")) {
            input.textComponent.color = Color.yellow;
        } else if(text.Length>1){
            input.textComponent.color = Color.blue;
        }
    }


}

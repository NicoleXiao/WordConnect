using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchWord : MonoBehaviour {
    public Text word;
    public RectTransform done;
    public void SetTextAndDoneWidth(string text) {
        word.text = text.ToUpper();
        done.anchoredPosition = new Vector2 (word.GetComponent<RectTransform>().anchoredPosition.x, done.anchoredPosition.y);
        float width = word.preferredWidth;
        done.sizeDelta = new Vector2 (width,done.sizeDelta.y);
        
    }
    public void ClearDone() {
        done.GetComponent<Image> ().color = Color.white;
        done.gameObject.SetActive (false);
        word.color = Color.white;
    }
    public void Done(Color color) {
        done.GetComponent<Image> ().color = color;
        word.color = Color.gray;
        done.gameObject.SetActive (true);
    }
}

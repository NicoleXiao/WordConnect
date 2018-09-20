using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleColor : MonoBehaviour {
    public int blockNum;//根据颜色来确定方块的标序
    private Image bg;
    private Toggle toggle;
    private void Start() {
        
        toggle = this.GetComponent<Toggle> ();
        toggle.onValueChanged.AddListener ((bool value)=> {
            PuzzleAreaManager.instance.clickColor = true;
            PuzzleAreaManager.instance.currentColor = blockNum;
        });
    }

    public void SetColor(int index,Color color) {
        bg = this.GetComponent<Image> ();
        bg.color = color;
        blockNum = index;
    }
}

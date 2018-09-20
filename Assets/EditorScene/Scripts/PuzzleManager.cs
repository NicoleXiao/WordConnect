using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class PuzzleManager : MonoBehaviour {
    public InputField level;
    public PuzzleAreaManager manager;
    public Text hint;
    public GameObject load;
    public void ClickLoadNew() {
        PuzzleAreaManager obj = Instantiate (manager, this.transform);
        hint.text = "";
        load.SetActive (false);
        obj.LoadNew ();
    }
    public void ClickLoadOld() {
        if (string.IsNullOrEmpty (level.text)) {
            hint.text = "请输入Puzzle要加载的关卡ID";
        }else if(!File.Exists (Application.dataPath + "/WordChef/PuzzlePos/puzzlePos_" + level.text + ".json")) {
            hint.text = "Puzzle不存在id为"+ level.text+"的数据，请重新输入！";
        }else {
            PuzzleAreaManager obj = Instantiate (manager, this.transform);
            hint.text = "";
            load.SetActive (false);
            obj.LoadData (Application.dataPath + "/WordChef/PuzzlePos/puzzlePos_" + level.text + ".json",level.text);
        }
    }
}

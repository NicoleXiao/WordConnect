using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorManager : MonoBehaviour {
    public AreaManager area;
    public GameObject selectGroup;
    public static EditorManager instance;
    private void Awake() {
        instance = this;
    }
    public void Show() {
        selectGroup.SetActive (true);
    }
    public void Search() {
        selectGroup.SetActive (false);
        AreaManager obj = Instantiate (area, this.transform);
        obj.SetSearchType ();
    }
    public void Crossy() {
        selectGroup.SetActive (false);
        AreaManager obj = Instantiate (area, this.transform);
        obj.SetCrossyType ();
    }
}
//    #region load
//    //public void LoadOldLevel() {
//    //    loadGroup.SetActive (true);
//    //    selectGroup.SetActive (false);
//    //}
//    //public void StartLoadOld() {
//    //    if (string.IsNullOrEmpty (loadId.text)) {
//    //        HintShow("请输入加载关卡的ID ");
//    //        return;
//    //    }
//    //    string path = Application.streamingAssetsPath + "/LineWord/" + "LineWord_" + loadId.text + ".txt";
//    //    if (!File.Exists (path)) {
//    //        HintShow ("不存在ID为 ："+loadId.text+"的关卡,请重新输入关卡");
//    //        return;
//    //    }
//    //    loadGroup.SetActive (false);
//    //    opBtn.SetActive (true);
//    //    hint.text = "";
//    //    int id = 0;
//    //    int.TryParse (loadId.text, out id);
//    //    level = loadId.text;
//    //    LoadLevel (id);

//    //}
//    //public void LoadLevel(int id) {
//    //    List<List<string>> data = new List<List<string>> ();
//    //    data = TextManager.m_instance.ReadLineData (path + "crossy_" + id + ".json");
//    //    SetLoadItem (data);
//    //}
//    #endregion
//}

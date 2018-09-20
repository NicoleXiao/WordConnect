using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossyController : BaseController {
    private CrossyData data;
    public int levelId;
    public Pan pan;
    public CrossyWordManager manager;
    protected override void Awake() {
        base.Awake ();
        data = CSVReadManager.Instance.crossyData;
        if (data != null) {
            manager.SetItem (data);
            pan.Load (data.word);

        }
     }

 
}

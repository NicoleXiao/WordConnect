using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchController : BaseController {
    public SearchWordManager searchManager;
    public SearchLineDrawer drawer;
    public static SearchController instance;
    protected override void Awake() {
        base.Awake ();
        instance = this;
        SearchData data = new SearchData ();
        data = CSVReadManager.Instance.searchData;
        if (data != null) {
            searchManager.LoadWord (data);
        } 
    }

}

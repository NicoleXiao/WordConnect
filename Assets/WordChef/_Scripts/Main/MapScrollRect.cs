using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapScrollRect :ScrollRect {
    public override void OnBeginDrag(PointerEventData eventData) {
        base.OnBeginDrag (eventData);
       WorldGroupManager._instance.opeBtnGroup.SetActive (false);
    }
    public override void OnEndDrag(PointerEventData eventData) {
        base.OnEndDrag (eventData);
        WorldGroupManager._instance.opeBtnGroup.SetActive (true);
    }
}

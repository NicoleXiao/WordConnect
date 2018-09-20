using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PaintCheck : MonoBehaviour {
    private RenderTexturePainter painter;
    private PaintCompleteChecker checker;
    public bool canDraw;
    public Action OnPaintComplete;
    void Start () {
   
        painter = this.GetComponent<RenderTexturePainter> ();
        checker = this.GetComponent<PaintCompleteChecker> ();
        painter.isErase = true;
        checker.SetDataByTexture ((Texture2D)painter.sourceTexture,painter.penTex,painter.brushScale);
	}
	
	void Update () {
        if (canDraw) {
            if (Input.GetMouseButton (0)) {
                painter.Drawing (Input.mousePosition);
                checker.Drawing (Input.mousePosition);
                if (checker.Progress >= 0.2f) {
                    if (OnPaintComplete != null) {
                        canDraw = false;
                        OnPaintComplete ();
                    }
                }
            }
            if (Input.GetMouseButtonUp (0)) {
                painter.EndDraw ();
                checker.EndDraw ();
            }
        }
	}
}

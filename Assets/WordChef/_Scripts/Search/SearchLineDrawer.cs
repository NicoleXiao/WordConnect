using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SearchLineDrawer : MonoBehaviour {
    public SearchTextView view;
    public RectTransform line;
    [HideInInspector]
    public RectTransform cloneLine;
    [HideInInspector]
    public Vector2 begin = Vector2.one;
    [HideInInspector]
    public Vector2 end = Vector2.one;
    [HideInInspector]
    public SearchCell beginCell;
    [HideInInspector]
    public SearchCell endCell;
    private Color color;
    private bool isRight = false;//画线的角度是否正确
    private float width = 0;
    private bool isDraw = false;
    //private bool setBegin = false;

    public void AddPos(SearchCell cell,bool isDown=false) {
        if (isDraw|| isDown) {
            //只有开始滑动的时候才有按下事件
            if (isDown) {
                begin = cell.GetComponent<RectTransform> ().anchoredPosition;
                end = begin;
                beginCell = cell;
                endCell = beginCell;
               
               // Debug.Log ("鼠标落下了 --- --- ----- ----- ");
            } else {
                end = cell.GetComponent<RectTransform> ().anchoredPosition;
                endCell = cell;
            }
            if(begin!=Vector2.one && end!=Vector2.one && beginCell!=null && endCell != null) {
                AddLine ();
            }
           
        }

    }
    public void SetRightLine(Vector2 lineStart,Vector2 lineEnd,Color lineColor) {
        float x = lineEnd.x - lineStart.x;
        float y = lineEnd.y - lineStart.y;
        float angle = Mathf.Atan2 (y, x) * Mathf.Rad2Deg;
        float linewidth = width * 0.5f;
        RectTransform clone = Instantiate (line, line.transform.parent);
        Vector2 pos = new Vector2 (lineStart.x, lineStart.y);
        pos = SetPos (angle, pos, linewidth * 0.6f);
        clone.localEulerAngles = new Vector3 (0, 0, angle);
        float lineLength = Mathf.Sqrt (x * x + y * y) + width * 0.6f;
        clone.sizeDelta = new Vector2 (lineLength, linewidth);
        clone.anchoredPosition = pos;
        clone.GetComponent<Image> ().color = new Color(lineColor.r, lineColor.g, lineColor.b,0.5f);
        clone.gameObject.SetActive (true);
    }
	
	void Update () {
        if (Input.GetMouseButton (0)) {
            isDraw = true;
        }
        if (Input.GetMouseButtonUp (0)) {
            view.HideText ();
            isDraw = false;
            if (isRight) {
                isRight = false;
                if (!SearchController.instance.searchManager.CheckAnswer (beginCell, endCell)) {
                    if (cloneLine != null) {
                        Destroy (cloneLine.gameObject);
                    }
                }

            } else {
                if (cloneLine != null) {
                    Destroy (cloneLine.gameObject);
                }
            }
            begin = Vector2.one;
            end = Vector2.one;
            beginCell = null;
            endCell = null;
            cloneLine = null;
        }
	}
    void AddLine() {
        float x = end.x - begin.x;
        float y = end.y - begin.y;
        float angle = Mathf.Atan2 (y, x) * Mathf.Rad2Deg;
       // Debug.Log ("角度为 : "+ angle);
        float linewidth = this.width*0.5f;
        if (Mathf.Abs (angle % 45) == 0) {
            isRight = true;
            view.ShowText (SearchController.instance.searchManager.SetAnswer (beginCell.pos, endCell.pos));
        } else {
            view.HideText ();
            isRight = false;
            linewidth = 5;
        }
        if (cloneLine==null) {
            cloneLine = Instantiate (line, line.transform.parent);
        }
        Vector2 pos = new Vector2 (begin.x, begin.y);
        pos = SetPos (angle,pos,linewidth*0.6f);
        cloneLine.localEulerAngles = new Vector3 (0, 0, angle);
        float lineLength = Mathf.Sqrt (x * x + y * y)+width*0.6f;
        cloneLine.sizeDelta = new Vector2 (lineLength, linewidth);
        cloneLine.anchoredPosition =pos;
        cloneLine.GetComponent<Image> ().color = color;
        cloneLine.gameObject.SetActive (true);
    }
    Vector2 SetPos(float angle,Vector2 pos,float offset) {
        if (angle == 0) {
            pos = new Vector2 (pos.x - offset, pos.y);
        }else if (angle == 180) {
            pos = new Vector2 (pos.x + offset, pos.y);
        }else if (angle == 45) {
            pos = new Vector2 (pos.x - offset, pos.y -offset);
        }else if (angle == -45) {
            pos = new Vector2 (pos.x - offset, pos.y +offset);
        }else if (angle == -90) {
            pos = new Vector2 (pos.x , pos.y + offset);
        }else if (angle == 90) {
            pos = new Vector2 (pos.x, pos.y-offset);
        }else if (angle == 135) {
            pos = new Vector2 (pos.x + offset, pos.y - offset); 
        }else if (angle == -135) {
            pos = new Vector2 (pos.x +offset, pos.y +offset);
        }
        return pos;
    }
    public void SetColor(Color color) {
        this.color= new Color (color.r, color.g, color.b, 0.5f);
    }
    public void SetWidth(float min) {
        width = min;
    }
}

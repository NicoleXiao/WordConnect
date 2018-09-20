using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class PuzzleBlock : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler, IEndDragHandler,IBeginDragHandler {
    public Image bg;
    public Text letter;
    public Image wrong;
    public Position pos = new Position ();
    private BlockParentManager manager;
    private RectTransform parentRect;//这个是最上面的物体

    private Vector2 offset = Vector2.zero;    //用来得到鼠标和图片的差值

    public void SetSize(float min) {
        letter.GetComponent<RectTransform> ().sizeDelta = new Vector2 (min, min);
        bg.GetComponent<RectTransform> ().sizeDelta = new Vector2 (min, min);
        wrong.GetComponent<RectTransform> ().sizeDelta = new Vector2 (min, min);
    }
    public void SetBlockInfo(string text, Color bgColor, Transform parent) {
        letter.text = text;
        bg.color = bgColor;
        transform.SetParent (parent, true);
        manager = parent.GetComponent<BlockParentManager> ();
        parentRect = parent.transform.parent.GetComponent<RectTransform> ();
    }

    public void OnPointerDown(PointerEventData eventData) {
        Vector2 mouseDown = eventData.position;
        Vector2 mouseUguiPos = new Vector2 ();
        bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle (parentRect, mouseDown, eventData.enterEventCamera, out mouseUguiPos);
        if (isRect) {
            offset = this.transform.parent.GetComponent<RectTransform> ().anchoredPosition - mouseUguiPos;
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        manager.SetCellState (false);
        manager.SetRightCellState (false);
    }

    public void OnDrag(PointerEventData eventData) {
     
        this.transform.parent.SetAsLastSibling ();
        Vector2 mouseDrag = eventData.position;
        Vector2 uguiPos = new Vector2 ();
        bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle (parentRect, mouseDrag, eventData.enterEventCamera, out uguiPos);
        if (isRect) {
            //设置图片的ugui坐标与鼠标的ugui坐标保持不变
            this.transform.parent.GetComponent<RectTransform> ().anchoredPosition = offset + uguiPos;
        }

    }

    public void OnPointerUp(PointerEventData eventData) {
        offset = Vector2.zero;
    }

    public void OnEndDrag(PointerEventData eventData) {
        manager.DragEnd ();
        offset = Vector2.zero;
    }

    public void SetWrong() {
        Sequence sq = DOTween.Sequence ();
        sq.Append (wrong.DOFade (1f, 0.8f))
            .Append (wrong.DOFade (0f, 1f)).SetDelay (0.2f);
        sq.Play ();
    }

}

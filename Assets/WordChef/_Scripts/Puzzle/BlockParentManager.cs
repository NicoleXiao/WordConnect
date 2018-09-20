using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class BlockParentManager : MonoBehaviour {
    public int blockNum;
    public bool isPlaceRight = false;
    [HideInInspector]
    public int[,] pos;//用二维数组保存方块的形状
    private float halfWidth = 0;
    private float halfHeight = 0;
    private int row;
    private int col;


    [HideInInspector]
    public List<PuzzleLetterCell> cells = new List<PuzzleLetterCell> ();//方块放置的格子
    private List<PuzzleLetterCell> rightCells = new List<PuzzleLetterCell> ();//正确位置的格子
    private List<PuzzleBlock> blocks = new List<PuzzleBlock> ();
    private RectTransform rectTrans;
    private Vector2 originalPos = Vector2.zero; //方块原来的位置
    private Vector2 wordCenter = Vector2.zero;//正确的位置  
    private int startX = 0;
    private int startY = 0;
    private int siblingIndex = 0;

    public void SetBlockInfo(Vector2 wordPos, float width, float height, int row, int col, Position min) {
        halfWidth = width;
        halfHeight = height;
        this.row = row;
        this.col = col;
        rectTrans = this.GetComponent<RectTransform> ();
        originalPos = rectTrans.anchoredPosition;
        wordCenter = wordPos;
        siblingIndex = transform.GetSiblingIndex ();
        pos = new int[row + 1, col + 1];
        startX = min.x;
        startY = min.y;
    }

    public void AddBlock(PuzzleBlock block) {
        blocks.Add (block);
    }

    public void SetRightCells(List<PuzzleLetterCell> cell) {
        rightCells = cell;
    }
    //设置在二维数组中的位置
    public void SetPos(int x, int y) {
        pos[x - startX, y - startY] = 1;
    }

    public void DragEnd() {

        CalculateUpperLeft ();

    }
    public void SetBlockPosition(Vector2 pos, List<PuzzleLetterCell> cells) {
        this.cells = cells;
        if (Enumerable.SequenceEqual (cells, rightCells)) {
            SetRightCellState (true);
            PuzzleController.instance.Correct (this);
        } else {
            SetCellState (true);
            PuzzleController.instance.NotCorrect (this);
        }
        PuzzleController.instance.CheckGameComplete ();
        rectTrans.anchoredPosition = new Vector2 (pos.x + halfWidth, pos.y - halfHeight);
    }

    public void SetCellState(bool state) {
        if (cells.Count > 0) {
            for (int i = 0; i < cells.Count; i++) {
                cells[i].isShowing = state;
            }
            if (state == false) {
                cells.Clear ();
            }
        }
    }

    //摆放正确的情况下才清除状态
    public void SetRightCellState(bool state) {
        if (rightCells.Count > 0) {
            if (state == false && !isPlaceRight) {
                return;
            }
            isPlaceRight = false;
            for (int i = 0; i < rightCells.Count; i++) {
                rightCells[i].isShowing = state;
            }
        }
    }

    private void CalculateUpperLeft() {
        Vector2 pos = rectTrans.anchoredPosition;

        Vector2 upperLeftPos = new Vector2 (pos.x - halfWidth, pos.y + halfHeight);
        PuzzleController.instance.CalculateBlockLocation (this, upperLeftPos, row, col);
    }

    public void Back() {
        PuzzleController.instance.PlaceBlock (this, false);
        PuzzleController.instance.NotCorrect (this);
        rectTrans.DOKill ();
        rectTrans.DOAnchorPos (originalPos, 0.5f).OnComplete (() => {
            this.transform.SetSiblingIndex (siblingIndex);
        });
    }

    public void ShowHint() {
        rectTrans.DOKill ();
        rectTrans.DOAnchorPos (wordCenter, 0.5f);
        SetCellState (false);
        SetRightCellState (true);

        PuzzleController.instance.RemoveHintBlock (rightCells,this);
    }

    public void ShowBlocksWrong() {
        foreach (PuzzleBlock block in blocks) {
            block.SetWrong ();
        }
    }
}

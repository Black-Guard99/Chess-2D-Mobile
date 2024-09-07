using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoveringPieceView : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image visual;
    [SerializeField] private RectTransform activeView;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private BoardPieceHolder boardPieceHolder;
    [SerializeField] private Piece holderPieceData;
    

    public void Init(Canvas canvas,BoardPieceHolder boardPieceHolder){
        this.canvas = canvas;
        this.boardPieceHolder = boardPieceHolder;
    }
    
    public void OnBeginDrag(PointerEventData eventData) {
        canvasGroup.blocksRaycasts = false;
        holderPieceData = boardPieceHolder.GetPiece();
        activeView.SetParent(canvas.transform);
        activeView.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData) {
        activeView.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData) {
        Debug.Log("Dropping: " + eventData.pointerDrag.transform.name);
        canvasGroup.blocksRaycasts = true;
        ResetPos();
    }
    public void ResetHolderPiece(){
        boardPieceHolder.SetPiece(new Piece{pieceType = PieceType.None,colorType = ColorType.None});
        ResetPos();
    }
    private void ResetPos(){
        activeView.SetParent(boardPieceHolder.transform);
        activeView.anchoredPosition = Vector2.zero;
        canvasGroup.blocksRaycasts = true;
        boardPieceHolder.ResetColor();
    }
    public Piece GetPiece(){
        return holderPieceData;
    }

    public void SetSprite(Sprite sprite) {
        visual.sprite = sprite;
    }

    public BoardPieceHolder GetBoardPieceHolder() {
        return boardPieceHolder;
    }

    public void OnPointerUp(PointerEventData eventData) {
        boardPieceHolder.ResetColor();
    }

    public void OnPointerDown(PointerEventData eventData) {
        Debug.Log("Clicking On Pices..");
    }
}
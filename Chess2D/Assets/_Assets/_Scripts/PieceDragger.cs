using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PieceDragger : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler {
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image visual;
    [SerializeField] private RectTransform activeView;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private BoardPieceHolder boardPieceHolder;
    [SerializeField] private Piece holderPieceData;
    private bool canDrag;
    public void Init(Canvas canvas,BoardPieceHolder boardPieceHolder){
        this.canvas = canvas;
        this.boardPieceHolder = boardPieceHolder;
    }
    public void OnBeginDrag(PointerEventData eventData) {
        if(!ChessGameController.Instance.CanPlay(boardPieceHolder.GetColorType())){
            canDrag = false;
            return;
        }
        canDrag = true;
        canvasGroup.blocksRaycasts = false;
        holderPieceData = boardPieceHolder.GetPiece();
        activeView.SetParent(canvas.transform);
        activeView.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData) {
        if(canDrag){
            activeView.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        Debug.Log("Dropping: " + eventData.pointerDrag.transform.name);
        canvasGroup.blocksRaycasts = true;
        ResetPos();
        canDrag = false;
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

    
}
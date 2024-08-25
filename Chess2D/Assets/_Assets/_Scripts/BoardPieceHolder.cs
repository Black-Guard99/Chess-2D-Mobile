using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardPieceHolder : MonoBehaviour,IDropHandler,IPointerDownHandler,IPointerUpHandler {
    [SerializeField] private Color freeSquareHightingColor = Color.green,opponentSquareColor = Color.red;
    [SerializeField] private TextMeshProUGUI indexText;
    [SerializeField] private RectTransform currentRect;
    [SerializeField] private Image pieceBg;
    [SerializeField] private Sprite b_king,w_king;
    [SerializeField] private Sprite b_pawn,w_pawn;
    [SerializeField] private Sprite b_knight,w_knight;
    [SerializeField] private Sprite b_bishop,w_bishop;
    [SerializeField] private Sprite b_rook,w_rook;
    [SerializeField] private Sprite b_queen,w_queen;
    [SerializeField] private Piece piece;
    [SerializeField] private HoveringPieceView hoveringPieceView;
    private Board board;
    public Color originalColor;
    private List<BoardPieceHolder> boardPieceHoldersToMove;
    private bool hasMoved;

    public void OnDrop(PointerEventData eventData) {
        Debug.Log("Dropping: " + eventData.pointerDrag.transform.name);
        if(eventData.pointerDrag.TryGetComponent(out HoveringPieceView hoveringBox)){
            if(this.piece != null){
                if(this.piece != hoveringBox.GetPiece()){
                    hasMoved = true;
                }
            }
            hoveringBox.ResetHolderPiece();
            SetPiece(hoveringBox.GetPiece());
        }

    }
    public Piece GetPiece(){
        return piece;
    }
    public void SetPiece(Piece piece){
        this.piece = piece;
        hoveringPieceView.gameObject.SetActive(false);
        piece.activePos = currentRect.anchoredPosition;
        UpdatePieceVisual();
        ResetColor();
    }
    public void Init(Vector2 pos,Color boardColor,Board board,Canvas canvas){
        hasMoved = false;
        originalColor = boardColor;
        ResetColor();
        this.board = board;
        currentRect.anchoredPosition = pos;
        hoveringPieceView.Init(canvas,this);
        SetPiece(new Piece{pieceType = PieceType.None,colorType = ColorType.None});
    }
    public void UpdatePieceVisual(){
        hoveringPieceView.gameObject.SetActive(true);
        switch(piece.pieceType){
            case PieceType.None:
                hoveringPieceView.gameObject.SetActive(false);
            break;
            case PieceType.King:
                hoveringPieceView.SetSprite(piece.colorType == ColorType.White? b_king : w_king);
            break;
            case PieceType.Pawn:
                hoveringPieceView.SetSprite(piece.colorType == ColorType.White? b_pawn : w_pawn);
            break;
            case PieceType.Knight:
                hoveringPieceView.SetSprite(piece.colorType == ColorType.White? b_knight : w_knight);
            break;
            case PieceType.Bishop:
                hoveringPieceView.SetSprite(piece.colorType == ColorType.White? b_bishop : w_bishop);
            break;
            case PieceType.Rook:
                hoveringPieceView.SetSprite(piece.colorType == ColorType.White? b_rook : w_rook);
            break;
            case PieceType.Queen:
                hoveringPieceView.SetSprite(piece.colorType == ColorType.White? b_queen : w_queen);
            break;

        }
    }
    public Vector2 PieceCoord(){
        return piece.activePos;
    }
    public void CalculateAvailableDirection(){
        switch(piece.pieceType){
            case PieceType.Bishop:
                boardPieceHoldersToMove = board.GetBishopMoveToDirectionsFromStartingPoint(piece.activePos,piece.colorType);
            break;
            case PieceType.King:
                boardPieceHoldersToMove = board.GetKingMoveToDirectionsFromStartingPoint(piece.activePos,piece.colorType);
            break;
            case PieceType.Knight:
                boardPieceHoldersToMove = board.GetKnightMoveDirection(piece.activePos,piece.colorType);
            break;
            case PieceType.Pawn:
                boardPieceHoldersToMove = board.GetPawnMoveToDirectionsFromStartingPoint(hasMoved,piece.activePos,piece.colorType);
            break;
            case PieceType.Queen:
                boardPieceHoldersToMove = board.GetQueenMoveToDirectionsFromStartingPoint(piece.activePos,piece.colorType);
            break;
            case PieceType.Rook:
                boardPieceHoldersToMove = board.GetRookMoveToDirectionsFromStartingPoint(piece.activePos,piece.colorType);
            break;
        }
        foreach(var pieceToMove in boardPieceHoldersToMove){
            pieceToMove.HighlightColor(pieceToMove.OpponentSquare(piece.colorType));
        }
    }
    public void HighlightColor(bool IsOpponentColor){
        pieceBg.color = IsOpponentColor ? opponentSquareColor : freeSquareHightingColor;
    }
    public void ResetColor(){
        pieceBg.color = originalColor;
        if(boardPieceHoldersToMove != null){
            if(boardPieceHoldersToMove.Count > 0){
                boardPieceHoldersToMove.ForEach(b => b.ResetColor());
                boardPieceHoldersToMove = new List<BoardPieceHolder>();
            }
        }
    }
    public bool IsEmptyPiece(){
        return piece.IsEmptyPieceType();
    }
    private bool OpponentSquare(ColorType otherPieceColor){
        if(otherPieceColor == ColorType.None){
            return false;
        }
        return otherPieceColor != piece.colorType;
    }
    public bool IsOpponent(ColorType otherPiece){
        if(piece.colorType == ColorType.None){
            return true;
        }
        if(piece.colorType != otherPiece){
            return true;
        }
        return false;
    }
    public void SetBoardIndex(int file, int rank) {
        indexText.SetText(string.Concat(file,",",rank));
    }
    public void OnPointerDown(PointerEventData eventData) {
        CalculateAvailableDirection();
    }

    public void OnPointerUp(PointerEventData eventData) {
        ResetColor();
    }
}
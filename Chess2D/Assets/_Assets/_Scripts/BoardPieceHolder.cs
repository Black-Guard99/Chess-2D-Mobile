using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using System.Linq;

public class BoardPieceHolder : MonoBehaviour,IDropHandler,IPointerDownHandler{
    [SerializeField] private HoveringPieceView hoveringPieceView;
    [SerializeField] private Color freeSquareHightingColor = Color.green,opponentSquareColor = Color.red;
    [SerializeField] private Color originalColor;
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
    [SerializeField] private List<BoardPieceHolder> boardPieceHoldersToMove;
    private Board board;
    private bool hasMoved;
    private void Awake() {
        boardPieceHoldersToMove = new List<BoardPieceHolder>();
    }
    public void OnDrop(PointerEventData eventData) {
        if(eventData.pointerDrag.TryGetComponent(out HoveringPieceView hoveringBox)){
            if(CanDropOn(hoveringBox.GetBoardPieceHolder())){
                if(piece != null){
                    if(piece != hoveringBox.GetPiece()){
                        hasMoved = true;
                    }
                }
                hoveringBox.ResetHolderPiece();
                SetPiece(hoveringBox.GetPiece());
            }
        }
        ChessGameController.Instance.UpdatePieceForPlayers();
    }
    public void OnPointerDown(PointerEventData eventData) {
        boardPieceHoldersToMove = CalculateAvailableDirection();
        ShowHighlitedSquares();
    }
    public bool CanDropOn(BoardPieceHolder boardPieceHolder){
        return boardPieceHolder.GetAvailableMoves().Contains(this);
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
                hoveringPieceView.SetSprite(piece.colorType == ColorType.Black? b_king : w_king);
            break;
            case PieceType.Pawn:
                hoveringPieceView.SetSprite(piece.colorType == ColorType.Black? b_pawn : w_pawn);
            break;
            case PieceType.Knight:
                hoveringPieceView.SetSprite(piece.colorType == ColorType.Black? b_knight : w_knight);
            break;
            case PieceType.Bishop:
                hoveringPieceView.SetSprite(piece.colorType == ColorType.Black? b_bishop : w_bishop);
            break;
            case PieceType.Rook:
                hoveringPieceView.SetSprite(piece.colorType == ColorType.Black? b_rook : w_rook);
            break;
            case PieceType.Queen:
                hoveringPieceView.SetSprite(piece.colorType == ColorType.Black? b_queen : w_queen);
            break;

        }
    }
    public Vector2 PieceCoord(){
        return piece.activePos;
    }
    public List<BoardPieceHolder> CalculateAvailableDirection(){
        List<BoardPieceHolder> availablePiecesToMove = new List<BoardPieceHolder>();
        switch(piece.pieceType){
            case PieceType.Bishop:
                availablePiecesToMove = board.GetBishopMoveToDirectionsFromStartingPoint(piece.activePos,piece.colorType);
            break;
            case PieceType.King:
                availablePiecesToMove = board.GetKingMoveToDirectionsFromStartingPoint(piece.activePos,piece.colorType);
            break;
            case PieceType.Knight:
                availablePiecesToMove = board.GetKnightMoveDirection(piece.activePos,piece.colorType);
            break;
            case PieceType.Pawn:
                availablePiecesToMove = board.GetPawnMoveToDirectionsFromStartingPoint(hasMoved,piece.activePos,piece.colorType);
            break;
            case PieceType.Queen:
                availablePiecesToMove = board.GetQueenMoveToDirectionsFromStartingPoint(piece.activePos,piece.colorType);
            break;
            case PieceType.Rook:
                availablePiecesToMove = board.GetRookMoveToDirectionsFromStartingPoint(piece.activePos,piece.colorType);
            break;
        }
        return availablePiecesToMove;
        
    }
    private void ShowHighlitedSquares(){
        foreach(var pieceToMove in boardPieceHoldersToMove){
            // Debug.Log("Piece: " + pieceToMove.GetPiece().pieceType);
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
                boardPieceHoldersToMove.ForEach(piece => piece.ResetColor());
                boardPieceHoldersToMove = new List<BoardPieceHolder>();
            }
        }
    }
    public bool IsEmptyPiece(){
        return piece.IsEmptyPieceType();
    }
    private bool OpponentSquare(ColorType otherPieceColor){
        if(piece.colorType == ColorType.White && otherPieceColor == ColorType.Black){
            return true;
        }
        if(piece.colorType == ColorType.Black && otherPieceColor == ColorType.White){
            return true;
        }
        return false;
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
    public ColorType GetColorType(){
        return piece.colorType;
    }
    public void SetBoardIndex(int file, int rank) {
        indexText.SetText(string.Concat(file,",",rank));
    }
    

    public List<BoardPieceHolder> GetAvailableMoves(){
        return boardPieceHoldersToMove;
    }

    public bool CanMoveToOpponentKing(Vector2 opponenetActivePos) {
        List<BoardPieceHolder> moveToPices = CalculateAvailableDirection();
        if(moveToPices.Count == 0) return false;
        foreach (BoardPieceHolder pieceCanToMove in moveToPices){
            if(pieceCanToMove.GetPiece().activePos == opponenetActivePos) return true;
        }
        return false;
    }
    public bool CanIvadeAttack(List<BoardPieceHolder> opponenetCanMoveToPos){
        List<BoardPieceHolder> moveToPices = CalculateAvailableDirection();
        if(moveToPices.Count == 0) return false;
        List<BoardPieceHolder> filtterdMoves = moveToPices.Where(piece => !opponenetCanMoveToPos.Contains(piece)).ToList();
        return true;
    }

}
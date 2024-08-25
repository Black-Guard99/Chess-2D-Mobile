using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Board : MonoBehaviour {
    private const int BOARD_SIZE = 8;
    private const string findString = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    [SerializeField] private Canvas canvas;
    [SerializeField] private BoardPieceHolder boardPiceView;
    [SerializeField] private float squareSize = 3.5f;
    [SerializeField] private Color lightColor = Color.white,darkColor = Color.black;
    [SerializeField] private RectTransform boardHolder;
    [SerializeField] private BoardPieceHolder[,] board;
    [SerializeField] private Vector2[] knightCheckOffset;
    [SerializeField] private Vector2[] directionToCheckRook,directionToCheckBishop,directionToCheckKing,directionToCheckQueen;
    [SerializeField] private List<BoardPieceHolder> boardPieceHoldersList;

    /* private void Start() {
        SetUp();
    } */
    public void SetUp(){
        board = new BoardPieceHolder[BOARD_SIZE,BOARD_SIZE];
        for (int i = 0; i < BOARD_SIZE; i++) {
            for (int j = 0; j < BOARD_SIZE; j++) {
                BoardPieceHolder piece = Instantiate(boardPiceView,boardHolder);
                bool isLight = (i + j) % 2 != 0;
                Color pieceColor = isLight ? lightColor : darkColor;
                Vector2 pos = new Vector2(squareSize * i,squareSize * j);
                piece.Init(pos,pieceColor,this,canvas);
                piece.SetBoardIndex(i,j);
                board[i,j] = piece;
            }
        }
        SetPieces();
    }
    private void SetPieces(){
        var pieceTypeFromSymbol = new Dictionary<char,PieceType>(){
            ['k'] = PieceType.King,['p'] = PieceType.Pawn,['n'] = PieceType.Knight,['b'] = PieceType.Bishop,['r'] = PieceType.Rook,['q'] = PieceType.Queen
        };
        string fenBoard = findString.Split(' ')[0];
        int file = 0,rank = 7;
        foreach(char symbol in fenBoard){
            if(symbol == '/'){
                file = 0;
                rank --;
            }else{
                if(char.IsDigit(symbol)){
                    file += (int)char.GetNumericValue(symbol);
                }else{
                    ColorType pieceColor = char.IsUpper(symbol)? ColorType.White : ColorType.Black;
                    PieceType pieceType = pieceTypeFromSymbol[char.ToLower(symbol)];
                    board[file,rank].SetPiece(new Piece{pieceType = pieceType,colorType = pieceColor});
                    file++;
                }
            }
        }
    }
    
    public List<BoardPieceHolder> GetRookMoveToDirectionsFromStartingPoint(Vector2 startingPoint,ColorType friendlyPieceType){
        boardPieceHoldersList = new List<BoardPieceHolder>();
        float range = BOARD_SIZE;
        foreach(Vector2 direction in directionToCheckRook){
            for (int i = 1; i <= range; i++) {
                Vector2 nextCoords = startingPoint + direction * squareSize * i;
                Debug.Log("Coord: " + nextCoords);
                if(CheckCoordsInsideBoard(nextCoords)){
                    BoardPieceHolder piece = GetBoardCoordFromPos(nextCoords);
                    if(piece.IsEmptyPiece()){
                        boardPieceHoldersList.Add(piece);
                    }else{
                        if(piece.IsOpponent(friendlyPieceType)){
                            boardPieceHoldersList.Add(piece);
                        }
                        break;
                    }
                }
            }
        }
        return boardPieceHoldersList;

    }
    public List<BoardPieceHolder> GetPawnMoveToDirectionsFromStartingPoint(bool HasMoved,Vector2 startingPoint,ColorType currentPiece){
        boardPieceHoldersList = new List<BoardPieceHolder>();
        float range = HasMoved ? 1: 2;
        Vector2 directionToCheckPawn = currentPiece == ColorType.White ? new Vector2(0,1):new Vector2(0,-1); 
        for (int i = 1; i <= range; i++) {
            Vector2 nextCoords = startingPoint + directionToCheckPawn * squareSize * i;
            if(CheckCoordsInsideBoard(nextCoords)){
                BoardPieceHolder piece = GetBoardCoordFromPos(nextCoords);
                if(piece.IsEmptyPiece()){
                    boardPieceHoldersList.Add(piece);
                }else{
                    break;
                }
            }
        }
        Vector2[] takeDirection = new Vector2[]{new Vector2(1,directionToCheckPawn.y),new Vector2(-1,directionToCheckPawn.y)};
        Debug.Log(takeDirection);
        for (int i = 0; i < takeDirection.Length; i++) {
            Vector2 nextCoords = startingPoint + takeDirection[i] * squareSize;
            if(CheckCoordsInsideBoard(nextCoords)){
                BoardPieceHolder piece = GetBoardCoordFromPos(nextCoords);

                if(!piece.IsEmptyPiece()){
                    if(piece.IsOpponent(currentPiece)){
                        boardPieceHoldersList.Add(piece);
                    }
                }
            }
        }
        return boardPieceHoldersList;

    }
    public List<BoardPieceHolder> GetKingMoveToDirectionsFromStartingPoint(Vector2 startingPoint,ColorType friendlyPieceType){
        boardPieceHoldersList = new List<BoardPieceHolder>();
        float range = 2;
        foreach(Vector2 direction in directionToCheckKing){
            for (int i = 1; i <= range; i++) {
                Vector2 nextCoords = startingPoint + direction * squareSize * i;
                Debug.Log("Coord: " + nextCoords);
                if(CheckCoordsInsideBoard(nextCoords)){
                    BoardPieceHolder piece = GetBoardCoordFromPos(nextCoords);
                    if(piece.IsEmptyPiece()){
                        boardPieceHoldersList.Add(piece);
                    }else{
                        if(piece.IsOpponent(friendlyPieceType)){
                            boardPieceHoldersList.Add(piece);
                        }
                        break;
                    }
                }
            }
        }
        return boardPieceHoldersList;

    }
    public List<BoardPieceHolder> GetKnightMoveDirection(Vector2 startingPoint,ColorType friendlyPieceType){
        boardPieceHoldersList = new List<BoardPieceHolder>();
        for (int i = 0; i < knightCheckOffset.Length; i++) {
            Vector2 nextCoords = startingPoint + knightCheckOffset[i] * squareSize;
            Debug.Log("Coord: " + nextCoords);
            if(CheckCoordsInsideBoard(nextCoords)){
                BoardPieceHolder piece = GetBoardCoordFromPos(nextCoords);
                if(piece.IsOpponent(friendlyPieceType) || piece.IsEmptyPiece()){
                    boardPieceHoldersList.Add(piece);
                }
            }
            
        }
        return boardPieceHoldersList;
    }
   
    public List<BoardPieceHolder> GetBishopMoveToDirectionsFromStartingPoint(Vector2 startingPoint,ColorType friendlyPieceType){
        boardPieceHoldersList = new List<BoardPieceHolder>();
        float range = BOARD_SIZE;
        foreach(Vector2 direction in directionToCheckBishop){
            for (int i = 1; i <= range; i++) {
                Vector2 nextCoords = startingPoint + direction * squareSize * i;
                Debug.Log("Coord: " + nextCoords);
                if(CheckCoordsInsideBoard(nextCoords)){
                    BoardPieceHolder piece = GetBoardCoordFromPos(nextCoords);
                    if(piece.IsEmptyPiece()){
                        boardPieceHoldersList.Add(piece);
                    }else{
                        if(piece.IsOpponent(friendlyPieceType)){
                            boardPieceHoldersList.Add(piece);
                        }
                        break;
                    }
                }
            }
        }
        return boardPieceHoldersList;

    }
    public List<BoardPieceHolder> GetQueenMoveToDirectionsFromStartingPoint(Vector2 startingPoint,ColorType friendlyPieceType){
        boardPieceHoldersList = new List<BoardPieceHolder>();
        float range = BOARD_SIZE;
        foreach(Vector2 direction in directionToCheckQueen){
            for (int i = 1; i <= range; i++) {
                Vector2 nextCoords = startingPoint + direction * squareSize * i;
                Debug.Log("Coord: " + nextCoords);
                if(CheckCoordsInsideBoard(nextCoords)){
                    BoardPieceHolder piece = GetBoardCoordFromPos(nextCoords);
                    if(piece.IsEmptyPiece()){
                        boardPieceHoldersList.Add(piece);
                    }else{
                        if(piece.IsOpponent(friendlyPieceType)){
                            boardPieceHoldersList.Add(piece);
                        }
                        break;
                    }
                }
            }
        }
        return boardPieceHoldersList;

    }


    public BoardPieceHolder GetBoardCoordFromPos(Vector2 pos){
        int x = Mathf.FloorToInt(pos.x / squareSize);
        int y = Mathf.FloorToInt(pos.y / squareSize);
        return board[x,y];
    }
    public bool CheckCoordsInsideBoard(Vector2 pos){
        int x = Mathf.FloorToInt(pos.x / squareSize);
        int y = Mathf.FloorToInt(pos.y / squareSize);
        // Debug.Log("x: " + x + "Y : " + y);
        if(x < 0 || y < 0 || x >= BOARD_SIZE || y >= BOARD_SIZE || x <= -BOARD_SIZE || y <= -BOARD_SIZE){
            return false;
        }
        return true;
    }


    public List<BoardPieceHolder> GetPiecesByColorType(ColorType colorType){
        List<BoardPieceHolder> pieceHolders = new List<BoardPieceHolder>();
        for (int i = 0; i < board.GetLength(0); i++) {
            for (int j = 0; j < board.GetLength(1); j++) {
                if(board[i,j].GetColorType() == colorType){
                    pieceHolders.Add(board[i,j]);
                }
            }
        }
        return pieceHolders;
    }

}

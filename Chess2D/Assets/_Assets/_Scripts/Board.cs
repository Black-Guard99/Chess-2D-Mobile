using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Board : MonoBehaviour {
    public const int BOARD_SIZE = 8;
    [SerializeField] private Canvas canvas;
    [SerializeField] private string findString;
    [SerializeField] private BoardPieceHolder boardPiceView;
    [SerializeField] private float squareSize = 3.5f;
    [SerializeField] private Color lightColor = Color.white,darkColor = Color.black;
    [SerializeField] private RectTransform boardHolder;
    [SerializeField] private BoardPieceHolder[,] board;
    [SerializeField] private DirectionChecks[] allDirectionsToCheckArray;
    [SerializeField] private List<BoardPieceHolder> boardPieceHoldersList;
    private void Start() {
        board = new BoardPieceHolder[BOARD_SIZE,BOARD_SIZE];
        SetUp();
    }
    private void SetUp(){
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
        var pieceTypeFromSymbole = new Dictionary<char,PieceType>(){
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
                    PieceType pieceType = pieceTypeFromSymbole[char.ToLower(symbol)];
                    board[file,rank].SetPiece(new Piece{pieceType = pieceType,colorType = pieceColor});
                    file++;
                }
            }
        }
    }
    
    public List<BoardPieceHolder> GetRookMoveToDirectionsFromStartingPoint(PieceType pieceType,Vector2 startingPoint,ColorType friendlyPieceType){
        DirectionChecks directionChecks = allDirectionsToCheckArray.FirstOrDefault(d => d.pieceType == pieceType);
        boardPieceHoldersList = new List<BoardPieceHolder>();
        float range = directionChecks.maxCheckIteration < 1 ? BOARD_SIZE : directionChecks.maxCheckIteration;
        foreach(Vector2 direction in directionChecks.checkingDirections){
            for (int i = 1; i < range; i++) {
                Vector2 nextCoords = startingPoint + direction * squareSize * i;
                Debug.Log("Coord: " + nextCoords);
                if(CheckCoordsInsideBoard(nextCoords)){
                    BoardPieceHolder piece = GetBoardCoordFromPos(nextCoords);
                    if(piece.IsEmptyPiece()){
                        // availableMovesDirection.Add(nextCoords);
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
        Debug.Log("x: " + x + "Y : " + y);
        if(x < 0 || y < 0 || x >= BOARD_SIZE || y >= BOARD_SIZE || x <= -BOARD_SIZE || y <= -BOARD_SIZE){
            return false;
        }
        return true;
    }
}

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Board : MonoBehaviour {
    public static Board Instance{get;private set;}
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
    private void Awake() {
        if(Instance == null){
            Instance = this;
        }else{
            Destroy(Instance.gameObject);
        }
    }
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
        // Debug.Log(takeDirection);
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
        float range = 1;
        foreach(Vector2 direction in directionToCheckKing){
            for (int i = 1; i <= range; i++) {
                Vector2 nextCoords = startingPoint + direction * squareSize * i;
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
        // Debug.Log("King Check Coord: " + boardPieceHoldersList.Count);
        return boardPieceHoldersList;

    }
    public List<BoardPieceHolder> GetKnightMoveDirection(Vector2 startingPoint,ColorType friendlyPieceType){
        boardPieceHoldersList = new List<BoardPieceHolder>();
        for (int i = 0; i < knightCheckOffset.Length; i++) {
            Vector2 nextCoords = startingPoint + knightCheckOffset[i] * squareSize;
            // Debug.Log("Coord: " + nextCoords);
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
                // Debug.Log("Coord: " + nextCoords);
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
                // Debug.Log("Coord: " + nextCoords);
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
    public void CheckGameOver(BoardPieceHolder lastRunPiece){
        ChessGameController.Instance.UpdateBoard();
        if(IsOpponentKingCheck(lastRunPiece)){
            Debug.Log("check Mate");
            return;
        }
        ChessGameController.Instance.ChangePlayers();
        /* Debug.Log("Turn Change: " + lastRunPiece.GetPiece().pieceType.ToString());
        var pieceToMove = lastRunPiece.GetAllPositionToMove();
        pieceToMove.ForEach(p => p.SetYellow());
        await Task.Delay(500);
        pieceToMove.ForEach(p => p.ResetColor());
        var opponentKingPiece = ChessGameController.Instance.GetOpponentChessPlayer().allPieces.FirstOrDefault(k => k.GetPiece().pieceType == PieceType.King);
        if(opponentKingPiece == null){
            Debug.LogError("No Opponent King piece found");
            ChessGameController.Instance.ChangePlayers();
            return;
        }
        if(pieceToMove.Contains(opponentKingPiece)){
            pieceToMove.ForEach(p => p.ResetColor());
            var currentMoveToPiece = lastRunPiece.GetAllPositionToMove();
            var moveToGoForKing = opponentKingPiece.GetAllPositionToMove();
            // opponentKingPiece.SetYellow();
            List<BoardPieceHolder> availableMove = new List<BoardPieceHolder>();
            foreach(var moveToMove in moveToGoForKing){
                if(!currentMoveToPiece.Contains(moveToMove)){
                    availableMove.Add(moveToMove);
                }
            }
            Debug.Log("Available Move: " + availableMove.Count);
            if(availableMove.Count == 0){
                Debug.LogError("No available move for King");
                ChessGameController.Instance.ChangePlayers();
                return;
            }
            availableMove.ForEach(p => p.SetYellow());
            await Task.Delay(800);
            availableMove.ForEach(p => p.ResetColor());
            
        } */
    }
    public bool IsOpponentKingCheck(BoardPieceHolder lastRunPiece){
        var activePlayer = ChessGameController.Instance.GetActiveChessPlayer();
        var currentOpponent = ChessGameController.Instance.GetOpponentChessPlayer();
        
        // Find the opponent's king piece
        var opponentKingPiece = currentOpponent.allPieces.FirstOrDefault(p => p.GetPiece().pieceType == PieceType.King);
        if (opponentKingPiece == null) {
            Debug.Log("No Opponent King piece found");
            return false;
        }
        Debug.Log("Opponent King: " + opponentKingPiece.GetPiece().pieceType + " " + opponentKingPiece.GetPiece().colorType);

        // Get all available moves for the opponent's king
        List<BoardPieceHolder> opponentKingMoves = opponentKingPiece.GetAllAvailableCanMoveToPieces();

        // Gather all of the active player's available attacking moves
        List<BoardPieceHolder> attackingPositions = new List<BoardPieceHolder>();
        foreach (var piece in activePlayer.allPieces) {
            attackingPositions.AddRange(piece.GetAllAvailableCanMoveToPieces());
        }

        // Check if the opponent's king is in check
        if (!attackingPositions.Contains(opponentKingPiece)) {
            Debug.Log("Opponent King is Not in Check!");
            return false;
        }
        
        // Identify potential moves that the king could take to escape check
        List<BoardPieceHolder> safeKingMoves = opponentKingMoves.Where(move => !attackingPositions.Contains(move)).ToList();
        safeKingMoves.ForEach(p => p.SetYellow(Color.blue)); // Highlight safe moves
        attackingPositions.ForEach(p => p.SetYellow(Color.magenta)); // Highlight attacking positions
        
        // Debug log for king's available moves
        Debug.Log($"{currentOpponent.colorType} Opponent King Available Moves: {safeKingMoves.Count}");
        
        // If there are no safe moves, check if any opponent piece can block or capture the checking piece
        if (safeKingMoves.Count == 0) {
            Debug.LogError($"{currentOpponent.colorType} Opponent King Cannot Move to a Safe Place");
            
            // Check if other pieces can help block or move the king to safety
            bool isSafe = false;
            var opponentsOtherPieces = currentOpponent.allPieces.Where(p => p.GetPiece().pieceType != PieceType.King).ToList();
            
            foreach (var opponentPiece in opponentsOtherPieces) {
                var opponentPieceMoves = opponentPiece.GetAllAvailableCanMoveToPieces();
                if (opponentPieceMoves.Any(move => safeKingMoves.Contains(move))) {
                    isSafe = true;
                    break;
                }
            }

            if (isSafe) {
                Debug.Log($"{currentOpponent.colorType} Opponent King is Safe (Other Piece can help)");
                return false;
            } else {
                Debug.LogError($"{currentOpponent.colorType} Opponent King has No Safe Moves and Cannot Escape Check");
                return true;
            }
        } else {
            // If there are safe moves, continue the game
            // ChessGameController.Instance.ChangePlayers();
            return false;
        }
    }

}

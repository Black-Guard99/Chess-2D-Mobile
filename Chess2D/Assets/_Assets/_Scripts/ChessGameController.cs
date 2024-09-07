using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChessGameController : MonoBehaviour {
    public static ChessGameController Instance{get; private set;}
    [SerializeField]private List<BoardPieceHolder> opponentsPicesCanMoveToPos = new List<BoardPieceHolder>();
    [SerializeField] private List<BoardPieceHolder> activePiecesCanMoveToOpponentKing;
    [SerializeField] private List<BoardPieceHolder> fillterdMoves;
    [SerializeField] private Board board;

    [SerializeField] private ChessGamePlayer whitePlayer,blackPlayer;
    [SerializeField] private ChessGamePlayer activePlayer;
    private void Awake(){
        if(Instance == null){
            Instance = this;
        }else{
            Destroy(Instance);
        }
    }
    private void Start() {
        board.SetUp();
        
        whitePlayer.colorType = ColorType.White;

        blackPlayer.colorType = ColorType.Black;
        UpdatePieceForPlayers();
        activePlayer = whitePlayer;
    }
    public void CheckIsOpponenetKingCaptured(){
        BoardPieceHolder opponenetKing = GetOpponentChessPlayer().allPieces.FirstOrDefault(p => p.GetPiece().pieceType == PieceType.King);
        activePiecesCanMoveToOpponentKing = activePlayer.allPieces.Where(p => p.CanMoveToOpponentKing(opponenetKing.GetPiece().activePos)).ToList();
        Debug.Log("Found Pieces Can Move to King: " + activePiecesCanMoveToOpponentKing.Count);
        if(activePiecesCanMoveToOpponentKing.Count > 0){
            opponentsPicesCanMoveToPos = new List<BoardPieceHolder>();
            foreach(var piece in activePlayer.allPieces){
                var moveToPos = piece.CalculateAvailableDirection();
                foreach(var move in moveToPos){
                    opponentsPicesCanMoveToPos.Add(move);
                }
            }
            // bool canMoveToAwayFromAttackingPiece = opponenetKing.CanIvadeAttack(opponentsPicesCanMoveToPos);
            // Debug.Log("Can Ivade the Opponent Attack : " + canMoveToAwayFromAttackingPiece);
            fillterdMoves = new List<BoardPieceHolder>();
            List<BoardPieceHolder> opponenetKingMoveToPices = opponenetKing.CalculateAvailableDirection();
            foreach(var kingMovePos in opponenetKingMoveToPices){
                if(!opponentsPicesCanMoveToPos.Contains(kingMovePos)){
                    fillterdMoves.Add(kingMovePos);
                }
            }
            if(fillterdMoves.Count == 0){
                Debug.Log("King Trapped!");
            }
        }
    }

    public void ChangePlayers(){
        if(activePlayer.colorType == whitePlayer.colorType){
            activePlayer = blackPlayer;
        }else{
            activePlayer = whitePlayer;
        }
    }
    public void UpdatePieceForPlayers(){
        whitePlayer.allPieces = board.GetPiecesByColorType(ColorType.White);
        blackPlayer.allPieces = board.GetPiecesByColorType(ColorType.Black);
        CheckIsOpponenetKingCaptured();
    }
    public ChessGamePlayer GetOpponentChessPlayer(){
        return activePlayer.colorType == whitePlayer.colorType ? blackPlayer : whitePlayer;
    }
}
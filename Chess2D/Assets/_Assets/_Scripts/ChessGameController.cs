using UnityEngine;

public class ChessGameController : MonoBehaviour {
    public static ChessGameController Instance{get; private set;}
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
        // whitePlayer.allPieces = board.GetPiecesByColorType(ColorType.White);

        blackPlayer.colorType = ColorType.Black;
        // blackPlayer.allPieces = board.GetPiecesByColorType(ColorType.Black);
        UpdatePieceForPlayers();
        activePlayer = whitePlayer;
    }
    public void CheckIsOpponenetKingCaptured(){
        
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
    }
    public ChessGamePlayer GetOpponentChessPlayer(){
        return activePlayer.colorType == whitePlayer.colorType ? blackPlayer : whitePlayer;
    }
}
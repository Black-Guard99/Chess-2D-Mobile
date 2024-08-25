using UnityEngine;

public class ChessGameController : MonoBehaviour {
    [SerializeField] private Board board;

    [SerializeField] private ChessGamePlayer whitePlayer,blackPlayer;
    [SerializeField] private ChessGamePlayer activePlayer;
    private void Start() {
        board.SetUp();
        
        whitePlayer.colorType = ColorType.White;
        whitePlayer.allPieces = board.GetPiecesByColorType(ColorType.White);

        blackPlayer.colorType = ColorType.Black;
        blackPlayer.allPieces = board.GetPiecesByColorType(ColorType.Black);
        activePlayer = whitePlayer;
    }


    public void ChangePlayers(){
        if(activePlayer.colorType == whitePlayer.colorType){
            activePlayer = blackPlayer;
        }else{
            activePlayer = whitePlayer;
        }
    }
    public ChessGamePlayer GetOpponentChessPlayer(){
        return activePlayer.colorType == whitePlayer.colorType ? blackPlayer : whitePlayer;
    }
}
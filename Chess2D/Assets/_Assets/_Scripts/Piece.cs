using UnityEngine;

public enum PieceType{
    None = 0,
    King = 1,
    Pawn = 2,
    Knight = 3,
    Bishop = 4,
    Rook = 5,
    Queen = 6
}
public enum ColorType{
    None,
    White,
    Black,
}
[System.Serializable]
public class Piece {
    public PieceType pieceType;
    public ColorType colorType;
    public Vector2 activePos;
    public bool IsEmptyPieceType(){
        return pieceType == PieceType.None && colorType == ColorType.None;
    }
}
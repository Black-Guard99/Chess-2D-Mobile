using UnityEngine;

[System.Serializable]
public class DirectionChecks {
    public PieceType pieceType;
    public int maxCheckIteration = 0;
    public Vector2[] checkingDirections;
}
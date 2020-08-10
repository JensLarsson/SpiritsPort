using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovable
{
    void Move(BoardTile targetTile, bool useMoveAction);
    void ShowDirectionArrow(BoardTile from, BoardTile to);
    Vector2 Position { get; }
    bool Pushable { get; }
    int MaxMovement { get;  set; }
    BoardTile OccupiedTile { get; }
}

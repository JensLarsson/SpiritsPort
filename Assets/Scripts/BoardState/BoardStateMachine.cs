using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class BoardStateMachine
{
    BoardState currentState;
    public BoardStateMachine(BoardState startState)
    {
        currentState = startState;
    }
    public void Update(Vector2Int boardPos, GameBoard gameBoard) => currentState.Update(gameBoard, boardPos);
    public void Interact(GameBoard board, Vector2Int pos) => currentState.Interact(board, pos);
    public void ChangeState(BoardState state, GameBoard board)
    {
        currentState.LeaveState(board);
        currentState = state;
        currentState.EnterState(board);
    }
}


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class BoardState_PlaceUnits : BoardState
{
    Player selectedPlayer;
    bool placeOnLeftSide;
    BoardTile previousTile;
    Stack<Creature> creatures = new Stack<Creature>();
    BoardState upcommingState;
    Vector2Int lastBoardPos;

    BoardUnit selectedUnit;
    public BoardState_PlaceUnits(Player player, bool leftSide, BoardState nextState)
    {
        selectedPlayer = player;
        placeOnLeftSide = leftSide;
        creatures = new Stack<Creature>(player.Creatures);
        upcommingState = nextState;
    }
    public override void EnterState(GameBoard gameBoard, (int x, int y)[] positions = null)
    {
        int x = placeOnLeftSide ? 0 : (int)gameBoard.boardWidth - (int)gameBoard.boardWidth / 2;
        for (int i = 0; i < gameBoard.boardWidth / 2; i++)
        {
            for (int y = 0; y < gameBoard.boardHeight; y++)
            {
                BoardTile tile = gameBoard.GetBoardTile(x + i, y);
                if (!tile.Occupied())
                {
                    tile.SetState(TILE_MODE.MoveAllowed);
                }
            }
        }
        selectedUnit = gameBoard.CreateBoardUnit(creatures.Pop(), selectedPlayer);
    }

    //unit.UnitConstructor(creature, GetBoardTile(position));
    public override void Update(GameBoard gameBoard, Vector2Int boardPosition)
    {
        BoardTile tile = gameBoard.GetBoardTile(boardPosition);
        if (tile?.State == TILE_MODE.MoveAllowed)
        {
            if (selectedUnit != null && lastBoardPos != boardPosition)
            {
                selectedUnit.transform.position = tile.transform.position;
            }
        }
        if (Input.GetButtonDown(KeyBindingLibrary.EndTurn) && creatures.Count < 1)
        {
            gameBoard.ChangeState(upcommingState);
        }
    }
    public override void Interact(GameBoard gameBoard, Vector2Int position)
    {
        BoardTile tile = gameBoard.GetBoardTile(position);

        if (tile?.GetUnit != null && tile?.State == TILE_MODE.MoveAllowed)
        {
            BoardUnit temp = tile?.GetUnit;
            selectedUnit?.Move(tile);
            selectedUnit = temp;
        }
        else if (selectedUnit != null && tile?.State == TILE_MODE.MoveAllowed)
        {
            selectedUnit.Move(tile);
            selectedUnit = null;
            if (creatures.Count > 0)
            {
                selectedUnit = gameBoard.CreateBoardUnit(creatures.Pop(), selectedPlayer);
            }
        }
    }
    public override void LeaveState(GameBoard gameBoard)
    {
        foreach (BoardTile tile in gameBoard.Board)
        {
            tile.SetState(TILE_MODE.Unselected);
        }
    }
}


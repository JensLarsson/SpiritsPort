using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardState_Selected : BoardState
{
    BoardTile selectedTile;
    public BoardState_Selected(BoardTile tile)
    {
        selectedTile = tile;
    }

    public override void EnterState(GameBoard gameBoard, (int x, int y)[] positions = null)
    {
        BoardUnitBaseClass unit = selectedTile.GetUnit;
        if (unit.MayAct)
        {
            if (unit.MayMove)
            {
                SetBoardForMovement(gameBoard, selectedTile);
            }
            if (unit.mayUseAbility)
            {
                UI_Abilities.instance.AddAbilities(selectedTile.GetUnit.abilities, selectedTile);
            }
        }
        gameBoard.SetSelectionMarker(selectedTile);
    }

    public override void Interact(GameBoard gameBoard, Vector2Int position)
    {
        var boardTile = gameBoard.GetBoardTile(position);
        if (boardTile != null)
        {
            if (boardTile.GetUnit == null
                && boardTile.State == TILE_MODE.MoveAllowed)
            {
                gameBoard.MoveUnit(selectedTile.GetUnit, boardTile.BoardPosition, true);
            }
            else if (boardTile.GetUnit?.OwningPlayer == gameBoard.CurrentPlayer)
            {
                gameBoard.ChangeAllBoardTilesState(TILE_MODE.Unselected);
                gameBoard.ChangeState(new BoardState_Selected(boardTile));
                return;
            }
        }
        gameBoard.ChangeState(new BoardState_UnSelected(gameBoard.CurrentPlayer));
    }
 

    public override void LeaveState(GameBoard gameBoard)
    {
        UI_Abilities.instance.ClearButtons();
        gameBoard.HideSelectionMarker();
    }

    void SetBoardForMovement(GameBoard gameBoard, BoardTile startTile)
    {
        bool[,] accessableTiles = PathFinding.DijkstraPath(gameBoard.GetAccessibleTiles(),
            startTile.GetUnit.MaxMovement, startTile.BoardPosition);
        for (int x = 0; x < accessableTiles.GetLength(0); x++)
        {
            for (int y = 0; y < accessableTiles.GetLength(1); y++)
            {
                gameBoard.GetBoardTile(x, y).SetState(
                    accessableTiles[x, y] ? TILE_MODE.MoveAllowed : TILE_MODE.Unselected
                    );
            }
        }
    }
}


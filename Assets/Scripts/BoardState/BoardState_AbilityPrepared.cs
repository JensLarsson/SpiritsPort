using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class BoardState_AbilityPrepared : BoardState
{
    Ability activeAbility;
    BoardTile selectedTile;
    BoardTile lastTargetedTile;

    public BoardState_AbilityPrepared(Ability ability, BoardTile tile)
    {
        activeAbility = ability;
        selectedTile = tile;

    }
    public override void EnterState(GameBoard gameBoard, (int x, int y)[] positions = null)
    {
        foreach (BoardTile tile in gameBoard.Board)
        {
            tile.SetState(TILE_MODE.Unselected);
        }
        activeAbility.GetTargetableTiles(gameBoard, selectedTile);
        gameBoard.SetSelectionMarker(selectedTile);
    }

    public override void Update(GameBoard gameBoard, Vector2Int boardPosition)
    {
        if (boardPosition != lastTargetedTile?.BoardPosition)
        {
            lastTargetedTile?.GetUnit?.HideDirectionArrow();
            base.Update(gameBoard, boardPosition);
            BoardTile mouseOverTile = gameBoard.GetBoardTile(boardPosition);
            if (mouseOverTile?.State == TILE_MODE.AttackAllowed && activeAbility.PushesTarget)
            {
                mouseOverTile?.GetUnit?.ShowDirectionArrow(selectedTile, mouseOverTile);
            }

            lastTargetedTile = mouseOverTile;
        }
    }

    public override void Interact(GameBoard gameBoard, Vector2Int position)
    {
        if (gameBoard.GetBoardTile(position).State == TILE_MODE.AttackAllowed)
        {
            Action OnAbilityHit = () =>
            {
                gameBoard.ChangeState(new BoardState_UnSelected(gameBoard.CurrentPlayer));
            };

            gameBoard.ChangeState(new BoardState_Awaiting());
            activeAbility.Invoke(selectedTile, gameBoard.GetBoardTile(position), OnAbilityHit);
        }
        else
        {
            gameBoard.ChangeState(new BoardState_UnSelected(gameBoard.CurrentPlayer));
        }
        gameBoard.ChangeAllBoardTilesState(TILE_MODE.Unselected);
    }
    public override void LeaveState(GameBoard gameBoard)
    {
        gameBoard.HideSelectionMarker();
        foreach (BoardTile tile in gameBoard.Board)
        {
            tile.GetUnit?.HideDirectionArrow();
        }
    }

}


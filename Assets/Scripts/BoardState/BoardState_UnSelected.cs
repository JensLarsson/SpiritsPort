using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BoardState_UnSelected : BoardState
{
    Player currentPlayer;
    public BoardState_UnSelected(Player player)
    {
        currentPlayer = player;
    }
    public override void EnterState(GameBoard gameBoard, (int x, int y)[] positions = null)
    {
        gameBoard.ChangeAllBoardTilesState(TILE_MODE.Unselected);
    }

    public override void Interact(GameBoard gameBoard, Vector2Int position)
    {

        if (gameBoard.GetBoardTile(position).GetUnit != null && gameBoard.GetBoardTile(position).GetUnit.OwningPlayer == currentPlayer)
        {
            gameBoard.ChangeState(new BoardState_Selected(gameBoard.GetBoardTile(position)));
        }
    }
}
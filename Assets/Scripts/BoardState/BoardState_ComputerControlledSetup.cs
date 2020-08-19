using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class BoardState_ComputerControlledSetup : BoardState
{
    Player currentPlayer;
    public BoardState_ComputerControlledSetup(Player player)
    {
        currentPlayer = player;
    }

    public override void EnterState(GameBoard gameBoard, (int x, int y)[] positions = null)
    {

    }

    public override void Interact(GameBoard gameBoard, Vector2Int position)
    {
    }
}


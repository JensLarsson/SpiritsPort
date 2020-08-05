using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class BoardState_PlaceUnits : BoardState
{

    public BoardState_PlaceUnits(Player player)
    {

    }
    public override void EnterState(GameBoard gameBoard, (int x, int y)[] positions = null)
    {
        throw new NotImplementedException();
    }

    public override void Interact(GameBoard gameBoard, Vector2Int position)
    {
        throw new NotImplementedException();
    }
}


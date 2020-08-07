using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
[Serializable]
public abstract class BoardState
{
    public abstract void EnterState(GameBoard gameBoard, (int x, int y)[] positions = null);
    public abstract void Interact(GameBoard gameBoard, Vector2Int position);

    public virtual void LeaveState(GameBoard gameBoard) { }
    public virtual void Update(GameBoard gameBoard, Vector2Int boardPosition)
    {
        if (Input.GetButtonDown(KeyBindingLibrary.EndTurn))
        {
            EndTurnWindow.Instance.EndTurnPopup();
        }
    }
}


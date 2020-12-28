using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player", menuName = "Player")]
public class Player : ScriptableObject
{
    [SerializeField] string playerName;
    [SerializeField] Color playerColour = Color.white;
    [SerializeField] List<BoardUnit> creatures;
    [SerializeField] bool isComputer = false;

    //public void AddUnit(BoardUnitBaseClass unit)
    //{
    //    boardUnits.Add(unit);
    //}
    //public void RemoveUnit(BoardUnitBaseClass unit)
    //{
    //    boardUnits.Add(unit);
    //}



    public string PlayerName => playerName;
    public ReadOnlyCollection<BoardUnit> Creatures => creatures.AsReadOnly();
    public Color PlayerColour => playerColour;
    public BoardState ControllState => isComputer ?
        new BoardState_ComputerControlled(this, GameBoard.Instance.useMiniMax, GameBoard.Instance.miniMaxDepth)
        : new BoardState_UnSelected(this) as BoardState;
}


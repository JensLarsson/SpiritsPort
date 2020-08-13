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


    //public void AddUnit(BoardUnitBaseClass unit)
    //{
    //    boardUnits.Add(unit);
    //}
    //public void RemoveUnit(BoardUnitBaseClass unit)
    //{
    //    boardUnits.Add(unit);
    //}

    public Player(Color col)
    {
        playerColour = col;
    }



    public string PlayerName => playerName;
    public ReadOnlyCollection<BoardUnit> Creatures => creatures.AsReadOnly();
    public Color PlayerColour => playerColour;
}


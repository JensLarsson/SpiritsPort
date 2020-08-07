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
    [SerializeField] List<Creature> creatures;
    List<BoardUnit> boardUnits = new List<BoardUnit>();


    public void AddUnit(BoardUnit unit)
    {
        boardUnits.Add(unit);
    }
    public void RemoveUnit(BoardUnit unit)
    {
        boardUnits.Add(unit);
    }

    public Player(Color col)
    {
        playerColour = col;
    }



    public string PlayerName => playerName;
    public ReadOnlyCollection<Creature> Creatures => creatures.AsReadOnly();
    public List<BoardUnit> BoardUnits => boardUnits;
    public Color PlayerColour => playerColour;
}


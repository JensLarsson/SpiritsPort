using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using System.Collections;
using UnityEngine;

class BoardState_ComputerControlled : BoardState
{
    Player currentPlayer;
    Queue<BoardUnitBaseClass> units;
    public BoardState_ComputerControlled(Player player)
    {
        currentPlayer = player;
    }

    public override void EnterState(GameBoard gameBoard, (int x, int y)[] positions = null)
    {
        units = new Queue<BoardUnitBaseClass>(gameBoard.GetAllUnitsOfFaction(currentPlayer));
        RandomUnitRound(gameBoard);
    }

    void RandomUnitRound(GameBoard gameBoard)
    {
        if (units.Count > 0)
        {
            BoardUnit unit = units.Dequeue() as BoardUnit;
            List<BoardTile> accessableTiles = GetAccessableTiles(gameBoard, unit);
            int offset = Random.Range(0, accessableTiles.Count);
            bool abilityUsed = false;
            for (int i = 0; i < accessableTiles.Count && !abilityUsed; i++)
            {
                int index = (i + offset) % accessableTiles.Count;
                Ability ability = unit.Abilites[0];
                List<BoardTile> targetableTiles = ability.GetLinearTiles(gameBoard, accessableTiles[index].BoardPosition, ability.movesThroughUnits);
                foreach (BoardTile tile in targetableTiles)
                {
                    if (tile.Occupied() && tile?.GetUnit?.OwningPlayer != currentPlayer && tile?.GetUnit?.Targetable == true)
                    {
                        ActionDelayer.RunAfterDelay(0.2f, () =>
                        {
                            gameBoard.MoveUnit(unit, accessableTiles[index].BoardPosition, true);
                            ActionDelayer.RunAfterDelay(0.5f, () =>
                            {
                                ability.Invoke(unit.OccupiedTile, tile, () => { RandomUnitRound(gameBoard); });
                            });
                        });
                        abilityUsed = true;
                        break;
                    }
                }
            }
            if (!abilityUsed)
            {
                RandomUnitRound(gameBoard);
            }
        }
        else
        {
            gameBoard.EndTurn();
        }
    }





    List<BoardTile> GetAccessableTiles(GameBoard gameBoard, BoardUnit unit)
    {
        bool[,] tileAccesArray = PathFinding.DijkstraPath(gameBoard.GetAccessibleTiles(),
       unit.MaxMovement, unit.OccupiedTile.BoardPosition);
        List<BoardTile> accessableTiles = new List<BoardTile>();
        for (int x = 0; x < tileAccesArray.GetLength(0); x++)
        {
            for (int y = 0; y < tileAccesArray.GetLength(1); y++)
            {
                if (tileAccesArray[x, y] == true)
                {
                    accessableTiles.Add(gameBoard.Board[x, y]);
                }
            }
        }
        return accessableTiles;
    }




    public override void Interact(GameBoard gameBoard, Vector2Int position)
    {

    }

}


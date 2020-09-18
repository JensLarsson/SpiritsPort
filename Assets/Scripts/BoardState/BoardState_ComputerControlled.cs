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
    Queue<BoardUnitBaseClass> playerUnits;
    List<Vector2Int> enemyPositions = new List<Vector2Int>();
    public BoardState_ComputerControlled(Player player)
    {
        currentPlayer = player;
    }

    public override void EnterState(GameBoard gameBoard, (int x, int y)[] positions = null)
    {
        playerUnits = new Queue<BoardUnitBaseClass>(gameBoard.GetAllUnitsOfFaction(currentPlayer));
        var enemies = gameBoard.GetAllUnitsOfFaction(gameBoard.NotCurrentPlayer);
        foreach (BoardUnitBaseClass enemy in enemies)
        {
            enemyPositions.Add(((BoardUnit)enemy).BoardPosition);
        }



        int i = 0;
        //BoardTurnInfo BestMove = MinMaxSearch(gameBoard, new BoardTurnInfo(gameBoard.Board, gameBoard.CurrentPlayer), new Queue<BoardUnitBaseClass>(playerUnits), ref i);
        //Debug.Log(i);
        //MinMaxRoundEngage(gameBoard, BestMove);

        System.Diagnostics.Stopwatch a = new System.Diagnostics.Stopwatch();
        List<BoardUnitBaseClass> derp = new List<BoardUnitBaseClass>(playerUnits);
        a.Start();
        PathFinding.DijkstraPath(gameBoard.GetAccessibleTiles(), 3, playerUnits.Peek().OccupiedTile.BoardPosition);
        a.Stop();
        Debug.Log(a.ElapsedTicks);
        a.Reset();
        a.Start();
        var test = PathFinding.aStar(gameBoard.GetAccessibleTiles(), derp[0].OccupiedTile.BoardPosition, enemyPositions, derp[0].MaxMovement);
        a.Stop();
        Debug.Log(a.ElapsedTicks);
        Debug.Log("tiles: " + test.Count());
        foreach (var tile in test)
        {
            Debug.Log(tile.pos);
            gameBoard.GetBoardTile(tile.pos).SetState(TILE_MODE.AttackAllowed);
        }
        //a.Reset();
        //a.Start();
        //test = PathFinding.aStar(gameBoard.GetAccessibleTiles(), derp[1].OccupiedTile.BoardPosition, enemyPositions, derp[1].MaxMovement);
        //a.Stop();
        //Debug.Log(a.ElapsedTicks);
        //Debug.Log("tiles: " + test.Count()); foreach (var tile in test)
        //{
        //    gameBoard.GetBoardTile(tile.pos).SetState(TILE_MODE.AttackAllowed);
        //}
        //a.Reset();
        //a.Start();
        //test = PathFinding.aStar(gameBoard.GetAccessibleTiles(), derp[2].OccupiedTile.BoardPosition, enemyPositions, derp[2].MaxMovement);
        //a.Stop();
        //Debug.Log(a.ElapsedTicks);
        //Debug.Log("tiles: " + test.Count());
        foreach (var tile in test)
        {
            gameBoard.GetBoardTile(tile.pos).SetState(TILE_MODE.AttackAllowed);
        }

        //RandomUnitRound(gameBoard);
    }

    void RandomUnitRound(GameBoard gameBoard)
    {
        if (playerUnits.Count > 0)
        {
            BoardUnit unit = playerUnits.Dequeue() as BoardUnit;
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

    BoardTurnInfo MinMaxSearch(GameBoard gameBoard, BoardTurnInfo state, Queue<BoardUnitBaseClass> units, ref int count)
    {
        BoardTurnInfo bestState = state;
        if (units.Count > 0)
        {
            count++;
            BoardUnit unit = units.Dequeue() as BoardUnit;
            //List<BoardTile> accessableTiles = GetAccessableTiles(gameBoard, unit);
            List<WeightedTile> accessableTiles = PathFinding.aStar(gameBoard.GetAccessibleTiles(), unit.BoardPosition, enemyPositions, unit.MaxMovement);
            for (int i = 0; i < accessableTiles.Count; i++)
            {
                Ability ability = unit.Abilites[0];
                List<BoardTile> targetableTiles = ability.GetLinearTiles(gameBoard, accessableTiles[i].pos, ability.movesThroughUnits);
                foreach (BoardTile tile in targetableTiles)
                {
                    BoardTurnInfo stateClone = state.Clone();
                    stateClone.Move(unit.OccupiedTile.BoardPosition, accessableTiles[i].pos);
                    stateClone.DamageTile(tile.BoardPosition, ability);

                    BoardTurnInfo recursionState = MinMaxSearch(gameBoard, stateClone, new Queue<BoardUnitBaseClass>(units), ref count);
                    if (recursionState.turnValue > bestState.turnValue)
                    {
                        bestState = recursionState;
                    }
                }
            }
        }
        return bestState;
    }

    void MinMaxRoundEngage(GameBoard gameBoard, BoardTurnInfo turnInfo)
    {
        if (turnInfo.abilities.Count > 0 && turnInfo.moves.Count > 0)
        {
            var move = turnInfo.moves.Dequeue();
            var abilityInfo = turnInfo.abilities.Dequeue();
            Debug.Log(move.to);
            Debug.Log(abilityInfo.pos);
            BoardTile tile = gameBoard.GetBoardTile(move.from);
            gameBoard.MoveUnit(tile.GetUnit, move.to);
            abilityInfo.ability.Invoke(gameBoard.GetBoardTile(move.to), gameBoard.GetBoardTile(abilityInfo.pos),
                () =>
                {
                    MinMaxRoundEngage(gameBoard, turnInfo);
                });
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


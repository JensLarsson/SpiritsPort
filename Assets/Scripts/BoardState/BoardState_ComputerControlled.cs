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
    Queue<BoardUnitBaseClass> computerUnits;
    Queue<BoardUnitBaseClass> enemyUnits;
    public BoardState_ComputerControlled(Player player)
    {
        currentPlayer = player;
    }

    public override void EnterState(GameBoard gameBoard, (int x, int y)[] positions = null)
    {
        computerUnits = new Queue<BoardUnitBaseClass>(gameBoard.GetAllUnitsOfFaction(currentPlayer));
        enemyUnits = new Queue<BoardUnitBaseClass>(gameBoard.GetAllUnitsOfFaction(gameBoard.NotCurrentPlayer));




        int i = 0;
        //BoardTurnInfo BestMove = MaxTurn(gameBoard, new BoardTurnInfo(gameBoard.Board, gameBoard.CurrentPlayer), new Queue<BoardUnitBaseClass>(playerUnits), ref i);
        BoardTurnInfo BestMove = MiniMax(gameBoard, new BoardTurnInfo(gameBoard.Board, gameBoard.CurrentPlayer),
            new Queue<BoardUnitBaseClass>[] {
            new Queue<BoardUnitBaseClass>(computerUnits),
            new Queue<BoardUnitBaseClass>(enemyUnits)
            },
            ref i);
        Debug.Log(i);
        MinMaxRoundEngage(gameBoard, BestMove, computerUnits.Count);

        //RandomUnitRound(gameBoard);
    }

    void RandomUnitRound(GameBoard gameBoard)
    {
        if (computerUnits.Count > 0)
        {
            BoardUnit unit = computerUnits.Dequeue() as BoardUnit;
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

    BoardTurnInfo MaxTurn(GameBoard gameBoard, BoardTurnInfo state, Queue<BoardUnitBaseClass> units, ref int count, bool maximizing = true)
    {
        count++;
        BoardTurnInfo bestState = state;
        if (units.Count > 0)
        {
            BoardUnit unit = units.Dequeue() as BoardUnit;
            Stack<Vector2Int> accessableTiles = state.GetAccessableTiles(unit.BoardPosition, unit.MaxMovement);
            //List<WeightedTile> accessableTiles = PathFinding.aStar(gameBoard.GetAccessibleTiles(), unit.BoardPosition, enemyPositions, unit.MaxMovement);
            while (accessableTiles.Count > 0)
            {
                Vector2Int pos = accessableTiles.Pop();
                Ability ability = unit.Abilites[0];
                List<BoardTile> targetableTiles = ability.GetLinearTiles(gameBoard, pos, ability.movesThroughUnits);
                foreach (BoardTile tile in targetableTiles)
                {
                    BoardTurnInfo stateClone = state.Clone();
                    stateClone.Move(unit.OccupiedTile.BoardPosition, pos);
                    stateClone.DamageTile(tile.BoardPosition, ability);

                    BoardTurnInfo recursionState = MaxTurn(gameBoard, stateClone, new Queue<BoardUnitBaseClass>(units), ref count);
                    bestState = maximizing ?
                        (recursionState.turnValue > bestState.turnValue ? recursionState : bestState)
                        :
                        (recursionState.turnValue < bestState.turnValue ? recursionState : bestState);
                }
            }
        }
        return bestState;
    }

    //BoardTurnInfo MaxTurn(GameBoard gameBoard, BoardTurnInfo state, Queue<BoardUnitBaseClass> units, ref int count, bool maximizing = true)
    //{
    //    count++;
    //    BoardTurnInfo bestState = state;
    //    if (units.Count > 0)
    //    {
    //        BoardUnit unit = units.Dequeue() as BoardUnit;
    //        Stack<Vector2Int> accessableTiles = state.GetAccessableTiles(unit.BoardPosition, unit.MaxMovement);
    //        //List<WeightedTile> accessableTiles = PathFinding.aStar(gameBoard.GetAccessibleTiles(), unit.BoardPosition, enemyPositions, unit.MaxMovement);
    //        for (int i = 0; i < accessableTiles.Count; i++)
    //        {
    //            Ability ability = unit.Abilites[0];
    //            List<BoardTile> targetableTiles = ability.GetLinearTiles(gameBoard, gameBoard.GetBoardTile(accessableTiles[i]), ability.movesThroughUnits);
    //            foreach (BoardTile tile in targetableTiles)
    //            {
    //                BoardTurnInfo stateClone = state.Clone();
    //                stateClone.Move(unit.OccupiedTile.BoardPosition, accessableTiles[i]);
    //                stateClone.DamageTile(tile.BoardPosition, ability);

    //                BoardTurnInfo recursionState = MaxTurn(gameBoard, stateClone, new Queue<BoardUnitBaseClass>(units), ref count);
    //                bestState = maximizing ?
    //                    (recursionState.turnValue > bestState.turnValue ? recursionState : bestState)
    //                    :
    //                    (recursionState.turnValue < bestState.turnValue ? recursionState : bestState);
    //            }
    //        }
    //    }
    //    return bestState;
    //}

    BoardTurnInfo MiniMax(GameBoard gameBoard, BoardTurnInfo state, Queue<BoardUnitBaseClass>[] units, ref int count, bool maximizing = true, int depth = 0)
    {
        if (depth == 3)
        {
            return state;
        }
        if (maximizing)
        {
            BoardTurnInfo newState = MaxTurn(gameBoard, state.Clone(), new Queue<BoardUnitBaseClass>(units[0]), ref count, true);
            return MiniMax(gameBoard, newState, units, ref count, false, depth + 1);
        }
        else
        {
            BoardTurnInfo newState = MaxTurn(gameBoard, state.Clone(), new Queue<BoardUnitBaseClass>(units[1]), ref count, false);
            return MiniMax(gameBoard, newState, units, ref count, true, depth + 1);
        }
        return state;
    }

    //List<BoardTile> GetAccessableTiles(BoardTurnInfo gameBoard, BoardUnit unit)
    //{

    //}
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
    void MinMaxRoundEngage(GameBoard gameBoard, BoardTurnInfo turnInfo, int units)
    {
        if (units > 0)
        {
            var move = turnInfo.moves.Dequeue();
            var abilityInfo = turnInfo.abilities.Dequeue();
            BoardTile tile = gameBoard.GetBoardTile(move.from);
            gameBoard.MoveUnit(tile.GetUnit, move.to);
            abilityInfo.ability.Invoke(gameBoard.GetBoardTile(move.to), gameBoard.GetBoardTile(abilityInfo.pos),
                () =>
                {
                    MinMaxRoundEngage(gameBoard, turnInfo, units - 1);
                });
        }
        else
        {
            gameBoard.EndTurn();
        }
    }





    public override void Interact(GameBoard gameBoard, Vector2Int position)
    {

    }

}


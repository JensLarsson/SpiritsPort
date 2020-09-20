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

    const int MAX = 9999;
    const int MIN = -9999;
    public BoardState_ComputerControlled(Player player)
    {
        currentPlayer = player;
    }

    public override void EnterState(GameBoard gameBoard, (int x, int y)[] positions = null)
    {
        computerUnits = new Queue<BoardUnitBaseClass>(gameBoard.GetAllUnitsOfFaction(currentPlayer));
        enemyUnits = new Queue<BoardUnitBaseClass>(gameBoard.GetAllUnitsOfFaction(gameBoard.NotCurrentPlayer));

        //var test = new BoardTurnInfo(gameBoard.Board, gameBoard.CurrentPlayer);
        //test.Print();
        //test.Move(new Vector2Int(4, 4), new Vector2Int(3, 3));
        //Debug.LogError("<---------------------------------------------------->");
        //test = test.Clone();
        //test.Print();
        //var tiles = test.GetAccessableTiles(new Vector2Int(4, 4), 3);
        //foreach (Vector2Int pos in tiles)
        //{
        //    Debug.Log(pos);
        //}


        int i = 0;
        //BoardTurnInfo BestMove = MaxTurn(gameBoard, new BoardTurnInfo(gameBoard.Board, gameBoard.CurrentPlayer), new Queue<BoardUnitBaseClass>(playerUnits), ref i);
        BoardTurnInfo BestMove = MiniMax(gameBoard, new BoardTurnInfo(gameBoard.Board, currentPlayer), ref i);
        //BoardTurnInfo BestMove = AlphaBeta(gameBoard, new BoardTurnInfo(gameBoard.Board, currentPlayer), ref i, MIN, MAX, true, 3);
        Debug.Log(i);
        Debug.Log(BestMove.turnValue);
        Debug.Log(BestMove.moves.Count);
        Debug.Log(BestMove.abilities.Count);
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

    BoardTurnInfo MiniMax(GameBoard gameBoard, BoardTurnInfo state, ref int count, Queue<TileTurnInfo> unitList = null, bool maximizing = true, int depth = 3, bool unchanged = true)
    {
        count++;
        if (depth == 0)
        {
            return state;
        }

        BoardTurnInfo bestState = state;
        if (unitList == null || unitList.Count == 0)
        {
            unitList = maximizing ? state.GetUnitTiles(TargetType.Friend) : state.GetUnitTiles(TargetType.Foe);
        }

        if (unitList.Count > 0)
        {
            TileTurnInfo unit = unitList.Dequeue();
            depth = unitList.Count > 0 ? depth : depth--;
            bool nextMaximizing = unitList.Count > 0 ? maximizing : !maximizing;
            Stack<Vector2Int> accessableTiles = state.GetAccessableTiles(unit.boardPosition, unit.speed);
            while (accessableTiles.Count > 0)
            {
                Vector2Int pos = accessableTiles.Pop();
                Ability ability = unit.abilities[0];
                List<BoardTile> targetableTiles = ability.GetLinearTiles(gameBoard, pos, ability.movesThroughUnits);
                foreach (BoardTile tile in targetableTiles)
                {
                    BoardTurnInfo stateClone = state.Clone();
                    stateClone.Move(unit.boardPosition, pos);
                    stateClone.DamageTile(tile.BoardPosition, ability);

                    stateClone = MiniMax(gameBoard, stateClone, ref count, new Queue<TileTurnInfo>(unitList), nextMaximizing, depth);
                    bestState = maximizing ?
                         (stateClone.turnValue >= bestState.turnValue || unchanged ? stateClone : bestState)
                         :
                         (stateClone.turnValue <= bestState.turnValue || unchanged ? stateClone : bestState);
                    unchanged = false;
                }
            }
        }
        return bestState;
    }

    //BoardTurnInfo MiniMax(GameBoard gameBoard, BoardTurnInfo state, ref int count, bool maximizing = true, int depth = 3, bool unchanged = true)
    //{
    //    count++;
    //    if (depth == 0)
    //    {
    //        return state;
    //    }
    //    depth--;

    //    BoardTurnInfo bestState = state;
    //    Queue<TileTurnInfo> unitList = maximizing ? state.GetUnitTiles(TargetType.Friend) : state.GetUnitTiles(TargetType.Foe);
    //    if (unitList.Count > 0)
    //    {
    //        TileTurnInfo unit = unitList.Dequeue();
    //        Stack<Vector2Int> accessableTiles = state.GetAccessableTiles(unit.boardPosition, unit.speed);
    //        while (accessableTiles.Count > 0)
    //        {
    //            Vector2Int pos = accessableTiles.Pop();
    //            Ability ability = unit.abilities[0];
    //            List<BoardTile> targetableTiles = ability.GetLinearTiles(gameBoard, pos, ability.movesThroughUnits);
    //            foreach (BoardTile tile in targetableTiles)
    //            {
    //                BoardTurnInfo stateClone = state.Clone();
    //                stateClone.Move(unit.boardPosition, pos);
    //                stateClone.DamageTile(tile.BoardPosition, ability);

    //                stateClone = MiniMax(gameBoard, stateClone, ref count, !maximizing, depth);
    //                bestState = maximizing ?
    //                     (stateClone.turnValue >= bestState.turnValue || unchanged ? stateClone : bestState)
    //                     :
    //                     (stateClone.turnValue <= bestState.turnValue || unchanged ? stateClone : bestState);
    //                unchanged = false;
    //            }
    //        }
    //    }
    //    return bestState;
    //}


    //BoardTurnInfo AlphaBeta(GameBoard gameBoard, BoardTurnInfo state, ref int count, bool maximizing = true, int depth = 3, bool unchanged = true)
    //{
    //    count++;
    //    if (depth == 0)
    //    {
    //        return state;
    //    }
    //    depth--;

    //    BoardTurnInfo bestState = state;
    //    List<TileTurnInfo> unitList = maximizing ? state.GetUnitTiles(TargetType.Friend) : state.GetUnitTiles(TargetType.Foe);
    //    if (unitList.Count > 0)
    //    {
    //        TileTurnInfo unit = unitList[0];
    //        Stack<Vector2Int> accessableTiles = state.GetAccessableTiles(unit.boardPosition, unit.speed);
    //        while (accessableTiles.Count > 0)
    //        {
    //            Vector2Int pos = accessableTiles.Pop();
    //            Ability ability = unit.abilities[0];
    //            List<BoardTile> targetableTiles = ability.GetLinearTiles(gameBoard, pos, ability.movesThroughUnits);
    //            foreach (BoardTile tile in targetableTiles)
    //            {
    //                BoardTurnInfo stateClone = state.Clone();
    //                stateClone.Move(unit.boardPosition, pos);
    //                stateClone.DamageTile(tile.BoardPosition, ability);

    //                stateClone = MiniMax(gameBoard, stateClone, ref count, !maximizing, depth);
    //                bestState = maximizing ?
    //                     (stateClone.turnValue >= bestState.turnValue || unchanged ? stateClone : bestState)
    //                     :
    //                     (stateClone.turnValue <= bestState.turnValue || unchanged ? stateClone : bestState);
    //                unchanged = false;
    //            }
    //        }
    //    }
    //    return bestState;
    //}

    //BoardTurnInfo AlphaBeta(GameBoard gameBoard, BoardTurnInfo state, ref int count, int alpha, int beta, bool maximizing = true, int depth = 3, bool unchanged = true)
    //{
    //    count++;
    //    if (depth == 0)
    //    {
    //        return state;
    //    }
    //    depth--;
    //    if (maximizing)
    //    {
    //        BoardTurnInfo bestState = state;
    //        List<TileTurnInfo> unitList = state.GetUnitTiles(TargetType.Friend);
    //        if (unitList.Count > 0)
    //        {
    //            TileTurnInfo unit = unitList[0];
    //            //Debug.Log(unit.boardPosition + " " + unit.speed);
    //            Stack<Vector2Int> accessableTiles = state.GetAccessableTiles(unit.boardPosition, unit.speed);
    //            while (accessableTiles.Count > 0)
    //            {
    //                Vector2Int pos = accessableTiles.Pop();
    //                // Debug.Log(unit.boardPosition + "  " + pos);
    //                Ability ability = unit.abilities[0];
    //                List<BoardTile> targetableTiles = ability.GetLinearTiles(gameBoard, pos, ability.movesThroughUnits);
    //                foreach (BoardTile tile in targetableTiles)
    //                {
    //                    BoardTurnInfo stateClone = state.Clone();
    //                    stateClone.Move(unit.boardPosition, pos, false);
    //                    stateClone.DamageTile(tile.BoardPosition, ability);
    //                    stateClone = AlphaBeta(gameBoard, stateClone, ref count, alpha, beta, false, depth);

    //                    bestState = stateClone.turnValue >= bestState.turnValue || unchanged ? stateClone : bestState;
    //                    alpha = Mathf.Max(alpha, bestState.turnValue);
    //                    unchanged = false;
    //                    if (alpha >= beta)
    //                    {
    //                        break;
    //                    }
    //                }
    //            }
    //        }
    //        return bestState;
    //    }
    //    else
    //    {
    //        BoardTurnInfo bestState = state;
    //        List<TileTurnInfo> unitList = state.GetUnitTiles(TargetType.Foe);
    //        if (unitList.Count > 0)
    //        {
    //            TileTurnInfo unit = unitList[0];
    //            Stack<Vector2Int> accessableTiles = state.GetAccessableTiles(unit.boardPosition, unit.speed);
    //            while (accessableTiles.Count > 0)
    //            {
    //                Vector2Int pos = accessableTiles.Pop();
    //                Ability ability = unit.abilities[0];
    //                List<BoardTile> targetableTiles = ability.GetLinearTiles(gameBoard, pos, ability.movesThroughUnits);
    //                foreach (BoardTile tile in targetableTiles)
    //                {
    //                    BoardTurnInfo stateClone = state.Clone();
    //                    stateClone.Move(unit.boardPosition, pos);
    //                    stateClone.DamageTile(tile.BoardPosition, ability);
    //                    stateClone = AlphaBeta(gameBoard, stateClone, ref count, alpha, beta, true, depth);

    //                    bestState = stateClone.turnValue <= bestState.turnValue || unchanged ? stateClone : bestState;
    //                    beta = Mathf.Min(beta, bestState.turnValue);
    //                    unchanged = false;
    //                    if (beta <= alpha)
    //                    {
    //                        break;
    //                    }
    //                }
    //            }
    //        }
    //        return bestState;
    //    }
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
            Debug.Log(move.from);
            Debug.Log(move.to);
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


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

    const int MAX = 99999;
    const int MIN = -99999;
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
        //BoardTurnInfo BestMove = MiniMax(gameBoard, new BoardTurnInfo(gameBoard.Board, currentPlayer), ref i);
        BoardTurnInfo BestMove = AplhaBeta(gameBoard, new BoardTurnInfo(gameBoard.Board, currentPlayer), ref i);
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

    BoardTurnInfo MiniMax(GameBoard gameBoard, BoardTurnInfo state, ref int count, int depth = 3, Queue<TileTurnInfo> unitList = null, bool maximizing = true, bool unchanged = true)
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
            depth = unitList.Count > 0 ? depth : --depth;
            bool nextMaximizing = unitList.Count > 0 ? maximizing : !maximizing;
            Stack<Vector2Int> accessableTiles = state.GetAccessableTiles(unit.boardPosition, unit.speed);
            while (accessableTiles.Count > 0)
            {
                Vector2Int pos = accessableTiles.Pop();
                Ability ability = unit.abilities[0];
                List<BoardTile> targetableTiles = ability.GetLinearTiles(gameBoard, pos, ability.movesThroughUnits);
                bool noTargetStateFound = false;
                foreach (BoardTile tile in targetableTiles)
                {
                    BoardTurnInfo stateClone = state.Clone();
                    stateClone.Move(unit.boardPosition, pos);
                    bool hit = stateClone.DamageTile(tile.BoardPosition, ability);
                    if (hit || noTargetStateFound == false)
                    {
                        stateClone = MiniMax(gameBoard, stateClone, ref count, depth, new Queue<TileTurnInfo>(unitList), nextMaximizing);
                        bestState = maximizing ?
                             (stateClone.turnValue >= bestState.turnValue || unchanged ? stateClone : bestState)
                             :
                             (stateClone.turnValue <= bestState.turnValue || unchanged ? stateClone : bestState);
                        unchanged = false;
                    }
                    noTargetStateFound = noTargetStateFound ? true : !hit;
                }
            }
        }
        return bestState;
    }
    BoardTurnInfo AplhaBeta(GameBoard gameBoard, BoardTurnInfo state, ref int count, int depth = 3, int alpha = MIN, int beta = MAX, bool maximizing = true, Queue<TileTurnInfo> unitList = null)
    {
        count++;
        if (depth == 0)
        {
            return state;
        }
        if (unitList == null || unitList.Count == 0)
        {
            unitList = maximizing ? state.GetUnitTiles(TargetType.Friend) : state.GetUnitTiles(TargetType.Foe);
        }
        BoardTurnInfo bestState = state;
        if (unitList.Count > 0)
        {
            TileTurnInfo unit = unitList.Dequeue();
            int nextDepth = unitList.Count > 0 ? depth : depth - 1;
            bool nextMax = unitList.Count > 0 ? maximizing : !maximizing;
            if (maximizing)
            {
                bestState.TempValue = MIN;
                foreach (Vector2Int pos in state.GetAccessableTiles(unit.boardPosition, unit.speed))
                {
                    Ability ability = unit.abilities[0];
                    bool repeatableState = false;
                    foreach (BoardTile targetTile in ability.GetLinearTiles(gameBoard, pos, ability.movesThroughUnits))
                    {
                        BoardTurnInfo stateClone = state.Clone();
                        stateClone.Move(unit.boardPosition, pos);
                        bool hit = stateClone.DamageTile(targetTile.BoardPosition, ability);
                        if (hit || repeatableState)
                        {

                            stateClone = AplhaBeta(gameBoard, stateClone, ref count, nextDepth, alpha, beta, nextMax, new Queue<TileTurnInfo>(unitList));
                            if (stateClone.turnValue > bestState.TempValue)
                            {
                                bestState = stateClone;
                                bestState.TempValue = stateClone.turnValue;
                            }
                            alpha = Mathf.Max(alpha, bestState.TempValue);
                            if (beta <= alpha)
                            {
                                return bestState;
                            }
                        }
                        repeatableState = repeatableState ? repeatableState : !hit;
                    }
                }
            }
            else
            {
                bestState.TempValue = MAX;
                foreach (Vector2Int pos in state.GetAccessableTiles(unit.boardPosition, unit.speed))
                {
                    Ability ability = unit.abilities[0];
                    bool repeatableState = false;
                    foreach (BoardTile targetTile in ability.GetLinearTiles(gameBoard, pos, ability.movesThroughUnits))
                    {
                        BoardTurnInfo stateClone = state.Clone();
                        stateClone.Move(unit.boardPosition, pos);
                        bool hit = stateClone.DamageTile(targetTile.BoardPosition, ability);
                        if (hit || repeatableState)
                        {
                            stateClone = AplhaBeta(gameBoard, stateClone, ref count, nextDepth, alpha, beta, nextMax, new Queue<TileTurnInfo>(unitList));
                            if (stateClone.turnValue < bestState.TempValue)
                            {
                                bestState = stateClone;
                                bestState.TempValue = stateClone.turnValue;
                            }
                            beta = Mathf.Min(beta, bestState.TempValue);
                            if (beta <= alpha)
                            {
                                return bestState;
                            }
                        }
                        repeatableState = repeatableState ? repeatableState : !hit;
                    }
                }
            }
        }
        return bestState;
    }
    //BoardTurnInfo AplhaBeta(GameBoard gameBoard, BoardTurnInfo state, ref int count, int depth = 3, int alpha = MIN, int beta = MAX, bool maximizing = true, Queue<TileTurnInfo> unitList = null)
    //{
    //    count++;
    //    if (depth == 0)
    //    {
    //        return state;
    //    }
    //    if (unitList == null || unitList.Count == 0)
    //    {
    //        unitList = maximizing ? state.GetUnitTiles(TargetType.Friend) : state.GetUnitTiles(TargetType.Foe);
    //    }
    //    BoardTurnInfo bestState = state;
    //    if (unitList.Count > 0)
    //    {
    //        TileTurnInfo unit = unitList.Dequeue();
    //        int nextDepth = unitList.Count > 0 ? depth : depth - 1;
    //        bool nextMax = unitList.Count > 0 ? maximizing : !maximizing;
    //        if (maximizing)
    //        {
    //            bestState.TempValue = MIN;
    //            foreach (Vector2Int pos in state.GetAccessableTiles(unit.boardPosition, unit.speed))
    //            {
    //                Ability ability = unit.abilities[0];
    //                foreach (BoardTile targetTile in ability.GetLinearTiles(gameBoard, pos, ability.movesThroughUnits))
    //                {
    //                    BoardTurnInfo stateClone = state.Clone();
    //                    stateClone.Move(unit.boardPosition, pos);
    //                    stateClone.DamageTile(targetTile.BoardPosition, ability);
    //                    stateClone = AplhaBeta(gameBoard, stateClone, ref count, nextDepth, alpha, beta, nextMax, new Queue<TileTurnInfo>(unitList));
    //                    if (stateClone.turnValue > bestState.TempValue)
    //                    {
    //                        bestState = stateClone;
    //                        bestState.TempValue = stateClone.turnValue;
    //                    }
    //                    alpha = Mathf.Max(alpha, bestState.TempValue);
    //                    if (beta <= alpha)
    //                    {
    //                        return bestState;
    //                    }
    //                }
    //            }
    //        }
    //        else
    //        {
    //            bestState.TempValue = MAX;
    //            foreach (Vector2Int pos in state.GetAccessableTiles(unit.boardPosition, unit.speed))
    //            {
    //                Ability ability = unit.abilities[0];
    //                foreach (BoardTile targetTile in ability.GetLinearTiles(gameBoard, pos, ability.movesThroughUnits))
    //                {
    //                    BoardTurnInfo stateClone = state.Clone();
    //                    stateClone.Move(unit.boardPosition, pos);
    //                    stateClone.DamageTile(targetTile.BoardPosition, ability);
    //                    stateClone = AplhaBeta(gameBoard, stateClone, ref count, nextDepth, alpha, beta, nextMax, new Queue<TileTurnInfo>(unitList));
    //                    if (stateClone.turnValue < bestState.TempValue)
    //                    {
    //                        bestState = stateClone;
    //                        bestState.TempValue = stateClone.turnValue;
    //                    }
    //                    beta = Mathf.Min(beta, bestState.TempValue);
    //                    if (beta <= alpha)
    //                    {
    //                        return bestState;
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    return bestState;
    //}

    //BoardTurnInfo AlphaBeta(GameBoard gameBoard, BoardTurnInfo state, ref int count, int depth = 3, int alpha = MIN, int beta = MAX, Queue<TileTurnInfo> unitList = null, bool maximizing = true, bool unchanged = true)
    //{

    //    int a = alpha;
    //    int b = beta;
    //    count++;
    //    if (depth == 0)
    //    {
    //        return state;
    //    }

    //    BoardTurnInfo bestState = state;
    //    if (unitList == null || unitList.Count == 0)
    //    {
    //        unitList = maximizing ? state.GetUnitTiles(TargetType.Friend) : state.GetUnitTiles(TargetType.Foe);
    //    }

    //    if (unitList.Count > 0)
    //    {
    //        TileTurnInfo unit = unitList.Dequeue();
    //        //Round itteration
    //        depth = unitList.Count > 0 ? depth : --depth;
    //        bool nextMaximizing = unitList.Count > 0 ? maximizing : !maximizing;
    //        //int value = maximizing ? MIN : MAX;
    //        //Get Movement
    //        Stack<Vector2Int> accessableTiles = state.GetAccessableTiles(unit.boardPosition, unit.speed);
    //        while (accessableTiles.Count > 0)
    //        {

    //            Vector2Int pos = accessableTiles.Pop();
    //            Ability ability = unit.abilities[0];
    //            List<BoardTile> targetableTiles = ability.GetLinearTiles(gameBoard, pos, ability.movesThroughUnits);
    //            bool noTargetStateFound = false;
    //            foreach (BoardTile tile in targetableTiles)
    //            {
    //                BoardTurnInfo stateClone = state.Clone();
    //                stateClone.Move(unit.boardPosition, pos);
    //                bool hit = stateClone.DamageTile(tile.BoardPosition, ability);

    //                if (hit || noTargetStateFound == false)
    //                {
    //                    stateClone = AlphaBeta(gameBoard, stateClone, ref count, depth, a, b, new Queue<TileTurnInfo>(unitList), nextMaximizing);
    //                    unchanged = false;
    //                    //Debug.Log(depth + 1 + (maximizing ? " max: " : " min: ") + "Beta:" + b + " Alpha:" + a);
    //                    if (maximizing)
    //                    {
    //                        if (stateClone.turnValue >= bestState.turnValue)
    //                        {
    //                            bestState = stateClone;
    //                            //value = stateClone.turnValue;
    //                        }
    //                        a = System.Math.Max(a, stateClone.turnValue);
    //                        if (b < a)
    //                        {
    //                            //Debug.Log(b + "<=" + a);
    //                            return bestState;
    //                        }
    //                    }
    //                    else
    //                    {
    //                        if (stateClone.turnValue >= bestState.turnValue)
    //                        {
    //                            bestState = stateClone;
    //                            //value = stateClone.turnValue;
    //                        }
    //                        b = System.Math.Min(b, stateClone.turnValue);
    //                        if (b < a)
    //                        {
    //                            //Debug.Log(b + "<=" + a);
    //                            return bestState;
    //                        }
    //                    }
    //                }
    //                noTargetStateFound = noTargetStateFound ? true : !hit;
    //            }

    //        }

    //    }
    //    return bestState;
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
            //Debug.Log(move.from);
            //Debug.Log(move.to);
            //Debug.Log(turnInfo.abilities.Peek());
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


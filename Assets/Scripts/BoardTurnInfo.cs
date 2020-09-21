using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public enum TargetType { Empty, Friend, Foe, Envirornment };
public class BoardTurnInfo
{
    const int ENEMY_MULTIPLIER = 50;
    const int ENVIRORNMENT_MULTIPLIER = 10;
    const int FRIEND_MULTIPLIER = -49;
    const int KILL_MULTIPLIER = 30;


    TileTurnInfo[,] boardInfo;
    public Queue<(Vector2Int from, Vector2Int to)> moves { get; private set; }
    public Queue<(Ability ability, Vector2Int pos)> abilities { get; private set; }
    public int turnValue { get; private set; }
    public int TempValue;
    public BoardTurnInfo(TileTurnInfo[,] board, int value, Queue<(Vector2Int from, Vector2Int to)> moveQueue, Queue<(Ability ability, Vector2Int pos)> abilityQueue)
    {
        boardInfo = board;
        turnValue = value;
        moves = new Queue<(Vector2Int from, Vector2Int to)>(moveQueue);
        abilities = new Queue<(Ability ability, Vector2Int pos)>(abilityQueue);
    }
    public BoardTurnInfo(BoardTile[,] board, Player player)
    {
        int xSize = board.GetLength(0);
        int ySize = board.GetLength(1);
        boardInfo = new TileTurnInfo[xSize, ySize];

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (board[x, y].Occupied())
                {
                    if (board[x, y].GetUnit.GetType() == typeof(BoardUnit))
                    {
                        boardInfo[x, y] = new TileTurnInfo
                        {
                            target = board[x, y].GetUnit.OwningPlayer == player ? TargetType.Friend : TargetType.Foe,
                            health = board[x, y].GetUnit.CurrentHealth,
                            speed = board[x, y].GetUnit.MaxMovement,
                            boardPosition = new Vector2Int(x, y),
                            abilities = board[x, y].GetUnit.Abilites
                        };
                    }
                    else
                    {
                        boardInfo[x, y] = new TileTurnInfo
                        {
                            target = TargetType.Envirornment,
                            health = board[x, y].GetUnit.CurrentHealth,
                        };
                    }
                }
            }
        }
        moves = new Queue<(Vector2Int from, Vector2Int to)>();
        abilities = new Queue<(Ability ability, Vector2Int pos)>();
    }

    public BoardTurnInfo Clone()
    {
        //int xSize = boardInfo.GetLength(0);
        //int ySize = boardInfo.GetLength(1);
        //TileTurnInfo[,] board = new TileTurnInfo[xSize, ySize];
        //int byteSize = System.Buffer.ByteLength(boardInfo);
        //System.Buffer.BlockCopy(boardInfo, 0, board, 0, byteSize);
        return new BoardTurnInfo(boardInfo.Clone() as TileTurnInfo[,], turnValue, moves, abilities);
    }
    public void Move(Vector2Int from, Vector2Int to, bool negative = false)
    {
        moves.Enqueue((from, to));
        if (from != to)
        {
            boardInfo[to.x, to.y] = boardInfo[from.x, from.y];
            boardInfo[to.x, to.y].boardPosition = to;
            boardInfo[from.x, from.y] = new TileTurnInfo();
        }
        //turnValue += negative ? -(int)Vector2Int.Distance(from, to) : (int)Vector2Int.Distance(from, to);
    }
    public bool DamageTile(Vector2Int target, Ability ability)
    {
        abilities.Enqueue((ability, target));
        bool empty = boardInfo[target.x, target.y].target == TargetType.Empty;
        boardInfo[target.x, target.y].health -= ability.Damage;
        int enemyMultiplier = targetMultiplier(target.x, target.y);
        bool killed = boardInfo[target.x, target.y].health <= 0;
        int killMultiplier = killed ? KILL_MULTIPLIER : 1;
        int damageDone = killed ? ability.Damage + boardInfo[target.x, target.y].health : ability.Damage;
        turnValue += damageDone * enemyMultiplier * killMultiplier;
        //if (boardInfo[target.x, target.y].target != TargetType.Empty)
        //    Debug.Log(boardInfo[target.x, target.y].target.ToString() + " " + damageDone * enemyMultiplier * killMultiplier);
        if (killed)
        {
            boardInfo[target.x, target.y] = new TileTurnInfo { health = 0, target = TargetType.Empty };
        }
        return !empty;
    }

    public Queue<TileTurnInfo> GetUnitTiles(TargetType type)
    {
        Queue<TileTurnInfo> queue = new Queue<TileTurnInfo>();
        foreach (TileTurnInfo tile in boardInfo)
        {
            if (tile.target == type)
            {
                queue.Enqueue(tile);
            }
        }
        //Debug.Log(list.Count);
        return queue;
    }

    int targetMultiplier(int x, int y)
    {
        switch (boardInfo[x, y].target)
        {
            case TargetType.Friend:
                return FRIEND_MULTIPLIER;
            case TargetType.Foe:
                return ENEMY_MULTIPLIER;
            case TargetType.Envirornment:
                return ENVIRORNMENT_MULTIPLIER;
            default: return 1;
        }
    }

    public Stack<Vector2Int> GetAccessableTiles(Vector2Int startPos, int maxDistance)
    {
        // currently loops through already visiste positions, but It's fast enough for this right now
        Queue<Vector2Int> openQueue = new Queue<Vector2Int>();
        Stack<Vector2Int> closedStack = new Stack<Vector2Int>();
        openQueue.Enqueue(startPos);
        Vector2Int[] offsetPosition = new Vector2Int[] { new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1) };
        int breadth = openQueue.Count;
        int distanceTraveled = 0;
        while (openQueue.Count > 0)
        {
            Vector2Int pos = openQueue.Dequeue();
            closedStack.Push(pos);
            foreach (Vector2Int offset in offsetPosition)
            {
                Vector2Int offsetPos = pos + offset;
                if (offsetPos.x < 0 || offsetPos.x >= boardInfo.GetLength(0)
                    || offsetPos.y < 0 || offsetPos.y >= boardInfo.GetLength(1)
                    || openQueue.Contains(offsetPos)
                    || closedStack.Contains(offsetPos))
                {
                    continue;
                }
                if (boardInfo[offsetPos.x, offsetPos.y].target == TargetType.Empty)
                {
                    openQueue.Enqueue(offsetPos);
                }
            }
            breadth--;
            if (breadth == 0)
            {
                distanceTraveled++;
                if (distanceTraveled > maxDistance)
                {
                    return closedStack;
                }
                breadth = openQueue.Count;
            }
        }
        return closedStack;
    }

    public void Print()
    {
        for (int y = boardInfo.GetLength(1) - 1; y >= 0; y--)
        {
            string s = "";
            for (int x = 0; x < boardInfo.GetLength(0); x++)
            {
                s += boardInfo[x, y].target == TargetType.Empty ? "null, " : boardInfo[x, y].target.ToString() + ", ";
            }
            Debug.Log(s);
        }
    }
}

public struct TileTurnInfo
{
    public TargetType target;
    public int health;
    public int speed;
    public Vector2Int boardPosition;
    public List<Ability> abilities;
}
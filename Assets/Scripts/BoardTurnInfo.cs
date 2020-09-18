using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public enum TargetType { Empty, Friend, Foe, Envirornment };
public class BoardTurnInfo
{
    const int ENEMY_MULTIPLIER = 5;
    const int KILL_MULTIPLIER = 3;


    TileTurnInfo[,] boardInfo;
    public Queue<(Vector2Int from, Vector2Int to)> moves { get; private set; }
    public Queue<(Ability ability, Vector2Int pos)> abilities { get; private set; }

    public int turnValue { get; private set; }
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
                            foe = board[x, y].GetUnit.OwningPlayer == player ? TargetType.Friend : TargetType.Foe,
                            health = board[x, y].GetUnit.CurrentHealth,
                            //abilities = board[x, y].GetUnit.Abilites
                        };
                    }
                    else
                    {
                        boardInfo[x, y] = new TileTurnInfo
                        {
                            foe = TargetType.Envirornment,
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
    public void Move(Vector2Int from, Vector2Int to)
    {
        moves.Enqueue((from, to));
        boardInfo[to.x, to.y] = boardInfo[from.x, from.y];
        boardInfo[from.x, from.y] = new TileTurnInfo();
        turnValue++;
    }
    public void DamageTile(Vector2Int target, Ability ability)
    {
        abilities.Enqueue((ability, target));
        boardInfo[target.x, target.y].health -= ability.Damage;
        int enemyMultiplier =
            (boardInfo[target.x, target.y].foe == TargetType.Foe) ? ENEMY_MULTIPLIER
                : (boardInfo[target.x, target.y].foe == TargetType.Envirornment) ? 1 : (boardInfo[target.x, target.y].foe == TargetType.Friend) ? -2 : 0;
        bool killed = boardInfo[target.x, target.y].health <= 0;
        int killMultiplier = killed ? KILL_MULTIPLIER : 1;
        int damageDone = killed ? ability.Damage + boardInfo[target.x, target.y].health : ability.Damage;
        turnValue += damageDone * enemyMultiplier * killMultiplier;
        if (killed)
        {
            boardInfo[target.x, target.y] = new TileTurnInfo { health = 0, foe = TargetType.Empty };
        }
    }

    int targetMultiplier(int x, int y)
    {
        switch (boardInfo[x, y].foe)
        {
            case TargetType.Friend:
                return-2;
            case TargetType.Foe:
                return ENEMY_MULTIPLIER;
            case TargetType.Envirornment:
                return 1;
            default: return 0;
        }
    }

    public void Print()
    {
        for (int y = boardInfo.GetLength(1) - 1; y >= 0; y--)
        {
            string s = "";
            for (int x = 0; x < boardInfo.GetLength(0); x++)
            {
                s += boardInfo[x, y].foe == TargetType.Empty ? "null, " : boardInfo[x, y].health + ", ";
            }
            Debug.Log(s);
        }
    }
}

public struct TileTurnInfo
{
    public TargetType foe;
    public int health;
    //public int speed;
    //public List<Ability> abilities;
}
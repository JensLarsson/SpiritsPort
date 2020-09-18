using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PathFinding
{
    static Vector2Int[] rotationArray = new Vector2Int[]
{
        new Vector2Int(0,1),
        new Vector2Int(1,0),
        new Vector2Int(0,-1),
        new Vector2Int(-1,0),
};

    public static bool[,] DijkstraPath(int[,] accessableTiles, int maxDistance, Vector2Int startPos, bool movesThroughUnits = false)
    {
        // currently loops through already visiste positions, but It's fast enough for this right now
        Queue<Vector2Int> positionsToCheck = new Queue<Vector2Int>();
        positionsToCheck.Enqueue(startPos);
        Vector2Int[] offsetPosition = new Vector2Int[] { new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1), };
        bool[,] InRangeTiles = new bool[accessableTiles.GetLength(0), accessableTiles.GetLength(1)];
        int breadth = positionsToCheck.Count;
        int distanceTraveled = 0;
        while (positionsToCheck.Count > 0)
        {
            Vector2Int pos = positionsToCheck.Dequeue();
            foreach (Vector2Int offset in offsetPosition)
            {
                Vector2Int offsetPos = pos + offset;
                if (offsetPos.x >= 0 && offsetPos.x < accessableTiles.GetLength(0)
                    && offsetPos.y >= 0 && offsetPos.y < accessableTiles.GetLength(1))
                {
                    if (accessableTiles[offsetPos.x, offsetPos.y] < 9000)
                    {
                        InRangeTiles[offsetPos.x, offsetPos.y] = true;
                        positionsToCheck.Enqueue(offsetPos);
                    }
                    else if (movesThroughUnits)
                    {
                        positionsToCheck.Enqueue(offsetPos);
                    }
                }
            }
            breadth--;
            if (breadth == 0)
            {
                breadth = positionsToCheck.Count;
                distanceTraveled++;
                if (distanceTraveled >= maxDistance)
                {
                    break;
                }
            }
        }
        return InRangeTiles;
    }

    public static List<WeightedTile> aStar(int[,] accessableTiles, Vector2Int startPos, List<Vector2Int> targets, int maxMoves = 99, bool movesThroughUnits = false)
    {
        Vector2Int target = targets[0];
        List<WeightedTile> openList = new List<WeightedTile>()
        {
            new WeightedTile
            {
                pos = startPos,
                weight = 0,
                distFromTarget = Mathf.Abs( startPos.x-target.x)+ Mathf.Abs(startPos.y-target.y)
            }
        };
        List<WeightedTile> closedList = new List<WeightedTile>();
        int test = 0;
        while (openList.Count > 0)
        {
            test = Math.Max(test, openList.Count);
            int lightest = GetLightestTile(openList);
            WeightedTile tile = openList[lightest];
            openList.RemoveAt(lightest);
            closedList.Add(tile);
            foreach (Vector2Int direction in rotationArray)
            {
                Vector2Int newpos = tile.pos + direction;
                if (newpos.x < 0 || newpos.y < 0 
                    || newpos.x >= accessableTiles.GetLength(0) || newpos.y >= accessableTiles.GetLength(1)
                    || ListContains(openList, newpos) > -1)
                {
                    break;
                }
                int i = ListContains(closedList, newpos);
                if (i > -1 && closedList[i].weight < tile.weight + 1)
                {
                    closedList[i].weight = tile.weight + 1;
                    break;
                }
                WeightedTile newTile = new WeightedTile
                {
                    pos = newpos,
                    weight = tile.weight + 1,
                    distFromTarget = Mathf.Abs(tile.pos.x - target.x) + Mathf.Abs(tile.pos.y - target.y)
                };
                if (target == (newTile.pos))
                {
                    closedList.Add(newTile);
                    closedList.Add(tile);
                    return closedList;
                }
                if (newTile.weight < maxMoves)
                {
                    openList.Add(newTile);
                }
                else if (newTile.weight == maxMoves)
                {
                    closedList.Add(newTile);
                }
            }
        }
        return closedList;
    }

    static int ListContains(List<WeightedTile> list, Vector2Int target)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].pos == target) return i;
        }
        return -1;
    }
    static int GetLightestTile(List<WeightedTile> tiles)
    {
        int lightest = 0;
        //WeightedTile tile = tiles[lightest];
        for (int i = 1; i < tiles.Count; i++)
        {
            if (tiles[i].distFromTarget < tiles[lightest].distFromTarget)
            {
                lightest = i;
                //tile = tiles[i];
            }
        }
        //tiles.RemoveAt(lightest);
        return lightest;
    }
}
public class WeightedTile
{
    public Vector2Int pos;
    public int weight;
    public int distFromTarget;
}
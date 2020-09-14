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

    public static List<WeightedTile> aStar( int[,] accessableTiles, Vector2Int startPos, List<Vector2Int> targets, bool movesThroughUnits = false)
    {
        Vector2Int target = ClosestPosition(startPos, targets);
        Debug.Log(target);
        List<WeightedTile> openList = new List<WeightedTile>()
        {
            new WeightedTile
            {
                pos = startPos,
                traveled = 0,
                distFromTarget = Mathf.Abs( startPos.x-target.x)+ Mathf.Abs(startPos.y-target.y)
            }
        };
        List<WeightedTile> closedList = new List<WeightedTile>();

        while (openList.Count > 0)
        {
            int lightest = GetLightestTile(openList);
            WeightedTile tile = openList[lightest];
            foreach (Vector2Int direction in rotationArray)
            {
                Vector2Int newPos = tile.pos + direction;
                //Skip if outside board, in ClosedList, or in Open List
                if (newPos.x < 0 || newPos.y < 0 || newPos.x >= accessableTiles.GetLength(0) || newPos.y >= accessableTiles.GetLength(1))
                {
                    continue;
                }
                if (ListContains(openList, newPos) > -1)
                {
                    continue;
                }
                int i = ListContains(closedList, newPos);
                if (i > -1)
                {
                    if (closedList[i].traveled > tile.traveled + accessableTiles[tile.pos.x, tile.pos.y])
                    {
                        closedList[i].traveled = tile.traveled + accessableTiles[tile.pos.x, tile.pos.y];
                    }
                    continue;
                }

                WeightedTile newTile = new WeightedTile
                {
                    pos = newPos,
                    traveled = tile.traveled + accessableTiles[newPos.x, newPos.y],
                    distFromTarget = Mathf.Abs(newPos.x - target.x) + Mathf.Abs(newPos.y - target.y)
                };
                //If at the closest enemy
                if (target == (newTile.pos))
                {
                    closedList.Add(newTile);
                    closedList.Add(tile);
                    return closedList;
                }
                openList.Add(newTile);
            }
            openList.RemoveAt(lightest);
            closedList.Add(tile);
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
            if (tiles[i].traveled + tiles[i].distFromTarget < tiles[lightest].traveled + tiles[lightest].distFromTarget)
            {
                lightest = i;
                //tile = tiles[i];
            }
        }
        //tiles.RemoveAt(lightest);
        return lightest;
    }
    static Vector2Int ClosestPosition(Vector2Int from, List<Vector2Int> to)
    {
        if (to.Count < 1)
        {
            return new Vector2Int(-1, -1);
        }
        Vector2Int closest = to[0];
        for (int i = 1; i < to.Count; i++)
        {
            if (Vector2Int.Distance(from, closest) > Vector2Int.Distance(from, to[i]))
            {
                closest = to[i];
            }
        }
        return closest;
    }
}
public class WeightedTile
{
    public Vector2Int pos;
    public int traveled;
    public int distFromTarget;
}
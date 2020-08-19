using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PathFinding
{
    public static bool[,] DijkstraPath(bool[,] accessableTiles, int maxDistance, Vector2Int startPos, bool movesThroughUnits = false)
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
                    if (accessableTiles[offsetPos.x, offsetPos.y])
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
}


using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder
{
   public List<Tile> FindPath(Tile start, Tile end)
    {
        List<Tile> openList = new List<Tile>();
        List<Tile> closeList = new List<Tile>();

        openList.Add(start);

        while(openList.Count > 0)
        {
            Tile currentTile = openList.OrderBy(x => x.f).First();

            openList.Remove(currentTile);
            closeList.Add(currentTile);

            if(currentTile == end)
            {
                //finalize path
                return GetFinishedList(start, end);
            }

            var neighbortiles = currentTile.GetNeighborTiles();

            foreach(var neighbor in neighbortiles)
            {
                if (neighbor == null || neighbor.isBlocked || closeList.Contains(neighbor)) continue;

                neighbor.g = GetManhattenDistance(start, neighbor);
                neighbor.h = GetManhattenDistance(end, neighbor);

                neighbor.previousTile = currentTile;

                if(!openList.Contains(neighbor)) openList.Add(neighbor);
            }
        }
        return new List<Tile>();
    }

    private List<Tile> GetFinishedList(Tile start, Tile end)
    {
        var finishedList = new List<Tile>();
        Tile currentTile = end;

        while(currentTile != start)
        {
            finishedList.Add(currentTile);
            currentTile = currentTile.previousTile;
        }

        finishedList.Reverse();
        return finishedList;
    }

    private int GetManhattenDistance(Tile start, Tile neighbor)
    {
        return (int)(Mathf.Abs(start.position.x - neighbor.position.x) + Mathf.Abs(start.position.y - neighbor.position.y));
    }
}

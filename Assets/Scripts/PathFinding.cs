using System.Collections.Generic;
using System;

public class PathFinding
{
    public static List<(int, int)> AStar(GameMap gameMap, int x, int y, int destX, int destY)
    {
        List<(int, int)> queue = new List<(int, int)>();
        queue.Add((x, y));

        Dictionary<(int, int), int> gScore = new Dictionary<(int, int), int>();
        Dictionary<(int, int), int> fScore = new Dictionary<(int, int), int>();
        Dictionary<(int, int), (int, int)> cameFrom = new Dictionary<(int, int), (int, int)>();

        gScore[(x, y)] = 0;
        fScore[(x, y)] = Heuristic(x, y, destX, destY);

        List<(int, int)> neighbours = new List<(int, int)>();

        while (queue.Count > 0)
        {
            (int, int) current = queue[0];
            queue.RemoveAt(0);
            if (current.Item1 == destX && current.Item2 == destY)
            {
                return ReconstructPath(cameFrom, current);
            }

            neighbours.Add((current.Item1, current.Item2 + 1));
            neighbours.Add((current.Item1 - 1, current.Item2));
            neighbours.Add((current.Item1, current.Item2 - 1));
            neighbours.Add((current.Item1 + 1, current.Item2));

            foreach ((int, int) neighbour in neighbours)
            {
                if (!gameMap.IsInBounds(neighbour.Item1, neighbour.Item2))
                {
                    continue;
                }

                Tile tile = gameMap.GetTileAtLocation(neighbour.Item1, neighbour.Item2);
                if (tile != null && tile.BlocksMovement)
                {
                    continue;
                }

                Tile actorTile = gameMap.GetActorAtLocation(neighbour.Item1, neighbour.Item2);
                if (actorTile != null)
                {
                    continue;
                }

                int tentativeGScore = GetDictValue(gScore, current) + 1;

                if (tentativeGScore < GetDictValue(gScore, neighbour))
                {
                    cameFrom[neighbour] = current;
                    gScore[neighbour] = tentativeGScore;
                    fScore[neighbour] = tentativeGScore + Heuristic(neighbour.Item1, neighbour.Item2, destX, destY);
                    if (!queue.Contains(neighbour))
                    {
                        queue.Add(neighbour);
                    }
                    queue.Sort((x, y) =>
                    {
                        int fX = GetDictValue(fScore, x);
                        int fY = GetDictValue(fScore, y);

                        if (fX < fY)
                        {
                            return -1;
                        }

                        if (fX == fY)
                        {
                            return 0;
                        }

                        return 1;
                    });
                }
            }
            neighbours.Clear();
        }

        return null;
    }

    private static int Heuristic(int x, int y, int destX, int destY)
    {
        return Math.Abs(destX - x) + Math.Abs(destY - y);
    }

    private static int GetDictValue(Dictionary<(int, int), int> dict, (int, int) key)
    {
        int value;
        if (!dict.TryGetValue(key, out value))
        {
            value = Int32.MaxValue;
        }

        return value;
    }

    private static List<(int, int)> ReconstructPath(Dictionary<(int, int), (int, int)> cameFrom, (int, int) current)
    {
        List<(int, int)> path = new List<(int, int)>();
        path.Add(current);

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }
}
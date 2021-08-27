using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BrogueRoom
{
    public List<List<char>> m_Room;
    public (int, int) m_TopDoor = (0, 0);
    public (int, int) m_LeftDoor = (0, 0);
    public (int, int) m_BottomDoor = (0, 0);
    public (int, int) m_RightDoor = (0, 0);
}

public class BrogueGen
{
    public static List<List<char>> GenerateDungeon(int width, int height)
    {
        List<List<char>> dungeon = new List<List<char>>();

        for (int i = 0; i < width; ++i)
        {
            List<char> column = new List<char>();

            for (int j = 0; j < height; ++j)
            {
                column.Add(' ');
            }

            dungeon.Add(column);
        }

        BrogueRoom room = GenerateRoom(true);
        MergeFirstRoom(dungeon, room);

        return dungeon;
    }

    private static BrogueRoom GenerateRoom(bool addDoors)
    {
        BrogueRoom room = new BrogueRoom();
        int r = Random.Range(0, 3);

        switch (r)
        {
            case 0:
                room.m_Room = GenerateRoomOverlappingRectangles();
                break;
            case 1:
                room.m_Room = GenerateRoomCellularAutomata();
                break;
            case 2:
                room.m_Room = GenerateRoomCircle();
                break;
        }

        if (addDoors)
        {
            AddDoors(room);
        }

        return room;
    }

    private static void AddDoors(BrogueRoom room)
    {
        // add top door
        RandomAccess(room.m_Room, (x, y, c) =>
        {
            if (c != '.')
            {
                return false;
            }

            int roomHeight = room.m_Room[0].Count;

            if (y + 1 >= roomHeight)
            {
                return false;
            }

            bool hasFloorAbove = false;
            for (int i = y + 1; i < roomHeight; ++i)
            {
                if (room.m_Room[x][i] == '.')
                {
                    hasFloorAbove = true;
                    break;
                }
            }

            if (hasFloorAbove)
            {
                return false;
            }

            if (GetRoomTile(room.m_Room, x - 1, y + 1) == '.' || GetRoomTile(room.m_Room, x + 1, y + 1) == '.')
            {
                return false;
            }

            room.m_Room[x][y + 1] = 'd';
            return true;
        });

        // add left door
        RandomAccess(room.m_Room, (x, y, c) =>
        {
            if (c != '.')
            {
                return false;
            }

            if (x - 1 < 0)
            {
                return false;
            }

            bool hasFloorToTheLeft = false;
            for (int i = 0; i < x; ++i)
            {
                if (room.m_Room[i][y] == '.')
                {
                    hasFloorToTheLeft = true;
                    break;
                }
            }

            if (hasFloorToTheLeft)
            {
                return false;
            }

            if (GetRoomTile(room.m_Room, x - 1, y + 1) == '.' || GetRoomTile(room.m_Room, x - 1, y - 1) == '.')
            {
                return false;
            }

            room.m_Room[x - 1][y] = 'd';
            return true;
        });

        // add bottom door
        RandomAccess(room.m_Room, (x, y, c) =>
        {
            if (c != '.')
            {
                return false;
            }

            if (y - 1 < 0)
            {
                return false;
            }

            bool hasFloorBelow = false;
            for (int i = 0; i < y; ++i)
            {
                if (room.m_Room[x][i] == '.')
                {
                    hasFloorBelow = true;
                    break;
                }
            }

            if (hasFloorBelow)
            {
                return false;
            }

            if (GetRoomTile(room.m_Room, x - 1, y - 1) == '.' || GetRoomTile(room.m_Room, x + 1, y - 1) == '.')
            {
                return false;
            }

            room.m_Room[x][y - 1] = 'd';
            return true;
        });

        // add right door
        RandomAccess(room.m_Room, (x, y, c) =>
        {
            if (c != '.')
            {
                return false;
            }

            int roomWidth = room.m_Room.Count;

            if (x + 1 >= roomWidth)
            {
                return false;
            }

            bool hasFloorToTheRight = false;
            for (int i = x + 1; i < roomWidth; ++i)
            {
                if (room.m_Room[i][y] == '.')
                {
                    hasFloorToTheRight = true;
                    break;
                }
            }

            if (hasFloorToTheRight)
            {
                return false;
            }

            if (GetRoomTile(room.m_Room, x + 1, y + 1) == '.' || GetRoomTile(room.m_Room, x + 1, y - 1) == '.')
            {
                return false;
            }

            room.m_Room[x + 1][y] = 'd';
            return true;
        });
    }

    private static List<List<char>> GenerateRoomOverlappingRectangles()
    {
        List<List<char>> room = new List<List<char>>();

        int maxWidth = 12;
        int maxHeight = 12;
        int maxRoomWidth = 10;
        int maxRoomHeight = 10;
        int minRoomWidth = 5;
        int minRoomHeight = 5;

        int room1Width = Random.Range(minRoomWidth, maxRoomWidth + 1);
        int room1Height = Random.Range(minRoomHeight, maxRoomHeight + 1);

        int room1X = Random.Range(1, maxWidth - room1Width - 1);
        int room1Y = Random.Range(1, maxHeight - room1Height - 1);

        for (int i = 0; i < maxWidth; ++i)
        {
            List<char> column = new List<char>();
            for (int j = 0; j < maxHeight; ++j)
            {
                column.Add('#');
            }
            room.Add(column);
        }

        Action<int, int, int, int> fill = (x, y, width, height) =>
        {
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    room[x + i][y + j] = '.';
                }
            }
        };

        fill(room1X, room1Y, room1Width, room1Height);

        int room1X2 = room1X + room1Width;
        int room1Y2 = room1Y + room1Height;

        while (true)
        {
            int room2Width = Random.Range(minRoomWidth, maxRoomWidth + 1);
            int room2Height = Random.Range(minRoomHeight, maxRoomHeight + 1);

            int room2X = Random.Range(1, maxWidth - room2Width - 1);
            int room2Y = Random.Range(1, maxHeight - room2Height - 1);

            int room2X2 = room2X + room2Width;
            int room2Y2 = room2Y + room2Height;

            bool intersect = room1X <= room2X2 &&
                room1X2 >= room2X &&
                room1Y <= room2Y2 &&
                room1Y2 >= room2Y;

            if (!intersect)
            {
                continue;
            }

            fill(room2X, room2Y, room2Width, room2Height);
            break;
        }

        return room;
    }

    private static List<List<char>> GenerateRoomCellularAutomata()
    {
        List<List<char>> room1 = new List<List<char>>();

        int maxSize = 9;
        int minSize = 8;
        int size = Random.Range(minSize, maxSize + 1);

        int listSize = size + 2;
        for (int i = 0; i < listSize; ++i)
        {
            List<char> column = new List<char>();

            for (int j = 0; j < listSize; ++j)
            {
                char c = Random.value > 0.5f ? '#' : '.';
                column.Add(c);
            }

            room1.Add(column);
        }

        List<List<char>> room2 = new List<List<char>>();
        for (int i = 0; i < room1.Count; ++i)
        {
            List<char> columnCopy = new List<char>();
            for (int j = 0; j < room1[i].Count; ++j)
            {
                columnCopy.Add(room1[i][j]);
            }
            room2.Add(columnCopy);
        }

        List<List<List<char>>> doubleBuffer = new List<List<List<char>>>();
        doubleBuffer.Add(room1);
        doubleBuffer.Add(room2);
        int front = 0;
        int back = 1;

        Func<int, int, int> testNeighbor = (x, y) =>
        {
            if (x < 0 || x >= listSize || y < 0 || y >= listSize)
            {
                return 0;
            }

            return doubleBuffer[back][x][y] == '.' ? 1 : 0;
        };

        int nbIterations = 5;
        for (int iteration = 0; iteration < nbIterations; ++iteration)
        {
            for (int i = 0; i < listSize; ++i)
            {
                for (int j = 0; j < listSize; ++j)
                {
                    int nbNeighbors = 0;
                    nbNeighbors += testNeighbor(i    , j + 1);
                    nbNeighbors += testNeighbor(i - 1, j + 1);
                    nbNeighbors += testNeighbor(i - 1, j);
                    nbNeighbors += testNeighbor(i - 1, j - 1);
                    nbNeighbors += testNeighbor(i    , j - 1);
                    nbNeighbors += testNeighbor(i + 1, j - 1);
                    nbNeighbors += testNeighbor(i + 1, j);
                    nbNeighbors += testNeighbor(i + 1, j + 1);

                    if (doubleBuffer[back][i][j] == '.')
                    {
                        doubleBuffer[front][i][j] = nbNeighbors <= 1 ? '#' : '.';
                    }
                    else
                    {
                        doubleBuffer[front][i][j] = nbNeighbors >= 5 ? '.' : '#';
                    }
                }
            }

            front = 1 - front;
            back = 1 - front;
        }

        List<List<char>> room = doubleBuffer[front];

        List<List<char>> floodFill;
        char biggestFlood;
        FloodFill(room, out floodFill, out biggestFlood);

        for (int i = 0; i < listSize; ++i)
        {
            for (int j = 0; j < listSize; ++j)
            {
                if (room[i][j] == '.' && floodFill[i][j] != ' ' && floodFill[i][j] != biggestFlood)
                {
                    room[i][j] = '#';
                }
            }
        }

        // pad for doors

        // pad top
        int rowSize = listSize;
        int columnSize = listSize;
        for (int i = 0; i < rowSize; ++i)
        {
            if (room[i][listSize - 1] == '.')
            {
                for (int j = 0; j < listSize; ++j)
                {
                    room[j].Add('#');
                }
                columnSize++;
                break;
            }
        }
        // pad left
        for (int i = 0; i < columnSize; ++i)
        {
            if (room[0][i] == '.')
            {
                List<char> newColumn = new List<char>();
                for (int j = 0; j < columnSize; ++j)
                {
                    newColumn.Add('#');
                }
                room.Insert(0, newColumn);
                rowSize++;
                break;
            }
        }
        // pad bottom
        for (int i = 0; i < rowSize; ++i)
        {
            if (room[i][0] == '.')
            {
                for (int j = 0; j < rowSize; ++j)
                {
                    room[j].Insert(0, '#');
                }
                columnSize++;
                break;
            }
        }
        // pad right
        for (int i = 0; i < columnSize; ++i)
        {
            if (room[listSize - 1][i] == '.')
            {
                List<char> newColumn = new List<char>();
                for (int j = 0; j < columnSize; ++j)
                {
                    newColumn.Add('#');
                }
                room.Add(newColumn);
                break;
            }
        }

        return room;
    }

    private static List<List<char>> GenerateRoomCircle()
    {
        List<List<char>> room = new List<List<char>>();

        int maxRadius = 5;
        int minRadius = 3;
        int radius = Random.Range(minRadius, maxRadius + 1);

        int size = radius * 2 + 3;
        float center = radius + 1;
        Debug.Log("Radius " + radius);
        Debug.Log("Center " + center);
        int radiusSq = radius * radius;
        
        for (int i = 0; i < size; ++i)
        {
            List<char> column = new List<char>();

            for (int j = 0; j < size; ++j)
            {
                float rSq = (i - center) * (i - center) + (j - center) * (j - center);
                char c = rSq > radiusSq ? '#' : '.';
                column.Add(c);
            }

            room.Add(column);
        }

        return room;
    }

    private static void MergeFirstRoom(List<List<char>> dungeon, BrogueRoom room)
    {
        int dungeonWidth = dungeon.Count;
        int dungeonHeight = dungeon[0].Count;
        int roomWidth = room.m_Room.Count;
        int roomHeight = room.m_Room[0].Count;

        int x = Random.Range(0, dungeonWidth - roomWidth);
        int y = Random.Range(0, dungeonHeight - roomHeight);

        for (int i = 0; i < roomWidth; ++i)
        {
            for (int j = 0; j < roomHeight; ++j)
            {
                dungeon[x + i][y + j] = room.m_Room[i][j];
            }
        }
    }

    private static void FloodFill(List<List<char>> dungeon, out List<List<char>> floodFill, out char biggestFlood)
    {
        int dungeonWidth = dungeon.Count;
        int dungeonHeight = dungeon[0].Count;

        floodFill = new List<List<char>>();
        for (int i = 0; i < dungeonWidth; ++i)
        {
            List<char> column = new List<char>();
            for (int j = 0; j < dungeonHeight; ++j)
            {
                column.Add(' ');
            }
            floodFill.Add(column);
        }

        char currentChar = 'a';
        biggestFlood = currentChar;
        int nbBiggestFlood = 0;

        for (int i = 0; i < dungeonWidth; ++i)
        {
            for (int j = 0; j < dungeonHeight; ++j)
            {
                if (dungeon[i][j] == '.' && floodFill[i][j] == ' ')
                {
                    Queue<(int, int)> queue = new Queue<(int, int)>();
                    HashSet<(int, int)> visited = new HashSet<(int, int)>();
                    queue.Enqueue((i, j));
                    int nbCurrent = 0;

                    Action<int, int, List<List<char>>> tryEnqueue = (x, y, floodFillInternal) =>
                    {
                        if (x < 0 || x >= dungeonWidth || y < 0 || y >= dungeonHeight)
                        {
                            return;
                        }

                        if (visited.Contains((x, y)))
                        {
                            return;
                        }

                        if (dungeon[x][y] == '.' && floodFillInternal[x][y] == ' ')
                        {
                            queue.Enqueue((x, y));
                        }
                    };

                    while (queue.Count > 0)
                    {
                        (int, int) current = queue.Dequeue();
                        if (visited.Contains((current.Item1, current.Item2)))
                        {
                            continue;
                        }

                        visited.Add(current);
                        floodFill[current.Item1][current.Item2] = currentChar;
                        nbCurrent++;

                        tryEnqueue(current.Item1, current.Item2 + 1, floodFill);
                        tryEnqueue(current.Item1 - 1, current.Item2, floodFill);
                        tryEnqueue(current.Item1, current.Item2 - 1, floodFill);
                        tryEnqueue(current.Item1 + 1, current.Item2, floodFill);
                    }

                    if (nbCurrent > nbBiggestFlood)
                    {
                        nbBiggestFlood = nbCurrent;
                        biggestFlood = currentChar;
                    }

                    currentChar++;
                }
            }
        }
    }

    private static void RandomAccess(List<List<char>> room, Func<int, int, char, bool> callback)
    {
        List<int> xIndices = new List<int>();
        List<int> yIndices = new List<int>();

        for (int i = 0; i < room.Count; ++i)
        {
            xIndices.Add(i);
        }

        for (int i = 0; i < room[0].Count; ++i)
        {
            yIndices.Add(i);
        }

        xIndices.Shuffle();
        yIndices.Shuffle();

        foreach (int x in xIndices)
        {
            foreach (int y in yIndices)
            {
                if (callback(x, y, room[x][y]))
                {
                    return;
                }
            }
        }
    }

    private static char GetRoomTile(List<List<char>> room, int x, int y)
    {
        if (x < 0 || x >= room.Count || y < 0 || y >= room[0].Count)
        {
            return '?';
        }

        return room[x][y];
    }
}
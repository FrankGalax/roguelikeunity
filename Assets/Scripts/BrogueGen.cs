using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using UnityEngine.Assertions;

public class BrogueRoom
{
    public List<List<char>> m_Room;
    public List<(int, int)> m_Floors;
    public (int, int) m_TopDoor = (0, 0);
    public (int, int) m_LeftDoor = (0, 0);
    public (int, int) m_BottomDoor = (0, 0);
    public (int, int) m_RightDoor = (0, 0);
    public bool m_HasTopDoor = false;
    public bool m_HasLeftDoor = false;
    public bool m_HasBottomDoor = false;
    public bool m_HasRightDoor = false;
    public int m_XOffset = 0;
    public int m_YOffset = 0;
}

public class RoomAlignment
{
    public int m_XOffset = 0;
    public int m_YOffset = 0;
    public int m_DoorX = 0;
    public int m_DoorY = 0;
    public int m_DoorDirection = 0;
}

public class BrogueGen
{
    public static List<List<char>> GenerateDungeon(int width, int height, FloorDefinition floorDefinition)
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

        BrogueRoom room = GenerateRoom(false, floorDefinition);

        // place player
        int playerFloorIndex = Random.Range(0, room.m_Floors.Count);
        (int, int) playerFloor = room.m_Floors[playerFloorIndex];
        room.m_Room[playerFloor.Item1][playerFloor.Item2] = 'p';

        MergeFirstRoom(dungeon, room);

        int failures = 0;
        BrogueRoom lastMergedRoom = null;
        while (failures < 10)
        {
            BrogueRoom newRoom = GenerateRoom(true, floorDefinition);
            bool isMerged = MergeRoom(dungeon, newRoom);

            if (isMerged)
            {
                lastMergedRoom = newRoom;
                failures = 0;
            }
            else
            {
                failures++;
            }
        }

        // place stairs
        Assert.IsNotNull(lastMergedRoom);
        int stairsFloorIndex = Random.Range(0, lastMergedRoom.m_Floors.Count);
        (int, int) stairsFloor = lastMergedRoom.m_Floors[stairsFloorIndex];
        dungeon[lastMergedRoom.m_XOffset + stairsFloor.Item1][lastMergedRoom.m_YOffset + stairsFloor.Item2] = 's';

        return dungeon;
    }

    private static BrogueRoom GenerateRoom(bool addDoorsAndTunnels, FloorDefinition floorDefinition)
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

        if (addDoorsAndTunnels)
        {
            AddDoors(room);

            float tunnelChance = 0.15f;
            if (Random.value < tunnelChance)
            {
                AddTunnel(room);
            }
        }

        Func<int, int, bool> isFloorNeighbor = (x, y) =>
        {
            if (x < 0 || x >= room.m_Room.Count || y < 0 || y >= room.m_Room[0].Count)
            {
                return false;
            }

            return room.m_Room[x][y] == '.';
        };

        // Remove useless walls and find floors
        room.m_Floors = new List<(int, int)>();
        for (int i = 0; i < room.m_Room.Count; ++i)
        {
            for (int j = 0; j < room.m_Room[i].Count; ++j)
            {
                if (room.m_Room[i][j] == '#')
                {
                    bool hasFloorNeighbor = isFloorNeighbor(i, j + 1);
                    hasFloorNeighbor |= isFloorNeighbor(i - 1, j + 1);
                    hasFloorNeighbor |= isFloorNeighbor(i - 1, j);
                    hasFloorNeighbor |= isFloorNeighbor(i - 1, j - 1);
                    hasFloorNeighbor |= isFloorNeighbor(i, j - 1);
                    hasFloorNeighbor |= isFloorNeighbor(i + 1, j - 1);
                    hasFloorNeighbor |= isFloorNeighbor(i + 1, j);
                    hasFloorNeighbor |= isFloorNeighbor(i + 1, j + 1);

                    if (!hasFloorNeighbor)
                    {
                        room.m_Room[i][j] = ' ';
                    }
                }
                else if (room.m_Room[i][j] == '.')
                {
                    room.m_Floors.Add((i, j));
                }
            }
        }

        if (floorDefinition != null)
        {
            int nbMobs = Random.Range(floorDefinition.MinMobsPerRoom, floorDefinition.MaxMobsPerRoom + 1);
            int nbItems = Random.Range(floorDefinition.MinItemsPerRoom, floorDefinition.MaxItemsPerRoom + 1);

            List<char> entities = new List<char>();
            for (int i = 0; i < nbMobs; ++i)
            {
                entities.Add('m');
            }
            for (int i = 0; i < nbItems; ++i)
            {
                entities.Add('i');
            }

            foreach (char entity in entities)
            {
                int floorIndex = Random.Range(0, room.m_Floors.Count);
                (int, int) floor = room.m_Floors[floorIndex];
                if (room.m_Room[floor.Item1][floor.Item2] == '.')
                {
                    room.m_Room[floor.Item1][floor.Item2] = entity;
                }
            }
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
            room.m_HasTopDoor = true;
            room.m_TopDoor = (x, y + 1);
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
            room.m_HasLeftDoor = true;
            room.m_LeftDoor = (x - 1, y);
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
            room.m_HasBottomDoor = true;
            room.m_BottomDoor = (x, y - 1);
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
            room.m_HasRightDoor = true;
            room.m_RightDoor = (x + 1, y);
            return true;
        });
    }

    private static void AddTunnel(BrogueRoom room)
    {
        Action<int, int> changeTopDoor = (x, y) =>
        {
            room.m_TopDoor = (x, y);
            room.m_Room[x][y] = 'd';
        };

        Action<int, int> changeLeftDoor = (x, y) =>
        {
            room.m_LeftDoor = (x, y);
            room.m_Room[x][y] = 'd';
        };

        Action<int, int> changeBottomDoor = (x, y) =>
        {
            room.m_BottomDoor = (x, y);
            room.m_Room[x][y] = 'd';
        };

        Action<int, int> changeRightDoor = (x, y) =>
        {
            room.m_RightDoor = (x, y);
            room.m_Room[x][y] = 'd';
        };

        room.m_Room[room.m_TopDoor.Item1][room.m_TopDoor.Item2] = '#';
        room.m_Room[room.m_LeftDoor.Item1][room.m_LeftDoor.Item2] = '#';
        room.m_Room[room.m_BottomDoor.Item1][room.m_BottomDoor.Item2] = '#';
        room.m_Room[room.m_RightDoor.Item1][room.m_RightDoor.Item2] = '#';

        int r = Random.Range(0, 4);
        int tunnelLength = Random.Range(5, 11);

        if (r == 0)
        {
            Assert.IsTrue(room.m_HasTopDoor);
            int topDoorX = room.m_TopDoor.Item1;
            int topDoorY = room.m_TopDoor.Item2;
            room.m_Room[topDoorX][topDoorY] = '.';

            int roomWidth = room.m_Room.Count;

            room.m_HasBottomDoor = false;

            for (int i = 1; i < tunnelLength + 1; ++i)
            {
                if (room.m_Room[0].Count <= topDoorY + i)
                {
                    for (int j = 0; j < roomWidth; ++j)
                    {
                        char c = j == topDoorX ? '.' : '#';
                        room.m_Room[j].Add(c);
                    }
                }
                else
                {
                    room.m_Room[topDoorX][topDoorY + i] = '.';
                }
            }

            changeTopDoor(topDoorX, topDoorY + tunnelLength);
            changeLeftDoor(topDoorX - 1, topDoorY + tunnelLength - 1);
            changeRightDoor(topDoorX + 1, topDoorY + tunnelLength - 1);
        }
        else if (r == 1)
        {
            Assert.IsTrue(room.m_HasLeftDoor);
            int leftDoorX = room.m_LeftDoor.Item1;
            int leftDoorY = room.m_LeftDoor.Item2;
            room.m_Room[leftDoorX][leftDoorY] = '.';

            int roomHeight = room.m_Room[0].Count;

            room.m_HasRightDoor = false;

            for (int i = 1; i < tunnelLength + 1; ++i)
            {
                if (leftDoorX - i < 0)
                {
                    List<char> column = new List<char>();
                    for (int j = 0; j < roomHeight; ++j)
                    {
                        char c = j == leftDoorY ? '.' : '#';
                        column.Add(c);
                    }
                    room.m_Room.Insert(0, column);
                }
                else
                {
                    room.m_Room[leftDoorX - i][leftDoorY] = '.';
                }
            }

            changeLeftDoor(0, leftDoorY);
            changeTopDoor(1, leftDoorY + 1);
            changeBottomDoor(1, leftDoorY - 1);
        }
        else if (r == 2)
        {
            Assert.IsTrue(room.m_HasBottomDoor);
            int bottomDoorX = room.m_BottomDoor.Item1;
            int bottomDoorY = room.m_BottomDoor.Item2;
            room.m_Room[bottomDoorX][bottomDoorY] = '.';

            int roomWidth = room.m_Room.Count;

            room.m_HasTopDoor = false;

            for (int i = 1; i < tunnelLength + 1; ++i)
            {
                if (bottomDoorY - i < 0)
                {
                    for (int j = 0; j < roomWidth; ++j)
                    {
                        char c = j == bottomDoorX ? '.' : '#';
                        room.m_Room[j].Insert(0, c);
                    }
                }
                else
                {
                    room.m_Room[bottomDoorX][bottomDoorY - i] = '.';
                }
            }

            changeBottomDoor(bottomDoorX, 0);
            changeLeftDoor(bottomDoorX - 1, 1);
            changeRightDoor(bottomDoorX + 1, 1);
        }
        else
        {
            Assert.IsTrue(room.m_HasRightDoor);
            int rightDoorX = room.m_RightDoor.Item1;
            int rightDoorY = room.m_RightDoor.Item2;
            room.m_Room[rightDoorX][rightDoorY] = '.';

            int roomHeight = room.m_Room[0].Count;

            room.m_HasLeftDoor = false;

            for (int i = 1; i < tunnelLength + 1; ++i)
            {
                if (room.m_Room.Count <= rightDoorX + i)
                {
                    List<char> column = new List<char>();
                    for (int j = 0; j < roomHeight; ++j)
                    {
                        char c = j == rightDoorY ? '.' : '#';
                        column.Add(c);
                    }
                    room.m_Room.Add(column);
                }
                else
                {
                    room.m_Room[rightDoorX + i][rightDoorY] = '.';
                }
            }

            changeRightDoor(rightDoorX + tunnelLength, rightDoorY);
            changeTopDoor(rightDoorX + tunnelLength - 1, rightDoorY + 1);
            changeBottomDoor(rightDoorX + tunnelLength - 1, rightDoorY - 1);
        }
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

    private static bool MergeRoom(List<List<char>> dungeon, BrogueRoom room)
    {
        Action<BrogueRoom> deleteTopDoor = (room) =>
        {
            if (room.m_HasTopDoor)
            {
                room.m_Room[room.m_TopDoor.Item1][room.m_TopDoor.Item2] = '#';
                room.m_HasTopDoor = false;
                room.m_TopDoor = (0, 0);
            }
        };
        Action<BrogueRoom> deleteLeftDoor = (room) =>
        {
            if (room.m_HasLeftDoor)
            {
                room.m_Room[room.m_LeftDoor.Item1][room.m_LeftDoor.Item2] = '#';
                room.m_HasLeftDoor = false;
                room.m_LeftDoor = (0, 0);
            }
        };
        Action<BrogueRoom> deleteBottomDoor = (room) =>
        {
            if (room.m_HasBottomDoor)
            {
                room.m_Room[room.m_BottomDoor.Item1][room.m_BottomDoor.Item2] = '#';
                room.m_HasBottomDoor = false;
                room.m_BottomDoor = (0, 0);
            }
        };
        Action<BrogueRoom> deleteRightDoor = (room) =>
        {
            if (room.m_HasRightDoor)
            {
                room.m_Room[room.m_RightDoor.Item1][room.m_RightDoor.Item2] = '#';
                room.m_HasRightDoor = false;
                room.m_RightDoor = (0, 0);
            }
        };

        bool isMerged = false;

        RandomAccess(dungeon, (x, y, c) =>
        {
            List<RoomAlignment> roomAlignments = AlignDoor(dungeon, x, y, room);
            if (roomAlignments.Count == 0)
            {
                return false;
            }

            foreach (RoomAlignment roomAlignment in roomAlignments)
            {
                GameObject debugRoom = null;
                bool debugRooms = false;
                if (debugRooms)
                {
                    debugRoom = DebugRoom(room.m_Room, roomAlignment);
                }

                if (CollidesWithDungeon(dungeon, room, roomAlignment, debugRoom))
                {
                    return false;
                }

                if (roomAlignment.m_DoorDirection == 0)
                {
                    deleteLeftDoor(room);
                    deleteBottomDoor(room);
                    deleteRightDoor(room);
                }
                else if (roomAlignment.m_DoorDirection == 1)
                {
                    deleteTopDoor(room);
                    deleteBottomDoor(room);
                    deleteRightDoor(room);
                }
                else if (roomAlignment.m_DoorDirection == 2)
                {
                    deleteTopDoor(room);
                    deleteLeftDoor(room);
                    deleteRightDoor(room);
                }
                else
                {
                    deleteTopDoor(room);
                    deleteLeftDoor(room);
                    deleteBottomDoor(room);
                }

                for (int i = 0; i < room.m_Room.Count; ++i)
                {
                    for (int j = 0; j < room.m_Room[i].Count; ++j)
                    {
                        if (room.m_Room[i][j] != ' ')
                        {
                            dungeon[roomAlignment.m_XOffset + i][roomAlignment.m_YOffset + j] = room.m_Room[i][j];
                        }
                    }
                }

                isMerged = true;
                room.m_XOffset = roomAlignment.m_XOffset;
                room.m_YOffset = roomAlignment.m_YOffset;

                return true;
            }

            return false;
        });

        return isMerged;
    }

    private static List<RoomAlignment> AlignDoor(List<List<char>> dungeon, int x, int y, BrogueRoom room)
    {
        List<RoomAlignment> roomAlignments = new List<RoomAlignment>();

        if (dungeon[x][y] != '#')
        {
            return roomAlignments;
        }

        int dungeonWidth = dungeon.Count;
        int dungeonHeight = dungeon[0].Count;

        for (int i = 0; i < 4; ++i)
        {
            if (i == 0)
            {
                if (room.m_HasTopDoor)
                {
                    if (y - 1 >= 0 && y + 1 < dungeonHeight && dungeon[x][y + 1] == '.')
                    {
                        RoomAlignment roomAlignment = new RoomAlignment();
                        roomAlignment.m_XOffset = x - room.m_TopDoor.Item1;
                        roomAlignment.m_YOffset = y - room.m_TopDoor.Item2;
                        roomAlignment.m_DoorX = room.m_TopDoor.Item1;
                        roomAlignment.m_DoorY = room.m_TopDoor.Item2;
                        roomAlignment.m_DoorDirection = 0;
                        roomAlignments.Add(roomAlignment);
                    }
                }
            }
            else if (i == 1)
            {
                if (room.m_HasLeftDoor)
                {
                    if (x + 1 < dungeonWidth && x - 1 >= 0 && dungeon[x - 1][y] == '.')
                    {
                        RoomAlignment roomAlignment = new RoomAlignment();
                        roomAlignment.m_XOffset = x - room.m_LeftDoor.Item1;
                        roomAlignment.m_YOffset = y - room.m_LeftDoor.Item2;
                        roomAlignment.m_DoorX = room.m_LeftDoor.Item1;
                        roomAlignment.m_DoorY = room.m_LeftDoor.Item2;
                        roomAlignment.m_DoorDirection = 1;
                        roomAlignments.Add(roomAlignment);
                    }
                }
            }
            else if (i == 2)
            {
                if (room.m_HasBottomDoor)
                {
                    if (y + 1 < dungeonHeight && y - 1 >= 0 && dungeon[x][y - 1] == '.')
                    {
                        RoomAlignment roomAlignment = new RoomAlignment();
                        roomAlignment.m_XOffset = x - room.m_BottomDoor.Item1;
                        roomAlignment.m_YOffset = y - room.m_BottomDoor.Item2;
                        roomAlignment.m_DoorX = room.m_BottomDoor.Item1;
                        roomAlignment.m_DoorY = room.m_BottomDoor.Item2;
                        roomAlignment.m_DoorDirection = 2;
                        roomAlignments.Add(roomAlignment);
                    }
                }
            }
            else
            {
                if (room.m_HasRightDoor)
                {
                    if (x - 1 >= 0 && x + 1 < dungeonWidth && dungeon[x + 1][y] == '.')
                    {
                        RoomAlignment roomAlignment = new RoomAlignment();
                        roomAlignment.m_XOffset = x - room.m_RightDoor.Item1;
                        roomAlignment.m_YOffset = y - room.m_RightDoor.Item2;
                        roomAlignment.m_DoorX = room.m_RightDoor.Item1;
                        roomAlignment.m_DoorY = room.m_RightDoor.Item2;
                        roomAlignment.m_DoorDirection = 3;
                        roomAlignments.Add(roomAlignment);
                    }
                }
            }
        }

        roomAlignments.Shuffle();
        return roomAlignments;
    }

    private static bool CollidesWithDungeon(List<List<char>> dungeon, BrogueRoom room, RoomAlignment roomAlignment, GameObject debugRoom)
    {
        for (int i = 0; i < room.m_Room.Count; ++i)
        {
            int dungeonX = i + roomAlignment.m_XOffset;

            for (int j = 0; j < room.m_Room[i].Count; ++j)
            {
                if (room.m_Room[i][j] == ' ')
                {
                    continue;
                }

                int dungeonY = j + roomAlignment.m_YOffset;

                if (dungeonX < 0 || dungeonX >= dungeon.Count || dungeonY < 0 || dungeonY >= dungeon[dungeonX].Count)
                {
                    AddDebugTile(debugRoom, i, j, Color.red);
                    return true;
                }

                if (room.m_Room[i][j] == 'd')
                {
                    if (dungeon[dungeonX][dungeonY] != '#' && dungeon[dungeonX][dungeonY] != ' ')
                    {
                        AddDebugTile(debugRoom, i, j, Color.cyan);
                        return true;
                    }
                }
                else if (dungeon[dungeonX][dungeonY] != ' ' && dungeon[dungeonX][dungeonY] != room.m_Room[i][j])
                {
                    AddDebugTile(debugRoom, i, j, Color.green);
                    return true;
                }
            }
        }

        return false;
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

    private static List<(int, int)> AStar(List<List<char>> dungeon, int x, int y, int destX, int destY)
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
                if (neighbour.Item1 < 0 || neighbour.Item1 >= dungeon.Count || neighbour.Item2 < 0 || neighbour.Item2 >= dungeon[0].Count)
                {
                    continue;
                }

                char c = dungeon[neighbour.Item1][neighbour.Item2];
                if (c == '#' || c == ' ')
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

    private static GameObject DebugRoom(List<List<char>> room, RoomAlignment roomAlignment)
    {
        GameObject debugRoom = new GameObject("RoomDebug");
        debugRoom.transform.position = new Vector3((float)roomAlignment.m_XOffset, (float)roomAlignment.m_YOffset, 0.0f);

        Action<GameObject, int, int> addTile = (prefab, x, y) =>
        {
            GameObject tile = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, debugRoom.transform);
            tile.transform.localPosition = new Vector3((float)x, (float)y, 0.0f);
        };

        GameMap gameMap = GameObject.FindObjectOfType<GameMap>();

        for (int i = 0; i < room.Count; ++i)
        {
            for (int j = 0; j < room[i].Count; ++j)
            {
                switch (room[i][j])
                {
                    case '.':
                        addTile(gameMap.Floor, i, j);
                        break;
                    case '#':
                        addTile(gameMap.Wall, i, j);
                        break;
                    case 'd':
                        addTile(gameMap.Door, i, j);
                        break;
                }
            }
        }

        AddDebugTile(debugRoom, roomAlignment.m_DoorX, roomAlignment.m_DoorY, Color.magenta);

        debugRoom.SetActive(false);
        return debugRoom;
    }

    private static void AddDebugTile(GameObject debugRoom, int x, int y, Color color)
    {
        if (debugRoom == null)
        {
            return;
        }
        GameObject debugTile = GameObject.Instantiate(Config.Instance.DebugTile, Vector3.zero, Quaternion.identity, debugRoom.transform);
        debugTile.transform.localPosition = new Vector3((float)x, (float)y, 0.0f);
        debugTile.GetComponent<SpriteRenderer>().color = color;
    }
}
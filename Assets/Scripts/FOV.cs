using System;

public class FOV
{
    private static void CastRay(GameMap map, int xOrigin, int yOrigin, int xDest, int yDest, int radiusSquared, bool lightWalls, Action<int, int, bool> fovCallback) 
    {
        BresenhamData bresenhamData = new BresenhamData();
        int currentX;
        int currentY;
        Bresenham.LineInit(xOrigin, yOrigin, xDest, yDest, bresenhamData);
        while (!Bresenham.LineStep(out currentX, out currentY, bresenhamData)) 
        {
            if (!map.IsInBounds(currentX, currentY)) 
            {
                return;  // Out of bounds.
            }
            if (radiusSquared > 0) 
            {
                int currentRadius = (currentX - xOrigin) * (currentX - xOrigin) + (currentY - yOrigin) * (currentY - yOrigin);
                if (currentRadius > radiusSquared) 
                {
                    return;  // Outside of radius.
                }
            }
            Tile tile = map.GetTileAtLocation(currentX, currentY);
            if (tile == null || !tile.Transparent)
            {
                if (lightWalls)
                {
                    fovCallback(currentX, currentY, true);
                }
                return;  // Blocked by wall.
            }
            // Tile is transparent.
            fovCallback(currentX, currentY, true);
        }
    }
    
    private static void MapPostProcessQuadrant(GameMap map, int x0, int y0, int x1, int y1, int dx, int dy, Action<int, int, bool> fovCallback)
    {
        if (Math.Abs(dx) != 1 || Math.Abs(dy) != 1)
        {
            return;  // Bad parameters.
        }
        for (int cx = x0; cx <= x1; cx++)
        {
            for (int cy = y0; cy <= y1; cy++)
            {
                int x2 = cx + dx;
                int y2 = cy + dy;

                Tile tile = map.GetTileAtLocation(cx, cy);
                if (tile != null && tile.IsVisible && tile.Transparent)
                {
                    if (x2 >= x0 && x2 <= x1)
                    {
                        Tile tile2 = map.GetTileAtLocation(x2, cy);
                        if (tile2 != null && !tile2.Transparent)
                        {
                            fovCallback(x2, cy, true);
                        }
                    }
                    if (y2 >= y0 && y2 <= y1)
                    {
                        Tile tile2 = map.GetTileAtLocation(cx, y2);
                        if (tile2 != null && !tile2.Transparent)
                        {
                            fovCallback(cx, y2, true);
                        }
                    }
                    if (x2 >= x0 && x2 <= x1 && y2 >= y0 && y2 <= y1)
                    {
                        Tile tile2 = map.GetTileAtLocation(x2, y2);
                        if (tile2 != null && !tile2.Transparent)
                        {
                            fovCallback(x2, y2, true);
                        }
                    }
                }
            }
        }
    }

    private static void MapPostProcess(GameMap map, int povX, int povY, int radius, Action<int, int, bool> fovCallback)
    {
        int xMin = 0;
        int yMin = 0;
        int xMax = map.DungeonWidth;
        int yMax = map.DungeonHeight;
        if (radius > 0)
        {
            xMin = Math.Max(xMin, povX - radius);
            yMin = Math.Max(yMin, povY - radius);
            xMax = Math.Min(xMax, povX + radius + 1);
            yMax = Math.Min(yMax, povY + radius + 1);
        }
        MapPostProcessQuadrant(map, xMin, yMin, povX, povY, -1, -1, fovCallback);
        MapPostProcessQuadrant(map, povX, yMin, xMax - 1, povY, 1, -1, fovCallback);
        MapPostProcessQuadrant(map, xMin, povY, povX, yMax - 1, -1, 1, fovCallback);
        MapPostProcessQuadrant(map, povX, povY, xMax - 1, yMax - 1, 1, 1, fovCallback);
    }

    public static void ComputeFOVCircularRaycasting(GameMap map, int povX, int povY, int maxRadius, bool lightWalls, Action<int, int, bool> fovCallback)
    {
        int xMin = 0;  // Field-of-view bounds.
        int yMin = 0;
        int xMax = map.DungeonWidth;
        int yMax = map.DungeonHeight;
        if (maxRadius > 0)
        {
            xMin = Math.Max(xMin, povX - maxRadius);
            yMin = Math.Max(yMin, povY - maxRadius);
            xMax = Math.Min(xMax, povX + maxRadius + 1);
            yMax = Math.Min(yMax, povY + maxRadius + 1);
        }
        if (!map.IsInBounds(povX, povY))
        {
            return;
        }
        fovCallback(povX, povY, true);

        // Cast rays along the perimeter.
        int radiusSquared = maxRadius * maxRadius;
        for (int x = xMin; x < xMax; ++x)
        {
            CastRay(map, povX, povY, x, yMin, radiusSquared, lightWalls, fovCallback);
        }
        for (int y = yMin + 1; y < yMax; ++y)
        {
            CastRay(map, povX, povY, xMax - 1, y, radiusSquared, lightWalls, fovCallback);
        }
        for (int x = xMax - 2; x >= xMin; --x)
        {
            CastRay(map, povX, povY, x, yMax - 1, radiusSquared, lightWalls, fovCallback);
        }
        for (int y = yMax - 2; y > yMin; --y)
        {
            CastRay(map, povX, povY, xMin, y, radiusSquared, lightWalls, fovCallback);
        }
        if (lightWalls)
        {
            MapPostProcess(map, povX, povY, maxRadius, fovCallback);
        }
    }
}

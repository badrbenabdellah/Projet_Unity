using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public RectInt bounds;
    public List<Vector2> generatedPositions;
    private int margin;
    private int minDistance;
    private RoomType type;

    public Room(Vector2Int location, Vector2Int size, int margin = 5, int minDistance = 2)
    {
        bounds = new RectInt(location, size);
        generatedPositions = new List<Vector2>();
        this.margin = margin;
        this.minDistance = minDistance;
        type = RoomType.None;

        // Vérifiez si la pièce est assez grande pour contenir une position valide
        if (bounds.width < 2 * margin || bounds.height < 2 * margin)
        {
            throw new System.ArgumentException("Room dimensions are too small to accommodate the margin.");
        }
    }

    public void SetRoomType(RoomType type)
    {
        this.type = type;
    }

    public RoomType GetRoomType()
    {
        return this.type;
    }

    public static bool Intersect(Room a, Room b)
    {
        return !((a.bounds.position.x >= (b.bounds.position.x + b.bounds.size.x)) || ((a.bounds.position.x + a.bounds.size.x) <= b.bounds.position.x)
            || (a.bounds.position.y >= (b.bounds.position.y + b.bounds.size.y)) || ((a.bounds.position.y + a.bounds.size.y) <= b.bounds.position.y));
    }

    public List<Vector2Int> GetAllPoints()
    {
        List<Vector2Int> points = new List<Vector2Int>();

        for (int x = bounds.x; x < bounds.x + bounds.width; x++)
        {
            for (int y = bounds.y; y < bounds.y + bounds.height; y++)
            {
                points.Add(new Vector2Int(x, y));
            }
        }

        return points;
    }

    public Vector2 GetRandomPosition()
    {
        Vector2 position;
        int attempts = 0;
        do
        {
            int x = UnityEngine.Random.Range(bounds.x + margin, bounds.x + bounds.width - margin);
            int y = UnityEngine.Random.Range(bounds.y + margin, bounds.y + bounds.height - margin);
            position = new Vector2(x, y);
            attempts++;
        } while (!IsPositionValid(position) && attempts < 100);

        if (attempts >= 100)
        {
            throw new System.Exception("Unable to find a valid position after 100 attempts.");
        }

        generatedPositions.Add(position);
        return position;
    }

    private bool IsPositionValid(Vector2 position)
    {
        foreach (var pos in generatedPositions)
        {
            if (Vector2.Distance(position, pos) < minDistance)
            {
                return false;
            }
        }
        return true;
    }
}

public enum RoomType
{
    None,
    Player
}

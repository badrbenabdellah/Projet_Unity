using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

//DungeonTileMapVisualizer.cs

public class DungeonTileMapVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap RoomfloorTilemap, HallwayTilemap, WallTilemap;

    [SerializeField]
    private TileBase RoomfloorTile, HallwayTile, WallTopTile,
        WallBottomTile, WallLeftTile, WallRightTile, WallLeftTopCornerTile, WallRightTopCornerTile,
        WallLeftDownCornerTile, WallRightDownCornerTile;

    private static DungeonTileMapVisualizer instance;

    public static DungeonTileMapVisualizer GetInstance()
    {
        if (instance == null)
        {
            instance = new DungeonTileMapVisualizer();
        }
        return instance;
    }

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        PaintTiles(floorPositions, RoomfloorTilemap, RoomfloorTile);

    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase title)
    {
        foreach (var position in positions)
        {
            PaintSingleTitle(tilemap, title, position);
        }
    }


    private void PaintSingleTitle(Tilemap tilemap, TileBase title, Vector2Int position)
    {
        var titlePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(titlePosition, title);
    }

    public void PaintHallway(Vector2Int position)
    {
        var titlePosition = HallwayTilemap.WorldToCell((Vector3Int)position);
        HallwayTilemap.SetTile(titlePosition, HallwayTile);
    }

    public List<Vector2Int> PaintRoomWalls(List<List<Vector2Int>> roomPoints, Grid2D<CellType> grid, List<Vector2Int> hallways)
    {
        List<Vector2Int> WallList = new List<Vector2Int>();
        foreach (var room in roomPoints)
        {
            foreach (var position in room)
            {
                // place wall top
                if (grid[position + Direction2D.cardinalPoint[0]] == CellType.None && grid[position + Direction2D.cardinalPoint[1]] == CellType.Room)
                {
                    var titlePosition = WallTilemap.WorldToCell((Vector3Int)(position + Direction2D.cardinalPoint[0]));
                    WallTilemap.SetTile(titlePosition, WallTopTile);
                    WallList.Add(position + Direction2D.cardinalPoint[0]);
                }
                // place wall bottom
                if (grid[position + Direction2D.cardinalPoint[0]] == CellType.Room && grid[position + Direction2D.cardinalPoint[1]] == CellType.None)
                {
                    var titlePosition = WallTilemap.WorldToCell((Vector3Int)(position + Direction2D.cardinalPoint[1]));
                    WallTilemap.SetTile(titlePosition, WallBottomTile);
                    WallList.Add(position + Direction2D.cardinalPoint[1]);
                }
                //place wall left
                if (grid[position + Direction2D.cardinalPoint[3]] == CellType.None && grid[position + Direction2D.cardinalPoint[2]] == CellType.Room)
                {
                    var titlePosition = WallTilemap.WorldToCell((Vector3Int)(position + Direction2D.cardinalPoint[3]));
                    WallTilemap.SetTile(titlePosition, WallLeftTile);
                    WallList.Add(position + Direction2D.cardinalPoint[3]);
                }
                //place wall right
                if (grid[position + Direction2D.cardinalPoint[3]] == CellType.Room && grid[position + Direction2D.cardinalPoint[2]] == CellType.None)
                {
                    var titlePosition = WallTilemap.WorldToCell((Vector3Int)(position + Direction2D.cardinalPoint[2]));
                    WallTilemap.SetTile(titlePosition, WallRightTile);
                    WallList.Add(position + Direction2D.cardinalPoint[2]);
                }
                //place wall top left
                if (grid[position + Direction2D.cardinalPoint[0]] == CellType.None && grid[position + Direction2D.cardinalPoint[1]] == CellType.Room && grid[position + Direction2D.cardinalPoint[3]] == CellType.None)
                {
                    var titlePosition = WallTilemap.WorldToCell((Vector3Int)(position + Direction2D.cardinalPoint[0] + Direction2D.cardinalPoint[3]));
                    WallTilemap.SetTile(titlePosition, WallLeftTopCornerTile);
                    WallList.Add(position + Direction2D.cardinalPoint[0] + Direction2D.cardinalPoint[3]);
                }
                // place wall top right 
                if (grid[position + Direction2D.cardinalPoint[0]] == CellType.None && grid[position + Direction2D.cardinalPoint[1]] == CellType.Room && grid[position + Direction2D.cardinalPoint[2]] == CellType.None)
                {
                    var titlePosition = WallTilemap.WorldToCell((Vector3Int)(position + Direction2D.cardinalPoint[0] + Direction2D.cardinalPoint[2]));
                    WallTilemap.SetTile(titlePosition, WallRightTopCornerTile);
                    WallList.Add(position + Direction2D.cardinalPoint[0] + Direction2D.cardinalPoint[2]);
                }
                //place wall down left
                if (grid[position + Direction2D.cardinalPoint[0]] == CellType.Room && grid[position + Direction2D.cardinalPoint[1]] == CellType.None && grid[position + Direction2D.cardinalPoint[3]] == CellType.None)
                {
                    var titlePosition = WallTilemap.WorldToCell((Vector3Int)(position + Direction2D.cardinalPoint[1] + Direction2D.cardinalPoint[3]));
                    WallTilemap.SetTile(titlePosition, WallLeftDownCornerTile);
                    WallList.Add(position + Direction2D.cardinalPoint[1] + Direction2D.cardinalPoint[3]);
                }
                //place wall down right
                if (grid[position + Direction2D.cardinalPoint[0]] == CellType.Room && grid[position + Direction2D.cardinalPoint[1]] == CellType.None && grid[position + Direction2D.cardinalPoint[2]] == CellType.None)
                {
                    var titlePosition = WallTilemap.WorldToCell((Vector3Int)(position + Direction2D.cardinalPoint[2] + Direction2D.cardinalPoint[1]));
                    WallTilemap.SetTile(titlePosition, WallRightDownCornerTile);
                    WallList.Add(position + Direction2D.cardinalPoint[2] + Direction2D.cardinalPoint[1]);
                }
            }

        }

        foreach (var position in hallways)
        {
            if (grid[position + Direction2D.cardinalPoint[2]] == CellType.None && grid[position + Direction2D.cardinalPoint[3]] == CellType.Hallway)
            {
                var titlePosition = WallTilemap.WorldToCell((Vector3Int)(position + Direction2D.cardinalPoint[2]));
                WallTilemap.SetTile(titlePosition, WallRightTile);
                WallList.Add(position + Direction2D.cardinalPoint[2]);
            }
            if (grid[position + Direction2D.cardinalPoint[2]] == CellType.Hallway && grid[position + Direction2D.cardinalPoint[3]] == CellType.None)
            {
                var titlePosition = WallTilemap.WorldToCell((Vector3Int)(position + Direction2D.cardinalPoint[3]));
                WallTilemap.SetTile(titlePosition, WallLeftTile);
                WallList.Add(position + Direction2D.cardinalPoint[3]);
            }
            if (grid[position + Direction2D.cardinalPoint[0]] == CellType.Hallway && grid[position + Direction2D.cardinalPoint[1]] == CellType.None)
            {
                var titlePosition = WallTilemap.WorldToCell((Vector3Int)(position + Direction2D.cardinalPoint[1]));
                WallTilemap.SetTile(titlePosition, WallBottomTile);
                WallList.Add(position + Direction2D.cardinalPoint[1]);
            }
            if (grid[position + Direction2D.cardinalPoint[0]] == CellType.None && grid[position + Direction2D.cardinalPoint[1]] == CellType.Hallway)
            {
                var titlePosition = WallTilemap.WorldToCell((Vector3Int)(position + Direction2D.cardinalPoint[0]));
                WallTilemap.SetTile(titlePosition, WallTopTile);
                WallList.Add(position + Direction2D.cardinalPoint[0]);
            }
            
        }

        foreach (var position in hallways)
        {
            if (grid[position + Direction2D.cardinalPoint[0]] == CellType.Hallway && grid[position + Direction2D.cardinalPoint[1]] == CellType.None
                && grid[position + Direction2D.cardinalPoint[2]] == CellType.None)
            {
                var titlePosition = WallTilemap.WorldToCell((Vector3Int)(position + Direction2D.cardinalPoint[1] + Direction2D.cardinalPoint[2]));
                WallTilemap.SetTile(titlePosition, WallRightDownCornerTile);
                WallList.Add(position + Direction2D.cardinalPoint[1] + Direction2D.cardinalPoint[2]);
            }
            if (grid[position + Direction2D.cardinalPoint[0]] == CellType.Hallway && grid[position + Direction2D.cardinalPoint[1]] == CellType.None
                && grid[position + Direction2D.cardinalPoint[3]] == CellType.None)
            {
                var titlePosition = WallTilemap.WorldToCell((Vector3Int)(position + Direction2D.cardinalPoint[1] + Direction2D.cardinalPoint[3]));
                WallTilemap.SetTile(titlePosition, WallLeftDownCornerTile);
                WallList.Add(position + Direction2D.cardinalPoint[1] + Direction2D.cardinalPoint[3]);
            }
            if (grid[position + Direction2D.cardinalPoint[0]] == CellType.None && grid[position + Direction2D.cardinalPoint[1]] == CellType.Hallway
                && grid[position + Direction2D.cardinalPoint[2]] == CellType.None)
            {
                var titlePosition = WallTilemap.WorldToCell((Vector3Int)(position + Direction2D.cardinalPoint[2] + Direction2D.cardinalPoint[0]));
                WallTilemap.SetTile(titlePosition, WallRightTopCornerTile);
                WallList.Add(position + Direction2D.cardinalPoint[2] + Direction2D.cardinalPoint[0]);
            }
            if (grid[position + Direction2D.cardinalPoint[0]] == CellType.None && grid[position + Direction2D.cardinalPoint[1]] == CellType.Hallway
                && grid[position + Direction2D.cardinalPoint[3]] == CellType.None)
            {
                var titlePosition = WallTilemap.WorldToCell((Vector3Int)(position + Direction2D.cardinalPoint[0] + Direction2D.cardinalPoint[3]));
                WallTilemap.SetTile(titlePosition, WallLeftTopCornerTile);
                WallList.Add(position + Direction2D.cardinalPoint[0] + Direction2D.cardinalPoint[3]);
            }
        }

        return WallList;

    }

    public void clear()
    {
        RoomfloorTilemap.ClearAllTiles();
        HallwayTilemap.ClearAllTiles();
        WallTilemap.ClearAllTiles();
    }

}

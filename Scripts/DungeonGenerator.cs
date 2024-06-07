using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using Graphs;
using System.IO;
using System;
using System.Linq;
using UnityEditor.Search;

public class DungeonGenerator : MonoBehaviour {

    [SerializeField]
    Vector2Int size;
    [SerializeField]
    int roomCount;
    [SerializeField]
    Vector2Int roomMaxSize;
    [SerializeField] 
    Vector2Int roomMinSize;
    [SerializeField]
    DungeonTileMapVisualizer dungeonTileMapVisualizer = DungeonTileMapVisualizer.GetInstance();
    Grid2D<CellType> grid;
    List<Room> rooms;
    List<Vector2Int> hallways;
    List<Vector2Int> walls;
    Delaunay2D delaunay;
    HashSet<Prim.Edge> selectedEdges;
    [SerializeField]
    GameObject playerPrefab;
    [SerializeField]
    GameObject coinPrefab;
    [SerializeField]
    GameObject enemyPrefab;
    GameObject playerObject;
    Player playerComponent;
    List<Vector2> CoinsPoint;
    List<Vector2> EnemyPoint;
    private float minDistance = 6.0f;


    void Start() {
        //dungeonTileMapVisualizer.clear();
        Generate();
        Invoke("StartCollectingCoins", 5f); // Call ProcessCollectCoins after 5 seconds
    }


    void Generate() {
        playerObject = new GameObject("Player");
        playerObject.tag = "Player";
        playerComponent = playerObject.AddComponent<Player>();
        try
        {
            // Initialiser la grille avec la taille spécifiée
            Grid2D<CellType>.Initialize(size, Vector2Int.zero);
        }
        catch (System.InvalidOperationException ex)
        {
            Debug.LogError("Grid2D<CellType> is already initialized. Error: " + ex.Message);
            return;
        }
        grid = Grid2D<CellType>.Instance;
        rooms = new List<Room>();
        CoinsPoint = new List<Vector2>();
        EnemyPoint = new List<Vector2>();

        PlaceRooms();
        Triangulate();
        CreateHallways();
        PathfindHallways();
        PlaceItems();
    }


    void PlaceRooms() {
        for (int i = 0; i < roomCount; i++) {
            Vector2Int location = new Vector2Int(
                UnityEngine.Random.Range(0, size.x),
                UnityEngine.Random.Range(0, size.y)
            );

            Vector2Int roomSize = new Vector2Int(
                UnityEngine.Random.Range(roomMinSize.x, roomMaxSize.x + 1),
                UnityEngine.Random.Range(roomMinSize.y, roomMaxSize.y + 1)
            );

            // Vérifier que la taille de la nouvelle salle est supérieure ou égale à roomMinSize
            if (roomSize.x < roomMinSize.x || roomSize.y < roomMinSize.y)
            {
                roomSize = roomMinSize; // Ajuster la taille si elle est inférieure à la taille minimale
            }

            bool add = true;
            Room newRoom = new Room(location, roomSize);
            Room buffer = new Room(location + new Vector2Int(-1, -1), roomSize + new Vector2Int(2, 2));

            foreach (var room in rooms) {
                if (Room.Intersect(room, buffer)) {
                    add = false;
                    break;
                }
            }

            if (newRoom.bounds.xMin < 0 || newRoom.bounds.xMax >= size.x
                || newRoom.bounds.yMin < 0 || newRoom.bounds.yMax >= size.y) {
                add = false;
            }

            if (add) {
                rooms.Add(newRoom);
                dungeonTileMapVisualizer.PaintFloorTiles(newRoom.GetAllPoints());

                foreach (var pos in newRoom.bounds.allPositionsWithin) {
                    grid[pos] = CellType.Room;
                }
            }
        }

    }

    void Triangulate() {
        List<Vertex> vertices = new List<Vertex>();

        foreach (var room in rooms) {
            vertices.Add(new Vertex<Room>((Vector2)room.bounds.position + ((Vector2)room.bounds.size) / 2, room));
        }

        delaunay = Delaunay2D.Triangulate(vertices);
    }

    void CreateHallways() {
        List<Prim.Edge> edges = new List<Prim.Edge>();

        foreach (var edge in delaunay.Edges) {
            edges.Add(new Prim.Edge(edge.U, edge.V));
        }

        List<Prim.Edge> mst = Prim.MinimumSpanningTree(edges, edges[0].U);

        selectedEdges = new HashSet<Prim.Edge>(mst);
    }

    void PathfindHallways() {
        DungeonPathfinder Dijkstra = DungeonPathfinder.GetInstance(size);
        
        hallways = new List<Vector2Int>();
        walls = new List<Vector2Int>();

        foreach (var edge in selectedEdges) {
            var startRoom = (edge.U as Vertex<Room>).Item;
            var endRoom = (edge.V as Vertex<Room>).Item;

             // Trouver les points les plus proches
             Vector2Int startPos = FindClosestPoint(startRoom.GetAllPoints(), endRoom.bounds.position);
             Vector2Int endPos = FindClosestPoint(endRoom.GetAllPoints(), startRoom.bounds.position);


            var path = Dijkstra.FindPath(startPos, endPos, (DungeonPathfinder.Node a, DungeonPathfinder.Node b) => {
                var pathCost = new DungeonPathfinder.PathCost();

                if (grid[b.Position] == CellType.Room)
                {
                    pathCost.cost = 10;
                }
                else if (grid[b.Position] == CellType.None)
                {
                    pathCost.cost = 5;
                }
                else if (grid[b.Position] == CellType.Hallway)
                {
                    pathCost.cost = 1;
                }

                pathCost.traversable = true;

                return pathCost;
            });


            List<Vector2Int> temp = new List<Vector2Int>();

            foreach (var pos in path)
            {
                if (grid[pos + Direction2D.cardinalPoint[2]] == CellType.None && grid[pos + Direction2D.cardinalPoint[3]] == CellType.None)
                {
                    temp.Add(pos + Direction2D.cardinalPoint[2]);
                    temp.Add(pos + Direction2D.cardinalPoint[2] + Direction2D.cardinalPoint[2]);
                }if (grid[pos + Direction2D.cardinalPoint[0]] == CellType.None && grid[pos + Direction2D.cardinalPoint[1]] == CellType.None)
                {
                    temp.Add(pos + Direction2D.cardinalPoint[1]);
                    temp.Add(pos + Direction2D.cardinalPoint[1] + Direction2D.cardinalPoint[1]);
                }
                if (grid[pos + Direction2D.cardinalPoint[0]] == CellType.Room && grid[pos + Direction2D.cardinalPoint[1]] == CellType.None)
                {
                    temp.Add(pos + Direction2D.cardinalPoint[1]);
                    temp.Add(pos + Direction2D.cardinalPoint[1] + Direction2D.cardinalPoint[1]);
                }
            }

            foreach (var item in temp)
            {
                path.Add(item);
            }



            if (path != null) {
                for (int i = 0; i < path.Count; i++) {
                    var current = path[i];

                    if (grid[current] == CellType.None) {
                        grid[current] = CellType.Hallway;
                        hallways.Add(current);
                    }

                    if (i > 0) {
                        var prev = path[i - 1];

                        var delta = current - prev;
                    }
                }

            }
        }

        // visualize hallways
        foreach (var pos in hallways)
        {
            dungeonTileMapVisualizer.PaintHallway(pos);
        }
        List<List<Vector2Int>> allRoomPoints = new List<List<Vector2Int>>();
        foreach (var room in rooms)
        {
            allRoomPoints.Add(room.GetAllPoints());
        }
        // visualize walls
        walls = dungeonTileMapVisualizer.PaintRoomWalls(allRoomPoints, grid, hallways);

        foreach (var pos in walls)
        {
            if (grid[pos] == CellType.None)
                grid[pos] = CellType.Wall;
        }
    }

    // Méthode pour trouver le point le plus proche d'une position donnée parmi une liste de points
    public Vector2Int FindClosestPoint(List<Vector2Int> points, Vector2Int targetPosition)
    {
        Vector2Int closestPoint = points[0];
        float closestDistance = Vector2Int.Distance(closestPoint, targetPosition);

        foreach (var point in points)
        {
            float distance = Vector2Int.Distance(point, targetPosition);
            if (distance < closestDistance)
            {
                closestPoint = point;
                closestDistance = distance;
            }
        }

        return closestPoint;
    }

    private void PlaceItems()
    {
        // player item
        Room playerRoom = rooms[UnityEngine.Random.Range(0, rooms.Count)];
        playerComponent.Initialize(playerRoom.GetRandomPosition(), playerPrefab);
        playerComponent.SpawnPlayer();
        playerRoom.SetRoomType(RoomType.Player);

        // enemy and coins
        foreach (var room in rooms)
        {
            if (room.GetRoomType() == RoomType.None)
            {
                int enemyCount = UnityEngine.Random.Range(2, 3);
                int coinsCount = UnityEngine.Random.Range(4, 8);

                for (int i = 0; i < enemyCount; i++)
                {
                    Vector2 enemyPosition;
                    int attempts = 0;

                    do
                    {
                        enemyPosition = room.GetRandomPosition();
                        attempts++;
                    } while (!IsPositionValid(enemyPosition) && attempts < 100);

                    if (attempts >= 100)
                    {
                        Debug.LogWarning("Impossible de trouver une position valide pour un ennemi après 100 tentatives.");
                        continue;
                    }

                    GameObject enemyObject = new GameObject("Enemy");
                    Enemy enemyComponent = enemyObject.AddComponent<Enemy>();
                    enemyComponent.Initialize(enemyPosition, enemyPrefab);
                    enemyObject.tag = "Enemy";
                    enemyComponent.SpawnEnemy();
                    Debug.Log("Tag des enemy : " + enemyObject.tag);

                    // Add the enemy position and its neighbors to EnemyPoint
                    AddPositionAndNeighborsToEnemyPoints(enemyPosition, 3);

                    // Debug log for enemy positions
                    foreach (var item in EnemyPoint)
                    {
                        Debug.Log("this is enemy point: " + item);
                    }
                }

                for (int i = 0; i < coinsCount; i++)
                {
                    Vector2 coinPosition;
                    int attempts = 0;

                    do
                    {
                        coinPosition = room.GetRandomPosition();
                        attempts++;
                    } while (!IsPositionValid(coinPosition) && attempts < 100);

                    if (attempts >= 100)
                    {
                        Debug.LogWarning("Impossible de trouver une position valide pour une pièce après 100 tentatives.");
                        continue;
                    }

                    Instantiate(coinPrefab, coinPosition, Quaternion.identity);
                    CoinsPoint.Add(coinPosition);
                }
            }
        }

        foreach (var item in EnemyPoint)
        {
            Debug.Log("this is enemy point: " + item);
        }
    }

    private void AddPositionAndNeighborsToEnemyPoints(Vector2 position, float rayon)
    {
        List<Vector2> neighbors = GetNeighbors(position, rayon);
        EnemyPoint.Add(position);
        EnemyPoint.AddRange(neighbors);
    }

    private List<Vector2> GetNeighbors(Vector2 position, float rayon)
    {
        List<Vector2> neighbors = new List<Vector2>();
        int rayonInt = Mathf.CeilToInt(rayon);

        for (int x = -rayonInt; x <= rayonInt; x++)
        {
            for (int y = -rayonInt; y <= rayonInt; y++)
            {
                Vector2 neighbor = new Vector2(position.x + x, position.y + y);
                if (Vector2.Distance(position, neighbor) <= rayon)
                {
                    neighbors.Add(neighbor);
                }
            }
        }

        return neighbors;
    }

    private bool IsPositionValid(Vector2 position)
    {
        // Vérifiez si la position est suffisamment éloignée des autres ennemis
        foreach (var enemy in EnemyPoint)
        {
            if (Vector2.Distance(position, enemy) < minDistance)
            {
                return false;
            }
        }

        // Vérifiez si la position est suffisamment éloignée des pièces
        foreach (var coin in CoinsPoint)
        {
            if (Vector2.Distance(position, coin) < minDistance)
            {
                return false;
            }
        }

        return true;
    }

    void StartCollectingCoins()
    {
        StartCoroutine(ProcessCollectCoins());
    }

    private Dictionary<(Vector2Int, Vector2Int), List<Vector2Int>> pathMemo = new Dictionary<(Vector2Int, Vector2Int), List<Vector2Int>>();
    private Dictionary<Vector2Int, float> coutMemo = new Dictionary<Vector2Int, float>();

    IEnumerator ProcessCollectCoins()
    {
        while (CoinsPoint.Count > 0)
        {
            Vector2Int playerCurrentPosition = Vector2Int.RoundToInt(playerComponent.GetPlayerPosition());
            Vector2Int nearestCoin = FindNearestPoint(playerComponent.GetPlayerPosition());
            var path = FindStockedPath(playerCurrentPosition, nearestCoin);

            if (path != null)
            {

                foreach (var position in path)
                {
                    if (EnemyPoint.Contains(new Vector2(position.x, position.y)))
                    {
                        Debug.Log("Encountered enemy, backtracking...");
                        yield return new WaitForSeconds(2); // Pause for 2 seconds

                        // Move back 3 positions or to the start of the path
                        int stepsBack = Mathf.Min(5, path.IndexOf(position));
                        Vector2Int backtrackPosition = path[path.IndexOf(position) - stepsBack];

                        yield return MovePlayer(backtrackPosition);

                        yield return new WaitForSeconds(2);

                        // Find a new path from the backtrack position
                        path = FindStockedPath(backtrackPosition, nearestCoin);

                        if (path == null)
                        {
                            Debug.LogWarning("No path found to collect the coins!");
                            break;
                        }
                    }
                    else
                    {
                        yield return MovePlayer(position);
                        CollectCoinAtPosition(position);
                    }
                }
            }
            else
            {
                Debug.LogWarning("No path found to collect the coins!");
            }
        }
    }

    private List<Vector2Int> FindStockedPath(Vector2Int start, Vector2Int end)
    {
        if (pathMemo.TryGetValue((start, end), out var chemin))
        {
            return chemin;
        }

        DungeonPathfinder Dijkstra = DungeonPathfinder.GetInstance(size);
        var path = Dijkstra.FindPath(start, end, GetPlayerToCoinPathCost);

        if (path != null)
        {
            pathMemo[(start, end)] = path;
            coutMemo[end] = CalculerCoutPath(path);
        }

        return path;
    }

    private float CalculerCoutPath(List<Vector2Int> path)
    {
        float coutTotal = 0;
        for (int i = 0; i < path.Count - 1; i++)
        {
            var a = path[i];
            var b = path[i + 1];
            coutTotal += GetPlayerToCoinPathCost(new DungeonPathfinder.Node(a), new DungeonPathfinder.Node(b)).cost;
        }
        return coutTotal;
    }

    DungeonPathfinder.PathCost GetPlayerToCoinPathCost(DungeonPathfinder.Node a, DungeonPathfinder.Node b)
    {
        var pathCost = new DungeonPathfinder.PathCost();

        if (grid[b.Position] == CellType.Room)
        {
            pathCost.cost = 10;
            pathCost.traversable = true;
        }
        else if (grid[b.Position] == CellType.None || grid[b.Position] == CellType.Wall)
        {
            pathCost.cost = float.PositiveInfinity;
            pathCost.traversable = false;
        }
        else if (grid[b.Position] == CellType.Hallway)
        {
            pathCost.cost = 1;
            pathCost.traversable = true;
        }
        else if ((grid[b.Position] == CellType.Hallway && grid[b.Position + (2 * Direction2D.cardinalPoint[3])] == CellType.None)
                || (grid[b.Position] == CellType.Hallway && grid[b.Position + (2 * Direction2D.cardinalPoint[2])] == CellType.None)
                || (grid[b.Position] == CellType.Hallway && grid[b.Position + (2 * Direction2D.cardinalPoint[0])] == CellType.None)
                || (grid[b.Position] == CellType.Hallway && grid[b.Position + (2 * Direction2D.cardinalPoint[1])] == CellType.None))
        {
            pathCost.cost = float.PositiveInfinity;
            pathCost.traversable = false;
        }
        else if ((grid[b.Position] == CellType.Room && grid[b.Position + (2 * Direction2D.cardinalPoint[3])] == CellType.None)
                || (grid[b.Position] == CellType.Room && grid[b.Position + (2 * Direction2D.cardinalPoint[2])] == CellType.None)
                || (grid[b.Position] == CellType.Room && grid[b.Position + (2 * Direction2D.cardinalPoint[0])] == CellType.None)
                || (grid[b.Position] == CellType.Room && grid[b.Position + (2 * Direction2D.cardinalPoint[1])] == CellType.None))
        {
            pathCost.cost = float.PositiveInfinity;
            pathCost.traversable = false;
        }

        return pathCost;
    }

    IEnumerator MovePlayer(Vector2Int targetPosition)
    {
        Vector2 startPosition = playerComponent.GetPlayerPosition();
        Vector2 endPosition = new Vector2(targetPosition.x, targetPosition.y);
        float elapsedTime = 0f;
        float moveDuration = 0.10f; // Adjust this value for speed

        while (elapsedTime < moveDuration)
        {
            playerComponent.MoveTo(Vector2.Lerp(startPosition, endPosition, elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        playerComponent.MoveTo(endPosition);
    }

    private void CollectCoinAtPosition(Vector2Int position)
    {
        Debug.Log($"Pièce collectée à la position {position}");
    }

    Vector2Int FindNearestPoint(Vector2 startPoint)
    {
        if (CoinsPoint == null || CoinsPoint.Count == 0)
        {
            Debug.LogError("La liste des points est vide ou nulle.");
            return Vector2Int.zero;
        }

        Vector2Int nearestPoint = new Vector2Int(Mathf.RoundToInt(CoinsPoint[0].x), Mathf.RoundToInt(CoinsPoint[0].y));
        float minDistance = Vector2.Distance(startPoint, nearestPoint);

        foreach (Vector2 point in CoinsPoint)
        {
            int i = 0;
            float distance = Vector2.Distance(startPoint, point);
            if (distance < minDistance)
            {
                List<Vector2Int> onlinePoints = GetPointsOnLine(startPoint, point);
                foreach (var pos in onlinePoints)
                {
                    if (grid[pos] == CellType.None)
                    {
                        i++;
                    }
                }
                if (i == 0)
                {
                    nearestPoint = new Vector2Int(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y));
                    minDistance = distance;
                }
            }
        }

        CoinsPoint.Remove(nearestPoint);

        return nearestPoint;
    }

    List<Vector2Int> GetPointsOnLine(Vector2 startPoint, Vector2 endPoint)
    {
        List<Vector2Int> pointsOnLine = new List<Vector2Int>();

        int x0 = Mathf.RoundToInt(startPoint.x);
        int y0 = Mathf.RoundToInt(startPoint.y);
        int x1 = Mathf.RoundToInt(endPoint.x);
        int y1 = Mathf.RoundToInt(endPoint.y);

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);

        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;

        int err = dx - dy;

        while (true)
        {
            pointsOnLine.Add(new Vector2Int(x0, y0));

            if (x0 == x1 && y0 == y1) break;

            int e2 = 2 * err;

            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }

        return pointsOnLine;
    }

}



public enum CellType
{
    None,
    Room,
    Hallway,
    Wall,
    Player,
    Coin,
    Enemy
}


public static class Direction2D
{
    public static List<Vector2Int> cardinalPoint = new List<Vector2Int> {
            new Vector2Int(0, 1), //up
            new Vector2Int(0, -1), //down
            new Vector2Int(1, 0), //right
            new Vector2Int(-1, 0) //left
        };
}

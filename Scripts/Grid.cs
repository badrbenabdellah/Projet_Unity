using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Grid.cs
public class GridPathFinder<T>
{
    // Tableau pour stocker les données de la grille
    T[] data;

    public Vector2Int Size { get; private set; }

    // Propriété pour le décalage de la grille par rapport à l'origine
    public Vector2Int Offset { get; set; }

    // Constructeur de la classe Grid2D
    public GridPathFinder(Vector2Int size, Vector2Int offset)
    {
        Size = size;
        Offset = offset;

        // Initialisation du tableau de données avec la taille de la grille
        data = new T[size.x * size.y];
    }

    // Méthode pour obtenir l'index dans le tableau à partir d'une position
    public int GetIndex(Vector2Int pos)
    {
        return pos.x + (Size.x * pos.y);
    }

    // Méthode pour vérifier si une position est à l'intérieur des limites de la grille
    public bool InBounds(Vector2Int pos)
    {
        return new RectInt(Vector2Int.zero, Size).Contains(pos + Offset);
    }

    // Indexeur pour accéder à un élément de la grille en utilisant des coordonnées x et y
    public T this[int x, int y]
    {
        get
        {
            return this[new Vector2Int(x, y)];
        }
        set
        {
            this[new Vector2Int(x, y)] = value;
        }
    }

    // Indexeur pour accéder à un élément de la grille en utilisant une position Vector2Int
    public T this[Vector2Int pos]
    {
        get
        {
            pos += Offset;
            return data[GetIndex(pos)];
        }
        set
        {
            pos += Offset;
            data[GetIndex(pos)] = value;
        }
    }
}

public class Grid2D<T>
{
    private static Grid2D<T> instance = null;
    private static readonly object padlock = new object();

    T[] data;

    public Vector2Int Size { get; private set; }
    public Vector2Int Offset { get; set; }

    private Grid2D(Vector2Int size, Vector2Int offset)
    {
        Size = size;
        Offset = offset;
        data = new T[size.x * size.y];
    }

    public static void Initialize(Vector2Int size, Vector2Int offset)
    {
        lock (padlock)
        {
            if (instance == null)
            {
                instance = new Grid2D<T>(size, offset);
            }
            else
            {
                throw new System.InvalidOperationException("Grid2D is already initialized.");
            }
        }
    }

    public static Grid2D<T> Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    throw new System.InvalidOperationException("Grid2D is not initialized. Call Initialize() first.");
                }
                return instance;
            }
        }
    }

    public int GetIndex(Vector2Int pos)
    {
        return pos.x + (Size.x * pos.y);
    }

    public bool InBounds(Vector2Int pos)
    {
        return new RectInt(Vector2Int.zero, Size).Contains(pos + Offset);
    }

    public T this[int x, int y]
    {
        get { return this[new Vector2Int(x, y)]; }
        set { this[new Vector2Int(x, y)] = value; }
    }

    public T this[Vector2Int pos]
    {
        get
        {
            pos += Offset;
            return data[GetIndex(pos)];
        }
        set
        {
            pos += Offset;
            data[GetIndex(pos)] = value;
        }
    }
}

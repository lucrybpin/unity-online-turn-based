using UnityEngine;

[System.Serializable]
public class Grid
{
    [SerializeField] private int [,] grid;

    public Grid(int sizeX, int sizeZ)
    {
        grid = new int[sizeX, sizeZ];

        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                grid[x,z] = -1;
            }
        }
    }

    public int GetXLength()
    {
        return grid.GetLength(0);
    }

    public int GetZLength()
    {
        return grid.GetLength(1);
    }

    public static Vector2 WorldToGridPosition(Vector3 worldPosition)
    {
        return new Vector2(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.z));
    }
}

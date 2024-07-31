using UnityEngine;

public enum GridObjectType
{
    Empty,
    Obstacle,
    Character
}

[System.Serializable]
public struct GridObject
{
    public GridObjectType Type;
    public ulong ParticipantId;

    public GridObject(GridObjectType type = GridObjectType.Empty, ulong participantId = ulong.MaxValue)
    {
        Type = type;
        ParticipantId = participantId;
    }
}

[System.Serializable]
public class GridSystem
{
    [SerializeField] private GridObject [,] grid;

    public GridObject[,] Grid { get => grid; }

    public GridSystem(int sizeX, int sizeZ)
    {
        grid = new GridObject[sizeX, sizeZ];

        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                grid[x,z] = new GridObject();
            }
        }
    }

    public static Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.z));
    }

    public void SetObstacle(int x, int z)
    {
        grid[x,z].Type = GridObjectType.Obstacle;
    }

    public void SetCharacter(int x, int z, ulong participantId)
    {
        grid[x,z].Type = GridObjectType.Character;
        grid[x,z].ParticipantId = participantId;
    }
    public void SetCharacter(Vector2Int position, ulong participantId)
    {
        grid[position.x, position.y].Type = GridObjectType.Character;
        grid[position.x, position.y].ParticipantId = participantId;
    }
}

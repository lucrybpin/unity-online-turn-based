using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
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
    // If not public, it will not be serialized
    [SerializeField] public GridObject[,] grid;

    public GridSystem(int sizeX, int sizeZ)
    {
        grid = new GridObject[sizeX, sizeZ];

        for (int x = 0; x < sizeX; x++)
        {
            List<int> row = new List<int>();
            for (int z = 0; z < sizeZ; z++)
            {
                grid[x, z] = new GridObject();
            }
        }
    }

    public static Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.z));
    }

    public void SetObstacle(int x, int z)
    {
        grid[x, z].Type = GridObjectType.Obstacle;
    }

    public void SetCharacter(int x, int z, ulong participantId)
    {
        grid[x, z].Type = GridObjectType.Character;
        grid[x, z].ParticipantId = participantId;
    }
    public void SetCharacter(Vector2Int position, ulong participantId)
    {
        grid[position.x, position.y].Type = GridObjectType.Character;
        grid[position.x, position.y].ParticipantId = participantId;
    }

    public List<Vector2> AStar(Vector2 origin, Vector2 destination)
    {
        List<Vector2> openList = new List<Vector2>();
        HashSet<Vector2> closedList = new HashSet<Vector2>();
        Dictionary<Vector2, Vector2> cameFrom = new Dictionary<Vector2, Vector2>();

        Dictionary<Vector2, float> gScore = new Dictionary<Vector2, float>();
        Dictionary<Vector2, float> fScore = new Dictionary<Vector2, float>();

        openList.Add(origin);
        gScore[origin] = 0;
        fScore[origin] = HeuristicCostEstimate(origin, destination);

        while (openList.Count > 0)
        {
            Vector2 current = GetNodeWithLowestFScore(openList, fScore);

            if (current == destination)
                return ReconstructPath(cameFrom, current);

            openList.Remove(current);
            closedList.Add(current);

            foreach (Vector2 neighbor in GetNeighbors(current))
            {
                if (closedList.Contains(neighbor) || grid[(int)neighbor.x, (int)neighbor.y].Type == GridObjectType.Obstacle)
                    continue;

                float tentativeGScore = gScore[current] + DistanceBetween(current, neighbor);

                if (!openList.Contains(neighbor))
                    openList.Add(neighbor);
                else if (tentativeGScore >= gScore[neighbor])
                    continue;

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, destination);
            }
        }

        return null;
    }

    private float HeuristicCostEstimate(Vector2 a, Vector2 b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private Vector2 GetNodeWithLowestFScore(List<Vector2> openList, Dictionary<Vector2, float> fScore)
    {
        Vector2 bestNode = openList[0];
        float bestFScore = fScore.ContainsKey(bestNode) ? fScore[bestNode] : float.MaxValue;

        foreach (Vector2 node in openList)
        {
            float score = fScore.ContainsKey(node) ? fScore[node] : float.MaxValue;
            if (score < bestFScore)
            {
                bestFScore = score;
                bestNode = node;
            }
        }

        return bestNode;
    }

    private List<Vector2> GetNeighbors(Vector2 node)
    {
        List<Vector2> neighbors = new List<Vector2>();

        Vector2[] directions = new Vector2[]
        {
            new Vector2(1, 0),
            new Vector2(-1, 0),
            new Vector2(0, 1),
            new Vector2(0, -1)
        };

        foreach (Vector2 direction in directions)
        {
            Vector2 neighborPos = node + direction;
            if (IsPositionInsideGrid(neighborPos))
            {
                neighbors.Add(neighborPos);
            }
        }

        return neighbors;
    }

    private bool IsPositionInsideGrid(Vector2 position)
    {
        return position.x >= 0 && position.x < grid.GetLength(0) && position.y >= 0 && position.y < grid.GetLength(1);
    }

    private float DistanceBetween(Vector2 a, Vector2 b)
    {
        return Vector2.Distance(a, b);
    }

    private List<Vector2> ReconstructPath(Dictionary<Vector2, Vector2> cameFrom, Vector2 current)
    {
        List<Vector2> totalPath = new List<Vector2> { current };

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Add(current);
        }

        totalPath.Reverse();
        return totalPath;
    }
}

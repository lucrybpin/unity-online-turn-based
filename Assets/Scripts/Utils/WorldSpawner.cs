using UnityEngine;

public class WorldSpawner : MonoBehaviour
{
    [SerializeField] private StartingCell _startingCellPrefab;
    [SerializeField] private GameObject _participantPrefab;
    
    public static WorldSpawner Instance;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public StartingCell SpawnStartingCell(Vector3 position)
    {
        StartingCell startingCell = Instantiate(_startingCellPrefab, position, Quaternion.identity);
        return startingCell;
    }

    public GameObject SpawnParticipant(Vector3 position)
    {
        GameObject participant = Instantiate(_participantPrefab, position, Quaternion.identity);
        return participant;
    }
}

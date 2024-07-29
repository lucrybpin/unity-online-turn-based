using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ServerController : NetworkBehaviour 
{
    [SerializeField] private ClientController _clients;
    [SerializeField] private List<ulong> _connectedParticipants; // this could be a hashset but Editor can't serialize it
    [SerializeField] private GameState _gameState;
    
    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void Update()
    {
        if(!IsServer)
            return;

        // Match Preparing
        if(_gameState.matchState == MatchState.Preparing)
        {
            _gameState.TimeLeft -= Time.deltaTime;
            if(_gameState.TimeLeft <= 0)
            {
                OnStartCombat();
                Debug.Log("Server - Starting Combat (no time left)");
            }
        }

        // Match Combat
        if(_gameState.matchState == MatchState.Combat)
        {
            _gameState.TimeLeft -= Time.deltaTime;
            if (_gameState.TimeLeft <= 0)
            {
                OnNextTurn();
            }
        }
    }

    // Events

    private void OnServerStarted()
    {
        Debug.Log("Server - Server Started");
        _connectedParticipants = new List<ulong>();
        _gameState = PrepareGame();
        _gameState.matchState = MatchState.Preparing;
    }

    private void OnClientConnected(ulong connectedClientId)
    {
        if(!IsServer)
            return;

        Debug.Log($"Server - Client Connected: {connectedClientId}");
        
        // First Connection
        if(!_gameState.Participants.Exists(p => p.ParticipantId == connectedClientId))
        {
            ParticipantData playerJoined = new ParticipantData();
            playerJoined.ParticipantId = connectedClientId;
            playerJoined.Initiative = Random.Range(0, 10000); // TODO: Get Participant Data from Server
            playerJoined.CurrentLife = 100;
            
            // Find Available Starting Position
            foreach (Vector3 startingPosition in _gameState.StartingPositions)
            {
                ParticipantData defaultPlayer = default;
                ParticipantData playerData = _gameState.Participants.Find(x => x.Position == startingPosition);
                if (playerData.Equals(defaultPlayer))
                    playerJoined.Position = startingPosition;
            }
            _gameState.Participants.Add(playerJoined);
            _connectedParticipants.Add(connectedClientId);
        }
        // Reconnection
        else
        {
            _connectedParticipants.Add(connectedClientId);
        }
        
        string gameStateJson = JsonUtility.ToJson(_gameState);
        _clients.OnConnectedClientRpc(gameStateJson, connectedClientId);

    }

    private void OnClientDisconnected(ulong disconnectedClientId)
    {
        Debug.Log($"Server - Client Disconnected: {disconnectedClientId}");
        _connectedParticipants.Remove(disconnectedClientId);
        _clients.OnDisconnectedClientRpc(disconnectedClientId);
    }

    private void OnStartCombat()
    {
        _gameState.matchState = MatchState.Combat;
        _gameState.TimeLeft = 25;
        _gameState.CurrentTurnIndex = 0;
        _gameState.SortParticipantsByInitiative();
        _clients.StartCombatClientRpc();
    }

    private void OnNextTurn()
    {
        Debug.Log("Server - Next Turn");
        _gameState.TimeLeft = 25;
        _gameState.CurrentTurnIndex = GetNextTurnIndex();
        ulong clientId = _gameState.Participants[_gameState.CurrentTurnIndex].ParticipantId;
        _clients.OnTurnStartClientRpc(clientId);
    }

    // Client Actions

    [ServerRpc(RequireOwnership = false)]
    public void ClientRepositionServerRpc(Vector3 requestedPosition, ServerRpcParams serverRpcParams = default)
    {
        if(!IsServer)
            return;

        if(_gameState.matchState != MatchState.Preparing)
            return;

        // Try Reposition
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        int playerIndexOccupyingCell =_gameState.Participants.FindIndex(p => p.Position == requestedPosition);
        if(playerIndexOccupyingCell == -1)
        {
            // Available Cell
            ParticipantData playerData = _gameState.GetParticipantData(clientId);
            if(playerData.IsReady)
            {
                Debug.Log($"Server - Match Preparation - Client {clientId} can't move because he is Ready");   
                return;
            }

            // Update Position
            playerData.Position = requestedPosition;
            _gameState.UpdatePlayerData(clientId, playerData);
            Debug.Log($"Server - Match Preparation - Client {clientId} moved to starting cell {requestedPosition}");
            _clients.RepositionClientRpc(clientId, requestedPosition);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ClientIsReadyServerRpc(bool isReady, ServerRpcParams serverRpcParams = default)
    {
        if(!IsServer)
            return;

        if(_gameState.matchState != MatchState.Preparing)
            return;

        Debug.Log($"Server - Client changed is Ready");

        ulong clientId = serverRpcParams.Receive.SenderClientId;
        ParticipantData participantData = _gameState.GetParticipantData(clientId);
        participantData.IsReady = isReady;
        _gameState.UpdatePlayerData(clientId, participantData);
        _clients.UpdateReadyStatusClientRpc(clientId, isReady);

        if(_gameState.AllPlayersAreReady())
        {
            OnStartCombat();
            Debug.Log("Server - Starting Combat (all players are ready)");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ClientPassTurnServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        if(clientId == _gameState.Participants[_gameState.CurrentTurnIndex].ParticipantId)
            OnNextTurn();
    }

    // Support Methods

    private GameState PrepareGame()
    {
        Debug.Log("Server - Perparing Game");
        GameState gameState = new GameState();

        // Initial State
        gameState.matchState = MatchState.Preparing;

        // Starting Positions
        gameState.StartingPositions = new List<Vector3>();
        gameState.StartingPositions.Add(new Vector3(17, 0, 19));
        gameState.StartingPositions.Add(new Vector3(2, 0, 1));

        gameState.StartingPositions.Add(new Vector3(7, 0, 19));
        gameState.StartingPositions.Add(new Vector3(12, 0, 1));

        gameState.StartingPositions.Add(new Vector3(1, 0, 18));
        gameState.StartingPositions.Add(new Vector3(18, 0, 2));

        gameState.StartingPositions.Add(new Vector3(17, 0, 1));
        gameState.StartingPositions.Add(new Vector3(2, 0, 19));

        gameState.StartingPositions.Add(new Vector3(7, 0, 1));
        gameState.StartingPositions.Add(new Vector3(12, 0, 19));

        gameState.StartingPositions.Add(new Vector3(1, 0, 2));
        gameState.StartingPositions.Add(new Vector3(18, 0, 18));
        // gameState.StartingPositions.Add(new Vector3(0, 0, 0));
        Debug.Log("Server - Created Starting Positions");
        
        // Participants
        gameState.Participants = new List<ParticipantData>();

        // Time Left
        gameState.TimeLeft = 10;
        Debug.Log("Server - Game Prepared");

        return gameState;
    }

    public int GetNextTurnIndex()
    {
        int turnIndex = (_gameState.CurrentTurnIndex + 1) % _gameState.Participants.Count;
        ParticipantData participantData = _gameState.Participants[turnIndex];
        while (participantData.CurrentLife <= 0)
        {
            turnIndex = (turnIndex + 1) % _gameState.Participants.Count;
            participantData = _gameState.Participants[turnIndex];
        }
        return turnIndex;
    }
}

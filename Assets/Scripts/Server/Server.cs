using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Server : NetworkBehaviour 
{
    [SerializeField] private Client _clients;
    [SerializeField] private GameState _gameState;
    private ServerMatchPreparation _matchPreparation;
    private ServerMatchCombat _matchCombat;
    private MatchState _lastMatchState;


    private void Start()
    {
        // Create GameState
        _gameState = new GameState();
        _gameState.matchState = MatchState.Preparing;

        _gameState.TimeLeft = 10;

        _gameState.StartingPositions = new List<Vector3>();
        _gameState.StartingPositions.Add(new Vector3(1,0,2));
        _gameState.StartingPositions.Add(new Vector3(2,0,1));

        _gameState.StartingPositions.Add(new Vector3(7,0,1));
        _gameState.StartingPositions.Add(new Vector3(12,0,1));

        _gameState.StartingPositions.Add(new Vector3(17,0,1));
        _gameState.StartingPositions.Add(new Vector3(18,0,2));

        _gameState.StartingPositions.Add(new Vector3(1,0,18));
        _gameState.StartingPositions.Add(new Vector3(2,0,19));

        _gameState.StartingPositions.Add(new Vector3(7,0,19));
        _gameState.StartingPositions.Add(new Vector3(12,0,19));

        _gameState.StartingPositions.Add(new Vector3(17,0,19));
        _gameState.StartingPositions.Add(new Vector3(18,0,18));

        _gameState.Participants = new List<ParticipantData>();

        _matchPreparation = new ServerMatchPreparation();
        _matchCombat = new ServerMatchCombat();

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void Update()
    {
        if(!IsServer)
            return;

        _lastMatchState = _gameState.matchState;

        // Current State
        switch (_gameState.matchState)
        {
            case MatchState.Preparing:
                _gameState = _matchPreparation.HandleUpdate(Time.deltaTime, _gameState);
                break;

            case MatchState.Combat:
                _gameState = _matchCombat.HandleUpdate(Time.deltaTime, _gameState);
                break;
        }

        // Changed State
        if(_lastMatchState != _gameState.matchState)
        {
            OnEnterState(_gameState.matchState);
        }

    }

    private void OnEnterState(MatchState matchState)
    {
        switch (matchState)
        {
            case MatchState.Combat:
                _matchCombat.StartCombat(_gameState, 60f, _clients);
            break;
        }

        _clients.OnEnterStateClientRpc(matchState);
    }

    private void OnClientConnected(ulong clientId)
    {
        if(!IsServer)
            return;

        switch (_gameState.matchState)
        {
            case MatchState.Preparing:
                _gameState = _matchPreparation.OnClientConnected(clientId, _gameState);
                break;

            case MatchState.Combat:
            default:
            break;
        }

        // Send GameState to Clients
        string gameStateJson = JsonUtility.ToJson(_gameState);
        _clients.OnClientConnectedClientRpc(gameStateJson, clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ClientRepositionServerRpc(Vector3 requestedPosition, ServerRpcParams serverRpcParams = default)
    {
        if(!IsServer)
            return;

        if(_gameState.matchState != MatchState.Preparing)
            return;

        // Try Reposition
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        _gameState = _matchPreparation.ClientReposition(clientId, _gameState, requestedPosition);
        
        // Send GameState to Clients
        string gameStateJson = JsonUtility.ToJson(_gameState);
        _clients.UpdateGameStateClientRpc(gameStateJson);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ClientChangeIsReadyStatusServerRpc(bool isReady, ServerRpcParams serverRpcParams = default)
    {
        if(!IsServer)
            return;

        // Change Is Ready Status
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        _matchPreparation.ClientChangeIsReady(clientId, _gameState, isReady);

        // Send GameState to Clients
        string gameStateJson = JsonUtility.ToJson(_gameState);
        _clients.UpdateGameStateClientRpc(gameStateJson);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ClientPassTurnServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if(!IsServer)
            return;

        ulong clientId = serverRpcParams.Receive.SenderClientId;
        _matchCombat.ClientPassTurn(clientId, _gameState);
    }

}

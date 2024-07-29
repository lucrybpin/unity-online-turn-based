using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Client : NetworkBehaviour 
{
    [SerializeField] private GameState _gameState;
    [SerializeField] private Server _server;
    [SerializeField] private TMP_Text _timeText;
    [SerializeField] private ClientUIController _clientUIController;
    [SerializeField] private ClientCameraController _clientCameraController;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private GameObject _playerViewPrefab;
    [SerializeField] private GameObject _startingCellPrefab;
    private Dictionary<ulong, GameObject> _playerViews = new Dictionary<ulong, GameObject>();
    private Dictionary<Vector3, GameObject> _startingCellsObjects = new Dictionary<Vector3, GameObject>();

    private float _timeLeft;
    private bool _isReady = false;

    private void Start()
    {
        _clientUIController.EnableStartingCanvas();
    }

    private void Update()
    {
        if(_timeLeft >= 0){
            _timeLeft -= Time.deltaTime;
        }

        _clientUIController.OnUpdate(_timeLeft);
    }

    [ClientRpc]
    public void UpdateGameStateClientRpc(string gameStateJson)
    {
        GameState serverGameState = JsonUtility.FromJson<GameState>(gameStateJson);
        _gameState = serverGameState;
        _timeLeft = serverGameState.TimeLeft;

        // Render Players
        foreach(ParticipantData player in serverGameState.Participants)
        {
            if(_playerViews.TryGetValue(player.ParticipantId, out GameObject playerViewFound))
            {
                playerViewFound.transform.position = player.Position;
                // if(_gameState.matchState == MatchState.Combat)
                    // _cameraController.UpdatePosition();
            }
            else
            {
                GameObject playerView = Instantiate(_playerViewPrefab, player.Position, Quaternion.identity);
                _playerViews.Add(player.ParticipantId, playerView);
            }
        }
    }

    [ClientRpc]
    public void SetTimeLeftClientRpc(float timeLeft, ClientRpcParams clientRpcParams)
    {
        Debug.Log($"client time set: {timeLeft}");
        _timeLeft = timeLeft;
    }

    [ClientRpc]
    public void OnClientConnectedClientRpc(string gameStateJson, ulong newClientConnected)
    {
        GameState serverGameState = JsonUtility.FromJson<GameState>(gameStateJson);

        _clientUIController.DisableStartingCanvas();
        _clientUIController.EnableMatchPreparingCanvas();

        // Spawn Starting Position Cells
        foreach (Vector3 position in serverGameState.StartingPositions)
        {
            if (!_startingCellsObjects.TryGetValue(position, out GameObject startingCellObject))
            {
                GameObject staringCell = Instantiate(_startingCellPrefab, position, Quaternion.identity);
                _startingCellsObjects.Add(position, staringCell);
            }
        }
        UpdateGameStateClientRpc(gameStateJson);

        //Setup Camera
        // if(NetworkManager.Singleton.LocalClientId == newClientConnected)
        // {
        //     Transform myParticipantView = GetMyView().transform;
        //     _cameraController.SetTarget(myParticipantView);
        // }
    }

    [ClientRpc]
    public void OnEnterStateClientRpc(MatchState matchState)
    {
        switch (matchState)
        {
            case MatchState.Preparing:
            _clientUIController.DisableStartingCanvas();
            _clientUIController.DisableMatchCombatCanvas();
            _clientUIController.EnableMatchPreparingCanvas();
            break;

            case MatchState.Combat:
            _clientUIController.DisableStartingCanvas();
            _clientUIController.DisableMatchPreparingCanvas();
            _clientUIController.EnableMatchCombatCanvas();
            break;
        }
    }

    public void ClientAskForReposition(Vector3 position)
    {
        _server.ClientRepositionServerRpc(position);
    }

    [ClientRpc]
    public void OnStartCombatClientRpc()
    {
        Debug.Log("Client - Combat Started");
        // _cameraController.DisableTopDownCamera();
        Transform myParticipantView = GetMyView().transform;
        // _cameraController.SetTarget(myParticipantView);
    }

    [ClientRpc]
    public void OnStartTurnClientRpc(float timeLeft, ClientRpcParams clientParameters)
    {
        Debug.Log("Client - My Turn Started");
        _timeLeft = timeLeft;
    }

    public void OnButtonClickIsReady()
    {
        _isReady = !_isReady;
        _server.ClientChangeIsReadyStatusServerRpc(_isReady);
    }

    public void OnButtonPassTurn()
    {
        _server.ClientPassTurnServerRpc();
    }

    private GameObject GetMyView()
    {
        GameObject participantView;
        if(_playerViews.TryGetValue(NetworkManager.Singleton.LocalClientId, out participantView)){}
        else
            Debug.Log("Client - Couldnt find my view");
        return participantView;
    } 
}

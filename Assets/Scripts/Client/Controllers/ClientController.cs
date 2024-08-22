using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;

public class ClientController : NetworkBehaviour 
{
    [SerializeField] private ServerController _serverController;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private UIController _uiController;
    [SerializeField] private GameState _gameState;

    private InputController _inputController;

    private Dictionary<ulong, GameObject> _participantViews = new Dictionary<ulong, GameObject>();
    
    // Unity Events

    public void Start()
    {
        _inputController = new InputController(this);
    }

    public void Update()
    {
        InputResult inputResult = _inputController.Update(_cameraController.CurrentCamera);
        HandleUserInput(inputResult);

        // Match Preparing
        if (_gameState.matchState == MatchState.Preparing)
        {
            _gameState.TimeLeft -= Time.deltaTime;
            if (_gameState.TimeLeft > 0)
                _uiController.SetTimeLeft(_gameState.TimeLeft);
        }

        // Match Combat
        if (_gameState.matchState == MatchState.Combat)
        {
            _gameState.TimeLeft -= Time.deltaTime;
            if (_gameState.TimeLeft > 0)
                _uiController.SetTimeLeft(_gameState.TimeLeft);
        }
    }

    private void HandleUserInput(InputResult inputResult)
    {
        switch (_gameState.matchState)
        {
            case MatchState.Preparing:
            if (inputResult.LeftClick)
            {
                GameObject foundObject = inputResult.Objects.FirstOrDefault(o => o.GetComponent<StartingCell>() != null);
                if (foundObject != null)
                    AskForReposition(foundObject.transform.position);
            }
            break;   

            case MatchState.Combat:
            if (inputResult.LeftClick)
            {
                ParticipantData participantData = _gameState.GetParticipantData(NetworkManager.Singleton.LocalClientId);
                Vector2Int playerGridPosition = GridSystem.WorldToGridPosition(participantData.Position.ToVector3());
                Vector2Int clickGridPosition = GridSystem.WorldToGridPosition(inputResult.WorldPosition);
                List<Vector2> path = _gameState.GridSystem.AStar(playerGridPosition, clickGridPosition);
                Debug.Log($"click world position: {inputResult.WorldPosition}, x grid = {GridSystem.WorldToGridPosition(inputResult.WorldPosition)}");
            }
            break;
        }
    }

    // Ask to Server

    public void AskForReposition(Vector3 requestedPosition)
    {
        _serverController.ClientRepositionServerRpc(requestedPosition);
    }

    public void AskIsReady(bool isReady)
    {
        _serverController.ClientIsReadyServerRpc(isReady);
    }

    public void AskPassTurn()
    {
        _serverController.ClientPassTurnServerRpc();
    }

    // Server Answers

    [ClientRpc]
    public void OnConnectedClientRpc(string gameStateJson, ulong connectedClientId)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.Converters.Add(new Vector2IntConverter());
        _gameState = JsonConvert.DeserializeObject<GameState>(gameStateJson, settings); // JsonUtility.FromJson<GameState>(gameStateJson);
        
        if(IsMe(connectedClientId))
        {
            Debug.Log($"Client - I have connected to the Server");

            _inputController = new InputController(this);
            // UI Is Ready Click
            _uiController.UiControllerPreparingState.IsReadyButton.onClick.AddListener(() => {
                ulong myId = NetworkManager.Singleton.LocalClientId;
                ParticipantData participant = _gameState.GetParticipantData(myId);
                participant.IsReady = !participant.IsReady;
                AskIsReady(participant.IsReady);
            });
            // UI Pass Turn
            _uiController.UiControllerCombatState.PassTurnButton.onClick.AddListener(() => {
                AskPassTurn();
            });

            // Spawn Starting Positions
            foreach (NetworkVector3 position in _gameState.StartingPositions)
            {
                WorldSpawner.Instance.SpawnStartingCell(position.ToVector3());
            }

            // Add New Participant to List 
            foreach (ParticipantData participant in _gameState.Participants)
            {
                if(!_participantViews.TryGetValue(participant.ParticipantId, out GameObject participantViewFound))
                {
                    GameObject participantView = WorldSpawner.Instance.SpawnParticipant(participant.Position.ToVector3());
                    _participantViews.Add(participant.ParticipantId, participantView);
                }
            }

            GameObject myView = GetMyView();
            PositionParticipants(_gameState.Participants);
            _cameraController.SetMode(CameraControllerMode.TopDown);
            _cameraController.SetFocus(myView.transform.position);
            _uiController.SetMode(UIControllerState.PreparingState);
        }
        else
        {
            Debug.Log($"Client - Client Connected: {connectedClientId}");
            foreach (ParticipantData participant in _gameState.Participants)
            {
                if(_participantViews.TryGetValue(participant.ParticipantId, out GameObject participantViewFound))
                {
                    participantViewFound.transform.position = participant.Position.ToVector3();
                }
                else
                {
                    GameObject participantView = WorldSpawner.Instance.SpawnParticipant(participant.Position.ToVector3());
                    _participantViews.Add(participant.ParticipantId, participantView);
                }
            }
        }
        
    }

    [ClientRpc]
    public void OnDisconnectedClientRpc(ulong disconnectedClientId)
    {
        Debug.Log($"Client - Client Disconnected: {disconnectedClientId}");
    }

    [ClientRpc]
    public void RepositionClientRpc(ulong clientId, Vector3 newPosition)
    {
        ParticipantData participant = _gameState.GetParticipantData(clientId);
        participant.Position = new NetworkVector3(newPosition);
        _gameState.UpdatePlayerData(clientId, participant);
        // Render Players
        PositionParticipants(_gameState.Participants);
        Debug.Log($"Client - Participant {clientId} repositioned to {newPosition}");
    }
    
    [ClientRpc]
    public void StartCombatClientRpc()
    {
        Debug.Log($"Client - Combat Started");
        _gameState.matchState = MatchState.Combat;
        _gameState.CurrentTurnIndex = 0;

        //Hide Starting Cells
        StartingCell [] startingCells = FindObjectsOfType<StartingCell>();
        foreach (StartingCell startingCell in startingCells)
            Destroy(startingCell.gameObject);

        _uiController.SetMode(UIControllerState.CombatState);
    }

    [ClientRpc]
    public void OnTurnStartClientRpc(ulong clientId)
    {        
        _gameState.TimeLeft = 25;
        if(IsMe(clientId))
        {
            Debug.Log($"Client - My Turn");
            _cameraController.SetFocus(GetMyView().transform.position);
            return;
        }

        Debug.Log($"Client - Player {clientId}");
    }

    [ClientRpc]
    public void UpdateReadyStatusClientRpc(ulong clientId, bool isReady)
    {
        ParticipantData participant = _gameState.GetParticipantData(clientId);
        participant.IsReady = isReady;
        _gameState.UpdatePlayerData(clientId, participant);
    }

    // Support Methods
    
    private bool IsMe(ulong clientId)
    {
        return NetworkManager.Singleton.LocalClientId == clientId;
    }

    private GameObject GetMyView()
    {
        GameObject participantView;
        if(_participantViews.TryGetValue(NetworkManager.Singleton.LocalClientId, out participantView)){}
        else
            Debug.Log("Client - Couldnt find my view");
        return participantView;
    } 

    private void PositionParticipants(List<ParticipantData> participants)
    {
        foreach(ParticipantData participant in participants)
        {
            if(_participantViews.TryGetValue(participant.ParticipantId, out GameObject playerViewFound))
            {
                playerViewFound.transform.position = participant.Position.ToVector3();
            }
        }
    }
}

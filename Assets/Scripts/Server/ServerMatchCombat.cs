
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public class ServerMatchCombat 
{
    private float _turnTime;
    [SerializeField] private int _turnIndex;
    
    public GameState StartCombat(GameState gameState, float turnTime, Client _clients)
    {
        Debug.Log("Server - Match Combat - Combat Started");
        gameState.SortParticipantsByInitiative();
        _turnTime = turnTime;
        _turnIndex = 0;
        gameState.TimeLeft = _turnTime;

        _clients.OnStartCombatClientRpc();

        // Check if is a Player or AI
        if(gameState.Participants[_turnIndex].ParticipantId <= 499)
        {
            ulong participantId = gameState.Participants[_turnIndex].ParticipantId;
            ClientRpcParams clientParameters = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { participantId }
                }
            };
            Debug.Log($"Server - Match Combat - {participantId} turn");
            _clients.OnStartTurnClientRpc(_turnTime, clientParameters);// enviar para todos os clientes
        }
        else
        {
            // AI Turn
            Debug.LogError("Server - Match Combat - NOT IMPLEMENTED AI Turn");
        }

        return gameState;
    }

    public GameState HandleUpdate(float deltaTime, GameState gameState)
    {
        // Timer de Turnos
        if (gameState.matchState == MatchState.Combat)
        {
            gameState.TimeLeft -= deltaTime;
            if(gameState.TimeLeft <= 0)
            {
                // Next Turn
                gameState = PassTurn(gameState);
            }
        }
        return gameState;
    }

    public void ClientPassTurn(ulong userId, GameState gameState)
    {
        ulong currentTurnId = gameState.Participants[_turnIndex].ParticipantId;

        if(userId != currentTurnId)
        {
            return;
        }

        gameState = PassTurn(gameState);
        Debug.Log($"Server - Match Combat - next turn index: {_turnIndex}");
    }

    private GameState PassTurn(GameState gameState)
    {
        _turnIndex = (_turnIndex + 1) % gameState.Participants.Count;
        gameState.TimeLeft = _turnTime;
        Debug.Log($"Server - Match Combat - new turn: {_turnIndex}");
        return gameState;
    }
}

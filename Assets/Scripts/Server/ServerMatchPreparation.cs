using UnityEngine;

[System.Serializable]
public class ServerMatchPreparation
{
    // Update
    public GameState HandleUpdate(float deltaTime, GameState gameState)
    {
        if(gameState.matchState == MatchState.Preparing)
        {
            gameState.TimeLeft -= deltaTime;
            if(gameState.TimeLeft <= 0)
            {
                gameState.matchState = MatchState.Combat;
                Debug.Log("Server - Match Preparation - Starting Game (no time left)");
            }
        }
        return gameState;
    }

    // Client Connect
    public GameState OnClientConnected(ulong clientId, GameState gameState)
    {
        // Add Player to the Game
        ParticipantData playerJoined = new ParticipantData();
        playerJoined.ParticipantId = clientId;
        playerJoined.Initiative = Random.Range(0, 10000);

        foreach (Vector3 startingPosition in gameState.StartingPositions)
        {
            // Find available Starting Position
            ParticipantData defaultPlayer = default;
            ParticipantData playerData = gameState.Participants.Find(x => x.Position == startingPosition);

            // Available Starting Position
            if(playerData.Equals(defaultPlayer))
                playerJoined.Position = startingPosition;
        }
        gameState.Participants.Add(playerJoined);
        gameState.SortParticipantsByInitiative();

        return gameState;
    }

    // Client Actions
    public GameState ClientReposition(ulong clientId, GameState gameState, Vector3 requestedPosition)
    {
        int playerIndexOccupyingCell = gameState.Participants.FindIndex(x => x.Position == requestedPosition);
        if (playerIndexOccupyingCell == -1) 
        {
            // Available Cell
            ParticipantData playerData = gameState.GetParticipantData(clientId);
            if(playerData.IsReady)
            {
                Debug.Log($"Server - Match Preparation - Client {clientId} can't move because he is Ready");   
                return gameState;
            }

            // Update Position
            playerData.Position = requestedPosition;
            gameState.UpdatePlayerData(clientId, playerData);
            Debug.Log($"Server - Match Preparation - Client {clientId} moved to starting cell {requestedPosition}");
        }
        return gameState;
    }

    public GameState ClientChangeIsReady(ulong clientId, GameState gameState, bool isReady)
    {
        ParticipantData playerData = gameState.GetParticipantData(clientId);
        playerData.IsReady = isReady;
        gameState.UpdatePlayerData(clientId, playerData);
        Debug.Log($"Server - Match Preparation - Player {clientId} is {(isReady ? "ready" : "not ready")}");

        if(gameState.Participants.Count >= 2)
        {
            if(gameState.AllPlayersAreReady())
            {
                Debug.Log("Server - Match Preparation - Starting Game (all players are ready)");
                gameState.matchState = MatchState.Combat;
            }
        }
        return gameState;
    }
}

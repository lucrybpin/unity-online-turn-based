
using System.Collections.Generic;

[System.Serializable]
public struct GameState
{
    public MatchState matchState;
    public List<ParticipantData> Participants;
    public List<NetworkVector3> StartingPositions;
    public float TimeLeft;
    public int CurrentTurnIndex;
    public GridSystem GridSystem;

    public ParticipantData GetParticipantData(ulong userId)
    {
        return Participants.Find(x => x.ParticipantId == userId);
    }

    public void UpdatePlayerData(ulong userId, ParticipantData updatedPlayerData)
    {
        int playerIndex = Participants.FindIndex(x => x.ParticipantId == userId);
        Participants[playerIndex] = updatedPlayerData;
    }

    public bool AllPlayersAreReady()
    {
        return !Participants.Exists(x => x.IsReady == false);
    }
    
    public void SortParticipantsByInitiative()
    {
        Participants.Sort((a,b) => b.Initiative.CompareTo(a.Initiative));
    }
}

[System.Serializable]
public struct ParticipantData
{
    public ulong ParticipantId; // 0 - 499 = Players, 500+ = NPCs
    public int Initiative;
    public int CurrentLife;
    public NetworkVector3 Position;
    public bool IsConnected;
    public bool IsReady;
    public int ParticipantTurn;
}

public enum MatchState
{
    None,
    Preparing,
    Combat
}
using TMPro;
using UnityEngine;

[System.Serializable]
public class ClientUIController
{
    [SerializeField] private Canvas _canvasStartingServer;
    [SerializeField] private Canvas _canvasMatchPreparing;
    [SerializeField] private Canvas _canvasMatchCombat;

    [SerializeField] private TMP_Text _timeTextMatchPrepare;
    [SerializeField] private TMP_Text _timeTextMatchCombat;

    public void OnUpdate(float timeLeft)
    {
        _timeTextMatchPrepare.text = timeLeft.ToString("N0");
        _timeTextMatchCombat.text = timeLeft.ToString("N0");
    }

    public void EnableStartingCanvas()
    {
        _canvasStartingServer.gameObject.SetActive(true);
    }

    public void DisableStartingCanvas()
    {
        _canvasStartingServer.gameObject.SetActive(false);
    }

    public void EnableMatchPreparingCanvas()
    {
        _canvasMatchPreparing.gameObject.SetActive(true);
    }

    public void DisableMatchPreparingCanvas()
    {
        _canvasMatchPreparing.gameObject.SetActive(false);
    }

    public void EnableMatchCombatCanvas()
    {
        _canvasMatchCombat.gameObject.SetActive(true);
    }

    public void DisableMatchCombatCanvas()
    {
        _canvasMatchCombat.gameObject.SetActive(false);
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIControllerCombatState : MonoBehaviour
{
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private Button _passTurnButton;

    public Button PassTurnButton { get => _passTurnButton; }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public void SetTimeLeft(float timeLeft)
    {
        _timerText.text = timeLeft.ToString("N0");
    }
}

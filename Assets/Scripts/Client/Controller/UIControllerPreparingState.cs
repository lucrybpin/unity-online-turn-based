using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIControllerPreparingState : MonoBehaviour
{
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private Button _IsReadyButton;

    public Button IsReadyButton { get => _IsReadyButton; }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public void SetTimeLeft(float timeLeft)
    {
        _timerText.text = timeLeft.ToString("N0");
    }
}

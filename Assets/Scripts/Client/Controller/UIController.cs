using UnityEngine;

public enum UIControllerState
{
    None,
    JoinServer,
    PreparingState,
    CombatState
}

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject _uiJoinServer;
    [SerializeField] private UIControllerPreparingState _uiControllerPreparingState;
    [SerializeField] private UIControllerCombatState _uiControllerCombatState;

    public UIControllerPreparingState UiControllerPreparingState { get => _uiControllerPreparingState; }
    public UIControllerCombatState UiControllerCombatState { get => _uiControllerCombatState; }

    public void SetMode(UIControllerState state)
    {
        switch (state)
        {
            case UIControllerState.JoinServer:
                _uiJoinServer.SetActive(true);
                _uiControllerPreparingState.SetActive(false);
                _uiControllerCombatState.SetActive(false);
            break;
            
            case UIControllerState.PreparingState:
                _uiJoinServer.SetActive(false);
                _uiControllerPreparingState.SetActive(true);
                _uiControllerCombatState.SetActive(false);
            break;
            
            case UIControllerState.CombatState:
                _uiJoinServer.SetActive(false);
                _uiControllerPreparingState.SetActive(false);
                _uiControllerCombatState.SetActive(true);
            break;
        }
    }

    public void SetTimeLeft(float timeLeft)
    {
        _uiControllerPreparingState.SetTimeLeft(timeLeft);
        _uiControllerCombatState.SetTimeLeft(timeLeft);
    }
}

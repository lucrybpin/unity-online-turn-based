using DG.Tweening;
using UnityEngine;

public enum CameraControllerMode
{
    None,
    TopDown,
    Combat 
}

public class CameraController : MonoBehaviour
{
    [SerializeField] private CameraControllerMode _currentMode;

    [SerializeField] private Camera _lobbyCamera;
    [SerializeField] private Camera _preparationCamera;
    [SerializeField] private Camera _combatCamera;
    [SerializeField] private FlatCamera _flatCamera;

    [SerializeField] private bool _debugView;

    private Camera _currentCamera;
    
    private PreparationCameraController _preparationCameraController;
    private CombatCameraController _combatCameraController;

    [SerializeField] private ServerController _serverController;

    public Camera CurrentCamera { get => _currentCamera; }

    private void Awake()
    {
        _currentCamera = _lobbyCamera;
    }

    private void Start()
    {
        _preparationCameraController = new PreparationCameraController(_preparationCamera, _flatCamera);
        _combatCameraController = new CombatCameraController(_combatCamera);
    }
    
    public void SetMode(CameraControllerMode mode)
    {
        Debug.Log($"Camera Controller - Setting Mode: {mode}");
        switch (mode)
        {
            case CameraControllerMode.None:
                _currentCamera = _lobbyCamera;
            break;

            case CameraControllerMode.TopDown:
                _lobbyCamera.gameObject.SetActive(false);
                _preparationCameraController.Enable();
                _combatCameraController.Disable();
                _currentCamera = _preparationCamera;
            break;

            case CameraControllerMode.Combat:
                _lobbyCamera.gameObject.SetActive(false);
                _preparationCameraController.Disable();
                //_combatCameraController.Enable();
                _currentCamera = _combatCamera;
            break;
        }
    }

    public void SetFocus(Vector3 focusPoint)
    {
        _preparationCameraController.SetFocus(focusPoint);
    }

    private void OnDrawGizmos()
    {
        if(!_debugView)
            return;

        Gizmos.color = Color.green;

        Grid grid = _serverController.GameState.grid;
        for (int x = 0; x < grid.GetXLength() ; x++)
        {
            for (int z = 0; z < grid.GetZLength(); z++)
            {
                Gizmos.DrawLine(new Vector3(x-0.5f, 0f, z-0.5f), new Vector3(x+0.5f, 0f, z-0.5f));
                Gizmos.DrawLine(new Vector3(x-0.5f, 0f, z-0.5f), new Vector3(x-0.5f, 0f, z+0.5f));
                Gizmos.DrawLine(new Vector3(x-0.5f, 0f, z+0.5f), new Vector3(x+0.5f, 0f, z+0.5f));
                Gizmos.DrawLine(new Vector3(x+0.5f, 0f, z-0.5f), new Vector3(x+0.5f, 0f, z+0.5f));
            }
        }
    }
}

[System.Serializable]
public class PreparationCameraController
{
    private Camera _camera;
    private FlatCamera _flatCamera;
    
    public PreparationCameraController(Camera topCamera, FlatCamera flatCamera)
    {
        _camera = topCamera;
        _flatCamera = flatCamera;
    }

    public void SetFocus(Vector3 focusPoint)
    {
        _flatCamera.SetFocus(focusPoint);
    }
    
    public void Enable()
    {
        _camera.gameObject.SetActive(true);
    }

    public void Disable()
    {
        _camera.gameObject.SetActive(false);
    }
}

[System.Serializable]
public class CombatCameraController
{
    [SerializeField] private Transform _target;
    private float _currentRotation = 0f;
    private float _rotationTime = 0.25f;
    private Camera _camera;
    private Transform _transform;

    public CombatCameraController(Camera combatCamera)
    {
        _camera = combatCamera;
        _transform = _camera.transform;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        UpdatePosition();
    }

    public void UpdatePosition()
    {
        TweenToTarget(_target);
    }

    private void TweenToTarget(Transform target)
    {
        _transform.DOMove(target.position, 1f).SetEase(Ease.Linear);
    }

    public void RotateLeft()
    {
        _currentRotation -= 90f;
        _transform.DORotate(new Vector3(0f, _currentRotation, 0f), _rotationTime).SetEase(Ease.InOutQuad);
    }

    public void RotateRight()
    {
        _currentRotation += 90f;
        _transform.DORotate(new Vector3(0f, _currentRotation, 0f), _rotationTime).SetEase(Ease.InOutQuad);
    }

    public void Enable()
    {
        _transform.gameObject.SetActive(true);
    }

    public void Disable()
    {
        _transform.gameObject.SetActive(false);
    }

}
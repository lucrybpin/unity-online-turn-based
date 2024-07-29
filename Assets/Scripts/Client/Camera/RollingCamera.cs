using DG.Tweening;
using UnityEngine;

[System.Serializable]
struct RollingCameraStep
{
    public Vector3 StartPosition;
    public Vector3 EndPosition;
    public Vector3 Rotation;
    public float Time;
}

public class RollingCamera : MonoBehaviour
{
    [SerializeField] private RollingCameraStep[] _rollingCameraStep;
    
    [SerializeField] private int _currentIndex;
    
    private void Start()
    {
        ExecuteStep(_currentIndex);
    }
    
    private void ExecuteStep(int index)
    {
        RollingCameraStep cameraStep = _rollingCameraStep[index];
        transform.position = cameraStep.StartPosition;
        transform.rotation = Quaternion.Euler(cameraStep.Rotation);
        transform.DOMove(cameraStep.EndPosition, cameraStep.Time).SetEase(Ease.Linear).OnComplete(() => {
            _currentIndex = (_currentIndex + 1) % _rollingCameraStep.Length;
            ExecuteStep(_currentIndex);
        });
    }
}


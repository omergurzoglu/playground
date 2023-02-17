
using Cinemachine;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField][Range(0f,10f)] private float defaultDistance=6f;
    [SerializeField][Range(0f,10f)] private float defaultMinDistance=1f;
    [SerializeField][Range(0f,10f)] private float defaultMaxDistance=6f;
    
    [SerializeField][Range(0f,10f)] private float smoothing=1f;
    [SerializeField][Range(0f,10f)] private float zoomSensitivity=6f;
    private float _currentTargetDistance;

    private CinemachineFramingTransposer _cineMachine;
    private CinemachineInputProvider _inputProvider;

    private void Awake()
    {
        _currentTargetDistance = defaultDistance;
        _cineMachine = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();    
        _inputProvider = GetComponent<CinemachineInputProvider>();
    }

    private void Update()
    {
        Zoom();
    }

    private void Zoom()
    {
        var zoomValue = _inputProvider.GetAxisValue(2) * zoomSensitivity;
        _currentTargetDistance =
            Mathf.Clamp(_currentTargetDistance + zoomValue, defaultMinDistance, defaultMaxDistance);
        var currentDistance = _cineMachine.m_CameraDistance;
        if (currentDistance == _currentTargetDistance)
        {
            return;
        }

        var lerpValue = Mathf.Lerp(currentDistance, _currentTargetDistance, smoothing * Time.deltaTime);
        _cineMachine.m_CameraDistance = lerpValue;
    }
}

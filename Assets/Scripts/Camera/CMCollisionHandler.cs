using Cinemachine;
using System;
using UnityEngine;

public class CMCollisionHandler : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _cinemachineCamera;
    [SerializeField] private Transform _cameraFollowTarget;
    [SerializeField] private LayerMask[] _obstacleMasks;
    [SerializeField] private float _minDistance = 2f;
    [SerializeField] private float _maxDistance = 6f;
    [SerializeField] private float _collisionMoveSpeed = 5f;

    private Cinemachine3rdPersonFollow _followComponent;
    private PlayerInputHandler _playerInputHandler;
    private Inventory _inventory;

    // Cinemachine
    [Header("Cinemachine")]
    public GameObject _cinemachineCameraTarget;

    //Cinemachine Rotation and Zoom valuse
    public float _cameraTopClamp = 70.0f;
    public float _cameraBottomClamp = -15.0f;
    public float _cameraAngleOverride = 0.0f;
    public float _deltaTimeMultiplier = 0.2f;
    public bool _lockCameraPosition = false;
    public float _zoomByDefault = 2f;
    public float _zoomModifier = 10f;
    public float _zoomLerpSpeed = 60f;
    public float _maxZoom = 6f;
    public float _minZoom = 2f;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private const float _threshold = 0.01f;

    //Cinemachine Collision Handling
    private bool _isColliding;

    private void Start()
    {
        _cameraFollowTarget = GameObject.Find("CinemachineFollowTarget").transform;
        _cinemachineCameraTarget = GameObject.Find("CinemachineFollowTarget");
        _followComponent = _cinemachineCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();

        _inventory = GameObject.Find("GUI").GetComponent<Inventory>();
        _cinemachineTargetYaw = _cinemachineCameraTarget.transform.rotation.eulerAngles.y;
        _cinemachineCamera = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        _cinemachineCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance = 5f;
        _playerInputHandler = GameObject.Find("Player(Clone)").GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        if (!Gamebahaivour._isPaused)
        {
            if (!_inventory._isInventoryOpen)
            {
                CameraMovement();
                CameraRotation();
                HandleCameraCollision();
                if(!_isColliding) CameraZooming();
            }
        }
    }

    private void CameraMovement()
    {
        _cinemachineCamera.m_Follow = _cameraFollowTarget;
    }

    private void HandleCameraCollision()
    {
        Ray ray = new Ray(transform.position, _cameraFollowTarget.transform.position);
        foreach (LayerMask obstacleMask in _obstacleMasks)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, _maxDistance, obstacleMask))
            {
                float distanceToObstacle = hit.distance - _minDistance;
                float desiredDistance = Mathf.Clamp(distanceToObstacle, 0f, _maxDistance);
                float currentCmDistance = Mathf.Lerp(_followComponent.CameraDistance, desiredDistance, Time.deltaTime * _collisionMoveSpeed);
                _followComponent.CameraDistance = currentCmDistance;
                _isColliding = true;
                return;
            }
        }
        _isColliding = false;
    }

    private void CameraZooming()
    {
        if (!_lockCameraPosition && _cinemachineCamera != null)
        {
            float zoomInput = _playerInputHandler._ZoomInput;

            if (zoomInput != 0f)
            {
                float zoomDelta = -zoomInput * _zoomModifier * Time.deltaTime;
                Cinemachine3rdPersonFollow followComponent = _cinemachineCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
                float targetZoom = Mathf.Clamp(followComponent.CameraDistance + zoomDelta, _minZoom, _maxZoom);
                followComponent.CameraDistance = Mathf.Lerp(followComponent.CameraDistance, targetZoom, _zoomLerpSpeed * Time.deltaTime);
            }
        }
    }

    private void CameraRotation()
    {
        if (_playerInputHandler._LookInput.sqrMagnitude >= _threshold && !_lockCameraPosition)
        {
            _cinemachineTargetYaw += _playerInputHandler._LookInput.x * _deltaTimeMultiplier;
            _cinemachineTargetPitch += _playerInputHandler._LookInput.y * _deltaTimeMultiplier;
        }

        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _cameraBottomClamp, _cameraTopClamp);

        _cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + _cameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}

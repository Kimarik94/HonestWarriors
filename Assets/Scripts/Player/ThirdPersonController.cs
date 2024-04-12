using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class ThirdPersonController : MonoBehaviour
{
    // Player Components
    private GameObject _mainCamera;
    private CharacterController _playerController;
    private PlayerInputHandler _playerInputHandler;
    private PlayerCharacteristics _playerCharacteristics;
    private Animator _playerAnimator;
    private InventoryUI _inventoryUI;

    // Player
    private float _playerSpeed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    public float _moveSpeed = 2.0f;
    public float _sprintSpeed = 5.335f;

    [Range(0.0f, 0.3f)] 
    public float _rotationSmoothTime = 0.12f;
    public float _speedChangeRate = 10.0f;

    public bool _isAttacking = false;

    // Audio
    [Header("Player Audio")]
    public AudioClip[] _footstepAudioClips;
    [Range(0, 1)] public float _footstepAudioVolume = 0.5f;

    // Animation
    private int _animSpeed;
    private int _animMotionSpeed;

    //Interaction with objects
    [Header("Interaction with Objects")]
    public float _interactionRadius = 2f;
    public bool _isInteraction = false;
    public LayerMask _interactableLayer;
    public Collider[] _interactableObjectsColliders = new Collider[3];
    public int _collidersNumFound;
    public GameObject _currentInteractableObject;
    public Interactable _focus;

    private void Awake()
    {
        if (_mainCamera == null) _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        _inventoryUI = GameObject.Find("GUI").GetComponent<InventoryUI>();
        _playerAnimator = GetComponent<Animator>();
        _playerController = GetComponent<CharacterController>();
        _playerCharacteristics = GetComponent<PlayerCharacteristics>();
        _playerInputHandler = GetComponent<PlayerInputHandler>();
    }

    private void Start()
    {
        AssignAnimationIDs();
    }

    private void Update()
    {
        if (!Gamebahaivour._isPaused)
        {
            if (!_isInteraction && !_playerCharacteristics._isDie && !_inventoryUI._isInventoryOpen)
            {
                AdjustHeight();
                Block();
                if (_playerInputHandler._isBlocking) _isAttacking = false;

                if (!_playerInputHandler._isBlocking)
                {
                    Attack();
                    Move();
                    InteractWith();
                }
            }
            if (_inventoryUI._isInventoryOpen)
            {
                _playerAnimator.SetFloat("Speed", 0);
            }
        }
    }

    private void AdjustHeight()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, LayerMask.GetMask("Ground")))
        {
            if (hit.distance > 0)
            {
                _playerController.Move(new Vector3(_playerController.velocity.x * 0.925f, -5f, _playerController.velocity.z * 0.925f) * Time.deltaTime);
            }
        }
    }

    private void AssignAnimationIDs()
    {
        _animSpeed = Animator.StringToHash("Speed");
        _animMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void Move()
    {
        RemoveFocus();
        float targetSpeed = _playerInputHandler._SprintPressed ? _sprintSpeed : _moveSpeed;

        if (_playerInputHandler._MoveInput == Vector2.zero) targetSpeed = 0.0f;

        float currentHorizontalSpeed = new Vector3(_playerController.velocity.x, 0.0f, _playerController.velocity.z).magnitude;
        float speedOffset = 0.1f;
        float inputMagnitude = 1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _playerSpeed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * _speedChangeRate);
            _playerSpeed = Mathf.Round(_playerSpeed * 1000f) / 1000f;
        }
        else _playerSpeed = targetSpeed;


        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * _speedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        Vector3 inputDirection = new Vector3(_playerInputHandler._MoveInput.x, 0.0f, _playerInputHandler._MoveInput.y).normalized;

        if (_playerInputHandler._MoveInput != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, _rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        _playerController.Move(targetDirection.normalized * (_playerSpeed * Time.deltaTime) +
                         new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        _playerAnimator.SetFloat(_animSpeed, _animationBlend);
        _playerAnimator.SetFloat(_animMotionSpeed, inputMagnitude);
    }

    private void Attack()
    {
        _playerAnimator.SetBool("Attack", _playerInputHandler._AttackPressed);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    private void Block()
    {
        _playerAnimator.SetBool("Block", _playerInputHandler._BlockPressed);
    }

    private void InteractWith()
    {
        _collidersNumFound = Physics.OverlapSphereNonAlloc(transform.position, _interactionRadius, _interactableObjectsColliders, _interactableLayer);

        if (_collidersNumFound > 0)
        {
            _currentInteractableObject = _interactableObjectsColliders[0].gameObject;
            Interactable interactableObject = _currentInteractableObject.GetComponent<Interactable>();
            if (interactableObject != null)
            {
                SetFocus(interactableObject);
                if (_focus != null)
                {
                    _focus.OnFocused(transform);
                }
            }
        }
        else
        {
            RemoveFocus();
        }
    }

    private void SetFocus(Interactable newFocus)
    {
        _focus = newFocus;
    }

    private void RemoveFocus()
    {
        if (_focus != null)
        {
            _focus.OnDefocused();
            _focus = null;
            _currentInteractableObject = null;
            for (int i = 0; i < _interactableObjectsColliders.Length; i++)
            {
                _interactableObjectsColliders[i] = null;
            }
        }
    }

    private void OnFootStep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (_footstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, _footstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(_footstepAudioClips[index], transform.TransformPoint(_playerController.center), _footstepAudioVolume);
            }
        }
    }

    private void OnAttackStart(AnimationEvent animationEvent)
    {
        _isAttacking = true;
    }

    private void OnAttackEnd(AnimationEvent animationEvent)
    {
        _isAttacking = false;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _interactionRadius);
    }
}

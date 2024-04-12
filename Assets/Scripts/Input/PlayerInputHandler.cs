using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset _playerControls;

    [Header("Action Map Name Reference")]
    [SerializeField] private string _actionMapName = "Player";

    [Header("Action Name Reference")]
    [SerializeField] private string _move = "Move";
    [SerializeField] private string _sprint = "Sprint";
    [SerializeField] private string _block = "Block";
    [SerializeField] private string _look = "Look";
    [SerializeField] private string _zoom = "Zoom";
    [SerializeField] private string _attack = "Attack";
    [SerializeField] private string _interact = "Interact";
    [SerializeField] private string _weaponEquip = "EquipUnequipAxe";

    //Input Actions
    private InputAction _moveAction;
    private InputAction _sprintAction;
    private InputAction _blockAction;
    private InputAction _lookAction;
    private InputAction _zoomAction;
    private InputAction _attackAction;
    private InputAction _interactAction;
    private InputAction _weaponEquipAction;

    //Additional flags
    public bool _isBlocking = false;

    //Inputs init
    public Vector2 _MoveInput { get; private set; }
    public bool _SprintPressed { get; private set; }
    public bool _BlockPressed { get; private set; }
    public Vector2 _LookInput { get; private set; }
    public float _ZoomInput { get; private set; }
    public bool _AttackPressed { get; private set; }
    public bool _InteractPressed { get; private set; }
    public bool _WeaponEquipPressed { get; private set; }

    public static PlayerInputHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Instance = null;
        }

        _moveAction = _playerControls.FindActionMap(_actionMapName).FindAction(_move);
        _sprintAction = _playerControls.FindActionMap(_actionMapName).FindAction(_sprint);
        _blockAction = _playerControls.FindActionMap(_actionMapName).FindAction(_block);
        _lookAction = _playerControls.FindActionMap(_actionMapName).FindAction(_look);
        _zoomAction = _playerControls.FindActionMap(_actionMapName).FindAction(_zoom);
        _attackAction = _playerControls.FindActionMap(_actionMapName).FindAction(_attack);
        _interactAction = _playerControls.FindActionMap(_actionMapName).FindAction(_interact);
        _weaponEquipAction = _playerControls.FindActionMap(_actionMapName).FindAction(_weaponEquip);

        RegisterInputAction();
    }

    void RegisterInputAction()
    {
        _moveAction.performed += context => _MoveInput = context.ReadValue<Vector2>();
        _moveAction.canceled += context => _MoveInput = Vector2.zero;

        _sprintAction.performed += context => _SprintPressed = true;
        _sprintAction.canceled += context => _SprintPressed = false;

        _blockAction.performed += context =>
        {
            _BlockPressed = true;
            _isBlocking = true;
        };
        _blockAction.canceled += context =>
        {
            _BlockPressed = false;
            _isBlocking = false;
        };

        _lookAction.performed += context => _LookInput = context.ReadValue<Vector2>();
        _lookAction.canceled += context => _LookInput = Vector2.zero;


        _zoomAction.performed += context => _ZoomInput = context.ReadValue<float>();
        _zoomAction.canceled += context => _ZoomInput = 0f;

        _attackAction.performed += context => _AttackPressed = true;
        _attackAction.canceled += context => _AttackPressed = false;

        _interactAction.performed += context => _InteractPressed = true;
        _interactAction.canceled += context => _InteractPressed = false;

        _weaponEquipAction.performed += context => _WeaponEquipPressed = true;
        _weaponEquipAction.canceled += context => _WeaponEquipPressed = false;
    }

    private void OnEnable()
    {
        _moveAction.Enable();
        _sprintAction.Enable();
        _blockAction.Enable();
        _lookAction.Enable();
        _zoomAction.Enable();
        _attackAction.Enable();
        _interactAction.Enable();
        _weaponEquipAction.Enable();
    }

    private void OnDisable()
    {
        _moveAction.Disable();
        _sprintAction.Disable();
        _blockAction.Disable();
        _lookAction.Disable();
        _zoomAction.Disable();
        _attackAction.Disable();
        _interactAction.Disable();
        _weaponEquipAction.Disable();
    }
}

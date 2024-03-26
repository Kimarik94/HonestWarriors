using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name Reference")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Action Name Reference")]
    [SerializeField] private string move = "Move";
    [SerializeField] private string look = "Look";
    [SerializeField] private string sprint = "Sprint";
    [SerializeField] private string zoom = "Zoom";
    [SerializeField] private string attack = "Attack";
    [SerializeField] private string interact = "Interact";

    //Movements and Camera controll
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction sprintAction;
    private InputAction zoomAction;
    private InputAction attackAction;
    private InputAction interactAction;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool SprintPressed { get; private set; }
    public float ZoomInput { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool InteractPressed { get; private set; }

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

        moveAction = playerControls.FindActionMap(actionMapName).FindAction(move);
        lookAction = playerControls.FindActionMap(actionMapName).FindAction(look);
        sprintAction = playerControls.FindActionMap(actionMapName).FindAction(sprint);
        zoomAction = playerControls.FindActionMap(actionMapName).FindAction(zoom);
        attackAction = playerControls.FindActionMap(actionMapName).FindAction(attack);
        interactAction = playerControls.FindActionMap(actionMapName).FindAction(interact);

        RegisterInputAction();
    }

    void RegisterInputAction()
    {
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;

        lookAction.performed += context => LookInput = context.ReadValue<Vector2>();
        lookAction.canceled += context => LookInput = Vector2.zero;

        sprintAction.performed += context => SprintPressed = true;
        sprintAction.canceled += context => SprintPressed = false;

        zoomAction.performed += context => ZoomInput = context.ReadValue<float>();
        zoomAction.canceled += context => ZoomInput = 0f;

        attackAction.performed += context => AttackPressed = true;
        attackAction.canceled += context => AttackPressed = false;

        interactAction.performed += context => InteractPressed = true;
        interactAction.canceled += context => InteractPressed = false;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        sprintAction.Enable();
        zoomAction.Enable();
        attackAction.Enable();
        interactAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        sprintAction.Disable();
        zoomAction.Disable();
        attackAction.Disable();
        interactAction.Disable();
    }
}

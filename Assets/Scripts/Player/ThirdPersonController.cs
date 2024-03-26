using Cinemachine;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    // Player Components
    private GameObject mainCamera;
    private CharacterController playerController;
    private CinemachineVirtualCamera cinemachineCamera;
    private PlayerInputHandler playerInputHandler;
    private PlayerCharacteristics playerCharacteristics;
    private Animator playerAnimator;
    private InventoryUI inventoryUI;

    // Player
    private float playerSpeed;
    private float animationBlend;
    private float targetRotation = 0.0f;
    private float rotationVelocity;
    private float verticalVelocity;
    public float MoveSpeed = 2.0f;
    public float SprintSpeed = 5.335f;

    [Range(0.0f, 0.3f)] 
    public float RotationSmoothTime = 0.12f;
    public float SpeedChangeRate = 10.0f;

    public bool isAttacking = false;

    // Audio
    [Header("Player Audio")]
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    // Cinemachine
    [Header("Cinemachine")]
    public GameObject CinemachineCameraTarget;

    //Cinemachine Rotation and Zoom valuse
    public float CameraTopClamp = 70.0f;
    public float CameraBottomClamp = -15.0f;
    public float CameraAngleOverride = 0.0f;
    public float deltaTimeMultiplier = 0.2f;
    public bool LockCameraPosition = false;
    public float ZoomByDefault = 2f;
    public float ZoomModifier = 10f;
    public float ZoomLerpSpeed = 70f;
    public float MaxZoom = 8f;
    public float MinZoom = 2f;
    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;

    // Animation
    private int animSpeed;
    private int animMotionSpeed;
    private const float threshold = 0.01f;

    //Interaction with objects
    [Header("Interaction with Objects")]
    public float interactionRadius = 2f;
    public bool isInteraction = false;
    public LayerMask interactableLayer;
    public Collider[] interactableObjectsColliders = new Collider[3];
    public int collidersNumFound;
    public GameObject currentInteractableObject;
    public Interactable focus;

    private void Awake()
    {
        if (mainCamera == null) mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        inventoryUI = GameObject.Find("GUI").GetComponent<InventoryUI>();
        cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        cinemachineCamera = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        cinemachineCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance = 5f;
        playerAnimator = GetComponent<Animator>();
        playerController = GetComponent<CharacterController>();
        playerCharacteristics = GetComponent<PlayerCharacteristics>();
        playerInputHandler = GetComponent<PlayerInputHandler>();
    }

    private void Start()
    {
        AssignAnimationIDs();
    }

    private void Update()
    {
        if (!Gamebahaivour.isPaused)
        {
            if (!isInteraction && !playerCharacteristics.isDie && !inventoryUI.isInventoryOpen)
            {
                Attack();
                Move();
                InteractWith();
            }
        }
    }

    private void LateUpdate()
    {
        if (!Gamebahaivour.isPaused)
        {
            if (!inventoryUI.isInventoryOpen && !isInteraction)
            {
                CameraRotation();
                CameraZooming();
            }
        }
    }

    private void AssignAnimationIDs()
    {
        animSpeed = Animator.StringToHash("Speed");
        animMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void CameraRotation()
    {
        if (playerInputHandler.LookInput.sqrMagnitude >= threshold && !LockCameraPosition)
        {
            cinemachineTargetYaw += playerInputHandler.LookInput.x * deltaTimeMultiplier;
            cinemachineTargetPitch += playerInputHandler.LookInput.y * deltaTimeMultiplier;
        }

        cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, CameraBottomClamp, CameraTopClamp);

        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch + CameraAngleOverride,
            cinemachineTargetYaw, 0.0f);
    }

    private void CameraZooming()
    {
        if (!LockCameraPosition && cinemachineCamera != null)
        {
            float zoomInput = playerInputHandler.ZoomInput;

            if (zoomInput != 0f)
            {
                float zoomDelta = -zoomInput * ZoomModifier * Time.deltaTime;
                Cinemachine3rdPersonFollow followComponent = cinemachineCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
                float targetZoom = Mathf.Clamp(followComponent.CameraDistance + zoomDelta, MinZoom, MaxZoom);
                followComponent.CameraDistance = Mathf.Lerp(followComponent.CameraDistance, targetZoom, ZoomLerpSpeed * Time.deltaTime);
            }
        }
    }

    private void Move()
    {
        RemoveFocus();
        float targetSpeed = playerInputHandler.SprintPressed ? SprintSpeed : MoveSpeed;

        if (playerInputHandler.MoveInput == Vector2.zero) targetSpeed = 0.0f;

        float currentHorizontalSpeed = new Vector3(playerController.velocity.x, 0.0f, playerController.velocity.z).magnitude;
        float speedOffset = 0.1f;
        float inputMagnitude = 1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            playerSpeed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
            playerSpeed = Mathf.Round(playerSpeed * 1000f) / 1000f;
        }
        else playerSpeed = targetSpeed;


        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (animationBlend < 0.01f) animationBlend = 0f;

        Vector3 inputDirection = new Vector3(playerInputHandler.MoveInput.x, 0.0f, playerInputHandler.MoveInput.y).normalized;

        if (playerInputHandler.MoveInput != Vector2.zero)
        {
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, RotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

        playerController.Move(targetDirection.normalized * (playerSpeed * Time.deltaTime) +
                         new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);

        playerAnimator.SetFloat(animSpeed, animationBlend);
        playerAnimator.SetFloat(animMotionSpeed, inputMagnitude);
    }

    private void Attack()
    {
        playerAnimator.SetBool("Attack", playerInputHandler.AttackPressed);
    }

    private void InteractWith()
    {
        collidersNumFound = Physics.OverlapSphereNonAlloc(transform.position, interactionRadius, interactableObjectsColliders, interactableLayer);

        if (collidersNumFound > 0)
        {
            currentInteractableObject = interactableObjectsColliders[0].gameObject;
            Interactable interactableObject = currentInteractableObject.GetComponent<Interactable>();
            if (interactableObject != null)
            {
                SetFocus(interactableObject);
                if (focus != null)
                {
                    focus.OnFocused(transform);
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
        focus = newFocus;
    }

    private void RemoveFocus()
    {
        if (focus != null)
        {
            focus.OnDefocused();
            focus = null;
            currentInteractableObject = null;
            for (int i = 0; i < interactableObjectsColliders.Length; i++)
            {
                interactableObjectsColliders[i] = null;
            }
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnFootStep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(playerController.center), FootstepAudioVolume);
            }
        }
    }

    private void OnAttackStart(AnimationEvent animationEvent)
    {
        isAttacking = true;
    }

    private void OnAttackEnd(AnimationEvent animationEvent)
    {
        isAttacking = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}

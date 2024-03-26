using UnityEngine;
using UnityEngine.AI;

public class WolfMovements : MonoBehaviour
{
    private WolfCharacteristics wolfCharacteristics;
    private NavMeshAgent wolfAgent;
    private Animator wolfAnimator;
    public AudioClip wolfHowl;
    public AudioClip wolfAttack;

    private GameObject playerObject;
    private PlayerCharacteristics playerCharacteristics;

    public float walkSpeed = 2.5f;
    public float runSpeed = 5f;
    public bool isMoving = false;
    public float changeSpeedDistance = 2.0f;
    private float distanceToPlayer;

    Vector3 walkPoint;
    private bool walkPointSet = false;
    public float walkPointRange = 15f;

    [Header("Howl Logic")]
    private float lastHowlTime;
    private float howlTimer = 30f;
    public bool isHowling = false;


    [Header("Enemy Detection / Attack Player")]
    public float sightRange = 10f;
    public float attackRange = 1.0f;
    private float attackSoundDelay = 1.8f;
    private float lastAttackSound = 0f;

    public bool playerInSightRange;
    public bool playerInAttackRange;
    public bool playerDead;

    [Header("Player Layer")]
    public LayerMask playerMask;

    private void Awake()
    {
        wolfAgent = GetComponent<NavMeshAgent>();
        wolfCharacteristics = GetComponent<WolfCharacteristics>();
        wolfAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        playerObject = GameObject.Find("Player");
        playerCharacteristics = playerObject.GetComponent<PlayerCharacteristics>();
        playerCharacteristics.onPlayerDead += Howl;

        lastHowlTime = Time.time;
    }

    private void FixedUpdate()
    {
        isMoving = wolfAgent.velocity.magnitude > 2 ? true : false; 

        distanceToPlayer = (transform.position - playerObject.transform.position).magnitude;
        wolfAnimator.SetFloat("Speed", wolfAgent.velocity.magnitude);
        wolfAnimator.SetBool("Howl", isHowling);
        wolfAnimator.SetBool("isMoving", isMoving);

        if (!wolfCharacteristics.isDie && !playerCharacteristics.isDie)
        {
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerMask);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);
        }
        
        if (wolfCharacteristics.isDie || isHowling) wolfAgent.destination = transform.position;

        if (Time.time - lastHowlTime >= howlTimer + 10f) lastHowlTime = Time.time;

        if(!wolfCharacteristics.isDie) WolfDecision();
    }

    private void WolfDecision()
    {
        if (playerCharacteristics.isDie)
        {
            playerDead = playerCharacteristics.isDie;
            playerInSightRange = false;
            playerInAttackRange = false;
            wolfAnimator.SetBool("playerDead", playerCharacteristics.isDie);
        }
        if (!playerInSightRange && !playerInAttackRange && Time.time - lastHowlTime >= howlTimer) Howl();
        if (!playerInSightRange && !playerInAttackRange && !isHowling) Patrol();
        if (playerInSightRange && !playerInAttackRange) Chase();
        if (playerInSightRange && playerInAttackRange) Attack();
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (NavMesh.SamplePosition(walkPoint, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            walkPointSet = true;
        }
    }

    private void Patrol()
    {
        wolfAgent.speed = walkSpeed;
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet) wolfAgent.destination = walkPoint;

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 2f) walkPointSet = false;
        if (walkPointSet && wolfAgent.velocity == Vector3.zero) walkPointSet = false;
    }

    private void Chase()
    {
        wolfAgent.speed = (distanceToPlayer > changeSpeedDistance) ? runSpeed : walkSpeed;
        wolfAgent.destination = playerObject.transform.position;
        if (wolfAgent.velocity == Vector3.zero) wolfAgent.transform.LookAt(playerObject.transform.position);
    }

    private void Attack()
    {
        if (distanceToPlayer <= attackRange)
        {
            wolfAgent.destination = transform.position;
            wolfAgent.transform.LookAt(playerObject.transform.position);
            wolfAnimator.SetTrigger("Attack");
        }
        else Chase();
    }

    private void Howl()
    {
        isHowling = true;
        lastHowlTime = Time.time;
    }

    private void OnHowl(AnimationEvent animationEvent)
    {
        AudioSource.PlayClipAtPoint(wolfHowl, transform.position);
    }

    private void OnHowlEnd(AnimationEvent animationEvent)
    {
        isHowling = false;
    }

    private void OnWolfAttackSound(AnimationEvent animationEvent)
    {
        if(Time.time - lastAttackSound >= attackSoundDelay)
        {
            AudioSource.PlayClipAtPoint(wolfAttack, transform.position);
        }
    }

    private void OnAttackEnd(AnimationEvent animationEvent)
    {
       lastAttackSound = Time.time;
    }
}

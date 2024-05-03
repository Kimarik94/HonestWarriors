using UnityEngine;
using UnityEngine.AI;

public class WolfMovements : MonoBehaviour
{
    private WolfCharacteristics _wolfCharacteristics;
    private NavMeshAgent _wolfAgent;
    private Animator _wolfAnimator;
    public AudioClip _wolfHowl;
    public AudioClip _wolfAttack;
    private AudioSource _wolfAudioSource;

    private GameObject _playerObject;
    private PlayerCharacteristics _playerCharacteristics;

    public float _walkSpeed = 2.5f;
    public float _runSpeed = 5f;
    public bool _isMoving = false;
    public float _changeSpeedDistance = 2.0f;
    private float _distanceToPlayer;

    private Vector3 _walkPoint;
    private bool _walkPointSet = false;
    public float _walkPointRange = 15f;

    [Header("Howl Logic")]
    private float _lastHowlTime;
    private float _howlTimer = 30f;
    public bool _isHowling = false;


    [Header("Enemy Detection / Attack Player")]
    public float _sightRange = 10f;
    public float _attackRange = 1.1f;
    public bool _isAttacking = false;

    private bool _playerInSightRange;
    private bool _playerInAttackRange;

    [Header("Player Layer")]
    public LayerMask _playerMask;

    private void Awake()
    {
        _wolfAgent = GetComponent<NavMeshAgent>();
        _wolfCharacteristics = GetComponent<WolfCharacteristics>();
        _wolfAnimator = GetComponent<Animator>();
        _wolfAudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _playerObject = GameObject.Find("Player(Clone)");
        _playerCharacteristics = _playerObject.GetComponent<PlayerCharacteristics>();
        _playerCharacteristics._onPlayerDead += Howl;

        _lastHowlTime = Time.time;
    }

    private void FixedUpdate()
    {
        _isMoving = _wolfAgent.velocity.magnitude > 2 ? true : false; 

        _distanceToPlayer = (transform.position - _playerObject.transform.position).magnitude;
        _wolfAnimator.SetFloat("Speed", _wolfAgent.velocity.magnitude);
        _wolfAnimator.SetBool("Howl", _isHowling);
        _wolfAnimator.SetBool("isMoving", _isMoving);

        if (!_wolfCharacteristics._isDie && !_playerCharacteristics._isDie)
        {
            _playerInSightRange = Physics.CheckSphere(transform.position, _sightRange, _playerMask);
            _playerInAttackRange = Physics.CheckSphere(transform.position, _attackRange, _playerMask);
        }
        
        if (_wolfCharacteristics._isDie || _isHowling) _wolfAgent.destination = transform.position;

        if (Time.time - _lastHowlTime >= _howlTimer + 10f) _lastHowlTime = Time.time;

        if(!_wolfCharacteristics._isDie) WolfDecision();
    }

    private void WolfDecision()
    {
        if (_playerCharacteristics._isDie)
        {
            _playerInSightRange = false;
            _playerInAttackRange = false;
            _wolfAnimator.SetBool("playerDead", _playerCharacteristics._isDie);
        }
        if (!_playerInSightRange && !_playerInAttackRange && Time.time - _lastHowlTime >= _howlTimer) Howl();
        if (!_playerInSightRange && !_playerInAttackRange && !_isHowling) Patrol();
        if (_playerInSightRange && !_playerInAttackRange) Chase();
        if (_playerInSightRange && _playerInAttackRange) Attack();
    }

    private void SearchWalkPoint()
    {
        _isAttacking = false;
        float randomZ = Random.Range(-_walkPointRange, _walkPointRange);
        float randomX = Random.Range(-_walkPointRange, _walkPointRange);
        _walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (NavMesh.SamplePosition(_walkPoint, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            _walkPointSet = true;
        }
    }

    private void Patrol()
    {
        _wolfAgent.speed = _walkSpeed;
        if (!_walkPointSet) SearchWalkPoint();
        if (_walkPointSet) _wolfAgent.destination = _walkPoint;

        Vector3 distanceToWalkPoint = transform.position - _walkPoint;
        if (distanceToWalkPoint.magnitude < 2f) _walkPointSet = false;
        if (_walkPointSet && _wolfAgent.velocity == Vector3.zero) _walkPointSet = false;
    }

    private void Chase()
    {
        _isMoving = true;
        if (_wolfAudioSource.clip == _wolfHowl) _wolfAudioSource.Stop();
        if (_wolfAudioSource.clip == _wolfAttack)
        {
            float currentVolume = _wolfAudioSource.volume;
            _wolfAudioSource.volume = Mathf.Lerp(currentVolume, 0, Time.deltaTime);
        } 
        _wolfAgent.speed = (_distanceToPlayer > _changeSpeedDistance) ? _runSpeed : _walkSpeed;
        _wolfAgent.destination = _playerObject.transform.position;
        if (_wolfAgent.velocity == Vector3.zero) _wolfAgent.transform.LookAt(_playerObject.transform.position);
    }

    private void Attack()
    {
        if (_distanceToPlayer <= _attackRange)
        {
            _wolfAgent.destination = transform.position;
            _wolfAgent.transform.LookAt(_playerObject.transform.position);
            _wolfAnimator.SetTrigger("Attack");
            _isAttacking = true;
        }
        else
        {
            Chase();
            _isAttacking = false;
        }
    }

    private void Howl()
    {
        _isHowling = true;
        _lastHowlTime = Time.time;
    }

    private void OnHowl(AnimationEvent animationEvent)
    {
        _wolfAudioSource.clip = _wolfHowl;
        _wolfAudioSource.Play();
    }

    private void OnHowlEnd(AnimationEvent animationEvent)
    {
        _isHowling = false;
    }

    private void OnWolfAttackSound(AnimationEvent animationEvent)
    {
        _wolfAudioSource.clip = _wolfAttack;
        _wolfAudioSource.volume = 0.3f;
        _wolfAudioSource.Play();
    }
}

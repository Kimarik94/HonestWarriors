using System;
using UnityEngine;

public class PlayerCharacteristics : MonoBehaviour
{
    public event Action _onPlayerDead;
    public event Action<float> onHealthDecrease = delegate { };
    public event Action<float> onHealthIncrease = delegate { };

    private CharacterController _playerController;
    private PlayerInputHandler _playerInputHandler;
    private Animator _playerAnimator;
    public Transform _interactionPoint;

    public AudioClip _damageTakeSound;
    public AudioClip _blockSound;
    public AudioClip _deathSound;

    private float _currentHealth;
    private float _maxHealth = 100.0f;
    private float _damage = 15f;
    private float _passiveHealPerSecond = 0.01f;
    private float _lastDamageTime = 0f;
    private float _regenerationDelay = 5f;
    private float _damageInterval = 1.25f;

    //Enemy
    private Collider[] _enemyDamageColliders = new Collider[3];
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private float _interactionRadius = 5f;
    [SerializeField] private int _collidersNumFound;
    [SerializeField] private float detectionAngle = 90f;

    private WolfAnimEvents _wolfAnimEvents;

    //Healths stats
    private bool _canDamage;
    public bool _isDie = false;

    private void Start()
    {
        _playerController = GetComponent<CharacterController>();
        _playerAnimator = GetComponent<Animator>();
        _playerInputHandler = GetComponent<PlayerInputHandler>();
        _currentHealth = _maxHealth;
        _interactionPoint = transform;
    }
    private void Update()
    {
        if (!_isDie)
        {
            RegenerationHP();
            CalculateEnemiesDamage();
            TakeDamage();
        }
    }

    private void CalculateEnemiesDamage()
    {
        _collidersNumFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionRadius, _enemyDamageColliders, _enemyLayer);
        if (_collidersNumFound > 0)
        {
            if (!_enemyDamageColliders[0].GetComponentInParent<WolfCharacteristics>()._isDie
                && Time.time - _lastDamageTime >= _damageInterval)
            {
                _wolfAnimEvents = _enemyDamageColliders[0].GetComponentInParent<WolfAnimEvents>();
                Vector3 directionToPlayer = _enemyDamageColliders[0].GetComponentInParent<Transform>().position - transform.position;
                directionToPlayer.Normalize();

                Vector3 forward = transform.forward;

                if (!(Vector3.Dot(forward, directionToPlayer) > Mathf.Cos(detectionAngle * 0.5f * Mathf.Deg2Rad))
                    && !_playerInputHandler._isBlocking)
                {
                    _canDamage = true;
                }
                else if (!(Vector3.Dot(forward, directionToPlayer) > Mathf.Cos(detectionAngle * 0.5f * Mathf.Deg2Rad))
                    && _playerInputHandler._isBlocking)
                {
                    if (_wolfAnimEvents.attackTouch)
                    {
                        AudioSource.PlayClipAtPoint(_blockSound, transform.position);
                        _wolfAnimEvents.attackTouch = false;
                    }
                }
            }
        }
        else _canDamage = false;
    }

    public void DecreaseHP(float amount)
    {
        _currentHealth -= amount;
        _lastDamageTime = Time.time;
        float currentHealthPercent = _currentHealth / _maxHealth;
        onHealthDecrease(currentHealthPercent);
    }

    public void IncreaseHP(float passiveHealPerSecond)
    {
        _currentHealth += passiveHealPerSecond;
        onHealthIncrease(_currentHealth / _maxHealth);
        if (_currentHealth > _maxHealth) _currentHealth = _maxHealth;
    }

    private void RegenerationHP()
    {
        if ((Time.time - _lastDamageTime >= _regenerationDelay) && _currentHealth <= _maxHealth && !_canDamage)
        {
            IncreaseHP(_passiveHealPerSecond);
        }
    }

    private void TakeDamage()
    {
        if (_currentHealth <= 0)
        {
            _isDie = true;
            AudioSource.PlayClipAtPoint(_deathSound,transform.position);
            _playerAnimator.SetBool("isDie", _isDie);
            _onPlayerDead.Invoke();
            _canDamage = false;
        }
        if (_canDamage)
        {
            if(_wolfAnimEvents != null && _wolfAnimEvents.attackTouch &&
                _playerController.bounds.Intersects(GameObject.FindWithTag("EnemyDamageCollider").GetComponent<Collider>().bounds))
            {
                _wolfAnimEvents.attackTouch = false;
                AudioSource.PlayClipAtPoint(_damageTakeSound, transform.position);
                DecreaseHP(_damage);
                _canDamage = false;
                _lastDamageTime = Time.time;
            }
        }
    }
}

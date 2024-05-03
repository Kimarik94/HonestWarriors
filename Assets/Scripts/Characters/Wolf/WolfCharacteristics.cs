using System;
using UnityEngine;

public class WolfCharacteristics : MonoBehaviour
{
    private Animator _wolfAnimator;
    private Collider _wolfCollider;
    public AudioClip _wolfDie;
    public AudioClip _wolfAxeHit;
    public AudioClip _wolfKickDamage;

    private PlayerDetectionObject _detection;

    public event Action<float> onHealthDecrease = delegate { };
    public event Action<float> onHealthIncrease = delegate { };

    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _currentHealth;

    private float _lastDamageTime = 0f;

    private float _rechargeHitpointsTimer = 15f;

    public bool _isDie = false;
    private bool _isDestroyed = false;
    [SerializeField] private float _destroyTimer = 10f;

    private void Start()
    {
        _wolfAnimator = GetComponent<Animator>();
        _wolfCollider = GetComponent<Collider>();
        _detection = GetComponentInChildren<PlayerDetectionObject>();
        _currentHealth = _maxHealth;
    }
    private void Update()
    {
        if (!_isDie && Time.time - _lastDamageTime >= _rechargeHitpointsTimer)
        {
            RechargeHitPoints();
        }

        if (_isDie)
        {
            _wolfCollider.isTrigger = true;
        }

        if(_isDie && !_isDestroyed && Time.time - _lastDamageTime >= _destroyTimer)
        {
            _isDestroyed = true;
            Destroy(gameObject);
        }
    }
    public void DecreaseHP(float amount)
    {
        _currentHealth -= amount;
        _lastDamageTime = Time.time;
        float currentHealthPercent = _currentHealth / _maxHealth;
        onHealthDecrease(currentHealthPercent);

        if (_currentHealth <= 0) WolfDie();
    }

    public void IncreaseHP(float maxHealth)
    {
        _currentHealth = maxHealth;
        onHealthIncrease(_currentHealth / maxHealth);
        if (_currentHealth > maxHealth) _currentHealth = maxHealth;
    }

    public void TakeDamage(string skillNumber)
    {
        if(_detection != null && _detection._playerInInteractionArea && _detection._aimRay._iteractableObjectInFocus && !_isDie)
        {
            if(skillNumber == "Alpha2") AudioSource.PlayClipAtPoint(_wolfKickDamage, transform.position);
            else AudioSource.PlayClipAtPoint(_wolfAxeHit, transform.position);
            
            DecreaseHP(_detection._playerCharacteristics._dealDamage);
            _lastDamageTime = Time.time;
        }
    }

    private void RechargeHitPoints()
    {
        if (Time.time - _lastDamageTime >= _rechargeHitpointsTimer)
        {
            IncreaseHP(_maxHealth);
        }
    }

    private void WolfDie() 
    {
        _isDie = true;
        _wolfAnimator.SetBool("isDie", _isDie);
        _lastDamageTime = Time.time;
    }

    private void OnWolfDie(AnimationEvent animationEvent)
    {
        AudioSource.PlayClipAtPoint(_wolfDie, transform.position);
    }
}

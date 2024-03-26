using System;
using UnityEngine;

public class WolfCharacteristics : MonoBehaviour
{
    private Collider wolfCollider;
    private Animator wolfAnimator;
    public AudioClip wolfDie;
    public AudioClip wolfAxeHit;
    private ThirdPersonController playerController;
    private PlayerCharacteristics playerCharacteristics;

    public event Action<float> onHealthDecrease = delegate { };
    public event Action<float> onHealthIncrease = delegate { };

    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float damage = 25f;

    private float lastDamageTime = 0f;
    private float takeDamageInterval = 0.70f;

    private float rechargeHitpointsTimer = 15f;

    [SerializeField] private bool canDamage = true;
    public bool isDie = false;
    private bool isDestroyed = false;
    [SerializeField] private float destroyTimer = 60f;

    private void Start()
    {
        wolfAnimator = GetComponent<Animator>();
        wolfCollider = GetComponent<Collider>();
        playerController = GameObject.Find("Player").GetComponent<ThirdPersonController>();
        playerCharacteristics = GameObject.Find("Player").GetComponent<PlayerCharacteristics>();
        currentHealth = maxHealth;
    }
    private void Update()
    {
        if (!isDie && Time.time - lastDamageTime >= takeDamageInterval)
        {
            canDamage = true;
            TakeDamage();
        }

        if (!isDie && Time.time - lastDamageTime >= rechargeHitpointsTimer)
        {
            canDamage = true;
            RechargeHitPoints();
        }

        if(isDie && !isDestroyed && Time.time - lastDamageTime >= destroyTimer)
        {
            isDestroyed = true;
            Destroy(gameObject);
        }
    }
    public void DecreaseHP(float amount)
    {
        currentHealth -= amount;
        lastDamageTime = Time.time;
        float currentHealthPercent = currentHealth / maxHealth;
        onHealthDecrease(currentHealthPercent);

        if (currentHealth == 0) WolfDie();
    }

    public void IncreaseHP(float maxHealth)
    {
        currentHealth = maxHealth;
        onHealthIncrease(currentHealth / maxHealth);
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    private void TakeDamage()
    {
        if (!playerCharacteristics.isDie)
        {
            if (wolfCollider.bounds.Intersects(GameObject.Find("Axe").GetComponent<Collider>().bounds))
            {
                if (canDamage && playerController.isAttacking)
                {
                    AudioSource.PlayClipAtPoint(wolfAxeHit,transform.position);
                    DecreaseHP(damage);
                    canDamage = false;
                    lastDamageTime = Time.time;
                }
            }
        }
    }

    private void RechargeHitPoints()
    {
        if (Time.time - lastDamageTime >= rechargeHitpointsTimer)
        {
            IncreaseHP(maxHealth);
        }
    }

    private void WolfDie() 
    {
        isDie = true;
        wolfAnimator.SetBool("isDie", isDie);
        lastDamageTime = Time.time;
    }

    private void OnWolfDie(AnimationEvent animationEvent)
    {
        AudioSource.PlayClipAtPoint(wolfDie, transform.position);
    }
}

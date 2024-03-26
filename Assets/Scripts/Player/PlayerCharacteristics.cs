using System;
using UnityEngine;

public class PlayerCharacteristics : MonoBehaviour
{
    public event Action onPlayerDead;
    public event Action<float> onHealthDecrease = delegate { };
    public event Action<float> onHealthIncrease = delegate { };

    private CharacterController playerController;
    private Animator playerAnimator;

    [SerializeField] private float currentHealth;
    private float maxHealth = 100.0f;
    private float damage = 15f;
    private float passiveHealPerSecond = 0.01f;
    private float lastDamageTime = 0f;
    private float regenerationDelay = 5f;
    private float damageInterval = 1.8f;

    private bool canDamage;
    public bool isDie = false;

    [Header("Enemies Damage Control")]
    private Collider[] enemyDamageColliders = new Collider[3];
    public Transform interactionPoint;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float interactionRadius = 5f;
    [SerializeField] private int collidersNumFound;

    private void Start()
    {
        playerController = GetComponent<CharacterController>();
        playerAnimator = GetComponent<Animator>();
        currentHealth = maxHealth;
        interactionPoint = transform;
    }
    private void Update()
    {
        if (!isDie)
        {
            RegenerationHP();
            CalculateEnemiesDamage();
            TakeDamage();
        }
    }

    private void CalculateEnemiesDamage()
    {
        collidersNumFound = Physics.OverlapSphereNonAlloc(interactionPoint.position, interactionRadius, enemyDamageColliders, enemyLayer);
        if (collidersNumFound > 0)
        {
            if (!enemyDamageColliders[0].GetComponentInParent<WolfCharacteristics>().isDie
                && Time.time - lastDamageTime >= damageInterval) canDamage = true;
        }
        else canDamage = false;
    }

    public void DecreaseHP(float amount)
    {
        currentHealth -= amount;
        lastDamageTime = Time.time;
        float currentHealthPercent = currentHealth / maxHealth;
        onHealthDecrease(currentHealthPercent);
    }

    public void IncreaseHP(float passiveHealPerSecond)
    {
        currentHealth += passiveHealPerSecond;
        onHealthIncrease(currentHealth / maxHealth);
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    private void RegenerationHP()
    {
        if ((Time.time - lastDamageTime >= regenerationDelay) && currentHealth <= maxHealth && !canDamage)
        {
            IncreaseHP(passiveHealPerSecond);
        }
    }

    private void TakeDamage()
    {
        if (currentHealth <= 0)
        {
            isDie = true;
            playerAnimator.SetBool("isDie", isDie);
            onPlayerDead.Invoke();
            canDamage = false;
        }
        if (canDamage)
        {
            if (playerController.bounds.Intersects(GameObject.FindWithTag("EnemyDamageCollider").GetComponent<Collider>().bounds))
            {
                DecreaseHP(damage);
                canDamage = false;
                lastDamageTime = Time.time;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(interactionPoint.position, interactionRadius);
    }
}

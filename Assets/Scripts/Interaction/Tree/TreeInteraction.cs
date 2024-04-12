using UnityEngine;

public class TreeInteraction : MonoBehaviour
{
    private Collider _treeCollider;
    private PlayerDetectionObject _detection;

    private Animation _treeAnimation;

    [Header("Falling Tree Object")]
    [SerializeField] private GameObject _fallingTree;

    [SerializeField] private AudioClip _chopSound;

    [Header("Tree stats")]
    public float _treeHealth = 100f;
    public float _damagePerHit = 20f;
    public bool _treeChopped = false;
    public bool _canDamageTree = true;

    [Header("Resource")]
    public GameObject _treeResourcePrefab;
    public float _resourceDropChance = 0.5f;
    public float _treeSpawnRadius = 5f;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _fallingLeaves;

    private void Start()
    {
        _detection = GetComponentInChildren<PlayerDetectionObject>();
        _treeCollider = GetComponent<Collider>();
        _treeAnimation = GetComponent<Animation>();
    }

    private void FixedUpdate()
    {
        if (_treeHealth <= 0 && !_treeChopped)
        {
            FallingTree();
            _treeChopped = true;
        }
    }

    public void SpawnResource()
    {
        Vector2 randomOffset = Random.insideUnitCircle * _treeSpawnRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, 0.0f, randomOffset.y);
        spawnPosition.y = 0.25f;

        if ((spawnPosition - _detection._player.transform.position).magnitude > 3f)
        {
            Instantiate(_treeResourcePrefab, spawnPosition, Quaternion.identity);
        }
        else SpawnResource();
    }

    private void FallingTree()
    {
        Instantiate(_fallingTree, gameObject.transform.position, Quaternion.identity);

        _fallingTree.GetComponent<Rigidbody>().isKinematic = false;
        gameObject.SetActive(false);
    }

    public void Chop()
    {
        if (_detection._playerInInteractionArea && _detection._aimRay._iteractableObjectInFocus 
            && !_treeChopped && _detection._axe != null && _detection._playerWeaponsEquip._isWeaponEquiped)
        {
            _canDamageTree = false;
            AudioSource.PlayClipAtPoint(_chopSound, transform.position);
            _treeAnimation.Play();
            _fallingLeaves.Play();

            if (_treeHealth > 0)
            {
                _treeHealth -= _damagePerHit;
            }
            else _treeChopped = true;

            if (Random.value > _resourceDropChance)
            {
                SpawnResource();
            }
        }
        
    }
}

using UnityEngine;

public class PlayerDetectionObject : MonoBehaviour
{
    private Collider _detectionCollider;

    //Player Objects
    public GameObject _player;
    public PlayerInputHandler _playerInputHandler;
    public PlayerWeaponsEquip _playerWeaponsEquip;
    public Collider _axe;
    public PlayerAimRay _aimRay;
    public PlayerAnimEvents _playerAnimEvents;
    public bool _playerInInteractionArea;

    private void Start()
    {
        _detectionCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInInteractionArea = true;
            _player = GameObject.Find("Player");
            _playerInputHandler = _player.GetComponent<PlayerInputHandler>();
            _playerWeaponsEquip = _player.GetComponent<PlayerWeaponsEquip>();
            try
            {
                _axe = GameObject.Find("Axe").GetComponent<Collider>();
            }
            catch
            {
                Debug.Log("No Axe equiped");
                _axe = null;
            }
            _aimRay = _player.GetComponentInChildren<PlayerAimRay>();
            _playerAnimEvents = _player.GetComponent<PlayerAnimEvents>();
            if (gameObject.GetComponentInParent<TreeInteraction>() != null) _playerAnimEvents.onTreeChop += GetComponentInParent<TreeInteraction>().Chop;
            if (gameObject.GetComponentInParent<WolfCharacteristics>() != null) _playerAnimEvents.onEnemyHit += GetComponentInParent<WolfCharacteristics>().TakeDamage;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInInteractionArea = true;
            _playerInputHandler = null;
            _playerWeaponsEquip = null;
            _player = null;
            _axe = null;
            _aimRay = null;
            if (_playerAnimEvents != null)
            {
                if (gameObject.GetComponentInParent<TreeInteraction>() != null) _playerAnimEvents.onTreeChop -= GetComponentInParent<TreeInteraction>().Chop;
                if (gameObject.GetComponentInParent<WolfCharacteristics>() != null) _playerAnimEvents.onEnemyHit -= GetComponentInParent<WolfCharacteristics>().TakeDamage;
                _playerAnimEvents = null;
            }
        }
    }
}

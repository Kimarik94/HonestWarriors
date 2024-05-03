using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float _radius = 1.5f;
    public Transform _interactionTransform;

    public bool _isFocus = false;
    Transform _player;

    bool _hasInteracted = false;

    public virtual void Interact()
    {
        
    }

    void Update()
    {
        if (_isFocus && !_hasInteracted)
        {   
            if (_player.gameObject.GetComponent<PlayerInputHandler>()._InteractPressed 
                && _player.GetComponent<ThirdPersonController>()._focus != null)
            {
                Interact();
                _hasInteracted = true;
            }
        }
    }

    public void OnFocused(Transform playerTransform)
    {
        _isFocus = true;
        _player = playerTransform;
        _hasInteracted = false;
    }

    public void OnDefocused()
    {
        _isFocus = false;
        _player = null;
        _hasInteracted = false;
    }

    void OnDrawGizmosSelected()
    {
        if (_interactionTransform == null)
            _interactionTransform = transform;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_interactionTransform.position, _radius);
    }

}

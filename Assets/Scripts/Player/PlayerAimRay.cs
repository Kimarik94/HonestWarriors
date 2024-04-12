using UnityEngine;

public class PlayerAimRay : MonoBehaviour
{
    private Camera _mainCamera;
    public bool _iteractableObjectInFocus;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        Vector3 crosshairPosition = _mainCamera.
            ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f));

        Vector3 direction = transform.position - crosshairPosition;

        Ray ray = new Ray(transform.position, direction * 40f);

        Debug.DrawRay(ray.origin, ray.direction, Color.blue);

        if(Physics.Raycast(ray, out RaycastHit hit)){
            if (hit.collider.CompareTag("Interactable") || hit.collider.CompareTag("Enemy"))
            {
                _iteractableObjectInFocus = true;
            }
            else
            {
                _iteractableObjectInFocus = false;
            }
        }
    }
}

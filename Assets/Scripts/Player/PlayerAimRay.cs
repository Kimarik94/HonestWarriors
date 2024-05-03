using UnityEngine;

public class PlayerAimRay : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    public bool _iteractableObjectInFocus;

    private void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
    }

    private void Update()
    {
        if(mainCamera == null) mainCamera = Camera.main;

        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        Vector3 crosshairPosition = mainCamera.ScreenToWorldPoint(screenCenter);

        Vector3 direction = crosshairPosition - transform.position;

        Ray ray = new Ray(transform.position, direction*-1);

        Debug.DrawRay(ray.origin, ray.direction, Color.blue);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
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

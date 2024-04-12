using UnityEngine;

public class BasicRigidBodyPush : MonoBehaviour
{
	public LayerMask _pushLayers;
	public bool _canPush;
	[Range(0.5f, 5f)] public float _strength = 1.1f;

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (_canPush) PushRigidBodies(hit);
	}

	private void PushRigidBodies(ControllerColliderHit hit)
	{
		// make sure we hit a non kinematic rigidbody
		Rigidbody body = hit.collider.attachedRigidbody;
		if (body == null || body.isKinematic) return;

		// make sure we only push desired layer(s)
		var bodyLayerMask = 1 << body.gameObject.layer;
		if ((bodyLayerMask & _pushLayers.value) == 0) return;

		// We dont want to push objects below us
		if (hit.moveDirection.y < -0.3f) return;

		// Calculate push direction from move direction, horizontal motion only
		Vector3 pushDir = new Vector3(hit.moveDirection.x, 0.0f, hit.moveDirection.z);

		// Apply the push and take strength into account
		body.AddForce(pushDir * _strength, ForceMode.Impulse);
	}
}
using UnityEngine;
using System.Collections;

public class moveObject : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool movementEnabled = true;

	public float speed;
	public float moveAmount;
	public Vector3 direction = Vector3.up;

	public bool useLocalPosition;

	[Space]
	[Header ("Debug")]
	[Space]

	public Vector3 originalPosition;

	Transform mainTransform;

	Vector3 vectorRight = Vector3.right;
	Vector3 vectorUp = Vector3.up;
	Vector3 vectorForward = Vector3.forward;


	void Start ()
	{
		mainTransform = transform;

		if (useLocalPosition) {
			originalPosition = mainTransform.localPosition;
		} else {
			originalPosition = mainTransform.position;
		}
	}

	void FixedUpdate ()
	{
		if (movementEnabled) {
			if (useLocalPosition) {
				
				mainTransform.localPosition = originalPosition +
				((Mathf.Cos (Time.time * speed)) / 2) * moveAmount *
				(direction.x * vectorRight + direction.y * vectorUp + direction.z * vectorForward);
				
			} else {
				
				mainTransform.position = originalPosition +
				((Mathf.Cos (Time.time * speed)) / 2) * moveAmount *
				(direction.x * mainTransform.right + direction.y * mainTransform.up + direction.z * mainTransform.forward);
				
			}
		}
	}

	public void enableMovement ()
	{
		setMovementEnabledState (true);
	}

	public void disableMovement ()
	{
		setMovementEnabledState (false);
	}

	public void setMovementEnabledState (bool state)
	{
		movementEnabled = state;
	}

	public void changeMovementEnabledState ()
	{
		setMovementEnabledState (!movementEnabled);
	}
}

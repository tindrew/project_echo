using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class preventCharacterCollisionOnTop : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool collisionCheckEnabled = true;

	public float minTimeToPushCharacter = 1;

	public float minDistanceToPushCharacter = 1.5f;

	public float pushDownExtraForce = 1;

	public LayerMask layerToIgnore;

	public float pushForce = 10;

	public ForceMode pushForceMode;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool pushInProcess;

	[Space]
	[Header ("Components")]
	[Space]

	public Rigidbody mainRigidbody;

	public playerController mainPlayerController;



	Transform lastTransformDetected;

	Coroutine updateCoroutine;

	float lastTimeObjectDetected;


	IEnumerator updateSystemCoroutine ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			updateSystem ();

			yield return waitTime;
		}
	}

	void updateSystem ()
	{
		if (Time.time > lastTimeObjectDetected + minTimeToPushCharacter) {
			stopUpdateCoroutine ();

			if (showDebugPrint) {
				print ("stop push for time");
			}

			return;
		}

		if (mainPlayerController.isPlayerOnGround ()) {
			stopUpdateCoroutine ();

			if (showDebugPrint) {
				print ("stop push for character on ground");
			}

			return;
		}

		float distanceToTarget = GKC_Utils.distance (mainRigidbody.position, lastTransformDetected.position);

		if (distanceToTarget < 0.6f) {
			stopUpdateCoroutine ();

			if (showDebugPrint) {
				print ("stop push for distance");
			}

			return;
		}

		Vector3 heading = mainRigidbody.position - lastTransformDetected.position;

		heading = heading - mainRigidbody.transform.up * mainRigidbody.transform.InverseTransformDirection (heading).y;

		float distance = heading.magnitude;
		Vector3 pushDirection = heading / distance;

		pushDirection.Normalize ();

		if (pushDownExtraForce > 0) {
			pushDirection -= mainRigidbody.transform.up * pushDownExtraForce;
		}

		mainRigidbody.AddForce (pushDirection * pushForce, pushForceMode);
	}

	public void stopUpdateCoroutine ()
	{
		if (updateCoroutine != null) {
			StopCoroutine (updateCoroutine);
		}

		pushInProcess = false;
	}

	void OnCollisionEnter (Collision col)
	{
		if (!collisionCheckEnabled) {
			return;
		}

		if (pushInProcess) {
			if (lastTransformDetected == col.transform) {
				return;
			}
		}

		GameObject objectDetected = col.gameObject;

		if ((1 << objectDetected.layer & layerToIgnore.value) == 1 << objectDetected.layer) {
			float distanceToTarget = GKC_Utils.distance (mainRigidbody.position, objectDetected.transform.position);

			if (showDebugPrint) {
				print ("distance to target " + distanceToTarget);
			}

			if (distanceToTarget < minDistanceToPushCharacter) {
				return;
			}

			lastTimeObjectDetected = Time.time;

			lastTransformDetected = objectDetected.transform;

			pushInProcess = true;

			updateCoroutine = StartCoroutine (updateSystemCoroutine ());

			if (showDebugPrint) {
				print ("push activated");
			}
		}
	}
}

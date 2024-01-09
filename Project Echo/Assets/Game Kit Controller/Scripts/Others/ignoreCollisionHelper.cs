using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ignoreCollisionHelper : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool collisionCheckEnabled = true;

	public LayerMask layerToIgnore;

	public bool ignoreByTagEnabled;
	public List<string> tagToIgnoreList = new List<string> ();

	public bool storeObjectsIgnored;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;
	public List<Collider> objectsIgnoredList = new List<Collider> ();

	public bool objectsStored;

	[Space]
	[Header ("Component")]
	[Space]

	public Collider mainCollider;

	bool mainColliderLocated;

	Coroutine componentTemporaryCoroutine;

	Coroutine reactivateWithDelayCoroutine;


	void OnCollisionEnter (Collision col)
	{
		if (!collisionCheckEnabled) {
			return;
		}

		GameObject objectDetected = col.gameObject;

		bool ignoreColliderResult = false;

		if ((1 << objectDetected.layer & layerToIgnore.value) == 1 << objectDetected.layer) {
			ignoreColliderResult = true;
		}

		if (!ignoreColliderResult) {
			if (ignoreByTagEnabled) {
				if (tagToIgnoreList.Contains (objectDetected.tag)) {
					ignoreColliderResult = true;
				}
			}
		}

		if (ignoreColliderResult) {
			if (!mainColliderLocated) {
				mainColliderLocated = mainCollider != null;

				if (!mainColliderLocated) {
					mainCollider = GetComponent<Collider> ();

					mainColliderLocated = mainCollider != null;
				}
			}

			if (!mainColliderLocated) {
				return;
			}

			Physics.IgnoreCollision (col.collider, mainCollider, true);

			if (showDebugPrint) {
				print ("collision ignored with " + col.collider.name);
			}

			if (storeObjectsIgnored) {
				if (!objectsIgnoredList.Contains (col.collider)) {
					objectsIgnoredList.Add (col.collider);

					objectsStored = true;
				}
			}
		}
	}

	public void setCollisionCheckEnabledState (bool state)
	{
		collisionCheckEnabled = state;
	}

	public void reactivateIgnoredCollisionsStored ()
	{
		if (objectsStored) {

			for (int i = 0; i < objectsIgnoredList.Count; i++) {
				if (objectsIgnoredList [i] != null) {
					Physics.IgnoreCollision (objectsIgnoredList [i], mainCollider, false);
				}
			}

			clearObjectsIgnoredList ();
		}
	}

	public void clearObjectsIgnoredList ()
	{
		objectsIgnoredList.Clear ();

		objectsStored = false;
	}

	public void enableComponentTemporary (float duration)
	{
		enableOrDisableComponentTemporary (true, duration);
	}

	public void disableComponentTemporary (float duration)
	{
		enableOrDisableComponentTemporary (false, duration);
	}

	public void enableOrDisableComponentTemporary (bool state, float duration)
	{
		if (state) {
			collisionCheckEnabled = true;

			stopReactivateCollisionsStoredWithDelayCoroutine ();

			componentTemporaryCoroutine = StartCoroutine (enableComponentTemporaryCoroutine (duration));
		} else {
			stopEnableComponentTemporaryCoroutine ();

			collisionCheckEnabled = false;
		}
	}

	void stopEnableComponentTemporaryCoroutine ()
	{
		if (componentTemporaryCoroutine != null) {
			StopCoroutine (componentTemporaryCoroutine);
		}
	}

	IEnumerator enableComponentTemporaryCoroutine (float duration)
	{
		WaitForSeconds delay = new WaitForSeconds (duration);

		yield return delay;
	
		collisionCheckEnabled = false;
	}

	public void reactivateCollisionsStoredWithDelay (float duration)
	{
		reactivateWithDelayCoroutine = StartCoroutine (reactivateCollisionsStoredWithDelayCoroutine (duration));
	}

	void stopReactivateCollisionsStoredWithDelayCoroutine ()
	{
		if (reactivateWithDelayCoroutine != null) {
			StopCoroutine (reactivateWithDelayCoroutine);
		}
	}

	IEnumerator reactivateCollisionsStoredWithDelayCoroutine (float duration)
	{
		WaitForSeconds delay = new WaitForSeconds (duration);

		yield return delay;

		reactivateIgnoredCollisionsStored ();
	}
}

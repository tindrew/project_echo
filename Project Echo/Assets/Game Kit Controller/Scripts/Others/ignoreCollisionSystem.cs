using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ignoreCollisionSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool ignoreCollisionEnabled = true;

	public bool useColliderList;
	public List<Collider> colliderList = new List<Collider> ();

	[Space]

	public bool activateCheckOnStart;

	public Collider colliderToIgnore;

	public bool forceIgnoreCollisionInBetweenObjectsOnColliderList;

	[Space]
	[Header ("Components")]
	[Space]

	public Collider mainCollider;


	void Start ()
	{
		if (activateCheckOnStart) {
			activateIgnoreCollision (colliderToIgnore);
		}
	}

	public void activateIgnoreCollision (Collider objectToIgnore)
	{
		if (ignoreCollisionEnabled) {
			
			if (useColliderList) {
				if (objectToIgnore != null) {
					ignoreCollisionOnCollider (objectToIgnore);
				}

				if (forceIgnoreCollisionInBetweenObjectsOnColliderList) {

					int colliderListCount = colliderList.Count;

					for (int i = 0; i < colliderListCount; i++) {
						ignoreCollisionOnCollider (colliderList [i]);
					}
				}
			} else {
				if (objectToIgnore == null) {
					return;
				}

				if (mainCollider != null) {
					Physics.IgnoreCollision (objectToIgnore, mainCollider, true);
				}
			}
		}
	}

	void ignoreCollisionOnCollider (Collider newCollider)
	{
		int colliderListCount = colliderList.Count;

		for (int i = 0; i < colliderListCount; i++) {
			if (colliderList [i] != null && colliderList [i] != newCollider) {
				Physics.IgnoreCollision (newCollider, colliderList [i], true);
			}
		}
	}
}

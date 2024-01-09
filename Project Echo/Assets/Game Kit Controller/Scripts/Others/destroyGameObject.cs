using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class destroyGameObject : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool destroyObjectEnabled = true;

	public bool destroyObjectOnEnable;

	public float timer = 0.6f;
	public bool destroyObjectAtStart = true;

	public bool disableInsteadOfDestroyActive;

	public bool sendObjectToPoolSystemToDisable;

	public bool destroyJustAllChildObjects;

	[Space]
	[Header ("Objects To Destroy Settings")]
	[Space]

	public GameObject objectToDestroy;

	[Space]
	[Space]

	public bool useGameObjectList;
	public List<GameObject> gameObjectList = new List<GameObject> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;
	public bool destroyCoroutineActive;

	public bool ignoreDestroyObjectEnabled;


	Coroutine destroyObjectCoroutine;

	bool destroyObjectFunctionCalled;


	void Start ()
	{
		checkToDestroyObjectInTime (true);
	}

	public void destroyObjectInTime ()
	{
		stopDestroyObjectCoroutine ();

//		print (gameObject.name + " " + timer);

		destroyObjectCoroutine = StartCoroutine (destroyObjectInTimeCoroutine ());
	}

	public void stopDestroyObjectCoroutine ()
	{
		destroyCoroutineActive = false;

		if (destroyObjectCoroutine != null) {
			StopCoroutine (destroyObjectCoroutine);
		}
	}

	IEnumerator destroyObjectInTimeCoroutine ()
	{
		destroyCoroutineActive = true;

//		print (gameObject.name + " " + timer);

		WaitForSeconds delay = new WaitForSeconds (timer);

		yield return delay;

		destroyCoroutineActive = false;

		destroy ();
	}

	public void setTimer (float timeToDestroy)
	{
		timer = timeToDestroy;
	}

	void OnDisable ()
	{
		if (timer > 0 && destroyObjectFunctionCalled) {
			if (!disableInsteadOfDestroyActive) {

				if (showDebugPrint) {
					print ("disabling objects " + Application.isPlaying + " time " + Time.timeScale + " " + Time.deltaTime);
				}

				if (Time.timeScale > 0 || Time.deltaTime > 0) {
					if (showDebugPrint) {
						print ("DESTROYING OBJECT");
					}

					destroy ();
				} else {
					if (showDebugPrint) {
						print ("TRYING TO DESTROY OBJECT OUT OF PLAY TIME");
					}
				}
			}
		}

//		print (gameObject.name + " " + timer + " " + destroyCoroutineActive);
	}

	void OnEnable ()
	{
		if (destroyObjectOnEnable) {
			checkToDestroyObjectInTime (false);
		}
	}

	public void destroy ()
	{
		if (!destroyObjectEnabled) {
			return;
		}

		if (ignoreDestroyObjectEnabled) {
			return;
		}

		if (objectToDestroy == null) {
			objectToDestroy = gameObject;
		}

		destroyObjectFunctionCalled = true;

		if (disableInsteadOfDestroyActive) {
			if (sendObjectToPoolSystemToDisable) {
				GKC_PoolingSystem.Despawn (objectToDestroy);
			} else {
				objectToDestroy.SetActive (false);
			}

			if (useGameObjectList) {
				for (int i = 0; i < gameObjectList.Count; i++) {
					if (gameObjectList [i] != null) {
						if (sendObjectToPoolSystemToDisable) {
							GKC_PoolingSystem.Despawn (gameObjectList [i]);
						} else {
							if (gameObjectList [i].activeSelf) {
								gameObjectList [i].SetActive (false);
							}
						}
					}
				}
			}
		} else {
			if (GKC_Utils.isApplicationPlaying () && Time.deltaTime > 0) {

				if (destroyJustAllChildObjects) {
					Component[] components = objectToDestroy.GetComponentsInChildren (typeof(Transform));

					int componentsLength = components.Length;

					for (int i = 0; i < componentsLength; i++) {

						Transform child = components [i] as Transform;
					
						if (child != objectToDestroy.transform) {
							Destroy (child.gameObject);
						}
					}
				} else {
					Destroy (objectToDestroy);

					if (useGameObjectList) {
						for (int i = 0; i < gameObjectList.Count; i++) {
							if (gameObjectList [i] != null) {
								Destroy (gameObjectList [i]);
							}
						}
					}
				}
			}
		}
	}

	public void setDestroyObjectEnabledState (bool state)
	{
		destroyObjectEnabled = state;
	}

	public void setIgnoreDestroyObjectEnabledState (bool state)
	{
		ignoreDestroyObjectEnabled = state;
	}

	public void changeDestroyForSetActiveFunction (bool state)
	{
		disableInsteadOfDestroyActive = state;
	}

	public void setSendObjectToPoolSystemToDisableState (bool state)
	{
		sendObjectToPoolSystemToDisable = state;

		if (sendObjectToPoolSystemToDisable) {
			disableInsteadOfDestroyActive = true;
		}
	}

	public void checkToDestroyObjectInTime (bool callingFromStart)
	{
		if (!destroyCoroutineActive) {
			if ((destroyObjectAtStart && callingFromStart) || !callingFromStart) {
				destroyObjectInTime ();
			}
		}
	}

	public void cancelDestroy ()
	{
		stopDestroyObjectCoroutine ();
	}

	public void addObjectToGameObjectList (GameObject newObject)
	{
		if (!gameObjectList.Contains (newObject)) {
			gameObjectList.Add (newObject);
		}

		for (int i = gameObjectList.Count - 1; i >= 0; i--) {	
			if (gameObjectList [i] == null) {
				gameObjectList.RemoveAt (i);
			}
		}
	}

	public void addObjectToGameObjectListFromEditor (GameObject newObject)
	{
		addObjectToGameObjectList (newObject);

		updateComponent ();
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Destroy GameObject", gameObject);
	}
}
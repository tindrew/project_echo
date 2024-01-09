using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleBulletTimeOnCharacterEffect : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool effectEnabled = true;

	public float characterAnimationSpeed = 1.6f;

	public bool setNewTimeScale;
	public float newTimeScale;

	public bool useModelMeshTrailEffectEnabled = true;

	public float modelMeshTrailRefreshRate = 0.3f;

	public Material meshTrailMaterial;

	public int meshTrailMaxNumber = 50;

	public int numberOfModelMeshes = 7;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool effectActive;

	[Space]
	[Header ("Components")]
	[Space]

	public playerController mainPlayerController;

	public Transform playerTransform;

	public timeBullet mainTimeBullet;

	public List<SkinnedMeshRenderer> mainSkinnedMeshRenderer = new List<SkinnedMeshRenderer> ();


	List<GameObject> meshTrailCopiesList = new List<GameObject> ();

	Coroutine updateCoroutine;


	float lastTimeModelMeshCreated = -1;


	public void stopUpdateCoroutine ()
	{
		if (updateCoroutine != null) {
			StopCoroutine (updateCoroutine);
		}
	}

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
		if (useModelMeshTrailEffectEnabled) {

			if (Time.time > modelMeshTrailRefreshRate + lastTimeModelMeshCreated) {
				if (mainPlayerController.isPlayerMoving (0.1f)) {

					int mainSkinnedMeshRendererCount = mainSkinnedMeshRenderer.Count;

					for (int i = 0; i < mainSkinnedMeshRendererCount; i++) {
						if (mainSkinnedMeshRenderer [i] != null) {
							GameObject newCopy = new GameObject ();

							newCopy.transform.SetParent (transform);

							Vector3 targetPosition = mainSkinnedMeshRenderer [i].transform.position;
							Vector3 targetRotation = mainSkinnedMeshRenderer [i].transform.eulerAngles;

							newCopy.transform.position = targetPosition;
							newCopy.transform.eulerAngles = targetRotation;

							MeshRenderer newMeshRenderer = newCopy.AddComponent<MeshRenderer> ();

							MeshFilter newMeshFilter = newCopy.AddComponent<MeshFilter> ();

							Mesh newMesh = new Mesh ();

							mainSkinnedMeshRenderer [i].BakeMesh (newMesh);

							newMeshFilter.mesh = newMesh;

							Material[] allMats = newMeshRenderer.materials;

							int materialsLength = allMats.Length;

							for (int m = 0; m < materialsLength; m++) {
								allMats [m] = meshTrailMaterial;
							}

							newMeshRenderer.materials = allMats;

							meshTrailCopiesList.Add (newCopy);
						}
					}

					lastTimeModelMeshCreated = Time.time;
				}

				if (meshTrailCopiesList.Count > meshTrailMaxNumber && meshTrailCopiesList.Count % numberOfModelMeshes == 0) {
					for (int i = numberOfModelMeshes - 1; i >= 0; i--) {	
						GameObject objectToDestroy = meshTrailCopiesList [i];

						Destroy (objectToDestroy);

						meshTrailCopiesList.RemoveAt (i);
					}
				}
			}
		}
	}

	public void enableOrDisableEffect (bool state)
	{
		if (effectEnabled) {
			effectActive = state;

			if (effectActive) {
				mainPlayerController.setReducedVelocity (characterAnimationSpeed);

				updateCoroutine = StartCoroutine (updateSystemCoroutine ());
			} else {
				mainPlayerController.setNormalVelocity ();

				stopUpdateCoroutine ();
			}

			if (setNewTimeScale) {
				if (effectActive) {
					mainTimeBullet.setBulletTimeState (true, newTimeScale);
				} else {
					mainTimeBullet.setBulletTimeState (false, 1);
				}
			}

			if (useModelMeshTrailEffectEnabled) {
				if (meshTrailCopiesList.Count > 0) {
					for (int i = meshTrailCopiesList.Count - 1; i >= 0; i--) {	
						GameObject objectToDestroy = meshTrailCopiesList [i];

						Destroy (objectToDestroy);

						meshTrailCopiesList.RemoveAt (i);
					}
				}
			}
		}
	}
}

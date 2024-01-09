using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class handleVehicleAIBehavior : AIBehaviorInfo
{
	[Header ("Custom Settings")]
	[Space]

	public bool searchVehicleEnabled = true;

	public bool startGameSearchingForVehicle;

	[Space]

	public bool useMaxDistanceToGetVehicle;

	public float maxDistanceToGetVehicle;

	public float minDistanceToGetOnCurrentVehicle = 3.5f;

	public bool ignoreCheckVehicleIfTargetToAttackFound;

	[Space]

	public bool stopBehaviorUpdateOnGetOffFromVehicle = true;
	public bool stopBehaviorUpdateOnVehicleReached = true;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool characterOnVehicle;

	public bool searchingVehicle;
	public bool vehicleToReachFound;

	public bool currentVehicleAINavmeshLocated;

	public bool getOffFromVehicleOnDestinyReached;

	[Space]

	public Transform currentVehicleToGet;
	public vehicleAINavMesh currentVehicleAINavMesh;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventOnNoVehicleToPickFromScene;
	public UnityEvent eventOnNoVehicleToPickFromScene;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform AITransform;
	public findObjectivesSystem mainFindObjectivesSystem;
	public AINavMesh mainAINavmeshManager;


	Coroutine updateCoroutine;

	bool followingTargetPreviously;


	void Start ()
	{
		if (searchVehicleEnabled) {
			if (startGameSearchingForVehicle) {
				StartCoroutine (startGameSearchingForVehicleCoroutine ());
			}
		}
	}

	IEnumerator startGameSearchingForVehicleCoroutine ()
	{
		yield return new WaitForSeconds (0.3f);

		activateAIBehavior ();
	}

	public override void activateAIBehavior ()
	{
		if (searchVehicleEnabled) {
			characterOnVehicle = mainFindObjectivesSystem.isCharacterOnVehicle ();

			updateCoroutine = StartCoroutine (updateSystemCoroutine ());
		}
	}

	public override void deactivateAIBehavior ()
	{
		if (searchVehicleEnabled) {
			stopUpdateCoroutine ();
		}
	}

	public void stopUpdateCoroutine ()
	{
		if (updateCoroutine != null) {
			StopCoroutine (updateCoroutine);
		}
	}

	IEnumerator updateSystemCoroutine ()
	{
		var waitTime = new WaitForSecondsRealtime (0.0001f);

		while (true) {
			updateSystem ();

			yield return waitTime;
		}
	}

	public void setGetOffFromVehicleOnDestinyReachedState (bool state)
	{
		getOffFromVehicleOnDestinyReached = state;
	}

	public void getOffFromVehicle ()
	{
		mainFindObjectivesSystem.getOffFromVehicle ();
	}

	void updateSystem ()
	{
		if (characterOnVehicle) {
			if (currentVehicleAINavmeshLocated) {
				if (currentVehicleAINavMesh.isFollowingTarget ()) {
					followingTargetPreviously = true;
				} else {
					if (followingTargetPreviously) {
						getOffFromVehicle ();

						if (stopBehaviorUpdateOnGetOffFromVehicle) {
							stopUpdateCoroutine ();
						}

						characterOnVehicle = false;

						return;
					}
				}
			}
		} else {
			if (searchingVehicle) {
				bool vehicleReached = false;

				if (currentVehicleToGet != null) {
					if (GKC_Utils.distance (mainAINavmeshManager.transform.position, currentVehicleToGet.position) < minDistanceToGetOnCurrentVehicle) {
						if (showDebugPrint) {
							print ("picking vehicle " + currentVehicleToGet.name);
						}

						vehicleHUDManager currentVehicleHUDManager = currentVehicleToGet.GetComponent<vehicleHUDManager> ();

						if (currentVehicleHUDManager != null) {
							IKDrivingSystem currentIKDrivingSystem = currentVehicleHUDManager.getIKDrivingSystem ();

							if (currentIKDrivingSystem != null) {
								currentIKDrivingSystem.setDriverExternally (AITransform.gameObject);

								if (showDebugPrint) {
									print ("sending character to vehicle");
								}

								mainAINavmeshManager.removeTarget ();
							}
						} else {
							GKCSimpleRiderSystem currentGKCSimpleRiderSystem = currentVehicleToGet.GetComponent<GKCSimpleRiderSystem> ();

							if (currentGKCSimpleRiderSystem != null) {
								currentGKCSimpleRiderSystem.setDriverExternally (AITransform.gameObject);

								if (showDebugPrint) {
									print ("sending character to vehicle");
								}
							}
						}

						vehicleReached = true;
					}
				} else {
					vehicleReached = true;
				}

				if (vehicleReached) {
					mainFindObjectivesSystem.setSearchingObjectState (false);

					searchingVehicle = false;

					currentVehicleToGet = null;

					mainFindObjectivesSystem.setIgnoreVisionRangeActiveState (true);

					mainFindObjectivesSystem.resetAITargets ();

					mainFindObjectivesSystem.setIgnoreVisionRangeActiveState (false);

					mainFindObjectivesSystem.setOriginalWanderEnabled ();

					characterOnVehicle = true;

					if (showDebugPrint) {
						print ("vehicle picked, resuming state on AI");
					}

					if (stopBehaviorUpdateOnVehicleReached) {
						stopUpdateCoroutine ();
					}
				}
			} else {
				checkToFindVehicle ();
			}
		}
	}

	void checkToFindVehicle ()
	{
		if (mainFindObjectivesSystem.isCharacterOnVehicle ()) {
			characterOnVehicle = true;

			return;
		}

		if (ignoreCheckVehicleIfTargetToAttackFound) {
			if (mainFindObjectivesSystem.isOnSpotted ()) {
				return;
			}
		}

		characterOnVehicle = false;

		vehicleHUDManager[] vehicleHUDManagerList = FindObjectsOfType (typeof(vehicleHUDManager)) as vehicleHUDManager[];

		List<Transform> vehiclesDetected = new List<Transform> ();

		if (vehicleHUDManagerList.Length > 0) {
			for (int i = 0; i < vehicleHUDManagerList.Length; i++) {
				bool checkObjectResult = true;

				if (vehicleHUDManagerList [i].getCurrentDriver () != null) {
					checkObjectResult = false;
				}

				if (useMaxDistanceToGetVehicle) {
					float distance = GKC_Utils.distance (vehicleHUDManagerList [i].transform.position, AITransform.position);

					if (distance > maxDistanceToGetVehicle) {
						checkObjectResult = false;
					}
				}

				if (checkObjectResult) {
					vehiclesDetected.Add (vehicleHUDManagerList [i].transform);
				}
			}
		}

		GKCSimpleRiderSystem[] GKCSimpleRiderSystemList = FindObjectsOfType (typeof(GKCSimpleRiderSystem)) as GKCSimpleRiderSystem[];

		if (GKCSimpleRiderSystemList.Length > 0) {
			for (int i = 0; i < GKCSimpleRiderSystemList.Length; i++) {
				bool checkObjectResult = true;

				if (GKCSimpleRiderSystemList [i].getCurrentDriver () != null) {
					checkObjectResult = false;
				}

				if (useMaxDistanceToGetVehicle) {
					float distance = GKC_Utils.distance (GKCSimpleRiderSystemList [i].transform.position, AITransform.position);

					if (distance > maxDistanceToGetVehicle) {
						checkObjectResult = false;
					}
				}

				if (checkObjectResult) {
					vehiclesDetected.Add (GKCSimpleRiderSystemList [i].transform);
				}
			}
		}

		if (vehiclesDetected.Count == 0) {
			if (showDebugPrint) {
				print ("no vehicles detected");
			}

			stopUpdateCoroutine ();
		}

		bool vehicleFound = false;

		if (vehiclesDetected.Count > 0) {
			
			vehiclesDetected.Sort (delegate(Transform a, Transform b) {
				return Vector3.Distance (AITransform.position, a.transform.position).CompareTo (Vector3.Distance (AITransform.position, b.transform.position));
			});
			
			if (showDebugPrint) {
				print ("vehicles found on scene " + vehiclesDetected.Count);
			}

			for (int i = 0; i < vehiclesDetected.Count; i++) {
				if (!vehicleFound) {

					if (showDebugPrint) {
						vehicleHUDManager currentVehicleHUDManager = vehiclesDetected [i].GetComponent<vehicleHUDManager> ();

						if (currentVehicleHUDManager != null) {
							print ("checking if vehicle with name " + currentVehicleHUDManager.getVehicleName () + " can be used");
						} else {
							GKCSimpleRiderSystem currentGKCSimpleRiderSystem = vehiclesDetected [i].GetComponent<GKCSimpleRiderSystem> ();

							if (currentGKCSimpleRiderSystem != null) {
								print ("checking if vehicle with name " + currentGKCSimpleRiderSystem.getVehicleName () + " can be used");
							}
						}
					}
						
					if (mainAINavmeshManager.updateCurrentNavMeshPath (vehiclesDetected [i].transform.position)) {
						mainFindObjectivesSystem.setDrawOrHolsterWeaponState (false);

						mainFindObjectivesSystem.setSearchingObjectState (true);

						mainFindObjectivesSystem.setWanderEnabledState (false);

						currentVehicleToGet = vehiclesDetected [i];

						vehicleHUDManager currentVehicleHUDManager = currentVehicleToGet.GetComponent<vehicleHUDManager> ();

						if (currentVehicleHUDManager != null) {
							currentVehicleAINavMesh = currentVehicleHUDManager.getAIVehicleNavmesh ();

							currentVehicleAINavmeshLocated = currentVehicleAINavMesh != null;
						}

						mainAINavmeshManager.setTarget (vehiclesDetected [i]);

						mainAINavmeshManager.setTargetType (false, true);

						vehicleFound = true;

						followingTargetPreviously = false;

						mainAINavmeshManager.lookAtTaget (false);

						if (showDebugPrint) {
							print ("vehicle to use located, setting searching vehicle state on AI");
						}

						searchingVehicle = true;
					}
				}
			}
		} else {
			if (showDebugPrint) {
				print ("no vehicles found");
			}
		}

		if (!vehicleFound) {
			if (showDebugPrint) {
				print ("vehicles found can't be reached, cancelling search");
			}

			stopUpdateCoroutine ();
		
			if (useEventOnNoVehicleToPickFromScene) {
				eventOnNoVehicleToPickFromScene.Invoke ();
			}
		}
	}

	public override void updateAI ()
	{
		if (!behaviorEnabled) {
			return;
		}


	}

	public override void updateAIBehaviorState ()
	{
		if (!behaviorEnabled) {
			return;
		}


	}

	public override void updateAIAttackState (bool canUseAttack)
	{
		if (!behaviorEnabled) {
			return;
		}


	}

	public override void setSystemActiveState (bool state)
	{
		if (!behaviorEnabled) {
			return;
		}


	}

	public override void setWaitToActivateAttackActiveState (bool state)
	{

	}
}

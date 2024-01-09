using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vacuumGun : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool vacuumEnabled = true;

	public LayerMask layerToDetect;

	public bool ignoreObjectsTag;

	public List<string> tagsToIgnore = new List<string> ();

	public string layerToSetOnObjectsFound = "Ignore Raycast";

	public float maxDistanceGrab;

	public bool storeObjectsOnInventory;

	public float minDistanceToStoreObjects;

	public float vacuumSpeed = 10;

	public float holdDistance = 3;

	public bool destroyAllObjectsOnReachPosition = true;

	[Space]
	[Header ("Expel Object Settings")]
	[Space]

	public bool expelEnabled;

	public Transform expelTransformPosition;

	public float expelRate = 0.5f;

	public float expelForce = 20;

	public bool expelObjectsOneByOne;

	[Space]
	[Header ("Ignore Expel Object Settings")]
	[Space]

	public bool ignoreInventoryObjectListToExpel;

	public List<string> ignoreInventoryObjectListToExpelList = new List<string> ();

	public bool ignoreInventoryObjectCategoryListToExpel;

	public List<string> ignoreInventoryObjectCategoryListToExpelList = new List<string> ();

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool useRotor = true;
	public Transform rotor;
	public float rotorRotationSpeed = 10;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool expelActive;

	public bool vacuumActive;

	public List<Rigidbody> rigidbodyList = new List<Rigidbody> ();

	public List<rigidbodyInfo> rigidbodyInfoList = new List<rigidbodyInfo> ();

	[Space]
	[Header ("Remote Events Settings")]
	[Space]

	public bool useRemoteEvents;
	public List<string> removeEventNameList = new List<string> ();

	[Space]
	[Header ("Components")]
	[Space]

	public inventoryManager mainInventoryManager;

	public playerController playerControllerManager;
	public playerCamera playerCameraManager;

	public playerWeaponSystem mainPlayerWeaponSystem;

	public Transform mainCameraTransform;


	Coroutine updateCoroutine;

	bool componentsInitialized;

	RaycastHit hit;

	GameObject currentObjectToGrabFound;

	Vector3 nextObjectHeldPosition;

	Vector3 currentObjectHeldPosition;

	Transform currentHoldTransform;

	float orignalHoldDistance;

	float lastTimeExpel;

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
		if (vacuumActive) {
			if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, maxDistanceGrab, layerToDetect)) {
				if (currentObjectToGrabFound != hit.collider.gameObject) {
					currentObjectToGrabFound = hit.collider.gameObject;

					bool checkObjectResult = true;

					if (ignoreObjectsTag) {
						if (tagsToIgnore.Contains (currentObjectToGrabFound.tag)) {
							checkObjectResult = false;
						}
					}

					if (!checkObjectResult) {
						return;
					}

					Rigidbody newRigidbody = currentObjectToGrabFound.GetComponent<Rigidbody> ();

					if (newRigidbody != null) {
						if (!rigidbodyList.Contains (newRigidbody)) {
							rigidbodyList.Add (newRigidbody);

							rigidbodyInfo newRigidbodyInfo = new rigidbodyInfo ();

							newRigidbodyInfo.mainGameObject = currentObjectToGrabFound;
							newRigidbodyInfo.objectLayer = currentObjectToGrabFound.layer;

							rigidbodyInfoList.Add (newRigidbodyInfo);

							newRigidbody.gameObject.layer = LayerMask.NameToLayer (layerToSetOnObjectsFound);
						}
					}

					if (useRemoteEvents) {
						remoteEventSystem currentRemoteEventSystem = currentObjectToGrabFound.GetComponent<remoteEventSystem> ();

						if (currentRemoteEventSystem != null) {
							for (int i = 0; i < removeEventNameList.Count; i++) {

								currentRemoteEventSystem.callRemoteEvent (removeEventNameList [i]);
							}
						}
					}
				}
			}

			if (rigidbodyList.Count > 0) {
				currentHoldTransform = mainCameraTransform;

				if (playerCameraManager.is2_5ViewActive ()) {
					currentHoldTransform = playerCameraManager.getCurrentLookDirection2_5d ();

					holdDistance = 0;
				}

				if (playerCameraManager.useTopDownView) {
					currentHoldTransform = playerCameraManager.getCurrentLookDirectionTopDown ();

					holdDistance = 0;
				}

				for (int i = 0; i < rigidbodyList.Count; i++) {
					Rigidbody currentRigidbody = rigidbodyList [i];

					if (currentRigidbody != null) {
						nextObjectHeldPosition = currentHoldTransform.position + mainCameraTransform.forward * holdDistance;
						
						currentObjectHeldPosition = currentRigidbody.position;
			
						currentRigidbody.velocity = (nextObjectHeldPosition - currentObjectHeldPosition) * vacuumSpeed;

						if (storeObjectsOnInventory) {
							if (GKC_Utils.distance (mainCameraTransform.position, currentRigidbody.position) < minDistanceToStoreObjects) {
								pickUpObject currentPickupObject = currentRigidbody.GetComponent<pickUpObject> ();

								if (currentPickupObject != null) {

									string inventoryObjectName = currentPickupObject.getPickupObjectName ();

//									print ("checking " + inventoryObjectName);

									if (inventoryObjectName != "") {
										int inventoryObjectAmount = (int)currentPickupObject.getAmountPicked ();

//										print ("checking " + inventoryObjectAmount);

										removeRigidbodyFromList (currentRigidbody);

										if (applyDamage.giveInventoryObjectToCharacter (playerControllerManager.gameObject, inventoryObjectName, 
											    inventoryObjectAmount, null, 
											    0, 0,
											    ForceMode.Impulse, 0, false)) {

											Destroy (currentRigidbody.gameObject);
										}

										checkNullObjects ();

										return;
									} else {
										if (destroyAllObjectsOnReachPosition) {
											Destroy (currentRigidbody.gameObject);

											checkNullObjects ();

											return;
										} else {
											removeRigidbodyFromList (currentRigidbody);

											checkNullObjects ();

											return;
										}
									}
								} else {
									if (destroyAllObjectsOnReachPosition) {
										Destroy (currentRigidbody.gameObject);

										checkNullObjects ();

										return;
									} else {
										removeRigidbodyFromList (currentRigidbody);

										checkNullObjects ();

										return;
									}
								}
							}
						} else {
							if (destroyAllObjectsOnReachPosition) {
								Destroy (currentRigidbody.gameObject);

								checkNullObjects ();

								return;
							} else {
								removeRigidbodyFromList (currentRigidbody);

								checkNullObjects ();

								return;
							}
						}
					} else {
						removeRigidbodyFromList (currentRigidbody);

						checkNullObjects ();

						return;
					}
				}
			}
		}

		if (expelActive) {
			if (Time.time > lastTimeExpel + expelRate) {
				if (!mainInventoryManager.isInventoryEmpty ()) {
					inventoryInfo currentInventoryInfo	= mainInventoryManager.getRandomInventoryInfo ();

					if (currentInventoryInfo != null) {
						bool canExpelObjectResult = true;

						if (!currentInventoryInfo.canBeDropped) {
							canExpelObjectResult = false;
						}

						if (canExpelObjectResult) {
							if (ignoreInventoryObjectListToExpel) {
								if (ignoreInventoryObjectListToExpelList.Contains (currentInventoryInfo.Name)) {
									canExpelObjectResult = false;
								}
							}

							if (canExpelObjectResult) {
								if (ignoreInventoryObjectCategoryListToExpel) {
									if (ignoreInventoryObjectCategoryListToExpelList.Contains (currentInventoryInfo.categoryName)) {
										canExpelObjectResult = false;
									}
								}
							}
						}

						if (canExpelObjectResult) {
							int amountToDrop = 0;

							if (expelObjectsOneByOne) {
								amountToDrop = 1;
							} else {
								amountToDrop = currentInventoryInfo.amount;
							}

							mainInventoryManager.dropObjectByName (currentInventoryInfo.Name, amountToDrop, true, false, false);

							GameObject lastObjectDropped = mainInventoryManager.getLastObjectDropped ();

							if (lastObjectDropped != null && previousObjectDropped != lastObjectDropped) {
								lastObjectDropped.transform.position = expelTransformPosition.position;

								Rigidbody currentObjectRigidbody = lastObjectDropped.GetComponent<Rigidbody> ();

								if (currentObjectRigidbody != null) {
									currentObjectRigidbody.AddForce (expelTransformPosition.forward * expelForce, ForceMode.Impulse);
								}

								previousObjectDropped = lastObjectDropped;
							}

							lastTimeExpel = Time.time;
						}
					}
				} else {
					enableOrDisableExpel (false);
				}
			}
		}

		if (useRotor) {
			rotor.Rotate (0, 0, Time.deltaTime * rotorRotationSpeed);
		}
	}

	GameObject previousObjectDropped;


	void checkNullObjects ()
	{
		for (int i = rigidbodyList.Count - 1; i >= 0; i--) {
			if (rigidbodyList [i] == null) {
				rigidbodyList.RemoveAt (i);
			}
		}
	}

	void removeRigidbodyFromList (Rigidbody currentRigidbody)
	{
		if (currentRigidbody == null) {
			return;
		}

		if (rigidbodyList.Contains (currentRigidbody)) {
			rigidbodyList.Remove (currentRigidbody);
		
			int currentIndex = rigidbodyInfoList.FindIndex (s => s.mainGameObject == currentRigidbody.gameObject);

			if (currentIndex > -1) {
				rigidbodyInfo newRigidbodyInfo = rigidbodyInfoList [currentIndex];

				if (newRigidbodyInfo.mainGameObject != null) {
					newRigidbodyInfo.mainGameObject.layer = newRigidbodyInfo.objectLayer;
				}

				rigidbodyInfoList.RemoveAt (currentIndex);
			}
		}
	}

	public void setVacuumEnabledState (bool state)
	{
		if (!state) {
			if (vacuumActive) {
				enableOrDisableVacuum (false);
			}
		}

		vacuumEnabled = state;
	}

	public void enableVacuum ()
	{
		enableOrDisableVacuum (true);
	}

	public void disableVacuum ()
	{
		enableOrDisableVacuum (false);
	}

	public void enableOrDisableVacuum (bool state)
	{
		if (!vacuumEnabled) {
			return;
		}

		vacuumActive = state;

		mainInventoryManager.setEquipWeaponsWhenPickedPausedState (state);

		if (state) {
			initializeComponents ();

			holdDistance = orignalHoldDistance;

			updateCoroutine = StartCoroutine (updateSystemCoroutine ());

			expelActive = false;
		} else {
			stopUpdateCoroutine ();

			currentObjectToGrabFound = null;

			rigidbodyList.Clear ();

			for (int i = 0; i < rigidbodyInfoList.Count; i++) {
				rigidbodyInfo newRigidbodyInfo = rigidbodyInfoList [i];

				if (newRigidbodyInfo.mainGameObject != null) {

					newRigidbodyInfo.mainGameObject.layer = newRigidbodyInfo.objectLayer;
				}
			}

			rigidbodyInfoList.Clear ();
		}
	}

	public void setExpelEnabledState (bool state)
	{
		if (!state) {
			if (expelActive) {
				enableOrDisableExpel (false);
			}
		}

		expelEnabled = state;
	}

	public void enableOrDisableExpel (bool state)
	{
		if (!expelEnabled) {
			return;
		}

		expelActive = state;

		previousObjectDropped = null;

		lastTimeExpel = Time.time;

		if (state) {
			initializeComponents ();

			updateCoroutine = StartCoroutine (updateSystemCoroutine ());

			if (vacuumActive) {
				mainInventoryManager.setEquipWeaponsWhenPickedPausedState (false);

				vacuumActive = false;
			}
		} else {
			stopUpdateCoroutine ();


		}
	}


	void initializeComponents ()
	{
		if (componentsInitialized) {
			return;
		}

		if (mainPlayerWeaponSystem != null) {
			GameObject playerControllerGameObject = mainPlayerWeaponSystem.getPlayerWeaponsManger ().gameObject;

			playerComponentsManager mainPlayerComponentsManager = playerControllerGameObject.GetComponent<playerComponentsManager> ();

			if (mainPlayerComponentsManager != null) {
				playerControllerManager = mainPlayerComponentsManager.getPlayerController ();

				playerCameraManager = mainPlayerComponentsManager.getPlayerCamera ();

				mainCameraTransform = playerCameraManager.getCameraTransform ();
			}
		}

		orignalHoldDistance = holdDistance;

		componentsInitialized = true;
	}

	[System.Serializable]
	public class rigidbodyInfo
	{
		public GameObject mainGameObject;
		public int objectLayer;
	}
}
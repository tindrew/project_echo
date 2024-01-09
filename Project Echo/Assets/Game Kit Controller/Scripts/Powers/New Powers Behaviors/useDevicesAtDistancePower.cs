using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class useDevicesAtDistancePower : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool powerEnabled;

	public LayerMask layer;

	public List<string> tagToCheck = new List<string> ();

	public bool useInfiniteRaycastDistance = true;

	public float raycastDistance = 100;

	[Space]
	[Header ("Components")]
	[Space]

	public usingDevicesSystem usingDevicesManager;
	public Transform mainCameraTransform;

	public playerInputManager playerInput;
	public Collider playerCollider;


	public GameObject player;

	RaycastHit hit;

	GameObject currentDeviceToUse;


	public void activatePower ()
	{
		if (!powerEnabled) {
			return;
		}

		float currentRaycastDistance = raycastDistance;

		if (useInfiniteRaycastDistance) {
			currentRaycastDistance = Mathf.Infinity;
		}

		if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, currentRaycastDistance, layer)) {
			if (hit.collider.isTrigger && tagToCheck.Contains (hit.collider.tag)) {
				currentDeviceToUse = hit.collider.gameObject;

				electronicDevice currentElectronicDevice = currentDeviceToUse.GetComponent<electronicDevice> ();

				if (currentElectronicDevice != null) {
					currentElectronicDevice.checkTriggerInfo (playerCollider, true);
					usingDevicesManager.useCurrentDevice (currentDeviceToUse);
					usingDevicesManager.setObjectToRemoveAferStopUse (currentDeviceToUse);
				}

				simpleSwitch currentSimpleSwitch = currentDeviceToUse.GetComponent<simpleSwitch> ();

				if (currentSimpleSwitch != null) {
					usingDevicesManager.useCurrentDevice (currentDeviceToUse);
					usingDevicesManager.setObjectToRemoveAferStopUse (currentDeviceToUse);
				}

				inventoryObject currentInventoryObject = currentDeviceToUse.GetComponent<inventoryObject> ();

				if (currentInventoryObject != null) {
					pickUpObject currentPickupObject = currentDeviceToUse.GetComponentInParent<pickUpObject> ();

					if (currentPickupObject != null) {
						currentPickupObject.checkTriggerInfo (playerCollider);
						usingDevicesManager.useCurrentDevice (currentDeviceToUse);
						usingDevicesManager.setObjectToRemoveAferStopUse (currentDeviceToUse);
					}
				}
			}
		}
	}
}

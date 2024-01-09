using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class vehicleInteractionPanelSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool vehicleInteractionEnabled = true;

	public string enterVehicleAsDriverInputName = "Enter Vehicle As Driver";
	public string enterVehicleAsPassengerInputName = "Enter Vehicle As Passenger";

	[Space]
	[Header ("Debug")]
	[Space]

	public bool vehicleDetected;

	public bool isVehicleFull;

	public bool isVehicleBeingDriven;

	public bool arePassengerSeatsFull;

	public int numberOfVehicleSeats;

	public GameObject currentDeviceDetected;
	public GameObject previousDeviceDetected;

	public GameObject currentVehicleGameObject;

	public IKDrivingSystem currentIKDrivingSystem;

	public vehicleHUDManager currentVehicleHUDManager;

	[Space]
	[Header ("Components")]
	[Space]

	public playerController mainPlayerController;

	public usingDevicesSystem mainUsingDevicesSystem;

	public playerInputManager mainPlayerInputManager;

	[Space]
	[Header ("UI Components")]
	[Space]

	public List<GameObject> vehicleSeatsAvailableIconList = new List<GameObject> ();
	public GameObject vehicleDriverIconPanel;
	public GameObject vehiclePassengerIconPanel;
	public Text vehicleDriverText;
	public Text vehiclePassengerText;
	public GameObject vehicleFullPanel;


	Coroutine updateCoroutine;


	void Start ()
	{
		if (vehicleInteractionEnabled) {
			updateCoroutine = StartCoroutine (updateSystemCoroutine ());
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
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			updateSystem ();

			yield return waitTime;
		}
	}

	void updateSystem ()
	{
		if (mainPlayerController.isPlayerDriving ()) {
			return;
		}

		if (mainUsingDevicesSystem.anyDeviceDetected ()) {

			currentDeviceDetected = mainUsingDevicesSystem.objectToUse;

			if (currentDeviceDetected != previousDeviceDetected) {
				previousDeviceDetected = currentDeviceDetected;

				setCurrentVehicleDetected (currentDeviceDetected);

			}
		} else {
			if (vehicleDetected) {

				setCurrentVehicleDetected (null);

				currentDeviceDetected = null;

				previousDeviceDetected = null;
			}
		}

	}

	public void setCurrentVehicleDetected (GameObject newVehicle)
	{
		if (!vehicleInteractionEnabled) {
			return;
		}

		currentVehicleGameObject = newVehicle;

		vehicleDetected = false;

		mainUsingDevicesSystem.setUseDeviceButtonEnabledState (true);

		if (currentVehicleGameObject != null) {
			if (applyDamage.isVehicle (currentVehicleGameObject)) {
				vehicleDetected = true;

				currentVehicleHUDManager = currentVehicleGameObject.GetComponent<vehicleHUDManager> ();

				if (currentVehicleHUDManager == null) {
					return;
				}

				currentIKDrivingSystem = currentVehicleHUDManager.getIKDrivingSystem ();

				numberOfVehicleSeats = currentIKDrivingSystem.getVehicleSeatsAmount ();

				for (int i = 0; i < vehicleSeatsAvailableIconList.Count; i++) {
					if (i < (numberOfVehicleSeats - 1)) {
						vehicleSeatsAvailableIconList [i].SetActive (true);
					} else {
						vehicleSeatsAvailableIconList [i].SetActive (false);
					}
				}

				isVehicleBeingDriven = currentIKDrivingSystem.isVehicleBeingDriven ();

				arePassengerSeatsFull = currentIKDrivingSystem.arePassengerSeatsFull ();

				if (vehiclePassengerIconPanel.activeSelf != !arePassengerSeatsFull) {
					vehiclePassengerIconPanel.SetActive (!arePassengerSeatsFull);
				}

				if (vehicleDriverIconPanel.activeSelf != !isVehicleBeingDriven) {
					vehicleDriverIconPanel.SetActive (!isVehicleBeingDriven);
				}

				if (!isVehicleBeingDriven) {
					string driverButtonKeyValue = mainPlayerInputManager.getButtonKey (enterVehicleAsDriverInputName);

					if (driverButtonKeyValue != "") {
						vehicleDriverText.text = driverButtonKeyValue;
					}
				}

				string passengerButtonKeyValue = mainPlayerInputManager.getButtonKey (enterVehicleAsPassengerInputName);

				if (passengerButtonKeyValue != "") {
					vehiclePassengerText.text = passengerButtonKeyValue;
				}

				isVehicleFull = currentIKDrivingSystem.isVehicleFull ();

				if (vehicleFullPanel.activeSelf != !isVehicleFull) {
					vehicleFullPanel.SetActive (!isVehicleFull);
				}

				mainUsingDevicesSystem.setUseDeviceButtonEnabledState (false);
			} else {

			}
		} else {

		}
	}

	public void inputEnterOnVehicleAsDriver ()
	{
		if (!vehicleInteractionEnabled) {
			return;
		}

		if (isVehicleFull) {
			return;
		}

		if (isVehicleBeingDriven) {
			return;
		}

		if (mainPlayerController.isPlayerDriving ()) {
			return;
		}

		if (vehicleDetected) {
			currentIKDrivingSystem.setDriverExternally (mainPlayerController.gameObject);

			mainUsingDevicesSystem.setUseDeviceButtonEnabledState (true);

			vehicleDetected = false;

			currentDeviceDetected = null;
			previousDeviceDetected = null;
		}
	}

	public void inputEnterOnVehicleAsPassenger ()
	{
		if (!vehicleInteractionEnabled) {
			return;
		}

		if (isVehicleFull) {
			return;
		}

		if (arePassengerSeatsFull) {
			return;
		}

		if (mainPlayerController.isPlayerDriving ()) {
			GameObject currentVehicle = mainPlayerController.getCurrentVehicle ();

			if (currentVehicle != null) {
				mainUsingDevicesSystem.clearDeviceList ();

				mainUsingDevicesSystem.addDeviceToList (currentVehicle);

				mainUsingDevicesSystem.updateClosestDeviceList ();

				mainUsingDevicesSystem.setCurrentVehicle (currentVehicle);

				mainUsingDevicesSystem.useCurrentDevice (currentVehicle);
			}

			return;
		}

		if (vehicleDetected) {
			currentIKDrivingSystem.setPassengerExternally (mainPlayerController.gameObject);

			mainUsingDevicesSystem.setUseDeviceButtonEnabledState (true);

			vehicleDetected = false;

			currentDeviceDetected = null;
			previousDeviceDetected = null;
		}
	}
}
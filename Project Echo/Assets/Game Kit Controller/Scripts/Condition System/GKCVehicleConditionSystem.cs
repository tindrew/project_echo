using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GKCVehicleConditionSystem : GKCConditionInfo
{
	[Header ("Custom Settings")]
	[Space]

	public bool checkMaxSpeedToGetOn;
	public float maxSpeedToGetOn;

	public bool checkMaxSpeedToGetOff;
	public float maxSpeedToGetOff;

	public Rigidbody mainVehicleRigidbody;


	public override bool checkIfConditionCompleteAndReturnResult ()
	{
		if (!conditionCheckEnabled) {
			return false;
		}

		if (checkConditionPaused) {
			return false;
		}

		bool conditionResult = checkConditionResult ();

		setConditionResult (conditionResult);

		return conditionResult;
	}
		
	public override void checkIfConditionComplete ()
	{
		if (!conditionCheckEnabled) {
			return;
		}

		if (checkConditionPaused) {
			return;
		}

		bool conditionResult = checkConditionResult ();

		setConditionResult (conditionResult);
	}

	bool checkConditionResult ()
	{
		bool conditionResult = false;

		bool currentConditionState = true;

		if (checkMaxSpeedToGetOn) {
			if (mainVehicleRigidbody != null) {
				if (Mathf.Abs (mainVehicleRigidbody.velocity.magnitude) > maxSpeedToGetOn) {
					currentConditionState = false;
				}
			}
		}

		if (checkMaxSpeedToGetOff) {
			if (mainVehicleRigidbody != null) {
				if (Mathf.Abs (mainVehicleRigidbody.velocity.magnitude) > maxSpeedToGetOff) {
					currentConditionState = false;
				}
			}
		}

		conditionResult = currentConditionState;

		return conditionResult;
	}
}

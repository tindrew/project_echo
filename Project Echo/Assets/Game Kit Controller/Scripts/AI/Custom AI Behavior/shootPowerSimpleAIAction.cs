using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shootPowerSimpleAIAction : simpleAIAction
{
	[Header ("Custom Settings")]
	[Space]

	public float shootPowerDuration = 2;

	public bool useAmountOfProjectilesToShoot;

	public int amountOfProjectilesToShoot;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool aimingPower;
	public bool shootingPower;

	[Space]
	[Header ("Components")]
	[Space]

	public otherPowers mainOtherPowers;
	public findObjectivesSystem mainFindObjectivesSystem;
	public playerCamera mainPlayerCamera;

	float lastTimeAiming;


	public override void startAIAction ()
	{
		base.startAIAction ();

		if (actionActive) {
			if (actionUsedByPlayer) {

			} else {
				mainFindObjectivesSystem.setPossibleTargetForTurnBasedCombatSystemAsCustomTargetToLook ();
			}

			mainPlayerCamera.pauseOrPlayCamera (true);

			mainPlayerCamera.changeCameraRotationState (true);

		
			if (useAmountOfProjectilesToShoot) {
				mainOtherPowers.resetLastAmountOfFiredProjectiles ();
			}

			if (!mainOtherPowers.isAimingPower ()) {
				mainOtherPowers.inputSetAimPowerState (true);

				if (showDebugPrint) {
					print ("aim power");
				}
			}

			if (actionUsedByPlayer) {
				mainPlayerCamera.lookAtTemporalObjectToLookAtTarget ();
			}

			lastTimeAiming = Time.time;

			aimingPower = true;
		}
	}


	public override void updateSystem ()
	{
		if (!actionActive) {
			return;
		}

		if (aimingPower) {
			if (shootingPower) {
				if (useAmountOfProjectilesToShoot) {
					if (mainOtherPowers.getLastAmountOfFiredProjectiles () >= amountOfProjectilesToShoot) {
						endAIAction ();

						return;
					}
				} else {
					if (Time.time > lastTimeAiming + shootPowerDuration) {
						endAIAction ();

						return;
					}
				}
			} 

			setShootWeaponState (true);
		} 

		if (!actionUsedByPlayer) {
			mainFindObjectivesSystem.lookAtCurrentPlaceToShoot ();

			mainFindObjectivesSystem.AINavMeshManager.lookAtTaget (true);

			mainFindObjectivesSystem.AINavMeshManager.moveNavMesh (Vector3.zero, false, false);
		}
	}

	public void setShootWeaponState (bool state)
	{
		mainOtherPowers.inputHoldOrReleaseShootPower (true);
		mainOtherPowers.inputHoldShootPower ();
		mainOtherPowers.inputHoldOrReleaseShootPower (false);

		shootingPower = state;
	}

	public override void resetStatesOnActionEnd ()
	{
		if (shootingPower) {
			shootingPower = false;

			mainOtherPowers.inputHoldOrReleaseShootPower (false);
		}

		if (aimingPower) {
			mainOtherPowers.inputSetAimPowerState (false);
		}

		shootingPower = false;

		aimingPower = false;

		mainPlayerCamera.pauseOrPlayCamera (false);

		mainPlayerCamera.changeCameraRotationState (false);

		if (actionUsedByPlayer) {
			mainPlayerCamera.setLookAtTargetStateInput (false);
		} else {
			mainFindObjectivesSystem.AINavMeshManager.lookAtTaget (false);

			mainFindObjectivesSystem.AINavMeshManager.moveNavMesh (Vector3.zero, false, false);
		}
	}
}

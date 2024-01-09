using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shootFireWeaponSimpleAIAction : simpleAIAction
{
	[Header ("Custom Settings")]
	[Space]

	public float shootWeaponDuration = 2;

	public bool useAmountOfProjectilesToShoot;

	public int amountOfProjectilesToShoot;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool aimingWeapon;
	public bool shootingWeapon;

	[Space]
	[Header ("Components")]
	[Space]

	public playerWeaponsManager mainPlayerWeaponsManager;
	public findObjectivesSystem mainFindObjectivesSystem;
	public playerCamera mainPlayerCamera;

	float lastTimeShooting;


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
				mainPlayerWeaponsManager.resetLastAmountOfFiredProjectiles ();
			} 

			if (actionUsedByPlayer) {
				mainPlayerWeaponsManager.inputAimWeapon ();
			} else {
				mainPlayerWeaponsManager.aimCurrentWeaponWhenItIsReady (true);
			}

			if (actionUsedByPlayer) {
				mainPlayerCamera.lookAtTemporalObjectToLookAtTarget ();
			}

			aimingWeapon = true;
		}
	}

	public override void updateSystem ()
	{
		if (!actionActive) {
			return;
		}

		if (aimingWeapon) {
			if (shootingWeapon) {
				if (useAmountOfProjectilesToShoot) {
					if (mainPlayerWeaponsManager.getLastAmountOfFiredProjectiles () >= amountOfProjectilesToShoot) {
						endAIAction ();

						return;
					}
				} else {
					if (Time.time > lastTimeShooting + shootWeaponDuration) {
						endAIAction ();

						return;
					}
				}
			} else {
				if (!mainPlayerWeaponsManager.currentWeaponIsMoving () &&
				    mainPlayerWeaponsManager.reloadingActionNotActive () &&
				    mainPlayerWeaponsManager.isPlayerCarringWeapon () &&

				    !mainPlayerWeaponsManager.isActionActiveInPlayer () &&
				    mainPlayerWeaponsManager.canPlayerMove ()) {

					setShootWeaponState (true);
				}
			}
		}

		if (!actionUsedByPlayer) {
			mainFindObjectivesSystem.lookAtCurrentPlaceToShoot ();

			mainFindObjectivesSystem.AINavMeshManager.lookAtTaget (true);

			mainFindObjectivesSystem.AINavMeshManager.moveNavMesh (Vector3.zero, false, false);
		}
	}

	public void setShootWeaponState (bool state)
	{
		if (actionUsedByPlayer) {
			mainPlayerWeaponsManager.inputHoldOrReleaseShootWeapon (true);
		} else {
			mainPlayerWeaponsManager.shootWeapon (state);
		}

		shootingWeapon = state;

		if (shootingWeapon) {
			lastTimeShooting = Time.time;
		}
	}

	public override void resetStatesOnActionEnd ()
	{
		if (shootingWeapon) {
			shootingWeapon = false;

			mainPlayerWeaponsManager.inputHoldOrReleaseShootWeapon (false);

			if (mainPlayerWeaponsManager.isCharacterShooting ()) {
				mainPlayerWeaponsManager.resetWeaponFiringAndAimingIfPlayerDisabled ();

				mainPlayerWeaponsManager.setHoldShootWeaponState (false);
			}
		}

		if (aimingWeapon) {
			if (actionUsedByPlayer) {
				mainPlayerWeaponsManager.inputAimWeapon ();

				print (mainPlayerWeaponsManager.isAimingWeapons ());

				if (mainPlayerWeaponsManager.isAimingWeapons ()) {
					mainPlayerWeaponsManager.setAimWeaponState (false);
				}

				print (mainPlayerWeaponsManager.isAimingWeapons ());
			} else {
				mainPlayerWeaponsManager.aimCurrentWeaponWhenItIsReady (false);
				mainPlayerWeaponsManager.stopAimCurrentWeaponWhenItIsReady (true);
			}
		}

		shootingWeapon = false;

		aimingWeapon = false;

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

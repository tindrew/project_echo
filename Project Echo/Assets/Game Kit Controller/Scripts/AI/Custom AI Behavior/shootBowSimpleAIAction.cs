using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shootBowSimpleAIAction : simpleAIAction
{
	[Header ("Custom Settings")]
	[Space]

	public float shootWeaponDuration = 2;

	public bool useAmountOfProjectilesToShoot;

	public int amountOfProjectilesToShoot;

	public bool useFireRate;
	public float fireRate;

	//	public string quickModeActionName = "Fire Arrow Bow Attack";

	[Space]
	[Header ("Debug")]
	[Space]

	public bool aimingWeapon;
	public bool shootingWeapon;

	[Space]
	[Header ("Components")]
	[Space]

	//	public quickPlayerModeChangeSystem mainQuickPlayerModeChangeSystem;
	public bowSystem mainBowSystem;
	public findObjectivesSystem mainFindObjectivesSystem;
	public playerCamera mainPlayerCamera;

	float lastTimeAiming;
	float lastTimeShoot;


	public override void startAIAction ()
	{
		base.startAIAction ();

		if (actionActive) {
			if (actionUsedByPlayer) {

			} else {
				mainFindObjectivesSystem.setPossibleTargetForTurnBasedCombatSystemAsCustomTargetToLook ();

				mainFindObjectivesSystem.setIgnoreLookAtDirectionActiveState (true);
			}

			mainPlayerCamera.pauseOrPlayCamera (true);

			mainPlayerCamera.changeCameraRotationState (true);


			if (useAmountOfProjectilesToShoot) {
				mainBowSystem.resetLastAmountOfFiredProjectiles ();
			}

			mainBowSystem.setArrowsManagedByInventoryState (false);

			mainBowSystem.inputSetAimBowState (true);

			if (showDebugPrint) {
				print ("aim bow");
			}

			if (actionUsedByPlayer) {
				mainPlayerCamera.lookAtTemporalObjectToLookAtTarget ();
			}

			lastTimeAiming = Time.time;

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
					if (mainBowSystem.getLastAmountOfFiredProjectiles () >= amountOfProjectilesToShoot) {
						endAIAction ();

						return;
					}
				} else {
					if (Time.time > lastTimeAiming + shootWeaponDuration) {
						endAIAction ();

						return;
					}
				}
			}

			if (useFireRate) {
				if (Time.time > lastTimeShoot + fireRate) {
					setShootWeaponState (true);

					lastTimeShoot = Time.time;
				}
			} else {
				setShootWeaponState (true);
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
//		mainQuickPlayerModeChangeSystem.inputActivateMode (quickModeActionName);

		mainBowSystem.inputFireArrow ();

		shootingWeapon = state;
	}

	public override void resetStatesOnActionEnd ()
	{
		if (shootingWeapon) {
			shootingWeapon = false;

//			mainQuickPlayerModeChangeSystem.inputActiveModePressUp (quickModeActionName);

			mainBowSystem.inputSetAimBowState (false);

			if (mainBowSystem.isAimingBowActive ()) {
				mainBowSystem.cancelBowLoadedStateIfActive ();
			}
		}

		mainBowSystem.setOriginalArrowsManagedByInventoryState ();
			
		shootingWeapon = false;

		aimingWeapon = false;

		mainPlayerCamera.pauseOrPlayCamera (false);

		mainPlayerCamera.changeCameraRotationState (false);

		if (actionUsedByPlayer) {
			mainPlayerCamera.setLookAtTargetStateInput (false);
		} else {
			mainFindObjectivesSystem.AINavMeshManager.lookAtTaget (false);

			mainFindObjectivesSystem.AINavMeshManager.moveNavMesh (Vector3.zero, false, false);

			mainFindObjectivesSystem.setIgnoreLookAtDirectionActiveState (false);
		}
	}
}

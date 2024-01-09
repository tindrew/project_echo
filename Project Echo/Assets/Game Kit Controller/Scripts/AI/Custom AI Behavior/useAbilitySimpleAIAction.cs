using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class useAbilitySimpleAIAction : simpleAIAction
{
	[Header ("Custom Settings")]
	[Space]

	public string currentAbilityName;

	public float minWaitTimeToFinishAbility = 0.4f;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool abilityInProcess;

	[Space]
	[Header ("Components")]
	[Space]

	public AIAbilitiesSystemBrain mainAIAbilitiesSystemBrain;
	public findObjectivesSystem mainFindObjectivesSystem;
	public playerCamera mainPlayerCamera;

	float lastTimeAbilityUsed;


	public void setCurrentAbilityName (string abilityName)
	{
		currentAbilityName = abilityName;
	}

	public void setCurrentAbilityNameAndStartAIAction (string abilityName)
	{
		setCurrentAbilityName (abilityName);

		startAIAction ();
	}

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

		
			if (actionUsedByPlayer) {
				mainPlayerCamera.lookAtTemporalObjectToLookAtTarget ();
			}

			lastTimeAbilityUsed = Time.time;

			mainAIAbilitiesSystemBrain.setAndActivateAbilityByName (currentAbilityName);
		}
	}

	public override void updateSystem ()
	{
		if (!actionActive) {
			return;
		}


		if (!mainAIAbilitiesSystemBrain.isAbilityInProcess () && Time.time > minWaitTimeToFinishAbility + lastTimeAbilityUsed) {
			endAIAction ();

			return;
		}

		if (!actionUsedByPlayer) {
			mainFindObjectivesSystem.lookAtCurrentPlaceToShoot ();

			mainFindObjectivesSystem.AINavMeshManager.lookAtTaget (true);

			mainFindObjectivesSystem.AINavMeshManager.moveNavMesh (Vector3.zero, false, false);
		}
	}

	public override void resetStatesOnActionEnd ()
	{
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

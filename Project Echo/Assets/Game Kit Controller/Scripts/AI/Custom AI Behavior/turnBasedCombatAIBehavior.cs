using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turnBasedCombatAIBehavior : AIBehaviorInfo
{
	[Header ("Custom Settings")]
	[Space]

	public AITurnBasedCombatSystemBrain mainAITurnBasedCombatSystemBrain;

	public override void updateAI ()
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAITurnBasedCombatSystemBrain.updateAI ();
	}

	public override void updateAIBehaviorState ()
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAITurnBasedCombatSystemBrain.updateBehavior ();
	}

	public override void updateAIAttackState (bool canUseAttack)
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAITurnBasedCombatSystemBrain.updateAIAttackState (canUseAttack);
	}

	public override void setSystemActiveState (bool state)
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAITurnBasedCombatSystemBrain.setSystemActiveState (state);
	}

	public override void setWaitToActivateAttackActiveState (bool state)
	{
		mainAITurnBasedCombatSystemBrain.setWaitToActivateAttackActiveState (state);
	}

	public override void setIsCurrentCharacterTurn (bool state)
	{
		mainAITurnBasedCombatSystemBrain.setIsCurrentCharacterTurn (state);
	}

	public override void setCurrentCharacterTurnTarget (GameObject newTarget)
	{
		mainAITurnBasedCombatSystemBrain.setCurrentCharacterTurnTarget (newTarget);
	}

	public override void checkStateOnCharacterDead ()
	{
		mainAITurnBasedCombatSystemBrain.checkStateOnCharacterDead ();
	}

	public override void setAIOnPlayerTeamState (bool state)
	{
		mainAITurnBasedCombatSystemBrain.setAIOnPlayerTeamState (state);
	}
}

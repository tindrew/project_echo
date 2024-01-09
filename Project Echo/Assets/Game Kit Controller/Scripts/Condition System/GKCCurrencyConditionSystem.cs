using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GKCCurrencyConditionSystem : GKCConditionInfo
{
	[Header ("Custom Settings")]
	[Space]

	public int amountToCheck;


	public override bool checkIfConditionCompleteAndReturnResult ()
	{
		if (!checkIfPlayerAssigned ()) {
			return false;
		}

		bool conditionResult = checkConditionResult ();

		setConditionResult (conditionResult);

		return conditionResult;
	}

	public override void checkIfConditionComplete ()
	{
		if (!checkIfPlayerAssigned ()) {
			return;
		}

		bool conditionResult = checkConditionResult ();

		setConditionResult (conditionResult);
	}

	bool checkConditionResult ()
	{
		bool conditionResult = false;

		playerComponentsManager mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (mainPlayerComponentsManager != null) {
			currencySystem mainCurrencySystem = mainPlayerComponentsManager.getCurrencySystem ();

			if (mainCurrencySystem != null) {
				bool currentConditionState = true;

				if (mainCurrencySystem.getCurrentMoneyAmount () < amountToCheck) {
					currentConditionState = false;
				}

				conditionResult = currentConditionState;
			}
		}

		return conditionResult;
	}
}

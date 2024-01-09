using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class energyOnInventory : objectOnInventory
{
	public override void activateUseObjectActionOnInventoryWithExternalCharacter (GameObject currentPlayer, GameObject currentExternalCharacterForInventoryUsage, int amountToUse)
	{
		useObject (currentPlayer, currentExternalCharacterForInventoryUsage, amountToUse);
	}

	public override void activateUseObjectActionOnInventory (GameObject currentPlayer, int amountToUse)
	{
		useObject (currentPlayer, currentPlayer, amountToUse);
	}

	void useObject (GameObject characterInventoryOwner, GameObject characterToReceiveObjectEffect, int amountToUse)
	{
		float totalAmountToUse = mainInventoryObject.inventoryObjectInfo.amountPerUnit * amountToUse;

		float totalAmountToPick = applyDamage.getEnergyAmountToPick (characterToReceiveObjectEffect, totalAmountToUse);

		applyDamage.setEnergy (totalAmountToPick, characterToReceiveObjectEffect);

		int totalAmountUsed = (int)totalAmountToPick / mainInventoryObject.inventoryObjectInfo.amountPerUnit;

		if (totalAmountToPick % totalAmountToUse > 0) {
			totalAmountUsed += 1;
		}

		if (!useOnlyAmountNeeded) {
			totalAmountUsed = amountToUse;
		}

		if (amountToUse > 0) {
			checkExternalElementsOnUseInventoryObject (characterToReceiveObjectEffect);
		}

		inventoryManager currentInventoryManager = characterInventoryOwner.GetComponent<inventoryManager> ();

		if (currentInventoryManager != null) {
			currentInventoryManager.setUseObjectWithNewBehaviorResult (totalAmountUsed);
		}
	}
}

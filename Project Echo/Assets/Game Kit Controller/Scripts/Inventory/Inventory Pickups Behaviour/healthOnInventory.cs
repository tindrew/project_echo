using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthOnInventory : objectOnInventory
{
	public bool showDebugPrint;

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

		float totalAmountToPick = applyDamage.getHealthAmountToPick (characterToReceiveObjectEffect, totalAmountToUse);

		int totalAmountUsed = 0;

		if (!useOnlyAmountNeeded || totalAmountToPick > 0) {
			applyDamage.setHeal (totalAmountToPick, characterToReceiveObjectEffect);

			totalAmountUsed = (int)totalAmountToPick / mainInventoryObject.inventoryObjectInfo.amountPerUnit;

			if (totalAmountToPick % totalAmountToUse > 0) {
				totalAmountUsed += 1;
			}

			if (!useOnlyAmountNeeded) {
				totalAmountUsed = amountToUse;
			}

			if (showDebugPrint) {
				print ("health refilled " + totalAmountToPick);
				print ("health units used " + totalAmountUsed);
			}

			checkExternalElementsOnUseInventoryObject (characterToReceiveObjectEffect);
		}

		inventoryManager currentInventoryManager = characterInventoryOwner.GetComponent<inventoryManager> ();

		if (currentInventoryManager != null) {
			currentInventoryManager.setUseObjectWithNewBehaviorResult (totalAmountUsed);

			if (closeInventoryOnObjectUsed && totalAmountUsed > 0) {
				if (currentInventoryManager.isInventoryMenuOpened ()) {
					currentInventoryManager.openOrCloseInventory (false);
				}
			}
		}
	}
}

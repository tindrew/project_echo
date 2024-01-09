using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleInventoryContentManager : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool setInitialInventoryListEnabled;

	[Space]
	[Header ("List Settings")]
	[Space]

	public List<simpleInventoryInfo> simpleInventoryInfoList = new List<simpleInventoryInfo> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public simpleInventoryInfo currentWeaponInfo;

	public bool currentWeaponInfoAssigned;

	[Space]
	[Header ("Components")]
	[Space]

	public inventoryManager mainInventoryManager;
	public playerWeaponsManager mainPlayerWeaponsManager;


	List<inventoryListElement> inventoryListManagerList = new List<inventoryListElement> ();


	// FUNCTION TO INITIALIZE THE INVENTORY WHEN STARTING THE GAME, OVERRIDING THE DEFAULT INITIAL INVENTORY ON INVENTORY MANAGER
	void Awake ()
	{
		if (setInitialInventoryListEnabled) {
			inventoryListManagerList.Clear ();

			int simpleInventoryInfoListCount = simpleInventoryInfoList.Count;

			for (int k = 0; k < simpleInventoryInfoListCount; k++) {

				simpleInventoryInfo currentSimpleInventoryInfo = simpleInventoryInfoList [k];

				if (currentSimpleInventoryInfo.addInventoryObject) {
					inventoryInfo currentInventoryInfo = mainInventoryManager.getInventoryInfoByName (currentSimpleInventoryInfo.Name);
				
					inventoryListElement newInventoryListElement = new inventoryListElement ();

					newInventoryListElement.Name = currentInventoryInfo.Name;
					newInventoryListElement.amount = currentSimpleInventoryInfo.amount;
					newInventoryListElement.infiniteAmount = currentInventoryInfo.infiniteAmount;
					newInventoryListElement.inventoryObjectName = currentInventoryInfo.Name;

					newInventoryListElement.categoryIndex = currentInventoryInfo.categoryIndex;
					newInventoryListElement.elementIndex = currentInventoryInfo.elementIndex;

					newInventoryListElement.isEquipped = currentSimpleInventoryInfo.isEquipped;

					newInventoryListElement.quickAccessSlotIndex = currentInventoryInfo.quickAccessSlotIndex;

					newInventoryListElement.useDurability = currentInventoryInfo.useDurability;
					newInventoryListElement.durabilityAmount = currentInventoryInfo.durabilityAmount;
					newInventoryListElement.maxDurabilityAmount = currentInventoryInfo.maxDurabilityAmount;
					newInventoryListElement.objectIsBroken = currentInventoryInfo.objectIsBroken;

					newInventoryListElement.isWeapon = currentInventoryInfo.isWeapon;
					newInventoryListElement.isMeleeWeapon = currentInventoryInfo.isMeleeWeapon;

					if (newInventoryListElement.isWeapon && !newInventoryListElement.isMeleeWeapon) {
						newInventoryListElement.projectilesInMagazine = currentSimpleInventoryInfo.projectilesInMagazine;
					}

					inventoryListManagerList.Add (newInventoryListElement);
				}
			}

			if (inventoryListManagerList.Count > 0) {
				mainInventoryManager.setNewInventoryListManagerList (inventoryListManagerList);
			}
		}
	}

	//FUNCTION TO SET WHICH IS THE CURRENT WEAPON CARRIED
	public void setCurrentWeaponObject (string weaponName)
	{
		updateCurrentSimpleInventoryList ();

		int simpleInventoryInfoListCount = simpleInventoryInfoList.Count;

		for (int k = 0; k < simpleInventoryInfoListCount; k++) {
			if (simpleInventoryInfoList [k].Name.Equals (weaponName)) {
				simpleInventoryInfoList [k].isCurrentWeapon = true;

				currentWeaponInfo = simpleInventoryInfoList [k];

				currentWeaponInfoAssigned = true;
			} else {
				simpleInventoryInfoList [k].isCurrentWeapon = false;
			}
		}
	}

	//FUNCTION TO UPDATE THE REMAIN AMMO ON THE MAGAZINE WHEN SHOOTING
	public void updateCurrentWeaponRemainMagazine (int amount)
	{
		if (currentWeaponInfoAssigned) {
			currentWeaponInfo.projectilesInMagazine = amount;
		}
	}

	//FUNCTION TO UPDATE AMMO OR ANY OTHER OBJECT AMOUNT
	public void updateSimpleInventoryObjectInfo (string objectName, int amount)
	{
		int currentIndex = simpleInventoryInfoList.FindIndex (s => s.Name.Equals (objectName));

		if (currentIndex > -1) {
			simpleInventoryInfo currentElement = simpleInventoryInfoList [currentIndex];

			currentElement.amount = amount;
		}
	}

	//FUNCTION CALLLED WHEN GETTING AMMO FROM THE SCENE OR ANY OTHER MOMENT WHILE PLAYING
	public void updateCurrentSimpleInventoryList ()
	{
		simpleInventoryInfoList.Clear ();

		List<inventoryInfo> inventoryList = mainInventoryManager.getInventoryList ();

		int inventoryListCount = inventoryList.Count;

		for (int k = 0; k < inventoryListCount; k++) {
			inventoryInfo currentInventoryInfo = inventoryList [k];

			if (currentInventoryInfo.amount > 0) {
				simpleInventoryInfo newSimpleInventoryInfo = new simpleInventoryInfo ();

				newSimpleInventoryInfo.Name = currentInventoryInfo.Name;
				newSimpleInventoryInfo.amount = currentInventoryInfo.amount;

				newSimpleInventoryInfo.isEquipped = currentInventoryInfo.isEquipped;

				newSimpleInventoryInfo.addInventoryObject = true;
			
				if (currentInventoryInfo.isWeapon && !currentInventoryInfo.isMeleeWeapon) {
					newSimpleInventoryInfo.projectilesInMagazine = currentInventoryInfo.mainWeaponObjectInfo.getProjectilesInWeaponMagazine ();
				}

				simpleInventoryInfoList.Add (newSimpleInventoryInfo);
			}
		}	

		string currentWeaponName = mainPlayerWeaponsManager.getCurrentWeaponName ();

		if (currentWeaponName != "") {
			setCurrentWeaponObject (currentWeaponName);
		}
	}


	//FUNCTION CALLED WHEN FINISHING THE GAME OR LEAVING THE GAME
	public void getCurrentInventoryList ()
	{
		inventoryListManagerList.Clear ();

		List<inventoryInfo> inventoryList = mainInventoryManager.getInventoryList ();

		int inventoryListCount = inventoryList.Count;

		for (int k = 0; k < inventoryListCount; k++) {
			inventoryInfo currentInventoryInfo = inventoryList [k];

			if (currentInventoryInfo.amount > 0) {
				inventoryListElement newInventoryListElement = new inventoryListElement ();

				newInventoryListElement.Name = currentInventoryInfo.Name;
				newInventoryListElement.amount = currentInventoryInfo.amount;
				newInventoryListElement.infiniteAmount = currentInventoryInfo.infiniteAmount;
				newInventoryListElement.inventoryObjectName = currentInventoryInfo.Name;

				newInventoryListElement.categoryIndex = currentInventoryInfo.categoryIndex;
				newInventoryListElement.elementIndex = currentInventoryInfo.elementIndex;

				newInventoryListElement.isEquipped = currentInventoryInfo.isEquipped;

				newInventoryListElement.quickAccessSlotIndex = currentInventoryInfo.quickAccessSlotIndex;

				newInventoryListElement.useDurability = currentInventoryInfo.useDurability;
				newInventoryListElement.durabilityAmount = currentInventoryInfo.durabilityAmount;
				newInventoryListElement.maxDurabilityAmount = currentInventoryInfo.maxDurabilityAmount;
				newInventoryListElement.objectIsBroken = currentInventoryInfo.objectIsBroken;

				newInventoryListElement.isWeapon = currentInventoryInfo.isWeapon;
				newInventoryListElement.isMeleeWeapon = currentInventoryInfo.isMeleeWeapon;

				if (newInventoryListElement.isWeapon && !newInventoryListElement.isMeleeWeapon) {
					newInventoryListElement.projectilesInMagazine = currentInventoryInfo.mainWeaponObjectInfo.getProjectilesInWeaponMagazine ();
				}

				inventoryListManagerList.Add (newInventoryListElement);
			}
		}	
	}

	[System.Serializable]
	public class simpleInventoryInfo
	{
		public string Name;
		public int amount;

		public bool isEquipped;

		public bool addInventoryObject = true;

		public int projectilesInMagazine = -1;

		[HideInInspector] public bool isCurrentWeapon;
	}
}

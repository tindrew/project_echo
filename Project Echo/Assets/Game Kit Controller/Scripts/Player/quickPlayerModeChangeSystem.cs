using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class quickPlayerModeChangeSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool quickChangeEnabled = true;

	public float minWaitTimeToChangeMode = 0.3f;

	public bool useInventoryQuickAccessSlotsSystem = true;

	[Space]
	[Header ("Mode Settings")]
	[Space]

	public List<quickModeInfo> quickModeInfoList = new List<quickModeInfo> ();

	[Space]
	[Header ("Selection List Settings")]
	[Space]

	public bool selectionInfoEnabled;

	public bool setSelectionInfoEnableStateEnabled = true;

	public bool externalSetSelectionInfoEnabled = true;

	public bool checkCurrentSelectionInfoOnStart;

	public bool setSelectionInfoWhenEnabled = true;

	public float minWaitTimeToChangeSelection = 0.3f;

	[Space]

	public bool activateSelectionOnStart;
	public bool setCustomSelectionIndexOnStart;
	public int customSelectionIndexOnStart;

	[Space]
	[Space]

	public List<selectionInfo> selectionInfoList = new List<selectionInfo> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;
	public string currentModeName;
	public bool changeOfModeInProcess;

	public int currentSelectionInfoIndex = -1;
	public string currentSelectionInfoName;

	[Space]
	[Header ("Components")]
	[Space]

	public playerController mainPlayerController;

	public playerWeaponsManager mainPlayerWeaponsManager;

	public meleeWeaponsGrabbedManager mainMeleeWeaponsGrabbedManager;

	public bowSystem mainBowSystem;

	public inventoryQuickAccessSlotsSystem mainInventoryQuickAccessSlotsSystem;

	public playerStatesManager mainPlayerStatesManager;

	public otherPowers mainOtherPowers;
	public closeCombatSystem mainCloseCombatSystem;


	quickModeInfo currentQuickModeInfo;

	int currentModeIndex;

	Coroutine checkChangeOfModeCoroutine;

	bool changingModeToUseMeleeWeapons;
	bool changingModeToUseFireWeapons;

	bool currentlyCarryingFireWeapon;
	bool currentlyCarringMeleeWeapon;

	bool playerIsCarryingWeapons;
	bool playerIsOnWeaponModeToUse;


	selectionInfo currentSelectionInfo;

	bool selectionInfoActive;

	bool initialSelectioInfoChecked;

	Coroutine updateCoroutine;

	float lastTimeSelectionInfoChanged;


	void Start ()
	{
		if (checkCurrentSelectionInfoOnStart) {
			updateCoroutine = StartCoroutine (updateSystemCoroutine ());
		}
	}

	public void stopUpdateCoroutine ()
	{
		if (updateCoroutine != null) {
			StopCoroutine (updateCoroutine);
		}
	}

	IEnumerator updateSystemCoroutine ()
	{
		WaitForSeconds delay = new WaitForSeconds (0.5f);

		yield return delay;

		string currentMode = mainPlayerStatesManager.getCurrentPlayersModeName ();

		if (currentMode != "") {
			int newIndex = -1;

			for (int i = 0; i < selectionInfoList.Count; i++) {
				if (newIndex == -1) {
					if (selectionInfoList [i].selectionEnabled) {
						if (selectionInfoList [i].setModeToSelect) {
							if (selectionInfoList [i].setModeToSelect.Equals (currentMode)) {
								newIndex = i;

							}
						} else {
							if (selectionInfoList [i].selectionIsWeapon) {
								if (selectionInfoList [i].selectionIsMelee) {
									string weaponName = mainMeleeWeaponsGrabbedManager.getCurrentWeaponName ();

									if (weaponName != "" && weaponName.Equals (selectionInfoList [i].weaponName)) {
										newIndex = i;
									}
								} else {
									string weaponName = mainPlayerWeaponsManager.getCurrentWeaponName ();

									if (weaponName != "" && weaponName.Equals (selectionInfoList [i].weaponName)) {
										newIndex = i;
									}
								}
							}
						}
					}
				}
			}

			if (newIndex != -1) {
				currentSelectionInfoIndex = newIndex;

				currentSelectionInfo = selectionInfoList [currentSelectionInfoIndex];

				currentSelectionInfo.isCurrentSelection = true;

				currentSelectionInfoName = currentSelectionInfo.Name;

				if (showDebugPrint) {
					print ("setting initial selection info " + currentSelectionInfoName);
				}

				if (activateSelectionOnStart) {
					if (setCustomSelectionIndexOnStart) {
						currentSelectionInfoIndex = customSelectionIndexOnStart;
					}

					currentSelectionInfoIndex--;

					if (currentSelectionInfoIndex < 0) {
						currentSelectionInfoIndex = selectionInfoList.Count - 1;
					}

					currentSelectionInfoName = "";

					inputSetNextOrPreviousSelectionInfo (true);
				}
			} else {
				if (showDebugPrint) {
					print ("initial selection not found");
				}
			}
		}

		stopUpdateCoroutine ();
	}



	//SET MODE FUNCTIONS
	void useMode (bool isSelectingNewMode)
	{
		if (currentQuickModeInfo.useEventOnUseMode) {
			currentQuickModeInfo.eventOnUseMode.Invoke ();
		}

		if (currentQuickModeInfo.modeUsesPressDownAndUpInput) {
			currentQuickModeInfo.pressDownActive = true;
		}

		if (currentQuickModeInfo.useWeaponOnMode) {
			if (currentQuickModeInfo.weaponIsMelee) {
				if (isSelectingNewMode) {
					if (showDebugPrint) {
						print ("selecting new mode with a melee weapon");
					}


					changingModeToUseMeleeWeapons = false;
					changingModeToUseFireWeapons = false;

					currentlyCarryingFireWeapon = false;
					currentlyCarringMeleeWeapon = false;

					playerIsCarryingWeapons = false;
					playerIsOnWeaponModeToUse = false;

					if (mainPlayerController.isPlayerUsingWeapons ()) {
						playerIsCarryingWeapons = true;

						currentlyCarryingFireWeapon = true;

						changingModeToUseMeleeWeapons = true;
					} else {
						if (mainPlayerWeaponsManager.isWeaponsModeActive ()) {
							changingModeToUseMeleeWeapons = true;
						}
					}

					if (mainPlayerController.isPlayerUsingMeleeWeapons ()) {
						playerIsCarryingWeapons = true;

						currentlyCarringMeleeWeapon = true;

						playerIsOnWeaponModeToUse = true;
					} else {
						if (mainMeleeWeaponsGrabbedManager.isMeleeWeaponsGrabbedManagerActive ()) {
							playerIsOnWeaponModeToUse = true;
						}
					}

					if (showDebugPrint) {
						print ("changingModeToUseMeleeWeapons " + changingModeToUseMeleeWeapons);
						print ("changingModeToUseFireWeapons " + changingModeToUseFireWeapons);
						print ("currentlyCarryingFireWeapon " + currentlyCarryingFireWeapon);
						print ("currentlyCarringMeleeWeapon " + currentlyCarringMeleeWeapon);
						print ("playerIsCarryingWeapons " + playerIsCarryingWeapons);
						print ("playerIsOnWeaponModeToUse " + playerIsOnWeaponModeToUse);
					}

					mainMeleeWeaponsGrabbedManager.setIgnoreUseDrawKeepWeaponAnimationState (true);

					mainBowSystem.setIgnoreUseDrawHolsterActionAnimationsState (true);

					bool activateCoroutineResult = false;

					if (playerIsOnWeaponModeToUse) {
						mainMeleeWeaponsGrabbedManager.resetCurrentAttackByIndex ();

						if (playerIsCarryingWeapons) {
							if (!currentQuickModeInfo.useCurrentWeaponIfAlreadyCarriedOnSameMode && !currentQuickModeInfo.useFirstWeaponTypeEquippedFound) {
								if (!currentQuickModeInfo.weaponNameToUse.Equals (mainMeleeWeaponsGrabbedManager.getCurrentWeaponName ())) {
									playerIsCarryingWeapons = false;

									if (showDebugPrint) {
										print ("current mode is melee and character is carrying melee weapon, but it is not the one for this mode, " +
										"selecting proper weapon");
									}
								}
							}
						}

						if (playerIsCarryingWeapons) {
							if (showDebugPrint) {
								print ("carrying melee weapon, activating attack by index");
							}

							if (currentQuickModeInfo.weaponIsBow) {
								mainBowSystem.inputFireArrow ();
							} else {
								if (currentQuickModeInfo.modeUsedToBlock) {
									mainMeleeWeaponsGrabbedManager.activateOrDeactivateBlockGrabbedObjectMeleeeCheckingCurrentState (true);
								} else {
									mainMeleeWeaponsGrabbedManager.activateGrabbedObjectMeleeAttackByIndex ();
								}
							}
						} else {
							if (showDebugPrint) {
								print ("not carrying melee weapon, drawing melee weapon and then, activating attack by index");
							}

							selectMeleeWeaponOnQuickAccessSlosts (true);

							activateCoroutineResult = true;
						}
					} else {
						if (showDebugPrint) {
							print ("player is not on melee weapons mode");
						}

						if (changingModeToUseMeleeWeapons) {
							if (showDebugPrint) {
								print ("currently on fire weapons mode, changing mode");
							}

							if (currentlyCarryingFireWeapon) {
								if (showDebugPrint) {
									print ("currently carrying a weapon, activating quick holster");
								}

								mainPlayerWeaponsManager.setUseQuickDrawWeaponState (true);

								mainPlayerWeaponsManager.setForceWeaponToUseQuickDrawKeepWeaponActiveState (true);

								mainPlayerWeaponsManager.setIgnoreUseDrawKeepWeaponAnimationState (true);

								selectMeleeWeaponOnQuickAccessSlosts (true);
							} else {
								selectMeleeWeaponOnQuickAccessSlosts (true);
							}
						} else {
							if (showDebugPrint) {
								print ("currently not carrying a weapon, activating change of mode");
							}

							selectMeleeWeaponOnQuickAccessSlosts (true);
						}


						activateCoroutineResult = true;
					}


					if (activateCoroutineResult) {
						stopUpdateCheckChangeOfModeCoroutine ();

						checkChangeOfModeCoroutine = StartCoroutine (updateCheckChangeOfModeCoroutine (isSelectingNewMode,
							currentQuickModeInfo.weaponIsMelee, currentQuickModeInfo.useWeaponOnMode, currentQuickModeInfo.weaponIsBow));
					}
				} else {
					if (currentQuickModeInfo.weaponIsBow) {
						mainBowSystem.inputFireArrow ();
					} else {
						if (currentQuickModeInfo.modeUsedToBlock) {
							mainMeleeWeaponsGrabbedManager.activateOrDeactivateBlockGrabbedObjectMeleeeCheckingCurrentState (true);
						} else {
							mainMeleeWeaponsGrabbedManager.activateGrabbedObjectMeleeAttackByIndex ();
						}
					}
				}
			} else {
				if (isSelectingNewMode) {
					if (showDebugPrint) {
						print ("selecting new mode with a fire weapon");
					}


					changingModeToUseMeleeWeapons = false;
					changingModeToUseFireWeapons = false;

					currentlyCarryingFireWeapon = false;
					currentlyCarringMeleeWeapon = false;

					playerIsCarryingWeapons = false;
					playerIsOnWeaponModeToUse = false;

					if (mainPlayerController.isPlayerUsingWeapons ()) {
						playerIsCarryingWeapons = true;

						currentlyCarryingFireWeapon = true;

						playerIsOnWeaponModeToUse = true;
					} else {
						if (mainPlayerWeaponsManager.isWeaponsModeActive ()) {
							playerIsOnWeaponModeToUse = true;
						}
					}

					if (mainPlayerController.isPlayerUsingMeleeWeapons ()) {
						playerIsCarryingWeapons = true;

						currentlyCarringMeleeWeapon = true;

						changingModeToUseFireWeapons = true;
					} else {
						if (mainMeleeWeaponsGrabbedManager.isMeleeWeaponsGrabbedManagerActive ()) {
							changingModeToUseFireWeapons = true;
						}
					}

					if (showDebugPrint) {
						print ("changingModeToUseMeleeWeapons " + changingModeToUseMeleeWeapons);
						print ("changingModeToUseFireWeapons " + changingModeToUseFireWeapons);
						print ("currentlyCarryingFireWeapon " + currentlyCarryingFireWeapon);
						print ("currentlyCarringMeleeWeapon " + currentlyCarringMeleeWeapon);
						print ("playerIsCarryingWeapons " + playerIsCarryingWeapons);
						print ("playerIsOnWeaponModeToUse " + playerIsOnWeaponModeToUse);
					}

					mainPlayerWeaponsManager.setUseQuickDrawWeaponState (true);
					
					mainPlayerWeaponsManager.setForceWeaponToUseQuickDrawKeepWeaponActiveState (true);
					
					mainPlayerWeaponsManager.setIgnoreUseDrawKeepWeaponAnimationState (true);

					bool activateCoroutineResult = false;

					if (playerIsOnWeaponModeToUse) {

						if (playerIsCarryingWeapons) {
							if (showDebugPrint) {
								print ("carrying fire weapon, activating attack by index");
							}
								
							mainPlayerWeaponsManager.inputHoldOrReleaseShootWeapon (true);
						} else {
							if (showDebugPrint) {
								print ("not carrying fire weapon, drawing fire weapon and then");
							}

							selectFireWeaponOnQuickAccessSlots (true);

							activateCoroutineResult = true;
						}
					} else {
						if (showDebugPrint) {
							print ("player is not on fire weapons mode");
						}

						if (changingModeToUseFireWeapons) {
							if (showDebugPrint) {
								print ("currently on melee weapons mode, changing mode");
							}

							if (currentlyCarringMeleeWeapon) {
								if (showDebugPrint) {
									print ("currently carrying a melee weapon, activating quick holster");
								}

								mainMeleeWeaponsGrabbedManager.setIgnoreUseDrawKeepWeaponAnimationState (true);

								mainBowSystem.setIgnoreUseDrawHolsterActionAnimationsState (true);

								selectFireWeaponOnQuickAccessSlots (true);
							} else {
								selectFireWeaponOnQuickAccessSlots (true);
							}
						} else {
							if (showDebugPrint) {
								print ("currently not carrying a weapon, activating change of mode");
							}

							selectFireWeaponOnQuickAccessSlots (true);
						}


						activateCoroutineResult = true;
					}

					if (activateCoroutineResult) {
						stopUpdateCheckChangeOfModeCoroutine ();

						checkChangeOfModeCoroutine = StartCoroutine (updateCheckChangeOfModeCoroutine (isSelectingNewMode,
							currentQuickModeInfo.weaponIsMelee, currentQuickModeInfo.useWeaponOnMode, currentQuickModeInfo.weaponIsBow));
					}
				} else {
					mainPlayerWeaponsManager.inputHoldOrReleaseShootWeapon (true);
				}
			}
		} else {
			mainMeleeWeaponsGrabbedManager.setIgnoreUseDrawKeepWeaponAnimationState (true);

			mainBowSystem.setIgnoreUseDrawHolsterActionAnimationsState (true);

			mainPlayerWeaponsManager.setUseQuickDrawWeaponState (true);

			mainPlayerWeaponsManager.setForceWeaponToUseQuickDrawKeepWeaponActiveState (true);

			mainPlayerWeaponsManager.setIgnoreUseDrawKeepWeaponAnimationState (true);

			if (isSelectingNewMode) {
				if (currentQuickModeInfo.setModeToSelect) {
					mainPlayerStatesManager.setPlayerModeByName (currentQuickModeInfo.playerModeToSelect);
				}

				if (currentQuickModeInfo.modeIsPower) {
					if (currentQuickModeInfo.setNewPower) {
						mainOtherPowers.setCurrentPowerByName (currentQuickModeInfo.powerNameToUse);
					}

					if (currentQuickModeInfo.modeUsesPressDownAndUpInput) {
						mainOtherPowers.inputHoldOrReleaseShootPower (true);
					}
				}

				if (currentQuickModeInfo.modeIsCloseCombat) {
					if (currentQuickModeInfo.modeUsedToBlock) {
						mainCloseCombatSystem.setBlockStateWithoutInputCheck (true);
					} else {
						mainCloseCombatSystem.useAttack (currentQuickModeInfo.closeCombatAttackName);
					}
				}
			}
		}
	}

	void selectMeleeWeaponOnQuickAccessSlosts (bool selectingMode)
	{
		if (selectingMode) {
			if (currentQuickModeInfo.useCurrentWeaponIfAlreadyCarriedOnSameMode) {
				mainMeleeWeaponsGrabbedManager.inputDrawOrKeepMeleeWeapon ();
			} else {
				if (currentQuickModeInfo.useFirstWeaponTypeEquippedFound) {
					if (currentQuickModeInfo.weaponIsBow) {
						string weaponNameToUse = mainMeleeWeaponsGrabbedManager.getFirstBowMeleeWeaponTypeAvailableName ();

						if (useInventoryQuickAccessSlotsSystem) {
							if (weaponNameToUse != "") {
								mainInventoryQuickAccessSlotsSystem.checkQuickAccessSlotToSelectByName (weaponNameToUse);
							} else {
								mainInventoryQuickAccessSlotsSystem.selectFirstMeleeWeaponAvailable ();
							}
						} else {
							if (weaponNameToUse != "") {
								checkQuickAccessSlotToSelectMeleeByNameForAI (currentQuickModeInfo.weaponNameToUse);
							} else {

							}
						}
					} else {
						string weaponNameToUse = mainMeleeWeaponsGrabbedManager.getFirstRegularMeleeWeaponTypeAvailableName ();

						if (useInventoryQuickAccessSlotsSystem) {
							if (weaponNameToUse != "") {
								mainInventoryQuickAccessSlotsSystem.checkQuickAccessSlotToSelectByName (weaponNameToUse);
							} else {
								mainInventoryQuickAccessSlotsSystem.selectFirstMeleeWeaponAvailable ();
							}
						} else {
							if (weaponNameToUse != "") {
								checkQuickAccessSlotToSelectMeleeByNameForAI (currentQuickModeInfo.weaponNameToUse);
							} else {

							}
						}
					}
				} else {
					bool weaponAvailable = false;

					if (mainMeleeWeaponsGrabbedManager.checkIfCarryingWeaponByName (currentQuickModeInfo.weaponNameToUse)) {
						weaponAvailable = true;
					}

					if (useInventoryQuickAccessSlotsSystem) {
						if (weaponAvailable) {
							mainInventoryQuickAccessSlotsSystem.checkQuickAccessSlotToSelectByName (currentQuickModeInfo.weaponNameToUse);
						} else {
							mainInventoryQuickAccessSlotsSystem.selectFirstMeleeWeaponAvailable ();
						}
					} else {
						if (weaponAvailable) {
							checkQuickAccessSlotToSelectMeleeByNameForAI (currentQuickModeInfo.weaponNameToUse);
						} else {

						}
					}
				}
			}
		} else {
			bool weaponAvailable = false;

			if (mainMeleeWeaponsGrabbedManager.checkIfCarryingWeaponByName (currentSelectionInfo.weaponName)) {
				weaponAvailable = true;
			}

			if (useInventoryQuickAccessSlotsSystem) {
				if (weaponAvailable) {
					mainInventoryQuickAccessSlotsSystem.setOverrideChangeToWeaponWithoutDoubleSelectionState (true);

					mainInventoryQuickAccessSlotsSystem.checkQuickAccessSlotToSelectByName (currentSelectionInfo.weaponName);
				} else {
					mainInventoryQuickAccessSlotsSystem.selectFirstMeleeWeaponAvailable ();
				}
			} else {
				if (weaponAvailable) {
					checkQuickAccessSlotToSelectMeleeByNameForAI (currentQuickModeInfo.weaponNameToUse);
				} else {

				}
			}
		}
	}

	void selectFireWeaponOnQuickAccessSlots (bool selectingMode)
	{
		if (selectingMode) {
			if (currentQuickModeInfo.useCurrentWeaponIfAlreadyCarriedOnSameMode) {
				mainPlayerWeaponsManager.inputDrawWeapon ();
			} else {
				if (currentQuickModeInfo.useFirstWeaponTypeEquippedFound) {
					if (useInventoryQuickAccessSlotsSystem) {
						mainInventoryQuickAccessSlotsSystem.selectFirstFireWeaponAvailable ();

					} else {

					}
				} else {
					bool weaponAvailable = false;

					if (mainPlayerWeaponsManager.checkIfWeaponEnabledByName (currentQuickModeInfo.weaponNameToUse)) {
						weaponAvailable = true;
					}

					if (useInventoryQuickAccessSlotsSystem) {
						if (weaponAvailable) {
							mainInventoryQuickAccessSlotsSystem.checkQuickAccessSlotToSelectByName (currentQuickModeInfo.weaponNameToUse);
						} else {
							mainInventoryQuickAccessSlotsSystem.selectFirstFireWeaponAvailable ();
						}
					} else {
						if (weaponAvailable) {
							checkQuickAccessSlotToSelectFireWeaponByNameForAI (currentQuickModeInfo.weaponNameToUse);
						} else {

						}
					}
				}
			}
		} else {
			bool weaponAvailable = false;

			if (mainPlayerWeaponsManager.checkIfWeaponEnabledByName (currentSelectionInfo.weaponName)) {
				weaponAvailable = true;
			}

			if (useInventoryQuickAccessSlotsSystem) {
				if (weaponAvailable) {
					mainInventoryQuickAccessSlotsSystem.setOverrideChangeToWeaponWithoutDoubleSelectionState (true);

					mainInventoryQuickAccessSlotsSystem.checkQuickAccessSlotToSelectByName (currentSelectionInfo.weaponName);
				} else {
					mainInventoryQuickAccessSlotsSystem.selectFirstFireWeaponAvailable ();
				}
			} else {
				if (weaponAvailable) {
					checkQuickAccessSlotToSelectFireWeaponByNameForAI (currentQuickModeInfo.weaponNameToUse);
				} else {

				}
			}
		}
	}

	public void stopUpdateCheckChangeOfModeCoroutine ()
	{
		if (checkChangeOfModeCoroutine != null) {
			StopCoroutine (checkChangeOfModeCoroutine);
		}

		changeOfModeInProcess = false;

		selectionInfoActive = false;
	}

	IEnumerator updateCheckChangeOfModeCoroutine (bool isSelectingNewMode, bool weaponIsMelee, bool useWeaponOnMode, bool weaponIsBow)
	{
		changeOfModeInProcess = true;

		bool checkChangeModeComplete = false;

		float movementTimer = 0;

		float delayTimer = minWaitTimeToChangeMode;

		while (!checkChangeModeComplete) {

			movementTimer += Time.deltaTime;

			if (movementTimer > delayTimer) {
				checkChangeModeComplete = true;
			}

			if (movementTimer > 1) {
				if (showDebugPrint) {
					print ("coroutine to check state ran out of time, resuming process");
				}

				checkChangeModeComplete = true;
			}
		
			yield return null;
		}

		if (useWeaponOnMode) {
			if (weaponIsMelee) {
				if (!selectionInfoActive) {
					if (weaponIsBow) {
						mainBowSystem.inputFireArrow ();
					} else {
						if (currentQuickModeInfo.modeUsedToBlock) {
							mainMeleeWeaponsGrabbedManager.activateOrDeactivateBlockGrabbedObjectMeleeeCheckingCurrentState (true);
						} else {
							mainMeleeWeaponsGrabbedManager.activateGrabbedObjectMeleeAttackByIndex ();
						}
					}
				}

				if (isSelectingNewMode) {
					reactivateRegularDrawActionsOnWeapons ();
				}
			} else {
				if (!selectionInfoActive) {
					mainPlayerWeaponsManager.inputHoldOrReleaseShootWeapon (true);
				}

				if (isSelectingNewMode) {
					reactivateRegularDrawActionsOnWeapons ();
				}
			}
		} else {
			if (isSelectingNewMode) {
				reactivateRegularDrawActionsOnWeapons ();
			}
		}

		changeOfModeInProcess = false;

		selectionInfoActive = false;
	}


	void reactivateRegularDrawActionsOnWeapons ()
	{
		mainMeleeWeaponsGrabbedManager.setOriginalIgnoreUseDrawKeepWeaponAnimationState ();

		mainBowSystem.setIgnoreUseDrawHolsterActionAnimationsState (false);

		mainPlayerWeaponsManager.setOriginalUseQuickDrawWeaponState ();

		mainPlayerWeaponsManager.setForceWeaponToUseQuickDrawKeepWeaponActiveState (false);

		mainPlayerWeaponsManager.setIgnoreUseDrawKeepWeaponAnimationState (false);
	}
		

	//SELECTION INFO FUNCTIONS
	void setSelectionInfo ()
	{
		if (currentSelectionInfo.selectionIsWeapon) {
			if (currentSelectionInfo.selectionIsMelee) {
				if (showDebugPrint) {
					print ("selecting new mode with a melee weapon");
				}

				changingModeToUseMeleeWeapons = false;
				changingModeToUseFireWeapons = false;

				currentlyCarryingFireWeapon = false;
				currentlyCarringMeleeWeapon = false;

				playerIsCarryingWeapons = false;
				playerIsOnWeaponModeToUse = false;

				if (mainPlayerController.isPlayerUsingWeapons ()) {
					playerIsCarryingWeapons = true;

					currentlyCarryingFireWeapon = true;

					changingModeToUseMeleeWeapons = true;
				} else {
					if (mainPlayerWeaponsManager.isWeaponsModeActive ()) {
						changingModeToUseMeleeWeapons = true;
					}
				}

				if (mainPlayerController.isPlayerUsingMeleeWeapons ()) {
					playerIsCarryingWeapons = true;

					currentlyCarringMeleeWeapon = true;

					playerIsOnWeaponModeToUse = true;
				} else {
					if (mainMeleeWeaponsGrabbedManager.isMeleeWeaponsGrabbedManagerActive ()) {
						playerIsOnWeaponModeToUse = true;
					}
				}

				if (showDebugPrint) {
					print ("changingModeToUseMeleeWeapons " + changingModeToUseMeleeWeapons);
					print ("changingModeToUseFireWeapons " + changingModeToUseFireWeapons);
					print ("currentlyCarryingFireWeapon " + currentlyCarryingFireWeapon);
					print ("currentlyCarringMeleeWeapon " + currentlyCarringMeleeWeapon);
					print ("playerIsCarryingWeapons " + playerIsCarryingWeapons);
					print ("playerIsOnWeaponModeToUse " + playerIsOnWeaponModeToUse);
				}

				mainMeleeWeaponsGrabbedManager.setIgnoreUseDrawKeepWeaponAnimationState (true);

				mainBowSystem.setIgnoreUseDrawHolsterActionAnimationsState (true);

				bool activateCoroutineResult = false;

				if (playerIsOnWeaponModeToUse) {
					mainMeleeWeaponsGrabbedManager.resetCurrentAttackByIndex ();

					if (playerIsCarryingWeapons) {
						if (!currentSelectionInfo.powerNameToUse.Equals (mainMeleeWeaponsGrabbedManager.getCurrentWeaponName ())) {
							playerIsCarryingWeapons = false;

							if (showDebugPrint) {
								print ("current mode is melee and character is carrying melee weapon, but it is not the one for this mode, " +
								"selecting proper weapon");
							}
						}
					}

					if (!playerIsCarryingWeapons) {
						if (showDebugPrint) {
							print ("not carrying melee weapon, drawing melee weapon and then, activating attack by index");
						}

						selectMeleeWeaponOnQuickAccessSlosts (false);

						activateCoroutineResult = true;
					}
				} else {
					if (showDebugPrint) {
						print ("player is not on melee weapons mode");
					}

					if (changingModeToUseMeleeWeapons) {
						if (showDebugPrint) {
							print ("currently on fire weapons mode, changing mode");
						}

						if (currentlyCarryingFireWeapon) {
							if (showDebugPrint) {
								print ("currently carrying a weapon, activating quick holster");
							}

							mainPlayerWeaponsManager.setUseQuickDrawWeaponState (true);

							mainPlayerWeaponsManager.setForceWeaponToUseQuickDrawKeepWeaponActiveState (true);

							mainPlayerWeaponsManager.setIgnoreUseDrawKeepWeaponAnimationState (true);

							selectMeleeWeaponOnQuickAccessSlosts (false);
						} else {
							selectMeleeWeaponOnQuickAccessSlosts (false);
						}
					} else {
						if (showDebugPrint) {
							print ("currently not carrying a weapon, activating change of mode");
						}

						selectMeleeWeaponOnQuickAccessSlosts (false);
					}


					activateCoroutineResult = true;
				}


				if (activateCoroutineResult) {
					stopUpdateCheckChangeOfModeCoroutine ();

					selectionInfoActive = true;

					checkChangeOfModeCoroutine = StartCoroutine (updateCheckChangeOfModeCoroutine (true,
						currentSelectionInfo.selectionIsMelee, currentSelectionInfo.selectionIsWeapon, currentSelectionInfo.selectionIsBow));
				}

			} else {
				if (showDebugPrint) {
					print ("selecting new mode with a fire weapon");
				}


				changingModeToUseMeleeWeapons = false;
				changingModeToUseFireWeapons = false;

				currentlyCarryingFireWeapon = false;
				currentlyCarringMeleeWeapon = false;

				playerIsCarryingWeapons = false;
				playerIsOnWeaponModeToUse = false;

				if (mainPlayerController.isPlayerUsingWeapons ()) {
					playerIsCarryingWeapons = true;

					currentlyCarryingFireWeapon = true;

					playerIsOnWeaponModeToUse = true;
				} else {
					if (mainPlayerWeaponsManager.isWeaponsModeActive ()) {
						playerIsOnWeaponModeToUse = true;
					}
				}

				if (mainPlayerController.isPlayerUsingMeleeWeapons ()) {
					playerIsCarryingWeapons = true;

					currentlyCarringMeleeWeapon = true;

					changingModeToUseFireWeapons = true;
				} else {
					if (mainMeleeWeaponsGrabbedManager.isMeleeWeaponsGrabbedManagerActive ()) {
						changingModeToUseFireWeapons = true;
					}
				}

				if (showDebugPrint) {
					print ("changingModeToUseMeleeWeapons " + changingModeToUseMeleeWeapons);
					print ("changingModeToUseFireWeapons " + changingModeToUseFireWeapons);
					print ("currentlyCarryingFireWeapon " + currentlyCarryingFireWeapon);
					print ("currentlyCarringMeleeWeapon " + currentlyCarringMeleeWeapon);
					print ("playerIsCarryingWeapons " + playerIsCarryingWeapons);
					print ("playerIsOnWeaponModeToUse " + playerIsOnWeaponModeToUse);
				}

				mainPlayerWeaponsManager.setUseQuickDrawWeaponState (true);

				mainPlayerWeaponsManager.setForceWeaponToUseQuickDrawKeepWeaponActiveState (true);

				mainPlayerWeaponsManager.setIgnoreUseDrawKeepWeaponAnimationState (true);

				bool activateCoroutineResult = false;

				if (playerIsOnWeaponModeToUse) {

//					if (!playerIsCarryingWeapons) {
					if (showDebugPrint) {
						print ("not carrying fire weapon, drawing fire weapon and then");
					}

					selectFireWeaponOnQuickAccessSlots (false);

					activateCoroutineResult = true;
//					}
				} else {
					if (showDebugPrint) {
						print ("player is not on fire weapons mode");
					}

					if (changingModeToUseFireWeapons) {
						if (showDebugPrint) {
							print ("currently on melee weapons mode, changing mode");
						}

						if (currentlyCarringMeleeWeapon) {
							if (showDebugPrint) {
								print ("currently carrying a melee weapon, activating quick holster");
							}

							mainMeleeWeaponsGrabbedManager.setIgnoreUseDrawKeepWeaponAnimationState (true);

							mainBowSystem.setIgnoreUseDrawHolsterActionAnimationsState (true);

							selectFireWeaponOnQuickAccessSlots (false);
						} else {
							selectFireWeaponOnQuickAccessSlots (false);
						}
					} else {
						if (showDebugPrint) {
							print ("currently not carrying a weapon, activating change of mode");
						}

						selectFireWeaponOnQuickAccessSlots (false);
					}


					activateCoroutineResult = true;
				}

				if (activateCoroutineResult) {
					stopUpdateCheckChangeOfModeCoroutine ();

					selectionInfoActive = true;

					checkChangeOfModeCoroutine = StartCoroutine (updateCheckChangeOfModeCoroutine (true,
						currentSelectionInfo.selectionIsMelee, currentSelectionInfo.selectionIsWeapon, currentSelectionInfo.selectionIsBow));
				}
			}
		} else {
			mainMeleeWeaponsGrabbedManager.setIgnoreUseDrawKeepWeaponAnimationState (true);

			mainBowSystem.setIgnoreUseDrawHolsterActionAnimationsState (true);

			mainPlayerWeaponsManager.setUseQuickDrawWeaponState (true);

			mainPlayerWeaponsManager.setForceWeaponToUseQuickDrawKeepWeaponActiveState (true);

			mainPlayerWeaponsManager.setIgnoreUseDrawKeepWeaponAnimationState (true);

			if (currentSelectionInfo.setModeToSelect) {
				mainPlayerStatesManager.setPlayerModeByName (currentSelectionInfo.playerModeToSelect);
			}

			if (currentSelectionInfo.selectionIsPower) {
				if (currentSelectionInfo.setNewPower) {
					mainOtherPowers.setCurrentPowerByName (currentSelectionInfo.powerNameToUse);
				}
			}
		}
	}

	public void setSelectionModeDisabled (string selectionName)
	{
		setSelectionModeEnabledOrDisabledState (false, selectionName);
	}

	public void setSelectionModeEnabled (string selectionName)
	{
		setSelectionModeEnabledOrDisabledState (true, selectionName);
	}

	public void setSelectionModeEnabledOrDisabledState (bool state, string selectionName)
	{
		if (selectionInfoEnabled && setSelectionInfoEnableStateEnabled && selectionName != null && selectionName != "") {
			
			int selectionIndex = selectionInfoList.FindIndex (s => s.Name.Equals (selectionName));

			if (selectionIndex > -1) {
				if (showDebugPrint) {
					print ("setting selection info enabled state for " + selectionName + " as " + state);
				}

				selectionInfoList [selectionIndex].selectionEnabled = state;

				if (!state) {
					if (currentSelectionInfoIndex == selectionIndex) {
						if (showDebugPrint) {
							print ("current selection has been disabled, so checking to set next one");
						}

						inputSetNextOrPreviousSelectionInfo (true);
					}


					return;
				}

				if (setSelectionInfoWhenEnabled) {
					if (state) {
						currentSelectionInfoIndex = selectionIndex;

						currentSelectionInfoIndex--;

						if (currentSelectionInfoIndex < 0) {
							currentSelectionInfoIndex = selectionInfoList.Count - 1;

							inputSetNextOrPreviousSelectionInfo (true);
						}
					}
				}
			}
		}
	}

	public void checkToSelectNextSelectionOnWeaponAmmoEmpty ()
	{
		if (!selectionInfoEnabled) {
			return;
		}

		if (!externalSetSelectionInfoEnabled) {
			return;
		}

		if (currentSelectionInfoIndex != -1) {
			if (showDebugPrint) {
				print ("checkToSelectNextSelectionOnWeaponAmmoEmpty");
			}

			if (mainPlayerWeaponsManager.isWeaponsModeActive ()) {
				string currentWeaponName = mainPlayerWeaponsManager.getCurrentWeaponName ();

				if (showDebugPrint) {
					print ("checking state empty ammon on " + currentWeaponName);

					print (currentSelectionInfo.selectionEnabled + " " + currentSelectionInfo.isCurrentSelection + " " +
					currentSelectionInfo.weaponName.Equals (currentWeaponName));
				}

				if (currentSelectionInfo.selectionEnabled && currentSelectionInfo.isCurrentSelection &&
				    currentSelectionInfo.weaponName.Equals (currentWeaponName)) {

					int remainAmmo = mainPlayerWeaponsManager.getAllRemainAmmoOnWeapon (currentWeaponName);

					if (showDebugPrint) {
						print ("remain ammo " + remainAmmo);
					}
						
					bool weaponAmmoCheckResult = remainAmmo <= 0;

					if (weaponAmmoCheckResult) {
						if (showDebugPrint) {
							print ("player using fire weapons mode, and the current weapon is empty, setting next mode or weapon");
						}

						inputSetNextOrPreviousSelectionInfo (true);
					}
				}
			}
		}
	}

	public void setSelectionInfoEnabledState (bool state)
	{
		selectionInfoEnabled = state;
	}

	public void setSetSelectionInfoEnableStateEnabledState (bool state)
	{
		setSelectionInfoEnableStateEnabled = state;
	}

	public void setExternalSetSelectionInfoEnabledState (bool state)
	{
		externalSetSelectionInfoEnabled = state;
	}
		

	//INPUT FUNCTIONS
	bool canUseInput ()
	{
		if (!mainPlayerController.canPlayerMove ()) {
			if (showDebugPrint) {
				print ("player can't move, cancelling");
			}

			return false;
		}

//		if (mainPlayerController.isUsingDevice ()) {
//			if (showDebugPrint) {
//				print ("1");
//			}
//		}
//
//		if (mainPlayerController.isUsingSubMenu ()) {
//			if (showDebugPrint) {
//				print ("2");
//			}
//		}
//
//		if (mainPlayerController.isPlayerMenuActive ()) {
//			if (showDebugPrint) {
//				print ("3");
//			}
//		}
//
//		if (mainPlayerController.isTurnBasedCombatActionActive ()) {
//			if (showDebugPrint) {
//				print ("4");
//			}
//		}

		if (mainPlayerController.playerIsBusy ()) {
			if (showDebugPrint) {
				print ("player busy, cancelling");
			}

			return false;
		}

		if (mainPlayerController.isGamePaused ()) {
			if (showDebugPrint) {
				print ("game paused, cancelling");
			}

			return false;
		}

		if (!mainPlayerController.checkPlayerIsNotCarringWeapons ()) {
			if (showDebugPrint) {
				print ("not carrying weapons, cancelling");
			}

			return false;
		}

		//		if (mainPlayerWeaponsManager.isActionActiveInPlayer ()) {
		//			return false;
		//		}

		if (mainPlayerController.iscloseCombatAttackInProcess ()) {
			if (showDebugPrint) {
				print ("close combat in progress, cancelling");
			}

			return false;
		}

		return true;
	}

	public void inputActiveModePressHold (string modeName)
	{
		if (!quickChangeEnabled) {
			return;
		}

		if (changeOfModeInProcess) {
			if (showDebugPrint) {
				print ("change of mode in process, cancelling press hold on mode " + modeName);
			}

			return;
		}

		//		if (!canUseInput ()) {
		//			if (showDebugPrint) {
		//				print ("player is busy, cancelling checking up action");
		//			}
		//
		//			return;
		//		}

		if (currentModeName.Equals (modeName)) {
			if (showDebugPrint) {
				print ("pressing hold on current mode " + modeName);
			}

			if (currentQuickModeInfo.modeUsesPressHoldInput) {
				if (currentQuickModeInfo.pressDownActive) {
					if (currentQuickModeInfo.modeIsPower) {
						mainOtherPowers.inputHoldShootPower ();
					}
				}
			}
		}
	}

	public void checkIfModeActiveToReleaseIt ()
	{
		if (!quickChangeEnabled) {
			return;
		}

		if (changeOfModeInProcess) {
			return;

		}

		if (currentQuickModeInfo != null) {
			if (currentQuickModeInfo.pressDownActive) {
				inputActiveModePressUp (currentQuickModeInfo.quickModeName);
			}
		}
	}

	public void inputActiveModePressUp (string modeName)
	{
		if (!quickChangeEnabled) {
			return;
		}

		if (changeOfModeInProcess) {
			if (showDebugPrint) {
				print ("change of mode in process, cancelling press up on mode " + modeName);
			}

			return;
		}

		//		if (!canUseInput ()) {
		//			if (showDebugPrint) {
		//				print ("player is busy, cancelling checking up action");
		//			}
		//
		//			return;
		//		}

		if (currentModeName.Equals (modeName)) {
			if (showDebugPrint) {
				print ("pressing up on current mode " + modeName);
			}

			if (currentQuickModeInfo.modeUsesPressDownAndUpInput) {
				if (currentQuickModeInfo.pressDownActive) {
					if (currentQuickModeInfo.useWeaponOnMode) {
						if (showDebugPrint) {
							print ("using weapons on mode");
						}

						if (currentQuickModeInfo.weaponIsMelee) {
							if (showDebugPrint) {
								print ("weapon is melee");
							}

							if (currentQuickModeInfo.weaponIsBow) {
								mainBowSystem.inputStopFireArrowWithoutAim ();

								if (showDebugPrint) {
									print ("weapon is bow, stoppig fire arrows");
								}
							} else {
								if (currentQuickModeInfo.modeUsedToBlock) {
									mainMeleeWeaponsGrabbedManager.activateOrDeactivateBlockGrabbedObjectMeleeeCheckingCurrentState (false);
								}
							}
						} else {
							mainPlayerWeaponsManager.inputHoldOrReleaseShootWeapon (false);

							if (showDebugPrint) {
								print ("weapon is fire weapon, stopping shoot");
							}
						}
					} else {
						if (currentQuickModeInfo.modeIsPower) {
							mainOtherPowers.inputHoldOrReleaseShootPower (false);

							if (showDebugPrint) {
								print ("mode is powers, stopping shoot");
							}

							reactivateRegularDrawActionsOnWeapons ();
						}

						if (currentQuickModeInfo.modeIsCloseCombat) {
							if (currentQuickModeInfo.modeUsedToBlock) {
								mainCloseCombatSystem.setBlockStateWithoutInputCheck (false);
							}
						}
					}

					currentQuickModeInfo.pressDownActive = false;
				}
			}
		} else {
			if (showDebugPrint) {
				print ("input mode name doesn't match the current mode to press up, cancelling check");
			}
		}
	}

	public void inputActivateMode (string modeName)
	{
		if (!quickChangeEnabled) {
			return;
		}

		if (changeOfModeInProcess) {
			if (showDebugPrint) {
				print ("change of mode in process, cancelling press up on mode " + modeName);
			}

			return;
		}

		if (!canUseInput ()) {
			if (showDebugPrint) {
				print ("player is busy, cancelling checking up action");
			}

			return;
		}

		if (currentModeName.Equals (modeName)) {
			useMode (false);
		} else {
			int modeIndex = quickModeInfoList.FindIndex (s => s.quickModeName.Equals (modeName));

			if (modeIndex > -1) {
				currentModeIndex = modeIndex;

				for (int i = 0; i < quickModeInfoList.Count; i++) {
					if (currentModeIndex != i) {
						quickModeInfo currentInfo = quickModeInfoList [i];

						if (currentInfo.isCurrentMode) {
							if (currentInfo.pressDownActive) {
								if (currentInfo.useWeaponOnMode) {
									if (currentInfo.weaponIsMelee) {
										if (currentInfo.weaponIsBow) {
											mainBowSystem.inputStopFireArrowWithoutAim ();
										}

										if (currentInfo.modeUsedToBlock) {
											mainMeleeWeaponsGrabbedManager.activateOrDeactivateBlockGrabbedObjectMeleeeCheckingCurrentState (false);
										}
									} else {
										mainPlayerWeaponsManager.inputHoldOrReleaseShootWeapon (false);
									}
								} else {
									if (currentInfo.modeIsPower) {
										mainOtherPowers.inputHoldOrReleaseShootPower (false);
									}

									if (currentQuickModeInfo.modeIsCloseCombat) {
										if (currentQuickModeInfo.modeUsedToBlock) {
											mainCloseCombatSystem.setBlockStateWithoutInputCheck (false);
										}
									}
								}
							}
						}

						currentInfo.isCurrentMode = false;

						currentInfo.pressDownActive = false;

						if (currentInfo.useEventsOnModeChange) {
							currentInfo.eventOnUnSelectMode.Invoke ();
						}
					}
				}

				currentQuickModeInfo = quickModeInfoList [modeIndex];

				currentModeName = currentQuickModeInfo.quickModeName;

				currentQuickModeInfo.isCurrentMode = true;

				if (currentQuickModeInfo.useEventsOnModeChange) {
					currentQuickModeInfo.eventOnSelectMode.Invoke ();
				}

				useMode (true);
			}
		}
	}

	public void inputActivateModeDownHoldUpExternally (string modeName)
	{
		inputActivateMode (modeName);

		inputActiveModePressHold (modeName);

		inputActiveModePressUp (modeName);
	}

	public void inputSetNextOrPreviousSelectionInfo (bool state)
	{
		if (!selectionInfoEnabled) {
			return;
		}

		if (changeOfModeInProcess) {
			if (showDebugPrint) {
				print ("change of selection in process, cancelling next or previous selection " + currentSelectionInfoIndex);
			}

			return;
		}

		if (!canUseInput ()) {
			if (showDebugPrint) {
				print ("player is busy, cancelling checking up action");
			}

			return;
		}

		if (lastTimeSelectionInfoChanged > 0) {
			if (Time.time < lastTimeSelectionInfoChanged + minWaitTimeToChangeSelection) {
				if (showDebugPrint) {
					print ("waiting for enough delay to change selection");
				}

				return;
			}
		}

		lastTimeSelectionInfoChanged = Time.time;

		int selectionInfoListCount = selectionInfoList.Count;

		if (!initialSelectioInfoChecked || currentSelectionInfoIndex == -1) {
			if (mainPlayerController.isPlayerUsingMeleeWeapons ()) {
				string currentWeaponName = mainPlayerWeaponsManager.getCurrentWeaponName ();

				if (currentWeaponName != "") {
					int selectionIndex = selectionInfoList.FindIndex (s => s.weaponName.Equals (currentWeaponName));

					if (selectionIndex > -1) {
						currentSelectionInfoIndex = selectionIndex;

						selectionInfoList [currentSelectionInfoIndex].isCurrentSelection = true;
					}
				}

			} else if (mainPlayerController.isPlayerUsingWeapons ()) {
				string currentWeaponName = mainMeleeWeaponsGrabbedManager.getCurrentWeaponName ();

				if (currentWeaponName != "") {
					int selectionIndex = selectionInfoList.FindIndex (s => s.weaponName.Equals (currentWeaponName));

					if (selectionIndex > -1) {
						currentSelectionInfoIndex = selectionIndex;

						selectionInfoList [currentSelectionInfoIndex].isCurrentSelection = true;
					}
				}
			} else {
				string currentMode = mainPlayerStatesManager.getCurrentPlayersModeName ();

				if (currentMode != "") {
					int selectionIndex = selectionInfoList.FindIndex (s => s.playerModeToSelect.Equals (currentMode));

					if (selectionIndex > -1) {
						currentSelectionInfoIndex = selectionIndex;

						selectionInfoList [currentSelectionInfoIndex].isCurrentSelection = true;
					}
				}
			}

			initialSelectioInfoChecked = true;
		}

		if (state) {
			currentSelectionInfoIndex++;

			if (currentSelectionInfoIndex >= selectionInfoListCount) {
				currentSelectionInfoIndex = 0;
			}

			if (selectionInfoList [currentSelectionInfoIndex].selectionEnabled) {
				if (selectionInfoList [currentSelectionInfoIndex].selectionIsWeapon &&
				    !selectionInfoList [currentSelectionInfoIndex].selectionIsMelee &&
				    selectionInfoList [currentSelectionInfoIndex].checkIfWeaponHasRemainAmmo) {

					int remainAmmo = mainPlayerWeaponsManager.getAllRemainAmmoOnWeapon (selectionInfoList [currentSelectionInfoIndex].weaponName);

					int inventoryAmmo = mainPlayerWeaponsManager.getWeaponAmmoAmountFromInventory (selectionInfoList [currentSelectionInfoIndex].weaponName);

					bool assignNewIndexResult = remainAmmo <= 0 && inventoryAmmo <= 0;

					if (showDebugPrint) {
						print ("checking if remain ammo on weapon " + remainAmmo + " " + inventoryAmmo + "  " + assignNewIndexResult);
					}

					if (assignNewIndexResult) {
						currentSelectionInfoIndex++;

						if (currentSelectionInfoIndex >= selectionInfoListCount) {
							currentSelectionInfoIndex = 0;
						}

						if (showDebugPrint) {
							print ("increasing selection index due to empty ammo");
						}
					}
				}
			}

			if (!selectionInfoList [currentSelectionInfoIndex].selectionEnabled) {
				bool newIndexFound = false;

				int loopCount = 0;

				int temporalIndex = currentSelectionInfoIndex;

				while (!newIndexFound) {
					temporalIndex++;

					if (temporalIndex >= selectionInfoListCount) {
						temporalIndex = 0;
					}

					if (selectionInfoList [temporalIndex].selectionEnabled) {
						bool assignNewIndexResult = true;

						if (selectionInfoList [temporalIndex].selectionIsWeapon &&
						    !selectionInfoList [temporalIndex].selectionIsMelee &&
						    selectionInfoList [temporalIndex].checkIfWeaponHasRemainAmmo) {

							int remainAmmo = mainPlayerWeaponsManager.getAllRemainAmmoOnWeapon (selectionInfoList [temporalIndex].weaponName);

							int inventoryAmmo = mainPlayerWeaponsManager.getWeaponAmmoAmountFromInventory (selectionInfoList [temporalIndex].weaponName);

							assignNewIndexResult = remainAmmo > 0 || inventoryAmmo > 0;
						
							if (showDebugPrint) {
								print ("checking if remain ammo on weapon " + remainAmmo + " " + inventoryAmmo + "  " + assignNewIndexResult);
							}
						}

						if (assignNewIndexResult) {

							currentSelectionInfoIndex = temporalIndex;

							newIndexFound = true;
						}
					}

					loopCount++;

					if (loopCount > 100) {
						newIndexFound = true;
					}
				}

				if (!newIndexFound) {
					return;
				}
			}
		} else {
			currentSelectionInfoIndex--;

			if (currentSelectionInfoIndex < 0) {
				currentSelectionInfoIndex = selectionInfoListCount - 1;
			}

			if (selectionInfoList [currentSelectionInfoIndex].selectionEnabled) {
				if (selectionInfoList [currentSelectionInfoIndex].selectionIsWeapon &&
				    !selectionInfoList [currentSelectionInfoIndex].selectionIsMelee &&
				    selectionInfoList [currentSelectionInfoIndex].checkIfWeaponHasRemainAmmo) {

					int remainAmmo = mainPlayerWeaponsManager.getAllRemainAmmoOnWeapon (selectionInfoList [currentSelectionInfoIndex].weaponName);

					int inventoryAmmo = mainPlayerWeaponsManager.getWeaponAmmoAmountFromInventory (selectionInfoList [currentSelectionInfoIndex].weaponName);

					bool assignNewIndexResult = remainAmmo <= 0 && inventoryAmmo <= 0;

					if (showDebugPrint) {
						print ("checking if remain ammo on weapon " + remainAmmo + " " + inventoryAmmo + "  " + assignNewIndexResult);
					}

					if (assignNewIndexResult) {
						currentSelectionInfoIndex--;

						if (currentSelectionInfoIndex < 0) {
							currentSelectionInfoIndex = selectionInfoListCount - 1;
						}

						if (showDebugPrint) {
							print ("decreasing selection index due to empty ammo");
						}
					}
				}
			}

			if (!selectionInfoList [currentSelectionInfoIndex].selectionEnabled) {
				bool newIndexFound = false;

				int loopCount = 0;

				int temporalIndex = currentSelectionInfoIndex;

				while (!newIndexFound) {
					temporalIndex--;

					if (temporalIndex < 0) {
						temporalIndex = selectionInfoListCount - 1;
					}

					if (selectionInfoList [temporalIndex].selectionEnabled) {
						bool assignNewIndexResult = true;

						if (selectionInfoList [temporalIndex].selectionIsWeapon &&
						    !selectionInfoList [temporalIndex].selectionIsMelee &&
						    selectionInfoList [temporalIndex].checkIfWeaponHasRemainAmmo) {

							int remainAmmo = mainPlayerWeaponsManager.getAllRemainAmmoOnWeapon (selectionInfoList [temporalIndex].weaponName);

							int inventoryAmmo = mainPlayerWeaponsManager.getWeaponAmmoAmountFromInventory (selectionInfoList [temporalIndex].weaponName);

							assignNewIndexResult = remainAmmo > 0 || inventoryAmmo > 0;

							if (showDebugPrint) {
								print ("checking if remain ammo on weapon " + remainAmmo + " " + inventoryAmmo + "  " + assignNewIndexResult);
							}
						}

						if (assignNewIndexResult) {
							currentSelectionInfoIndex = temporalIndex;

							newIndexFound = true;
						}
					}

					loopCount++;

					if (loopCount > 100) {
						newIndexFound = true;
					}
				}

				if (!newIndexFound) {
					return;
				}
			}
		}

		for (int i = 0; i < selectionInfoListCount; i++) {
			if (currentSelectionInfoIndex != i) {
				selectionInfo currentInfo = selectionInfoList [i];

				if (currentInfo.isCurrentSelection) {
					if (currentInfo.selectionIsWeapon) {
						if (currentInfo.selectionIsMelee) {
							if (currentInfo.selectionIsBow) {
								if (mainBowSystem.isAimingBowActive ()) {
									mainBowSystem.inputStopFireArrowWithoutAim ();
								}
							}
						} else {
							if (mainPlayerWeaponsManager.isCharacterShooting ()) {
								mainPlayerWeaponsManager.inputHoldOrReleaseShootWeapon (false);
							}
						}
					} else {
						if (currentInfo.selectionIsPower) {
							if (mainOtherPowers.isAimingPower ()) {
								mainOtherPowers.inputHoldOrReleaseShootPower (false);
							}
						}
					}
				}

				currentInfo.isCurrentSelection = false;
			}
		}

		bool cancelSetSelectionInfoResult = false;

		currentSelectionInfo = selectionInfoList [currentSelectionInfoIndex];

		currentSelectionInfo.isCurrentSelection = true;

		if (currentSelectionInfoName != "") {
			if (currentSelectionInfo.Name.Equals (currentSelectionInfoName)) {
				cancelSetSelectionInfoResult = true;
			}
		}

		currentSelectionInfoName = currentSelectionInfo.Name;

		if (showDebugPrint) {
			print ("setting selection info " + currentSelectionInfo.Name);
		}

		if (cancelSetSelectionInfoResult) {
			if (showDebugPrint) {
				print ("selection already current one, cancelling");
			}

			return;
		}

		selectionInfoActive = false;

		setSelectionInfo ();
	}

	public void setSelectionInfoEnabledStateFromEditor (bool state)
	{
		setSelectionInfoEnabledState (state);

		updateComponent ();
	}

	public void setSetSelectionInfoEnableStateEnabledStateFromEditor (bool state)
	{
		setSetSelectionInfoEnableStateEnabledState (state);

		updateComponent ();
	}

	public void setExternalSetSelectionInfoEnabledStateFromEditor (bool state)
	{
		setExternalSetSelectionInfoEnabledState (state);

		updateComponent ();
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Quick Mode System", gameObject);
	}

	[System.Serializable]
	public class quickModeInfo
	{
		[Space]
		[Header ("Main Settings")]
		[Space]

		public string quickModeName;
		public bool quickModeEnabled = true;

		[Space]

		public bool useWeaponOnMode;
		public string weaponNameToUse;
		public bool weaponIsMelee;

		public bool weaponIsBow;

		public bool modeUsedToBlock;

		[Space]

		public bool useFirstWeaponTypeEquippedFound;
		public bool useCurrentWeaponIfAlreadyCarriedOnSameMode;

		[Space]

		public bool modeIsPower;
		public bool setNewPower;
		public string powerNameToUse;

		[Space]

		public bool modeIsCloseCombat;
		public string closeCombatAttackName;

		[Space]
		[Header ("Select Mode Settings")]
		[Space]

		public bool setModeToSelect;
		public string playerModeToSelect;

		public bool modeUsesPressDownAndUpInput;
		public bool modeUsesPressHoldInput;

		[Space]
		[Header ("Debug")]
		[Space]

		public bool isCurrentMode;

		public bool pressDownActive;

		[Space]
		[Header ("Event Settings")]
		[Space]

		public bool useEventsOnModeChange;
		public UnityEvent eventOnSelectMode;
		public UnityEvent eventOnUnSelectMode;

		public bool useEventOnUseMode;
		public UnityEvent eventOnUseMode;
	}

	[System.Serializable]
	public class selectionInfo
	{
		[Space]
		[Header ("Main Settings")]
		[Space]

		public string Name;
		public bool selectionEnabled = true;

		[Space]

		public bool selectionIsWeapon;
		public bool checkIfWeaponHasRemainAmmo;

		public bool selectionIsMelee;

		public bool selectionIsBow;

		public string weaponName;

		[Space]

		public bool selectionIsPower;
		public bool setNewPower;
		public string powerNameToUse;

		[Space]
		[Header ("Select Mode Settings")]
		[Space]

		public bool setModeToSelect;
		public string playerModeToSelect;

		[Space]
		[Header ("Debug")]
		[Space]

		public bool isCurrentSelection;
	}



	void checkQuickAccessSlotToSelectMeleeByNameForAI (string objectName)
	{
		if (showDebugPrint) {
			print ("changingModeToUseMeleeWeapons " + changingModeToUseMeleeWeapons);
			print ("changingModeToUseFireWeapons " + changingModeToUseFireWeapons);
			print ("currentlyCarryingFireWeapon " + currentlyCarryingFireWeapon);
			print ("currentlyCarringMeleeWeapon " + currentlyCarringMeleeWeapon);
			print ("playerIsCarryingWeapons " + playerIsCarryingWeapons);
			print ("playerIsOnWeaponModeToUse " + playerIsOnWeaponModeToUse);
		}

		if (playerIsOnWeaponModeToUse) {
			bool selectWeaponResult = false;

			if (mainMeleeWeaponsGrabbedManager.characterIsCarryingWeapon ()) {
				string weaponName = mainMeleeWeaponsGrabbedManager.getCurrentWeaponName ();

				if (!weaponName.Equals (objectName)) {
					selectWeaponResult = true;
				}
			} else {
				selectWeaponResult = true;
			}

			if (selectWeaponResult) {
				mainMeleeWeaponsGrabbedManager.checkWeaponToSelectOnQuickAccessSlots (objectName);
			}
		} else {
			if (changingModeToUseMeleeWeapons || changingModeToUseFireWeapons) {

				if (playerIsCarryingWeapons) {
					if (currentlyCarryingFireWeapon) {
						if (mainPlayerWeaponsManager.isPlayerCarringWeapon ()) {
							mainPlayerWeaponsManager.checkIfKeepSingleOrDualWeapon ();
						}
					}

					if (changingModeToUseMeleeWeapons) {
						//holster weapon


					}

					if (changingModeToUseFireWeapons) {
						//holster weapon


					}
				}
			}
		
			if (showDebugPrint) {
				print ("changing from fire to melee weapons");
			}

			mainPlayerStatesManager.setPlayerModeByName (currentQuickModeInfo.playerModeToSelect);

			if (showDebugPrint) {
				print ("object selected is melee weapon");
			}

			mainMeleeWeaponsGrabbedManager.checkWeaponToSelectOnQuickAccessSlots (objectName);

		}
	}

	void checkQuickAccessSlotToSelectFireWeaponByNameForAI (string objectName)
	{
		print ("changingModeToUseMeleeWeapons " + changingModeToUseMeleeWeapons);
		print ("changingModeToUseFireWeapons " + changingModeToUseFireWeapons);
		print ("currentlyCarryingFireWeapon " + currentlyCarryingFireWeapon);
		print ("currentlyCarringMeleeWeapon " + currentlyCarringMeleeWeapon);
		print ("playerIsCarryingWeapons " + playerIsCarryingWeapons);
		print ("playerIsOnWeaponModeToUse " + playerIsOnWeaponModeToUse);

		if (playerIsOnWeaponModeToUse) {
			bool selectWeaponResult = false;

			if (mainPlayerWeaponsManager.isPlayerCarringWeapon ()) {

				string weaponName = mainPlayerWeaponsManager.getCurrentWeaponName ();

				if (!weaponName.Equals (objectName)) {
					selectWeaponResult = true;
				}
			} else {
				selectWeaponResult = true;
			}

			if (selectWeaponResult) {
				mainPlayerWeaponsManager.checkWeaponToSelectOnQuickAccessSlots (objectName, true);
			}
		} else {
			if (changingModeToUseMeleeWeapons || changingModeToUseFireWeapons) {

				if (playerIsCarryingWeapons) {
					if (currentlyCarringMeleeWeapon) {
						if (mainMeleeWeaponsGrabbedManager.characterIsCarryingWeapon ()) {
							mainMeleeWeaponsGrabbedManager.drawOrKeepMeleeWeaponWithoutCheckingInputActive ();
						}
					}

					if (changingModeToUseMeleeWeapons) {
						//holster weapon


					}

					if (changingModeToUseFireWeapons) {
						//holster weapon


					}
				}
			}

			if (showDebugPrint) {
				print ("changing from melee to fire weapons");
			}

			mainPlayerStatesManager.setPlayerModeByName (currentQuickModeInfo.playerModeToSelect);

			if (showDebugPrint) {
				print ("object selected is fire weapon");
			}

			mainPlayerWeaponsManager.checkWeaponToSelectOnQuickAccessSlots (objectName, true);
		}
	}
}
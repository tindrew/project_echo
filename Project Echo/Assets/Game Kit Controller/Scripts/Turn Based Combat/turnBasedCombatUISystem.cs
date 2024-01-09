using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class turnBasedCombatUISystem : ingameMenuPanel
{
	[Space]
	[Header ("Main Settings")]
	[Space]

	public float minWaitTimeToStartCombat = 2.5f;

	public LayerMask layerToCheckCharacterTargetOnPress;

	public bool loseCombatIfPlayerDiesEnabled;

	public bool pauseRestOfAIOnSceneDuringCombat;

	[Space]
	[Header ("Combat Result Settings")]
	[Space]

	public bool runFromCombatEnabled = true;

	public bool respawnEnemyTeamOnOriginalPositionOnCombatRun;

	public bool resurrectEnemyTeamOnCombatRun;

	public bool respawnEnemyTeamOnOriginalPositionOnCombatLost;

	public bool healEnemyTeamIfRunAway;

	public bool holsterWeaponsAfterCombat;

	public bool holsterWeaponsOnPlayerAfterCombat;

	public bool useRespawnPositionsOnCombatRun;

	public float maxRadiusToRespawn = 3;

	[Space]
	[Header ("Turn Combat Command Settings")]
	[Space]

	public List<commandCategoryInfo> commandCategoryInfoList = new List<commandCategoryInfo> ();

	[Space]
	[Header ("UI Panel Settings")]
	[Space]

	public List<panelCategoryInfo> panelCategoryInfoList = new List<panelCategoryInfo> ();

	[Space]
	[Header ("Input Settings")]
	[Space]

	public string mainTurnBasedCombatInputName = "Turn Based Combat System";

	public List<playerActionSystem.inputToPauseOnActionIfo> customInputToPauseOnActionInfoList = new List<playerActionSystem.inputToPauseOnActionIfo> ();

	[Space]
	[Header ("Other Settings")]
	[Space]

	public List<turnBasedCombatChararcterUIInfoObject.statNameLettersInfo> statNameLettersInfoList = new List<turnBasedCombatChararcterUIInfoObject.statNameLettersInfo> ();

	[Space]

	public bool toggleTurnAndFreeCombatInputEnabled = true;

	public float minDistanceToResumeTurnCombat = 30;

	public float minWaitToShowExitCombatAfterResult = 2;

	public bool resetCharacterPositionsSmoothly = true;

	public bool resetCharacterPositionsSmoothlyOnCombatStart = true;

	public float horizontalLimitOnRightScreenCategoryPanel = 400;

	public float horizontalLimitOnRightScreenConfirmationPanel = 400;

	[Space]
	[Header ("Inventory Settings")]
	[Space]

	public bool setNextTurnOnInventoryItemUsed;

	public string useInventoryItemMessageContent = "-ITEM- used";

	public string itemNameField = "-ITEM-";

	public float useInventoryItemCommandDuration = 3;

	public string unableToUseObjectMessageContent = "-ITEM- can't be used right now";

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;
	public bool showStatsDebugPrint;

	public bool combatActive;

	public bool mainTurnBasedCombatSystemAssigned;

	public bool menuOpened;

	public bool currentTurnForPlayerTeam;

	public string playerTeamPositionDataName;
	public string enemyTeamPositionDataName;

	public int currentTurnCharacterIndex;

	public bool useCustomCameraState;

	public string customCameraState;

	public bool delayToActivateCommandActive;

	public bool playingCommandActive;

	public bool turnCombatActive = true;
	public bool freeCombatActive;

	public bool selectingTargetForCommandActive;

	public bool showPanelObjectOnRightOrLeftSideScreen;

	public bool showConfirmationPanelObjectOnRightOrLeftSideScreen;

	[Space]
	[Header ("Combat Result Debug")]
	[Space]

	public bool allPlayerTeamDead;

	public bool allEnemyTeamDead;

	public bool combatFinished;

	public bool combatWinned;

	[Space]
	[Header ("Characters Debug")]
	[Space]

	public List<characterInfo> playerTeamCharacterList = new List<characterInfo> ();

	public List<characterInfo> enemyTeamCharacterList = new List<characterInfo> ();

	[Space]

	public List<turnBasedCombatChararcterUIInfoObject.turnBasedCombatChararcterUIInfo> turnBasedCombatChararcterUIInfoList = new List<turnBasedCombatChararcterUIInfoObject.turnBasedCombatChararcterUIInfo> ();

	public characterInfo currentCharacterInfo;

	public List<GameObject> fullCharacterOnCombatList = new List<GameObject> ();

	[Space]

	public int numberOfCharactersOnPlayerTeam;

	public int numberOfCharactersOnEnemyTeam;

	[Space]
	[Header ("Inventory Items Debug")]
	[Space]

	public string currentInventoryObjectNameSelected;

	public bool inventoryItemSelected;

	public List<simpleUIButtonInfo> simpleUIButtonInfoInventoryButtonList = new List<simpleUIButtonInfo> ();

	[Space]
	[Header ("Teams Debug")]
	[Space]

	public turnBasedCombatTeamInfo playerTurnBasedCombatTeamInfo;
	public turnBasedCombatTeamInfo enemyTurnBasedCombatTeamInfo;

	[Space]
	[Header ("Components")]
	[Space]

	public turnBasedCombatSystem mainTurnBasedCombatSystem;

	public friendListManager mainFriendListManager;

	public GameObject mainPlayer;
	public playerController mainPlayerController;
	public playerCamera mainPlayerCamera;
	public usingDevicesSystem mainUsingDevicesSystem;
	public Camera mainCamera;

	public playerInputManager mainPlayerInputManager;

	public inventoryManager mainInventoryManager;

	public playerComponentsManager mainPlayerComponentsManager;

	public CanvasScaler mainCanvasScaler;

	[Space]
	[Header ("UI Components")]
	[Space]

	public RectTransform mainActionButtonsPanel;

	public GameObject playerTeamStatsUIPrefab;
	public GameObject enemyTeamStatsUIPrefab;

	public RectTransform playerTeamHealthSliderParent;

	public RectTransform eneyTeamHealthSliderParent;

	public RectTransform currentCharacterIcon;

	public RectTransform currentCharacterTargetIcon;

	public GameObject commandDescriptionGameObject;
	public Text commandDescriptionText;

	public GameObject exitCombatAfterResultPanel;

	public GameObject toggleCombatModeTouchButton;

	public GameObject inputActionsPanel;

	[Space]
	[Header ("UI Inventory Components")]
	[Space]

	public Transform inventoryItemsPanelParent;
	public GameObject inventoryItemButtonPrefab;

	public GameObject confirmUseInventoryItemPanel;

	public bool checkRightOrLeftSideForConfirmUseInventoryItemPanel;

	public RectTransform rightSideConfirmUseInventoryItemPanel;
	public RectTransform leftSideConfirmUseInventoryItemPanel;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public UnityEvent eventOnCombatWinned;

	public UnityEvent eventOnCombatLost;

	public UnityEvent eventOnRunFromCombat;

	public bool useEventOnCommandUsed;
	public eventParameters.eventToCallWithString eventOnCommandUsed;

	public bool useEventOnNotEnoughStatOnCommnad;
	public eventParameters.eventToCallWithString eventOnNotEnoughStatOnCommnad;


	Vector2 currentResolution;

	float horizontaResolution;


	bool componentsAssigned;

	Coroutine menuCoroutine;

	bool usingScreenSpaceCamera;
	Vector2 mainCanvasSizeDelta;
	Vector2 halfMainCanvasSizeDelta;

	bool initialPositionMainActionButtonsPanelAdjusted;

	commandInfo currentCommandInfo;

	Coroutine commandCoroutine;

	panelInfo currentPanelInfo;

	GameObject currentCharacterTargetForCommand;

	characterInfo currentCharacterTargetInfo;
	int currentCharacterTargetInfoIndex;

	float lastTimeCombatActived;

	Coroutine activateCommandCoroutine;

	bool checkIfAllCharactersPositionAdjustedToShowActionsButtonPanel;


	void Start ()
	{
		if (!mainTurnBasedCombatSystemAssigned) {
			if (mainTurnBasedCombatSystem != null) {
				mainTurnBasedCombatSystemAssigned = true;
			}
		}
	}

	//TURN BASED COMBAT LOGIC FUNCTIONS
	public void stopMenuCoroutineUpdate ()
	{
		if (menuCoroutine != null) {
			StopCoroutine (menuCoroutine);
		}
	}

	IEnumerator menuCoroutineUpdate ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			if (!initialPositionMainActionButtonsPanelAdjusted) {
				if (currentTurnForPlayerTeam) {
					if (Time.time > minWaitTimeToStartCombat + lastTimeCombatActived) {
						updateMainActionButtonsPanelToCharacter ();

						updateCurrentCharacterIcon ();

						initialPositionMainActionButtonsPanelAdjusted = true;
					}
				} else {
					initialPositionMainActionButtonsPanelAdjusted = false;
				}
			}

			if (selectingTargetForCommandActive) {
				if (currentTurnForPlayerTeam) {
					int touchCount = Input.touchCount;
					if (!touchPlatform) {
						touchCount++;
					}

					RaycastHit hit;

					for (int i = 0; i < touchCount; i++) {
						if (!touchPlatform) {
							currentTouch = touchJoystick.convertMouseIntoFinger ();
						} else {
							currentTouch = Input.GetTouch (i);
						}

						if (currentTouch.phase == TouchPhase.Began) {
							Ray ray = mainCamera.ScreenPointToRay (currentTouch.position);

							if (Physics.Raycast (ray, out hit, 400, layerToCheckCharacterTargetOnPress)) {
								currentCharacterTargetForCommand = hit.collider.gameObject;

								for (int j = 0; j < numberOfCharactersOnEnemyTeam; j++) {
									if (!enemyTeamCharacterList [j].mainHealth.isDead () && enemyTeamCharacterList [j].mainGameObject == currentCharacterTargetForCommand) {
										selectingTargetForCommandActive = false;

										updateCurrentCharacterTargetIcon (enemyTeamCharacterList [j].mainGameObject.transform);

										currentCharacterTargetInfo = enemyTeamCharacterList [j];

										currentCharacterTargetInfoIndex = j;

										confirmCurrentCommand ();
									}
								}
							}
						}
					}
				} else {

				}
			}

			if (checkIfAllCharactersPositionAdjustedToShowActionsButtonPanel) {
				if (allCharactersPositionAdjusted ()) {

					enableOrDisableMainActionButtonsPanel (true);

					setNextCharacterInfo ();

					checkIfAllCharactersPositionAdjustedToShowActionsButtonPanel = false;
				}
			} else {
				if (currentTurnForPlayerTeam) {
					if (currentCharacterInfo != null) {
						if (currentCharacterInfo.mainHealth.isDead ()) {
							if (showDebugPrint) {
								print ("current character in turn has dead, moving turn to next character");
							}

							setNextTurn ();
						}
					}
				}
			}

			yield return waitTime;
		}
	}

	void setCurrentCommandInfoByIndex (int commandIndex, int commandCategoryIndex)
	{
		currentCommandInfo = commandCategoryInfoList [commandCategoryIndex].commandInfoList [commandIndex];

		if (!currentCommandInfo.isCommandEnabled) {
			return;
		}

		selectingTargetForCommandActive = false;

		enableOrDisableCurrentCharacterTargetIcon (false);

		bool canActivateCommandResult = true;

		if (currentCommandInfo.useStatToUseAction) {
			if (currentCharacterInfo.mainPlayerStatsSystem.getStatValue (currentCommandInfo.statNameToUseAction) < currentCommandInfo.statAmountToUseAction) {
				canActivateCommandResult = false;
			} else {
				currentCharacterInfo.mainPlayerStatsSystem.usePlayerStat (currentCommandInfo.statNameToUseAction, currentCommandInfo.statAmountToUseAction);
			
				updateAllCharacterStatsUIValue ();
			}
		}

		if (!canActivateCommandResult) {
			if (useEventOnNotEnoughStatOnCommnad) {
				eventOnNotEnoughStatOnCommnad.Invoke (currentCommandInfo.messageOnNotEnoughStat);
			}

			return;
		}

		if (currentCommandInfo.useEventsOnSelectCommand) {
			currentCommandInfo.eventOnSelectCommand.Invoke ();
		}

		if (currentCommandInfo.showConfirmationPanel) {
			currentCommandInfo.confirmationPanel.SetActive (true);

			if (currentCommandInfo.checkRightOrLeftSideForConfirmationPanel) {

				RectTransform currentPanelObjectRectTransform = currentCommandInfo.confirmationPanel.GetComponent<RectTransform> ();

				Vector3 currentCharacterPosition = currentCharacterInfo.mainTransform.position;

				Vector3 screenPoint = mainCamera.WorldToScreenPoint (currentCharacterPosition);

				float currentHorizontalPosition = screenPoint.x;

				showConfirmationPanelObjectOnRightOrLeftSideScreen = true;

				if (horizontaResolution - currentHorizontalPosition < horizontalLimitOnRightScreenConfirmationPanel) {
					showConfirmationPanelObjectOnRightOrLeftSideScreen = false;
				}

				if (showDebugPrint) {
					print ("currentHorizontalPosition " + horizontaResolution + " " + currentHorizontalPosition);
				}

				if (showConfirmationPanelObjectOnRightOrLeftSideScreen) {
					currentPanelObjectRectTransform.position = currentCommandInfo.rightSideConfirmationPanel.position;
				} else {
					currentPanelObjectRectTransform.position = currentCommandInfo.leftSideConfirmationPanel.position;
				}
			} else {
				currentCommandInfo.confirmationPanel.transform.localPosition = currentCommandInfo.originalConfirmationPanelPosition;
			}

			if (currentCommandInfo.selectTargetForCommand) {
				selectingTargetForCommandActive = true;

				if (currentTurnForPlayerTeam) {
					if (currentCharacterTargetInfo != null) {
						updateCurrentCharacterTargetIcon (currentCharacterTargetInfo.mainTransform);
					}
				} else {
					if (currentCharacterTargetInfo != null) {
						updateCurrentCharacterTargetIcon (currentCharacterTargetInfo.mainTransform);
					}
				}
			}
		} else {
			if (currentCommandInfo.selectTargetForCommand) {
				selectingTargetForCommandActive = true;

				if (currentTurnForPlayerTeam) {
					if (currentCharacterTargetInfo != null) {
						updateCurrentCharacterTargetIcon (currentCharacterTargetInfo.mainTransform);
					}
				} else {
					if (currentCharacterTargetInfo != null) {
						updateCurrentCharacterTargetIcon (currentCharacterTargetInfo.mainTransform);
					}
				}
			} else {
				closeAllPanelInfo ();

				confirmCurrentCommand ();
			}
		}
	}

	public void setCurrentCommandNameUsed (string commandName)
	{
		if (useEventOnCommandUsed) {
			eventOnCommandUsed.Invoke (commandName);
		}
	}

	public void confirmCurrentCommand ()
	{
		if (currentCommandInfo == null) {
			if (showDebugPrint) {
				print ("no command selecting, cancelling confirm command");
			}

			return;
		}

		setCurrentCommandNameUsed (currentCommandInfo.commandName);

		selectingTargetForCommandActive = false;

		enableOrDisableCurrentCharacterTargetIcon (false);

		if (currentCommandInfo.showConfirmationPanel) {
			currentCommandInfo.confirmationPanel.SetActive (false);
		}

		enableOrDisableMainActionButtonsPanel (false);

		enableOrDisableCurrentCharacterIcon (false);

		disableCurrentCommandDescription ();

	
		if (currentTurnForPlayerTeam) {
			if (currentCharacterTargetInfo != null) {
				if (currentCharacterInfo.isPlayer) {
					Transform placeToShoot = applyDamage.getPlaceToShoot (currentCharacterTargetInfo.mainGameObject);

					if (placeToShoot == null) {
						placeToShoot = currentCharacterTargetInfo.mainTransform;
					}

					currentCharacterInfo.mainPlayerCamera.setTemporalObjectToLookAtTarget (placeToShoot);

					if (showDebugPrint) {
						print ("target selected by the player for player character is " + currentCharacterTargetInfo.mainHealth.getCharacterName ());
					}
				} else {
					currentCharacterInfo.mainFindObjectivesSystem.setCurrentCharacterTurnTarget (currentCharacterTargetInfo.mainGameObject);

					currentCharacterInfo.mainMatchPlayerToTargetSystem.clearAllCharactersAround ();

					currentCharacterInfo.mainMatchPlayerToTargetSystem.addCharacterAround (currentCharacterTargetInfo.mainTransform);

					if (showDebugPrint) {
						print ("target selected by the player for friend character is " + currentCharacterTargetInfo.mainHealth.getCharacterName ());
					}
				}
			}
		}

		if (currentCharacterInfo.isPlayer) {
			currentCharacterInfo.mainMatchPlayerToTargetSystem.clearAllCharactersAround ();

			if (currentCharacterTargetInfo != null) {
				currentCharacterInfo.mainMatchPlayerToTargetSystem.addCharacterAround (currentCharacterTargetInfo.mainTransform);
			}

			if (currentCommandInfo.useMatchPositionSystem) {
				currentCharacterInfo.mainMatchPlayerToTargetSystem.activateMatchPosition (currentCommandInfo.matchPositionOffset);
			}
		}

		if (currentCommandInfo.useDelayToActivateCommand) {
			stopDelaytToActivateCommandCoroutine ();

			activateCommandCoroutine = StartCoroutine (delaytToActivateCommandCoroutine (currentCommandInfo.delayToActivateCommand));
		} else {
			activateCurrentCommand ();
		}
	}

	IEnumerator delaytToActivateCommandCoroutine (float currentDelay)
	{
		delayToActivateCommandActive = true;

		WaitForSeconds delay = new WaitForSeconds (currentDelay);

		yield return delay;

		delayToActivateCommandActive = false;

		activateCurrentCommand ();
	}

	void stopDelaytToActivateCommandCoroutine ()
	{
		if (activateCommandCoroutine != null) {
			StopCoroutine (activateCommandCoroutine);
		}

		delayToActivateCommandActive = false;
	}

	void activateCurrentCommand ()
	{ 
		stopCommandCoroutine ();

		commandCoroutine = StartCoroutine (playingCommnadCoroutine (currentCommandInfo.commandDuration));

		if (currentCommandInfo.checkCharacterToReceiveOrdersComponent) {
			if (currentCharacterInfo.isPlayer) {
				currentCharacterInfo.mainTurnBasedCombatActionsSystem.activateOrder (currentCommandInfo.commandName);
			} else {
				currentCharacterInfo.mainTurnBasedCombatActionsSystem.activateOrderToAIBrain (currentCommandInfo.commandName);
			}
		}

		if (currentCommandInfo.useCustomOrderBehavior) {
			currentCommandInfo.mainCustomOrderBehavior.activateOrder (currentCharacterInfo.mainTransform);
		}

		if (currentCommandInfo.useRemoteEvent) {
			for (int j = 0; j < currentCommandInfo.remoteEventNameList.Count; j++) {
				currentCharacterInfo.mainTurnBasedCombatActionsSystem.mainRemoteEventSystem.callRemoteEvent (currentCommandInfo.remoteEventNameList [j]);
			}
		}
	}

	public void cancelCurrentCommand ()
	{
		selectingTargetForCommandActive = false;

		enableOrDisableCurrentCharacterTargetIcon (false);

		if (currentCommandInfo != null) {
			if (currentCommandInfo.showConfirmationPanel) {
				currentCommandInfo.confirmationPanel.SetActive (false);
			}
		}
	}

	bool checkResetCharacterPositionActive;

	public void confirmPassTurn ()
	{
		checkResetCharacterPositionActive = true;

		setNextTurn ();

		checkResetCharacterPositionActive = false;
	}

	IEnumerator playingCommnadCoroutine (float commandDuration)
	{
		playingCommandActive = true;

		WaitForSeconds delay = new WaitForSeconds (commandDuration);

		yield return delay;

		setNextTurn ();
	}

	bool forceToChangeToEnemyTurnActive;

	public void setEnemyTeamTurnDirectly ()
	{
		forceToChangeToEnemyTurnActive = true;

		setNextTurn ();

		forceToChangeToEnemyTurnActive = false;
	}


	bool adjustBothTeamsPositionOnNextTurnActive;

	public void setNextTurn ()
	{
		bool adjustBothTeamPositionsResult = false;

		if (adjustBothTeamsPositionOnNextTurnActive) {
			adjustBothTeamPositionsResult = true;

			adjustBothTeamsPositionOnNextTurnActive = false;
		}

		if (currentTurnForPlayerTeam && !adjustBothTeamPositionsResult) {
			bool resetCharactersPositionResult = false;

			if (currentCommandInfo != null) {
				if (currentCommandInfo.checkResetCharacterPositionAfterCommand) {
					resetCharactersPositionResult = true;
				}
			}

			if (checkResetCharacterPositionActive) {
				resetCharactersPositionResult = true;
			}

			if (resetCharactersPositionResult) {
				if (showDebugPrint) {
					print ("RESET CHARACTER POSITIONS");
				}

				if (!currentCharacterInfo.mainHealth.isDead ()) {
					currentCharacterInfo.mainTurnBasedCombatActionsSystem.resetToCurrrentCombatPositionAndRotation (!resetCharacterPositionsSmoothly);
				}

				if (currentCharacterTargetInfo != null) {
					if (showDebugPrint) {
						print ("RESET TARGET POSITIONS");
					}

					if (!currentCharacterTargetInfo.mainHealth.isDead ()) {
						currentCharacterTargetInfo.mainTurnBasedCombatActionsSystem.resetToCurrrentCombatPositionAndRotation (!resetCharacterPositionsSmoothly);
					}
				}
			}
		} else {
			if (showDebugPrint) {
				print ("reset positions after enemy team attack");
			}

			resetCharacterCombatPositionsOnBothTeams ();
		}

		closeAllPanelInfo ();

		playingCommandActive = false;

		delayToActivateCommandActive = false;

		checkIfAllCharactersPositionAdjustedToShowActionsButtonPanel = true;

		updateAllCharacterStatsUIValue ();

		disableCurrentCommandDescription ();
	}

	public void stopCommandCoroutine ()
	{
		if (commandCoroutine != null) {
			StopCoroutine (commandCoroutine);
		}

		playingCommandActive = false;
	}

	public void runFromCurrentCombat ()
	{
		if (turnCombatActive) {
			if (runFromCombatEnabled) {
				mainTurnBasedCombatSystem.updateLastTimeCombatRunActive (Time.time);

				mainTurnBasedCombatSystem.clearCharactersAround ();

				if (respawnEnemyTeamOnOriginalPositionOnCombatRun) {
					for (int i = 0; i < numberOfCharactersOnEnemyTeam; i++) {
						if (resurrectEnemyTeamOnCombatRun) {
							if (enemyTeamCharacterList [i].mainHealth.isDead ()) {
								enemyTeamCharacterList [i].mainRagdollActivator.setForceQuickGetUpOnCharacterState (true);

								enemyTeamCharacterList [i].mainHealth.resurrectFromExternalCall ();

								enemyTeamCharacterList [i].mainRagdollActivator.setForceQuickGetUpOnCharacterState (false);
							} 
						}

						if (!enemyTeamCharacterList [i].mainHealth.isDead ()) {
							enemyTeamCharacterList [i].mainFindObjectivesSystem.resetAIToOriginalPosition ();

							enemyTeamCharacterList [i].mainFindObjectivesSystem.clearFullEnemiesList ();

							enemyTeamCharacterList [i].mainFindObjectivesSystem.removeCharacterAsTargetOnSameFaction ();

							enemyTeamCharacterList [i].mainFindObjectivesSystem.resetAITargets ();

							enemyTeamCharacterList [i].mainFindObjectivesSystem.removeTargetInfo ();

							if (healEnemyTeamIfRunAway) {
								enemyTeamCharacterList [i].mainHealth.setHealthAmountOnMaxValue ();
							}

							if (holsterWeaponsAfterCombat) {
								GKC_Utils.keepMeleeWeaponGrabbed (enemyTeamCharacterList [i].mainGameObject);

								enemyTeamCharacterList [i].mainPlayerWeaponsManager.checkIfDisableCurrentWeapon ();

								enemyTeamCharacterList [i].mainPlayerWeaponsManager.resetWeaponHandIKWeight ();
							}
						
							if (showDebugPrint) {
								print ("run away from combat, reseting AI target detection info");
							}
						}
					}
				}

				Vector3 positionToRespawn = Vector3.zero;

				if (useRespawnPositionsOnCombatRun) {
					positionToRespawn = mainTurnBasedCombatSystem.getClosestRespawnPosition (mainPlayer.transform.position);
				}

				for (int i = 0; i < numberOfCharactersOnPlayerTeam; i++) {
					if (playerTeamCharacterList [i].mainHealth.isDead ()) {
						if (playerTeamCharacterList [i].isPlayer) {
							playerTeamCharacterList [i].mainRagdollActivator.setForceQuickGetUpOnCharacterState (true);

							playerTeamCharacterList [i].mainRagdollActivator.disabledCheckGetUpPaused ();

							playerTeamCharacterList [i].mainRagdollActivator.setcheckToGetUpState ();
						
							playerTeamCharacterList [i].mainRagdollActivator.setForceQuickGetUpOnCharacterState (false);
						}
					} else {

						if (playerTeamCharacterList [i].isPlayer) {
							if (holsterWeaponsOnPlayerAfterCombat) {
								GKC_Utils.keepMeleeWeaponGrabbed (playerTeamCharacterList [i].mainGameObject);

								playerTeamCharacterList [i].mainPlayerWeaponsManager.checkIfDisableCurrentWeapon ();

								playerTeamCharacterList [i].mainPlayerWeaponsManager.resetWeaponHandIKWeight ();
							}

						} else {
							playerTeamCharacterList [i].mainFindObjectivesSystem.clearFullEnemiesList ();

							playerTeamCharacterList [i].mainFindObjectivesSystem.removeCharacterAsTargetOnSameFaction ();

							playerTeamCharacterList [i].mainFindObjectivesSystem.resetAITargets ();

							playerTeamCharacterList [i].mainFindObjectivesSystem.removeTargetInfo ();

							if (holsterWeaponsAfterCombat) {
								GKC_Utils.keepMeleeWeaponGrabbed (playerTeamCharacterList [i].mainGameObject);

								playerTeamCharacterList [i].mainPlayerWeaponsManager.checkIfDisableCurrentWeapon ();

								playerTeamCharacterList [i].mainPlayerWeaponsManager.resetWeaponHandIKWeight ();
							}
						}

						if (positionToRespawn != Vector3.zero) {
							Vector3 targetPositionToRespawn = positionToRespawn;

							RaycastHit hit;

							Vector3 raycastPosition = targetPositionToRespawn + Vector3.up * 2;

							raycastPosition += maxRadiusToRespawn * Random.insideUnitSphere;

							if (Physics.Raycast (raycastPosition, -Vector3.up, out hit, 200, mainTurnBasedCombatSystem.layerToAdjustToGround)) {
								targetPositionToRespawn = hit.point;
							}

							playerTeamCharacterList [i].mainTransform.position = targetPositionToRespawn;

							playerTeamCharacterList [i].mainPlayerController.getPlayerCameraGameObject ().transform.position = targetPositionToRespawn;
						}
					}
				}

				openOrCloseMenuPanel (false);

				if (!respawnEnemyTeamOnOriginalPositionOnCombatRun) {
					mainTurnBasedCombatSystem.enableWaitTimeToActivateTurnBasedCombat ();
				}

				eventOnRunFromCombat.Invoke ();
			}
		}
	}

	public void finishCurrentCombat ()
	{
		if (turnCombatActive) {
			if (showDebugPrint) {
				print ("finish current combat");
			}

			openOrCloseMenuPanel (false);
		}
	}

	void setCurrentCharacterInfo ()
	{
		if (currentTurnForPlayerTeam) {
			currentCharacterInfo = playerTeamCharacterList [currentTurnCharacterIndex];
		
		} else {
			currentCharacterInfo = enemyTeamCharacterList [currentTurnCharacterIndex];

			enableOrDisableMainActionButtonsPanel (false);
		}
	
		if (showDebugPrint) {
			print ("setCurrentCharacterInfo " + currentCharacterInfo.mainGameObject.name);
		}
			
		GameObject currentCharacterTarget = null;

		if (currentTurnForPlayerTeam) {
			currentCharacterTarget = getCurrentCharacterTarget (enemyTeamCharacterList);

		} else {
			currentCharacterTarget = getCurrentCharacterTarget (playerTeamCharacterList);
		}

		if (showDebugPrint) {
			if (currentCharacterTarget != null) {
				print ("currentCharacterTarget selected" + currentCharacterTarget.name);
			} else {
				print ("WARNING: no currentCharacterTarget found on any team");
			}
		}

		if (currentCharacterInfo.isPlayer) {
			Transform placeToShoot = applyDamage.getPlaceToShoot (currentCharacterTarget);

			if (placeToShoot == null) {
				placeToShoot = currentCharacterTarget.transform;
			}

			currentCharacterInfo.mainPlayerCamera.setTemporalObjectToLookAtTarget (placeToShoot);
		} else {
			currentCharacterInfo.mainFindObjectivesSystem.setCurrentCharacterTurnTarget (currentCharacterTarget);

			currentCharacterInfo.mainMatchPlayerToTargetSystem.clearAllCharactersAround ();

			currentCharacterInfo.mainMatchPlayerToTargetSystem.addCharacterAround (currentCharacterTarget.transform);
		}
			
		currentCharacterInfo.mainFindObjectivesSystem.setIsCurrentCharacterTurn (true);

		currentCharacterInfo.mainTurnBasedCombatActionsSystem.checkEventOnCurrentCharacterTurn ();

		inventoryItemSelected = false;
	}

	GameObject getCurrentCharacterTarget (List<characterInfo> characterList)
	{
		bool selectRandomTarget = currentCharacterInfo.mainTurnBasedCombatActionsSystem.selectRandomTarget;
		bool selectTargetInOrder = currentCharacterInfo.mainTurnBasedCombatActionsSystem.selectTargetInOrder;
		bool selectTargetByCharacterPriority = currentCharacterInfo.mainTurnBasedCombatActionsSystem.selectTargetByCharacterPriority;

		GameObject currentCharacterTarget = null;

		int characterListCount = characterList.Count;

		bool characterFound = false;

		int currentIndex = -1;

		if (selectTargetByCharacterPriority) {
			int highestPriority = -1;

			for (int i = 0; i < characterListCount; i++) {
				if (!characterList [i].mainHealth.isDead ()) {
					if (characterList [i].mainTurnBasedCombatActionsSystem.characterTargetPriority > highestPriority) {
						highestPriority = characterList [i].mainTurnBasedCombatActionsSystem.characterTargetPriority;

						currentIndex = i;

						characterFound = true;
					}
				}
			}
		}

		if (selectTargetInOrder) {
			for (int i = 0; i < characterListCount; i++) {
				if (!characterFound && !characterList [i].mainHealth.isDead ()) {
					characterFound = true;

					currentIndex = i;
				}
			}
		}

		if (selectRandomTarget) {
			int loopCount = 0;

			while (!characterFound) {
				currentIndex = Random.Range (0, characterListCount - 1);
			
				if (!characterList [currentIndex].mainHealth.isDead ()) {
					characterFound = true;
				}

				if (currentIndex >= characterListCount) {
					currentIndex = 0;

					characterFound = true;
				}

				loopCount++;

				if (loopCount > 100) {
					currentIndex = 0;

					characterFound = true;
				}
			}
		}

		if (characterFound) {
			currentCharacterTarget = characterList [currentIndex].mainGameObject;

			currentCharacterTargetInfo = characterList [currentIndex];

			if (showDebugPrint) {
				print ("currnet character target info is " + currentCharacterTargetInfo.mainHealth.getCharacterName ());
			}
		}

		return currentCharacterTarget;
	}

	void setNextCharacterInfo ()
	{
		bool checkCharactersStateResult = true;

		if (loseCombatIfPlayerDiesEnabled && mainPlayerController.isPlayerDead ()) {
			checkCharactersStateResult = false;
		}

		allPlayerTeamDead = true;

		allEnemyTeamDead = true;

		if (checkCharactersStateResult) {
			for (int i = 0; i < numberOfCharactersOnPlayerTeam; i++) {
				if (allPlayerTeamDead && !playerTeamCharacterList [i].mainHealth.isDead ()) {

					playerTeamCharacterList [i].mainFindObjectivesSystem.setIsCurrentCharacterTurn (false);

					allPlayerTeamDead = false;
				}
			}

			for (int i = 0; i < numberOfCharactersOnEnemyTeam; i++) {
				if (allEnemyTeamDead && !enemyTeamCharacterList [i].mainHealth.isDead ()) {
					enemyTeamCharacterList [i].mainFindObjectivesSystem.setIsCurrentCharacterTurn (false);

					allEnemyTeamDead = false;
				}
			}
		} else {
			allPlayerTeamDead = true;
		}

		if (allPlayerTeamDead || allEnemyTeamDead) {
			if (showDebugPrint) {
				if (allEnemyTeamDead) {
					print ("combat result, the player wins ");
				} else {
					print ("combat result, the enemy wins ");
				}
			}

			if (allEnemyTeamDead) {
				eventOnCombatWinned.Invoke ();

				combatWinned = true;
			} else {
				eventOnCombatLost.Invoke ();
			}

			enableOrDisableMainActionButtonsPanel (false);

			combatFinished = true;

			StartCoroutine (showExitCOmbatAfterResultPanelCoroutine ());

			return;
		}
			
		if (showDebugPrint) {
			int numberOfCharactersOnPlayerTeamAlive = getPlayerTeamAliveAmount ();

			int numberOfCharactersOnEnemyTeamAlive = getEnemyTeamAliveAmount ();

			print ("player team alive amount " + numberOfCharactersOnPlayerTeamAlive + " enemy team alive amount " + numberOfCharactersOnEnemyTeamAlive);
		}

		bool currentTurnChangedToOppositeTeamResult = currentTurnForPlayerTeam;

		currentTurnCharacterIndex++;

		if (currentTurnForPlayerTeam) {
			if (currentTurnCharacterIndex >= numberOfCharactersOnPlayerTeam) {
				currentTurnCharacterIndex = 0;

				currentTurnForPlayerTeam = false;
			} else {
				bool nextCharacterFound = false;

				for (int i = currentTurnCharacterIndex; i < numberOfCharactersOnPlayerTeam; i++) {
					if (!playerTeamCharacterList [i].mainHealth.isDead ()) {
						nextCharacterFound = true;
					}
				}

				if (!nextCharacterFound) {
					currentTurnCharacterIndex = 0;

					currentTurnForPlayerTeam = false;
				}
			}
		} else {
			if (currentTurnCharacterIndex >= numberOfCharactersOnEnemyTeam) {
				currentTurnCharacterIndex = 0;

				currentTurnForPlayerTeam = true;
			} else {
				bool nextCharacterFound = false;

				for (int i = currentTurnCharacterIndex; i < numberOfCharactersOnEnemyTeam; i++) {
					if (!enemyTeamCharacterList [i].mainHealth.isDead ()) {
						nextCharacterFound = true;
					}
				}

				if (!nextCharacterFound) {
					currentTurnCharacterIndex = 0;

					currentTurnForPlayerTeam = true;
				}
			}
		}

		if (forceToChangeToEnemyTurnActive) {
			currentTurnCharacterIndex = 0;

			currentTurnForPlayerTeam = false;
		}

		if (showDebugPrint) {
			if (currentTurnForPlayerTeam) {
				print ("setting next character info and turn for player team " + currentTurnCharacterIndex);
			} else {
				print ("setting next character info and turn for enemy team " + currentTurnCharacterIndex);
			}
		}

		if (currentTurnForPlayerTeam) {
			for (int i = 0; i < numberOfCharactersOnPlayerTeam; i++) {
				playerTeamCharacterList [i].mainTurnBasedCombatActionsSystem.checkEventOnEachTurn ();

			}
		} else {
			for (int i = 0; i < numberOfCharactersOnEnemyTeam; i++) {
				enemyTeamCharacterList [i].mainTurnBasedCombatActionsSystem.checkEventOnEachTurn ();
			}
		}

		if (currentTurnChangedToOppositeTeamResult != currentTurnForPlayerTeam) {
			if (showDebugPrint) {
				print ("turn changed for the opposite team");

				if (currentTurnForPlayerTeam) {
					for (int i = 0; i < numberOfCharactersOnPlayerTeam; i++) {
						playerTeamCharacterList [i].mainTurnBasedCombatActionsSystem.checkEventOnCurrentTeamTurn ();

					}
				} else {
					for (int i = 0; i < numberOfCharactersOnEnemyTeam; i++) {
						enemyTeamCharacterList [i].mainTurnBasedCombatActionsSystem.checkEventOnCurrentTeamTurn ();
					}
				}
			}
		}

		if (currentTurnForPlayerTeam) {

			bool newIndexFound = false;

			int loopCount = 0;

			int temporalIndex = currentTurnCharacterIndex;

			while (!newIndexFound) {

				if (!playerTeamCharacterList [temporalIndex].mainHealth.isDead ()) {
					newIndexFound = true;

					currentTurnCharacterIndex = temporalIndex;
				} else {

					temporalIndex++;

					if (temporalIndex >= numberOfCharactersOnPlayerTeam) {
						temporalIndex = 0;
					}

					loopCount++;

					if (loopCount > 100) {
						newIndexFound = true;
					}
				}
			}
		} else {
			bool newIndexFound = false;

			int loopCount = 0;

			int temporalIndex = currentTurnCharacterIndex;

			while (!newIndexFound) {

				if (!enemyTeamCharacterList [temporalIndex].mainHealth.isDead ()) {
					newIndexFound = true;

					currentTurnCharacterIndex = temporalIndex;
				} else {

					temporalIndex++;

					if (temporalIndex >= numberOfCharactersOnEnemyTeam) {
						temporalIndex = 0;
					}

					loopCount++;

					if (loopCount > 100) {
						newIndexFound = true;
					}
				}
			}
		}


		setCurrentCharacterInfo ();

		if (currentTurnForPlayerTeam) {
			updateMainActionButtonsPanelToCharacter ();
		} else {
			enableOrDisableMainActionButtonsPanel (false);
		}

		updateCurrentCharacterIcon ();

		currentCharacterInfo.mainTurnBasedCombatActionsSystem.checkEventsOnCharacterCurrentTurnStart ();
	}

	IEnumerator showExitCOmbatAfterResultPanelCoroutine ()
	{
		WaitForSeconds delay = new WaitForSeconds (minWaitToShowExitCombatAfterResult);

		yield return delay;

		enableOrDisabeExitCombatAfterResultPanel (true);
	}

	public void enableOrDisabeExitCombatAfterResultPanel (bool state)
	{
		if (exitCombatAfterResultPanel.activeSelf != state) {
			exitCombatAfterResultPanel.SetActive (state);
		}
	}

	public void exitFromCombatAfterWinningOrLoosing ()
	{
		if (combatFinished) {
			mainTurnBasedCombatSystem.clearCharactersAround ();

			if (allPlayerTeamDead) {
				playerTurnBasedCombatTeamInfo.checkEventsOnTeamDefeated ();

				List<GameObject> enemyGameObjectTeamList = new List<GameObject> ();

				for (int i = 0; i < numberOfCharactersOnEnemyTeam; i++) {
					enemyGameObjectTeamList.Add (enemyTeamCharacterList [i].mainGameObject);
				}

				playerTurnBasedCombatTeamInfo.checkEventOnRewardForOpponentTeam (enemyGameObjectTeamList);



				Vector3 positionToRespawn = mainTurnBasedCombatSystem.getClosestRespawnPosition (mainPlayer.transform.position);

				for (int i = 0; i < numberOfCharactersOnPlayerTeam; i++) {
					playerTeamCharacterList [i].mainRagdollActivator.setForceQuickGetUpOnCharacterState (true);

					playerTeamCharacterList [i].mainRagdollActivator.disabledCheckGetUpPaused ();

					if (playerTeamCharacterList [i].isPlayer) {
						playerTeamCharacterList [i].mainRagdollActivator.setcheckToGetUpState ();
					} else {
						playerTeamCharacterList [i].mainHealth.resurrectFromExternalCall ();
					}

					playerTeamCharacterList [i].mainRagdollActivator.setForceQuickGetUpOnCharacterState (false);

					Vector3 targetPositionToRespawn = positionToRespawn;

					RaycastHit hit;

					Vector3 raycastPosition = targetPositionToRespawn + Vector3.up * 2;

					raycastPosition += maxRadiusToRespawn * Random.insideUnitSphere;

					if (Physics.Raycast (raycastPosition, -Vector3.up, out hit, 200, mainTurnBasedCombatSystem.layerToAdjustToGround)) {
						targetPositionToRespawn = hit.point;
					}

					playerTeamCharacterList [i].mainTransform.position = targetPositionToRespawn;

					playerTeamCharacterList [i].mainPlayerController.getPlayerCameraGameObject ().transform.position = targetPositionToRespawn;
				}


				if (respawnEnemyTeamOnOriginalPositionOnCombatLost && !combatWinned) {
					for (int i = 0; i < numberOfCharactersOnEnemyTeam; i++) {
						if (enemyTeamCharacterList [i].mainHealth.isDead ()) {
							enemyTeamCharacterList [i].mainRagdollActivator.setForceQuickGetUpOnCharacterState (true);

							enemyTeamCharacterList [i].mainHealth.resurrectFromExternalCall ();

							enemyTeamCharacterList [i].mainRagdollActivator.setForceQuickGetUpOnCharacterState (false);
						} else {
							enemyTeamCharacterList [i].mainHealth.setHealthAmountOnMaxValue ();
						}

						enemyTeamCharacterList [i].mainFindObjectivesSystem.resetAIToOriginalPosition ();

						enemyTeamCharacterList [i].mainFindObjectivesSystem.clearFullEnemiesList ();

						enemyTeamCharacterList [i].mainFindObjectivesSystem.removeCharacterAsTargetOnSameFaction ();

						enemyTeamCharacterList [i].mainFindObjectivesSystem.resetAITargets ();

						enemyTeamCharacterList [i].mainFindObjectivesSystem.removeTargetInfo ();

						if (holsterWeaponsAfterCombat) {
							GKC_Utils.keepMeleeWeaponGrabbed (enemyTeamCharacterList [i].mainGameObject);

							enemyTeamCharacterList [i].mainPlayerWeaponsManager.checkIfDisableCurrentWeapon ();

							enemyTeamCharacterList [i].mainPlayerWeaponsManager.resetWeaponHandIKWeight ();
						}
					}
				}
			} else {
				if (combatWinned) {
					if (showDebugPrint) {
						print ("combat winned, checking if some member team needs to be resurrected");
					}

					for (int i = 0; i < numberOfCharactersOnPlayerTeam; i++) {
						if (playerTeamCharacterList [i].mainHealth.isDead ()) {
							playerTeamCharacterList [i].mainRagdollActivator.setForceQuickGetUpOnCharacterState (true);

							playerTeamCharacterList [i].mainRagdollActivator.disabledCheckGetUpPaused ();

							if (playerTeamCharacterList [i].isPlayer) {
								playerTeamCharacterList [i].mainRagdollActivator.setcheckToGetUpState ();
							} else {
								playerTeamCharacterList [i].mainHealth.resurrectFromExternalCall ();
							}

							playerTeamCharacterList [i].mainRagdollActivator.setForceQuickGetUpOnCharacterState (false);
						}

						if (playerTeamCharacterList [i].isPlayer) {
							if (holsterWeaponsOnPlayerAfterCombat) {
								GKC_Utils.keepMeleeWeaponGrabbed (playerTeamCharacterList [i].mainGameObject);

								playerTeamCharacterList [i].mainPlayerWeaponsManager.checkIfDisableCurrentWeapon ();

								playerTeamCharacterList [i].mainPlayerWeaponsManager.resetWeaponHandIKWeight ();
							}
						} else {
							playerTeamCharacterList [i].mainFindObjectivesSystem.clearFullEnemiesList ();

							playerTeamCharacterList [i].mainFindObjectivesSystem.removeCharacterAsTargetOnSameFaction ();

							playerTeamCharacterList [i].mainFindObjectivesSystem.resetAITargets ();

							playerTeamCharacterList [i].mainFindObjectivesSystem.removeTargetInfo ();

							if (holsterWeaponsAfterCombat) {
								GKC_Utils.keepMeleeWeaponGrabbed (playerTeamCharacterList [i].mainGameObject);

								playerTeamCharacterList [i].mainPlayerWeaponsManager.checkIfDisableCurrentWeapon ();

								playerTeamCharacterList [i].mainPlayerWeaponsManager.resetWeaponHandIKWeight ();
							}
						}
					}
				}
			}

			if (allEnemyTeamDead) {
				enemyTurnBasedCombatTeamInfo.checkEventsOnTeamDefeated ();

				List<GameObject> playerGameObjectTeamList = new List<GameObject> ();

				for (int i = 0; i < numberOfCharactersOnPlayerTeam; i++) {
					playerGameObjectTeamList.Add (playerTeamCharacterList [i].mainGameObject);
				}

				enemyTurnBasedCombatTeamInfo.checkEventOnRewardForOpponentTeam (playerGameObjectTeamList);
			}

			finishCurrentCombat ();

			turnCombatActive = true;

			freeCombatActive = false;

			mainTurnBasedCombatSystem.setFreeCombatActiveState (freeCombatActive);
		}
	}

	//OPEN AND CLOSE MENU AND INITIALIZE TEAMS INFO
	public override void initializeMenuPanel ()
	{
		if (mainTurnBasedCombatSystem == null) {
			mainTurnBasedCombatSystemAssigned = mainTurnBasedCombatSystem != null;

			if (!mainTurnBasedCombatSystemAssigned) {
				mainTurnBasedCombatSystem = turnBasedCombatSystem.Instance;

				mainTurnBasedCombatSystemAssigned = mainTurnBasedCombatSystem != null;
			}

			if (!mainTurnBasedCombatSystemAssigned) {
				GKC_Utils.instantiateMainManagerOnSceneWithTypeOnApplicationPlaying (menuPanelName, typeof(turnBasedCombatSystem), true);

				mainTurnBasedCombatSystem = turnBasedCombatSystem.Instance;

				mainTurnBasedCombatSystemAssigned = mainTurnBasedCombatSystem != null;
			}

			if (!mainTurnBasedCombatSystemAssigned) {

				mainTurnBasedCombatSystem = FindObjectOfType<turnBasedCombatSystem> ();

				mainTurnBasedCombatSystemAssigned = mainTurnBasedCombatSystem != null;
			} 

			checkCharacterComponents ();
		}
	}

	public void checkMainPlayerStateBeforeActivateCombat ()
	{
		checkCharacterComponents ();

		if (mainPlayerController.isPlayerDriving ()) {
			GameObject currentVehicle = mainPlayerController.getCurrentVehicle ();

			if (currentVehicle != null) {
				Vector3 closestSpawnPosition = mainTurnBasedCombatSystem.getClosestRespawnPosition (currentVehicle.transform.position);

				if (closestSpawnPosition != Vector3.zero) {
					currentVehicle.transform.position = closestSpawnPosition;
				}
			}

			applyDamage.activateGetOffFromVehicleIfDriving (mainPlayerController.gameObject);
		}

		if (mainPlayerController.isPlayerOnFirstPerson () || mainPlayerController.isFullBodyAwarenessActive ()) {
			mainPlayerCamera.changeCameraToThirdOrFirstView ();
		}
	}

	void checkCharacterComponents ()
	{
		if (!componentsAssigned) {
			if (pauseManager != null) {
				mainPlayer = pauseManager.getPlayerControllerGameObject ();

				mainPlayerComponentsManager = mainPlayer.GetComponent<playerComponentsManager> ();

				if (mainPlayerComponentsManager != null) {
					mainPlayerController = mainPlayerComponentsManager.getPlayerController ();

					mainPlayerCamera = mainPlayerComponentsManager.getPlayerCamera ();

					mainUsingDevicesSystem = mainPlayerComponentsManager.getUsingDevicesSystem ();

					mainCamera = mainPlayerCamera.getMainCamera ();

					mainPlayerInputManager = mainPlayerComponentsManager.getPlayerInputManager ();

					mainFriendListManager = mainPlayerComponentsManager.getFriendListManager ();

					mainInventoryManager = mainPlayerComponentsManager.getInventoryManager ();
				}
			}

			mainCanvasSizeDelta = mainPlayerCamera.getMainCanvasSizeDelta ();
			halfMainCanvasSizeDelta = mainCanvasSizeDelta * 0.5f;
			usingScreenSpaceCamera = mainPlayerCamera.isUsingScreenSpaceCamera ();

			componentsAssigned = true;
		}
	}

	public bool isCombatActive ()
	{
		return combatActive;
	}

	public override void openOrCloseMenuPanel (bool state)
	{
		base.openOrCloseMenuPanel (state);

		menuOpened = state;

		combatActive = state;

		initialPositionMainActionButtonsPanelAdjusted = false;

		lastTimeCombatActived = Time.time;

		checkCharacterComponents ();

		stopMenuCoroutineUpdate ();

		touchPlatform = touchJoystick.checkTouchPlatform ();

		bool checkTouchControlsResult = pauseManager.isUsingTouchControls ();

		if (toggleCombatModeTouchButton != null) {
			toggleCombatModeTouchButton.SetActive (checkTouchControlsResult);
		}

		if (inputActionsPanel != null) {
			inputActionsPanel.SetActive (!checkTouchControlsResult);
		}

		checkInputListToPauseDuringAction (customInputToPauseOnActionInfoList, state);

		if (turnCombatActive) {
			mainPlayerInputManager.setPlayerInputMultiAxesState (state, mainTurnBasedCombatInputName);
		}

		mainPlayerInputManager.setIgnoreEnablePlayerInputMultiAxesActiveState (state);

		pauseManager.setIgnoreDeathStateOnMenuPauseState (state);

		pauseManager.setIgnoreCheckCloseIngameMenusIfOpenedState (state);

		if (state) {
			if (mainCanvasScaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize) {
				horizontaResolution = mainCanvasScaler.referenceResolution.x;
			} else {
				currentResolution = GKC_Utils.getScreenResolution ();

				horizontaResolution = currentResolution.x;
			}

			allPlayerTeamDead = false;

			allEnemyTeamDead = false;

			combatFinished = false;

			combatWinned = false;

			enableOrDisabeExitCombatAfterResultPanel (false);

			for (int i = 0; i < panelCategoryInfoList.Count; i++) {
				for (int j = 0; j < panelCategoryInfoList [i].panelInfoList.Count; j++) {
					bool panelButtonEnabled = panelCategoryInfoList [i].panelInfoList [j].panelButtonEnabled;

					if (panelCategoryInfoList [i].panelInfoList [j].panelButton.activeSelf != panelButtonEnabled) {
						panelCategoryInfoList [i].panelInfoList [j].panelButton.SetActive (panelButtonEnabled);
					}

					panelCategoryInfoList [i].panelInfoList [j].isCurrentPanel = false;

					if (panelCategoryInfoList [i].panelInfoList [j].showConfirmationPanel) {
						if (panelCategoryInfoList [i].panelInfoList [j].confirmationPanel.activeSelf) {
							panelCategoryInfoList [i].panelInfoList [j].confirmationPanel.SetActive (false);
						}

						if (panelCategoryInfoList [i].panelInfoList [j].originalCofirmationPanelPosition == Vector3.zero) {
							panelCategoryInfoList [i].panelInfoList [j].originalCofirmationPanelPosition = panelCategoryInfoList [i].panelInfoList [j].confirmationPanel.transform.localPosition;
						}
					}

					if (panelCategoryInfoList [i].panelInfoList [j].originalPanelPosition == Vector3.zero) {
						panelCategoryInfoList [i].panelInfoList [j].originalPanelPosition = panelCategoryInfoList [i].panelInfoList [j].panelObject.transform.localPosition;
					}
				}
			}

			closeAllCommandConfirmationPanels ();

			enableOrDisableMainActionButtonsPanel (false);

			enableOrDisableCurrentCharacterIcon (false);

			enableOrDisableCurrentCharacterTargetIcon (false);
		} else {
			closeAllPanelInfo ();

			resetInputListToPauseDuringAction ();
		}

		if (state) {
			menuCoroutine = StartCoroutine (menuCoroutineUpdate ());

			mainTurnBasedCombatSystem.adjustTurnBasedCombatPositionToPlayer ();

			mainTurnBasedCombatSystem.updateLastTimeCombatRunActive (-1);
		}

		if (turnCombatActive) {
			setCharacterTeamsInfo ();
		} else {
			if (!state) {
				setCharactersTeamState (false, playerTeamCharacterList, "", true);

				setCharactersTeamState (false, enemyTeamCharacterList, "", false);
			}
		}

		if (mainTurnBasedCombatSystem != null) {
			if (state) {
				mainTurnBasedCombatSystem.setUseCustomCameraState (useCustomCameraState, customCameraState);
			}

			if (numberOfCharactersOnPlayerTeam > numberOfCharactersOnEnemyTeam) {
				mainTurnBasedCombatSystem.setMinTeamAmountToCheck (numberOfCharactersOnPlayerTeam);
			} else {
				mainTurnBasedCombatSystem.setMinTeamAmountToCheck (numberOfCharactersOnEnemyTeam);
			}
				
			mainTurnBasedCombatSystem.setTurnBasedCombatActiveState (state);
		}

		if (pauseRestOfAIOnSceneDuringCombat) {
			GKC_Utils.pauseOrResumeAIOnSceneWithExceptionList (state, 0, fullCharacterOnCombatList);
		}

		disableCurrentCommandDescription ();

		inventoryItemSelected = false;
	}

	public void setCharacterTeamsInfo ()
	{
		if (menuOpened) {
			playerTeamCharacterList.Clear ();

			enemyTeamCharacterList.Clear ();

			int turnBasedCombatChararcterUIInfoListCount = turnBasedCombatChararcterUIInfoList.Count;

			for (int i = 0; i < turnBasedCombatChararcterUIInfoListCount; i++) {
				if (turnBasedCombatChararcterUIInfoList [i].characterUIGameObject.activeSelf) {
					turnBasedCombatChararcterUIInfoList [i].characterUIGameObject.SetActive (false);
				}
			}

			playerTeamPositionDataName = "";
			enemyTeamPositionDataName = "";

			playerComponentsManager currentPlayerComponentsManager = mainPlayer.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
				playerTurnBasedCombatTeamInfo = currentPlayerComponentsManager.getTurnBasedCombatTeamInfo ();

				playerTurnBasedCombatTeamInfo.removeDeadCharactersFromTeam ();

				List<GameObject> characterTeamList = playerTurnBasedCombatTeamInfo.getCharacterTeamList ();

				if (characterTeamList.Count == 1) {
					if (mainFriendListManager.anyFriendOnList ()) {
						characterTeamList = mainFriendListManager.getAllFriendList ();

						characterTeamList.Insert (0, mainPlayer);
					}
				}

				for (int i = 0; i < characterTeamList.Count; i++) {
					if (characterTeamList [i] != null) {
						characterInfo newCharacterInfo = new characterInfo ();

						newCharacterInfo.isPlayer = characterTeamList [i] == mainPlayer;

						newCharacterInfo.mainGameObject = characterTeamList [i];

						newCharacterInfo.mainTransform = newCharacterInfo.mainGameObject.transform;

						currentPlayerComponentsManager = newCharacterInfo.mainGameObject.GetComponent<playerComponentsManager> ();

						newCharacterInfo.mainHealth = currentPlayerComponentsManager.getHealth ();

						newCharacterInfo.mainTurnBasedCombatActionsSystem = currentPlayerComponentsManager.getTurnBasedCombatActionsSystem ();

						newCharacterInfo.mainFindObjectivesSystem = currentPlayerComponentsManager.getFindObjectivesSystem ();

						newCharacterInfo.mainPlayerController = currentPlayerComponentsManager.getPlayerController ();

						newCharacterInfo.mainPlayerWeaponsManager = currentPlayerComponentsManager.getPlayerWeaponsManager ();

						newCharacterInfo.mainPlayerCamera = currentPlayerComponentsManager.getPlayerCamera ();

						newCharacterInfo.mainMenuPause = currentPlayerComponentsManager.getPauseManager ();

						newCharacterInfo.mainRagdollActivator = currentPlayerComponentsManager.getRagdollActivator ();

						newCharacterInfo.mainHeadTrack = currentPlayerComponentsManager.getHeadTrack ();

						newCharacterInfo.statesManager = currentPlayerComponentsManager.getPlayerStatesManager ();

						newCharacterInfo.mainAINavMesh = newCharacterInfo.mainFindObjectivesSystem.getAINavMesh ();

						newCharacterInfo.mainPlayerStatsSystem = currentPlayerComponentsManager.getPlayerStatsSystem ();

						newCharacterInfo.previousCharacterParent = newCharacterInfo.mainTransform.parent;

						newCharacterInfo.mainMatchPlayerToTargetSystem = currentPlayerComponentsManager.getMatchPlayerToTargetSystem ();

						newCharacterInfo.mainPlayerActionSystem = currentPlayerComponentsManager.getPlayerActionSystem ();

						addNewTurnBasedCombatChararcterUIInfo (newCharacterInfo.mainGameObject, 
							playerTeamStatsUIPrefab,
							newCharacterInfo.mainHealth.getCharacterName (),
							playerTeamCharacterList.Count, 
							playerTeamHealthSliderParent, 
							newCharacterInfo);

						playerTeamCharacterList.Add (newCharacterInfo);
					}
				}

				currentTurnForPlayerTeam = true;

				if (playerTurnBasedCombatTeamInfo.useTeamPositionDataName) {
					playerTeamPositionDataName = playerTurnBasedCombatTeamInfo.teamPositionDataName;
				}
			}

			List<Transform> charactersAround = mainTurnBasedCombatSystem.mainAIAroundManager.getCharactersAround ();

			currentPlayerComponentsManager = null;

			enemyTurnBasedCombatTeamInfo = null;

			bool enemyTeamLeaderFound = false;

			for (int i = 0; i < charactersAround.Count; i++) {
				if (!enemyTeamLeaderFound) {
					currentPlayerComponentsManager = charactersAround [i].GetComponent<playerComponentsManager> ();

					enemyTurnBasedCombatTeamInfo = currentPlayerComponentsManager.getTurnBasedCombatTeamInfo ();

					enemyTurnBasedCombatTeamInfo.removeDeadCharactersFromTeam ();

					List<GameObject> characterTeamList = enemyTurnBasedCombatTeamInfo.getCharacterTeamList ();

					for (int j = 0; j < characterTeamList.Count; j++) {
						if (!enemyTeamLeaderFound) {
							if (characterTeamList [j] != null) {
								currentPlayerComponentsManager = characterTeamList [j].GetComponent<playerComponentsManager> ();

								enemyTurnBasedCombatTeamInfo = currentPlayerComponentsManager.getTurnBasedCombatTeamInfo ();

								if (enemyTurnBasedCombatTeamInfo.isTeamLeaderValue ()) {
									enemyTeamLeaderFound = true;
								}
							}
						}
					}
				}
			}

			if (enemyTeamLeaderFound) {
				enemyTurnBasedCombatTeamInfo.removeDeadCharactersFromTeam ();

				List<GameObject> characterTeamList = enemyTurnBasedCombatTeamInfo.getCharacterTeamList ();

				for (int i = 0; i < characterTeamList.Count; i++) {
					if (characterTeamList [i] != null) {
						characterInfo newCharacterInfo = new characterInfo ();

						newCharacterInfo.mainGameObject = characterTeamList [i];

						newCharacterInfo.mainTransform = newCharacterInfo.mainGameObject.transform;

						currentPlayerComponentsManager = newCharacterInfo.mainGameObject.GetComponent<playerComponentsManager> ();

						newCharacterInfo.mainHealth = currentPlayerComponentsManager.getHealth ();

						newCharacterInfo.mainTurnBasedCombatActionsSystem = currentPlayerComponentsManager.getTurnBasedCombatActionsSystem ();

						newCharacterInfo.mainFindObjectivesSystem = currentPlayerComponentsManager.getFindObjectivesSystem ();

						newCharacterInfo.mainPlayerController = currentPlayerComponentsManager.getPlayerController ();

						newCharacterInfo.mainPlayerWeaponsManager = currentPlayerComponentsManager.getPlayerWeaponsManager ();

						newCharacterInfo.mainPlayerCamera = currentPlayerComponentsManager.getPlayerCamera ();

						newCharacterInfo.mainRagdollActivator = currentPlayerComponentsManager.getRagdollActivator ();

						newCharacterInfo.mainHeadTrack = currentPlayerComponentsManager.getHeadTrack ();

						newCharacterInfo.statesManager = currentPlayerComponentsManager.getPlayerStatesManager ();

						newCharacterInfo.mainAINavMesh = newCharacterInfo.mainFindObjectivesSystem.getAINavMesh ();

						newCharacterInfo.mainPlayerStatsSystem = currentPlayerComponentsManager.getPlayerStatsSystem ();

						newCharacterInfo.previousCharacterParent = newCharacterInfo.mainTransform.parent;

						newCharacterInfo.mainMatchPlayerToTargetSystem = currentPlayerComponentsManager.getMatchPlayerToTargetSystem ();

						newCharacterInfo.mainPlayerActionSystem = currentPlayerComponentsManager.getPlayerActionSystem ();

						addNewTurnBasedCombatChararcterUIInfo (newCharacterInfo.mainGameObject, 
							enemyTeamStatsUIPrefab, 
							newCharacterInfo.mainHealth.getCharacterName (), 
							(playerTeamCharacterList.Count + enemyTeamCharacterList.Count), 
							eneyTeamHealthSliderParent, 
							newCharacterInfo);

						enemyTeamCharacterList.Add (newCharacterInfo);
					}
				}

				if (enemyTurnBasedCombatTeamInfo.thisTeamAlwaysSelectFirst) {
					currentTurnForPlayerTeam = false;
				}

				if (enemyTurnBasedCombatTeamInfo.useTeamPositionDataName) {
					enemyTeamPositionDataName = enemyTurnBasedCombatTeamInfo.teamPositionDataName;
				}

				useCustomCameraState = enemyTurnBasedCombatTeamInfo.useCustomCameraState;
				customCameraState = enemyTurnBasedCombatTeamInfo.customCameraState;
			} else {
				if (showDebugPrint) {
					print ("WARNING: ENEMY TEAM INFO COMPONENT NOT LOCATED ON ANY OF THE CHARACTERS AROUND");
				}
			}

			numberOfCharactersOnPlayerTeam = playerTeamCharacterList.Count;

			numberOfCharactersOnEnemyTeam = enemyTeamCharacterList.Count;

			currentTurnCharacterIndex = 0;

			setCharactersTeamState (true, playerTeamCharacterList, playerTeamPositionDataName, true);

			setCharactersTeamState (true, enemyTeamCharacterList, enemyTeamPositionDataName, false);

			setCurrentCharacterInfo ();


			fullCharacterOnCombatList.Clear ();

			turnBasedCombatChararcterUIInfoListCount = turnBasedCombatChararcterUIInfoList.Count;

			for (int i = 0; i < turnBasedCombatChararcterUIInfoListCount; i++) {
				fullCharacterOnCombatList.Add (turnBasedCombatChararcterUIInfoList [i].characterUIOwner);
			}
		} else {
			setCharactersTeamState (false, playerTeamCharacterList, "", true);

			setCharactersTeamState (false, enemyTeamCharacterList, "", false);
		}
	}

	void setCharactersTeamState (bool state, List<characterInfo> characterList, string teamPositionDataName, bool isPlayerTeam)
	{
		int characterListCount = characterList.Count;

		turnBasedCombatTeamPositionsInfoData currentTurnBasedCombatTeamPositionsInfoData = null;

		bool currentTurnBasedCombatTeamPositionsInfoDataFound = false;

		int currenCharacterPositionDataIndex = 0;

		if (teamPositionDataName != "") {
			currentTurnBasedCombatTeamPositionsInfoData = 
				mainTurnBasedCombatSystem.getTurnBasedCombatTeamPositionsInfoData (characterListCount, teamPositionDataName, isPlayerTeam);
		} else {
			currentTurnBasedCombatTeamPositionsInfoData = 
				mainTurnBasedCombatSystem.getTurnBasedCombatTeamPositionsInfoData (characterListCount, "", isPlayerTeam);
		}

		currentTurnBasedCombatTeamPositionsInfoDataFound = currentTurnBasedCombatTeamPositionsInfoData != null;

		if (showDebugPrint) {
			print ("currentTurnBasedCombatTeamPositionsInfoDataFound " + currentTurnBasedCombatTeamPositionsInfoDataFound + " " + characterListCount + " " + teamPositionDataName);
		}

		for (int i = 0; i < characterListCount; i++) {
			characterInfo newCharacterInfo = characterList [i];

			setSingleCharacterTeamState (state, newCharacterInfo, isPlayerTeam, false);

			if (state) {
				if (currentTurnBasedCombatTeamPositionsInfoDataFound) {
					if (currentTurnBasedCombatTeamPositionsInfoData.turnBasedCombatCharacterPositionsInfoList.Count > currenCharacterPositionDataIndex) {
						turnBasedCombatTeamPositionsInfoData.turnBasedCombatCharacterPositionsInfo currentturnBasedCombatCharacterPositionsInfo =
							currentTurnBasedCombatTeamPositionsInfoData.turnBasedCombatCharacterPositionsInfoList [currenCharacterPositionDataIndex];

						newCharacterInfo.mainTurnBasedCombatActionsSystem.setCurrentCombatPositionAndRotationValues (currentturnBasedCombatCharacterPositionsInfo.positionValue, 
							currentturnBasedCombatCharacterPositionsInfo.rotationValue);

						if (resetCharacterPositionsSmoothlyOnCombatStart) {
							newCharacterInfo.mainTurnBasedCombatActionsSystem.activateCharacterMovement (currentturnBasedCombatCharacterPositionsInfo.positionValue, 
								currentturnBasedCombatCharacterPositionsInfo.rotationValue, true);
						} else {
							newCharacterInfo.mainTurnBasedCombatActionsSystem.setCharacterPositionAtonce (currentturnBasedCombatCharacterPositionsInfo.positionValue, 
								currentturnBasedCombatCharacterPositionsInfo.rotationValue, true);
						}
					}
				}

				currenCharacterPositionDataIndex++;
			} 
				
			if (newCharacterInfo.mainTurnBasedCombatActionsSystem.addAmountToCharacterStatsOnCombat) {
				for (int j = 0; j < newCharacterInfo.mainTurnBasedCombatActionsSystem.simpleStatInfoList.Count; j++) {
					if (state) {
						newCharacterInfo.mainPlayerStatsSystem.increasePlayerStat (
							newCharacterInfo.mainTurnBasedCombatActionsSystem.simpleStatInfoList [j].statName, 
							newCharacterInfo.mainTurnBasedCombatActionsSystem.simpleStatInfoList [j].statAmountToAdd);
					} else {
						newCharacterInfo.mainPlayerStatsSystem.increasePlayerStat (
							newCharacterInfo.mainTurnBasedCombatActionsSystem.simpleStatInfoList [j].statName, 
							-newCharacterInfo.mainTurnBasedCombatActionsSystem.simpleStatInfoList [j].statAmountToAdd);
					}
				}
			}
		}
	}

	//MORE UI ELEMENTS

	void closeAllCommandConfirmationPanels ()
	{
		int commandCategoryInfoListCount = commandCategoryInfoList.Count;

		for (int i = 0; i < commandCategoryInfoListCount; i++) {

			int commandInfoListCount = commandCategoryInfoList [i].commandInfoList.Count;

			for (int j = 0; j < commandInfoListCount; j++) {
				if (commandCategoryInfoList [i].commandInfoList [j].showConfirmationPanel) {
					if (commandCategoryInfoList [i].commandInfoList [j].confirmationPanel.activeSelf) {
						commandCategoryInfoList [i].commandInfoList [j].confirmationPanel.SetActive (false);
					}

					if (commandCategoryInfoList [i].commandInfoList [j].originalConfirmationPanelPosition == Vector3.zero) {
						commandCategoryInfoList [i].commandInfoList [j].originalConfirmationPanelPosition = commandCategoryInfoList [i].commandInfoList [j].confirmationPanel.transform.localPosition;
					}
				}
			}
		}
	}

	public void openPanelInfo (GameObject buttonObject)
	{
		closeAllCommandConfirmationPanels ();

		int panelCategoryIndex = -1;
		int panelIndex = -1;

		for (int i = 0; i < panelCategoryInfoList.Count; i++) {
			for (int j = 0; j < panelCategoryInfoList [i].panelInfoList.Count; j++) {
				if (panelCategoryInfoList [i].panelCategoryEnabled) {
					if (panelCategoryInfoList [i].panelInfoList [j].panelButton == buttonObject) {

						panelIndex = j;

						panelCategoryIndex = i;
					}

					if (panelCategoryInfoList [i].panelInfoList [j].showConfirmationPanel) {
						if (panelCategoryInfoList [i].panelInfoList [j].confirmationPanel.activeSelf) {
							panelCategoryInfoList [i].panelInfoList [j].confirmationPanel.SetActive (false);
						}
					}
				}
			}
		}

		if (panelIndex == -1 || panelCategoryIndex == -1) {
			return;
		}

		closeAllPanelInfo ();

		currentPanelInfo = panelCategoryInfoList [panelCategoryIndex].panelInfoList [panelIndex];

		if (currentPanelInfo.showConfirmationPanel) {
			currentPanelInfo.confirmationPanel.SetActive (true);

			if (currentPanelInfo.checkRightOrLeftSideForConfirmationPanel) {
				RectTransform currentPanelObjectRectTransform = currentPanelInfo.confirmationPanel.GetComponent<RectTransform> ();

				Vector3 currentCharacterPosition = currentCharacterInfo.mainTransform.position;

				Vector3 screenPoint = mainCamera.WorldToScreenPoint (currentCharacterPosition);

				float currentHorizontalPosition = screenPoint.x;

				showPanelObjectOnRightOrLeftSideScreen = true;

				if (horizontaResolution - currentHorizontalPosition < horizontalLimitOnRightScreenCategoryPanel) {
					showPanelObjectOnRightOrLeftSideScreen = false;
				}

				if (showDebugPrint) {
					print ("currentHorizontalPosition " + horizontaResolution + " " + currentHorizontalPosition);
				}

				if (showPanelObjectOnRightOrLeftSideScreen) {
					currentPanelObjectRectTransform.position = currentPanelInfo.rightSideConfirmationPanel.position;
				} else {
					currentPanelObjectRectTransform.position = currentPanelInfo.leftSideConfirmationPanel.position;
				}
			} else {
				currentPanelInfo.confirmationPanel.transform.localPosition = currentPanelInfo.originalCofirmationPanelPosition;
			}
		} else {
			confirmCurrentCategoryCommnand ();
		}

		currentPanelInfo.isCurrentPanel = true;

		return;
	}

	public void confirmCurrentCategoryCommnand ()
	{
		if (currentPanelInfo.showConfirmationPanel) {
			currentPanelInfo.confirmationPanel.SetActive (false);
		}

		if (currentPanelInfo.panelObject.activeSelf != true) {
			currentPanelInfo.panelObject.SetActive (true);
		}

		if (currentPanelInfo.checkRightOrLeftSideForPanel) {

			RectTransform currentPanelObjectRectTransform = currentPanelInfo.panelObject.GetComponent<RectTransform> ();

			Vector3 currentCharacterPosition = currentCharacterInfo.mainTransform.position;

			Vector3 screenPoint = mainCamera.WorldToScreenPoint (currentCharacterPosition);

			float currentHorizontalPosition = screenPoint.x;

			showPanelObjectOnRightOrLeftSideScreen = true;

			if (horizontaResolution - currentHorizontalPosition < horizontalLimitOnRightScreenCategoryPanel) {
				showPanelObjectOnRightOrLeftSideScreen = false;
			}

			if (showDebugPrint) {
				print ("currentHorizontalPosition " + horizontaResolution + " " + currentHorizontalPosition);
			}

			if (showPanelObjectOnRightOrLeftSideScreen) {
				currentPanelObjectRectTransform.position = currentPanelInfo.rightSidePanelObject.position;
			} else {
				currentPanelObjectRectTransform.position = currentPanelInfo.leftSidePanelObject.position;
			}
		} else {
			currentPanelInfo.panelObject.transform.localPosition = currentPanelInfo.originalPanelPosition;
		}

		if (currentPanelInfo.useEventsOnSelectPanel) {
			currentPanelInfo.eventOnSelectPanel.Invoke ();
		}
	}

	public void cancelCurrentCategoryCommnand ()
	{
		if (currentPanelInfo.showConfirmationPanel) {
			currentPanelInfo.confirmationPanel.SetActive (false);
		}
	}

	public void closeAllPanelInfo ()
	{
		for (int i = 0; i < panelCategoryInfoList.Count; i++) {
			for (int j = 0; j < panelCategoryInfoList [i].panelInfoList.Count; j++) {
				if (panelCategoryInfoList [i].panelInfoList [j].panelObject.activeSelf != false) {
					panelCategoryInfoList [i].panelInfoList [j].panelObject.SetActive (false);
				}

				panelCategoryInfoList [i].panelInfoList [j].isCurrentPanel = false;

				if (panelCategoryInfoList [i].panelInfoList [j].showConfirmationPanel) {
					if (panelCategoryInfoList [i].panelInfoList [j].confirmationPanel.activeSelf) {
						panelCategoryInfoList [i].panelInfoList [j].confirmationPanel.SetActive (false);
					}
				}
			}
		}

		setConfirmUseInventoryItemPanelActiveState (false);
	}

	public void showCurrentCommandDescription (GameObject buttonObject)
	{
		int commandCategoryIndex = -1;
		int commandIndex = -1;

		int commandCategoryInfoListCount = commandCategoryInfoList.Count;

		for (int i = 0; i < commandCategoryInfoListCount; i++) {

			int commandInfoListCount = commandCategoryInfoList [i].commandInfoList.Count;

			for (int j = 0; j < commandInfoListCount; j++) {
				if (commandCategoryInfoList [i].commandInfoList [j].commandButton == buttonObject) {

					commandIndex = j;

					commandCategoryIndex = i;
				}
			}
		}

		if (commandIndex == -1 || commandCategoryIndex == -1) {
			return;
		}

		if (commandCategoryInfoList [commandCategoryIndex].commandInfoList [commandIndex].showCommandInfoOnHover) {

			string commandDescription = commandCategoryInfoList [commandCategoryIndex].commandInfoList [commandIndex].commandInfoOnHover;

			if (!commandDescriptionGameObject.activeSelf) {
				commandDescriptionGameObject.SetActive (true);
			}

			commandDescriptionText.text = commandDescription;
		} else {
			if (commandDescriptionGameObject.activeSelf) {
				commandDescriptionGameObject.SetActive (false);
			}
		}
	}

	public void disableCurrentCommandDescription ()
	{
		if (commandDescriptionGameObject.activeSelf) {
			commandDescriptionGameObject.SetActive (false);
		}
	}

	public void checkButtonPressed (GameObject buttonObject)
	{
		if (showDebugPrint) {
			print (buttonObject.name);
		}

		if (playingCommandActive) {
			return;
		}

		if (delayToActivateCommandActive) {
			return;
		}

		checkButton (buttonObject);
	}

	void checkButton (GameObject buttonObject)
	{
		int commandCategoryIndex = -1;
		int commandIndex = -1;

		int commandCategoryInfoListCount = commandCategoryInfoList.Count;

		for (int i = 0; i < commandCategoryInfoListCount; i++) {

			int commandInfoListCount = commandCategoryInfoList [i].commandInfoList.Count;

			for (int j = 0; j < commandInfoListCount; j++) {
				if (commandCategoryInfoList [i].commandInfoList [j].commandButton == buttonObject) {

					commandIndex = j;

					commandCategoryIndex = i;
				}
			}
		}

		if (commandIndex == -1 || commandCategoryIndex == -1) {
			return;
		}

		setCurrentCommandInfoByIndex (commandIndex, commandCategoryIndex);
	}

	//UPDATE UI ELEMENTS
	void updateMainActionButtonsPanelToCharacter ()
	{
		enableOrDisableMainActionButtonsPanel (true);

		if (showDebugPrint) {
			print ("updateMainActionButtonsPanelToCharacter");
		}

		Vector3 screenPoint = Vector3.zero;

		Vector3 currentCharacterPosition = currentCharacterInfo.mainTransform.position;

		if (usingScreenSpaceCamera) {
			screenPoint = mainCamera.WorldToViewportPoint (currentCharacterPosition);
		} else {
			screenPoint = mainCamera.WorldToScreenPoint (currentCharacterPosition);
		}

		if (usingScreenSpaceCamera) {
			Vector2 iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, (screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);

			mainActionButtonsPanel.anchoredPosition = iconPosition2d;
		} else {
			mainActionButtonsPanel.transform.position = screenPoint;
		}
	}

	void updateCurrentCharacterIcon ()
	{
		enableOrDisableCurrentCharacterIcon (true);

		Vector3 screenPoint = Vector3.zero;

		Vector3 currentCharacterPosition = currentCharacterInfo.mainTransform.position;

		if (usingScreenSpaceCamera) {
			screenPoint = mainCamera.WorldToViewportPoint (currentCharacterPosition);
		} else {
			screenPoint = mainCamera.WorldToScreenPoint (currentCharacterPosition);
		}

		if (usingScreenSpaceCamera) {
			Vector2 iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, (screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);

			currentCharacterIcon.anchoredPosition = iconPosition2d;
		} else {
			currentCharacterIcon.transform.position = screenPoint;
		}
	}

	void updateCurrentCharacterTargetIcon (Transform characterTransform)
	{
		enableOrDisableCurrentCharacterTargetIcon (true);

		Vector3 screenPoint = Vector3.zero;

		Vector3 currentCharacterPosition = characterTransform.position;

		if (usingScreenSpaceCamera) {
			screenPoint = mainCamera.WorldToViewportPoint (currentCharacterPosition);
		} else {
			screenPoint = mainCamera.WorldToScreenPoint (currentCharacterPosition);
		}

		if (usingScreenSpaceCamera) {
			Vector2 iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, (screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);

			currentCharacterTargetIcon.anchoredPosition = iconPosition2d;
		} else {
			currentCharacterTargetIcon.transform.position = screenPoint;
		}
	}

	void enableOrDisableMainActionButtonsPanel (bool state)
	{
		if (mainActionButtonsPanel.gameObject.activeSelf != state) {
			mainActionButtonsPanel.gameObject.SetActive (state);
		}
	}

	void enableOrDisableCurrentCharacterIcon (bool state)
	{
		if (currentCharacterIcon.gameObject.activeSelf != state) {
			currentCharacterIcon.gameObject.SetActive (state);
		}
	}

	void enableOrDisableCurrentCharacterTargetIcon (bool state)
	{
		if (currentCharacterTargetIcon.gameObject.activeSelf != state) {
			currentCharacterTargetIcon.gameObject.SetActive (state);
		}
	}

	//STATS FUNCTIONS
	public void addNewTurnBasedCombatChararcterUIInfo (GameObject characterUIOwner, GameObject sliderPrefab,
	                                                   string ownerName, int objectID, Transform healtSlidersParent, 
	                                                   characterInfo newCharacterInfo)
	{
		bool instantiateNewSliderResult = false;

		turnBasedCombatChararcterUIInfoObject.turnBasedCombatChararcterUIInfo newTurnBasedCombatChararcterUIInfo = new
			turnBasedCombatChararcterUIInfoObject.turnBasedCombatChararcterUIInfo ();

		if (objectID >= turnBasedCombatChararcterUIInfoList.Count || turnBasedCombatChararcterUIInfoList.Count == 0) {
			instantiateNewSliderResult = true;
		} else {
			newTurnBasedCombatChararcterUIInfo = turnBasedCombatChararcterUIInfoList [objectID];
		}

		GameObject characterUIGameObject = null;

		if (instantiateNewSliderResult) {
			characterUIGameObject = Instantiate (sliderPrefab, healtSlidersParent);

			newTurnBasedCombatChararcterUIInfo = characterUIGameObject.GetComponent<turnBasedCombatChararcterUIInfoObject> ().mainTurnBasedCombatChararcterUIInfo;
		} else {
			characterUIGameObject = newTurnBasedCombatChararcterUIInfo.characterUIGameObject;
		}

		newTurnBasedCombatChararcterUIInfo.Name = ownerName;
		newTurnBasedCombatChararcterUIInfo.characterUIOwner = characterUIOwner;
		newTurnBasedCombatChararcterUIInfo.ID = objectID;

		characterUIGameObject.name = "Health Bar " + ownerName;

		characterUIGameObject.transform.localScale = Vector3.one;
		characterUIGameObject.transform.localPosition = Vector3.zero;
		characterUIGameObject.transform.localRotation = Quaternion.identity;

		newTurnBasedCombatChararcterUIInfo.characterUIGameObject = characterUIGameObject;

		newTurnBasedCombatChararcterUIInfo.characterNameText.text = ownerName;

		if (!newTurnBasedCombatChararcterUIInfo.characterUIGameObject.activeSelf) {
			newTurnBasedCombatChararcterUIInfo.characterUIGameObject.SetActive (true);
		}

		if (instantiateNewSliderResult) {
			turnBasedCombatChararcterUIInfoList.Add (newTurnBasedCombatChararcterUIInfo);
		}

		int turnBasedCombatStatUIInfoListCount = 
			newTurnBasedCombatChararcterUIInfo.turnBasedCombatStatUIInfoList.Count;
		
		for (int i = 0; i < turnBasedCombatStatUIInfoListCount; i++) {
			turnBasedCombatChararcterUIInfoObject.turnBasedCombatStatUIInfo currentTurnBasedCombatStatUIInfo = 
				newTurnBasedCombatChararcterUIInfo.turnBasedCombatStatUIInfoList [i];

			if (currentTurnBasedCombatStatUIInfo.statObject.activeSelf) {
				currentTurnBasedCombatStatUIInfo.statObject.SetActive (false);
			}

			currentTurnBasedCombatStatUIInfo.statAssigned = false;
		}

		List<string> characterStatsToShowList = newCharacterInfo.mainTurnBasedCombatActionsSystem.characterStatsToShowList;

		int characterStatsToShowListCount = characterStatsToShowList.Count;

		for (int i = 0; i < characterStatsToShowListCount; i++) {
			turnBasedCombatChararcterUIInfoObject.turnBasedCombatStatUIInfo currentTurnBasedCombatStatUIInfo = 
				newTurnBasedCombatChararcterUIInfo.turnBasedCombatStatUIInfoList [i];
				
			currentTurnBasedCombatStatUIInfo.statObject.SetActive (true);

			currentTurnBasedCombatStatUIInfo.statAssigned = true;

			string statName = characterStatsToShowList [i];

			currentTurnBasedCombatStatUIInfo.Name = statName;

			if (currentTurnBasedCombatStatUIInfo.useStatNameText) {
				int statIndex = statNameLettersInfoList.FindIndex (s => s.Name.Equals (statName));

				if (statIndex > -1) {
					currentTurnBasedCombatStatUIInfo.statNameText.text = statNameLettersInfoList [statIndex].statLetters;
				} else {
					currentTurnBasedCombatStatUIInfo.statNameText.text = statName;
				}
			}

			float statMaxValue = newCharacterInfo.mainPlayerStatsSystem.getStatMaxAmountByName (statName);

			float statValue = newCharacterInfo.mainPlayerStatsSystem.getStatValue (statName);

			currentTurnBasedCombatStatUIInfo.statSlider.maxValue = statMaxValue;

			currentTurnBasedCombatStatUIInfo.statSlider.value = statValue;

			if (currentTurnBasedCombatStatUIInfo.useStatAmountText) {
				currentTurnBasedCombatStatUIInfo.statAmountText.text = statValue.ToString ();
			}
		}

		newCharacterInfo.mainTurnBasedCombatChararcterUIInfo = newTurnBasedCombatChararcterUIInfo;
	}

	public void updateAllCharacterStatsUIValue ()
	{
		int playerTeamCharacterListCount = playerTeamCharacterList.Count;

		for (int i = 0; i < playerTeamCharacterListCount; i++) {
			updateCharacterStatsUIValue (playerTeamCharacterList [i].mainTransform.gameObject, 
				playerTeamCharacterList [i].mainPlayerStatsSystem);
		}

		int enemyTeamCharacterListCount = enemyTeamCharacterList.Count;

		for (int i = 0; i < enemyTeamCharacterListCount; i++) {
			updateCharacterStatsUIValue (enemyTeamCharacterList [i].mainTransform.gameObject,
				enemyTeamCharacterList [i].mainPlayerStatsSystem);
		}
	}

	public void updateCharacterStatsUIValue (GameObject characterUIOwner, playerStatsSystem mainPlayerStatsSystem)
	{
		int turnBasedCombatChararcterUIInfoListCount = turnBasedCombatChararcterUIInfoList.Count;

		for (int i = 0; i < turnBasedCombatChararcterUIInfoListCount; i++) {
			turnBasedCombatChararcterUIInfoObject.turnBasedCombatChararcterUIInfo temporalInfo = 
				turnBasedCombatChararcterUIInfoList [i];

	
			if (temporalInfo.characterUIOwner == characterUIOwner) {

				if (showStatsDebugPrint) {
					print ("update stats info on " + temporalInfo.Name);
				}

				int turnBasedCombatStatUIInfoListCount = temporalInfo.turnBasedCombatStatUIInfoList.Count;

				for (int j = 0; j < turnBasedCombatStatUIInfoListCount; j++) {
					turnBasedCombatChararcterUIInfoObject.turnBasedCombatStatUIInfo currentTurnBasedCombatStatUIInfo = 
						temporalInfo.turnBasedCombatStatUIInfoList [j];

					if (currentTurnBasedCombatStatUIInfo.statAssigned) {
						string statName = currentTurnBasedCombatStatUIInfo.Name;

						float statMaxValue = mainPlayerStatsSystem.getStatMaxAmountByName (statName);

						float statValue = mainPlayerStatsSystem.getStatValue (statName);

						currentTurnBasedCombatStatUIInfo.statSlider.maxValue = statMaxValue;

						currentTurnBasedCombatStatUIInfo.statSlider.value = statValue;

						if (currentTurnBasedCombatStatUIInfo.useStatAmountText) {
							currentTurnBasedCombatStatUIInfo.statAmountText.text = statValue.ToString ();
						}

						if (showStatsDebugPrint) {
							print ("update stat " + statName + " " + statValue);
						}
					}
				}

				return;
			}
		}
	}

	//OTHER FUNCTIONS TO CHECK STATES
	public bool allCharactersPositionAdjusted ()
	{
		for (int i = 0; i < numberOfCharactersOnEnemyTeam; i++) {
			if (!enemyTeamCharacterList [i].mainHealth.isDead ()) {
				if (enemyTeamCharacterList [i].mainTurnBasedCombatActionsSystem.isAdjustingCharacterPositionInProcess ()) {
					return false;
				}
			}
		}

		for (int i = 0; i < numberOfCharactersOnPlayerTeam; i++) {
			if (!playerTeamCharacterList [i].mainHealth.isDead ()) {
				if (playerTeamCharacterList [i].mainTurnBasedCombatActionsSystem.isAdjustingCharacterPositionInProcess ()) {
					return false;
				}
			}
		}

		return true;
	}

	public characterInfo getFirstCharacterAliveOfEnemyTeam ()
	{
		for (int i = 0; i < numberOfCharactersOnEnemyTeam; i++) {
			if (!enemyTeamCharacterList [i].mainHealth.isDead ()) {
				return enemyTeamCharacterList [i];
			}
		}

		return null;
	}

	public characterInfo getFirstCharacterAliveOfPlayerTeam ()
	{
		for (int i = 0; i < numberOfCharactersOnPlayerTeam; i++) {
			if (!playerTeamCharacterList [i].mainHealth.isDead ()) {
				return playerTeamCharacterList [i];
			}
		}

		return null;
	}

	public void checkTeamsDeadStateAfterCharacterDeath (Transform characterTransform)
	{
		if (showDebugPrint) {
			print ("character dead during other's turn, checking if any team dead");
		}

		if (isAllEnemyTeamDead ()) {
			if (showDebugPrint) {
				print ("all enemy team dead, setting next turn");
			}

			setNextTurn ();

			return;
		} 

		if (isAllPlayerTeamDead ()) {
			if (showDebugPrint) {
				print ("all player team dead, setting next turn");
			}

			setNextTurn ();
		}
	}

	void resetCharacterCombatPositionsOnBothTeams ()
	{
		for (int i = 0; i < numberOfCharactersOnEnemyTeam; i++) {
			if (!enemyTeamCharacterList [i].mainHealth.isDead ()) {
				enemyTeamCharacterList [i].mainTurnBasedCombatActionsSystem.resetToCurrrentCombatPositionAndRotation (!resetCharacterPositionsSmoothly);
			}
		}

		for (int i = 0; i < numberOfCharactersOnPlayerTeam; i++) {
			if (!playerTeamCharacterList [i].mainHealth.isDead ()) {
				playerTeamCharacterList [i].mainTurnBasedCombatActionsSystem.resetToCurrrentCombatPositionAndRotation (!resetCharacterPositionsSmoothly);
			}
		}
	}

	public void checkPlayerStateOnDeathDuringCombat ()
	{
		if (combatActive && turnCombatActive) {
			if (mainPlayerController.isPlayerDead ()) {
//				checkInputListToPauseDuringAction (customInputToPauseOnActionInfoList, false);

//				resetInputListToPauseDuringAction ();
			}
		}
	}

	public void checkCharacterStateAfterResurrect (Transform characterTransform)
	{
		if (combatActive && turnCombatActive) {
			for (int i = 0; i < numberOfCharactersOnEnemyTeam; i++) {
				if (enemyTeamCharacterList [i].mainTransform == characterTransform) {
					setSingleCharacterTeamState (true, enemyTeamCharacterList [i], false, true);

					adjustBothTeamsPositionOnNextTurnActive = true;

					return;
				}
			}

			for (int i = 0; i < numberOfCharactersOnPlayerTeam; i++) {
				if (playerTeamCharacterList [i].mainTransform == characterTransform) {
					setSingleCharacterTeamState (true, playerTeamCharacterList [i], true, true);

					adjustBothTeamsPositionOnNextTurnActive = true;

					return;
				}
			}
		}
	}

	void setSingleCharacterTeamState (bool state, characterInfo singleCharacterInfo, bool isPlayerTeam, bool characterHasResurrected)
	{
		singleCharacterInfo.mainPlayerController.setTurnBasedCombatActionActiveState (state);

		singleCharacterInfo.mainPlayerWeaponsManager.setTurnBasedCombatActionActiveState (state);

		if (singleCharacterInfo.mainPlayerController.isCharacterUsedByAI ()) {
			singleCharacterInfo.mainFindObjectivesSystem.setTurnBasedCombatActionActiveState (state);
		}

		singleCharacterInfo.mainTurnBasedCombatActionsSystem.setTurnBasedCombatActionActiveState (state);

		bool isPlayer = singleCharacterInfo.isPlayer;

		if (isPlayer) {
			if (singleCharacterInfo.mainMenuPause != null) {
				singleCharacterInfo.mainMenuPause.setChangeBetweenIngameMenuPausedState (state);
			}
		} else {
			if (!state) {
				singleCharacterInfo.mainAINavMesh.setTurnBasedCombatActionActiveState (state);
			}

			singleCharacterInfo.mainAINavMesh.pauseAI (state);

			if (state) {
				singleCharacterInfo.mainAINavMesh.setTurnBasedCombatActionActiveState (state);
			}

			singleCharacterInfo.mainMatchPlayerToTargetSystem.setMatchSystemEnabledstate (state);

			if (isPlayerTeam) {
				singleCharacterInfo.mainMatchPlayerToTargetSystem.setAddMainPlayerOnListForAIState (!state);
			}

			singleCharacterInfo.mainFindObjectivesSystem.setRemovePartnerPausedState (state);
		}

		if (state) {
			if (singleCharacterInfo.mainPlayerController.isActionActive ()) {
				singleCharacterInfo.mainPlayerController.stopAllActionsOnActionSystem ();
			}
		} else {
			bool characterAlive = singleCharacterInfo.mainHealth.isDead ();

			singleCharacterInfo.mainTurnBasedCombatActionsSystem.checkEventsForAliveOrDeadAfterCombat (characterAlive);
		}

		//better input pause state
		singleCharacterInfo.mainPlayerController.setMoveInputPausedState (state);

		singleCharacterInfo.mainPlayerCamera.changeCameraRotationState (!state);

		singleCharacterInfo.mainHeadTrack.setSmoothHeadTrackDisableState (state);

		singleCharacterInfo.mainPlayerController.setHeadTrackCanBeUsedState (!state);

		singleCharacterInfo.mainPlayerController.resetPlayerControllerInput ();

		singleCharacterInfo.mainPlayerController.resetOtherInputFields ();

		singleCharacterInfo.mainPlayerController.resetAnimatorState ();

		singleCharacterInfo.statesManager.checkPlayerStates (false, true, false, true, false, false, true, true);

		singleCharacterInfo.mainPlayerActionSystem.setIgnoreInputChangeActiveState (state);

		if (state) {
			singleCharacterInfo.mainHealth.setShieldStateAndCheckEventsForShieldState (false);

			singleCharacterInfo.mainHealth.setIgnoreSetShieldStateActive (true);

			singleCharacterInfo.mainHealth.setUseEventOnHealthValueListState (false);

			singleCharacterInfo.mainHealth.setUseWeakSpotsActiveState (false);

			singleCharacterInfo.mainHealth.setActivateRagdollOnDamageReceivedState (false);
		} else {
			singleCharacterInfo.mainHealth.setIgnoreSetShieldStateActive (false);

			singleCharacterInfo.mainHealth.setOriginalUseShieldState ();

			singleCharacterInfo.mainHealth.checkEventsForShieldState ();

			singleCharacterInfo.mainHealth.setOriginalUseEventOnHealthValueListState ();

			singleCharacterInfo.mainHealth.setOriginalUseWeakSpotsActiveState ();

			singleCharacterInfo.mainHealth.setOriginalActivateRagdollOnDamageReceivedState ();
		}

		if (!isPlayer) {
			singleCharacterInfo.mainFindObjectivesSystem.clearFullEnemiesList ();

			singleCharacterInfo.mainFindObjectivesSystem.removeCharacterAsTargetOnSameFaction ();

			singleCharacterInfo.mainFindObjectivesSystem.resetAITargets ();

			singleCharacterInfo.mainFindObjectivesSystem.removeTargetInfo ();

			singleCharacterInfo.mainFindObjectivesSystem.stopCurrentAttackInProcess ();
		}

		singleCharacterInfo.mainFindObjectivesSystem.setAIOnPlayerTeamState (isPlayerTeam);

		singleCharacterInfo.mainPlayerController.getPlayerCameraManager ().pauseOrPlayCamera (!state);

		if (state) {
			singleCharacterInfo.mainPlayerController.setPlayerAndCameraParent (mainTurnBasedCombatSystem.transform);

		} else {
			singleCharacterInfo.mainPlayerController.setPlayerAndCameraParent (singleCharacterInfo.previousCharacterParent);
		}

		if (freeCombatActive) {
			if (!isPlayer) {
				singleCharacterInfo.mainFindObjectivesSystem.resetAITargets ();
			}
		}

		if (characterHasResurrected) {
			if (state) {
				if (isPlayer) {
					resetInputListToPauseDuringAction ();

					checkInputListToPauseDuringAction (customInputToPauseOnActionInfoList, state);
				}
			}
		}
	}

	public bool isAllEnemyTeamDead ()
	{
		for (int i = 0; i < numberOfCharactersOnEnemyTeam; i++) {
			if (!enemyTeamCharacterList [i].mainHealth.isDead ()) {
				return false;
			}
		}

		return true;
	}

	public bool isAllPlayerTeamDead ()
	{
		for (int i = 0; i < numberOfCharactersOnPlayerTeam; i++) {
			if (!playerTeamCharacterList [i].mainHealth.isDead ()) {
				return false;
			}
		}

		return true;
	}


	public int getEnemyTeamAliveAmount ()
	{
		int amount = 0;

		for (int i = 0; i < numberOfCharactersOnEnemyTeam; i++) {
			if (!enemyTeamCharacterList [i].mainHealth.isDead ()) {
				amount++;
			}
		}

		return amount;
	}

	public int getPlayerTeamAliveAmount ()
	{
		int amount = 0;

		for (int i = 0; i < numberOfCharactersOnPlayerTeam; i++) {
			if (!playerTeamCharacterList [i].mainHealth.isDead ()) {
				amount++;
			}
		}

		return amount;
	}

	public void checkInputListToPauseDuringAction (List<playerActionSystem.inputToPauseOnActionIfo> inputList, bool state)
	{
		for (int i = 0; i < inputList.Count; i++) {
			if (state) {
				inputList [i].previousActiveState = mainPlayerInputManager.setPlayerInputMultiAxesStateAndGetPreviousState (false, inputList [i].inputName);
			} else {
				if (inputList [i].previousActiveState) {
					mainPlayerInputManager.setPlayerInputMultiAxesState (inputList [i].previousActiveState, inputList [i].inputName);
				}
			}
		}
	}

	void resetInputListToPauseDuringAction ()
	{
		for (int i = 0; i < customInputToPauseOnActionInfoList.Count; i++) {
			customInputToPauseOnActionInfoList [i].previousActiveState = false;
		}
	}

	void selectNextOrPreviousCharacterTarget (bool state)
	{
		if (selectingTargetForCommandActive) {
			if (currentTurnForPlayerTeam) {

				currentCharacterTargetInfoIndex = enemyTeamCharacterList.IndexOf (currentCharacterTargetInfo);

				currentCharacterTargetInfoIndex++;

				if (currentCharacterTargetInfoIndex >= enemyTeamCharacterList.Count) {
					currentCharacterTargetInfoIndex++;
				}


				bool newIndexFound = false;

				int loopCount = 0;

				int temporalIndex = currentCharacterTargetInfoIndex;

				while (!newIndexFound) {

					if (!enemyTeamCharacterList [temporalIndex].mainHealth.isDead ()) {
						newIndexFound = true;

						currentCharacterTargetInfoIndex = temporalIndex;
					} else {

						temporalIndex++;

						if (temporalIndex >= numberOfCharactersOnEnemyTeam) {
							temporalIndex = 0;
						}

						loopCount++;

						if (loopCount > 100) {
							newIndexFound = true;
						}
					}
				}

				currentCharacterTargetInfo = enemyTeamCharacterList [currentCharacterTargetInfoIndex];

				updateCurrentCharacterTargetIcon (currentCharacterTargetInfo.mainGameObject.transform);

			} else {

			}
		}
	}

	//INVENTORY FUNCTIONS
	public void selectInventoryItem (GameObject inventoryButton)
	{
		inventoryItemSelected = false;

		int currentIndex = simpleUIButtonInfoInventoryButtonList.FindIndex (s => s.buttonObject.Equals (inventoryButton));

		if (showDebugPrint) {
			print ("selecting inventory item button " + inventoryButton.name);
		}

		if (currentIndex > -1) {
			if (showDebugPrint) {
				print ("item found");
			}

			simpleUIButtonInfo currentSimpleUIButtonInfo = simpleUIButtonInfoInventoryButtonList [currentIndex];

			currentInventoryObjectNameSelected = currentSimpleUIButtonInfo.getCurrentName ();

			inventoryItemSelected = true;

			setConfirmUseInventoryItemPanelActiveState (true);
		} else {
			if (showDebugPrint) {
				print ("item not found");
			}
		}
	}

	void setConfirmUseInventoryItemPanelActiveState (bool state)
	{
		if (confirmUseInventoryItemPanel.activeSelf != state) {
			confirmUseInventoryItemPanel.SetActive (state);
		}

		if (state) {
			if (checkRightOrLeftSideForConfirmUseInventoryItemPanel) {

				RectTransform currentPanelObjectRectTransform = confirmUseInventoryItemPanel.GetComponent<RectTransform> ();

				Vector3 currentCharacterPosition = currentCharacterInfo.mainTransform.position;

				Vector3 screenPoint = mainCamera.WorldToScreenPoint (currentCharacterPosition);

				float currentHorizontalPosition = screenPoint.x;

				showConfirmationPanelObjectOnRightOrLeftSideScreen = true;

				if (horizontaResolution - currentHorizontalPosition < horizontalLimitOnRightScreenConfirmationPanel) {
					showConfirmationPanelObjectOnRightOrLeftSideScreen = false;
				}

				if (showDebugPrint) {
					print ("currentHorizontalPosition " + horizontaResolution + " " + currentHorizontalPosition);
				}

				if (showConfirmationPanelObjectOnRightOrLeftSideScreen) {
					currentPanelObjectRectTransform.position = rightSideConfirmUseInventoryItemPanel.position;
				} else {
					currentPanelObjectRectTransform.position = leftSideConfirmUseInventoryItemPanel.position;
				}
			} else {
				confirmUseInventoryItemPanel.transform.localPosition = rightSideConfirmUseInventoryItemPanel.localPosition;
			}
		}
	}

	public void showInventoryItemListPanel ()
	{
		for (int i = 0; i < simpleUIButtonInfoInventoryButtonList.Count; i++) {
			if (simpleUIButtonInfoInventoryButtonList [i].buttonObject.activeSelf) {
				simpleUIButtonInfoInventoryButtonList [i].buttonObject.SetActive (false);
			}
		}

		List<inventoryInfo> inventoryList = mainInventoryManager.getInventoryList ();

		int inventoryListCount = inventoryList.Count;

		int inventoryItemCount = 0;

		for (int i = 0; i < inventoryListCount; i++) {
			inventoryInfo currentInventoryInfo = inventoryList [i];

			if (currentInventoryInfo.amount > 0) {
				if (currentInventoryInfo.canBeUsed) {
					addInventoryUIPanelElement (currentInventoryInfo.amount, currentInventoryInfo.Name, 
						inventoryItemCount, currentInventoryInfo.icon);

					inventoryItemCount++;
				}
			}
		}
	}

	public void updateInventoryItemListPanel ()
	{
		showInventoryItemListPanel ();
	}

	void addInventoryUIPanelElement (int amount, string objectName, int objectID, Texture objectIcon)
	{
		bool instantiateNewObjectResult = false;

		simpleUIButtonInfo newSimpleUIButtonInfo = null;

		if (objectID >= simpleUIButtonInfoInventoryButtonList.Count || simpleUIButtonInfoInventoryButtonList.Count == 0) {
			instantiateNewObjectResult = true;
		} else {
			newSimpleUIButtonInfo = simpleUIButtonInfoInventoryButtonList [objectID];
		}

		GameObject currentUIGameObject = null;

		if (instantiateNewObjectResult) {
			currentUIGameObject = Instantiate (inventoryItemButtonPrefab, inventoryItemsPanelParent);

			newSimpleUIButtonInfo = currentUIGameObject.GetComponent<simpleUIButtonInfo> ();
		} else {
			currentUIGameObject = newSimpleUIButtonInfo.buttonObject;
		}

		newSimpleUIButtonInfo.ID = objectID;

		currentUIGameObject.name = "Item " + objectID;

		currentUIGameObject.transform.localScale = Vector3.one;
		currentUIGameObject.transform.localPosition = Vector3.zero;
		currentUIGameObject.transform.localRotation = Quaternion.identity;

		newSimpleUIButtonInfo.buttonObject = currentUIGameObject;

		newSimpleUIButtonInfo.mainText.text = objectName + " x" + amount;

		newSimpleUIButtonInfo.currentName = objectName;

		if (!newSimpleUIButtonInfo.buttonObject.activeSelf) {
			newSimpleUIButtonInfo.buttonObject.SetActive (true);
		}

		if (newSimpleUIButtonInfo.mainIcon != null) {
			newSimpleUIButtonInfo.mainIcon.texture = objectIcon;
		}

		if (instantiateNewObjectResult) {
			simpleUIButtonInfoInventoryButtonList.Add (newSimpleUIButtonInfo);
		}

	}

	public void confirmToUseInventoryItem ()
	{
		if (inventoryItemSelected) {
			bool currentCharacterInfoisPlayer = currentCharacterInfo.isPlayer;

			if (!currentCharacterInfoisPlayer) {
				mainInventoryManager.setExternalCharacterForInventoryUsage (currentCharacterInfo.mainGameObject);
			}

			mainInventoryManager.useInventoryObjectByName (currentInventoryObjectNameSelected, 1);

			bool wasLastObjectWasUsedSuccessfully = mainInventoryManager.wasLastObjectWasUsedSuccessfully ();

			if (!currentCharacterInfoisPlayer) {
				mainInventoryManager.setExternalCharacterForInventoryUsage (null);
			}

			if (showDebugPrint) {
				print ("object used result " + wasLastObjectWasUsedSuccessfully);
			}

			if (wasLastObjectWasUsedSuccessfully) {
				updateInventoryItemListPanel ();

				setConfirmUseInventoryItemPanelActiveState (false);

				inventoryItemSelected = false;

				updateAllCharacterStatsUIValue ();

				string objectUsedMessage = useInventoryItemMessageContent;

				if (objectUsedMessage.Contains (itemNameField)) {
					objectUsedMessage = objectUsedMessage.Replace (itemNameField, currentInventoryObjectNameSelected);
				}

				setCurrentCommandNameUsed (objectUsedMessage);

				if (setNextTurnOnInventoryItemUsed) {
					selectingTargetForCommandActive = false;

					enableOrDisableCurrentCharacterTargetIcon (false);

					enableOrDisableMainActionButtonsPanel (false);

					enableOrDisableCurrentCharacterIcon (false);

					stopCommandCoroutine ();

					commandCoroutine = StartCoroutine (playingCommnadCoroutine (useInventoryItemCommandDuration));

					disableCurrentCommandDescription ();
				}
			} else {
				string objectUsedMessage = unableToUseObjectMessageContent;

				if (objectUsedMessage.Contains (itemNameField)) {
					objectUsedMessage = objectUsedMessage.Replace (itemNameField, currentInventoryObjectNameSelected);
				}

				setCurrentCommandNameUsed (objectUsedMessage);

				setConfirmUseInventoryItemPanelActiveState (false);

				inventoryItemSelected = false;
			}
		}
	}

	public void cancelUseInventoryItem ()
	{
		if (inventoryItemSelected) {

			setConfirmUseInventoryItemPanelActiveState (false);

			inventoryItemSelected = false;
		}
	}
	//END INVENTORY FUNCTIONS

	//INPUT FUNCTIONS
	public void inputSelectNextOrPreviousCharacterTarget (bool state)
	{
		if (menuOpened) {
			selectNextOrPreviousCharacterTarget (state);
		}
	}

	public void inputConfirmCommandOnCurrentCharacterTarget ()
	{
		if (menuOpened) {
			if (selectingTargetForCommandActive) {
				if (currentCharacterTargetInfo != null) {
					confirmCurrentCommand ();
				}
			}
		}
	}

	public void inputConfirmCommand ()
	{
		if (menuOpened) {
			if (currentTurnForPlayerTeam) {
				confirmCurrentCommand ();
			}
		}
	}

	public void inputCancelCommand ()
	{
		if (menuOpened) {
			if (currentTurnForPlayerTeam) {
				cancelCurrentCommand ();
			}
		}
	}

	public void inputSelectNextTarget ()
	{
		if (menuOpened) {
			if (currentTurnForPlayerTeam) {
				selectNextOrPreviousCharacterTarget (true);
			}
		}
	}

	public void inputSelectPreviousTarget ()
	{
		if (menuOpened) {
			if (currentTurnForPlayerTeam) {
				selectNextOrPreviousCharacterTarget (false);
			}
		}
	}

	public void setFreeCombatActiveStateOnAllCharacters (bool state)
	{
		for (int i = 0; i < numberOfCharactersOnEnemyTeam; i++) {
			enemyTeamCharacterList [i].mainTurnBasedCombatActionsSystem.setFreeCombatActiveState (state);
		}

		for (int i = 0; i < numberOfCharactersOnPlayerTeam; i++) {
			playerTeamCharacterList [i].mainTurnBasedCombatActionsSystem.setFreeCombatActiveState (state);
		}
	}

	void openOrCloseMenuPanelToKeepCombatActive (bool state)
	{
		if (showDebugPrint) {
			print ("toogling turn and free combat");
		}

		if (state) {
			if (showDebugPrint) {
				print ("checking to resuming turn combat mode");
			}

			bool reactivateTurnCombatResult = true;

			//check here if the player is close enough to the enemy team or not to cancel it

			int enemyTeamCharacterListCount = enemyTeamCharacterList.Count;

			float minDistance = Mathf.Infinity;
	
			Vector3 currentPosition = mainPlayerController.transform.position;

			for (int i = 0; i < enemyTeamCharacterListCount; i++) {
				float currentDistance = GKC_Utils.distance (enemyTeamCharacterList [i].mainTransform.position, currentPosition);

				if (currentDistance < minDistance) {
					minDistance = currentDistance;
				}
			}

			if (minDistance > minDistanceToResumeTurnCombat) {
				reactivateTurnCombatResult = false;
			}

			if (reactivateTurnCombatResult) {
				turnCombatActive = true;

				freeCombatActive = false;

				mainTurnBasedCombatSystem.setFreeCombatActiveState (freeCombatActive);

				openOrCloseMenuPanel (true);

				if (showDebugPrint) {
					print ("resuming turn combat mode");
				}
			} else {
				turnCombatActive = true;

				freeCombatActive = false;

				mainTurnBasedCombatSystem.setFreeCombatActiveState (freeCombatActive);

				mainTurnBasedCombatSystem.clearCharactersAround ();

				openOrCloseMenuPanel (false);

				if (showDebugPrint) {
					print ("too far from enemy team, stopping turn combat directly");
				}
			}
		} else {
			if (showDebugPrint) {
				print ("resuming free combat mode");
			}

			freeCombatActive = true;

			mainTurnBasedCombatSystem.setFreeCombatActiveState (freeCombatActive);

			turnCombatActive = false;

			openOrCloseMenuPanel (false);
		}
	}

	public void inputToggleCombatMode ()
	{
		if (!toggleTurnAndFreeCombatInputEnabled) {
			return;
		}

		if (!currentTurnForPlayerTeam) {
			return;
		}

		if (menuOpened) {
			if (turnCombatActive) {
				if (allCharactersPositionAdjusted ()) {
					openOrCloseMenuPanelToKeepCombatActive (false);
				}
			} 
		} else {
			if (freeCombatActive) {
				if (mainPlayerController.playerIsBusy ()) {
					if (showDebugPrint) {
						print ("cancel toggle combat mode");
					}

					return;
				}

				if (mainPlayerController.isGamePaused ()) {
					if (showDebugPrint) {
						print ("cancel toggle combat mode");
					}

					return;
				}

				if (mainPlayerController.isActionActive ()) {
					if (showDebugPrint) {
						print ("cancel toggle combat mode");
					}

					return;
				}

				if (mainUsingDevicesSystem.anyDeviceDetected ()) {
					if (showDebugPrint) {
						print ("cancel toggle combat mode");
					}

					return;
				}

				openOrCloseMenuPanelToKeepCombatActive (true);
			}
		}
	}

	[System.Serializable]
	public class panelCategoryInfo
	{
		public string Name;

		public bool panelCategoryEnabled = true;

		[Space]

		public List<panelInfo> panelInfoList = new List<panelInfo> ();
	}

	[System.Serializable]
	public class panelInfo
	{
		public string Name;

		public bool panelButtonEnabled = true;

		public bool showConfirmationPanel;

		public GameObject confirmationPanel;

		[Space]
		[Space]

		public GameObject panelButton;

		public GameObject panelObject;

		[Space]

		public bool checkRightOrLeftSideForPanel;

		public RectTransform rightSidePanelObject;
		public RectTransform leftSidePanelObject;

		[Space]

		public bool checkRightOrLeftSideForConfirmationPanel;

		public RectTransform rightSideConfirmationPanel;
		public RectTransform leftSideConfirmationPanel;

		[HideInInspector] public Vector3 originalPanelPosition;

		[HideInInspector] public Vector3 originalCofirmationPanelPosition;

		[Space]
		[Space]

		public bool isCurrentPanel;

		[Space]
		[Space]

		public bool useEventsOnSelectPanel;
		public UnityEvent eventOnSelectPanel;
	}

	[System.Serializable]
	public class commandCategoryInfo
	{
		public string Name;

		[Space]

		public List<commandInfo> commandInfoList = new List<commandInfo> ();
	}

	[System.Serializable]
	public class commandInfo
	{
		public string Name;

		public bool isCommandEnabled = true;

		public bool selectTargetForCommand;

		[Space]

		public string commandName;

		public bool checkCharacterToReceiveOrdersComponent;

		[Space]

		public bool useRemoteEvent;

		public List<string> remoteEventNameList = new List<string> ();

		[Space]

		public bool useCustomOrderBehavior;

		public customOrderBehavior mainCustomOrderBehavior;

		[Space]

		public bool checkResetCharacterPositionAfterCommand;

		[Space]
		[Space]

		public float commandDuration;

		[Space]

		public bool useDelayToActivateCommand;
		public float delayToActivateCommand;

		[Space]

		public bool useMatchPositionSystem;
		public float matchPositionOffset = 1;

		[Space]
		[Space]

		public GameObject commandButton;

		[Space]

		public bool showConfirmationPanel;

		public GameObject confirmationPanel;

		[Space]

		public bool checkRightOrLeftSideForConfirmationPanel;

		public RectTransform rightSideConfirmationPanel;
		public RectTransform leftSideConfirmationPanel;

		[HideInInspector] public Vector3 originalConfirmationPanelPosition;

		[Space]
		[Space]

		public bool useStatToUseAction;
		public string statNameToUseAction;
		public float statAmountToUseAction;

		[Space]
		[Space]

		[TextArea (10, 11)] public string messageOnNotEnoughStat;

		[Space]

		public bool showInventoryObjects;

		[Space]
		[Space]

		public bool showCommandInfoOnHover;
		[TextArea (10, 11)] public string commandInfoOnHover;

		[Space]
		[Space]

		public bool isCurrentCommand;

		[Space]
		[Space]

		public bool useEventsOnSelectCommand;
		public UnityEvent eventOnSelectCommand;
	}

	[System.Serializable]
	public class characterInfo
	{
		public bool isPlayer;

		public GameObject mainGameObject;

		public health mainHealth;

		public Transform mainTransform;

		public turnBasedCombatActionsSystem mainTurnBasedCombatActionsSystem;

		public playerController mainPlayerController;

		public playerWeaponsManager mainPlayerWeaponsManager;

		public playerCamera mainPlayerCamera;

		public menuPause mainMenuPause;

		public ragdollActivator mainRagdollActivator;

		public headTrack mainHeadTrack;

		public playerStatesManager statesManager;

		public findObjectivesSystem mainFindObjectivesSystem;

		public AINavMesh mainAINavMesh;

		public playerStatsSystem mainPlayerStatsSystem;

		public matchPlayerToTargetSystem mainMatchPlayerToTargetSystem;

		public playerActionSystem mainPlayerActionSystem;

		public Transform previousCharacterParent;

		public healthSliderInfo mainHealthSliderInfo;

		public turnBasedCombatChararcterUIInfoObject.turnBasedCombatChararcterUIInfo mainTurnBasedCombatChararcterUIInfo;
	}
}
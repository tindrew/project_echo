using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AITurnBasedCombatSystemBrain : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool systemEnabled = true;


	public bool checkDistanceToTargetEnabled;

	public float currentMinDistanceToAttack = 7;

	public float defaultMatchOffset = 1;

	[Space]
	[Header ("Attack Settings")]
	[Space]

	public bool attackEnabled;

	public float minWaitToActivateAttack = 0.5f;

	public bool useProbabilityToUseAttack;
	[Range (0, 100)] public float probabilityToUseAttack;

	public bool ignoreProbabilityToCheckCategory;
	public bool ignoreProbabilityToUseAttack;

	public bool ignoreUseStatToUseAction;

	[Space]

	public bool ignoreIfLastCategoryPreviouslySelected;

	public bool ignoreIfLastAttackPreviouslySelected;

	[Space]

	public List<turnBasedCombatAttackCategoryInfo> turnBasedCombatAttackCategoryInfoList = new List<turnBasedCombatAttackCategoryInfo> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool systemActive;

	public bool isCurrentCharacterTurn;

	public bool AIOnPlayerTeam;

	[Space]

	public int lastAttackUsedIndex = -1;

	public int lastCategoryIndex = -1;

	public bool currentAttackInProcess;

	public bool delayToActiveteAttackInProcess;

	public string currentCommandName;

	[Space]

	public bool waitingForAttackActive;
	float currentRandomTimeToAttack;

	public bool canUseAttackActive;

	public bool attackStatePaused;

	public bool insideMinDistanceToAttack;

	//	public float currentAttackRate;

	//	public bool onSpotted;

	public bool waitToActivateAttackActive;

	public bool blockActive;

	public GameObject currentTarget;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventsOnCombatActive;
	public UnityEvent eventOnCombatActive;
	public UnityEvent eventOnCombatDeactivate;

	[Space]

	public bool useEventIfBlockActiveOnCurrentTurn;
	public UnityEvent eventIfBlockActiveOnCurrentTurn;

	[Space]
	[Header ("Components")]
	[Space]

	public findObjectivesSystem mainFindObjectivesSystem;
	public AINavMesh mainAINavmeshManager;

	public playerController mainPlayerController;

	public characterToReceiveOrders mainCharacterToReceiveOrders;

	public turnBasedCombatActionsSystem mainTurnBasedCombatActionsSystem;

	public matchPlayerToTargetSystem mainMatchPlayerToTargetSystem;

	public playerStatsSystem mainPlayerStatsSystem;

	public health mainHealth;


	float lastTimeAttack;

	int currentAttackTypeIndex;

	int currentAttackIndex;

	int currentAttackTypeToAlternateIndex;

	float currentPauseAttackStateDuration;
	float lastTimeAttackPauseWithDuration;

	float randomWaitTime;

	float lastTimeAttackActivated;

	float currentPathDistanceToTarget;

	bool AIPaused;

	turnBasedCombatAttackInfo currentTurnBasedCombatAttackInfo;

	void Start ()
	{
		if (systemActive) {
			systemActive = false;

			setSystemActiveState (true);
		}
	}

	public void updateAI ()
	{
		if (systemActive) {
			AIPaused = mainFindObjectivesSystem.isAIPaused ();

			if (!AIPaused) {

			}
		}
	}

	public void checkStateOnCharacterDead ()
	{
		if (systemActive) {
			if (isCurrentCharacterTurn) {
				if (mainPlayerController.checkIfPlayerDeadFromHealthComponent ()) {
					if (showDebugPrint) {
						print ("current character in turn has dead, moving turn to next character");
					}
		
					setNextTurn ();
		
					currentAttackInProcess = false;
		
					isCurrentCharacterTurn = false;
		
					delayToActiveteAttackInProcess = false;
				}
			} else {
				mainTurnBasedCombatActionsSystem.checkTeamsDeadStateAfterCharacterDeath ();
			}
		}
	}

	public void resetStates ()
	{


	}

	public void updateInsideMinDistance (bool newInsideMinDistanceToAttack)
	{
		insideMinDistanceToAttack = newInsideMinDistanceToAttack;

		if (insideMinDistanceToAttack) {

		} else {

		}
	}

	public void updateBehavior ()
	{
		if (!systemActive) {
			return;
		}

		if (AIPaused) {
			return;
		}

		currentPathDistanceToTarget = mainFindObjectivesSystem.currentPathDistanceToTarget;

		checkAttackState ();
	}

	public void checkAttackState ()
	{
		if (!attackEnabled) {
			return;
		}

		if (checkDistanceToTargetEnabled) {
			insideMinDistanceToAttack = mainFindObjectivesSystem.insideMinDistanceToAttack;

			if (!insideMinDistanceToAttack) {
				return;
			}
		}

		if (currentPauseAttackStateDuration > 0) {
			if (Time.time > currentPauseAttackStateDuration + lastTimeAttackPauseWithDuration) {

				attackStatePaused = false;

				currentPauseAttackStateDuration = 0;
			} else {
				return;
			}
		}

		if (waitToActivateAttackActive) {
			return;
		}


		if (!canUseAttackActive) {
			return;
		}

//		if (mainFindObjectivesSystem.isOnSpotted ()) {
//			if (!onSpotted) {
//				lastTimeAttackActivated = Time.time;
//
//				onSpotted = true;
//			}
//		} else {
//			if (onSpotted) {
//
//				onSpotted = false;
//			}
//		}
//
//		if (onSpotted) {

		if (AIOnPlayerTeam) {
			return;
		}

		if (isCurrentCharacterTurn) {
			if (currentAttackInProcess) {
				if (!delayToActiveteAttackInProcess) {
					if (!mainCharacterToReceiveOrders.isOrderInProcess ()) {
						setNextTurn ();

						currentAttackInProcess = false;

						isCurrentCharacterTurn = false;

						delayToActiveteAttackInProcess = false;
					}
				}
			} else {
				if (Time.time > minWaitToActivateAttack + lastTimeAttackActivated) {
					if (!mainPlayerController.isActionActive () || blockActive) {
						bool canActivateAttackResult = true;

						if (checkDistanceToTargetEnabled) {
							if (!mainPlayerController.canPlayerMove () || currentPathDistanceToTarget > currentMinDistanceToAttack ||
							    !mainFindObjectivesSystem.checkIfMinimumAngleToAttack ()) {
								canActivateAttackResult = false;
							}
						}

						if (canActivateAttackResult) {
							if (mainFindObjectivesSystem.isAIBehaviorAttackInProcess ()) {
								if (showDebugPrint) {
									print ("attack in process in current main behavior, cancelling action");
								}

								canActivateAttackResult = false;
							}
						}

						if (canActivateAttackResult) {
							if (mainFindObjectivesSystem.isAIPaused ()) {
								canActivateAttackResult = false;
							}
						}

						if (canActivateAttackResult) {
							if (mainTurnBasedCombatActionsSystem.isAdjustingCharacterPositionInProcess ()) {
								canActivateAttackResult = false;
							}
						}

						if (canActivateAttackResult) {
							if (useProbabilityToUseAttack) {
								float currentProbability = Random.Range (0, 100);

								if (currentProbability > probabilityToUseAttack) {
									lastTimeAttackActivated = Time.time;

									if (showDebugPrint) {
										print ("probability to activate attack failed, cancelling");
									}

									return;
								}
							}

							attackTarget ();
						}
					}
				}
			}
//			}
		}
	}

	public void updateAIAttackState (bool newCanUseAttackActiveState)
	{
		canUseAttackActive = newCanUseAttackActiveState;
	}

	public void setSystemEnabledState (bool state)
	{
		if (systemEnabled == state) {
			return;
		}

		systemEnabled = state;

		setSystemActiveState (systemEnabled);
	}

	public void setSystemActiveState (bool state)
	{
		if (!systemEnabled) {
			return;
		}

		if (systemActive == state) {
			return;
		}

		systemActive = state;

		if (systemActive) {


		} else {
			isCurrentCharacterTurn = false;

			currentAttackInProcess = false;

			delayToActiveteAttackInProcess = false;
		
			blockActive = false;
		}

//		onSpotted = false;

		checkEventsOnCombatStateChange (systemActive);
	}

	void checkEventsOnCombatStateChange (bool state)
	{
		if (useEventsOnCombatActive) {
			if (state) {
				eventOnCombatActive.Invoke ();
			} else {
				eventOnCombatDeactivate.Invoke ();
			}
		}
	}

	public void pauseAttackDuringXTime (float newDuration)
	{
		currentPauseAttackStateDuration = newDuration;

		lastTimeAttackPauseWithDuration = Time.time;

		attackStatePaused = true;
	}

	public void setWaitToActivateAttackActiveState (bool state)
	{
		waitToActivateAttackActive = state;
	}

	public void resetBehaviorStates ()
	{
		resetStates ();

		waitingForAttackActive = false;



		insideMinDistanceToAttack = false;
	}

	public void attackTarget ()
	{
		if (showDebugPrint) {
			print ("activate attack");
		}


		int randomAttackCategoryIndex = Random.Range (0, turnBasedCombatAttackCategoryInfoList.Count);

		turnBasedCombatAttackCategoryInfo currentTurnBasedCombatAttackCategoryInfo = turnBasedCombatAttackCategoryInfoList [randomAttackCategoryIndex];

		bool checkCategoryResult = false;

		if (!ignoreIfLastCategoryPreviouslySelected) {
			if (lastCategoryIndex == randomAttackCategoryIndex) {
				checkCategoryResult = true;

				if (showDebugPrint) {
					print ("category selected on previous attack, selecting new one");
				}
			}
		}

		if (!currentTurnBasedCombatAttackCategoryInfo.attackCategoryEnabled) {
			checkCategoryResult = true;

			if (showDebugPrint) {
				print ("category not enabled, selecting new index " + currentTurnBasedCombatAttackCategoryInfo.Name);
			}
		}

		if (!ignoreProbabilityToCheckCategory) {
			if (currentTurnBasedCombatAttackCategoryInfo.useProbabilityToCheckCategory) {
				float currentProbability = Random.Range (0, 100);

				if (currentProbability > currentTurnBasedCombatAttackCategoryInfo.probabilityToCheckCategory) {
					checkCategoryResult = true;

					if (showDebugPrint) {
						print ("category probability not reached, selecting new index " + currentProbability);
					}
				} else {
					if (showDebugPrint) {
						print ("category probability reached, selecting command in category " + currentTurnBasedCombatAttackCategoryInfo.Name);
					}
				}
			}
		}

		if (checkCategoryResult) {
			bool nextIndexFound = false;

			int loopCount = 0;

			while (!nextIndexFound) {
				randomAttackCategoryIndex = Random.Range (0, turnBasedCombatAttackCategoryInfoList.Count);

				currentTurnBasedCombatAttackCategoryInfo = turnBasedCombatAttackCategoryInfoList [randomAttackCategoryIndex];

				if (currentTurnBasedCombatAttackCategoryInfo.attackCategoryEnabled) {
					if (ignoreProbabilityToCheckCategory) {
						nextIndexFound = true;
					} else {
						if (currentTurnBasedCombatAttackCategoryInfo.useProbabilityToCheckCategory) {
							float currentProbability = Random.Range (0, 100);

							if (currentProbability < currentTurnBasedCombatAttackCategoryInfo.probabilityToCheckCategory) {
								nextIndexFound = true;

								if (showDebugPrint) {
									print ("category probability reached");
								}
							}
						} else {
							nextIndexFound = true;
						}
					}
				}

				loopCount++;

				if (loopCount > 100) {
					nextIndexFound = true;
				}
			}
		}

		if (lastCategoryIndex != randomAttackCategoryIndex) {
			if (showDebugPrint) {
				print ("selecting new category, reseting las attack used index");
			}

			if (lastAttackUsedIndex != -1) {
				lastAttackUsedIndex = -1;
			}

			lastCategoryIndex = randomAttackCategoryIndex;
		}

		if (showDebugPrint) {
			print ("category selected " + currentTurnBasedCombatAttackCategoryInfo.Name);
		}

		int turnBasedCombatAttackInfoListCount = currentTurnBasedCombatAttackCategoryInfo.turnBasedCombatAttackInfoList.Count;

		int randomAttackIndex = Random.Range (0, turnBasedCombatAttackInfoListCount);

		currentTurnBasedCombatAttackInfo = currentTurnBasedCombatAttackCategoryInfo.turnBasedCombatAttackInfoList [randomAttackIndex];

		bool checkAttackIndexResult = true;

		if (!ignoreIfLastAttackPreviouslySelected) {
			if (randomAttackIndex == lastAttackUsedIndex) {
				checkAttackIndexResult = true;

				if (showDebugPrint) {
					print ("already used on last command " + currentTurnBasedCombatAttackInfo.Name);
				}
			}
		}

		if (!currentTurnBasedCombatAttackInfo.attackEnabled) {
			checkAttackIndexResult = true;

			if (showDebugPrint) {
				print ("command not enabled " + currentTurnBasedCombatAttackInfo.Name);
			}
		}

		if (!ignoreProbabilityToUseAttack) {
			if (currentTurnBasedCombatAttackInfo.useProbabilityToUseAttack) {
				float currentProbability = Random.Range (0, 100);

				if (currentProbability > currentTurnBasedCombatAttackInfo.probabilityToUseAttack) {
					checkAttackIndexResult = true;

					if (showDebugPrint) {
						print ("command probability not reached " + currentTurnBasedCombatAttackInfo.Name + "  " + currentProbability);
					}
				}
			}
		}

		if (checkAttackIndexResult) {
			bool nextIndexFound = false;

			int loopCount = 0;

			while (!nextIndexFound) {

				randomAttackIndex = Random.Range (0, turnBasedCombatAttackInfoListCount);

				if (randomAttackIndex != lastAttackUsedIndex) {
					currentTurnBasedCombatAttackInfo = currentTurnBasedCombatAttackCategoryInfo.turnBasedCombatAttackInfoList [randomAttackIndex];
					
					if (currentTurnBasedCombatAttackInfo.attackEnabled) {
						if (ignoreProbabilityToUseAttack) {
							bool canActivateCommandResult = true;

							if (!ignoreUseStatToUseAction) {
								if (currentTurnBasedCombatAttackInfo.useStatToUseAction) {
									if (mainPlayerStatsSystem.getStatValue (currentTurnBasedCombatAttackInfo.statNameToUseAction) <
									    currentTurnBasedCombatAttackInfo.statAmountToUseAction) {
										canActivateCommandResult = false;
									}
								}
							}
	
							if (canActivateCommandResult) {
								lastAttackUsedIndex = randomAttackIndex;

								nextIndexFound = true;
							}
						} else {
							if (currentTurnBasedCombatAttackInfo.useProbabilityToUseAttack) {
								float currentProbability = Random.Range (0, 100);

								if (currentProbability < currentTurnBasedCombatAttackCategoryInfo.probabilityToCheckCategory) {
									bool canActivateCommandResult = true;

									if (!ignoreUseStatToUseAction) {
										if (currentTurnBasedCombatAttackInfo.useStatToUseAction) {
											if (mainPlayerStatsSystem.getStatValue (currentTurnBasedCombatAttackInfo.statNameToUseAction) <
											    currentTurnBasedCombatAttackInfo.statAmountToUseAction) {
												canActivateCommandResult = false;
											}
										}
									}

									if (canActivateCommandResult) {
										lastAttackUsedIndex = randomAttackIndex;

										nextIndexFound = true;

										if (showDebugPrint) {
											print ("command probability reached " + currentProbability);
										}
									}
								}
							} else {
								bool canActivateCommandResult = true;

								if (!ignoreUseStatToUseAction) {
									if (currentTurnBasedCombatAttackInfo.useStatToUseAction) {
										if (mainPlayerStatsSystem.getStatValue (currentTurnBasedCombatAttackInfo.statNameToUseAction) <
										    currentTurnBasedCombatAttackInfo.statAmountToUseAction) {
											canActivateCommandResult = false;
										}
									}
								}

								if (canActivateCommandResult) {
									lastAttackUsedIndex = randomAttackIndex;

									nextIndexFound = true;
								}
							}
						}
					}
				}

				loopCount++;

				if (loopCount > 100) {
					lastAttackUsedIndex = randomAttackIndex;

					nextIndexFound = true;
				}
			}
		} else {
			lastAttackUsedIndex = randomAttackIndex;
		}

		currentTurnBasedCombatAttackInfo = currentTurnBasedCombatAttackCategoryInfo.turnBasedCombatAttackInfoList [lastAttackUsedIndex];
	
		activateAttack ();
	}

	void activateAttack ()
	{
		if (!ignoreUseStatToUseAction) {
			if (currentTurnBasedCombatAttackInfo.useStatToUseAction) {
				mainPlayerStatsSystem.usePlayerStat (currentTurnBasedCombatAttackInfo.statNameToUseAction,
					currentTurnBasedCombatAttackInfo.statAmountToUseAction);

				mainTurnBasedCombatActionsSystem.updateAllCharacterStatsUIValue ();
			}
		}

		currentCommandName = currentTurnBasedCombatAttackInfo.Name;

		mainTurnBasedCombatActionsSystem.setCurrentCommandNameUsed (currentCommandName);

		if (showDebugPrint) {
			print ("current command selected " + currentCommandName);
		}

		if (currentTurnBasedCombatAttackInfo.useMatchPositionSystem) {
			activateMatchPosition (currentTurnBasedCombatAttackInfo.matchPositionOffset);
		}

		delayToActiveteAttackInProcess = false;

		if (currentTurnBasedCombatAttackInfo.useDelayToActivateAttack) {
			stopDelaytToActivateAttackCoroutine ();

			activateAttackCoroutine = StartCoroutine (delaytToActivateAttackCoroutine (currentTurnBasedCombatAttackInfo.delayToActivateAttack));
		} else {
			mainCharacterToReceiveOrders.activateOrder (currentCommandName);
		}

		currentAttackInProcess = true;
	}

	Coroutine activateAttackCoroutine;

	IEnumerator delaytToActivateAttackCoroutine (float currentDelay)
	{
		delayToActiveteAttackInProcess = true;

		WaitForSeconds delay = new WaitForSeconds (currentDelay);

		yield return delay;

		mainCharacterToReceiveOrders.activateOrder (currentCommandName);

		delayToActiveteAttackInProcess = false;
	}

	void stopDelaytToActivateAttackCoroutine ()
	{
		if (activateAttackCoroutine != null) {
			StopCoroutine (activateAttackCoroutine);
		}
	}

	public void activateAttackByName (string attackName)
	{
		int turnBasedCombatAttackCategoryInfoListCount = turnBasedCombatAttackCategoryInfoList.Count;

		for (int i = 0; i < turnBasedCombatAttackCategoryInfoListCount; i++) {
			if (turnBasedCombatAttackCategoryInfoList [i].attackCategoryEnabled) {
				int turnBasedCombatAttackInfoListCount = turnBasedCombatAttackCategoryInfoList [i].turnBasedCombatAttackInfoList.Count;

				for (int j = 0; j < turnBasedCombatAttackInfoListCount; j++) {
					if (turnBasedCombatAttackCategoryInfoList [i].turnBasedCombatAttackInfoList [j].attackEnabled) {
						if (turnBasedCombatAttackCategoryInfoList [i].turnBasedCombatAttackInfoList [j].Name.Equals (attackName)) {
							currentTurnBasedCombatAttackInfo = turnBasedCombatAttackCategoryInfoList [i].turnBasedCombatAttackInfoList [j];

							lastAttackUsedIndex = j;
							lastCategoryIndex = i;

							activateAttack ();

							return;
						}
					}
				}
			}
		}
	}

	public void setBlockActiveState (bool state)
	{
		blockActive = state;
	}

	public void resetAttackState ()
	{

	}


	public void disableOnSpottedState ()
	{

	}

	public void setNewMinDistanceToAttack (float newValue)
	{
		currentMinDistanceToAttack = newValue;
	}

	public void setAIOnPlayerTeamState (bool state)
	{
		AIOnPlayerTeam = state;
	}

	public void setIsCurrentCharacterTurn (bool state)
	{
		isCurrentCharacterTurn = state;

		if (isCurrentCharacterTurn) {
			mainFindObjectivesSystem.setAIPausedState (false);

			mainFindObjectivesSystem.setPossibleTargetForTurnBasedCombatSystem (currentTarget);

			lastTimeAttackActivated = Time.time;
		} else {
			mainFindObjectivesSystem.setAIPausedState (true);

			mainFindObjectivesSystem.setPossibleTargetForTurnBasedCombatSystem (null);
		}

		if (isCurrentCharacterTurn) {
			if (useEventIfBlockActiveOnCurrentTurn) {
				blockActive = mainHealth.isBlockDamageActiveState ();

				if (showDebugPrint) {
					print ("block was active, calling event to disable state to activate turn combat attack");
				}

				if (blockActive) {
					eventIfBlockActiveOnCurrentTurn.Invoke (); 

					blockActive = false;
				}
			}
		}
	}

	public void setCurrentCharacterTurnTarget (GameObject newTarget)
	{
		currentTarget = newTarget;
	}

	public void setNextTurn ()
	{
		if (systemActive && isCurrentCharacterTurn) {
			mainTurnBasedCombatActionsSystem.setNextTurn ();
		}
	}

	public void setCurrentTurnBasedCombatAttackComplete ()
	{
		setNextTurn ();
	}

	public void activateMatchPosition (float customMatchOffset)
	{
		float currentMatchOffset = defaultMatchOffset;

		if (customMatchOffset > 0) {
			currentMatchOffset = customMatchOffset;
		}

		mainMatchPlayerToTargetSystem.activateMatchPosition (currentMatchOffset);
	}

	[System.Serializable]
	public class turnBasedCombatAttackCategoryInfo
	{
		public string Name;

		public bool attackCategoryEnabled = true;

		public bool useProbabilityToCheckCategory;
		[Range (0, 100)] public float probabilityToCheckCategory;

		[Space]

		public List<turnBasedCombatAttackInfo> turnBasedCombatAttackInfoList = new List<turnBasedCombatAttackInfo> ();
	}

	[System.Serializable]
	public class turnBasedCombatAttackInfo
	{
		public string Name;

		[Space]
		public bool attackEnabled = true;

		public float duration;

		[Space]

		public bool useProbabilityToUseAttack;
		[Range (0, 100)] public float probabilityToUseAttack;

		[Space]

		public float minDistanceToUseAttack;

		public bool isAttackFromDistance;

		[Space]

		public bool useDelayToActivateAttack;
		public float delayToActivateAttack;

		[Space]

		public bool useMatchPositionSystem;
		public float matchPositionOffset = 1;

		[Space]
		[Space]

		public bool useStatToUseAction;
		public string statNameToUseAction;
		public float statAmountToUseAction;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AIAroundManager : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool AIManagerEnabled = true;

	public float maxDistanceToSetStateOnAI;

	public bool onlyStoreAIAndIgnoreToSetStates;

	[Space]
	[Header ("AI Management Settings")]
	[Space]

	public float timeToAssignAttackToNextAI;

	[Space]
	[Header ("Dynamic Obstacle Detection Settings")]
	[Space]

	public bool useDynamicObstacleDetection;

	public bool useRandomWalkEnabled;

	public float minObstacleRotation = 45;
	public float mediumObstacleRotation = 65;
	public float maximumObstacleRotation = 85;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool sendInfoToTurnBasedCombatSystemEnabled;
	public string mainTurnBasedCombatSystemPanelName = "Turn Combat System";

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool pauseAttacksOnAllAIAround;

	public bool AIManagerActive;

	public int currentTargetsAmount;

	public int lastAIAttackAssignedIndex = -1;

	public List<Transform> charactersAround = new List<Transform> ();

	[Space]
	[Header ("Events")]
	[Space]

	public bool useEventsOnTargetsChange;
	public UnityEvent eventOnTargetsDetected;
	public UnityEvent eventOnTargetsRemoved;

	public bool useEventOnFirstTargetDetected;
	public UnityEvent eventOnFirstTargetDetected;

	public bool useEventOnLastTargetRemoved;
	public UnityEvent eventOnLastTargetRemoved;

	[Space]
	[Header ("Components")]
	[Space]

	public turnBasedCombatSystem mainTurnBasedCombatSystem;

	bool mainTurnBasedCombatSystemLocated;

	Coroutine updateCoroutine;

	List<findObjectivesSystem> findObjectivesSystemList = new List<findObjectivesSystem> ();

	float lastTimeAttackStateAssigned;


	public void stopUpdateCoroutine ()
	{
		if (updateCoroutine != null) {
			StopCoroutine (updateCoroutine);
		}
	}

	IEnumerator updateSystemCoroutine ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			updateSystem ();

			yield return waitTime;
		}
	}

	void updateSystem ()
	{
		if (Time.time > lastTimeAttackStateAssigned + timeToAssignAttackToNextAI) {
			updateAttackAssignState ();

			lastTimeAttackStateAssigned = Time.time;
		}
	}

	void updateAttackAssignState ()
	{
		bool indexSelected = false;

		int randomAIIndexToUse = -1;

		int counter = 0;

		while (!indexSelected) {
			randomAIIndexToUse = Random.Range (0, (currentTargetsAmount));

			if (lastAIAttackAssignedIndex == -1 || lastAIAttackAssignedIndex != randomAIIndexToUse) {
				indexSelected = true;

				lastAIAttackAssignedIndex = randomAIIndexToUse;
			}

			counter++;

			if (counter > 100) {
				if (showDebugPrint) {
					print ("COUNTER LOOP");

				}

				indexSelected = true;

				randomAIIndexToUse = 0;
			}
		}

		for (int i = 0; i < currentTargetsAmount; i++) {
			if (findObjectivesSystemList.Count > i) {
				if (randomAIIndexToUse == i) {
					if (!pauseAttacksOnAllAIAround) {
						findObjectivesSystemList [i].setWaitToActivateAttackActiveState (false);
					}

					if (useRandomWalkEnabled) {
						findObjectivesSystemList [i].setOriginalUseRandomWalkEnabledState ();
					}
				} else {
					findObjectivesSystemList [i].setWaitToActivateAttackActiveState (true);

					if (useRandomWalkEnabled) {
						findObjectivesSystemList [i].setUseRandomWalkEnabledState (true);
					}
				}

				if (useDynamicObstacleDetection) {
					findObjectivesSystemList [i].AINavMeshManager.setUseDynamicObstacleDetectionState (true);

					findObjectivesSystemList [i].AINavMeshManager.setMinObstacleRotation (minObstacleRotation);
					findObjectivesSystemList [i].AINavMeshManager.setMediumObstacleRotation (mediumObstacleRotation);
					findObjectivesSystemList [i].AINavMeshManager.setMaximumObstacleRotation (maximumObstacleRotation);
				}
			}
		}

		for (int i = charactersAround.Count - 1; i >= 0; i--) {	
			if (charactersAround [i] != null) {
				if (applyDamage.checkIfDead (charactersAround [i].gameObject) || charactersAround [i] == null) {
					charactersAround.RemoveAt (i);

					if (findObjectivesSystemList.Count > i) {
						findObjectivesSystemList.RemoveAt (i);
					}
				}
			}
		}

		currentTargetsAmount = charactersAround.Count;

		if (currentTargetsAmount <= 1) {
			if (AIManagerActive) {
				stopUpdateCoroutine ();

				AIManagerActive = false;
			}
		}

		if (charactersAround.Count == 0) {
			checkEventsOnTargetsChange (false);

			checkEventOnLastTargetRemoved ();

			lastTimeAttackStateAssigned = 0;

			lastAIAttackAssignedIndex = -1;
		}

		if (showDebugPrint) {
			print ("AI to attack selected " + randomAIIndexToUse.ToString ());
		}
	}

	public void addCharacterAround (Transform newCharacter)
	{
		if (!AIManagerEnabled) {
			return;
		}

		if (!charactersAround.Contains (newCharacter)) {
			charactersAround.Add (newCharacter);

			checkRemoveEmptyObjects ();

			if (charactersAround.Count == 1) {
				checkEventOnFirstTargetDetected ();
			}

			checkEventsOnTargetsChange (true);

			if (showDebugPrint) {
				print ("Adding target " + newCharacter.name);
			}

			playerComponentsManager currentPlayerComponentsManager = newCharacter.GetComponent<playerComponentsManager> ();

			findObjectivesSystem currentFindObjectivesSystem = null;

			if (currentPlayerComponentsManager != null) {
				currentFindObjectivesSystem = currentPlayerComponentsManager.getFindObjectivesSystem ();

				if (currentFindObjectivesSystem != null) {
					findObjectivesSystemList.Add (currentFindObjectivesSystem);
				}
			}

			currentTargetsAmount = charactersAround.Count;

			if (onlyStoreAIAndIgnoreToSetStates) {
				return;
			}

			if (!AIManagerActive) {
				if (charactersAround.Count >= 2) {
					stopUpdateCoroutine ();

					updateCoroutine = StartCoroutine (updateSystemCoroutine ());

					AIManagerActive = true;
				}
			} else {
				if (currentFindObjectivesSystem != null) {
					currentFindObjectivesSystem.setWaitToActivateAttackActiveState (true);

					if (useDynamicObstacleDetection) {
						currentFindObjectivesSystem.AINavMeshManager.setUseDynamicObstacleDetectionState (true);

						currentFindObjectivesSystem.AINavMeshManager.setMinObstacleRotation (minObstacleRotation);
						currentFindObjectivesSystem.AINavMeshManager.setMediumObstacleRotation (mediumObstacleRotation);
						currentFindObjectivesSystem.AINavMeshManager.setMaximumObstacleRotation (maximumObstacleRotation);
					}

					if (useRandomWalkEnabled) {
						currentFindObjectivesSystem.setUseRandomWalkEnabledState (true);
					}
				}
			}
		}
	}

	public void removeCharacterAround (Transform newCharacter)
	{
		if (!AIManagerEnabled) {
			return;
		}

		if (charactersAround.Contains (newCharacter)) {
			charactersAround.Remove (newCharacter);

			checkRemoveEmptyObjects ();

			if (showDebugPrint) {
				print ("Removing target " + newCharacter.name);
			}
		}

		playerComponentsManager currentPlayerComponentsManager = newCharacter.GetComponent<playerComponentsManager> ();

		if (currentPlayerComponentsManager != null) {
			findObjectivesSystem currentFindObjectivesSystem = currentPlayerComponentsManager.getFindObjectivesSystem ();

			if (currentFindObjectivesSystem != null) {
				if (!onlyStoreAIAndIgnoreToSetStates) {
					if (!pauseAttacksOnAllAIAround) {
						currentFindObjectivesSystem.setWaitToActivateAttackActiveState (false);
					}

					if (useDynamicObstacleDetection) {
						currentFindObjectivesSystem.AINavMeshManager.setOriginalUseDynamicObstacleDetection ();
					}

					if (useRandomWalkEnabled) {
						currentFindObjectivesSystem.setOriginalUseRandomWalkEnabledState ();
					}
				}

				if (findObjectivesSystemList.Contains (currentFindObjectivesSystem)) {
					findObjectivesSystemList.Remove (currentFindObjectivesSystem);
				}
			}
		}

		currentTargetsAmount = charactersAround.Count;

		if (currentTargetsAmount <= 1) {
			if (AIManagerActive) {
				stopUpdateCoroutine ();

				AIManagerActive = false;
			}
		}

		lastTimeAttackStateAssigned = 0;

		lastAIAttackAssignedIndex = -1;

		if (charactersAround.Count == 0) {
			checkEventsOnTargetsChange (false);

			checkEventOnLastTargetRemoved ();
		}
	}

	public void clearCharactersAround ()
	{
		if (!AIManagerEnabled) {
			return;
		}
			
		checkRemoveEmptyObjects ();

		currentTargetsAmount = charactersAround.Count;

		for (int i = 0; i < currentTargetsAmount; i++) {
			playerComponentsManager currentPlayerComponentsManager = charactersAround [i].GetComponent<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
				findObjectivesSystem currentFindObjectivesSystem = currentPlayerComponentsManager.getFindObjectivesSystem ();

				if (currentFindObjectivesSystem != null) {
					if (!onlyStoreAIAndIgnoreToSetStates) {
						if (!pauseAttacksOnAllAIAround) {
							currentFindObjectivesSystem.setWaitToActivateAttackActiveState (false);
						}

						if (useDynamicObstacleDetection) {
							currentFindObjectivesSystem.AINavMeshManager.setOriginalUseDynamicObstacleDetection ();
						}

						if (useRandomWalkEnabled) {
							currentFindObjectivesSystem.setOriginalUseRandomWalkEnabledState ();
						}
					}

					if (findObjectivesSystemList.Contains (currentFindObjectivesSystem)) {
						findObjectivesSystemList.Remove (currentFindObjectivesSystem);
					}
				}
			}
		}

		charactersAround.Clear ();

		currentTargetsAmount = charactersAround.Count;

		if (currentTargetsAmount <= 1) {
			if (AIManagerActive) {
				stopUpdateCoroutine ();

				AIManagerActive = false;
			}
		}

		lastTimeAttackStateAssigned = 0;

		lastAIAttackAssignedIndex = -1;

		if (charactersAround.Count == 0) {
			checkEventsOnTargetsChange (false);

			checkEventOnLastTargetRemoved ();
		}
	}

	void checkEventsOnTargetsChange (bool state)
	{
		if (useEventsOnTargetsChange) {
			if (state) {
				eventOnTargetsDetected.Invoke ();
			} else {
				eventOnTargetsRemoved.Invoke ();
			}
		}

		if (sendInfoToTurnBasedCombatSystemEnabled) {
			if (!mainTurnBasedCombatSystemLocated) {
				mainTurnBasedCombatSystemLocated = mainTurnBasedCombatSystem != null;

				if (!mainTurnBasedCombatSystemLocated) {
					mainTurnBasedCombatSystem = turnBasedCombatSystem.Instance;

					mainTurnBasedCombatSystemLocated = mainTurnBasedCombatSystem != null;
				}

				if (!mainTurnBasedCombatSystemLocated) {
					GKC_Utils.instantiateMainManagerOnSceneWithTypeOnApplicationPlaying (mainTurnBasedCombatSystemPanelName, typeof(turnBasedCombatSystem), true);

					mainTurnBasedCombatSystem = turnBasedCombatSystem.Instance;

					mainTurnBasedCombatSystemLocated = mainTurnBasedCombatSystem != null;
				}

				if (!mainTurnBasedCombatSystemLocated) {
					
					mainTurnBasedCombatSystem = FindObjectOfType<turnBasedCombatSystem> ();

					mainTurnBasedCombatSystemLocated = mainTurnBasedCombatSystem != null;
				} 
			}

			if (mainTurnBasedCombatSystemLocated) {
				if (state) {
					mainTurnBasedCombatSystem.enableWaitTimeToActivateTurnBasedCombat ();
				} else {
					mainTurnBasedCombatSystem.disableWaitTimeToActivateTurnBasedCombat ();
				}
			}
		}
	}

	void checkEventOnFirstTargetDetected ()
	{
		if (useEventOnFirstTargetDetected) {
			eventOnFirstTargetDetected.Invoke ();
		}
	}

	void checkEventOnLastTargetRemoved ()
	{
		if (useEventOnLastTargetRemoved) {
			eventOnLastTargetRemoved.Invoke ();
		}
	}

	void checkRemoveEmptyObjects ()
	{
		for (int i = charactersAround.Count - 1; i >= 0; i--) {	
			if (charactersAround [i] == null) {
				charactersAround.RemoveAt (i);

				if (findObjectivesSystemList.Count > i) {
					findObjectivesSystemList.RemoveAt (i);
				}
			}
		}
	}

	public List<Transform> getCharactersAround ()
	{
		return charactersAround;
	}

	public bool characterDetectedByAIAround ()
	{
		int findObjectivesSystemListCount = findObjectivesSystemList.Count;

		for (int i = 0; i < findObjectivesSystemListCount; i++) {
			if (findObjectivesSystemList [i] != null) {
				if (findObjectivesSystemList [i].isOnSpotted ()) {
					return true;
				}
			}
		}

		return false;
	}

	public bool checkIfPlayerDetectedByAIAround (GameObject mainPlayerGameObject)
	{
		if (mainPlayerGameObject == null) {
			return false;
		}

		int findObjectivesSystemListCount = findObjectivesSystemList.Count;

		if (findObjectivesSystemListCount == 0) {
			return false;
		}

		for (int i = 0; i < findObjectivesSystemListCount; i++) {
			if (findObjectivesSystemList [i] != null) {
				if (findObjectivesSystemList [i].isOnSpotted () && findObjectivesSystemList [i].getCurrentTargetToAttack () == mainPlayerGameObject) {
					return true;
				}
			}
		}

		return false;
	}

	public float getMinDistanceToPlayer (Transform playerTransform, bool ignoreIfPlayerIsCurrentTarget)
	{
		if (playerTransform == null) {
			return -1;
		}

		int findObjectivesSystemListCount = findObjectivesSystemList.Count;

		if (findObjectivesSystemListCount == 0) {
			return -1;
		}

		float minDistance = Mathf.Infinity;

		bool distanceFound = false;

		Vector3 playerPosition = playerTransform.position;

		for (int i = 0; i < findObjectivesSystemListCount; i++) {
			if (findObjectivesSystemList [i] != null) {
				if (findObjectivesSystemList [i].isOnSpotted ()) {
					if (ignoreIfPlayerIsCurrentTarget) {
						float currentDistance = GKC_Utils.distance (findObjectivesSystemList [i].transform.position, playerPosition);

						if (currentDistance < minDistance) {
							minDistance = currentDistance;

							distanceFound = true;
						}
					} else {
						if (findObjectivesSystemList [i].getCurrentTargetToAttack () == playerTransform.gameObject) {

							float currentDistance = GKC_Utils.distance (findObjectivesSystemList [i].transform.position, playerPosition);

							if (currentDistance < minDistance) {
								minDistance = currentDistance;

								distanceFound = true;
							}
						}
					}
				}
			}
		}

		if (distanceFound) {
			return minDistance;
		} else {
			return -1;
		}
	}
}

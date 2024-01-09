using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class turnBasedCombatActionsSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public int characterTargetPriority;

	public Transform mainTransform;

	public Transform playerCameraTrnsform;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public string menuPanelName = "Turn Combat System";

	public float movementSpeed = 10;

	[Space]
	[Header ("Commands Settings")]
	[Space]

	public bool useBlockedCommandList;
	public List<string> blockedCommandList = new List<string> ();

	[Space]

	public bool useBlockedCommandCategoryList;
	public List<string> blockedCommandCategoryList = new List<string> ();

	[Space]
	[Header ("Stats Settings")]
	[Space]

	public List<string> characterStatsToShowList = new List<string> ();

	public bool addAmountToCharacterStatsOnCombat;
	public List<simpleStatInfo> simpleStatInfoList = new List<simpleStatInfo> ();

	[Space]
	[Header ("Target Selection Settings")]
	[Space]

	public bool selectTargetInOrder = true;
	public bool selectRandomTarget;
	public bool selectTargetByCharacterPriority;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool turnBasedCombatActionActive;

	public Vector3 currentCombatPosition;
	public Vector3 currentCombatRotation;

	public bool adjustingCharacterPositionInProcess;

	public bool freeCombatActive;

	public bool mainTurnBasedCombatSystemAssigned;

	[Space]
	[Header ("Components")]
	[Space]

	public remoteEventSystem mainRemoteEventSystem;

	public characterToReceiveOrders mainCharacterToReceiveOrders;

	public turnBasedCombatSystem mainTurnBasedCombatSystem;

	public AITurnBasedCombatSystemBrain mainAITurnBasedCombatSystemBrain;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public bool useEventOnCombatActive;
	public UnityEvent eventOnCombatActive;

	[Space]

	public bool useEventOnCombatDeactivate;
	public UnityEvent eventOnCombatDeactivate;

	[Space]

	public bool useEventsIfCharacterAliveAfterCombat;
	public UnityEvent eventsIfCharacterAliveAfterCombat;

	[Space]

	public bool useEventsIfCharacterDeadAfterCombat;
	public UnityEvent eventsIfCharacterDeadAfterCombat;

	[Space]

	public bool useEventsOnCharacterCurrentTurnStart;
	public UnityEvent eventsOnCharacterCurrentTurnStart;

	[Space]

	public bool useEventOnCurrentCharacterTurn;
	public UnityEvent eventOnCurrentCharacterTurn;

	[Space]

	public bool useEventOnCurrentTeamTurn;
	public UnityEvent eventOnCurrentTeamTurn;

	[Space]

	public bool useEventOnEachTurn;
	public UnityEvent eventOnEachTurn;


	Coroutine movementCoroutine;


	public bool isTurnBasedCombatActionActive ()
	{
		return turnBasedCombatActionActive;
	}

	public void setTurnBasedCombatActionActiveState (bool state)
	{
		turnBasedCombatActionActive = state;

		if (turnBasedCombatActionActive) {
			initializeMainTurnBasedCombatSystem ();

			if (useEventOnCombatActive) {
				eventOnCombatActive.Invoke ();
			}

		} else {


			if (useEventOnCombatDeactivate) {
				eventOnCombatDeactivate.Invoke ();
			}
		}
	}

	public void setCurrentCombatPositionAndRotationValues (Vector3 currentCombatPositionValues, Vector3 currentCombatRotationValues)
	{
		currentCombatPosition = currentCombatPositionValues;
		currentCombatRotation = currentCombatRotationValues;
	}

	public void resetToCurrrentCombatPositionAndRotation (bool setPositionAtOnce)
	{
		if (setPositionAtOnce) {
			setCharacterPositionAtonce (currentCombatPosition, currentCombatRotation, true);
		} else {
			activateCharacterMovement (currentCombatPosition, currentCombatRotation, true);
		}
	}

	public void setCharacterPositionAtonce (Vector3 targetPosition, Vector3 targetRotation, bool adjustPlayerCameraTransform)
	{
		stopMovement ();

		mainTransform.localPosition = targetPosition;
		mainTransform.localRotation = Quaternion.Euler (targetRotation);

		if (adjustPlayerCameraTransform) {
			playerCameraTrnsform.localPosition = targetPosition;
			playerCameraTrnsform.localRotation = Quaternion.Euler (targetRotation);
		}
	}

	public void activateCharacterMovement (Vector3 targetPosition, Vector3 targetRotation, bool adjustPlayerCameraTransform)
	{
		stopMovement ();

		movementCoroutine = StartCoroutine (activateCharacterMovementCoroutine (targetPosition, targetRotation, adjustPlayerCameraTransform));
	}

	IEnumerator activateCharacterMovementCoroutine (Vector3 targetPosition, Vector3 targetEuler, bool adjustPlayerCameraTransform)
	{
		adjustingCharacterPositionInProcess = true;

		float dist = GKC_Utils.distance (mainTransform.position, targetPosition);

		float duration = dist / movementSpeed;

		float t = 0;

		float movementTimer = 0;

		bool targetReached = false;

		float angleDifference = 0;

		float positionDifference = 0;

		Quaternion targetRotation = Quaternion.Euler (targetEuler);

		while (!targetReached) {
			t += Time.deltaTime / duration; 

			mainTransform.localPosition = Vector3.Lerp (mainTransform.localPosition, targetPosition, t);
			mainTransform.localRotation = Quaternion.Lerp (mainTransform.localRotation, targetRotation, t);

			if (adjustPlayerCameraTransform) {
				playerCameraTrnsform.localPosition = Vector3.Lerp (playerCameraTrnsform.localPosition, targetPosition, t);
				playerCameraTrnsform.localRotation = Quaternion.Lerp (playerCameraTrnsform.localRotation, targetRotation, t);
			}

			angleDifference = Quaternion.Angle (mainTransform.localRotation, targetRotation);

			positionDifference = GKC_Utils.distance (mainTransform.localPosition, targetPosition);

			movementTimer += Time.deltaTime;

			if ((positionDifference < 0.01f && angleDifference < 0.2f) || movementTimer > (duration + 0.4f)) {
				targetReached = true;
			}

			yield return null;
		}

		adjustingCharacterPositionInProcess = false;
	}

	public void stopMovement ()
	{
		if (movementCoroutine != null) {
			StopCoroutine (movementCoroutine);
		}

		adjustingCharacterPositionInProcess = false;
	}

	public void activateOrder (string orderName)
	{
		mainCharacterToReceiveOrders.activateOrder (orderName);
	}

	public void activateOrderToAIBrain (string orderName)
	{
		mainAITurnBasedCombatSystemBrain.activateAttackByName (orderName);
	}

	void initializeMainTurnBasedCombatSystem ()
	{
		if (!mainTurnBasedCombatSystemAssigned) {

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
		}
	}

	public bool isAdjustingCharacterPositionInProcess ()
	{
		return adjustingCharacterPositionInProcess;
	}

	public void checkEventsForAliveOrDeadAfterCombat (bool state)
	{
		if (state) {
			if (useEventsIfCharacterAliveAfterCombat) {
				eventsIfCharacterAliveAfterCombat.Invoke ();
			}
		} else {
			if (useEventsIfCharacterDeadAfterCombat) {
				eventsIfCharacterDeadAfterCombat.Invoke ();
			}
		}
	}

	public void checkEventsOnCharacterCurrentTurnStart ()
	{
		if (useEventsOnCharacterCurrentTurnStart) {
			eventsOnCharacterCurrentTurnStart.Invoke ();
		}
	}

	public void checkEventOnCurrentCharacterTurn ()
	{
		if (useEventOnCurrentCharacterTurn) {
			eventOnCurrentCharacterTurn.Invoke ();
		}
	}

	public void checkEventOnCurrentTeamTurn ()
	{
		if (useEventOnCurrentTeamTurn) {
			eventOnCurrentTeamTurn.Invoke ();
		}
	}

	public void checkEventOnEachTurn ()
	{
		if (useEventOnEachTurn) {
			eventOnEachTurn.Invoke ();
		}
	}

	public void checkTurnBasedCombatOnDamageReceived ()
	{
		if (turnBasedCombatActionActive) {
			if (mainTurnBasedCombatSystemAssigned) {
				updateAllCharacterStatsUIValue ();

				activateEffect ("");
			}
		}
	}

	public void updateAllCharacterStatsUIValue ()
	{
		if (turnBasedCombatActionActive) {
			if (mainTurnBasedCombatSystemAssigned) {
				mainTurnBasedCombatSystem.updateAllCharacterStatsUIValue ();
			}
		}
	}

	public void setNextTurn ()
	{
		if (turnBasedCombatActionActive) {
			if (mainTurnBasedCombatSystemAssigned) {
				mainTurnBasedCombatSystem.setNextTurn ();
			}
		}
	}

	public void setCurrentCommandNameUsed (string commandName)
	{
		if (turnBasedCombatActionActive) {
			if (mainTurnBasedCombatSystemAssigned) {
				mainTurnBasedCombatSystem.setCurrentCommandNameUsed (commandName);
			}
		}
	}

	public void activateEffect (string effectName)
	{
		if (turnBasedCombatActionActive) {
			if (mainTurnBasedCombatSystemAssigned) {
				mainTurnBasedCombatSystem.activateEffect (effectName);
			}
		}
	}

	public void setFreeCombatActiveState (bool state)
	{
		freeCombatActive = state;
	}

	public bool checkIfCommandCanBeUsed (string commandName)
	{
		if (useBlockedCommandList) {
			return !blockedCommandList.Contains (commandName);
		}

		return true;
	}

	public bool checkIfCommandCategoryCanBeUsed (string commandCategoryName)
	{
		if (useBlockedCommandCategoryList) {
			return !blockedCommandCategoryList.Contains (commandCategoryName);
		}

		return true;
	}

	public void checkTeamsDeadStateAfterCharacterDeath ()
	{
		if (turnBasedCombatActionActive) {
			if (mainTurnBasedCombatSystemAssigned) {
				mainTurnBasedCombatSystem.checkTeamsDeadStateAfterCharacterDeath (mainTransform);
			}
		}
	}

	public void checkCharacterStateAfterResurrect ()
	{
		if (turnBasedCombatActionActive) {
			if (mainTurnBasedCombatSystemAssigned) {
				mainTurnBasedCombatSystem.checkCharacterStateAfterResurrect (mainTransform);
			}
		}
	}

	public void checkPlayerStateOnDeathDuringCombat ()
	{
		if (turnBasedCombatActionActive) {
			if (mainTurnBasedCombatSystemAssigned) {
				mainTurnBasedCombatSystem.checkPlayerStateOnDeathDuringCombat ();
			}
		}
	}

	//INPUT FUNCTIONS
	public void inputConfirmCommand ()
	{
		if (turnBasedCombatActionActive) {
			if (mainTurnBasedCombatSystemAssigned) {
				mainTurnBasedCombatSystem.inputConfirmCommand ();
			}
		}
	}

	public void inputCancelCommand ()
	{
		if (turnBasedCombatActionActive) {
			if (mainTurnBasedCombatSystemAssigned) {
				mainTurnBasedCombatSystem.inputCancelCommand ();
			}
		}
	}

	public void inputSelectNextTarget ()
	{
		if (turnBasedCombatActionActive) {
			if (mainTurnBasedCombatSystemAssigned) {
				mainTurnBasedCombatSystem.inputSelectNextTarget ();
			}
		}
	}

	public void inputSelectPreviousTarget ()
	{
		if (turnBasedCombatActionActive) {
			if (mainTurnBasedCombatSystemAssigned) {
				mainTurnBasedCombatSystem.inputSelectPreviousTarget ();
			}
		}
	}

	public void inputToggleCombatMode ()
	{
		if (turnBasedCombatActionActive || freeCombatActive) {
			if (mainTurnBasedCombatSystemAssigned) {
				mainTurnBasedCombatSystem.inputToggleCombatMode ();
			}
		}
	}

	[System.Serializable]
	public class simpleStatInfo
	{
		public string statName;
		public float statAmountToAdd;
	}
}

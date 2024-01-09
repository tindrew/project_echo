using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class turnBasedCombatSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool turnBasedCombatEnabled = true;

	public float minWaitTimeToActiveCombatOnSpotted = 2;

	public float minExtraWaitToReactivateCombatAfterRun = 3;

	public LayerMask layerToAdjustToGround = 1 << 0 | 1 << 23;

	public float minDistanceToPlayerToStartCombat = 10;

	[Space]
	[Header ("Camera Settings")]
	[Space]

	public string defaultCameraStateNameOnTurnBasedCombatActive = "Default Turn Based Combat Camera";
	public string defaultCameraStateNameOnReturnCameraToPlayer = "Return Camera To Player";

	[Space]

	public List<turnBasedCombatCameraInfo> turnBasedCombatCameraInfoList = new List<turnBasedCombatCameraInfo> ();

	[Space]

	public List<turnBasedCombatTeamPositionsInfoData> turnBasedCombatTeamPositionsInfoDataList = new List<turnBasedCombatTeamPositionsInfoData> ();

	[Space]
	[Header ("Effect Settings")]
	[Space]

	public string defaultEffectName = "Default Effect";

	[Space]

	public List<turnBasedCombatEffectInfo> turnBasedCombatEffectInfoList = new List<turnBasedCombatEffectInfo> ();

	[Space]
	[Header ("Respawn Position Settings")]
	[Space]

	public List<Transform> positionToRespawnList = new List<Transform> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool turnBasedCombatActive;

	public bool checkingMinWaitTimeToActiveCombatOnSpotted;

	public bool freeCombatActive;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject mainPlayer;

	public Camera mainCamera;

	public playerCamera playerCameraManager;

	public turnBasedCombatUISystem mainTurnBasedCombatUISystem;

	public AIAroundManager mainAIAroundManager;

	public friendListManager mainFriendListManager;

	[HideInInspector] public GameObject newCharacterTeamLeader;

	[HideInInspector] public string teamPositionDataName;
	[HideInInspector] public List<GameObject> newCharacterTeamInfoList = new List<GameObject> ();
	[HideInInspector] public bool newTeamAlwaysSelectFirst;
	[HideInInspector] public bool useRewardSystem;
	[HideInInspector] public objectExperienceSystem mainRewardSystem;

	[HideInInspector] public bool useCustomCameraStateOnNewTeam;

	[HideInInspector] public string customCameraStateOnNewTeam;

	[HideInInspector] public List<turnBasedCombatTeamInfo> turnBasedCombatTeamInfoOnSceneList = new List<turnBasedCombatTeamInfo> ();


	Transform cameraParentTransform;
	Vector3 mainCameraTargetPosition;
	Quaternion mainCameraTargetRotation;

	Coroutine cameraState;

	playerComponentsManager mainPlayerComponentsManager;

	turnBasedCombatCameraInfo currentTurnBasedCombatCameraInfo;

	Coroutine updateCoroutine;

	float lastTimeCheckingMinWaitTimeToActiveCombatOnSpotted;

	bool mainTurnBasedCombatUISystemLocated;

	bool useCustomCameraState;

	string customCameraState;

	float lastTimeCombatRunActive = -1;

	List<GameObject> currentPlayerFriendList = new List<GameObject> ();

	int minTeamAmountToCheck;


	private static turnBasedCombatSystem _turnBasedCombatSystemInstance;

	public static turnBasedCombatSystem Instance { get { return _turnBasedCombatSystemInstance; } }

	bool instanceInitialized;


	public void getComponentInstance ()
	{
		if (instanceInitialized) {
			return;
		}

		if (_turnBasedCombatSystemInstance != null && _turnBasedCombatSystemInstance != this) {
			Destroy (this.gameObject);

			return;
		} 

		_turnBasedCombatSystemInstance = this;

		instanceInitialized = true;
	}

	void Awake ()
	{
		getComponentInstance ();
	}

	void Start ()
	{
		if (mainPlayer == null) {
			findMainPlayer ();
		}
	}

	void findMainPlayer ()
	{
		if (mainPlayer == null) {
			mainPlayer = GKC_Utils.findMainPlayerOnScene ();
		}

		if (mainPlayer != null) {
			setCurrentPlayer (mainPlayer);
		}
	}

	public void adjustTurnBasedCombatPositionToPlayer ()
	{
		if (mainPlayer == null) {
			findMainPlayer ();
		}

		RaycastHit hit;

		Vector3 targetPosition = Vector3.zero;

		for (int i = 0; i < positionToRespawnList.Count; i++) {
			if (positionToRespawnList [i] != null) {
				positionToRespawnList [i].SetParent (null);
			}

			targetPosition = positionToRespawnList [i].position + positionToRespawnList [i].up * 3;

			if (Physics.Raycast (targetPosition, -Vector3.up, out hit, 200, layerToAdjustToGround)) {
				targetPosition = hit.point;
			}

			positionToRespawnList [i].position = targetPosition;
		}

		targetPosition = mainPlayer.transform.position + mainPlayer.transform.up * 3;

		if (Physics.Raycast (targetPosition, -Vector3.up, out hit, 200, layerToAdjustToGround)) {
			targetPosition = hit.point;
		}

		transform.position = targetPosition;
		transform.rotation = playerCameraManager.transform.rotation;
	}

	public void enableWaitTimeToActivateTurnBasedCombat ()
	{
		if (!turnBasedCombatEnabled) {
			return;
		}

		if (freeCombatActive) {
			return;
		}

		if (!checkingMinWaitTimeToActiveCombatOnSpotted) {
			checkingMinWaitTimeToActiveCombatOnSpotted = true;

			lastTimeCheckingMinWaitTimeToActiveCombatOnSpotted = Time.time;

			if (currentPlayerFriendList != null) {
				currentPlayerFriendList.Clear ();
			}

			currentPlayerFriendList = mainFriendListManager.getAllFriendList ();

			stopUpdateCoroutine ();

			updateCoroutine = StartCoroutine (updateSystemCoroutine ());
		}
	}

	public void disableWaitTimeToActivateTurnBasedCombat ()
	{
		if (!turnBasedCombatEnabled) {
			return;
		}

		if (freeCombatActive) {
			return;
		}

		if (checkingMinWaitTimeToActiveCombatOnSpotted) {
			checkingMinWaitTimeToActiveCombatOnSpotted = false;

			stopUpdateCoroutine ();
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
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			if (checkingMinWaitTimeToActiveCombatOnSpotted) {
				bool checkStateResult = true;

				if (lastTimeCombatRunActive != -1) {
					if (Time.time < minExtraWaitToReactivateCombatAfterRun + lastTimeCombatRunActive) {
						checkStateResult = false;
					}
				}

				if (checkStateResult) {
					if (Time.time > lastTimeCheckingMinWaitTimeToActiveCombatOnSpotted + minWaitTimeToActiveCombatOnSpotted) {
						bool checkActivateCombatResult = false;

						if (mainAIAroundManager.checkIfPlayerDetectedByAIAround (mainPlayer)) {

							float distanceToPlayer = mainAIAroundManager.getMinDistanceToPlayer (mainPlayer.transform, false);

							if (distanceToPlayer < minDistanceToPlayerToStartCombat) {
								checkActivateCombatResult = true;
							}
						} else {
							if (currentPlayerFriendList != null) {
								int currentPlayerFriendListCount = currentPlayerFriendList.Count;

								for (int i = 0; i < currentPlayerFriendListCount; i++) {
									if (mainAIAroundManager.checkIfPlayerDetectedByAIAround (currentPlayerFriendList [i])) {

										float distanceToPlayer = mainAIAroundManager.getMinDistanceToPlayer (mainPlayer.transform, true);

										if (distanceToPlayer < minDistanceToPlayerToStartCombat) {
											checkActivateCombatResult = true;
										}
									}
								}
							}
						}

						if (checkActivateCombatResult) {
							checkingMinWaitTimeToActiveCombatOnSpotted = false;

							stopUpdateCoroutine ();

							activateTurnBasedCombat ();
						}
					}
				}
			} else {

			}

			yield return waitTime;
		}
	}

	public void updateLastTimeCombatRunActive (float value)
	{
		lastTimeCombatRunActive = value;
	}

	public void clearCharactersAround ()
	{
		mainAIAroundManager.clearCharactersAround ();
	}

	public void activateTurnBasedCombat ()
	{
		activateOrDeactivateTurnBasedCombat (true);
	}

	public void deactivateTurnBasedCombat ()
	{
		activateOrDeactivateTurnBasedCombat (false);
	}

	void activateOrDeactivateTurnBasedCombat (bool state)
	{
		if (!mainTurnBasedCombatUISystemLocated) {
			mainTurnBasedCombatUISystemLocated = mainTurnBasedCombatUISystem != null;

			if (!mainTurnBasedCombatUISystemLocated) {
				mainTurnBasedCombatUISystem = FindObjectOfType<turnBasedCombatUISystem> ();

				mainTurnBasedCombatUISystemLocated = mainTurnBasedCombatUISystem != null;
			}
		}

		if (mainTurnBasedCombatUISystemLocated) {
			if (state) {
				mainTurnBasedCombatUISystem.checkMainPlayerStateBeforeActivateCombat ();
			}

			mainTurnBasedCombatUISystem.openOrCloseMenuPanel (state);
		}
	}

	public void setTurnBasedCombatActiveState (bool state)
	{
		if (turnBasedCombatEnabled) {
			turnBasedCombatActive = state;

			if (turnBasedCombatActive) {					
				if (mainPlayer == null) {
					findMainPlayer ();
				}

				if (mainPlayer != null) {
					if (useCustomCameraState) {
						setCameraState (customCameraState);
					} else {
						setCameraState (defaultCameraStateNameOnTurnBasedCombatActive);
					}

				} else {
					print ("WARNING: Main Player not found on scene");

					return;
				}

				if (currentPlayerFriendList != null) {
					currentPlayerFriendList.Clear ();
				}

				currentPlayerFriendList = mainFriendListManager.getAllFriendList ();

				updateCoroutine = StartCoroutine (updateSystemCoroutine ());
			} else {
				setCameraState (defaultCameraStateNameOnReturnCameraToPlayer);

				stopUpdateCoroutine ();
			}
		}
	}

	public void setMinTeamAmountToCheck (int newAmount)
	{
		minTeamAmountToCheck = newAmount;
	}

	void setCameraState (string cameraStateName)
	{
		int cameraStateIndex = turnBasedCombatCameraInfoList.FindIndex (s => s.Name.Equals (cameraStateName));

		if (cameraStateIndex == -1) {
			for (int i = 0; i < turnBasedCombatCameraInfoList.Count; i++) {
				if (cameraStateIndex == -1) {
					if (turnBasedCombatCameraInfoList [i].minTeamAmountToCheck != 0 &&
					    turnBasedCombatCameraInfoList [i].minTeamAmountToCheck >= minTeamAmountToCheck) {
						cameraStateIndex = i;
					}
				}
			}
		}

		if (cameraStateIndex > -1) {

			currentTurnBasedCombatCameraInfo = turnBasedCombatCameraInfoList [cameraStateIndex];

			mainCameraTargetRotation = Quaternion.identity;
			mainCameraTargetPosition = Vector3.zero;

			if (currentTurnBasedCombatCameraInfo.setCameraBackToPlayer) {
				if (cameraParentTransform != null) {
					mainCamera.transform.SetParent (cameraParentTransform);

					cameraParentTransform = null;
				}
			} else {
				if (cameraParentTransform == null) {
					cameraParentTransform = mainCamera.transform.parent;
				}

				mainCamera.transform.SetParent (currentTurnBasedCombatCameraInfo.cameraPosition);
			}

			if (currentTurnBasedCombatCameraInfo.smoothCameraMovement) {
				stopMovement ();

				cameraState = StartCoroutine (adjustCamera ());
			} else {
				mainCamera.transform.localRotation = mainCameraTargetRotation;
				mainCamera.transform.localPosition = mainCameraTargetPosition;
			}
		}
	}

	IEnumerator adjustCamera ()
	{
		Transform mainCameraTransform = mainCamera.transform;

		if (currentTurnBasedCombatCameraInfo.useFixedLerpMovement) {
			float i = 0;
			//store the current rotation of the camera
			Quaternion currentQ = mainCameraTransform.localRotation;
			//store the current position of the camera
			Vector3 currentPos = mainCameraTransform.localPosition;

			//translate position and rotation camera
			while (i < 1) {
				i += Time.deltaTime * currentTurnBasedCombatCameraInfo.fixedLerpMovementSpeed;

				mainCameraTransform.localRotation = Quaternion.Lerp (currentQ, mainCameraTargetRotation, i);
				mainCameraTransform.localPosition = Vector3.Lerp (currentPos, mainCameraTargetPosition, i);

				yield return null;
			}

		} else {

			float dist = GKC_Utils.distance (mainCameraTransform.localPosition, mainCameraTargetPosition);

			float duration = dist / currentTurnBasedCombatCameraInfo.regularMovementSpeed;

			float t = 0;

			float movementTimer = 0;

			bool targetReached = false;

			float angleDifference = 0;

			float positionDifference = 0;

			while (!targetReached) {
				t += Time.deltaTime / duration; 

				mainCameraTransform.localPosition = Vector3.Lerp (mainCameraTransform.localPosition, mainCameraTargetPosition, t);
				mainCameraTransform.localRotation = Quaternion.Lerp (mainCameraTransform.localRotation, mainCameraTargetRotation, t);

				angleDifference = Quaternion.Angle (mainCameraTransform.localRotation, mainCameraTargetRotation);

				positionDifference = GKC_Utils.distance (mainCameraTransform.localPosition, mainCameraTargetPosition);

				movementTimer += Time.deltaTime;

				if ((positionDifference < 0.01f && angleDifference < 0.2f) || movementTimer > (duration + 1)) {
					targetReached = true;
				}

				yield return null;
			}
		}
	}

	public void stopMovement ()
	{
		if (cameraState != null) {
			StopCoroutine (cameraState);
		}
	}

	public void setCurrentPlayer (GameObject player)
	{
		mainPlayer = player;

		if (mainPlayer != null) {
			mainPlayerComponentsManager = mainPlayer.GetComponent<playerComponentsManager> ();

			playerCameraManager = mainPlayerComponentsManager.getPlayerCamera ();

			mainCamera = playerCameraManager.getMainCamera ();

			mainAIAroundManager = mainPlayerComponentsManager.getAIAroundManager ();

			mainFriendListManager = mainPlayerComponentsManager.getFriendListManager ();
		}
	}

	public turnBasedCombatTeamPositionsInfoData getTurnBasedCombatTeamPositionsInfoData (int teamSize, string teamPositionInfoName, bool isPlayerTeam)
	{
		int currentIndex = -1;

		if (teamPositionInfoName != "") {
			currentIndex = turnBasedCombatTeamPositionsInfoDataList.FindIndex (s => s.Name.Equals (teamPositionInfoName));

			if (currentIndex > -1) {
				bool getDataResult = true;

				if (turnBasedCombatTeamPositionsInfoDataList [currentIndex].turnBasedCombatCharacterPositionsInfoList.Count < teamSize) {
					getDataResult = false;
				}

				if (getDataResult) {
					return turnBasedCombatTeamPositionsInfoDataList [currentIndex];
				}
			}
		} 
			
		int turnBasedCombatTeamPositionsInfoDataListCount = turnBasedCombatTeamPositionsInfoDataList.Count;

		for (int i = 0; i < turnBasedCombatTeamPositionsInfoDataListCount; i++) {
			if (turnBasedCombatTeamPositionsInfoDataList [i].turnBasedCombatCharacterPositionsInfoList.Count == teamSize) {

				if (turnBasedCombatTeamPositionsInfoDataList [i].usedForPlayerTeam == isPlayerTeam) {
					currentIndex = i;
				}
			}
		}

		if (currentIndex > -1) {
			return turnBasedCombatTeamPositionsInfoDataList [currentIndex];
		}

		return null;
	}

	public void setUseCustomCameraState (bool state, string cameraStateName)
	{
		useCustomCameraState = state;

		customCameraState = cameraStateName;
	}

	public List<Transform> getPositionToRespawnList ()
	{
		return positionToRespawnList;
	}

	public Vector3 getClosestRespawnPosition (Vector3 currentPosition)
	{
		float minDistance = Mathf.Infinity;

		int positionToRespawnListCount = positionToRespawnList.Count;

		int closestPointIndex = -1;

		for (int i = 0; i < positionToRespawnListCount; i++) {
			float currentDistance = GKC_Utils.distance (positionToRespawnList [i].position, currentPosition);

			if (currentDistance < minDistance) {
				minDistance = currentDistance;

				closestPointIndex = i;
			}
		}

		if (closestPointIndex != -1) {
			return positionToRespawnList [closestPointIndex].position;
		}

		return Vector3.zero;
	}

	public void updateAllCharacterStatsUIValue ()
	{
		if (turnBasedCombatActive) {
			if (mainTurnBasedCombatUISystemLocated) {
				mainTurnBasedCombatUISystem.updateAllCharacterStatsUIValue ();
			}
		}
	}

	public void setNextTurn ()
	{
		if (turnBasedCombatActive) {
			if (mainTurnBasedCombatUISystemLocated) {
				mainTurnBasedCombatUISystem.setNextTurn ();
			}
		}
	}

	public void setCurrentCommandNameUsed (string commandName)
	{
		if (turnBasedCombatActive) {
			if (mainTurnBasedCombatUISystemLocated) {
				mainTurnBasedCombatUISystem.setCurrentCommandNameUsed (commandName);
			}
		}
	}

	public void activateEffect (string effectName)
	{
		if (turnBasedCombatActive) {
			if (effectName == "") {
				effectName = defaultEffectName;
			}

			int effectIndex = turnBasedCombatEffectInfoList.FindIndex (s => s.Name.Equals (effectName));

			if (effectIndex > -1) {
				turnBasedCombatEffectInfo currentTurnBasedCombatEffectInfo = turnBasedCombatEffectInfoList [effectIndex];

				if (currentTurnBasedCombatEffectInfo.effectEnabled) {
					currentTurnBasedCombatEffectInfo.eventOnEffect.Invoke ();
				}
			}
		}
	}

	public void setFreeCombatActiveState (bool state)
	{
		freeCombatActive = state;

		if (mainTurnBasedCombatUISystemLocated) {
			mainTurnBasedCombatUISystem.setFreeCombatActiveStateOnAllCharacters (state);
		}
	}

	public void checkTeamsDeadStateAfterCharacterDeath (Transform characterTransform)
	{
		if (mainTurnBasedCombatUISystemLocated) {
			mainTurnBasedCombatUISystem.checkTeamsDeadStateAfterCharacterDeath (characterTransform);
		}
	}

	public void checkCharacterStateAfterResurrect (Transform characterTransform)
	{
		if (mainTurnBasedCombatUISystemLocated) {
			mainTurnBasedCombatUISystem.checkCharacterStateAfterResurrect (characterTransform);
		}
	}

	public void checkPlayerStateOnDeathDuringCombat ()
	{
		if (mainTurnBasedCombatUISystemLocated) {
			mainTurnBasedCombatUISystem.checkPlayerStateOnDeathDuringCombat ();
		}
	}

	//INPUT FUNCTIONS
	public void inputConfirmCommand ()
	{
		if (turnBasedCombatActive) {
			if (mainTurnBasedCombatUISystemLocated) {
				mainTurnBasedCombatUISystem.inputConfirmCommand ();
			}
		}
	}

	public void inputCancelCommand ()
	{
		if (turnBasedCombatActive) {
			if (mainTurnBasedCombatUISystemLocated) {
				mainTurnBasedCombatUISystem.inputCancelCommand ();
			}
		}
	}

	public void inputSelectNextTarget ()
	{
		if (turnBasedCombatActive) {
			if (mainTurnBasedCombatUISystemLocated) {
				mainTurnBasedCombatUISystem.inputSelectNextTarget ();
			}
		}
	}

	public void inputSelectPreviousTarget ()
	{
		if (turnBasedCombatActive) {
			if (mainTurnBasedCombatUISystemLocated) {
				mainTurnBasedCombatUISystem.inputSelectPreviousTarget ();
			}
		}
	}

	public void inputToggleCombatMode ()
	{
		if (turnBasedCombatActive || freeCombatActive) {
			if (mainTurnBasedCombatUISystemLocated) {
				mainTurnBasedCombatUISystem.inputToggleCombatMode ();
			}
		}
	}

	public void configureNewTeamInfo ()
	{
		bool teamConfiguredProperly = false;

		if (newCharacterTeamLeader != null && newCharacterTeamInfoList.Count > 0) {

			List<GameObject> characterGameobjectTeamList = new List<GameObject> ();

			int newCharacterTeamInfoListCount = newCharacterTeamInfoList.Count;

			playerComponentsManager currentPlayerComponentsManager = newCharacterTeamLeader.GetComponentInChildren<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
				newCharacterTeamLeader = currentPlayerComponentsManager.gameObject;
			}
				
			for (int i = 0; i < newCharacterTeamInfoListCount; i++) {
				if (newCharacterTeamInfoList [i] != null) {
					currentPlayerComponentsManager = newCharacterTeamInfoList [i].GetComponentInChildren<playerComponentsManager> ();

					if (currentPlayerComponentsManager != null) {
						characterGameobjectTeamList.Add (currentPlayerComponentsManager.gameObject);
					}
				}
			}

			int characterGameobjectTeamListCount = characterGameobjectTeamList.Count;

			for (int i = 0; i < characterGameobjectTeamListCount; i++) {

				currentPlayerComponentsManager = characterGameobjectTeamList [i].GetComponentInChildren<playerComponentsManager> ();

				if (currentPlayerComponentsManager != null) {
					GameObject currentCharacterGameObject = currentPlayerComponentsManager.gameObject;

					turnBasedCombatTeamInfo	currentTurnBasedCombatTeamInfo = currentPlayerComponentsManager.getTurnBasedCombatTeamInfo ();

					currentTurnBasedCombatTeamInfo.clearCharacterTeamList ();

					currentTurnBasedCombatTeamInfo.setCharacterTeamList (characterGameobjectTeamList);

					currentTurnBasedCombatTeamInfo.isTeamLeader = (currentCharacterGameObject == newCharacterTeamLeader);

					print ("is leader " + currentTurnBasedCombatTeamInfo.isTeamLeader);

					currentTurnBasedCombatTeamInfo.teamPositionDataName = teamPositionDataName;

					currentTurnBasedCombatTeamInfo.thisTeamAlwaysSelectFirst = newTeamAlwaysSelectFirst;

					currentTurnBasedCombatTeamInfo.useCustomCameraState = useCustomCameraStateOnNewTeam;

					currentTurnBasedCombatTeamInfo.customCameraState = customCameraStateOnNewTeam;

					if (useRewardSystem) {
						currentTurnBasedCombatTeamInfo.useRewardSystem = true;

						if (mainRewardSystem != null) {
							currentTurnBasedCombatTeamInfo.mainRewardSystem = mainRewardSystem;
						}
					}

					GKC_Utils.updateComponent (currentTurnBasedCombatTeamInfo);

					GKC_Utils.updateDirtyScene ("Update Turn Based Team Combat System", currentTurnBasedCombatTeamInfo.gameObject);
				}
			}

			teamConfiguredProperly = true;
		}

		if (teamConfiguredProperly) {
			newCharacterTeamLeader = null;

			teamPositionDataName = "";

			newTeamAlwaysSelectFirst = false;

			useCustomCameraStateOnNewTeam = false;

			customCameraStateOnNewTeam = "";

			useRewardSystem = false;

			mainRewardSystem = null;

			newCharacterTeamInfoList.Clear ();

			print ("New Turn Based Combat Team Configured");

			updateComponent ();
		} else {
			print ("WARNING: make sure all team info is configured properly on the inspector");
		}
	}

	public void showAllTeamsInScene ()
	{
		turnBasedCombatTeamInfoOnSceneList.Clear ();

		turnBasedCombatTeamInfo[] turnBasedCombatTeamInfoList = FindObjectsOfType<turnBasedCombatTeamInfo> ();

		foreach (turnBasedCombatTeamInfo currentTurnBasedCombatTeamInfo in turnBasedCombatTeamInfoList) {
			if (currentTurnBasedCombatTeamInfo.isTeamLeaderValue ()) {
				turnBasedCombatTeamInfoOnSceneList.Add (currentTurnBasedCombatTeamInfo);
			}
		}

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Turn Based Combat System", gameObject);
	}

	[System.Serializable]
	public class turnBasedCombatCameraInfo
	{
		public string Name;

		public Transform cameraPosition;

		[Space]

		public int minTeamAmountToCheck;

		[Space]

		public bool useFixedLerpMovement = true;
		public float fixedLerpMovementSpeed = 2;

		public float regularMovementSpeed = 2;

		public bool smoothCameraMovement = true;

		public bool setCameraBackToPlayer;
	}

	[System.Serializable]
	public class turnBasedCombatEffectInfo
	{
		public string Name;

		public bool effectEnabled = true;

		[Space]

		public UnityEvent eventOnEffect;
	}
}

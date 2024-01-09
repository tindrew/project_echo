using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class grapplingHookSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool grapplingHookEnabled = true;

	public float maxRaycastDistance = 100;
	public LayerMask layerToCheckSurfaces;

	public float minDistanceToAttract = 0.5f;

	public bool applySpeedOnHookStop;
	public float extraSpeedOnHookStopMultiplier = 1;

	public bool rotatePlayerTowardTargetDirection;
	public float rotatePlayerSpeed;
	public float minAngleDifferenceToRotatePlayer;

	public float extraForceOnHookStopMultiplier = 4;

	public float maxForceToApplyOnHookStop = 40;

	[Space]
	[Header ("Swing Settings")]
	[Space]

	public bool useSwingForcesActive;

	public float gravityForceDown = 9.8f;

	public float hookPullForceOnGround = 300;

	public float hookPullForceOnAir = 300;

	public ForceMode hookPullForceMode;

	public float airControlPrecisionRegularSpeed = 3;
	public float airControlPrecisionIncreasedSpeed = 7;

	public float durationAutoPullOnThrowHookOnGround = 0.5f;
	public float durationAutoPullOnThrowHookOnAir = 0.5f;

	[Space]

	public bool useMaxSwingLength;
	public float maxSwingLength;

	public float raycastDistanceToCheckBelowPlayer = 5;

	public bool jumpOnSwingEnabled = true;
	public Vector3 jumpOnSwingForceAmount;

	public bool checkIfTooCloseToGroundToStopSwing;
	public float minDistanceToStopSwingOnGroundDetected;
	public bool onlyCheckIfCloseToGroundWhenAutomaticPullNotActive;

	public ForceMode swingForceMode = ForceMode.Impulse;

	public float maxSwingSpeedClamp = 40;

	public bool manualPullEnabled = true;
	public bool manualExtendEnabled = true;

	[Space]
	[Space]

	public bool chainNextSwingIfInputHold;
	public float minTimeToKeepSwingBeforeNext = 1;
	public float minWaitTimeToThrowNextSwing = 0.5f;
	public bool stopSwingHoldChainIfNotSurfaceLocated = true;

	public bool getClosestSurfaceIfNotFoundOnMainRaycast;
	public Vector2 swingClosestSurfaceAngleSearchRange = new Vector2 (-40, 40);

	public float probabilityToUseJumpOnSwingEnd = 30;

	[Space]
	[Header ("Attract Objects Settings")]
	[Space]

	public bool attractObjectsEnabled = true;
	public float regularAttractionForce;
	public float increasedAttractionForce;
	public float minDistanceToStopAttractObject;

	public bool addUpForceForAttraction;
	public float upForceForAttraction;
	public float addUpForceForAttractionDuration;

	[Space]
	[Header ("Movement Settings")]
	[Space]

	public float regularMovementSpeed = 6;
	public float increasedMovementSpeed;

	public float inputMovementMultiplier = 3;

	public float airControlAmount = 20;

	public bool useVerticalMovementOnHook;
	public bool ignoreBackWardsMovementOnHook;

	public bool addVerticalFallingSpeed;
	public float verticalFallingSpeed;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool checkIfObjectStuck = true;
	public float timeToStopHookIfStuck = 2;
	public float minDistanceToCheckStuck = 1;

	[Space]
	[Header ("External Controller Settings")]
	[Space]

	public bool checkExternalControllerStatesToIgnore;

	public List<otherExternalControllerInfo> otherExternalControllerInfoList = new List<otherExternalControllerInfo> ();

	[Space]
	[Header ("Action System Settings")]
	[Space]

	public int customActionCategoryID = -1;
	public int regularActionCategoryID = -1;

	[Space]
	[Header ("Camera Settings")]
	[Space]

	public bool changeCameraStateOnThirdPerson;
	public string cameraStateNameOnGrapplingHookActivate;
	public string cameraStateNameOnGrapplingHookDeactivate;
	public bool keepCameraStateActiveWhileOnAir;

	public bool changeFovOnHookActive;
	public float changeFovSpeed;
	public float regulaFov;
	public float increaseSpeedFov;

	public bool useCameraShake;
	public string regularCameraShakeName;
	public string increaseCamaraShakeName;

	[Space]
	[Header ("Animator Settings")]
	[Space]

	public bool setAnimatorState;
	public string hookStartActionName;
	public string hookEndActionName;

	public int hookStartActionID;
	public int hookEndActionID;

	public string actionActiveAnimatorName = "Action Active";
	public string actionIDAnimatorName = "Action ID";

	public float minDistancePercentageToUseHookEndAction = 0.1f;

	[Space]
	[Header ("Swing Animation Settings")]
	[Space]

	public bool useSwingJumpOnAir;
	public int swingJumpOnAirID = 39475;

	public int swingMovingPassCenterID = 92783;

	public int swingMovingForwardID = 92782;

	[Space]
	[Header ("Throw Hook Animation Settings")]
	[Space]

	public bool useAnimationToThrowHook;

	public bool useAnimationToThrowHookOnlyOnGround;

	public string throwHookAnimationName;
	public float throwHookAnimationDuration;

	public bool throwAnimationInProcess;

	public bool checkIfSurfaceDetectedBeforeAnimation;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool grapplingHookActive;
	public bool grapplingHookUpdateActive;

	public Vector3 currentForceToApply;
	public Vector3 movementDirection;

	public float currentDistance;

	public float currentMovementSpeed;

	public Transform currentGrapplingHookTarget;

	public float angleToTargetDirection;

	public bool attractingObjectActive;

	public bool checkingToRemoveHookActive;

	public bool manualExtendActive;

	public bool manualPullActive;

	public bool automaticPullActive;

	public float currentMinDistanceToStopAttractObject;

	public bool attractPlayerActive;

	public bool ignoreApplySpeedOnHookStopOnPlayer;

	public bool resetPlayerSpeedOnHookStop;

	[Space]
	[Header ("Debug Swing")]
	[Space]

	public Vector3 swingGravityForces = Vector3.zero;
	public bool swingHoldInputActive;

	public float angleWithCenter;
	public bool swingPassCenterChecked;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventsOnGrapplingHook;
	public UnityEvent eventOnGrapplingHookActivate;
	public UnityEvent eventOnGrapplingHookDeactivate;
	public eventParameters.eventToCallWithVector3 eventWithDirectionOnHookActive;

	public bool useEventsOnChangeCameraView;
	public UnityEvent eventOnSetFirstPersonView;
	public UnityEvent eventOnSetThirdPersonView;

	public eventParameters.eventToCallWithTransform eventToSendNewGrapplingHookTipTransform;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform grapplingHookTipTransform;

	public Transform playerControllerTransform;
	public playerController mainPlayerController;
	public gravitySystem mainGravitySystem;
	public Transform mainCameraTransform;
	public Rigidbody mainRigidbody;
	public playerCamera mainPlayerCamera;

	public remoteEventSystem mainRemoteEventSystem;

	public Animator mainAnimator;

	public Rigidbody mainSwingRigidbody;

	public Collider mainSwingCollider;

	public SpringJoint swingJoint;


	RaycastHit hit;

	bool increaseSpeedActive;

	bool checkinGrapplingHookCameraStateAfterDeactivate;

	Vector3 pullForceToApply;

	Vector3 pullForceToApplyNormalize;

	int actionActiveAnimatorID;
	int actionIDAnimatorID;

	bool closeToReachTargetChecked;

	float initialDistanceToTarget;

	bool firstPersonActive;
	bool previoslyFirstPersonActive;

	objectToAttractWithGrapplingHook currentobjectToAttractWithGrapplingHook;

	GameObject currentObjectToAttract;
	Rigidbody currentRigidbodyToAttract;

	float lastTimeHookActive;

	float customMinDistanceToStopAttractObject;

	bool useCustomForceAttractionValues;
	bool customAddUpForceForAttraction;
	float customUpForceForAttraction;
	float customAddUpForceForAttractionDuration;

	bool attractionHookRemovedByDistance;

	Vector3 currentRaycastPosition;
	Vector3 currentRaycastDirection;

	Coroutine grapplingHookCoroutine;

	float lastTimeObjectMoving;
	float lastDistanceToObject;

	Coroutine animationCoroutine;

	Vector3 lastVelocityAdded;

	float lastTimeAutomaticPullActive;

	bool swingRaycastDetected;

	RaycastHit swingRacyastCheck;

	bool useHookTargetPostionOffset;
	Vector3 hookTargetPositionOffset = Vector3.zero;

	bool ignoreApplySpeedOnHookStopTemporally;

	bool lastTimeHookActivatedPlayerOnGround;


	void Start ()
	{
		actionActiveAnimatorID = Animator.StringToHash (actionActiveAnimatorName);
		actionIDAnimatorID = Animator.StringToHash (actionIDAnimatorName);
	}

	public void stopGrapplingHookCoroutine ()
	{
		if (grapplingHookCoroutine != null) {
			StopCoroutine (grapplingHookCoroutine);
		}

		grapplingHookUpdateActive = false;
	}

	IEnumerator activateGrapplingHookCorouine ()
	{
		var waitTime = new WaitForFixedUpdate ();
		
		while (true) {
			yield return waitTime;

			updateGrapplingHookState ();
		}
	}

	void updateGrapplingHookState ()
	{
		if (attractingObjectActive) {
			applyAttractionForces ();
		} else {
			if (grapplingHookActive) {
				applyHookForces ();

				if (useEventsOnChangeCameraView) {
					firstPersonActive = mainPlayerCamera.isFirstPersonActive ();

					if (firstPersonActive != previoslyFirstPersonActive) {
						previoslyFirstPersonActive = firstPersonActive;

						if (firstPersonActive) {
							eventOnSetFirstPersonView.Invoke ();
						} else {
							eventOnSetThirdPersonView.Invoke ();
						}
					}
				}
			}

			if (checkinGrapplingHookCameraStateAfterDeactivate) {
				if (mainPlayerController.isPlayerOnGround ()) {
					if (!mainPlayerCamera.isFirstPersonActive () && mainPlayerCamera.isCameraTypeFree ()) {
						mainPlayerCamera.setCameraStateExternally (cameraStateNameOnGrapplingHookDeactivate);
					}

					checkinGrapplingHookCameraStateAfterDeactivate = false;

					stopGrapplingHookCoroutine ();
				}
			}
		}
	}

	public Vector3 TransformDirectionHorizontal (Transform t, Vector3 v)
	{
		return ToHorizontal (t.TransformDirection (v)).normalized;
	}

	public Vector3 ToHorizontal (Vector3 v)
	{
		return Vector3.ProjectOnPlane (v, Vector3.up);
	}

	void updateSwingForces (Vector3 playerPosition, Vector3 targetPosition)
	{
		Vector2 rawAxisValues = mainPlayerController.getRawAxisValues ();

		movementDirection = new Vector3 (rawAxisValues.x, 0, rawAxisValues.y).normalized;

		if (lastTimeAutomaticPullActive > 0) {
			if (Time.time > lastTimeAutomaticPullActive + 0.5f) {
				lastTimeAutomaticPullActive = 0;
			}
		} else {
			automaticPullActive = false;
		}

		if (Physics.Raycast (playerPosition, -playerControllerTransform.up, out hit, raycastDistanceToCheckBelowPlayer, layerToCheckSurfaces)) {
			automaticPullActive = true;

			lastTimeAutomaticPullActive = Time.time;
		} 

		if (lastTimeHookActivatedPlayerOnGround) {
			if (durationAutoPullOnThrowHookOnGround > 0) {
				if (Time.time < durationAutoPullOnThrowHookOnGround + lastTimeHookActive) {
					automaticPullActive = true;

					lastTimeAutomaticPullActive = Time.time;
				}
			}
		} else {
			if (durationAutoPullOnThrowHookOnAir > 0) {
				if (Time.time < durationAutoPullOnThrowHookOnAir + lastTimeHookActive) {
					automaticPullActive = true;

					lastTimeAutomaticPullActive = Time.time;
				}
			}
		}

		if (useMaxSwingLength) {
			if (currentDistance > maxSwingLength) {
				automaticPullActive = true;

				lastTimeAutomaticPullActive = Time.time;
			}
		}

		if (currentDistance < 3) {
			removeHook ();

			return;
		}

		float deltaTime = Time.smoothDeltaTime;

		Vector3 wishDir = TransformDirectionHorizontal (mainPlayerCamera.transform, movementDirection);

		if (increaseSpeedActive) {
			wishDir *= airControlPrecisionIncreasedSpeed;
		} else {
			wishDir *= airControlPrecisionRegularSpeed;
		}

		wishDir += -mainPlayerController.getCurrentNormal () * gravityForceDown;

		mainSwingRigidbody.AddForce (wishDir, swingForceMode);

		if (manualPullActive || automaticPullActive) {
			Vector3 directionToPoint = targetPosition - mainSwingRigidbody.position;

			float currentHookPullForce = hookPullForceOnAir;

			if (lastTimeHookActivatedPlayerOnGround) {
				currentHookPullForce = hookPullForceOnGround;
			}

			Vector3 extraForce = directionToPoint.normalized * currentHookPullForce * deltaTime;

			mainSwingRigidbody.AddForce (extraForce, hookPullForceMode);

			float distanceFromPoint = GKC_Utils.distance (mainSwingRigidbody.position, targetPosition);

			swingJoint.maxDistance = distanceFromPoint * 0.8f;
			swingJoint.minDistance = distanceFromPoint * 0.25f;
		}

		if (manualExtendActive) {
			float extendedDistanceFromPoint = GKC_Utils.distance (mainSwingRigidbody.position, targetPosition) + hookPullForceOnAir;

			swingJoint.maxDistance = extendedDistanceFromPoint * 0.8f;
			swingJoint.minDistance = extendedDistanceFromPoint * 0.25f;

		}

		if (maxSwingSpeedClamp != 0) {
			mainSwingRigidbody.velocity = Vector3.ClampMagnitude (mainSwingRigidbody.velocity, maxSwingSpeedClamp);
		}

		currentDistance = GKC_Utils.distance (playerPosition, grapplingHookTipTransform.position);

		mainPlayerController.setExtraCharacterVelocity (mainSwingRigidbody.velocity);

		mainRigidbody.position = mainSwingRigidbody.position;

		if (checkIfTooCloseToGroundToStopSwing && Time.time > lastTimeHookActive + 0.6f) {
			if (!automaticPullActive || !onlyCheckIfCloseToGroundWhenAutomaticPullNotActive) {
				Vector3 raycastPosition = playerControllerTransform.position + playerControllerTransform.up;

				if (Physics.Raycast (raycastPosition, -playerControllerTransform.up, out hit, minDistanceToStopSwingOnGroundDetected, layerToCheckSurfaces)) {
					removeHook ();
				}
			}
		}
			

		Vector3 heading = new Vector3 (targetPosition.x, 0, targetPosition.z) - new Vector3 (playerPosition.x, 0, playerPosition.z);

		float distance = heading.magnitude;

		Vector3 movementDirectionWithHookPoint = heading / distance;

		angleWithCenter = Vector3.Angle (movementDirectionWithHookPoint, playerControllerTransform.forward);

		bool swingCenterReached = Mathf.Abs (angleWithCenter) > 90;

		if (swingCenterReached) {
			if (!swingPassCenterChecked) {
				if (setAnimatorState) {
					mainAnimator.SetInteger (actionIDAnimatorID, swingMovingPassCenterID);
				}

				swingPassCenterChecked = true;

				lastTimeSwingPassCenterChecked = Time.time;

				if (showDebugPrint) {
					print ("swing center reached");
				}
			}
		} else {
			if (swingPassCenterChecked) {
				if (setAnimatorState) {
					mainAnimator.SetInteger (actionIDAnimatorID, swingMovingForwardID);
				}

				swingPassCenterChecked = false;

				lastTimeSwingPassCenterChecked = -1;
			
				if (showDebugPrint) {
					print ("swing moving forward reached");
				}
			}
		}
	}

	float lastTimeSwingPassCenterChecked = -1;

	public void applyHookForces ()
	{
		currentForceToApply = Vector3.zero;

		Vector3 playerPosition = playerControllerTransform.position + playerControllerTransform.up;

		Vector3 targetPosition = grapplingHookTipTransform.position;

		if (useHookTargetPostionOffset) {
			targetPosition += hookTargetPositionOffset;
		}

		currentDistance = GKC_Utils.distance (playerPosition, targetPosition);

		if (useSwingForcesActive) {
			updateSwingForces (playerPosition, targetPosition);
		} else {
			if (attractPlayerActive) {
				if (useCustomForceAttractionValues) {
					currentMinDistanceToStopAttractObject = customMinDistanceToStopAttractObject;
				} else {
					currentMinDistanceToStopAttractObject = minDistanceToAttract;
				}
			} else {
				currentMinDistanceToStopAttractObject = minDistanceToAttract;
			}

			if (currentDistance > currentMinDistanceToStopAttractObject) {
				pullForceToApply = (targetPosition - playerPosition).normalized;
			} else {
				removeHook ();
			}

			if (increaseSpeedActive) {
				currentMovementSpeed = increasedMovementSpeed;
			} else {
				currentMovementSpeed = regularMovementSpeed;
			}

			currentForceToApply += pullForceToApply * currentMovementSpeed;

			pullForceToApplyNormalize = pullForceToApply.normalized;

			movementDirection = 
				(mainPlayerController.getHorizontalInput () * Vector3.Cross (playerControllerTransform.up, pullForceToApplyNormalize));

			if (useVerticalMovementOnHook) {
				movementDirection += (mainPlayerController.getVerticalInput () * playerControllerTransform.up);
			} else {
				float verticalInputValue = mainPlayerController.getVerticalInput ();

				if (ignoreBackWardsMovementOnHook) {
					verticalInputValue = Mathf.Clamp (verticalInputValue, 0, 1);
				}

				movementDirection += (verticalInputValue * pullForceToApplyNormalize);
			}

			movementDirection *= inputMovementMultiplier;

			currentForceToApply += movementDirection;

			if (addVerticalFallingSpeed) {
				currentForceToApply -= playerControllerTransform.up * verticalFallingSpeed;
			}

			mainPlayerController.setExternalForceOnAir (currentForceToApply, airControlAmount);

			lastVelocityAdded = currentForceToApply;
		}

		if (rotatePlayerTowardTargetDirection) {
			if (mainPlayerController.isFullBodyAwarenessActive ()) {
				Transform playerCameraTransform = mainPlayerCamera.transform;

				pullForceToApplyNormalize -= playerCameraTransform.up * playerCameraTransform.InverseTransformDirection (pullForceToApplyNormalize).y;

				angleToTargetDirection = Vector3.SignedAngle (playerCameraTransform.forward, pullForceToApplyNormalize, playerCameraTransform.up);

				if (Mathf.Abs (angleToTargetDirection) > minAngleDifferenceToRotatePlayer) {
					playerCameraTransform.Rotate (0, (angleToTargetDirection / 2) * rotatePlayerSpeed * Time.deltaTime, 0);
				}
			} else {
				pullForceToApplyNormalize -= playerControllerTransform.up * playerControllerTransform.InverseTransformDirection (pullForceToApplyNormalize).y;

				angleToTargetDirection = Vector3.SignedAngle (playerControllerTransform.forward, pullForceToApplyNormalize, playerControllerTransform.up);

				if (Mathf.Abs (angleToTargetDirection) > minAngleDifferenceToRotatePlayer) {
					playerControllerTransform.Rotate (0, (angleToTargetDirection / 2) * rotatePlayerSpeed * Time.deltaTime, 0);
				}
			}
		}

		if (!useSwingForcesActive) {
			if (!closeToReachTargetChecked) {
				if (currentDistance < initialDistanceToTarget * minDistancePercentageToUseHookEndAction) {

					if (setAnimatorState) {
						mainAnimator.SetInteger (actionIDAnimatorID, hookEndActionID);
						mainAnimator.CrossFadeInFixedTime (hookEndActionName, 0.1f);
					}

					closeToReachTargetChecked = true;
				}
			}

			checkIfObjectIsMoving ();
		}
	}

	public void applyAttractionForces ()
	{

		bool stopHookResult = false;

		if (currentRigidbodyToAttract == null) {
			stopHookResult = true;
		}

		if (grapplingHookTipTransform == null) {
			stopHookResult = true;
		}

		if (stopHookResult) {
			if (grapplingHookTipTransform == null) {
				GameObject grapplingHookTipTransformGameObject = new GameObject ();

				grapplingHookTipTransform = grapplingHookTipTransformGameObject.transform;

				grapplingHookTipTransform.name = "Grappling Hook Tip Transform";

				eventToSendNewGrapplingHookTipTransform.Invoke (grapplingHookTipTransform);
			}

			removeGrapplingHook ();

			attractingObjectActive = false;

			return;
		}

		currentForceToApply = Vector3.zero;

		Vector3 playerPosition = playerControllerTransform.position + playerControllerTransform.up;

		currentDistance = GKC_Utils.distance (playerPosition, grapplingHookTipTransform.position);

		if (useCustomForceAttractionValues) {
			currentMinDistanceToStopAttractObject = customMinDistanceToStopAttractObject;
		} else {
			currentMinDistanceToStopAttractObject = minDistanceToStopAttractObject;
		}

		if (currentDistance > currentMinDistanceToStopAttractObject) {
			pullForceToApply = (playerPosition - grapplingHookTipTransform.position).normalized;
		} else {
			attractionHookRemovedByDistance = true;

			removeHook ();

			return;
		}

		if (increaseSpeedActive) {
			currentMovementSpeed = increasedAttractionForce;
		} else {
			currentMovementSpeed = regularAttractionForce;
		}

		currentForceToApply += pullForceToApply * currentMovementSpeed;

		movementDirection *= inputMovementMultiplier;

		currentForceToApply += movementDirection;

		if (useCustomForceAttractionValues) {
			if (customAddUpForceForAttraction) {
				if (Time.time < lastTimeHookActive + customAddUpForceForAttractionDuration) {
					currentForceToApply += playerControllerTransform.up * customUpForceForAttraction;
				}
			}
		} else {
			if (addUpForceForAttraction) {
				if (Time.time < lastTimeHookActive + addUpForceForAttractionDuration) {
					currentForceToApply += playerControllerTransform.up * upForceForAttraction;
				}
			}
		}
	
		if (currentRigidbodyToAttract != null) {
			currentRigidbodyToAttract.velocity = currentForceToApply;

			lastVelocityAdded = currentForceToApply;
		}

		checkIfObjectIsMoving ();
	}

	public void checkIfObjectIsMoving ()
	{
		if (checkIfObjectStuck) {
			if (lastTimeObjectMoving == 0) {
				Vector3 targetPosition = grapplingHookTipTransform.position;

				if (useHookTargetPostionOffset) {
					targetPosition += hookTargetPositionOffset;
				}

				currentDistance = GKC_Utils.distance (playerControllerTransform.position, targetPosition);

				lastDistanceToObject = currentDistance;

				lastTimeObjectMoving = Time.time;
			}

			if (Time.time > lastTimeObjectMoving + timeToStopHookIfStuck) {
				lastTimeObjectMoving = Time.time;

				if ((currentDistance + minDistanceToCheckStuck) >= lastDistanceToObject) {
					if (showDebugPrint) {
						print ("position hasn't changed in " + timeToStopHookIfStuck + " time, stop hook");
					}

					removeHook ();
				}

				lastDistanceToObject = currentDistance;
			}
		}
	}

	public void removeHook ()
	{
		if (grapplingHookActive) {
		
			grapplingHookActive = false;

			setCurrentPlayerActionSystemCustomActionCategoryID (false);
		
			if (attractingObjectActive) {
				checkEventsOnGrapplingHook (false);

				stopGrapplingHookCoroutine ();
			} else {
				pauseOrResumePlayerState (false);

				mainPlayerController.disableExternalForceOnAirActive ();
			}

			increaseSpeedActive = false;

			grapplingHookTipTransform.SetParent (transform);

			if (attractObjectsEnabled) {
				if (currentobjectToAttractWithGrapplingHook != null) {
					if (currentobjectToAttractWithGrapplingHook.useRemoteEventsOnStateChange) {
						if (mainRemoteEventSystem != null) {
							for (int i = 0; i < currentobjectToAttractWithGrapplingHook.remoteEventNameListOnEnd.Count; i++) {
								mainRemoteEventSystem.callRemoteEvent (currentobjectToAttractWithGrapplingHook.remoteEventNameListOnEnd [i]);
							}
						}
					}

					if (attractPlayerActive) {

					} else {
						currentobjectToAttractWithGrapplingHook.setAttractionHookRemovedByDistanceState (attractionHookRemovedByDistance);

						currentobjectToAttractWithGrapplingHook.setAttractObjectState (false);


						bool canUseExtraInteraction = currentobjectToAttractWithGrapplingHook.autoGrabObjectOnCloseDistance ||
						                              currentobjectToAttractWithGrapplingHook.activateInteractionActionWithObject;

						if (canUseExtraInteraction) {
							if (currentRigidbodyToAttract != null) {
								Vector3 playerPosition = playerControllerTransform.position + playerControllerTransform.up;

								currentDistance = GKC_Utils.distance (playerPosition, currentRigidbodyToAttract.transform.position);

								if (currentobjectToAttractWithGrapplingHook.autoGrabObjectOnCloseDistance) {
									if (currentDistance < currentobjectToAttractWithGrapplingHook.minDistanceToAutoGrab) {
										GKC_Utils.grabPhysicalObjectExternally (playerControllerTransform.gameObject, currentRigidbodyToAttract.gameObject);
									}
								}
							
								if (currentobjectToAttractWithGrapplingHook.activateInteractionActionWithObject) {
									if (currentDistance < currentobjectToAttractWithGrapplingHook.minDistanceToActivateInteractionActionWithObject) {
										GKC_Utils.useObjectExternally (playerControllerTransform.gameObject, currentobjectToAttractWithGrapplingHook.objectToActivate);
									}
								}
							}
						}

						attractingObjectActive = false;

						currentobjectToAttractWithGrapplingHook = null;

						currentObjectToAttract = null;

						currentRigidbodyToAttract = null;

						attractionHookRemovedByDistance = false;


					}
				}
			}
		}
	}

	public void setGrapplingHookTarget (Transform newTarget)
	{
		currentGrapplingHookTarget = newTarget;
	}

	public void setGrapplingHookEnabledState (bool state)
	{
		grapplingHookEnabled = state;

		if (!grapplingHookEnabled) {
			removeGrapplingHook ();
		}
	}

	void calculateRaycastValues ()
	{
		currentRaycastPosition = mainCameraTransform.position;
		currentRaycastDirection = mainCameraTransform.forward;

		if (!mainPlayerCamera.isCameraTypeFree () && !mainPlayerCamera.isPlayerAiming ()) {
			currentRaycastPosition = playerControllerTransform.position + playerControllerTransform.up * 1.3f;
			currentRaycastDirection = playerControllerTransform.forward;
		}

		if (currentGrapplingHookTarget != null) {
			currentRaycastDirection = currentGrapplingHookTarget.position - currentRaycastPosition;
			currentRaycastDirection.Normalize ();
		}
	}

	public void checkThrowGrapplingHook ()
	{
		if (grapplingHookEnabled) {
			if (!grapplingHookActive) {

				bool surfaceDetected = false;

				if (useSwingForcesActive) {
					if (swingRaycastDetected) {
						hit = swingRacyastCheck;

						surfaceDetected = true;
					}
				} else {
					calculateRaycastValues ();

					if (Physics.Raycast (currentRaycastPosition, currentRaycastDirection, out hit, maxRaycastDistance, layerToCheckSurfaces)) {
						surfaceDetected = true;
					}
				}

				if (surfaceDetected) {
					grapplingHookTipTransform.position = hit.point;

					grapplingHookTipTransform.SetParent (hit.collider.gameObject.transform);

					grapplingHookActive = true;

					stopGrapplingHookCoroutine ();

					grapplingHookCoroutine = StartCoroutine (activateGrapplingHookCorouine ());

					grapplingHookUpdateActive = true;

					lastTimeHookActive = Time.time;

					lastTimeHookActivatedPlayerOnGround = mainPlayerController.isPlayerOnGround ();

					lastTimeObjectMoving = 0;

					ignoreApplySpeedOnHookStopOnPlayer = false;

					resetPlayerSpeedOnHookStop = false;

					if (attractObjectsEnabled) {
						currentObjectToAttract = hit.collider.gameObject;

						GameObject currentVehicle = applyDamage.getVehicle (currentObjectToAttract);

						if (currentVehicle != null) {
							currentObjectToAttract = currentVehicle;
						}

						currentobjectToAttractWithGrapplingHook = currentObjectToAttract.GetComponent<objectToAttractWithGrapplingHook> ();

						if (currentobjectToAttractWithGrapplingHook == null) {
							playerComponentsManager currentPlayerComponentsManager = currentObjectToAttract.GetComponent<playerComponentsManager> ();

							if (currentPlayerComponentsManager != null) {
								currentobjectToAttractWithGrapplingHook = currentPlayerComponentsManager.getObjectToAttractWithGrapplingHook ();
							}
						}

						attractPlayerActive = false;


						customMinDistanceToStopAttractObject = 0;

						useCustomForceAttractionValues = false;

						customAddUpForceForAttraction = false;

						customUpForceForAttraction = 0;

						customAddUpForceForAttractionDuration = 0;

						useHookTargetPostionOffset = false;

						hookTargetPositionOffset = Vector3.zero;

						if (currentobjectToAttractWithGrapplingHook != null) {
							attractPlayerActive = currentobjectToAttractWithGrapplingHook.attractPlayerEnabled;

							ignoreApplySpeedOnHookStopOnPlayer = currentobjectToAttractWithGrapplingHook.ignoreApplySpeedOnHookStopOnPlayer;

							resetPlayerSpeedOnHookStop = currentobjectToAttractWithGrapplingHook.resetPlayerSpeedOnHookStop;

							useHookTargetPostionOffset = currentobjectToAttractWithGrapplingHook.useHookTargetPostionOffset;
							hookTargetPositionOffset = currentobjectToAttractWithGrapplingHook.hookTargetPositionOffset;

							if (attractPlayerActive) {

							} else {
								attractingObjectActive = currentobjectToAttractWithGrapplingHook.setAttractObjectState (true);

								if (attractingObjectActive) {
									currentRigidbodyToAttract = currentobjectToAttractWithGrapplingHook.getRigidbodyToAttract ();

									if (currentRigidbodyToAttract == null) {
										print ("WARNING: No rigidbody has been configured in the object " + currentobjectToAttractWithGrapplingHook.name);
								
										removeGrapplingHook ();

										return;
									}

									grapplingHookTipTransform.SetParent (currentRigidbodyToAttract.transform);

									grapplingHookTipTransform.position = currentRigidbodyToAttract.position;
								}
							}

							customMinDistanceToStopAttractObject = currentobjectToAttractWithGrapplingHook.customMinDistanceToStopAttractObject;

							useCustomForceAttractionValues = currentobjectToAttractWithGrapplingHook.useCustomForceAttractionValues;

							customAddUpForceForAttraction = currentobjectToAttractWithGrapplingHook.customAddUpForceForAttraction;

							customUpForceForAttraction = currentobjectToAttractWithGrapplingHook.customUpForceForAttraction;

							customAddUpForceForAttractionDuration = currentobjectToAttractWithGrapplingHook.customAddUpForceForAttractionDuration;

							if (currentobjectToAttractWithGrapplingHook.useRemoteEventsOnStateChange) {
								if (mainRemoteEventSystem != null) {
									for (int i = 0; i < currentobjectToAttractWithGrapplingHook.remoteEventNameListOnStart.Count; i++) {
										mainRemoteEventSystem.callRemoteEvent (currentobjectToAttractWithGrapplingHook.remoteEventNameListOnStart [i]);
									}
								}
							}
						}
					}

					if (attractingObjectActive) {
						checkEventsOnGrapplingHook (true);
					} else {
						pauseOrResumePlayerState (true);
					}

					checkingToRemoveHookActive = false;

					eventWithDirectionOnHookActive.Invoke (currentRaycastDirection);

					closeToReachTargetChecked = false;

					swingPassCenterChecked = false;

					lastTimeSwingPassCenterChecked = -1;

					Vector3 playerPosition = playerControllerTransform.position + playerControllerTransform.up;

					Vector3 targetPosition = grapplingHookTipTransform.position;

					if (useHookTargetPostionOffset) {
						targetPosition += hookTargetPositionOffset;
					}
						
					initialDistanceToTarget = GKC_Utils.distance (playerPosition, targetPosition);

					swingGravityForces = Vector3.zero;

					setCurrentPlayerActionSystemCustomActionCategoryID (true);
				}
			}
		}
	}

	public void removeGrapplingHook ()
	{
		if (grapplingHookActive) {
			if (checkingToRemoveHookActive) {
				removeHook ();
			}

			checkingToRemoveHookActive = true;
		}
	}

	public void checkRemoveGrapplingHook ()
	{
		if (grapplingHookEnabled) {
			removeGrapplingHook ();

			if (swingHoldInputActive) {
				swingHoldInputActive = false;
				stopUpdateSwingHoldInputActiveCoroutine ();
			}
		}
	}

	public void checkEventsOnGrapplingHook (bool state)
	{
		if (useEventsOnGrapplingHook) {
			if (state) {
				eventOnGrapplingHookActivate.Invoke ();
			} else {
				eventOnGrapplingHookDeactivate.Invoke ();
			}
		}
	}

	public void checkExternalControllerBehaviorToPause (string behaviorName)
	{
		if (grapplingHookActive) {
			if (checkExternalControllerStatesToIgnore) {
				externalControllerBehavior currentExternalControllerBehavior = mainPlayerController.getCurrentExternalControllerBehavior ();

				if (currentExternalControllerBehavior != null && behaviorName.Equals (currentExternalControllerBehavior.behaviorName)) {
					int currentIndex = otherExternalControllerInfoList.FindIndex (s => s.Name.Equals (behaviorName));

					if (currentIndex > -1) {
						currentExternalControllerBehavior.checkPauseStateDuringExternalForceOrBehavior ();

						if (otherExternalControllerInfoList [currentIndex].useEventsOnExternalController) {
							otherExternalControllerInfoList [currentIndex].eventToPauseExternalController.Invoke ();
						}
					}
				}
			}
		}
	}

	public void pauseOrResumePlayerState (bool state)
	{
		if (state) {
			if (mainPlayerController.isExternalControllBehaviorActive ()) {
				externalControllerBehavior currentExternalControllerBehavior = mainPlayerController.getCurrentExternalControllerBehavior ();

				if (currentExternalControllerBehavior != null) {
					bool canDisableExternalControllerState = true;
						
					if (checkExternalControllerStatesToIgnore) {
						
						int currentIndex = otherExternalControllerInfoList.FindIndex (s => s.Name.Equals (currentExternalControllerBehavior.behaviorName));

						if (currentIndex > -1) {
							currentExternalControllerBehavior.checkPauseStateDuringExternalForceOrBehavior ();

							if (otherExternalControllerInfoList [currentIndex].useEventsOnExternalController) {
								otherExternalControllerInfoList [currentIndex].eventToPauseExternalController.Invoke ();
							}

							canDisableExternalControllerState = false;
						}
					}

					if (canDisableExternalControllerState) {
						currentExternalControllerBehavior.disableExternalControllerState ();
					}
				}

				if (mainPlayerController.isPlayerOnFreeFloatingMode ()) {
					GKC_Utils.enableOrDisableFreeFloatingModeOnState (mainPlayerController.gameObject, false);
				}
			}

			mainPlayerController.setGravityForcePuase (true);

			mainPlayerController.setCheckOnGroungPausedState (true);

			mainPlayerController.setPlayerOnGroundState (false);

			mainGravitySystem.setCameraShakeCanBeUsedState (false);

			mainPlayerController.setJumpLegExternallyActiveState (true);

			mainPlayerController.setIgnoreExternalActionsActiveState (true);

			mainPlayerController.setIKBodyPausedState (true);
		} else {
			mainPlayerController.setGravityForcePuase (false);

			mainPlayerController.setCheckOnGroungPausedState (false);

			mainGravitySystem.setCameraShakeCanBeUsedState (true);

			mainPlayerController.setJumpLegExternallyActiveState (false);

			mainPlayerController.setLastTimeFalling ();

			mainPlayerController.setIgnoreExternalActionsActiveState (false);

			mainPlayerController.setIKBodyPausedState (false);
		}

		if (useSwingForcesActive) {
			if (state) {
				mainPlayerController.setOnGroundAnimatorIDValueWithoutCheck (false);
			}
//			mainPlayerController.setPauseMainRigidbodyVelocityUpdateState (state);

			if (!state) {
				removeJoint ();
			}
		}

		checkEventsOnGrapplingHook (state);

		bool stopCoroutine = false;

		if (changeCameraStateOnThirdPerson) {
			if (!mainPlayerCamera.isFirstPersonActive ()) {
				if (state) {
					if (mainPlayerCamera.isCameraTypeFree ()) {
						mainPlayerCamera.setCameraStateExternally (cameraStateNameOnGrapplingHookActivate);
					}
				} else {
					if (!keepCameraStateActiveWhileOnAir) {
						if (mainPlayerCamera.isCameraTypeFree ()) {
							mainPlayerCamera.setCameraStateExternally (cameraStateNameOnGrapplingHookDeactivate);
						}

						stopCoroutine = true;
					} else {
						checkinGrapplingHookCameraStateAfterDeactivate = true;
					}
				}
			} else {
				stopCoroutine = true;
			}
		} else {
			stopCoroutine = true;
		}

		if (changeFovOnHookActive) {
			if (state) {
				mainPlayerCamera.setMainCameraFov (regulaFov, changeFovSpeed);
			} else {
				mainPlayerCamera.setOriginalCameraFov ();
			}
		}

		if (useCameraShake) {
			if (state) {
				mainPlayerCamera.setShakeCameraState (true, regularCameraShakeName);
			} else {
				mainPlayerCamera.setShakeCameraState (false, "");

				mainPlayerCamera.stopShakeCamera ();
			}
		}

		if (showDebugPrint) {
			print ("set pause or resume climb state " + state);
		}

		if (applySpeedOnHookStop) {
			if (!state) {
				if (!ignoreApplySpeedOnHookStopOnPlayer && !ignoreApplySpeedOnHookStopTemporally) {
					lastVelocityAdded *= extraForceOnHookStopMultiplier;

					Vector3 forceOnHooKEnd = Vector3.ClampMagnitude (lastVelocityAdded, maxForceToApplyOnHookStop);

					mainPlayerController.addExternalForce (forceOnHooKEnd * extraSpeedOnHookStopMultiplier);

					if (showDebugPrint) {
						print ("addExternalForce");
					}
				}

				if (resetPlayerSpeedOnHookStop) {
					mainPlayerController.setPlayerVelocityToZero ();
				}
			}
		}

		ignoreApplySpeedOnHookStopTemporally = false;

		if (setAnimatorState) {
			if (state) {
				mainAnimator.SetInteger (actionIDAnimatorID, hookStartActionID);
				mainAnimator.CrossFadeInFixedTime (hookStartActionName, 0.1f);
			}

			mainAnimator.SetBool (actionActiveAnimatorID, state);
		}

		if (rotatePlayerTowardTargetDirection) {
			mainPlayerController.setAddExtraRotationPausedState (state);
		}

		if (state) {
			firstPersonActive = mainPlayerCamera.isFirstPersonActive ();
			previoslyFirstPersonActive = !firstPersonActive;
		}

		if (!state) {
			if (stopCoroutine) {
				stopGrapplingHookCoroutine ();
			}
		}

		manualPullActive = false;

		manualExtendActive = false;

		lastTimeAutomaticPullActive = 0;

		automaticPullActive = false;

		if (!state) {
			if (checkExternalControllerStatesToIgnore) {
				if (mainPlayerController.isExternalControllBehaviorActive ()) {
					externalControllerBehavior currentExternalControllerBehavior = mainPlayerController.getCurrentExternalControllerBehavior ();

					if (currentExternalControllerBehavior != null) {
						int currentIndex = otherExternalControllerInfoList.FindIndex (s => s.Name == currentExternalControllerBehavior.behaviorName);

						if (currentIndex > -1) {
							currentExternalControllerBehavior.checkResumeStateAfterExternalForceOrBehavior ();

							if (otherExternalControllerInfoList [currentIndex].useEventsOnExternalController) {
								otherExternalControllerInfoList [currentIndex].eventToResumeExternalController.Invoke ();
							}
						}
					}
				}
			}
		}
	}

	public void setIgnoreApplySpeedOnHookStopTemporallyState (bool state)
	{
		if (grapplingHookActive) {
			ignoreApplySpeedOnHookStopTemporally = state;
		}
	}

	void stopThrowHookAnimationCoroutine ()
	{
		if (animationCoroutine != null) {
			StopCoroutine (animationCoroutine);
		}

		throwAnimationInProcess = false;
	}

	IEnumerator throwHookAnimationCoroutine ()
	{
		throwAnimationInProcess = true;

		mainPlayerController.setIKBodyPausedState (true);

		mainAnimator.CrossFade (throwHookAnimationName, 0.1f);

		mainAnimator.SetBool (actionActiveAnimatorID, true);


		WaitForSeconds delay = new WaitForSeconds (throwHookAnimationDuration);

		yield return delay;


		mainAnimator.SetBool (actionActiveAnimatorID, false);

		checkThrowGrapplingHook ();

		if (grapplingHookActive) {
			checkingToRemoveHookActive = true;
		}
	
		throwAnimationInProcess = false;
	}

	bool checkGetClosestSurfaceIfNotFoundOnMainRaycast (float rightAngle)
	{
		bool surfaceFoundResult = false;

		Vector3 forward = mainPlayerCamera.transform.forward;

		forward = Quaternion.AngleAxis (-rightAngle, mainPlayerCamera.transform.right) * forward;

//		print (forward + " " + rightAngle);

		Debug.DrawRay (currentRaycastPosition, forward * maxRaycastDistance * 200, Color.red, 5);

		float startAngle = 0;
		float endAngle = swingClosestSurfaceAngleSearchRange.y;

		Vector3 currentForwardDirection = forward;

		int loopCount = 0;

		bool pointLocated = false;

		float closestDistance = Mathf.Infinity;

		RaycastHit closestHit = new RaycastHit ();

		for (float angle = startAngle; angle <= endAngle; angle += 5f) {
//			if (!pointLocated) {

			Quaternion rotation = Quaternion.Euler (0, angle, 0);
			Vector3 direction = rotation * currentForwardDirection;

			Debug.DrawRay (currentRaycastPosition, direction * maxRaycastDistance, Color.green, 5);

			surfaceFoundResult = Physics.Raycast (currentRaycastPosition, direction, out hit, maxRaycastDistance, layerToCheckSurfaces);

			if (surfaceFoundResult) {
//					pointLocated = true;

//					print ("point located");

				currentRaycastDirection = direction;

				float distance = hit.distance;

				// Comprobar si la distancia es la más cercana hasta ahora
				if (distance < closestDistance) {
					closestDistance = distance;

					closestHit = hit;
				}
			}

//				loopCount++;
//
//				if (loopCount > 100) {
//					pointLocated = true;
//				}
//			}
		}

		if (surfaceFoundResult) {
			hit = closestHit;
		}

		if (!surfaceFoundResult) {
			startAngle = 0;
			endAngle = -swingClosestSurfaceAngleSearchRange.x;

			currentForwardDirection = forward;

			loopCount = 0;

			pointLocated = false;

			for (float angle = startAngle; angle <= endAngle; angle += 5f) {
				if (!pointLocated) {

					Quaternion rotation = Quaternion.Euler (0, -angle, 0);
					Vector3 direction = rotation * currentForwardDirection;

					Debug.DrawRay (currentRaycastPosition, direction * maxRaycastDistance, Color.green, 5);

					surfaceFoundResult = Physics.Raycast (currentRaycastPosition, direction, out hit, maxRaycastDistance, layerToCheckSurfaces);

					if (surfaceFoundResult) {
						pointLocated = true;

//						print ("point located");

						currentRaycastDirection = direction;
					}

					loopCount++;

					if (loopCount > 100) {
						pointLocated = true;
					}
				}
			}
		}

		return surfaceFoundResult;
	}

	public void inputThrowGrapplingHook ()
	{
		if (!grapplingHookEnabled) {
			return;
		}

		if (throwAnimationInProcess) {
			return;
		}

		bool useAnimationResult = false;

		if (useAnimationToThrowHook && !mainPlayerCamera.isFirstPersonActive ()) {
			useAnimationResult = true;

			if (useAnimationToThrowHookOnlyOnGround) {
				if (!mainPlayerController.isPlayerOnGround ()) {
					useAnimationResult = false;
				}
			}
		}
			
		if (useAnimationResult) {
			if (!grapplingHookActive) {
				if (checkIfSurfaceDetectedBeforeAnimation) {

					calculateRaycastValues ();

					swingRaycastDetected = false;

					bool surfaceFoundResult =
						Physics.Raycast (currentRaycastPosition, currentRaycastDirection, out hit, maxRaycastDistance, layerToCheckSurfaces);

					if (!surfaceFoundResult) {
						if (useSwingForcesActive) {
							if (getClosestSurfaceIfNotFoundOnMainRaycast) {
								surfaceFoundResult = checkGetClosestSurfaceIfNotFoundOnMainRaycast (60);

								if (!surfaceFoundResult) {
									surfaceFoundResult = checkGetClosestSurfaceIfNotFoundOnMainRaycast (50);
								}

								if (!surfaceFoundResult) {
									surfaceFoundResult = checkGetClosestSurfaceIfNotFoundOnMainRaycast (40);
								}

								if (!surfaceFoundResult) {
									surfaceFoundResult = checkGetClosestSurfaceIfNotFoundOnMainRaycast (30);
								}
							}
						}
					}

					if (!surfaceFoundResult) {
						return;
					} else {
						if (useSwingForcesActive) {

							if (hit.point.y < playerControllerTransform.position.y ||
							    Mathf.Abs (Mathf.Abs (hit.point.y) - Mathf.Abs (playerControllerTransform.position.y)) < 2) {

								if (GKC_Utils.distance (playerControllerTransform.position, hit.point) < 5) {
									if (showDebugPrint) {
										print ("point detected is below player, cancelling swing");
									}

									return;
								}
							}

							swingRaycastDetected = true;

							swingRacyastCheck = hit;

							addJoint (hit.point);
						}
					}
				}

				stopThrowHookAnimationCoroutine ();

				animationCoroutine = StartCoroutine (throwHookAnimationCoroutine ());
			}
		} else {
			if (useSwingForcesActive) {
				swingRaycastDetected = false;

				calculateRaycastValues ();

				bool surfaceFoundResult = 
					Physics.Raycast (currentRaycastPosition, currentRaycastDirection, out hit, maxRaycastDistance, layerToCheckSurfaces);

				if (!surfaceFoundResult) {
					if (useSwingForcesActive) {
						if (getClosestSurfaceIfNotFoundOnMainRaycast) {
							surfaceFoundResult = checkGetClosestSurfaceIfNotFoundOnMainRaycast (60);

							if (!surfaceFoundResult) {
								surfaceFoundResult = checkGetClosestSurfaceIfNotFoundOnMainRaycast (50);
							}

							if (!surfaceFoundResult) {
								surfaceFoundResult = checkGetClosestSurfaceIfNotFoundOnMainRaycast (40);
							}

							if (!surfaceFoundResult) {
								surfaceFoundResult = checkGetClosestSurfaceIfNotFoundOnMainRaycast (30);
							}
						}
					}
				}

				if (surfaceFoundResult) {
					if (hit.point.y < playerControllerTransform.position.y ||
					    Mathf.Abs (Mathf.Abs (hit.point.y) - Mathf.Abs (playerControllerTransform.position.y)) < 2) {
						if (GKC_Utils.distance (playerControllerTransform.position, hit.point) < 5) {
							if (showDebugPrint) {
								print ("point detected is below player, cancelling swing");
							}

							return;
						}
					}

					swingRaycastDetected = true;

					swingRacyastCheck = hit;

					addJoint (hit.point);
				}
			}
				
			checkThrowGrapplingHook ();
		}
	}

	void addJoint (Vector3 jointPosition)
	{
		mainSwingRigidbody.transform.position = mainRigidbody.position;

		mainSwingRigidbody.transform.rotation = mainRigidbody.rotation;

		mainSwingRigidbody.velocity = mainRigidbody.velocity;

		swingJoint.autoConfigureConnectedAnchor = false;
		swingJoint.connectedAnchor = jointPosition;

		float distanceFromPoint = GKC_Utils.distance (playerControllerTransform.position, jointPosition);
	
		swingJoint.maxDistance = distanceFromPoint * 0.8f;
		swingJoint.minDistance = distanceFromPoint * 0.25f;
	
		Physics.IgnoreCollision (mainSwingCollider, mainPlayerController.getMainCollider (), true);

		mainSwingRigidbody.gameObject.SetActive (true);
	}

	void removeJoint ()
	{
		Vector3 lastVelocity = mainSwingRigidbody.velocity;

		mainPlayerController.setExtraCharacterVelocity (Vector3.zero);

		mainSwingRigidbody.gameObject.SetActive (false);

		mainPlayerController.addExternalForce (lastVelocity);
	}

	public void inputRemoveGrapplingHook ()
	{
		checkRemoveGrapplingHook ();
	}

	public void inputIncreaseOrDecreaseMovementSpeed (bool state)
	{
		if (grapplingHookEnabled) {
			if (grapplingHookActive) {
				increaseSpeedActive = state;

				if (changeFovOnHookActive) {
					if (increaseSpeedActive) {
						mainPlayerCamera.setMainCameraFov (increaseSpeedFov, changeFovSpeed);
					} else {
						mainPlayerCamera.setMainCameraFov (regulaFov, changeFovSpeed);
					}
				}

				if (useCameraShake) {
					if (increaseSpeedActive) {
						mainPlayerCamera.setShakeCameraState (true, increaseCamaraShakeName);
					} else {
						mainPlayerCamera.setShakeCameraState (true, regularCameraShakeName);
					}
				}
			}
		}
	}

	public void inputThrowGrapplingHookIfTargetLocated ()
	{
		if (grapplingHookEnabled) {
			if (currentGrapplingHookTarget != null) {
				if (grapplingHookActive) {
					checkRemoveGrapplingHook ();
				} else {
					inputThrowGrapplingHook ();
				}
			}
		}
	}

	public void inputSetManualPullActiveState (bool state)
	{
		if (grapplingHookEnabled) {
			if (!manualPullEnabled) {
				return;
			}

			if (grapplingHookActive && useSwingForcesActive) {
				manualPullActive = state;

				manualExtendActive = false;
			}
		}
	}

	public void inputSetManualExtendActiveState (bool state)
	{
		if (grapplingHookEnabled) {
			if (!manualExtendEnabled) {
				return;
			}

			if (grapplingHookActive && useSwingForcesActive) {
				manualExtendActive = state;

				manualPullActive = false;
			}
		}
	}

	public void inputToggleManualPullActiveState ()
	{
		inputSetManualPullActiveState (!manualPullActive);
	}

	public void inputUseJumpOnSwing ()
	{
		if (grapplingHookEnabled) {
			if (grapplingHookActive && useSwingForcesActive) {
				if (jumpOnSwingEnabled) {
					removeGrapplingHook ();

					if (useSwingJumpOnAir) {
						mainAnimator.SetInteger (actionIDAnimatorID, swingJumpOnAirID);
					}

					Vector3 totalForce = jumpOnSwingForceAmount.y * playerControllerTransform.up + jumpOnSwingForceAmount.z * playerControllerTransform.forward;

					mainPlayerController.useJumpPlatform (totalForce, ForceMode.Impulse);

					lastTimeSwingHoldStopped = Time.time;
				}
			}
		}
	}

	public void inputSetSwingHoldInputActive (bool state)
	{
		if (grapplingHookEnabled) {
			if (chainNextSwingIfInputHold) {
				swingHoldInputActive = state;

				if (swingHoldInputActive) {
					swingHoldInputActiveCoroutine = StartCoroutine (updateSwingHoldInputActiveCoroutine ());
				} else {
					stopUpdateSwingHoldInputActiveCoroutine ();

					if (grapplingHookActive) {
						float probabilityToUseJump = Random.Range (0, 100);

						if (probabilityToUseJumpOnSwingEnd > probabilityToUseJump) {
							inputUseJumpOnSwing ();
						} else {
							removeHook ();
						}
					}
				}
			}
		}
	}

	Coroutine swingHoldInputActiveCoroutine;

	float lastTimeSwingHoldStopped = -1;

	void stopUpdateSwingHoldInputActiveCoroutine ()
	{
		if (swingHoldInputActiveCoroutine != null) {
			StopCoroutine (swingHoldInputActiveCoroutine);
		}
	}

	IEnumerator updateSwingHoldInputActiveCoroutine ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			if (grapplingHookActive) {
				bool removeHookResult = false;

				if (swingPassCenterChecked) {
					if (lastTimeSwingPassCenterChecked > -1 && Time.time > lastTimeSwingPassCenterChecked + minTimeToKeepSwingBeforeNext) {
						removeHookResult = true;
					}
				} else {
					if (Time.time > lastTimeHookActive + minTimeToKeepSwingBeforeNext * 3) {
						removeHookResult = true;
					}
				}

				if (removeHookResult) {
					float probabilityToUseJump = Random.Range (0, 100);

					if (probabilityToUseJumpOnSwingEnd > probabilityToUseJump) {
						inputUseJumpOnSwing ();
					} else {
						removeHook ();
					}

					lastTimeSwingHoldStopped = Time.time;
				}
			} else {
				if (Time.time > lastTimeSwingHoldStopped + minWaitTimeToThrowNextSwing) {
					inputThrowGrapplingHook ();

					if (grapplingHookActive) {
						lastTimeSwingHoldStopped = -1;
					} else {
						if (stopSwingHoldChainIfNotSurfaceLocated) {
							inputSetSwingHoldInputActive (false);

						} else {
							lastTimeSwingHoldStopped = Time.time;
						}
					}
				}
			}

			yield return waitTime;
		}
	}

	public void setAttractObjectsEnabled (bool state)
	{
		attractObjectsEnabled = state;
	}

	public void setUseSwingForcesActiveState (bool state)
	{
		useSwingForcesActive = state;
	}

	public void setCurrentPlayerActionSystemCustomActionCategoryID (bool state)
	{
		if (state) {
			if (customActionCategoryID > -1) {
				mainPlayerController.setCurrentCustomActionCategoryID (customActionCategoryID);
			}
		} else {
			if (regularActionCategoryID > -1) {
				mainPlayerController.setCurrentCustomActionCategoryID (regularActionCategoryID);
			}
		}
	}

	[System.Serializable]
	public class otherExternalControllerInfo
	{
		public string Name;

		[Space]
		[Header ("Events Settings")]
		[Space]

		public bool useEventsOnExternalController;
		public UnityEvent eventToPauseExternalController;
		public UnityEvent eventToResumeExternalController;
	}
}

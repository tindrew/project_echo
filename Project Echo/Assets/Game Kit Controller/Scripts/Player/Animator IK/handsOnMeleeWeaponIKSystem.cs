using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class handsOnMeleeWeaponIKSystem : OnAnimatorIKComponent
{
	[Header ("Main Settings")]
	[Space]

	public bool adjustHandsEnabled = true;

	public float weightLerpSpeed = 6;

	public float busyBodyPartWeightLerpSpeed = 3;

	public float minWaitTimeToActiveHands = 0.3f;

	[Space]
	[Header ("Hands Info List Settings")]
	[Space]

	public List<handInfo> handInfoList = new List<handInfo> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool handsOnMeleeActive;

	public bool usingHands;

	public bool isBusy;

	public bool adjustHandsPaused;

	public bool smoothBusyDisableActive;

	public bool handsCheckPausedWithDelayActive;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public bool useEventsOnActiveStateChange;
	public UnityEvent eventOnActive;
	public UnityEvent eventOnDeactivate;

	[Space]
	[Header ("Component Elements")]
	[Space]

	public Animator animator;
	public IKSystem IKManager;
	public playerController playerControllerManager;
	public meleeWeaponsGrabbedManager mainMeleeWeaponsGrabbedManager;

	handInfo currentHandInfo;

	bool isThirdPersonView;

	float originalWeightLerpSpeed;

	float currentWeightToApply;

	AvatarIKGoal currentIKGoal;

	int handInfoListCount;

	float minIKValue = 0.001f;

	Coroutine delayToResumeStateCoroutine;

	bool valuesInitialized;

	float lastTimeHandsActive = 0;

	float currentWeightLerpSpeed;


	public override void updateOnAnimatorIKState ()
	{
		if (handsOnMeleeActive) {
			isThirdPersonView = !playerControllerManager.isPlayerOnFirstPerson ();

			usingHands = IKManager.getUsingHands ();

			if (IKManager.isObjectGrabbed ()) {
				usingHands = false;
			}

			isBusy = playerIsBusy ();

			if (Time.time < lastTimeHandsActive + minWaitTimeToActiveHands) {
				isBusy = true;
			}

			bool IKBodyPaused = IKManager.isIKBodyPaused ();

			bool pauseIKBodyResult = playerControllerManager.isActionActive () ||
			                         mainMeleeWeaponsGrabbedManager.isAimingBowActive () ||
			                         mainMeleeWeaponsGrabbedManager.isCuttingModeActive ();

			if (!isBusy || smoothBusyDisableActive) {

				for (int j = 0; j < handInfoListCount; j++) {

					currentHandInfo = handInfoList [j];

					if (currentHandInfo.IKBodyPartEnabled) {
						if (IKBodyPaused || pauseIKBodyResult) {
							if (!currentHandInfo.bodyPartBusy) {
								currentHandInfo.bodyPartBusy = true;
								currentHandInfo.IKBodyWeigthTarget = 0;
							}
						} else {

							if (!usingHands) {
								if (currentHandInfo.bodyPartBusy) {
									currentHandInfo.bodyPartBusy = false;
									currentHandInfo.IKBodyWeigthTarget = 1;
								} else {
									currentHandInfo.IKBodyWeigthTarget = 1;
								}
							} else {
								if (!currentHandInfo.bodyPartBusy) {
									currentHandInfo.bodyPartBusy = true;
									currentHandInfo.IKBodyWeigthTarget = 0;
								}
							}
						}

						if (currentHandInfo.currentIKWeight != currentHandInfo.IKBodyWeigthTarget) {
							
							if (currentHandInfo.bodyPartBusy) {
								currentWeightLerpSpeed = busyBodyPartWeightLerpSpeed;
							} else {
								currentWeightLerpSpeed = weightLerpSpeed;
							}

							currentHandInfo.currentIKWeight = Mathf.MoveTowards (currentHandInfo.currentIKWeight,
								currentHandInfo.IKBodyWeigthTarget, Time.fixedDeltaTime * currentWeightLerpSpeed);
						}

						bool applyIKToBodyPart = false;

						if (!currentHandInfo.bodyPartBusy) {

							currentHandInfo.IKGoalPosition = currentHandInfo.targetToFollow.position;          
							currentHandInfo.IKGoalRotation = currentHandInfo.targetToFollow.rotation;

							applyIKToBodyPart = true;

						} else if (currentHandInfo.currentIKWeight != currentHandInfo.IKBodyWeigthTarget) {
							applyIKToBodyPart = true;
						}

						if (applyIKToBodyPart) {

							currentWeightToApply = currentHandInfo.currentIKWeight;

							if (currentWeightToApply > minIKValue) {
								currentIKGoal = currentHandInfo.IKGoal;

								animator.SetIKRotationWeight (currentIKGoal, currentWeightToApply);
								animator.SetIKPositionWeight (currentIKGoal, currentWeightToApply);
								animator.SetIKPosition (currentIKGoal, currentHandInfo.IKGoalPosition);
								animator.SetIKRotation (currentIKGoal, currentHandInfo.IKGoalRotation);
							}
						}
					}
				}
			} else {
				for (int j = 0; j < handInfoListCount; j++) {

					currentHandInfo = handInfoList [j];

					currentHandInfo.bodyPartBusy = false;

					currentHandInfo.IKBodyWeigthTarget = 0;

					currentHandInfo.currentIKWeight = 0;
				}
			}
		}
	}

	public bool playerIsBusy ()
	{
		return 

			(usingHands && !IKManager.isObjectGrabbed ()) ||
		!isThirdPersonView ||
		playerControllerManager.isPlayerOnFFOrZeroGravityModeOn () ||
		playerControllerManager.isUsingCloseCombatActive () ||
		playerControllerManager.isPlayerDead () ||
		adjustHandsPaused ||
		playerControllerManager.isUsingJetpack () ||
		playerControllerManager.isFlyingActive () ||
		playerControllerManager.isSwimModeActive ();
	}

	public void setWeightLerpSpeedValue (float newValue)
	{
		weightLerpSpeed = newValue;
	}

	public void setOriginalWeightLerpSpeed ()
	{
		setWeightLerpSpeedValue (originalWeightLerpSpeed);
	}

	public void setAdjustHandsPausedState (bool state)
	{
		adjustHandsPaused = state;
	}

	public void setSmoothBusyDisableActiveState (bool state)
	{
		smoothBusyDisableActive = state;
	}

	public void setAdjustHandsEnabledState (bool state)
	{
		adjustHandsEnabled = state;
	}


	public void enableHandsCheckPausedWithDelayActive (float duration)
	{
		stopUpdateDelayToResumeStateCoroutine ();

		delayToResumeStateCoroutine = StartCoroutine (updateDelayToResumeStateCoroutine (duration));
	}

	public void stopUpdateDelayToResumeStateCoroutine ()
	{
		if (delayToResumeStateCoroutine != null) {
			StopCoroutine (delayToResumeStateCoroutine);
		}

		handsCheckPausedWithDelayActive = false;
	}

	IEnumerator updateDelayToResumeStateCoroutine (float duration)
	{
		handsCheckPausedWithDelayActive = true;

		WaitForSeconds delay = new WaitForSeconds (duration);

		yield return delay;

		handsCheckPausedWithDelayActive = false;
	}

	public override void enableBothHands ()
	{
		enableOrDisableRightOrLeftHand (true, false, true);
	}

	public override void enableOnlyLeftHand ()
	{
		enableOrDisableRightOrLeftHand (true, false, false);
	}

	public override void enableOnlyRightHand ()
	{
		enableOrDisableRightOrLeftHand (true, true, false);
	}

	public override void enableOrDisableRightOrLeftHand (bool state, bool isRightHand, bool setStateOnBothHands)
	{
		for (int j = 0; j < handInfoListCount; j++) {

			currentHandInfo = handInfoList [j];

			if (setStateOnBothHands) {
				currentHandInfo.IKBodyPartEnabled = state;
			} else {
				if (currentHandInfo.isRightHand) {
					if (isRightHand) {
						currentHandInfo.IKBodyPartEnabled = state;
					} else {
						currentHandInfo.IKBodyPartEnabled = false;

						currentHandInfo.bodyPartBusy = false;

						currentHandInfo.IKBodyWeigthTarget = 0;

						currentHandInfo.currentIKWeight = 0;
					}
				} else {
					if (isRightHand) {
						currentHandInfo.IKBodyPartEnabled = false;

						currentHandInfo.bodyPartBusy = false;

						currentHandInfo.IKBodyWeigthTarget = 0;

						currentHandInfo.currentIKWeight = 0;
					} else {
						currentHandInfo.IKBodyPartEnabled = state;
					}
				}
			}

			bool checkEnableEventsResult = currentHandInfo.IKBodyPartEnabled;

			if (currentHandInfo.useEventOnIKPartEnabledStateChange) {
				if (checkEnableEventsResult) {
					currentHandInfo.eventOnIKPartEnabled.Invoke ();
				} else {
					currentHandInfo.eventOnIKPartDisabled.Invoke ();
				}
			}
		}
	}

	public override void setActiveState (bool state)
	{
		if (!adjustHandsEnabled) {
			return;
		}

		if (handsOnMeleeActive == state) {
			return;
		}

		handsOnMeleeActive = state;

		if (handsOnMeleeActive) {
			if (!valuesInitialized) {
				originalWeightLerpSpeed = weightLerpSpeed;

				handInfoListCount = handInfoList.Count;

				valuesInitialized = true;
			}

			lastTimeHandsActive = Time.time;
		} else {
			for (int j = 0; j < handInfoListCount; j++) {

				currentHandInfo = handInfoList [j];

				currentHandInfo.bodyPartBusy = false;

				currentHandInfo.IKBodyWeigthTarget = 0;

				currentHandInfo.currentIKWeight = 0;
			}
		}

		if (useEventsOnActiveStateChange) {
			if (state) {
				eventOnActive.Invoke ();
			} else {
				eventOnDeactivate.Invoke ();
			}
		}
	}

	public void disableStateIfNotCurrentlyActiveOnIKSystem ()
	{
		if (handsOnMeleeActive) {
			if (!IKManager.checkIfTemporalOnAnimatorIKComponentIfIsCurrent (this)) {
				setActiveState (false);
			}
		}
	}

	//EDITOR FUNCTIONS
	public void setAdjustHandsEnabledStateFromEditor (bool state)
	{
		setAdjustHandsEnabledState (state);

		updateComponent ();
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}

	[System.Serializable]
	public class handInfo
	{
		[Header ("Main Settings")]
		[Space]

		public string Name;
		public bool IKBodyPartEnabled = true;

		public bool isRightHand;

		public Transform targetToFollow;

		[Space]
		[Header ("IK Settings")]
		[Space]

		public AvatarIKGoal IKGoal;

		[Space]
		[Header ("Debug")]
		[Space]

		public bool bodyPartBusy;

		public float currentIKWeight;
		public float IKBodyWeigthTarget;

		[HideInInspector] public Vector3 IKGoalPosition;
		[HideInInspector] public Quaternion IKGoalRotation;

		[Space]
		[Header ("Event Settings")]
		[Space]

		public bool useEventOnIKPartEnabledStateChange;
		public UnityEvent eventOnIKPartEnabled;
		public UnityEvent eventOnIKPartDisabled;
	}
}
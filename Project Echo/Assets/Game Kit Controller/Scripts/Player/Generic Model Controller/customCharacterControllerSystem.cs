using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class customCharacterControllerSystem : customCharacterControllerBase
{
	[Header ("Custom Settings")]
	[Space]

	public string horizontalAnimatorName = "Horizontal";
	public string verticalAnimatorName = "Vertical";
	public string stateAnimatorName = "State";

	public string groundedStateAnimatorName = "Grounded";
	public string movementAnimatorName = "Movement";
	public string speedMultiplierAnimatorName = "SpeedMultiplier";

	[Space]
	[Header ("Other Settings")]
	[Space]

	public int jumpState = 2;
	public int movementState = 1;
	public int fallState = 3;
	public int deathState = 10;

	[Space]
	[Header ("Rotation To Surface Settings")]
	[Space]

	public bool rotateCharacterModelToSurfaceEnabled;

	public float maxCharacterRotationToSurface = 40;

	public float minSurfaceRotation = 5;

	public float characterRotationToSurfaceSpeed = 10;

	public float characterPositionAdjustmentToSurfaceSpeed = 10;

	public float characterRotationToSurfaceRaycastDistance = 2.5f;

	public Transform characterPivotTransform;

	public LayerMask surfaceRotationLayermask;

	[Space]
	[Header ("Debug")]
	[Space]

	public int currentState;

	public Vector3 targetPosition;

	public bool surfaceFound;

	public float currentSurfaceHitAngle;


	int horizontalAnimatorID;
	int verticalAnimatorID;

	int stateAnimatorID;
	int groundedStateAnimatorID;

	int movementAnimatorID;

	bool valuesInitialized;

	//	int speedMultiplierAnimatorID;
	//	int deltaAngleAnimatorID;

	RaycastHit hit;

	void initializeValues ()
	{
		if (!valuesInitialized) {
			horizontalAnimatorID = Animator.StringToHash (horizontalAnimatorName);
			verticalAnimatorID = Animator.StringToHash (verticalAnimatorName);

			stateAnimatorID = Animator.StringToHash (stateAnimatorName);
			groundedStateAnimatorID = Animator.StringToHash (groundedStateAnimatorName);
			movementAnimatorID = Animator.StringToHash (movementAnimatorName);

//			speedMultiplierAnimatorID = Animator.StringToHash (speedMultiplierAnimatorName);
//			deltaAngleAnimatorID = Animator.StringToHash (deltaAngleAnimatorName);

			valuesInitialized = true;
		}
	}

	public override void updateCharacterControllerState ()
	{
		updateAnimatorFloatValueLerping (horizontalAnimatorID, turnAmount, animatorTurnInputLerpSpeed, Time.fixedDeltaTime);

		updateAnimatorFloatValueLerping (verticalAnimatorID, forwardAmount, animatorForwardInputLerpSpeed, Time.fixedDeltaTime);

		updateAnimatorBoolValue (groundedStateAnimatorID, onGround);

		updateAnimatorBoolValue (movementAnimatorID, playerUsingInput);

		if (canMove && !ragdollCurrentlyActive) {
			if (rotateCharacterModelToSurfaceEnabled) {

				Vector3 raycastPosition = playerControllerTransform.position + playerControllerTransform.up;

				Vector3 raycastDirection = -playerControllerTransform.up;

				currentSurfaceHitAngle = 0;

				float hitDistance = 0;

				targetPosition = originalCharacterPivotTransformPosition;

				surfaceFound = false;

				if (Physics.Raycast (raycastPosition, raycastDirection, out hit, characterRotationToSurfaceRaycastDistance, surfaceRotationLayermask)) {
					currentSurfaceHitAngle = Vector3.SignedAngle (playerControllerTransform.up, hit.normal, playerControllerTransform.right);

					hitDistance = hit.distance - 1;

					targetPosition.y -= hitDistance;

					surfaceFound = true;
				}

				currentSurfaceHitAngle = Mathf.Clamp (currentSurfaceHitAngle, -maxCharacterRotationToSurface, maxCharacterRotationToSurface);

				if (Mathf.Abs (currentSurfaceHitAngle) < minSurfaceRotation) {
					currentSurfaceHitAngle = 0;

					surfaceFound = false;
				}

				Vector3 targetEuler = new Vector3 (currentSurfaceHitAngle, 0, 0);

				Quaternion targetRotation = Quaternion.Euler (targetEuler);

				if (currentSurfaceHitAngle != 0 || Mathf.Abs (characterPivotTransform.localEulerAngles.x) > 2) {
					characterPivotTransform.localRotation = Quaternion.Lerp (characterPivotTransform.localRotation, targetRotation, Time.deltaTime * characterRotationToSurfaceSpeed);
				}

				if (surfaceFound) {
					characterPivotTransform.localPosition = Vector3.Lerp (characterPivotTransform.localPosition, targetPosition, Time.deltaTime * characterPositionAdjustmentToSurfaceSpeed);
				} else {
					characterPivotTransform.localPosition = Vector3.Lerp (characterPivotTransform.localPosition, originalCharacterPivotTransformPosition, Time.deltaTime * characterPositionAdjustmentToSurfaceSpeed);
				}
			}
		}
	}
		
	public override void updateCharacterControllerAnimator ()
	{
		
	}

	public override void updateMovementInputValues (Vector3 newValues)
	{

	}

	public override void updateHorizontalVerticalInputValues (Vector2 newValues)
	{

	}

	public override void activateJumpAnimatorState ()
	{
		updateAnimatorIntegerValue (stateAnimatorID, jumpState);

		currentState = jumpState;
	}

	public override void updateOnGroundValue (bool state)
	{
		base.updateOnGroundValue (state);

		if (currentState == 1) {
			if (!onGround) {
				updateAnimatorIntegerValue (stateAnimatorID, 3);

				currentState = 3;
			}
		} else {
			if (onGround) {
				updateAnimatorIntegerValue (stateAnimatorID, 1);

				currentState = 1;
			} else {

//				if (currentState == 2) {
//					updateAnimatorIntegerValue (stateAnimatorID, 20);
//
//					currentState = 20;
//				}
			}
		}
	}

	public override void setCharacterControllerActiveState (bool state)
	{
		base.setCharacterControllerActiveState (state);

		if (state) {
			initializeValues ();
		}
	}
}

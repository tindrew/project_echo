using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class freeClimbSystemIK : OnAnimatorIKComponent
{
	[Header ("Main Settings")]
	[Space]

	public bool climbSystemIKActive;

	public LayerMask layerMask;

	public float IKWeightEnabledSpeed = 10;
	public float IKWeightDisabledSpeed = 2;

	public float raycastDistance = 1.3f;

	public List<bodyPartIKInfo> bodyPartIKInfoList = new List<bodyPartIKInfo> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool playerIsMoving;

	[Space]
	[Header ("Components")]
	[Space]

	public freeClimbSystem mainFreeClimbSystem;
	public Animator mainAnimator;
	public Transform playerTransform;

	RaycastHit hit;

	bool checkRaycast;

	Vector3 currentTransformForward;

	AvatarIKGoal currentIKGoal;

	float minIKValue = 0.001f;

	float currentAdjustmentSpeed;

	bodyPartIKInfo currentBodyPartIKInfo;

	int bodyPartIKInfoListCount;


	public override void updateOnAnimatorIKState ()
	{
		if (!updateIKEnabled) {
			return;
		}

		if (!mainFreeClimbSystem.climbActive) {
			return;
		}

		currentTransformForward = playerTransform.forward;

		bool isPlayerMoving = mainFreeClimbSystem.isPlayerMoving ();

		float currentDeltatime = Time.fixedDeltaTime;

		for (int i = 0; i < bodyPartIKInfoListCount; i++) {
			currentBodyPartIKInfo = bodyPartIKInfoList [i];

			AvatarIKGoal currentIKGoal = currentBodyPartIKInfo.IKGoal;

			checkRaycast = true;

			currentBodyPartIKInfo.targetWeight = 0;

			currentAdjustmentSpeed = IKWeightDisabledSpeed;

			if (isPlayerMoving) {
				checkRaycast = false;
			}

			if (checkRaycast) {
				Vector3 direction = currentTransformForward;

				if (Physics.Raycast (currentBodyPartIKInfo.IKGoalTransform.position - direction, direction, out hit, raycastDistance, layerMask)) {
					currentBodyPartIKInfo.newBodyPartPosition = hit.point - direction * currentBodyPartIKInfo.bodyPartOffset;

					currentBodyPartIKInfo.targetWeight = 1;

					currentAdjustmentSpeed = IKWeightEnabledSpeed;
				}
			}
				
			currentBodyPartIKInfo.IKWeight = 
				Mathf.Clamp01 (Mathf.Lerp (currentBodyPartIKInfo.IKWeight, currentBodyPartIKInfo.targetWeight, currentAdjustmentSpeed * currentDeltatime));

			if (currentBodyPartIKInfo.IKWeight >= minIKValue) {

				mainAnimator.SetIKPosition (currentIKGoal, currentBodyPartIKInfo.newBodyPartPosition);

				mainAnimator.SetIKPositionWeight (currentIKGoal, currentBodyPartIKInfo.IKWeight);
			}
		}
	}

	public override void setActiveState (bool state)
	{
		climbSystemIKActive = state;

		if (climbSystemIKActive) {

			bodyPartIKInfoListCount = bodyPartIKInfoList.Count;

			for (int i = 0; i < bodyPartIKInfoListCount; i++) {
				if (bodyPartIKInfoList [i].IKGoalTransform == null) {
					bodyPartIKInfoList [i].IKGoalTransform = mainAnimator.GetBoneTransform (bodyPartIKInfoList [i].mainBone);
				}

				bodyPartIKInfoList [i].IKWeight = 0;
				bodyPartIKInfoList [i].targetWeight = 0;

				bodyPartIKInfoList [i].newBodyPartPosition = Vector3.zero;
			}
		} else {

		}
	}

	[System.Serializable]
	public class bodyPartIKInfo
	{
		public string Name;
		public AvatarIKGoal IKGoal;
		public HumanBodyBones mainBone;
		public Transform IKGoalTransform;

		public float IKWeight;
		public float targetWeight;

		public Vector3 newBodyPartPosition;

		public float bodyPartOffset = 0.1f;
	}
}
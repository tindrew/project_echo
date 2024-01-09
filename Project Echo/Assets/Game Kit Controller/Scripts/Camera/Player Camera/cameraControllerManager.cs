using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControllerManager : MonoBehaviour
{
	public virtual bool isFirstPersonActive ()
	{
		return false;
	}

	public virtual bool isMoveInXAxisOn2_5d ()
	{
		return false;
	}

	public virtual void setMoveInXAxisOn2_5dState (bool state)
	{
		
	}

	public virtual Vector3 getOriginalLockedCameraPivotPosition ()
	{
		return Vector3.zero;
	}

	public virtual void changeCameraFov (bool state)
	{

	}

	public virtual Transform getPivotCameraTransform ()
	{
		return null;
	}

	public virtual Transform getLockedCameraTransform ()
	{
		return null;
	}

	public virtual void crouch (bool isCrouching)
	{

	}

	public virtual void setMainCameraFovStartAndEnd (float startTargetValue, float endTargetValue, float speed)
	{

	}

	public virtual float getOriginalCameraFov ()
	{
		return -1;
	}

	public virtual bool isPlayerLookingAtTarget ()
	{
		return false;
	}

	public virtual void setShakeCameraState (bool state, string stateName)
	{
		
	}

	public virtual float getLastTimeMoved ()
	{
		return -1;
	}

	public virtual void setDrivingState (bool state)
	{
		
	}

	public virtual bool is2_5ViewActive ()
	{
		return false;
	}

	public virtual bool istargetToLookLocated ()
	{
		return false;
	}

	public virtual void setPlayerAndCameraParent (Transform newParent)
	{

	}

	public virtual void stopShakeCamera ()
	{
		
	}

	public virtual Transform getCurrentTargetToLook ()
	{
		return null;
	}

	public virtual void setUpdatePlayerCameraPositionOnLateUpdateActiveState (bool state)
	{
		
	}

	public virtual void setUpdatePlayerCameraPositionOnFixedUpdateActiveState (bool state)
	{
		
	}

	public virtual bool isFullBodyAwarenessEnabled ()
	{
		return false;
	}

	public virtual void setPivotCameraTransformParentCurrentTransformToFollow ()
	{

	}

	public virtual void setPivotCameraTransformOriginalParent ()
	{
		
	}

	public virtual void resetPivotCameraTransformLocalRotation ()
	{
		
	}

	public virtual void setHeadColliderStateOnFBA (bool state)
	{

	}

	public virtual void setFBAPivotCameraTransformActiveState (bool state)
	{

	}

	public virtual void startOrStopUpdatePivotPositionOnFBA (bool state)
	{

	}

	public virtual void checkActivateOrDeactivateHeadColliderOnFBA (bool state)
	{

	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnAnimatorIKComponent : MonoBehaviour
{
	public bool updateIKEnabled = true;

	public virtual void updateOnAnimatorIKState ()
	{
		if (!updateIKEnabled) {
			return;
		}
	}

	public void setUpdateIKEnabledState (bool state)
	{
		updateIKEnabled = state;
	}

	public virtual void setActiveState (bool state)
	{

	}

	public virtual void enableBothHands ()
	{
		enableOrDisableRightOrLeftHand (true, false, true);
	}

	public virtual void enableOnlyLeftHand ()
	{
		enableOrDisableRightOrLeftHand (true, false, false);
	}

	public virtual void enableOnlyRightHand ()
	{
		enableOrDisableRightOrLeftHand (true, true, false);
	}

	public virtual void enableOrDisableRightOrLeftHand (bool state, bool isRightHand, bool setStateOnBothHands)
	{

	}
}

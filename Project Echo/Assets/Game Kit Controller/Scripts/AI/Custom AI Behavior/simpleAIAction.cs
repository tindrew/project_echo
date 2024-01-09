using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class simpleAIAction : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool actionEnabled = true;

	public bool actionUsedByPlayer;

	public float actionDuration;

	public bool useUpdateSystemCoroutine;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool actionActive;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public bool useEventOnStartAction;
	public UnityEvent eventOnStartAction;

	public bool useEventOnEndAction;
	public UnityEvent eventOnEndAction;

	[HideInInspector] public Coroutine actionCoroutine;


	public virtual void startAIAction ()
	{
		if (actionEnabled) {
			activateCoroutine ();

			actionActive = true;

			if (useEventOnStartAction) {
				eventOnStartAction.Invoke ();
			}

			if (showDebugPrint) {
				print ("startAIAction");
			}
		}
	}

	public virtual void endAIAction ()
	{
		if (actionEnabled) {
			if (actionActive) {
				if (useEventOnEndAction) {
					eventOnEndAction.Invoke ();
				}

				actionActive = false;

				stopActionCoroutine ();

				resetStatesOnActionEnd ();

				if (showDebugPrint) {
					print ("endAIAction");
				}
			}
		}
	}

	public virtual void stopActionCoroutine ()
	{
		if (actionCoroutine != null) {
			StopCoroutine (actionCoroutine);
		}
	}

	public virtual void activateCoroutine ()
	{
		if (useUpdateSystemCoroutine) {
			actionCoroutine = StartCoroutine (updateSystemCoroutine ());
		} else {
			actionCoroutine = StartCoroutine (updateActionCoroutine ());
		}
	}

	IEnumerator updateActionCoroutine ()
	{
		WaitForSeconds delay = new WaitForSeconds (actionDuration);

		yield return delay;

		endAIAction ();
	}

	IEnumerator updateSystemCoroutine ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			updateSystem ();

			yield return waitTime;
		}
	}

	public virtual void updateSystem ()
	{

	}

	public virtual void resetStatesOnActionEnd ()
	{

	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UIMouseHoverEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[Header ("Main Settings")]
	[Space]

	public bool eventOnTriggerEnterEnabled = true;
	public bool eventOnTriggerExitEnabled = true;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public UnityEvent eventOnTriggerEnter;

	[Space]

	public UnityEvent eventOnTriggerExit;

	//Detect if the Cursor starts to pass over the GameObject
	public void OnPointerEnter (PointerEventData pointerEventData)
	{
		if (eventOnTriggerEnterEnabled) {
			eventOnTriggerEnter.Invoke ();
		}
	}

	//Detect when Cursor leaves the GameObject
	public void OnPointerExit (PointerEventData pointerEventData)
	{
		if (eventOnTriggerExitEnabled) {
			eventOnTriggerExit.Invoke ();
		}
	}
}

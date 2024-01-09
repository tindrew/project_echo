using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class remoteEventSystem : MonoBehaviour
{
	public bool remoteEventsEnabled = true;

	public bool activateRemoteEventsOnStart;

	public List<string> remoteEventsOnStartList = new List<string> ();

	public List<eventInfo> eventInfoList = new List<eventInfo> ();

	public bool showDebugPrint;


	public List<string> objectSearchResultList = new List<string> ();

	public string objectSearcherName = "";

	public bool searchObjectsActive;


	eventInfo currentEventInfo;

	int eventInfoListCount;


	void Start ()
	{
		if (activateRemoteEventsOnStart) {

			StartCoroutine (checkRemoteEventsOnStartListCoroutine ());
		}
	}

	IEnumerator checkRemoteEventsOnStartListCoroutine ()
	{
		yield return new WaitForEndOfFrame ();


		checkRemoteEventsOnStartList ();
	}

	void checkRemoteEventsOnStartList ()
	{
		int remoteEventsOnStartListCount = remoteEventsOnStartList.Count;

		for (int i = 0; i < remoteEventsOnStartListCount; i++) {
			callRemoteEvent (remoteEventsOnStartList [i]);
		}
	}

	public void callRemoveEvent (string eventName)
	{
		callRemoteEvent (eventName);
	}

	public void callRemoteEvent (string eventName)
	{
		if (!remoteEventsEnabled) {
			return;
		}

		if (eventInfoListCount == 0) {
			eventInfoListCount = eventInfoList.Count;
		}

		for (int i = 0; i < eventInfoListCount; i++) {
			currentEventInfo = eventInfoList [i];

			if (currentEventInfo.eventEnabled) {
				if (currentEventInfo.Name.Equals (eventName)) {
					if (showDebugPrint) {
						print (eventName);
					}

					if (currentEventInfo.useRegularEvent) {
						currentEventInfo.eventToActive.Invoke ();
					}

					if (currentEventInfo.disableEventAfterActivation) {
						currentEventInfo.eventEnabled = false;
					}

					return;
				}
			}
		}
	}

	public void callRemoteEventWithAmount (string eventName, float amount)
	{
		if (!remoteEventsEnabled) {
			return;
		}

		if (eventInfoListCount == 0) {
			eventInfoListCount = eventInfoList.Count;
		}

		for (int i = 0; i < eventInfoListCount; i++) {
			currentEventInfo = eventInfoList [i];

			if (currentEventInfo.eventEnabled) {
				if (currentEventInfo.Name.Equals (eventName)) {
					if (currentEventInfo.useAmountOnEvent) {
						currentEventInfo.eventToActiveAmount.Invoke (amount);
					}

					if (currentEventInfo.disableEventAfterActivation) {
						currentEventInfo.eventEnabled = false;
					}

					if (showDebugPrint) {
						print (eventName);
					}

					return;
				}
			}
		}
	}

	public void callRemoteEventWithBool (string eventName, bool state)
	{
		if (!remoteEventsEnabled) {
			return;
		}

		if (eventInfoListCount == 0) {
			eventInfoListCount = eventInfoList.Count;
		}

		for (int i = 0; i < eventInfoListCount; i++) {
			currentEventInfo = eventInfoList [i];

			if (currentEventInfo.eventEnabled) {
				if (currentEventInfo.Name.Equals (eventName)) {
					if (currentEventInfo.useBoolOnEvent) {
						currentEventInfo.eventToActiveBool.Invoke (state);
					}

					if (currentEventInfo.disableEventAfterActivation) {
						currentEventInfo.eventEnabled = false;
					}

					if (showDebugPrint) {
						print (eventName);
					}

					return;
				}
			}
		}
	}

	public void callRemoteEventWithGameObject (string eventName, GameObject objectToSend)
	{
		if (!remoteEventsEnabled) {
			return;
		}

		if (eventInfoListCount == 0) {
			eventInfoListCount = eventInfoList.Count;
		}

		for (int i = 0; i < eventInfoListCount; i++) {
			currentEventInfo = eventInfoList [i];

			if (currentEventInfo.eventEnabled) {
				if (currentEventInfo.Name.Equals (eventName)) {
					if (currentEventInfo.useGameObjectOnEvent) {
						currentEventInfo.eventToActiveGameObject.Invoke (objectToSend);
					}

					if (currentEventInfo.disableEventAfterActivation) {
						currentEventInfo.eventEnabled = false;
					}

					if (showDebugPrint) {
						print (eventName);
					}

					return;
				}
			}
		}
	}

	public void callRemoteEventWithTransform (string eventName, Transform transformToSend)
	{
		if (!remoteEventsEnabled) {
			return;
		}

		if (eventInfoListCount == 0) {
			eventInfoListCount = eventInfoList.Count;
		}

		for (int i = 0; i < eventInfoListCount; i++) {
			currentEventInfo = eventInfoList [i];

			if (currentEventInfo.eventEnabled) {
				if (currentEventInfo.Name.Equals (eventName)) {
					if (currentEventInfo.useTransformOnEvent) {
						currentEventInfo.eventToActiveTransform.Invoke (transformToSend);
					}

					if (currentEventInfo.disableEventAfterActivation) {
						currentEventInfo.eventEnabled = false;
					}

					if (showDebugPrint) {
						print (eventName);
					}

					return;
				}
			}
		}
	}

	public void setEnabledEventState (string eventName)
	{
		setEnabledOrDisabledEventState (true, eventName);
	}

	public void setDisabledEventState (string eventName)
	{
		setEnabledOrDisabledEventState (false, eventName);
	}

	public void setEnabledOrDisabledEventState (bool state, string eventName)
	{
		if (eventInfoListCount == 0) {
			eventInfoListCount = eventInfoList.Count;
		}

		for (int i = 0; i < eventInfoListCount; i++) {
			currentEventInfo = eventInfoList [i];

			if (currentEventInfo.Name.Equals (eventName)) {
				currentEventInfo.eventEnabled = state;

				return;
			}
		}
	}

	public void clearObjectsSearcResultList ()
	{
		objectSearchResultList.Clear ();

		objectSearcherName = "";

		searchObjectsActive = false;

		int objectToSelectInfoListCount = eventInfoList.Count;

		for (int i = 0; i < objectToSelectInfoListCount; i++) {
			eventInfo currentEventInfo = eventInfoList [i];

			currentEventInfo.pauseVisibleOnEditor = false;
		}

		updateComponent ();
	}

	public void showObjectsBySearchName ()
	{
		if (objectSearcherName != null && objectSearcherName != "") {
			objectSearchResultList.Clear ();

			searchObjectsActive = true;

			string currentTextToSearch = objectSearcherName;

			if (currentTextToSearch != "") {
				currentTextToSearch = currentTextToSearch.ToLower ();

				int objectToSelectInfoListCount = eventInfoList.Count;

				for (int i = 0; i < objectToSelectInfoListCount; i++) {
					eventInfo currentEventInfo = eventInfoList [i];

					if (currentEventInfo.Name != "") { 
						string objectName = currentEventInfo.Name.ToLower ();

						if (objectName.Contains (currentTextToSearch) ||
							objectName.Equals (currentTextToSearch)) {

							if (!objectSearchResultList.Contains (currentEventInfo.Name)) {
								objectSearchResultList.Add (currentEventInfo.Name);
							}
						}
					}
				}
			}
		}
	}

	public void setEnabledEventStateFromEditor (string eventName)
	{
		setEnabledOrDisabledEventState (true, eventName);

		updateComponent ();
	}

	public void setDisabledEventStateFromEditor (string eventName)
	{
		setEnabledOrDisabledEventState (false, eventName);

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Remote Event System", gameObject);
	}

	[System.Serializable]
	public class eventInfo
	{
		public string Name;

		public bool eventEnabled = true;

		public bool disableEventAfterActivation;

		public bool useRegularEvent = true;
		public UnityEvent eventToActive;

		public bool useAmountOnEvent;
		public eventParameters.eventToCallWithAmount eventToActiveAmount;

		public bool useBoolOnEvent;
		public eventParameters.eventToCallWithBool eventToActiveBool;

		public bool useGameObjectOnEvent;
		public eventParameters.eventToCallWithGameObject eventToActiveGameObject;

		public bool useTransformOnEvent;
		public eventParameters.eventToCallWithTransform eventToActiveTransform;

		public bool pauseVisibleOnEditor;
	}
}

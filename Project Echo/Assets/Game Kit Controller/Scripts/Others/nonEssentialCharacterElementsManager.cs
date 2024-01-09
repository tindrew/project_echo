using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

public class nonEssentialCharacterElementsManager : MonoBehaviour
{
	public List<objectToSelectInfo> objectToSelectInfoList = new List<objectToSelectInfo> ();

	[TextArea (3, 15)]public string explanation = "You can remove any of the elements on this list which you " +
	                                              "don't want to use on this character, as all of them are non essential for the proper " +
	                                              "work of the character. Press R if you want to remove just certain elements of the list.\n\n" +
	                                              "Buttons usage:\n\n" +
	                                              "x-remove element of the list\n" +
	                                              "v ^-change element position on the list\n" +
	                                              "o-select object on the list\n" +
	                                              "R-destroy object\n" +
	                                              "E-enable object\n" +
	                                              "D-disable object";



	public List<string> objectSearchResultList = new List<string> ();

	public string objectSearcherName = "";

	public bool searchObjectsActive;


	public void clearObjectsSearcResultList ()
	{
		objectSearchResultList.Clear ();

		objectSearcherName = "";

		searchObjectsActive = false;
	}

	public void showObjectsBySearchName ()
	{
		if (objectSearcherName != null && objectSearcherName != "") {
			objectSearchResultList.Clear ();

			searchObjectsActive = true;

			string currentTextToSearch = objectSearcherName;

			if (currentTextToSearch != "") {
				currentTextToSearch = currentTextToSearch.ToLower ();

				int objectToSelectInfoListCount = objectToSelectInfoList.Count;

				for (int i = 0; i < objectToSelectInfoListCount; i++) {
					objectToSelectInfo currentObjectToSelectInfo = objectToSelectInfoList [i];

					if (currentObjectToSelectInfo.Name != "") { 
						string objectName = currentObjectToSelectInfo.Name.ToLower ();

						if (objectName.Contains (currentTextToSearch) ||
						    objectName.Equals (currentTextToSearch)) {

							if (!objectSearchResultList.Contains (currentObjectToSelectInfo.Name)) {
								objectSearchResultList.Add (currentObjectToSelectInfo.Name);
							}
						}
					}
				}
			}
		}
	}

	public void selectObjectByName (string objectName)
	{
		int curretIndex = objectToSelectInfoList.FindIndex (s => s.Name.Equals (objectName));

		if (curretIndex > -1) {
			selectObjectByIndex (curretIndex);
		}
	}

	public void selectObjectByIndex (int index)
	{
		objectToSelectInfo currentObjectToSelectInfo = objectToSelectInfoList [index];

		if (currentObjectToSelectInfo.objectToSelect != null) {
			GKC_Utils.setActiveGameObjectInEditor (currentObjectToSelectInfo.objectToSelect);
		}
	}

	public void removeObjectByName (string objectName)
	{
		int curretIndex = objectToSelectInfoList.FindIndex (s => s.Name.Equals (objectName));

		if (curretIndex > -1) {
			removeObjectByIndex (curretIndex);
		}
	}

	public void destroyAllObjects ()
	{
		int objectToSelectInfoListCount = objectToSelectInfoList.Count;

		for (int i = 0; i < objectToSelectInfoListCount; i++) {
			objectToSelectInfo currentObjectToSelectInfo = objectToSelectInfoList [i];

			if (currentObjectToSelectInfo.objectToSelect != null) {
				DestroyImmediate (currentObjectToSelectInfo.objectToSelect);
			}
		}

		objectToSelectInfoList.Clear ();

		updateComponent ();
	}

	public void removeObjectByIndex (int index)
	{
		objectToSelectInfo currentObjectToSelectInfo = objectToSelectInfoList [index];

		if (currentObjectToSelectInfo.objectToSelect != null) {
			DestroyImmediate (currentObjectToSelectInfo.objectToSelect);

			objectToSelectInfoList.RemoveAt (index);

			updateComponent ();
		}
	}

	public void updateObjectNames ()
	{
		int objectToSelectInfoListCount = objectToSelectInfoList.Count;

		for (int i = 0; i < objectToSelectInfoListCount; i++) {
			objectToSelectInfo currentObjectToSelectInfo = objectToSelectInfoList [i];

			if (currentObjectToSelectInfo.objectToSelect != null) {
				currentObjectToSelectInfo.Name = currentObjectToSelectInfo.objectToSelect.name;
			}
		}

		updateComponent ();
	}

	public void enableOrDisableObjectByName (string objectName, bool state)
	{
		int curretIndex = objectToSelectInfoList.FindIndex (s => s.Name.Equals (objectName));

		if (curretIndex > -1) {
			enableOrDisableObjectByIndex (curretIndex, state);
		}

		updateComponent ();
	}

	public void enableOrDisableObjectByIndex (int index, bool state)
	{
		objectToSelectInfo currentObjectToSelectInfo = objectToSelectInfoList [index];

		if (currentObjectToSelectInfo.objectToSelect != null) {
			currentObjectToSelectInfo.objectToSelect.SetActive (state);

			if (currentObjectToSelectInfo.useEventsOnEnableDisableObject) {
				if (state) {
					currentObjectToSelectInfo.eventOnEnableObject.Invoke ();
				} else {
					currentObjectToSelectInfo.eventOnDisableObject.Invoke ();
				}
			}
		}

		updateComponent ();
	}

	public void enableOrDisableAllObjects (bool state)
	{
		int objectToSelectInfoListCount = objectToSelectInfoList.Count;

		for (int i = 0; i < objectToSelectInfoListCount; i++) {
			objectToSelectInfo currentObjectToSelectInfo = objectToSelectInfoList [i];

			if (currentObjectToSelectInfo.objectToSelect != null) {
				currentObjectToSelectInfo.objectToSelect.SetActive (state);

				if (currentObjectToSelectInfo.useEventsOnEnableDisableObject) {
					if (state) {
						currentObjectToSelectInfo.eventOnEnableObject.Invoke ();
					} else {
						currentObjectToSelectInfo.eventOnDisableObject.Invoke ();
					}
				}
			}
		}

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Non essential character elements manager", gameObject);
	}

	[System.Serializable]
	public class objectToSelectInfo
	{
		public string Name;

		public GameObject objectToSelect;

		public bool useEventsOnEnableDisableObject;
		public UnityEvent eventOnEnableObject;
		public UnityEvent eventOnDisableObject;
	}
}

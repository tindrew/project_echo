﻿using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(nonEssentialCharacterElementsManager))]
public class nonEssentialCharacterElementsManagerEditor : Editor
{
	SerializedProperty objectToSelectInfoList;
	SerializedProperty explanation;

	SerializedProperty objectSearchResultList;

	SerializedProperty objectSearcherName;

	SerializedProperty searchObjectsActive;

	bool confirmDestroyAllObjects;

	nonEssentialCharacterElementsManager manager;

	bool expanded;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		objectToSelectInfoList = serializedObject.FindProperty ("objectToSelectInfoList");
		explanation = serializedObject.FindProperty ("explanation");

		objectSearchResultList = serializedObject.FindProperty ("objectSearchResultList");

		objectSearcherName = serializedObject.FindProperty ("objectSearcherName");

		searchObjectsActive = serializedObject.FindProperty ("searchObjectsActive");

		manager = (nonEssentialCharacterElementsManager)target;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Information", "window");

		EditorGUILayout.PropertyField (explanation);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Object To Select Info List", "window");
		showObjectToSelectInfoList (objectToSelectInfoList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Object Searcher", "window");
		EditorGUILayout.PropertyField (objectSearcherName, new GUIContent ("Name To Search"), false);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Search Objects By Name")) {
			manager.showObjectsBySearchName ();

			if (objectSearchResultList.arraySize > 0) {
				objectSearchResultList.isExpanded = true;
			}
		}

		if (searchObjectsActive.boolValue) {
			if (GUILayout.Button ("Clear Results")) {
				manager.clearObjectsSearcResultList ();
			}

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Object To Search Result List", "window");
			showObjectSearchResultList (objectSearchResultList);
			GUILayout.EndVertical ();
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showObjectToSelectInfoList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Elements: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Object")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Update Object Names")) {
				manager.updateObjectNames ();
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Enable All Objects")) {
				manager.enableOrDisableAllObjects (true);
			}
	
			EditorGUILayout.Space ();

			if (GUILayout.Button ("Disable All Objects")) {
				manager.enableOrDisableAllObjects (false);
			}

			EditorGUILayout.Space ();

			if (confirmDestroyAllObjects) {
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("Confirm Destroy All")) {
					manager.destroyAllObjects ();

					confirmDestroyAllObjects = false;
				}

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Cancel Destroy")) {
					confirmDestroyAllObjects = false;
				}
				GUILayout.EndHorizontal ();
			} else {
				if (GUILayout.Button ("Destroy All Objects")) {
					confirmDestroyAllObjects = true;
				}
			}

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showObjectToSelectInfoListElement (list.GetArrayElementAtIndex (i));
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();

				if (GUILayout.Button ("X")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}

				if (GUILayout.Button ("o", buttonStyle)) {
					if (!Application.isPlaying) {
						manager.selectObjectByIndex (i);

						return;
					}
				}

				if (GUILayout.Button ("R", buttonStyle)) {
					if (!Application.isPlaying) {
						manager.removeObjectByIndex (i);

						Repaint ();

						return;
					}
				}

				if (GUILayout.Button ("E", buttonStyle)) {
					if (!Application.isPlaying) {
						manager.enableOrDisableObjectByIndex (i, true);

						Repaint ();

						return;
					}
				}

				if (GUILayout.Button ("D", buttonStyle)) {
					if (!Application.isPlaying) {
						manager.enableOrDisableObjectByIndex (i, false);

						Repaint ();

						return;
					}
				}
					
				GUILayout.EndHorizontal ();
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showObjectToSelectInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectToSelect"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventsOnEnableDisableObject"));
		if (list.FindPropertyRelative ("useEventsOnEnableDisableObject").boolValue) {

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnEnableObject"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnDisableObject"));
		}
	
		GUILayout.EndVertical ();
		GUILayout.EndVertical ();
	}


	void showObjectSearchResultList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		GUILayout.Label ("Number of Results: " + list.arraySize);

		EditorGUILayout.Space ();

		for (int i = 0; i < list.arraySize; i++) {
			expanded = false;
			GUILayout.BeginHorizontal ();
			GUILayout.BeginHorizontal ("box");

			EditorGUILayout.Space ();

			if (i < list.arraySize && i >= 0) {
				EditorGUILayout.BeginVertical ();

				GUILayout.Label (list.GetArrayElementAtIndex (i).stringValue, EditorStyles.boldLabel, GUILayout.MaxWidth (200));

				EditorGUILayout.Space ();

				GUILayout.EndVertical ();
			}
			GUILayout.EndHorizontal ();
			if (expanded) {
				GUILayout.BeginVertical ();
			} else {
				GUILayout.BeginHorizontal ();
			}

			if (GUILayout.Button ("o", buttonStyle)) {
				if (!Application.isPlaying) {
					manager.selectObjectByName (list.GetArrayElementAtIndex (i).stringValue);

					return;
				}
			}

			if (GUILayout.Button ("R", buttonStyle)) {
				if (!Application.isPlaying) {
					manager.removeObjectByName (list.GetArrayElementAtIndex (i).stringValue);

					Repaint ();

					return;
				}
			}

			if (GUILayout.Button ("E", buttonStyle)) {
				if (!Application.isPlaying) {
					manager.enableOrDisableObjectByName (list.GetArrayElementAtIndex (i).stringValue, true);

					Repaint ();

					return;
				}
			}

			if (GUILayout.Button ("D", buttonStyle)) {
				if (!Application.isPlaying) {
					manager.enableOrDisableObjectByName (list.GetArrayElementAtIndex (i).stringValue, false);

					Repaint ();

					return;
				}
			}

			if (expanded) {
				GUILayout.EndVertical ();
			} else {
				GUILayout.EndHorizontal ();
			}
			GUILayout.EndHorizontal ();
		}
	}

	void showSimpleList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Elements: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""), false);
				}
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("X")) {
					list.DeleteArrayElementAtIndex (i);
					return;
				}
				GUILayout.EndHorizontal ();
				GUILayout.EndHorizontal ();
			}
		}       
	}
}
#endif
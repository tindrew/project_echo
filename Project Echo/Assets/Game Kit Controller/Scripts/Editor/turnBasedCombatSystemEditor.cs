using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(turnBasedCombatSystem))]
public class turnBasedCombatSystemEditor : Editor
{
	turnBasedCombatSystem manager;

	SerializedProperty newCharacterTeamLeader;

	SerializedProperty teamPositionDataName;

	SerializedProperty newCharacterTeamInfoList;

	SerializedProperty newTeamAlwaysSelectFirst;

	SerializedProperty useCustomCameraStateOnNewTeam;

	SerializedProperty customCameraStateOnNewTeam;

	SerializedProperty useRewardSystem;

	SerializedProperty mainRewardSystem;

	SerializedProperty turnBasedCombatTeamInfoOnSceneList;

	void OnEnable ()
	{
		newCharacterTeamLeader = serializedObject.FindProperty ("newCharacterTeamLeader");
		teamPositionDataName = serializedObject.FindProperty ("teamPositionDataName");
		newCharacterTeamInfoList = serializedObject.FindProperty ("newCharacterTeamInfoList");
		newTeamAlwaysSelectFirst = serializedObject.FindProperty ("newTeamAlwaysSelectFirst");

		useCustomCameraStateOnNewTeam = serializedObject.FindProperty ("useCustomCameraStateOnNewTeam");

		customCameraStateOnNewTeam = serializedObject.FindProperty ("customCameraStateOnNewTeam");

		useRewardSystem = serializedObject.FindProperty ("useRewardSystem");

		mainRewardSystem = serializedObject.FindProperty ("mainRewardSystem");

		turnBasedCombatTeamInfoOnSceneList = serializedObject.FindProperty ("turnBasedCombatTeamInfoOnSceneList");


		manager = (turnBasedCombatSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		EditorGUILayout.Space ();

		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Configure New Teams Settings", "window");
		EditorGUILayout.PropertyField (newCharacterTeamLeader);
		EditorGUILayout.PropertyField (teamPositionDataName);
		EditorGUILayout.PropertyField (newTeamAlwaysSelectFirst);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (useCustomCameraStateOnNewTeam);
		if (useCustomCameraStateOnNewTeam.boolValue) {
			EditorGUILayout.PropertyField (customCameraStateOnNewTeam);
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (useRewardSystem);
		if (useRewardSystem.boolValue) {
			EditorGUILayout.PropertyField (mainRewardSystem);
		}

		EditorGUILayout.Space ();

		showSimpleList (newCharacterTeamInfoList, true);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
	
	
		if (GUILayout.Button ("\n CONFIGURE NEW TEAM INFO \n")) {
			manager.configureNewTeamInfo ();

			Repaint ();
		}

		EditorGUILayout.Space ();


		if (GUILayout.Button ("\n SHOW ALL TEAMS IN SCENE \n")) {
			manager.showAllTeamsInScene ();

			Repaint ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Current Teams On Scene List", "window");
		showSimpleList (turnBasedCombatTeamInfoOnSceneList, false);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showSimpleList (SerializedProperty list, bool showAddRemoveButtons)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (showAddRemoveButtons) {
				if (GUILayout.Button ("Add")) {
					list.arraySize++;

					Repaint ();
				}
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;

				Repaint ();
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();

				if (showAddRemoveButtons) {
					if (GUILayout.Button ("x")) {
						list.DeleteArrayElementAtIndex (i);
					}
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}
}

#endif
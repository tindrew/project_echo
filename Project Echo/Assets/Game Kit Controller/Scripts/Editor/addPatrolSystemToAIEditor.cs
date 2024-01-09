using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(addPatrolSystemToAI))]
public class addPatrolSystemToAIEditor : Editor
{
	addPatrolSystemToAI manager;

	void OnEnable ()
	{
		manager = (addPatrolSystemToAI)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Add Patrol System To AI")) {
			manager.addPatrolSystem ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Assign AI Waypoint Patrol")) {
			manager.assignAIWaypointPatrol ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Enable Patrol On AI")) {
			manager.enableOrdisablePatrolOnAI (true);
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Disable Patrol On AI")) {
			manager.enableOrdisablePatrolOnAI (false);
		}

		EditorGUILayout.Space ();
	}
}
#endif
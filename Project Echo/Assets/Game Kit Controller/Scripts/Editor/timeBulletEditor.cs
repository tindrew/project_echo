using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(timeBullet))]
public class timeBulletEditor : Editor
{
	timeBullet manager;

	void OnEnable ()
	{
		manager = (timeBullet)target;
	}

	GUIStyle style = new GUIStyle ();


	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		style.fontStyle = FontStyle.Bold;
		style.fontSize = 20;
		style.alignment = TextAnchor.MiddleCenter;

		EditorGUILayout.LabelField ("EDITOR BUTTONS", style);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Toggle Bullet Time")) {
			if (Application.isPlaying) {
				manager.inputActivateBulletTime ();
			}
		}
		EditorGUILayout.Space ();
	}
}
#endif
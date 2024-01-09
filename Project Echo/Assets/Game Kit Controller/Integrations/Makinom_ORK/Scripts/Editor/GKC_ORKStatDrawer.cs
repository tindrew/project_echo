#if UNITY_EDITOR && GAME_KIT_CONTROLLER_USE_ORK
using GamingIsLove.Makinom;
using GamingIsLove.Makinom.Editor;
using GamingIsLove.ORKFramework;
using UnityEditor;
using UnityEngine;

namespace GameKitController.Integrations.ORKFramework.Editor
{
	[CustomPropertyDrawer (typeof(GKC_ORKStat))]
	public class GKC_ORKStatDrawer : PropertyDrawer
	{
		private SerializedProperty transferMode;
		private SerializedProperty gkcStat;
		private SerializedProperty orkStat;

		private playerStatsSystem mainPlayerStatsSystem;

		private readonly string[] _transferModeOptionsDisplayNames = new string[] {
			"None",
			"ORK to GKC",
			"GKC to ORK",
			"Both",
		};

		private const string MissingPlayerStatsSystemMessage = "This GameObject needs a Player Stats System component to display the GKC Stats list here.";

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight * 3;
		}

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			if (!Maki.Initialized) {
				Maki.Initialize (MakinomAssetHelper.LoadProjectAsset());
			}

			if (!ORK.Initialized) {
				ORK.Initialize ();
			}

			if (mainPlayerStatsSystem == null) {
				mainPlayerStatsSystem = ((MonoBehaviour) property.serializedObject.targetObject).gameObject.GetComponent<playerStatsSystem>();

				if (mainPlayerStatsSystem == null) {
					EditorGUI.HelpBox (position, MissingPlayerStatsSystemMessage, MessageType.Error);
					return;
				}
			}


			EditorGUI.BeginProperty (position, label, property);

			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			transferMode = property.FindPropertyRelative ("transferMode");
			gkcStat = property.FindPropertyRelative ("gkcStat");
			orkStat = property.FindPropertyRelative ("orkStat");

			var y = position.y;
			var mapModeRect = new Rect (position.x, y, position.width, EditorGUIUtility.singleLineHeight);
			y += EditorGUIUtility.singleLineHeight;
			var gkcStatRect = new Rect (position.x, y, position.width, EditorGUIUtility.singleLineHeight);
			y += EditorGUIUtility.singleLineHeight;
			var orkStatRect = new Rect (position.x, y, position.width, EditorGUIUtility.singleLineHeight);

			transferMode.intValue = EditorGUI.Popup (mapModeRect, "Transfer Mode", transferMode.intValue, _transferModeOptionsDisplayNames);
			gkcStat.intValue = EditorGUI.Popup (gkcStatRect, "GKC Stat", gkcStat.intValue, mainPlayerStatsSystem.getStatsNames().ToArray());
			orkStat.intValue = EditorGUI.Popup (orkStatRect, "ORK Stat", orkStat.intValue, ORK.StatusValues.GetNames().ToArray());

			EditorGUI.indentLevel = indent;

			EditorGUI.EndProperty ();
		}
	}
}
#endif

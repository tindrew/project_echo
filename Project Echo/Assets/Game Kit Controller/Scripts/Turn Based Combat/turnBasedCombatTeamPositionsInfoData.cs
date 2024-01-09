using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Turn Based Combat Team Positions Info Data", menuName = "GKC/Create Turn Based Combat Team Positions Info Data", order = 51)]
public class turnBasedCombatTeamPositionsInfoData : ScriptableObject
{
	public string Name;

	public int ID;

	public bool usedForPlayerTeam;

	[Space]

	public List<turnBasedCombatCharacterPositionsInfo> turnBasedCombatCharacterPositionsInfoList = new List<turnBasedCombatCharacterPositionsInfo> ();

	[System.Serializable]
	public class turnBasedCombatCharacterPositionsInfo
	{
		public Vector3 positionValue;
		public Vector3 rotationValue;
	}
}

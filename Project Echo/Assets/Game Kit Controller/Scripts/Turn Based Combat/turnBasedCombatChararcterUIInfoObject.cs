using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class turnBasedCombatChararcterUIInfoObject : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public turnBasedCombatChararcterUIInfo mainTurnBasedCombatChararcterUIInfo;

	[System.Serializable]
	public class turnBasedCombatChararcterUIInfo
	{
		public string Name;

		public int ID;

		public GameObject characterUIGameObject;

		public GameObject characterUIOwner;

		public Text characterNameText;

		public List<turnBasedCombatStatUIInfo> turnBasedCombatStatUIInfoList = new List<turnBasedCombatStatUIInfo> ();
	}

	[System.Serializable]
	public class turnBasedCombatStatUIInfo
	{
		public string Name;

		public bool statAssigned;

		public bool useStatNameText;

		public Text statNameText;

		public bool useStatAmountText;

		public Text statAmountText;

		public Slider statSlider;

		public GameObject statObject;
	}

	[System.Serializable]
	public class statNameLettersInfo
	{
		public string Name;
		public string statLetters;
	}
}

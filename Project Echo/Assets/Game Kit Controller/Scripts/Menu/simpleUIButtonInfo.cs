using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class simpleUIButtonInfo : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public string currentName;

	public int ID;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject buttonObject;

	public Text mainText;

	public RawImage mainIcon;


	public void setCurrentName (string newValue)
	{
		currentName = newValue;
	}

	public string getCurrentName ()
	{
		return currentName;
	}
}

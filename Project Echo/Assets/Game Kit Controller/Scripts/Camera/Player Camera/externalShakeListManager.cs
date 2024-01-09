using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class externalShakeListManager : MonoBehaviour
{
	public List<externalShakeInfoListElement> externalShakeInfoList = new List<externalShakeInfoListElement> ();


	private static externalShakeListManager _externalShakeListManagerInstance;

	public static externalShakeListManager Instance { get { return _externalShakeListManagerInstance; } }

	bool instanceInitialized;


	public void getComponentInstance ()
	{
		if (instanceInitialized) {
//			print ("already initialized manager");

			return;
		}

		if (_externalShakeListManagerInstance != null && _externalShakeListManagerInstance != this) {
			Destroy (this.gameObject);

			return;
		} 

		_externalShakeListManagerInstance = this;

		instanceInitialized = true;
	}

	void Awake ()
	{
		getComponentInstance ();
	}

	public void setShakeInManagerList (externalShakeInfoListElement element, int index)
	{
		externalShakeInfoList [index] = element;
	}

	public void udpateAllHeadbobShakeList ()
	{
		headBob[] headBobList = FindObjectsOfType<headBob> ();
		foreach (headBob bob in headBobList) {
			bob.updateExternalShakeInfoList (externalShakeInfoList);
		}

		print ("All head bob in the scene have been updated with the current shake list");
	}

	public void setExternalShakeStateByIndex (int index, bool isFirstPerson)
	{
		externalShakeInfoListElement newShake = externalShakeInfoList [index];

		headBob[] headBobList = FindObjectsOfType<headBob> ();

		int headBobListLength = headBobList.Length;

		for (int i = 0; i < headBobListLength; i++) {
			headBob bob = headBobList [i];

			if (isFirstPerson) {
				bob.setExternalShakeState (newShake.firstPersonDamageShake);

			} else {
				bob.setExternalShakeState (newShake.thirdPersonDamageShake);
			}
		}
	}
}
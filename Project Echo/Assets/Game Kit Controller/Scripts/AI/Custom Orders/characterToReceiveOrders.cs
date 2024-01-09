using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class characterToReceiveOrders : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool receiveOrdersEnabled = true;

	[Space]
	[Header ("Order Info Settings")]
	[Space]

	public List<characterOrderInfo> characterOrderInfoList = new List<characterOrderInfo> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool orderInProcess;


	characterOrderInfo newCharacterOrderInfo;

	Coroutine orderActionCoroutine;


	public bool containsOrderName (string orderName)
	{
		int orderIndex = characterOrderInfoList.FindIndex (a => a.Name.Equals (orderName));

		if (orderIndex > -1) {
			if (!characterOrderInfoList [orderIndex].orderEnabled) {
				return false;
			}

			return true;
		}

		return false;
	}

	public void activateOrder (string orderName)
	{
		if (showDebugPrint) {
			print ("order to Check " + orderName);
		}

		int orderIndex = characterOrderInfoList.FindIndex (a => a.Name.Equals (orderName));

		if (orderIndex > -1) {
			newCharacterOrderInfo = characterOrderInfoList [orderIndex];

			if (!newCharacterOrderInfo.orderEnabled) {
				return;
			}

			if (showDebugPrint) {
				print ("order activated " + orderName);
			}

			if (newCharacterOrderInfo.useEventOnOrderReceived) {
				newCharacterOrderInfo.eventOnOrderReceived.Invoke ();
			}

			if (newCharacterOrderInfo.useOrderActionDuration) {
				stopOrderActionCoroutine ();

				orderActionCoroutine = StartCoroutine (updateOrderActionCoroutine ());
			}
		}
	}

	void stopOrderActionCoroutine ()
	{
		if (orderActionCoroutine != null) {
			StopCoroutine (orderActionCoroutine);
		}

		orderInProcess = false;
	}

	IEnumerator updateOrderActionCoroutine ()
	{
		orderInProcess = true;

		WaitForSeconds delay = new WaitForSeconds (newCharacterOrderInfo.orderActionDuration);

		yield return delay;

		newCharacterOrderInfo.eventOnOrderActionComplete.Invoke ();

		orderInProcess = false;
	}

	public bool isOrderInProcess ()
	{
		return orderInProcess;
	}

	[System.Serializable]
	public class characterOrderInfo
	{
		public string Name;

		public bool orderEnabled = true;

		[Space]

		public bool useEventOnOrderReceived;

		public UnityEvent eventOnOrderReceived;

		[Space]

		public bool useOrderActionDuration;
		public float orderActionDuration;

		public UnityEvent eventOnOrderActionComplete;
	}
}

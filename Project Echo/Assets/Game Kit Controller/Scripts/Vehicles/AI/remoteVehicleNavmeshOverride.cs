using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class remoteVehicleNavmeshOverride : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool targetIsFriendly;

	public bool targetIsObject;

	public bool setAutoBrakeOnRemoveTargetState;

	public bool autoBrakeOnRemoveTarget;

	[Space]
	[Header ("Components")]
	[Space]

	public vehicleAINavMesh mainVehicleAINavMesh;

	public Transform targetTranform;


	public void setVehicleNavMeshTargetPosition ()
	{
		if (targetTranform == null) {
			targetTranform = transform;
		}

		mainVehicleAINavMesh.follow (targetTranform);

		mainVehicleAINavMesh.setTargetType (targetIsFriendly, targetIsObject);

		if (setAutoBrakeOnRemoveTargetState) {
			mainVehicleAINavMesh.setAutoBrakeOnRemoveTargetState (autoBrakeOnRemoveTarget);
		}
	}

	public void removeVehicleNavmeshTarget ()
	{
		if (setAutoBrakeOnRemoveTargetState) {
			mainVehicleAINavMesh.setAutoBrakeOnRemoveTargetState (autoBrakeOnRemoveTarget);
		}

		mainVehicleAINavMesh.removeTarget ();
	}
}

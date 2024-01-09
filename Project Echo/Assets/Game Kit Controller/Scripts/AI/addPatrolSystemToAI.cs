using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class addPatrolSystemToAI : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool addUniversalWaypointPrefab;

	[Space]
	[Header ("Components")]
	[Space]

	public AIPatrolSystem mainAIPatrolSystem;

	public GameObject AIWaypointPatrolPrefab;

	public GameObject AIWaypointPatrolUniversalPrefab;

	public AIWayPointPatrol mainAIWaypointPatrol;


	public void enableOrdisablePatrolOnAI (bool state)
	{
		if (mainAIPatrolSystem == null) {
			playerComponentsManager currentPlayerComponentsManager = GetComponentInChildren<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
			

				findObjectivesSystem currentFindObjectivesSystem = currentPlayerComponentsManager.getFindObjectivesSystem ();

				if (currentFindObjectivesSystem != null) {
					mainAIPatrolSystem = currentFindObjectivesSystem.AIPatrolManager;
				}
			}
		}

		if (mainAIPatrolSystem != null) {
			mainAIPatrolSystem.pauseOrPlayPatrol (!state);

			mainAIPatrolSystem.gameObject.SetActive (state);

			GKC_Utils.updateComponent (mainAIPatrolSystem);

			updateComponent ();
		}
	}

	public void assignAIWaypointPatrol ()
	{
		if (mainAIWaypointPatrol == null) {
			mainAIWaypointPatrol = FindObjectOfType<AIWayPointPatrol> ();
		}

		if (mainAIWaypointPatrol == null) {
			GameObject newAIWaypointPatrol = (GameObject)Instantiate (AIWaypointPatrolUniversalPrefab, transform.position + Vector3.forward * 6, Quaternion.identity);
			newAIWaypointPatrol.name = "AI Waypoint Patrol";

			mainAIWaypointPatrol = newAIWaypointPatrol.GetComponent<AIWayPointPatrol> ();
		}

		if (mainAIWaypointPatrol != null) {
			playerComponentsManager currentPlayerComponentsManager = GetComponentInChildren<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {

				AIPatrolSystem currentAIPatrolSystem = currentPlayerComponentsManager.getAIPatrolSystem ();

				findObjectivesSystem currentFindObjectivesSystem = currentPlayerComponentsManager.getFindObjectivesSystem ();

				if (currentAIPatrolSystem != null) {
					currentAIPatrolSystem.patrolPath = mainAIWaypointPatrol;

					currentAIPatrolSystem.pauseOrPlayPatrol (false);

					currentFindObjectivesSystem.AIPatrolManager = currentAIPatrolSystem;

					GKC_Utils.updateComponent (currentAIPatrolSystem);

					GKC_Utils.updateComponent (currentFindObjectivesSystem);

					updateComponent ();
				}
			}
		}
	}

	public void addPatrolSystem ()
	{
		if (AIWaypointPatrolPrefab == null) {
			print ("Patrol prefab not configured");

			return;
		}

		findObjectivesSystem currentFindObjectivesSystem = GetComponentInChildren<findObjectivesSystem> ();

		if (currentFindObjectivesSystem != null && currentFindObjectivesSystem.AIPatrolManager == null) {

			GameObject patrolToInstantiate = AIWaypointPatrolPrefab;

			if (addUniversalWaypointPrefab) {
				patrolToInstantiate = AIWaypointPatrolUniversalPrefab;
			}

			GameObject newAIWaypointPatrol = (GameObject)Instantiate (patrolToInstantiate, currentFindObjectivesSystem.transform.position + Vector3.forward * 6, Quaternion.identity);
			newAIWaypointPatrol.name = "AI Waypoint Patrol";

			if (addUniversalWaypointPrefab) {
				playerComponentsManager currentPlayerComponentsManager = GetComponentInChildren<playerComponentsManager> ();

				if (currentPlayerComponentsManager != null) {

					AIPatrolSystem currentAIPatrolSystem = currentPlayerComponentsManager.getAIPatrolSystem ();

					if (currentAIPatrolSystem != null) {
						currentAIPatrolSystem.patrolPath = newAIWaypointPatrol.GetComponent<AIWayPointPatrol> ();

						currentAIPatrolSystem.pauseOrPlayPatrol (false);

						currentFindObjectivesSystem.AIPatrolManager = currentAIPatrolSystem;

						GKC_Utils.updateComponent (currentAIPatrolSystem);
					}
				}
			} else {
				AIPatrolSystem currentAIPatrolSystem = newAIWaypointPatrol.GetComponent<AIPatrolSystem> ();

				currentAIPatrolSystem.AICharacter = currentFindObjectivesSystem.transform;

				currentAIPatrolSystem.pauseOrPlayPatrol (false);

				AINavMesh mainAINavMesh = GetComponentInChildren<AINavMesh> ();

				currentAIPatrolSystem.mainAINavmesh = mainAINavMesh;

				currentFindObjectivesSystem.AIPatrolManager = currentAIPatrolSystem;

				GKC_Utils.updateComponent (currentAIPatrolSystem);
			}

			GKC_Utils.updateComponent (currentFindObjectivesSystem);

			updateComponent ();

			print ("Patrol system added to AI");
		} else {
			print ("WARNING: patrol system already configured on this AI");
		}
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Adding patrol to AI", gameObject);
	}
}

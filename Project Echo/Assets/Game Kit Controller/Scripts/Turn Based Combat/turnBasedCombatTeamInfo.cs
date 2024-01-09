using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class turnBasedCombatTeamInfo : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool isTeamLeader;

	public bool useTeamPositionDataName;

	public string teamPositionDataName;

	public bool thisTeamAlwaysSelectFirst;

	public bool useCustomCameraState;

	public string customCameraState;

	[Space]
	[Header ("Character Team List Settings")]
	[Space]

	public List<GameObject> characterTeamList = new List<GameObject> ();

	[Space]
	[Header ("Rewads Settings")]
	[Space]

	public bool useEventOnTeamDefeated;
	public UnityEvent eventOnTeamDefeated;

	[Space]

	public bool useEventOnRewardForOpponentTeam;
	public eventParameters.eventToCallWithGameObject eventOnRewardForOpponentTeam;

	[Space]

	public bool useRewardSystem;

	public objectExperienceSystem mainRewardSystem;


	public bool isTeamLeaderValue ()
	{
		return isTeamLeader;
	}

	public void removeDeadCharactersFromTeam ()
	{
		for (int i = characterTeamList.Count - 1; i >= 0; i--) {
			if (characterTeamList [i] == null || applyDamage.checkIfDead (characterTeamList [i])) {
				characterTeamList.RemoveAt (i);
			}
		}
	}

	public void clearCharacterTeamList ()
	{
		characterTeamList.Clear ();
	}

	public List<GameObject> getCharacterTeamList ()
	{
		return characterTeamList;
	}

	public void setCharacterTeamList (List<GameObject> newList)
	{
		characterTeamList = newList;
	}

	public void addCharacterToTeamList (GameObject newCharacter)
	{
		if (!characterTeamList.Contains (newCharacter)) {
			characterTeamList.Add (newCharacter);
		}
	}

	public void removeCharacterToTeamList (GameObject newCharacter)
	{
		if (characterTeamList.Contains (newCharacter)) {
			characterTeamList.Remove (newCharacter);
		}
	}

	public void checkEventsOnTeamDefeated ()
	{
		if (useEventOnTeamDefeated) {
			eventOnTeamDefeated.Invoke ();
		}
	}

	public void checkEventOnRewardForOpponentTeam (List<GameObject> opponentTeamList)
	{
		if (useEventOnRewardForOpponentTeam) {
			if (opponentTeamList.Count > 0) {
				for (int i = 0; i < opponentTeamList.Count; i++) {
					eventOnRewardForOpponentTeam.Invoke (opponentTeamList [i]);

					if (useRewardSystem) {
						if (mainRewardSystem != null) {
							mainRewardSystem.sendExperienceToAttacker (opponentTeamList [i]);
						}
					}
				}
			}
		}
	}
}

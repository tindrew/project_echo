using System.Collections;
using System.Collections.Generic;
using GameKitController.Audio;
using UnityEngine;

[RequireComponent (typeof(AudioElementHolder))]
public class weaponShellSystem : MonoBehaviour
{
	[Header ("Main Setting")]
	[Space]

	public bool addRandomRotationToShells = true;
	public Vector2 randomRotationXRange = new Vector2 (-20, 20);
	public Vector2 randomRotationYRange = new Vector2 (-20, 20);
	public Vector2 randomRotationZRange = new Vector2 (-20, 20);

	public float randomRotationMultiplier;

	[Space]
	[Header ("Components")]
	[Space]

	public Rigidbody mainRigidbody;
	public Collider mainCollider;
	public AudioSource mainAudioSource;
	public AudioElementHolder audioElementHolder;

	bool InitializeAudioElementsAssigned;


	private void Awake ()
	{
		InitializeAudioElements ();
	}

	private void InitializeAudioElements ()
	{
		if (InitializeAudioElementsAssigned) {
			return;
		}

		if (audioElementHolder == null) {
			audioElementHolder = GetComponent<AudioElementHolder> ();
		}

		if (mainAudioSource != null) {
			if (audioElementHolder != null) {
				audioElementHolder.audioElement.audioSource = mainAudioSource;
			}
		}

		InitializeAudioElementsAssigned = true;
	}

	public void setExtraColliderToIgnore (Collider extraCollider)
	{
		if (extraCollider != null) {
			Physics.IgnoreCollision (extraCollider, mainCollider, true);
		}
	}

	public void setShellValues (Vector3 forceDirection, Collider playerCollider, AudioElement clipToUse)
	{
		mainRigidbody.velocity = Vector3.zero;

		mainRigidbody.AddForce (forceDirection);

		if (addRandomRotationToShells) {
			float randomRotationX = Random.Range (randomRotationXRange.x, randomRotationXRange.y);
			float randomRotationY = Random.Range (randomRotationYRange.x, randomRotationYRange.y);
			float randomRotationZ = Random.Range (randomRotationZRange.x, randomRotationZRange.y);
			Vector3 randomRotation = new Vector3 (randomRotationX, randomRotationY, randomRotationZ);

			mainRigidbody.AddTorque (randomRotationMultiplier * randomRotation);
		}

		if (playerCollider != null) {
			Physics.IgnoreCollision (playerCollider, mainCollider, true);
		}

		if (clipToUse != null) {
			if (audioElementHolder != null) {
				audioElementHolder.audioElement = clipToUse;

				if (mainAudioSource != null) {
					audioElementHolder.audioElement.audioSource = mainAudioSource;
				}
			}
		}
	}
}

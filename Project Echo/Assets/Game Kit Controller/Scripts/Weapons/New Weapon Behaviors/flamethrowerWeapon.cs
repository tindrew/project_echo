using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameKitController.Audio;
using UnityEngine.Events;

public class flamethrowerWeapon : MonoBehaviour
{
	[Space]
	[Header ("Main Settings")]
	[Space]

	public float useEnergyRate;
	public int amountEnergyUsed;

	[Space]
	[Header ("Sound Settings")]
	[Space]

	public AudioClip soundEffect;
	public AudioElement soundEffectAudioElement;
	public float playSoundRate;
	public float minDelayToPlaySound;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool weaponEnabled;

	public bool reloading;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventsOnWeaponStateChange;
	public UnityEvent eventOnWeaponEnabled;
	public UnityEvent eventOnWeaponDisabled;

	[Space]
	[Header ("Components")]
	[Space]

	public bool configuredOnWeapon = true;
	public playerWeaponSystem weaponManager;

	public ParticleSystem mainParticleSystem;

	public AudioSource mainAudioSource;

	float lastTimeUsed;
	float lastTimeSoundPlayed;

	bool initialSoundWaitChecked;

	Coroutine updateCoroutine;


	private void Start ()
	{
		if (soundEffect != null) {
			soundEffectAudioElement.clip = soundEffect;
		}

		if (mainAudioSource != null) {
			soundEffectAudioElement.audioSource = mainAudioSource;
		}
	}

	public void stopUpdateCoroutine ()
	{
		if (updateCoroutine != null) {
			StopCoroutine (updateCoroutine);
		}
	}

	IEnumerator updateSystemCoroutine ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			updateSystem ();

			yield return waitTime;
		}
	}

	void updateSystem ()
	{
		if (configuredOnWeapon) {
			if (reloading) {
				if (weaponManager.remainAmmoInClip () && weaponManager.carryingWeapon () && !weaponManager.isWeaponReloading ()) {
					reloading = false;
				} else {
					return;
				}
			}

			if (!weaponEnabled) {
				return;
			}

			if (Time.time > lastTimeUsed + useEnergyRate) {
				if (weaponManager.remainAmmoInClip () && !weaponManager.isWeaponReloading ()) {
					lastTimeUsed = Time.time;

					weaponManager.useAmmo (amountEnergyUsed);

					weaponManager.checkToUpdateInventoryWeaponAmmoTextFromWeaponSystem ();
				}

				if (!weaponManager.remainAmmoInClip () || weaponManager.isWeaponReloading ()) {
					setWeaponState (false);

					reloading = true;

					return;
				}				
			}
		}

		if (weaponEnabled) {
			if (Time.time > lastTimeSoundPlayed + playSoundRate) {
				if (initialSoundWaitChecked || Time.time > lastTimeSoundPlayed + minDelayToPlaySound) {
					lastTimeSoundPlayed = Time.time;

					playWeaponSoundEffect ();

					initialSoundWaitChecked = true;
				}
			}
		}
	}

	public void enableWeapon ()
	{
		setWeaponState (true);
	}

	public void disableWeapon ()
	{
		setWeaponState (false);
	}

	public void setWeaponState (bool state)
	{
		if (reloading) {
			weaponEnabled = false;

			return;
		}

		initializeComponents ();

		if (weaponEnabled == state) {
			return;
		}

		weaponEnabled = state;

		if (mainParticleSystem != null) {
			if (weaponEnabled) {
				mainParticleSystem.Play ();
			} else {
				mainParticleSystem.Stop ();
			}
		}

		checkEventsOnStateChange (weaponEnabled);

		initialSoundWaitChecked = false;

		lastTimeSoundPlayed = 0;

		if (!weaponEnabled) {
			stopWeaponSoundEffect ();
		}

		if (weaponEnabled) {
			updateCoroutine = StartCoroutine (updateSystemCoroutine ());
		} else {
			stopUpdateCoroutine ();
		}
	}

	void playWeaponSoundEffect ()
	{
		if (soundEffectAudioElement != null) {
			AudioPlayer.PlayOneShot (soundEffectAudioElement, gameObject);
		}
	}

	void stopWeaponSoundEffect ()
	{
		if (soundEffectAudioElement != null) {
			AudioPlayer.Stop (soundEffectAudioElement, gameObject);
		}
	}

	void checkEventsOnStateChange (bool state)
	{
		if (useEventsOnWeaponStateChange) {
			if (state) {
				eventOnWeaponEnabled.Invoke ();
			} else {
				eventOnWeaponDisabled.Invoke ();
			}
		}
	}

	bool componentsInitialized;

	void initializeComponents ()
	{
		if (componentsInitialized) {
			return;
		}
			
		setObjectParentSystem mainSetObjectParentSystem = GetComponent<setObjectParentSystem> ();

		if (mainSetObjectParentSystem != null) {
			if (mainSetObjectParentSystem.getParentTransform () == null) {
				if (weaponManager != null) {
					playerWeaponsManager mainPlayerWeaponsManager = weaponManager.getPlayerWeaponsManger ();

					GameObject playerControllerGameObject = mainPlayerWeaponsManager.gameObject;

					playerComponentsManager mainPlayerComponentsManager = playerControllerGameObject.GetComponent<playerComponentsManager> ();

					if (mainPlayerComponentsManager != null) {
						playerController mainPlayerController = mainPlayerComponentsManager.getPlayerController ();

						if (mainPlayerController != null) {
							GameObject playerParentGameObject = mainPlayerController.getPlayerManagersParentGameObject ();

							if (playerParentGameObject != null) {
								mainSetObjectParentSystem.setParentTransform (playerParentGameObject.transform);
							}
						}
					}
				}
			}
		}

		componentsInitialized = true;
	}
}
#if GAME_KIT_CONTROLLER_USE_MAKINOM
using UnityEngine;

namespace GameKitController.Integrations.Makinom
{
	public class PausePlayerControllerAndCamera_ControlBehaviour : MonoBehaviour
	{
		private pauseOrResumePlayerControllerAndCameraSystem _pauseResumeController;
		private bool _isQuitting;

		private void Awake()
		{
			_pauseResumeController = GetComponent<pauseOrResumePlayerControllerAndCameraSystem>();
		}

		private void OnEnable()
		{
			if (_pauseResumeController == null)
				return;

			_pauseResumeController.pauseOrPlayPlayerComponents(false);
		}

		private void OnDisable()
		{
			if (_isQuitting)
				return;

			if (_pauseResumeController == null)
				return;

			_pauseResumeController.pauseOrPlayPlayerComponents(true);
		}

		private void OnApplicationQuit()
		{
			_isQuitting = true;
		}
	}
}
#endif

#if GAME_KIT_CONTROLLER_USE_MAKINOM
using GamingIsLove.Makinom;
using UnityEngine;

namespace GameKitController.Integrations.Makinom
{
	public class PauseGameHandlerMakinom : MonoBehaviour
	{
		public void PauseGame()
		{
			Maki.Game.PauseGame(true, true, false);
		}

		public void ResumeGame()
		{
			Maki.Game.PauseGame(false, true, false);
		}
	}
}
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleShowHideMouseCursor : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool showOrHideMouseCursorEnabled = true;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showMouseCursorActive;

	[Space]
	[Header ("Components")]
	[Space]

	public menuPause pauseManager;


	public void showMouseCursor ()
	{
		showOrHideMouseCursor (true);
	}

	public void hideMouseCursor ()
	{
		showOrHideMouseCursor (false);
	}

	public void showOrHideMouseCursor (bool state)
	{
		if (showOrHideMouseCursorEnabled) {
			if (showMouseCursorActive == state) {
				return;
			}

			if (pauseManager == null) {
				return;
			}

			showMouseCursorActive = state;

			pauseManager.showOrHideCursor (showMouseCursorActive);
			pauseManager.usingDeviceState (showMouseCursorActive);
			pauseManager.usingSubMenuState (showMouseCursorActive);
		}
	}
}

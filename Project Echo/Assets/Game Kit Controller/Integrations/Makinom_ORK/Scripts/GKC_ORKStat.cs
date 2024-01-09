#if GAME_KIT_CONTROLLER_USE_ORK
using System;

namespace GameKitController.Integrations.ORKFramework
{
	[Serializable]
	public class GKC_ORKStat
	{
		public GKC_ORKTransferMode transferMode = GKC_ORKTransferMode.OrkToGkc;
		public int gkcStat;
		public int orkStat;
	}
}
#endif

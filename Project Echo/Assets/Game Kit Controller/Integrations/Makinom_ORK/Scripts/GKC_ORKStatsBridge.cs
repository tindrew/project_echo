#if GAME_KIT_CONTROLLER_USE_ORK
using System.Collections;
using System.Collections.Generic;
using GamingIsLove.ORKFramework;
using UnityEngine;

namespace GameKitController.Integrations.ORKFramework
{
	public class GKC_ORKStatsBridge : MonoBehaviour
	{
		public playerStatsSystem mainPlayerStatsSystem;
		public externalStatsManager externalStatsManager;
		public GameObject combatantGameObject;

		[Space]
		public List<GKC_ORKStat> GKC_ORKStats;

		private Combatant _combatant;

		private void OnEnable ()
		{
			StartCoroutine (OnEnableCoroutine ());
		}

		private IEnumerator OnEnableCoroutine ()
		{
			while (_combatant == null) {
				_combatant = ORKComponentHelper.GetCombatant(combatantGameObject);
				yield return null;
			}

			RegisterOrkStatusValuesEventHandlers ();
			RegisterGkcStatusValuesEventHandlers ();
		}

		private void OnDisable ()
		{
			GetCombatantIfNull ();

			if (_combatant == null) {
				return;
			}

			UnregisterOrkStatusValuesEventHandlers ();
			UnregisterGkcStatusValuesEventHandlers ();
		}

		private void GetCombatantIfNull ()
		{
			if (_combatant == null) {
				_combatant = ORKComponentHelper.GetCombatant(combatantGameObject);
			}
		}

		public void RegisterOrkStatusValuesEventHandlers ()
		{
			foreach (var statInfo in mainPlayerStatsSystem.statInfoList) {
				if (!statInfo.useEventToSendValueOnUpdateStatExternally) {
					statInfo.useEventToSendValueOnUpdateStatExternally = true;
				}
			}

			for (var i = 0; i < ORK.StatusValues.Count; i++) {
				_combatant.Status[i].ValueChanged += OnOrkStatusValueChanged;
			}
		}

		public void UnregisterOrkStatusValuesEventHandlers ()
		{
			for (var i = 0; i < ORK.StatusValues.Count; i++) {
				_combatant.Status[i].ValueChanged -= OnOrkStatusValueChanged;
			}
		}

		public void RegisterGkcStatusValuesEventHandlers ()
		{
			foreach (var statInfo in mainPlayerStatsSystem.statInfoList) {
				if (!statInfo.useEventToSendValueOnUpdateStat) {
					statInfo.useEventToSendValueOnUpdateStat = true;
				}

				statInfo.eventToSendValueOnUpdateStat.AddListener(OnGkcStatusValueChanged);
			}
		}

		public void UnregisterGkcStatusValuesEventHandlers ()
		{
			foreach (var statInfo in mainPlayerStatsSystem.statInfoList) {
				statInfo.eventToSendValueOnUpdateStat.RemoveListener(OnGkcStatusValueChanged);
			}
		}

		public void OnOrkStatusValueChanged (Combatant combatant, StatusValue statusValue, int change)
		{
			foreach (var gkcOrkStat in GKC_ORKStats) {
				if (gkcOrkStat.transferMode != GKC_ORKTransferMode.OrkToGkc && gkcOrkStat.transferMode != GKC_ORKTransferMode.Both) {
					return;
				}

				if (gkcOrkStat.orkStat == statusValue.ID) {
					if (AreGkcOrkStatValuesEqual (gkcOrkStat)) {
						return;
					}

					var newValue = _combatant.Status[gkcOrkStat.orkStat].GetValue ();
					externalStatsManager.updateStatValueExternally (gkcOrkStat.gkcStat, newValue);
					return;
				}
			}
		}

		public void OnGkcStatusValueChanged (int statId, float newValue)
		{
			foreach (var gkcOrkStat in GKC_ORKStats) {
				if (gkcOrkStat.transferMode != GKC_ORKTransferMode.GkcToOrk && gkcOrkStat.transferMode != GKC_ORKTransferMode.Both) {
					return;
				}

				if (gkcOrkStat.gkcStat == statId) {
					if (AreGkcOrkStatValuesEqual (gkcOrkStat)) {
						return;
					}

					_combatant.Status[gkcOrkStat.orkStat].SetValue ((int) newValue, false, true, true, false, false, null);
					return;
				}
			}
		}

		public bool AreGkcOrkStatValuesEqual (GKC_ORKStat gkcOrkStat)
		{
			if (_combatant.Status[gkcOrkStat.orkStat].GetValue () ==
				(int) mainPlayerStatsSystem.getStatValueByIndex (gkcOrkStat.gkcStat)) {
				return true;
			}

			return false;
		}
	}
}
#endif

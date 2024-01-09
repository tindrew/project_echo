using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mapZoneUnlocker : MonoBehaviour
{
	public List<buildingInfo> buildingList = new List<buildingInfo> ();

	public float initialIndex = 0;
	public float finalIndex = 0;

	public string mainMapCreatorManagerName = "Map Creator";

	mapCreator mapCreatorManager;


	public void checkGetMapCreatorManager ()
	{
		bool mapCreatorManagerAssigned = (mapCreatorManager != null);

		if (!mapCreatorManagerAssigned) {
			mapCreatorManager = mapCreator.Instance;

			mapCreatorManagerAssigned = mapCreatorManager != null;
		}

		if (!mapCreatorManagerAssigned) {
			GKC_Utils.instantiateMainManagerOnSceneWithTypeOnApplicationPlaying (mainMapCreatorManagerName, typeof(mapCreator), true);

			mapCreatorManager = mapCreator.Instance;

			mapCreatorManagerAssigned = (mapCreatorManager != null);
		}

		if (!mapCreatorManagerAssigned) {
			mapCreatorManager = FindObjectOfType<mapCreator> ();

			mapCreatorManagerAssigned = mapCreatorManager != null;
		} 
	}

	public void unlockMapZone ()
	{
		checkGetMapCreatorManager ();

		if (mapCreatorManager != null) {
			for (int i = 0; i < buildingList.Count; i++) {
				for (int j = 0; j < buildingList [i].buildingFloorsList.Count; j++) {
					if (buildingList [i].buildingFloorsList [j].floorEnabled) {
						for (int k = 0; k < buildingList [i].buildingFloorsList [j].mapPartsList.Count; k++) {
							if (buildingList [i].buildingFloorsList [j].mapPartsList [k].mapPartEnabled) {
								mapCreatorManager.buildingList [i].buildingFloorsList [j].mapTileBuilderList [k].enableMapPart ();
							}
						}
					}
				}
			}
		}
	}

	public void enableOrDisableAllFloorParts (bool state, int buildingIndex, int floorIndex)
	{
		for (int j = 0; j < buildingList [buildingIndex].buildingFloorsList [floorIndex].mapPartsList.Count; j++) {
			buildingList [buildingIndex].buildingFloorsList [floorIndex].mapPartsList [j].mapPartEnabled = state;
		}

		updateComponent ();
	}

	public void enableOrDisableMapPartsRange (bool state, int buildingIndex, int floorIndex)
	{
		for (int j = 0; j < buildingList [buildingIndex].buildingFloorsList [floorIndex].mapPartsList.Count; j++) {
			if (j >= Mathf.Round (initialIndex) && j < Mathf.Round (finalIndex)) {
				buildingList [buildingIndex].buildingFloorsList [floorIndex].mapPartsList [j].mapPartEnabled = state;
			} else {
				buildingList [buildingIndex].buildingFloorsList [floorIndex].mapPartsList [j].mapPartEnabled = false;
			}
		}

		updateComponent ();
	}

	public void searchBuildingList ()
	{
		checkGetMapCreatorManager ();

		if (mapCreatorManager != null) {
			buildingList.Clear ();

			for (int i = 0; i < mapCreatorManager.buildingList.Count; i++) {
				buildingInfo newBuildingInfo = new buildingInfo ();

				newBuildingInfo.Name = mapCreatorManager.buildingList [i].Name;

				for (int j = 0; j < mapCreatorManager.buildingList [i].buildingFloorsList.Count; j++) {
					floorInfo newFloorInfo = new floorInfo ();

					newFloorInfo.Name = mapCreatorManager.buildingList [i].buildingFloorsList [j].Name;
					newFloorInfo.floorEnabled = mapCreatorManager.buildingList [i].buildingFloorsList [j].floorEnabled;

					newFloorInfo.mapPartsList = new List<mapPartInfo> ();

					for (int k = 0; k < mapCreatorManager.buildingList [i].buildingFloorsList [j].mapTileBuilderList.Count; k++) {
						mapTileBuilder currentMapTileBuilder = mapCreatorManager.buildingList [i].buildingFloorsList [j].mapTileBuilderList [k];

						mapPartInfo newMapPartInfo = new mapPartInfo ();

						if (currentMapTileBuilder != null) {
							newMapPartInfo.mapTileBuilderManager = currentMapTileBuilder;
							newMapPartInfo.mapPartName = currentMapTileBuilder.mapPartName;
						} else {
							print ("Warning, map tile builder component not found, make sure to use the button Set All Buildings Info or Get All Floor parts in every building " +
							"in the Map Creator inspector, too assign the elements needed to the map system");
						}

						newFloorInfo.mapPartsList.Add (newMapPartInfo);
					}

					newBuildingInfo.buildingFloorsList.Add (newFloorInfo);

				}

				buildingList.Add (newBuildingInfo);
			}
		}

		updateComponent ();
	}

	public void clearAllBuildingList ()
	{
		buildingList.Clear ();

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}

	[System.Serializable]
	public class floorInfo
	{
		public string Name;
		public bool floorEnabled;
		public List<mapPartInfo> mapPartsList = new List<mapPartInfo> ();
	}

	[System.Serializable]
	public class buildingInfo
	{
		public string Name;
		public List<floorInfo> buildingFloorsList = new List<floorInfo> ();
	}

	[System.Serializable]
	public class mapPartInfo
	{
		public string mapPartName;
		public bool mapPartEnabled;
		public mapTileBuilder mapTileBuilderManager;
	}
}

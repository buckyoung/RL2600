using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using RL2600.System;

namespace RL2600.Settings {
	public class PlaySettings : MonoBehaviour {
		private static bool[] isAI = new bool[PlayerManager.getNumberOfPlayers()];
		private static string[] carSelection = new string[PlayerManager.getNumberOfPlayers()];

		private static int ID = 1;

		private static string[] cars = {"car-donatomus", "car-scopecreep", "car-sprngr", "car-tm"};

		void Awake() {
			DontDestroyOnLoad(this);

			for (int i = 0; i < carSelection.Length; i++) {
				carSelection[i] = cars[0];
			}
		}

		public void setID(int value) {
			ID = value;
		}

		public void setIsAI(bool value) {
			isAI[ID - 1] = value;
		}

		public void setCarSelection(int value) {
			carSelection[ID - 1] = cars[value];
		}

		public static bool getIsAI(int id) {
			return isAI[id - 1];
		}

		public static string getCarSelection(int id) {
			return carSelection[id - 1];
		}
	}
}
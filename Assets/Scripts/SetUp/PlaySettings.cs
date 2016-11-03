using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using RL2600.System;

namespace RL2600.Settings {
	public class PlaySettings : MonoBehaviour {
		public const int NUM_PLAYERS = 2;

		public const int INITIAL_MINUTE = 3;
		public const float INITIAL_SECOND = 00;

		private static string[] carSelection;
		private static bool[] isAI;
		private static int ID;

		private static string[] cars = {"car-donatomus", "car-scopecreep", "car-sprngr", "car-tm", "car-experiment"};

		void Awake() {
			DontDestroyOnLoad(this);

			initializeFields();
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

		private static void initializeFields() {
			carSelection = new string[NUM_PLAYERS];
			isAI = new bool[NUM_PLAYERS];
			ID = 1;

			for (int i = 0; i < carSelection.Length; i++) {
				carSelection[i] = cars[4];
			}
		}
	}
}
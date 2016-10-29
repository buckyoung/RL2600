using UnityEngine;
using System.Collections;
using RL2600.Settings;

namespace RL2600.System {
	public class BoostManager : MonoBehaviour {
		private static float[] boosts;

		private const float SPEED_MODIFIER = 2.2f;
		private const int DECREASE_MODIFIER = 20;
		private const float START_AMT = 35;

		void Start() {
			initializeFields();
		}

		/*
		 * User functions
		 */
		public static float getBoost(int id) {
			return boosts[id - 1];
		}

		public static float getBoostModifier(int id) {
			// AI stuff
			if (PlayerManager.getIsAI(id)) {
				return aiBoost(id);
			}

			// Normal player stuff
			if (!Input.GetButton(id + "_BTN") || boosts[id - 1] <= 0.0f) {
				return 1.0f;
			}

			decrementBoost(id);

			return SPEED_MODIFIER;
		}

		// Returns true if can be picked up, false if not
		public static bool canPickupBoost(int id) {
			int index = id - 1;

			// Do not pick up if boost is full
			if (boosts[index] >= 100.0f) {
				return false;
			} 

			return true;
		}

		public static void resetBoosts() {
			for (int i = 0; i < boosts.Length; i++) {
				boosts[i] = START_AMT;
			}
		}

		public static void incrementBoost(int id, float amount) {
			int index = id - 1;

			boosts[index] += amount;

			if (boosts[index] > 100.0f) {
				boosts[index] = 100.0f;
			}
		}

		// Private 

		private static void initializeFields() {
			boosts = new float[PlaySettings.NUM_PLAYERS];
			resetBoosts();
		}

		private static void decrementBoost(int id) {
			int index = id - 1;

			boosts[index] -= Time.deltaTime * DECREASE_MODIFIER;

			if (boosts[index] < 0.0f) {
				boosts[index] = 0.0f;
			}
		}

		private static float aiBoost(int id) {
			int num = Random.Range(0, 100);

			if (num > 30 || boosts[id - 1] <= 0.0f) {
				return 1.0f;
			}

			decrementBoost(id);

			return SPEED_MODIFIER;
		}

	}
}

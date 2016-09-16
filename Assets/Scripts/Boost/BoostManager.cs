using UnityEngine;
using System.Collections;

namespace RL2600.System {
	public class BoostManager : MonoBehaviour {
		private static float[] boosts = new float[PlayerManager.getNumberOfPlayers()];

		private static float SPEED_MODIFIER = 1.8f;
		private static int DECREASE_MODIFIER = 20;
		private static float INCREASE_AMT = 35;
		private static float START_AMT = 35;

		void Start() {
			resetBoosts();
		}

		/*
		 * User functions
		 */
		public static float getBoost(int id) {
			return boosts[id - 1];
		}

		public static float getBoostModifier(int id) {
			// AI stuff
			if (PlayerManager.checkIsAI(id-1)) {
				return aiBoost(id);
			}

			// Normal player stuff
			if (!Input.GetButton(id + "_BTN") || boosts[id - 1] <= 0.0f) {
				return 1.0f;
			}

			decrementBoost(id);

			return SPEED_MODIFIER;
		}

		// Returns true if picked up, false if not
		public static bool pickupBoost(int id) {
			int index = id - 1;

			if (boosts[index] >= 100.0f) {
				return false;
			} 

			boosts[index] += INCREASE_AMT;

			if (boosts[index] > 100.0f) {
				boosts[index] = 100.0f;
			}

			return true;
		}

		public static void resetBoosts() {
			for (int i = 0; i < boosts.Length; i++) {
				boosts[i] = START_AMT;
			}
		}

		// Private 

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

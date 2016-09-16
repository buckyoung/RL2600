using UnityEngine;
using System.Collections;

namespace RL2600.System {
	public class ScoreManager : MonoBehaviour {
		private static int[] scores = new int[PlayerManager.getNumberOfPlayers()];

		/*
		 * User functions
		 */
		public static int getScore(int id) {
			return scores[id - 1];
		}

		public static void score(int id) {
			scores[id -1] += 1;

			GameManager.score();
		}
	}
}

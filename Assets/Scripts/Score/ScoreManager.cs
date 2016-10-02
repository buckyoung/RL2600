using UnityEngine;
using System.Collections;
using RL2600.Player;

namespace RL2600.System {
	public class ScoreManager : MonoBehaviour {
		private static int[] scores;

		void Start() {
			initializeFields();
		}

		public static int getScore(Team team) {
			return scores[(int)team];
		}

		public static void score(Team team) {
			scores[(int)team] += 1;

			GameManager.score();
		}

		public static bool getIsTied() {
			return scores[(int)Team.BLUE] == scores[(int)Team.RED];
		}

		public static void notifyWinner() {
			if (getIsTied()) {
				NotificationManager.notifyMidfield("DRAW!");
			} else {
				NotificationManager.notifyMidfield(getWinner() + " WINS!");
			}
		}

		private static string getWinner() {
			return scores[(int)Team.BLUE] > scores[(int)Team.RED] ? Team.BLUE.ToString() : Team.RED.ToString();
		}

		private static void initializeFields() {
			scores = new int[(int)Team.RED + 1];
		}
	}
}

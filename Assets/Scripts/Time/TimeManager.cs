using UnityEngine;
using System.Collections;
using RL2600.Settings;

namespace RL2600.System {
	public class TimeManager : MonoBehaviour {
		private static int minute;
		private static float second;
		private static bool isPaused;
		private static bool hasRegulationEnded;

		void Start() {
			initializeFields();
		}

		void Update() {
			if (GameManager.getHasGameEnded() || isPaused) { return; }

			float difference = Time.deltaTime;

			if (second - difference < 0 && minute > 0) {
				minute--;
				difference -= second;
				second = 60;
			}

			second -= difference;

			checkNotify();
			checkEndOfRegulation();
		}

		public static void pause() {
			BallManager.disableBall();
			PlayerManager.disablePlayers();
			isPaused = true;
		}

		public static void unpause() {
			BallManager.enableBall();
			PlayerManager.enablePlayers();
			isPaused = false;
		}

		// Allow ball to keep moving to support the
		// "one final hit" feature
		public static void endGamePause() {
			PlayerManager.disablePlayers();
			isPaused = true;
		}

		public static string getTime() {
			int wholeSecond = Mathf.CeilToInt(second);
			int reportMinute = minute;

			if (wholeSecond == 60) {
				wholeSecond = 0;
				reportMinute++;
			}

			return reportMinute + ":" + wholeSecond.ToString("00");
		}

		public static bool getIsPaused() {
			return isPaused;
		}

		public static bool getHasRegulationEnded() {
			return hasRegulationEnded;
		}

		// Private

		private static void checkEndOfRegulation() {
			if (minute == 0 && second <= 0) {
				minute = 0;
				second = 0;
				hasRegulationEnded = true;

				GameManager.onEndOfRegulation();
			}
		}

		private static void checkNotify() {
			if (minute != 0) { return; }

			int wholeSecond = Mathf.CeilToInt(second);

			if (wholeSecond == 60) { 
				NotificationManager.notifyMidfield("ONE MINUTE REMAINING!");
			}

			// Clear the ONE MINUTE REMAINING message
			if (wholeSecond == 58) {
				NotificationManager.clearMidfield();
			}

			if (wholeSecond == 30) {
				NotificationManager.notifyMidfield("30 SECONDS REMAINING!");
			} 

			// Clear the 30 SECONDS REMAINING message
			if (wholeSecond == 28) {
				NotificationManager.clearMidfield();
			}

			if (wholeSecond <= 10) {
				NotificationManager.notifyMidfield(wholeSecond.ToString());
			}

			if (wholeSecond == 0) {
				NotificationManager.notifyMidfield("- SUDDEN DEATH -");
			}
		}

		private static void initializeFields() {
			minute = PlaySettings.INITIAL_MINUTE;
			second = PlaySettings.INITIAL_SECOND;
			hasRegulationEnded = false;
			isPaused = false;
		}
	}
}
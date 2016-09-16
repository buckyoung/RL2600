using UnityEngine;
using System.Collections;

namespace RL2600.System {
	public class TimeManager : MonoBehaviour {
		private static int minute = 3;
		private static float second = 00;
		private static bool isPaused = false;
		private static bool hasEnded = false;

		void Update() {
			if (hasEnded || isPaused) { return; }

			float difference = Time.deltaTime;

			if (second - difference < 0 && minute > 0) {
				minute--;
				difference -= second;
				second = 60;
			}

			second -= difference;

			checkEndOfTime();
		}

		public static void pauseTime() {
			isPaused = true;
		}

		public static void unpauseTime() {
			isPaused = false;
		}

		public static string getTime() {
			return minute + ":" + Mathf.Floor(second).ToString("00");
		}

		// Private

		private static void checkEndOfTime() {
			if (minute == 0 && second <= 0) {
				minute = 0;
				second = 0;
				hasEnded = true;

				GameManager.endGame();
			}
		}
	}
}
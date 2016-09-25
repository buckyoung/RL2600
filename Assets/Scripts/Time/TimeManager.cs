﻿using UnityEngine;
using System.Collections;

namespace RL2600.System {
	public class TimeManager : MonoBehaviour {
		private static int minute = 0;
		private static float second = 03;
		private static bool isPaused = false;
		private static bool hasRegulationEnded = false;

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

		public static void pauseTime() {
			isPaused = true;
		}

		public static void unpauseTime() {
			isPaused = false;
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
	}
}
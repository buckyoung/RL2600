using UnityEngine;
using System.Collections;
using RL2600.Boost;
using RL2600.Score;
using RL2600.System;

namespace RL2600.HUD {
	public class Scoreboard : MonoBehaviour {
		public Font font;
		private GUIStyle guiStyle = new GUIStyle();

		int w = 100;
		int h = 100;

		void Start() {
			guiStyle.fontSize = 40;
			guiStyle.normal.textColor = new Vector4(0.25f, 0.25f, 0.25f, 1.0f);
			guiStyle.font = font;
		}
			
		void OnGUI() {
			drawHud();
		}

		private void drawHud() {
			int y = Screen.height - 80;

			GUI.Label(new Rect(w, y, w, h), ScoreManager.getScore(1).ToString(), guiStyle);
			GUI.Label(new Rect(w * 2, y, w, h), getBoost(1), guiStyle);

			GUI.Label(new Rect((Screen.width / 2) - (w / 2), y, w, h), TimeManager.getTime(), guiStyle);

			GUI.Label(new Rect(Screen.width - (w * 2), y, w, h), getBoost(2), guiStyle);
			GUI.Label(new Rect(Screen.width - w, y, w, h), ScoreManager.getScore(2).ToString(), guiStyle);
		}

		private string getBoost(int id) {
			return Mathf.Ceil(BoostManager.getBoost(id)).ToString("00");
		}
	}
}
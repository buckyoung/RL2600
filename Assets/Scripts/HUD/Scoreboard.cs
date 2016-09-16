using UnityEngine;
using System.Collections;
using RL2600.Boost;
using RL2600.Score;
using RL2600.System;

namespace RL2600.HUD {
	public class Scoreboard : MonoBehaviour {
		public Font font;

		private int x, y, w, h;
		private GUIStyle guiStyle = new GUIStyle();

		void Start() {
			guiStyle.fontSize = 40;
//			guiStyle.normal.textColor = new Vector4(0.8f, 0.8f, 0.8f, 1.0f); // Light gray
			guiStyle.normal.textColor = new Vector4(0.25f, 0.25f, 0.25f, 1.0f); // dark gray
			guiStyle.font = font;
		}
			
		void OnGUI() {
			setPosition();
			drawHud();
		}

		/*
		 * User Functions
		 */
		private void setPosition() {
			x = 240;
			h = 60;
			w = Screen.width - (x*2);
			y = Screen.height - h;
		}

		private void drawHud() {
			GUI.Label(new Rect(x, y, w, h), ScoreManager.getScore(1) + "        " + Mathf.Ceil(BoostManager.getBoost(1)).ToString("00") + "              " + TimeManager.getTime() + "              " + Mathf.Ceil(BoostManager.getBoost(2)).ToString("00") + "        " + ScoreManager.getScore(2), guiStyle);
		}
	}
}
using UnityEngine;
using System.Collections;
using RL2600.Boost;
using RL2600.Score;
using RL2600.System;
using RL2600.Player;

namespace RL2600.HUD {

	[ExecuteInEditMode]

	public class Scoreboard : MonoBehaviour {
		public Font font;
		private GUIStyle guiStyle = new GUIStyle();
		private GUIStyle blueStyle = new GUIStyle();
		private GUIStyle redStyle = new GUIStyle();

		int w = 60;
		int h = 50;
	
		void Start() {
			guiStyle.fontSize = 40;
			guiStyle.normal.textColor = new Vector4(0.25f, 0.25f, 0.25f, 1.0f);
			guiStyle.font = font;
			guiStyle.clipping = TextClipping.Clip;

			blueStyle.fontSize = 40;
			blueStyle.normal.textColor = new Vector4(0.25f, 0.25f, 0.5f, 1.0f);
			blueStyle.font = font;
			blueStyle.clipping = TextClipping.Clip;

			redStyle.fontSize = 40;
			redStyle.normal.textColor = new Vector4(0.5f, 0.25f, 0.25f, 1.0f);
			redStyle.font = font;
			redStyle.clipping = TextClipping.Clip;
		}

		void OnGUI() {
			drawHud();
		}

		private void drawHud() {
			int y = Screen.height - 80;

			GUI.Label(new Rect(w, y, w, h), getBoost(1), blueStyle);

			GUI.Label(new Rect((Screen.width / 2) - (w*2), y, w, h), ScoreManager.getScore(Team.BLUE).ToString(), blueStyle);
			GUI.Label(new Rect((Screen.width / 2) - (w / 2), y, w*2, h), TimeManager.getTime(), guiStyle);
			GUI.Label(new Rect((Screen.width / 2) + (w*2), y, w, h), ScoreManager.getScore(Team.RED).ToString(), redStyle);

			GUI.Label(new Rect(Screen.width - w*2, y, w, h), getBoost(2), redStyle);

		}

		private string getBoost(int id) {
			return Mathf.Ceil(BoostManager.getBoost(id)).ToString("00");
		}
	}
}

//	public Rect debugPosition = new Rect(Screen.width - 100, Screen.height - 80, 50, 100);
//	public string debugText = "000";
//	GUI.Label(debugPosition, debugText, guiStyle);
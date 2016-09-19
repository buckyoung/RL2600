using UnityEngine;
using System.Collections;

namespace RL2600.System {
	public class NotificationManager : MonoBehaviour {
		private static GUIText midfieldGUIText;

		void Awake() {
			GameObject midfield = GameObject.Find("NotifyMidfield");
			midfieldGUIText = midfield.GetComponent<GUIText>();
		}

		public static void notifyMidfield(string text) {
			midfieldGUIText.text = text;	
		}

		public static void clearMidfield() {
			midfieldGUIText.text = "";
		}
	}
}

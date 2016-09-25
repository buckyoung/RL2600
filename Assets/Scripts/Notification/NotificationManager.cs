using UnityEngine;
using System.Collections;

namespace RL2600.System {
	public class NotificationManager : MonoBehaviour {
		private static TextMesh midfieldGUIText;

		void Awake() {
			GameObject midfield = GameObject.Find("NotifyMidfield");
			midfieldGUIText = midfield.GetComponent<TextMesh>();
		}

		public static void notifyMidfield(string text) {
			midfieldGUIText.text = text;	
		}

		public static void clearMidfield() {
			midfieldGUIText.text = "";
		}
	}
}

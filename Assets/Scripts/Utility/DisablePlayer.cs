using UnityEngine;
using System.Collections;

namespace RL2600.Utility {

	public class DisablePlayer : MonoBehaviour {
		void Start () {
			Debug.LogWarning("DisablePlayer script is on GameObject: " + transform.name + "\n(This will disable a Collider2D on a child component)");
			GetComponentInChildren<Collider2D>().enabled = false;
		}
	}
}

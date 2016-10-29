using UnityEngine;
using System.Collections;
using RL2600.Boost;

namespace RL2600.System {
    public class PickupManager : MonoBehaviour {
		private static Pickup[] pickupScripts;

        void Start() {
			initializeFields();
        }

        public static void resetPickups() {
			foreach (Pickup p in pickupScripts) {
				p.resetPickup();
            }
        }

		private static void initializeFields() {
			pickupScripts = new Pickup[6];

			GameObject pickupContainer = GameObject.Find("PickupContainer");
			int i = 0;

			foreach (Transform child in pickupContainer.transform) {
				if (child != pickupContainer.transform) {
					pickupScripts[i++] = child.GetComponent<Pickup>();
				}
			}
		}
    }
}

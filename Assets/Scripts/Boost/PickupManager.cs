using UnityEngine;
using System.Collections;

namespace RL2600.System {
    public class PickupManager : MonoBehaviour {
        private static Renderer[] renderers = new Renderer[PlayerManager.getNumberOfPlayers()];

        void Start() {
            GameObject pickupContainer = GameObject.Find("PickupContainer");
            int i = 0;

            foreach (Transform child in pickupContainer.transform) {
                if (child != pickupContainer.transform) {
                    renderers[i++] = child.GetComponent<Renderer>();
                }
            }
        }

        public static void resetPickups() {
            foreach (Renderer r in renderers) {
                r.enabled = true;
            }
        }
    }
}

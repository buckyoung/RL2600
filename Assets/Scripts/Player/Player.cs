using UnityEngine;
using System.Collections;
using RL2600.Behavior;

namespace RL2600.Player {
	public class Player : MonoBehaviour {
		public IMoveable moveBehavior;
		public int id = 1;
		public bool isAI = false;

		void Start() {
			moveBehavior = GetComponentInChildren<IMoveable>();
		}

		void FixedUpdate() {
			moveBehavior.move();
		}
	}
}
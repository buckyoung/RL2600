using UnityEngine;
using System.Collections;
using RL2600.Behavior;
using RL2600.Settings;

namespace RL2600.Player {
	public class Player : MonoBehaviour {
		public int id = 1;
		public Team team = Team.BLUE;

		private IMoveable moveBehavior;

		void Awake() {
			GameObject car = (GameObject)Instantiate(Resources.Load(PlaySettings.getCarSelection(id)), this.transform.position, this.transform.rotation);
			car.transform.parent = this.transform;
		}

		void Start() {
			moveBehavior = GetComponentInChildren<IMoveable>();
		}

		void FixedUpdate() {
			if (moveBehavior == null) {
	            // Since the IMoveable component is added programatically to the
		        // vehicle (in Car.cs->Start()), there is no good time to get this compoent
                // except for during the first update cycle
                moveBehavior = GetComponentInChildren<IMoveable>();
	       	}
			
			moveBehavior.move();
		}
	}

	public enum Team {
		BLUE,
		RED
	};
}
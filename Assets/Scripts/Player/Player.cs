using UnityEngine;
using System.Collections;
using RL2600.Behavior;

namespace RL2600.Player {
	public class Player : MonoBehaviour {
		public int id = 1;
		public bool isAI = false;
		public Team team = Team.BLUE;
	}

	public enum Team {
		BLUE,
		RED
	};
}
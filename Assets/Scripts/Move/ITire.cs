using UnityEngine;
using System.Collections;

namespace RL2600.Behavior {
	public interface ITire {
		Vector2 getWorldFriction(Vector2 localVelocity);
	}
}
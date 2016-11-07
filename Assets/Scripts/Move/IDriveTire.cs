using UnityEngine;
using System.Collections;

namespace RL2600.Behavior {
	public interface IDriveTire : ITire {
		Vector2 getWorldDriveForce(float torque);
	}
}
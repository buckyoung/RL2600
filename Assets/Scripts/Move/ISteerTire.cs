using UnityEngine;
using System.Collections;

namespace RL2600.Behavior {
	public interface ISteerTire : ITire {
		void steer(float amount);
	}
}
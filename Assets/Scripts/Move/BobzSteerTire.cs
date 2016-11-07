using UnityEngine;
using System.Collections;
using RL2600.System;

namespace RL2600.Behavior {
	public class BobzSteerTire : MonoBehaviour, ISteerTire {
		public float maxYGripVelocity; // TODO
		public float normalGrip = 1.0f;
		public float slippingGrip = 0.7f;
		private float maxSteer = 36.0f; // degrees

		public float rollingResistance = 18.0f;

		public float grip;
		public Vector3 localTireRotation;
		public Vector2 totalLocalFriction;
		public Vector2 totalLocalDriveForce;

		public Vector2 getWorldFriction(Vector2 localVelocity) {
			// 1. Calculate side to side
			// if (is handbraking) { grip = handBrakeGrip} // TODO!
			if (localVelocity.y > maxYGripVelocity) {
				grip = slippingGrip;
			} else if (localVelocity.y < -maxYGripVelocity) {
				grip = -slippingGrip;
			} else if (localVelocity.y > 0) {
				grip = normalGrip;
			} else if (localVelocity.y < 0) {
				grip = -normalGrip;
			}

			totalLocalFriction.y = localVelocity.y * grip;

			// 2. Calculate front to back
			// Note: no slippage when wheel is just rolling
			totalLocalFriction.x = -localVelocity.x * rollingResistance;

			return transform.TransformDirection(totalLocalFriction);
		}

		public Vector2 getWorldDriveForce(float torque) {
			totalLocalDriveForce.x = torque;
			totalLocalDriveForce.y = 0;

			return transform.TransformDirection(totalLocalDriveForce);
		}

		public void steer(float amount) {
			localTireRotation.z = -amount * maxSteer;
			transform.localEulerAngles = localTireRotation;
		}
	}
}
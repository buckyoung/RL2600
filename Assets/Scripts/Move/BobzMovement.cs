using UnityEngine;
using System.Collections;
using RL2600.System;

namespace RL2600.Behavior {

	[RequireComponent (typeof (Rigidbody2D))]

	public class BobzMovement : MonoBehaviour, IMoveable {
		public int playerId;

		public const float engineForce = 8000;

		private Rigidbody2D rb;
		private ISteerTire frontLeftTire;
		private ISteerTire frontRightTire;
		private IDriveTire rearLeftTire;
		private IDriveTire rearRightTire;

		public float throttleInput;
		public float steeringInput;

		public Vector2 localVelocity;
		public float torque;

		public Vector2 totalLocalFriction;
		public Vector2 totalLocalDriveForce;

		void Awake() {
			playerId = GetComponentInParent<Player.Player>().id;
			rb = GetComponent<Rigidbody2D>();

			foreach(Transform t in transform) {
				Debug.Log(t.name);
				switch(t.name) {
				case "front-left":
					frontLeftTire = t.GetComponent<ISteerTire>();
					break;
				case "front-right":
					frontRightTire = t.GetComponent<ISteerTire>();
					break;
				case "back-left":
					rearLeftTire = t.GetComponent<IDriveTire>();
					break;
				case "back-right":
					rearRightTire = t.GetComponent<IDriveTire>();
					break;
				}
			}
		}

		public void move() {
			throttleInput = Input.GetAxis(playerId + "_AXIS_Y");
			steeringInput = Input.GetAxis(playerId + "_AXIS_X");

			localVelocity = transform.InverseTransformVector( rb.GetPointVelocity(transform.position) );

			torque = throttleInput * engineForce;

			// Steer
			frontLeftTire.steer(steeringInput);
			frontRightTire.steer(steeringInput);

			// Apply friction forces
//			totalLocalFriction = 
//				frontLeftTire.getLocalFriction(localVelocity)
//				+ frontRightTire.getLocalFriction(localVelocity)
//				+ rearLeftTire.getLocalFriction(localVelocity)
//				+ rearRightTire.getLocalFriction(localVelocity);
//
//			rb.AddForceAtPosition(transform.TransformDirection(-totalLocalFriction), transform.position);
			rb.AddForceAtPosition(-frontLeftTire.getWorldFriction(localVelocity), transform.position);
			rb.AddForceAtPosition(-frontRightTire.getWorldFriction(localVelocity), transform.position);
			rb.AddForceAtPosition(-rearLeftTire.getWorldFriction(localVelocity), transform.position);
			rb.AddForceAtPosition(-rearRightTire.getWorldFriction(localVelocity), transform.position);

			// Apply drive forces
//			totalLocalDriveForce =
//				frontLeftTire.getLocalDriveForce(torque)
//				+ frontRightTire.getLocalDriveForce(torque)
//				+ rearLeftTire.getLocalDriveForce(torque)
//				+ rearRightTire.getLocalDriveForce(torque);
//
//			rb.AddForceAtPosition(transform.TransformDirection(totalLocalDriveForce), transform.position);
//			rb.AddForceAtPosition(frontLeftTire.getWorldDriveForce(torque), transform.position); // TODO REMOVE ME
//			rb.AddForceAtPosition(frontRightTire.getWorldDriveForce(torque), transform.position); // TODO REMOVE ME
			rb.AddForceAtPosition(rearLeftTire.getWorldDriveForce(torque), transform.position);
			rb.AddForceAtPosition(rearRightTire.getWorldDriveForce(torque), transform.position);
		}
	}
}
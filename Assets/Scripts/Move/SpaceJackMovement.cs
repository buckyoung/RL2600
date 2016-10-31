using UnityEngine;
using System.Collections;
using RL2600.System;

// https://github.com/spacejack/carphysics2d/blob/master/public/js/Car.js

namespace RL2600.Behavior {

	[RequireComponent (typeof (Rigidbody2D))]

	public class SpaceJackMovement : MonoBehaviour, IMoveable {
		public float gravity = 9.81f;  // m/s^2
		public float inertiaScale = 1.0f;  // Multiply by mass for inertia
		public float halfWidth = 0.8f; // Centre to side of chassis (metres)
		public float cgToFront = 2.0f; // Centre of gravity to front of chassis (metres)
		public float cgToRear = 2.0f;   // Centre of gravity to rear of chassis
		public float cgToFrontAxle = 1.25f;  // Centre gravity to front axle
		public float cgToRearAxle = 1.25f;  // Centre gravity to rear axle
		public float cgHeight = 0.55f;  // Centre gravity height
		public float wheelRadius = 0.3f;  // Includes tire (also represents height of axle)
		public float wheelWidth = 0.2f;  // Used for render only
		public float tireGrip = 2.0f;  // How much grip tires have
		public float lockGrip = 0.7f;  // % of grip available when wheel is locked
		public float engineForce = 8000.0f;
		public float brakeForce = 12000.0f;
		public float eBrakeForce;
		public float weightTransfer = 0.2f;  // How much weight is transferred during acceleration/braking
		public float maxSteer = 0.6f;  // Maximum steering angle in radians
		public float cornerStiffnessFront = 5.0f;
		public float cornerStiffnessRear = 5.2f;
		public float airResist = 2.5f;	// air resistance (* vel)
		public float rollResist = 8.0f;	// rolling resistance force (* vel)
		public float inertia;
		public float wheelBase;
		public float axleWeightRatioFront;
		public float axleWeightRatioRear;

		//  Car state variables
		public float Heading = 0.0f;  // angle car is pointed at (radians)
		public Vector2 Velocity_car = new Vector2();  // m/s in LOCAL car coords (x is forward y is sideways)
		public Vector2 Acceleration = new Vector2();  // acceleration in WORLD coords
		public Vector2 Acceleration_car = new Vector2();   // accleration in LOCAL car coords
		public float AbsVelocity = 0.0f;  // absolute velocity m/s
		public float YawRate = 0.0f;   // angular velocity in radians
		public float SteeringAngle = 0.0f;  // actual front wheel steer angle (-maxSteer..maxSteer)
		
		public bool smoothSteer = true; // Use input smoothing (on by default)
		public bool safeSteer = true; // Use safe steering (angle limited by speed)

		private int id;
		private Rigidbody2D rb2d;

		void Start() {
			id = GetComponentInParent<Player.Player>().id;
			rb2d = GetComponent<Rigidbody2D>();

			inertia = rb2d.mass * inertiaScale;
			eBrakeForce = brakeForce / 2.5f;
			wheelBase = cgToFrontAxle + cgToRearAxle;
			axleWeightRatioFront = cgToRearAxle / wheelBase; // % car weight on the front axle
			axleWeightRatioRear = cgToFrontAxle / wheelBase; // % car weight on the rear axle
		}

		void Update() {
			// Velocity blue
			Debug.DrawLine(transform.position, transform.position + (Vector3)rb2d.velocity, Color.blue);
			// Facing white
			Debug.DrawLine(transform.position, transform.right * 0.3f + transform.position, Color.white);
			// Back tire black
			Debug.DrawLine(transform.position + transform.up - transform.right/3, transform.position - transform.up - transform.right/3, Color.black);
		}

		public void move() {
			// Get inputs
			float In_Steering = Input.GetAxis(id + "_AXIS_X"); // Steering angle (σ)
			float In_Throttle = Input.GetAxis(id + "_AXIS_Y");
			float In_EBrake = 0.0f; // TODO

			// Steering
			//  Perform filtering on steering...
			// TODO SMOOTHSTEER or SAFESTEER if toggled on
			//  Now set the actual steering angle
			SteeringAngle = -In_Steering * maxSteer;

			// Get velocity in local car coordinates
			Vector2 Velocity_car = (Vector2)transform.InverseTransformVector(rb2d.velocity);

			// Weight on axles based on centre of gravity and weight shift due to forward/reverse acceleration
			var axleWeightFront = rb2d.mass * (axleWeightRatioFront * gravity - weightTransfer * Acceleration_car.x * cgHeight / wheelBase);
			var axleWeightRear = rb2d.mass * (axleWeightRatioRear * gravity + weightTransfer * Acceleration_car.x * cgHeight / wheelBase);

			// Resulting velocity of the wheels as result of the yaw rate of the car body.
			// v = yawrate * r where r is distance from axle to CG and yawRate (angular velocity) in rad/s.
			var yawSpeedFront = cgToFrontAxle * YawRate;
			var yawSpeedRear = -cgToRearAxle * YawRate;

			// Calculate slip angles for front and rear wheels (a.k.a. alpha)
			var slipAngleFront = Mathf.Atan2(Velocity_car.y + yawSpeedFront, Mathf.Abs(Velocity_car.x)) - Mathf.Sign(Velocity_car.x) * SteeringAngle;
			var slipAngleRear  = Mathf.Atan2(Velocity_car.y + yawSpeedRear,  Mathf.Abs(Velocity_car.x));

			var tireGripFront = tireGrip;
			var tireGripRear = tireGrip * (1.0 - In_EBrake * (1.0 - lockGrip)); // reduce rear grip when ebrake is on

			var frictionForceFront_cy = Mathf.Clamp(-cornerStiffnessFront * slipAngleFront, -tireGripFront, tireGripFront) * axleWeightFront;
			var frictionForceRear_cy = Mathf.Clamp(-cornerStiffnessRear * slipAngleRear, -(float)tireGripRear, (float)tireGripRear) * axleWeightRear;

			//  Get amount of brake/throttle from our inputs
			var brake = 0.0f; // TODO Mathf.Min(inputs.brake * brakeForce + inputs.ebrake * eBrakeForce, brakeForce);
			var throttle = In_Throttle * engineForce;

			//  Resulting force in local car coordinates.
			//  This is implemented as a RWD car only.
			var tractionForce_cx = throttle - brake * Mathf.Sign(Velocity_car.x);
			var tractionForce_cy = 0.0f;

			var dragForce_cx = -rollResist * Velocity_car.x - airResist * Velocity_car.x * Mathf.Abs(Velocity_car.x);
			var dragForce_cy = -rollResist * Velocity_car.y - airResist * Velocity_car.y * Mathf.Abs(Velocity_car.y);

			// total force in car coordinates
			var totalForce_cx = dragForce_cx + tractionForce_cx;
			var totalForce_cy = dragForce_cy + tractionForce_cy + Mathf.Cos(SteeringAngle) * frictionForceFront_cy + frictionForceRear_cy;

			// acceleration along car axes
			Acceleration_car.x = totalForce_cx / rb2d.mass;  // forward/reverse accel
			Acceleration_car.y = totalForce_cy / rb2d.mass;  // sideways accel

			// acceleration in world coordinates
			Acceleration = transform.TransformVector(new Vector2(Acceleration_car.x, Acceleration_car.y));

			// update velocity
			rb2d.velocity += Acceleration * Time.fixedDeltaTime;
			AbsVelocity = rb2d.velocity.magnitude;

			// calculate rotational forces
			var angularTorque = (frictionForceFront_cy + tractionForce_cy) * cgToFrontAxle - frictionForceRear_cy * cgToRearAxle;

			//  Sim gets unstable at very slow speeds, so just stop the car
			if( Mathf.Abs(AbsVelocity) < 0.2 && throttle == 0.0f )
			{
				rb2d.velocity = Vector2.zero;
				AbsVelocity = 0;
				angularTorque = YawRate = 0;
			}

			var angularAccel = angularTorque / inertia;

			YawRate += angularAccel * Time.fixedDeltaTime;
//			rb2d.rotation += YawRate * Time.fixedDeltaTime;
//			Heading += YawRate * Time.fixedDeltaTime;
			transform.Rotate(new Vector3(0, 0, SteeringAngle));

			//  finally we can update position
			transform.position += (Vector3)rb2d.velocity * Time.fixedDeltaTime;

			//  Display some data
			Debug.Log("========================");
			Debug.Log("speed " + Velocity_car.x * 3600 / 1000 );  // km/h
			Debug.Log("accleration " + Acceleration_car.x);
			Debug.Log("yawRate " + YawRate);
			Debug.Log("weightFront " + axleWeightFront);
			Debug.Log("weightRear " + axleWeightRear);
			Debug.Log("slipAngleFront " + slipAngleFront);
			Debug.Log("slipAngleRear " + slipAngleRear);
			Debug.Log("frictionFront " + frictionForceFront_cy);
			Debug.Log("frictionRear " + frictionForceRear_cy);			

			debugStuff(new Vector2(In_Steering, In_Throttle)); // DEBUG
			BoostManager.getBoostModifier(id); // TODO 
		}

		/*
		 * DEBUG
		 */
		private void debugStuff(Vector2 input) {
			// Gas green (input.y)
			Debug.DrawLine(transform.position, transform.position + input.y * transform.right, Color.green);
			// Steering green (input.x)
			Debug.DrawLine(transform.position, transform.position + input.x * -transform.up, Color.green);

			// Wheel Rotation
//			Debug.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(MAX_TURN * -input.x, Vector3.forward) * transform.right * 0.5f, Color.yellow);
		}
	}
}
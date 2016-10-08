using UnityEngine;
using System.Collections;
using RL2600.System;

// http://www.asawicki.info/Mirror/Car%20Physics%20for%20Games/Car%20Physics%20for%20Games.html
// https://nccastaff.bournemouth.ac.uk/jmacey/MastersProjects/MSc12/Srisuchat/Thesis.pdf

namespace RL2600.Behavior {

	[RequireComponent (typeof (Rigidbody2D))]

	public class MarcoMovement : MonoBehaviour, IMoveable {
		const float C_DRAG = 0.4257f;
		const float C_RR = 2.0f; // rolling-resistance // "SHOULD BE APPROX 30x C_DRAG" -- but be prepared to fine-tune TODO
		const float C_CS = 2.0f; // corning-stiffness

		private int id;
		private Rigidbody2D rb2d;

		void Start() {
			id = GetComponentInParent<Player.Player>().id;
			rb2d = GetComponent<Rigidbody2D>();
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
			// 0. Some needed variables
			var steeringAngle = new Vector2(Input.GetAxis(id + "_AXIS_X"), 0); // Steering angle (σ)  TODO: Get from user input?
			var w = new Vector2(0, Input.GetAxis(id + "_AXIS_Y")); // TODO angular velocity of the engine in rad/s: ωe = 2π Ωe / 60    \\Note\\ Ω === engine turnover rate // Surely this is related to how much gas you are giving it // or its related to #6
			var b = 0.5f; // TODO get from car (center to front axle)... no idea what units this should be
			var c = 0.5f; // TODO get from car (center to rear axle)... no idea what units this whould be

			// 1. Transform velocity in world reference frame to velocity in car reference frame 
			//    (Vx = Vlong, Vy = Vlat). Convention for car reference frame: 
			//    x – pointing to the front, z – pointing to the right
			Vector2 velocity = (Vector2)transform.InverseTransformVector(rb2d.velocity);

			// 2. Compute the slip angles for front and rear wheels (equation 5.2)
			//    αfront = arctan((Vlat + ω * b) / Vlong)) – σ * sgn(Vlong) 
			//    αrear = arctan((Vlat - ω * c) / Vlong))
			var slipAngleFront = Mathf.Atan((velocity.y) + w * b / velocity.x) - steeringAngle * Mathf.Sign(velocity.x);
			var slipAngleRear = Mathf.Atan((velocity.y) - w * c / velocity.x);

			// 3. Compute Flat = Ca * slip angle (do for both rear and front wheels)
			var F_latFront = slipAngleFront * C_CS;
			var F_latRear = slipAngleRear * C_CS;

			// 4. Cap Flat to maximum normalized frictional force (do for both rear and front wheels)
			// TODO: cap due to max torque an engine can put out at a given RPM? ...no that seems to be #7...
			var F_capFront = F_latFront; // TODO CAP THIS
			var F_capRear = F_latRear; // TODO CAP THIS

			// 5. Multiply Flat by the load (do for both rear and front wheels) to obtain the cornering forces.
			var F_cornFront = F_capFront * 0.5f; // TODO is .5 good?
			var F_cornRear = F_capRear * 0.5f; // TODO is .5 good?

//			6. Compute the engine turn over rate Ωe = Vx 60*gk*G / (2π * rw)
//			7. Clamp the engine turn over rate from 6 to the defined redline
//			8. If use automatic transmission call automaticTransmission() function
//			to shift the gear
//			9. Compute the constant that define the torque curve line from the engine turn over rate
//			10. From 9, compute the maximum engine torque, Te
//			11. Compute the maximum torque applied to the wheel Tw = Te * gk * G

			// 12. Multiply the maximum torque with the fraction of the throttle
			//     position to get the actual torque applied to the wheel (Ftraction - The traction force)
			var F_traction = 0; // TODO

//			13. If the player is braking replace the traction force from 12 to a defined braking force
//			14. If the car is in reverse gear replace the traction force from 12 to a defined reverse force

			// 15-17 compute total resistance 
			var F_rr = 0; // TODO
			var F_drag = 0; // TODO
			Vector2 F_resistance = F_rr + F_drag;

			// 18. Sum the force on the car body
			//     Fx = Ftraction + Flat,front * sin (σ) * Fresistance,x
			//     Fz = Flat, rear + Flat,front * cos (σ) * Fresistance,z
			var F_x = F_traction + F_latFront * Mathf.Sin(steeringAngle) * F_resistance.x;
			var F_y = F_latRear + F_latFront * Mathf.Cos(steeringAngle) * F_resistance.y;
			// TODO: do i have my x / x and z / y conversion right here?

			// 19. Compute the torque on the car body
			//     Torque=cos(σ)*Flat,front *b–Flat,rear *c
			var Torque = Mathf.Cos(steeringAngle) * F_latFront * b - F_latRear * c;

			// 20. Compute the  acceleration
			//     a=F/M
			var acceleration = 0; // TODO get mass from car... force is...?

			// 21. Compute angular acceleration
			//     α = Torque/Inertia
			var angularAcceleration = 0; // TODO

			// 22. Transform acceleration from car to world
			transform.TransformVector(acceleration);

			// 23. Integrate the acceleration to get the velocity (in world reference frame)
			// 24. Integrate the velocity to get the new position in world coordinate: Pwc += dt * Vwc
			// 25. Move the camera to follow the car
			// 26. Integrate the angular acceleration to get the angular velocity: ω += dt * α
			// 27. Integrate the angular velocity to get the angular orientation: Yaw angle += dt * ω
			// 28. Obtain the rotation rate of the wheels by dividing the car speed with the wheel radius: Wheel rotation rate = car speed / wheel radius
			// 29. Re-render the car






			// Marco only:

			var input = new Vector2(Input.GetAxis(id + "_AXIS_X"), Input.GetAxis(id + "_AXIS_Y"));

			float ENGINEPOWERBCYREMOVE = 30.0f;
			float engineForce = input.y * ENGINEPOWERBCYREMOVE;

			// Determine forces
			Vector2 F_long = getTotalLongitudinalForces(engineForce, rb2d.velocity);
			Vector2 F_lat = getTotalLateralForces(input.x);

			Debug.Log("input: " + input); // DEBUG
			Debug.Log("F_long: " + F_long); // DEBUG
			Debug.Log("F_lat: " + F_lat); // DEBUG

			// Gas green
			Debug.DrawLine(transform.position, transform.position + input.y * transform.right, Color.green);
			// Turning yellow
			Debug.DrawLine(transform.position, transform.position + input.x * -transform.up, Color.yellow);

			// Accel
			var accel = F_long / rb2d.mass;
			// New velocity
			var V_new = rb2d.velocity + accel * Time.fixedDeltaTime;
			// New position
			var P_new = (Vector2)transform.position + V_new * Time.fixedDeltaTime;

			rb2d.velocity = V_new;
			transform.position = P_new;




//			rb2d.AddForce(F_long);

//			rb2d.AddRelativeForce(F_long);

//			if (movement != Vector2.zero) {
//				float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
//				transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
//			}
		}

		/*
		 * LONGITUDINAL
		 */

		// Wheel force, braking force, rolling resistance, and drag
		private Vector2 getTotalLongitudinalForces(float engineForce, Vector2 carVelocity) {
			var localUnitHeading = Vector3.Normalize(transform.InverseTransformPoint(transform.right + transform.position));
	
			Vector2 F_traction = getTractiveForce(engineForce, localUnitHeading);
			Vector2 F_drag = getDrag(carVelocity);
			Vector2 F_rr = getRollingResistance(carVelocity);

			// DEBUG
			Debug.Log("F_traction: " + F_traction);
			Debug.Log("F_drag: " + F_drag);
			Debug.Log("F_rr: " + F_rr);

			return F_traction + F_drag + F_rr;
			// TODO (Revisit) should calculate a F_braking to replace F_traction when the brakes
			// are being applied. F_braking = -localUnitHeading * C_BRAKE; 
			// F_traction will natually go negative when engineForce is negative, so I am using
			// that as my braking method for now. In the future, there may be a better feel to use 
			// a separate braking force. 
		}

		// Get the force delivered by the engine via the rear wheels
		private Vector2 getTractiveForce(float engineForce, Vector2? unitHeading = null) {
			return engineForce * unitHeading ?? Vector2.right;
		}

		// Get the resistive force due to air resistance (aerodynamic drag)
		private Vector2 getDrag(Vector2 velocity, float dragConstant = C_DRAG) {
			return -dragConstant * velocity * velocity.magnitude;
		}

		// Get the resistive force due to friction of various components 
		private Vector2 getRollingResistance(Vector2 velocity, float rrConstant = C_RR) {
			return -rrConstant * velocity;
		}

		/*
		 * LATERAL
		 */

		private Vector2 getTotalLateralForces(float turning, float corningStiffnessConstant = C_CS) {
			var localUnitHeading = Vector3.Normalize(rb2d.transform.InverseTransformPoint(rb2d.transform.right + rb2d.transform.position));

			float slipAngle = getFrontSlipAngle();

			return turning * slipAngle * corningStiffnessConstant * localUnitHeading;
		}

		// Slip angle (alpha) for the front tires
		private float getFrontSlipAngle() {
			return 10.0f;
		}
	}
}
using UnityEngine;
using System.Collections;

namespace RL2600.Curve {
	public class MovementCurves : MonoBehaviour {
		public AnimationCurve SlipAngle;
		public AnimationCurve EngineTorque;
		public AnimationCurve SlipRatio;

		// http://answers.unity3d.com/answers/320729/view.html
		[ContextMenu ("Read Curves")]
		public void ReadCurves() {
			Keyframe[] points;

			// Slip Angle - (marco)
			points = new Keyframe[4]; 
			points[0] = new Keyframe(-20, -5000);
			points[1] = new Keyframe(-3, -5800);
			points[2] = new Keyframe(3, 5800);
			points[3] = new Keyframe(20, 5000);
			SlipAngle = new AnimationCurve(points);

			// Torque - (marco)
			points = new Keyframe[4]; 
			points[0] = new Keyframe(0, 220);
			points[1] = new Keyframe(1000, 220);
			points[2] = new Keyframe(4800, 310);
			points[3] = new Keyframe(7000, 220);
			EngineTorque = new AnimationCurve(points);

			// Slip ratio - bobz
			points = new Keyframe[3]; 
			points[0] = new Keyframe(0.00f, 0.00f);
			points[1] = new Keyframe(0.13f, 1.00f);
			points[1].outTangent = -0.90f;
			points[2] = new Keyframe(1.00f, 0.67f);
			SlipRatio = new AnimationCurve(points);
		}
	}
}
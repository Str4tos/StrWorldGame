using UnityEngine;
using System.Collections;

namespace Objectscripts
{
	public class OrbitMovement : MonoBehaviour
	{
		Transform Target;
		private float xDeg = 0.0f; 
		private float yDeg = 0.0f;
		private float maxSpeed = 0.8f;
		private float xSpeedCurrent = 0f; 
		private float ySpeedCurrent = 0f; 
		private float xSpeedDesired = 0.0f; 
		private float ySpeedDesired = 0.0f; 
		private float currentDistance = 2f;
		private float xSwitchSpeedAfter = 2f;
		private float ySwitchSpeedAfter = 2.5f;
		private float xSwitchSpeedTimer = 0;
		private float ySwitchSpeedTimer = 0;
		
		// Use this for initialization
		void Start ()
		{
			Target = GameObject.Find ("Graphics").transform;
			xSpeedDesired = Random.Range(-maxSpeed,maxSpeed);
			ySpeedDesired = Random.Range(-maxSpeed,maxSpeed);
			xSpeedCurrent = Random.Range(-maxSpeed,maxSpeed);
			ySpeedCurrent = Random.Range(-maxSpeed,maxSpeed);
		}
		
		// Update is called once per frame
		void Update ()
		{
			xSwitchSpeedTimer += Time.deltaTime;
			ySwitchSpeedTimer += Time.deltaTime;
			
			// Switch x Direction
			if(xSwitchSpeedTimer > xSwitchSpeedAfter)
			{
				xSwitchSpeedTimer = 0;
				xSpeedDesired = Random.Range(-maxSpeed,maxSpeed);
			}		
			
			// Switch y direction, also making sure the orb doesn't move below the target
			if(ySwitchSpeedTimer > ySwitchSpeedAfter || yDeg < -30 || yDeg > 120)
			{
				ySwitchSpeedTimer = 0;
				ySpeedDesired = Random.Range(-maxSpeed,maxSpeed);
			}
			
			xSpeedCurrent = Mathf.Lerp(xSpeedCurrent, xSpeedDesired, xSwitchSpeedTimer / xSwitchSpeedAfter / 50); 
			ySpeedCurrent = Mathf.Lerp(ySpeedCurrent, ySpeedDesired, ySwitchSpeedTimer / ySwitchSpeedAfter / 50); 
			
			xDeg += xSpeedCurrent; 
			yDeg -= ySpeedCurrent; 		
					
			Quaternion rotation = Quaternion.Euler (yDeg, xDeg, 0); 
		
		
			Vector3 position; 
		
	
			// Recalculate position based on the new currentDistance 
			position = Target.position - (rotation * Vector3.forward * currentDistance) + Vector3.up; 
			
			transform.position = position; 
		}
		
	}
}

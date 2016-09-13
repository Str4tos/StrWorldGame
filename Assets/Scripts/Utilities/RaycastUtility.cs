using UnityEngine;
using System.Collections;

namespace Utilities
{
	public class RaycastUtility : MonoBehaviour {
	
		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
		
		public static Vector3 GetNearestMouseRaycasthit()
		{
			RaycastHit hit;
			
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			Physics.Raycast(ray, out hit);
			
			return hit.point;
		}
		
			
		public static Vector3 GetSpecificMouseRaycasthit(string name)
		{		
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			RaycastHit[] hits = Physics.RaycastAll(ray);
			
			for(int i = 0; i < hits.Length; i++)
			{
				if(hits[i].collider.name == name)
				{
					return hits[i].point;	
				}
			}
			
			return Vector3.zero;
		}	
		
		public static GameObject GetSpecificMouseRaycasthitGameObject(string name)
		{		
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			RaycastHit[] hits = Physics.RaycastAll(ray);
			
			for(int i = 0; i < hits.Length; i++)
			{
				if(hits[i].collider.name == name)
				{
					return hits[i].collider.gameObject;	
				}
			}
			
			return null;
		}
	}
}

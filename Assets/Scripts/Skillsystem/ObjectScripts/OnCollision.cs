using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Objectscripts
{		
	public delegate void OnCollisionEventHandler(List<GameObject> colliders);

	public class OnCollision : MonoBehaviour {
	
		public event OnCollisionEventHandler AfterCollision;		
		
		// Use this for initialization
		void Start () 
		{
		}
		
		// Update is called once per frame
		void Update () 
		{

		}
		
		void OnCollisionEnter(Collision collision)
		{
			if(AfterCollision != null && collision.gameObject.layer != 8)
			{
				List<GameObject> col = new List<GameObject>();
				for(int i = 0; i < collision.contacts.Length; i++)
				{
					col.Add (collision.contacts[i].otherCollider.gameObject);
				}
				AfterCollision(col);	
			}
		}
	}
}

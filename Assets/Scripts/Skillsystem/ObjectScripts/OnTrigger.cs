using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Objectscripts
{		
	public delegate void OnTriggerEventHandler(GameObject sender, List<GameObject> colliders, Vector3 pos);

	public class OnTrigger : MonoBehaviour {
	
		public event OnTriggerEventHandler AfterCollision;
		
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
            foreach (ContactPoint contact in collision.contacts)
            {
                Debug.Log(contact.thisCollider.gameObject.name);
            }
        }


		void OnTriggerEnter(Collider collider)
		{
			if(AfterCollision != null && collider.gameObject.layer != LayerMask.NameToLayer("NotTargetable"))
			{
				List<GameObject> col = new List<GameObject>();
				col.Add(collider.gameObject);
				AfterCollision(gameObject, col, transform.position);	
			}
		}
	}
}

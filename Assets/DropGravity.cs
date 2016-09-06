using UnityEngine;
using System.Collections;

public class DropGravity : MonoBehaviour {

    private BoxCollider _BoxCollider;

	// Use this for initialization
	void Start () {
        _BoxCollider = GetComponent<BoxCollider>();
        
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "targetingPlane")
        {
            GetComponent<Rigidbody>().useGravity = false;
            _BoxCollider.isTrigger = false;
        }
    }
}

using UnityEngine;
using System.Collections;

public class SightEnemy : MonoBehaviour {

	// that bonus makes sphere collider larger when player crosses it. It is wise to keep it at least on the 1.1 level to not allow player run easly from enemy just after spotting it.
	public float sphCollRadiusBonus = 1.5f;

    private SphereCollider _SphereCollider;
    private float sphCollBaseRadius = 0.0f;
	private StatusPlayer targetStatus;
	private StatusEnemy _StatusEnemy;
    //private StatusPlayer _StatusPlayer;

	// Use this for initialization
	void Start () {
        _SphereCollider = gameObject.GetComponent<SphereCollider>();
        sphCollBaseRadius = _SphereCollider.radius;
		_StatusEnemy = transform.parent.GetComponent<StatusEnemy> ();
	}

    void Update()
    {
		if(_SphereCollider.radius != sphCollBaseRadius && _StatusEnemy.TargetStatus == null)
			_SphereCollider.radius = sphCollBaseRadius;
    }

    void OnTriggerEnter(Collider otherCollider)
	{
		if (_StatusEnemy.TargetStatus == null) 
			if (otherCollider.tag == "Player") {
				_StatusEnemy.TargetStatus = otherCollider.gameObject.GetComponent<StatusPlayer> ();
				_SphereCollider.radius = _SphereCollider.radius * sphCollRadiusBonus;
			}
	}

    void OnTriggerExit(Collider otherCollider)
    {
        if (otherCollider.tag == "Player")
        {
            _SphereCollider.radius = sphCollBaseRadius;
			_StatusEnemy.TargetStatus = null;
        }
    }
}

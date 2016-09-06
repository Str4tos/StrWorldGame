using UnityEngine;
using System.Collections;

public class SightEnemy : MonoBehaviour {

    
    [HideInInspector]
    private SphereCollider _SphereCollider;
    [HideInInspector]
    private float sphCollBaseRadius = 0.0f;
    // that bonus makes sphere collider larger when player crosses it. It is wise to keep it at least on the 1.1 level to not allow player run easly from enemy just after spotting it.
    public float sphCollRadiusBonus = 1.5f;
    private GameObject playerInRangeObj = null;
    private bool playerInRangeBool = false;
    //private StatusPlayer _StatusPlayer;

	// Use this for initialization
	void Start () {
        _SphereCollider = gameObject.GetComponent<SphereCollider>();
        sphCollBaseRadius = _SphereCollider.radius;
	}

    void Update()
    {

    }

    public bool GetPlayerInRangeBool 
    { 
        get {return playerInRangeBool;} 
		set { playerInRangeBool = value; }
        
    }
    public GameObject GetPlayerInRangeObj
    {
        get { return playerInRangeObj; }
    }

	

    void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.tag == "Player")
        {
            if (playerInRangeObj == null)
            {
                playerInRangeObj = otherCollider.gameObject;
                //_StatusPlayer = playerInRangeObj.GetComponent<StatusPlayer>();
            }
            //InvokeRepeating("CheckIfPlayerIsAlive", 0.5f, 0.5f);
            playerInRangeBool = true;
            _SphereCollider.radius = _SphereCollider.radius * sphCollRadiusBonus;
        }
    }


    void OnTriggerExit(Collider otherCollider)
    {
        if (otherCollider.tag == "Player")
        {
            playerInRangeBool = false;
            _SphereCollider.radius = sphCollBaseRadius;
            //CancelInvoke("CheckIfPlayerIsAlive");
        }
    }
    public void setDefaultSphreCollider()
    {
        _SphereCollider.radius = sphCollBaseRadius;
        playerInRangeObj = null;
		playerInRangeBool = false;
    }

    //void CheckIfPlayerIsAlive()
    //{
    //    if (playerInRangeBool)
    //    {
    //        if (_StatusPlayer.IsDead)
    //        {
    //            playerInRangeBool = false;
    //            _SphereCollider.radius = sphCollBaseRadius;
    //            CancelInvoke("CheckIfPlayerIsAlive");
                
    //        }
    //    }
    //}
}

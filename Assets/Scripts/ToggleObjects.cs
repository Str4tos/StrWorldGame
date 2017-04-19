using UnityEngine;
using System.Collections;

public class ToggleObjects : MonoBehaviour
{

    GameObject tempObject;

    // Use this for initialization
    void Start()
    {
        //		BoxCollider _BoxCollider = GetComponent<BoxCollider> ();
        //		Vector3 sizeCamera = new Vector3 (Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);  
        //		_BoxCollider.size = Camera.main.ViewportToWorldPoint (sizeCamera) + Vector3.up;
        //

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.tag == "enemy")
        {
            setActiveStates(otherCollider.gameObject.transform, true);

        }
    }


    void OnTriggerExit(Collider otherCollider)
    {
        if (otherCollider.tag == "enemy" && !otherCollider.gameObject.GetComponent<StatusEnemy>().IsDead)
        {
            setActiveStates(otherCollider.gameObject.transform, false);
        }
    }

    private void setActiveStates(Transform tempObject, bool activeState)
    {
        tempObject.GetComponent<Animator>().enabled = activeState;
        UnityEngine.AI.NavMeshAgent tempNavMeshAgent = tempObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (tempNavMeshAgent != null)
        {
            tempNavMeshAgent.enabled = activeState;
        }
        foreach (MonoBehaviour tempComponent in tempObject.GetComponents<MonoBehaviour>())
        {
            tempComponent.enabled = activeState;
        }
        for (int i = 0; i < tempObject.childCount; i++)
        {
            tempObject.GetChild(i).gameObject.SetActive(activeState);
        }
    }

    void LateUpdate()
    {


        //			Debug.Log ("----- Deactivate start " + Time.realtimeSinceStartup);
        //			for (int i = 0; i < gameObject.transform.childCount; i++) {
        //				gameObject.transform.GetChild (i).gameObject.SetActive (false);
        //			}
        //			NavMeshAgent _nma = gameObject.GetComponent<NavMeshAgent> ();
        //			StatusEnemy _stat = gameObject.GetComponent<StatusEnemy> ();
        //			MovementEnemy _Mov = gameObject.GetComponent<MovementEnemy>();
        //			Animator _anim = gameObject.GetComponent<Animator> ();
        //
        //			_nma.enabled = false;
        //			_anim.enabled = false;
        //			_Mov.enabled = false;
        //			_stat.enabled = false;
        //			Debug.Log ("Deactivate stop " + Time.realtimeSinceStartup);
        //
        //			Debug.Log("----- Activate start " + Time.realtimeSinceStartup);
        //			for (int i = 0; i < gameObject.transform.childCount; i++) {
        //				gameObject.transform.GetChild (i).gameObject.SetActive (true);
        //			}
        //			NavMeshAgent _nma2 = gameObject.GetComponent<NavMeshAgent> ();
        //			StatusEnemy _stat2 = gameObject.GetComponent<StatusEnemy> ();
        //			MovementEnemy _Mov2 = gameObject.GetComponent<MovementEnemy>();
        //			Animator _anim2 = gameObject.GetComponent<Animator> ();
        //
        //			_nma2.enabled = true;
        //			_anim2.enabled = true;
        //			_Mov2.enabled = true;
        //			_stat2.enabled = true;
        //			Debug.Log ("Activate stop" + Time.realtimeSinceStartup);
        //
        //			Debug.Log ("------ Deactivate all start " + Time.realtimeSinceStartup);
        //			if (gameObject.activeSelf)
        //				gameObject.SetActive (false);
        //
        //			Debug.Log ("Deactivate all stop " + Time.realtimeSinceStartup);
        //
        //			Debug.Log ("---- Activate all start " + Time.realtimeSinceStartup);
        //
        //			if (!gameObject.activeSelf)
        //				gameObject.SetActive (true);
        //
        //			Debug.Log ("Activate all stop " + Time.realtimeSinceStartup);

    }
}

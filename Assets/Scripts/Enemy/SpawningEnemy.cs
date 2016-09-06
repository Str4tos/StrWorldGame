using UnityEngine;
using System.Collections;

public class SpawningEnemy : MonoBehaviour
{

    public GameObject enemyPrefab;
    public float radiusSpawn = 4.0f;
    public int numberEnemy = 5;

    public float respTimeMultipl = 1.0f;
    private float staticRespTime = 7.0f; //Edit this value to adjust respawn time 
    private float respawnTime;

    private GameObject[] enemiesObj;
    private float[] enemiesTimeRespawn;
    private bool[] enemiesIsDie;


	private Vector3 tempPoition;

    // Use this for initialization
    void Start()
    {
        respawnTime = staticRespTime * respTimeMultipl;
		enemiesObj = new GameObject[numberEnemy];
        enemiesTimeRespawn = new float[numberEnemy];
        enemiesIsDie = new bool[numberEnemy];
        
        for (int i = 0; i < numberEnemy; i++)
        {
			NewEnemyObject (i);
        }

    }


    // Update is called once per frame
    void Update()
    {
            for (int i = 0; i < numberEnemy; i++)
            {
                if (enemiesIsDie[i] && Time.time > enemiesTimeRespawn[i])
                {
                    //Respawn enemy
				NewEnemyObject(i);
                   //enemiesObj[i].GetComponent<StatusEnemy>().Respawn();
                }
            }
        
    }
	private void NewEnemyObject(int index){
		tempPoition = getRandomPosition();
		enemiesObj [index] = Instantiate (enemyPrefab, tempPoition, new Quaternion (0, Random.rotation.y, 0, 1), transform) as GameObject;
		enemiesObj[index].name = index.ToString();
		enemiesIsDie[index] = false;
	}

	public Vector3 getRandomPosition()
    {
		return new Vector3(Random.Range(transform.position.x - radiusSpawn, transform.position.x + radiusSpawn),
			transform.position.y, Random.Range(transform.position.z - radiusSpawn, transform.position.z + radiusSpawn));
    }

    public void enemyDie(string nameEnemy)
    {
        int idEnemy = System.Int32.Parse(nameEnemy);
        //enemiesObj[idEnemy].SetActive(false);
        enemiesTimeRespawn[idEnemy] = Time.time + respawnTime;
        enemiesIsDie[idEnemy] = true;
    }


//	private void setActiveStates(Transform tempObject, bool activeState){
//		for (int i = 0; i < tempObject.childCount; i++) {
//			tempObject.GetChild (i).gameObject.SetActive (activeState);
//		}
//		tempObject.GetComponent<Animator> ().enabled = activeState;
//		NavMeshAgent tempNavMeshAgent = tempObject.GetComponent<NavMeshAgent> ();
//		if (tempNavMeshAgent != null) {
//			tempNavMeshAgent.enabled = activeState;
//		}
//		foreach (MonoBehaviour tempComponent in tempObject.GetComponents<MonoBehaviour>()) {
//			tempComponent.enabled = activeState;
//		}
//	}

//    void OnTriggerEnter(Collider otherCollider)
//    {
//        if (otherCollider.tag == "Player" && !spawnActive)
//        {
//            for (int i = 0; i < numberEnemy; i++)
//            {
//                enemiesObj[i].SetActive(true);
//            }
//            spawnActive = true;
//            //InvokeRepeating("CheckIfPlayerIsAlive", 0.5f, 0.5f);
//            //esMovement.playerInRange = true;
//            //sphColl.radius = sphColl.radius * sphCollRadiusBonus;
//        }
//    }
//
//
//    void OnTriggerExit(Collider otherCollider)
//    {
//        if (otherCollider.tag == "Player" && spawnActive)
//        {
//            for (int i = 0; i < numberEnemy; i++)
//            {
//                enemiesObj[i].SetActive(false);
//            }
//            spawnActive = false;
//            
//            //esMovement.playerInRange = false;
//            //sphColl.radius = sphCollBaseRadius;
//            //CancelInvoke("CheckIfPlayerIsAlive");
//        }
//    }

}

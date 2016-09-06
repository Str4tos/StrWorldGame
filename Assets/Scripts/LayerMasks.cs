using UnityEngine;
using System.Collections;

public class LayerMasks : MonoBehaviour {

	[HideInInspector] public int layerPlayer = 1 << 20;
	[HideInInspector] public int layerEnemies = 1 << 21;
	[HideInInspector] public int layerEnemyMouseCollider = 1 << 22; // this layer is for enemies - more precisly it is for ColliderMouse child game object of the enemy prefab. By changing the size of the collider you can change mouse precision for targeting enemies
	[HideInInspector] public int layerTargetingPlaneToMove = 1 << 23; // this layer is for registering movement. Targeting plane prefab should be placed just beneath the ground mesh used to bake nav mesh.
	[HideInInspector] public int layerObstacles = 1 << 24;
	[HideInInspector] public int layerInteractiveObject = 1 << 25; // this is doors mostly.
	[HideInInspector] public int layerBullets = 1 << 26;
	[HideInInspector] public int layerTargetingPlaneToShoot = 1 << 27;
	[HideInInspector] public int layerGround = 1 << 28;
	[HideInInspector] public int layerEnemiesProjectiles = 1 << 29;

	[HideInInspector] public int maskMoveEnemiesDoors = 0;
	[HideInInspector] public int maskMoveEnemies = 0;
	[HideInInspector] public int maskShootEnemies = 0;

	void OnStart(){
		maskMoveEnemiesDoors =  layerTargetingPlaneToMove | layerEnemyMouseCollider | layerInteractiveObject;
		maskMoveEnemies = layerTargetingPlaneToMove | layerEnemyMouseCollider;
		maskShootEnemies = layerTargetingPlaneToShoot | layerEnemyMouseCollider;

	}

}

using UnityEngine;
using System.Collections;

public class MouseOverEnemy : MonoBehaviour {
		// That function takes care of visual presentation of mouse movement over enemies - it makes sure that proper health bar is displayed and shader displays an outline.
		// It also has a function that changes shader based on monster rarity.
		// No mouse input is managed here.

		private GameObject hpBar;
		private GameObject textField;

		private Ray ray;
		private RaycastHit hit;
		private bool dataSent = false;
		private GameObject model;
		private Material material1;
		private Material material2;
		private Material material3;
		private Material material4;
		private int noOfmats = 0;
		private float colliderRadius = 0;
		private CapsuleCollider collider;
		private GuiInfoEnemy _GuiEnemyInfo;
		private int layerEnemyMouseCollider = 1 << 22;

		public bool increaseColliderRadiusOnMouseOver = true;

		void Start()
		{
			GameObject _MainCanvas = GameObject.Find ("MainCanvas");
			_GuiEnemyInfo = _MainCanvas.GetComponent <GuiInfoEnemy>();
			collider = gameObject.GetComponent<CapsuleCollider>();
			colliderRadius = collider.radius;
			SetMaterials();
		}

		void Update()
		{
			CustomMouseOver();
		}

		void CustomMouseOver()
		{
			if (!model.GetComponent<Renderer>().isVisible) { return; }
			if (_GuiEnemyInfo == null || _GuiEnemyInfo.enemyHPpanel == null) { return; }

			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit, 60.0f, layerEnemyMouseCollider))
			{
				if (hit.transform.gameObject.GetInstanceID() == gameObject.GetInstanceID())
				{
					if (dataSent == false)
					{
						_GuiEnemyInfo.enemyHPpanel.SetActive(true);
						SetMaterialOutline(true);
						_GuiEnemyInfo.GetTargetEnemy(gameObject.transform.parent.gameObject);
						if (increaseColliderRadiusOnMouseOver) { collider.radius = colliderRadius * 1.4f; }
						dataSent = true;
					}
				}
				else
				{
					dataSent = false;
					SetMaterialOutline(false);
					collider.radius = colliderRadius;
				}
			}
			else
			{
				_GuiEnemyInfo.enemyHPpanel.SetActive(false);
				SetMaterialOutline(false);
				dataSent = false;
				collider.radius = colliderRadius;
			}
		}

		// technical function, has to be called very early to connect materials to the variables.
		public void SetMaterials()
		{
			model = gameObject.transform.parent.FindChild("3DModel").gameObject;
			Renderer _Renderer = model.GetComponent<Renderer> ();
			noOfmats = _Renderer.materials.Length;
			if (noOfmats > 0) { material1 = _Renderer.materials[0]; }
			if (noOfmats > 1) { material2 = _Renderer.materials[1]; }
			if (noOfmats > 2) { material3 = _Renderer.materials[2]; }
			if (noOfmats > 3) { material4 = _Renderer.materials[3]; }
		}

		// sets the outline on and off, here you can change outline size.
		public void SetMaterialOutline(bool smBool)
		{
			if (smBool)
			{

				if (noOfmats > 0) {material1.SetFloat("_Outline", 0.0015f);}
				if (noOfmats > 1) {material2.SetFloat("_Outline", 0.0015f);}
				if (noOfmats > 2) {material3.SetFloat("_Outline", 0.0015f);}
				if (noOfmats > 3) {material4.SetFloat("_Outline", 0.0015f);}
			}
			else
			{
				if (noOfmats > 0) {material1.SetFloat("_Outline", 0.00f);}
				if (noOfmats > 1) {material2.SetFloat("_Outline", 0.00f);}
				if (noOfmats > 2) {material3.SetFloat("_Outline", 0.00f);}
				if (noOfmats > 3) {material4.SetFloat("_Outline", 0.00f);}

			}

		}

		// here materials colors and shaders are managed for Rare and Champion monsters.
		public void SetMaterialsColors(string colorToSet)
		{
			Shader shader1 = Shader.Find("aRPG/OutlinedRimBumpSpec");
			Shader shader2 = Shader.Find("aRPG/OutlinedBumpSpec");
			if (colorToSet == "blue")
			{
				if (noOfmats > 0)
				{
					material1.shader = shader1;
					material1.SetColor("_RimColor", Color.cyan);
				}
				if (noOfmats > 1)
				{

					material2.shader = shader1;
					material2.SetColor("_RimColor", Color.cyan);
				}
				if (noOfmats > 2)
				{

					material3.shader = shader1;
					material3.SetColor("_RimColor", Color.cyan);
				}
				if (noOfmats > 3)
				{

					material4.shader = shader1;
					material4.SetColor("_RimColor", Color.cyan);
				}
			}
			if (colorToSet == "rare")
			{
				if (noOfmats > 0)
				{
					material1.shader = shader1;
					material1.SetColor("_RimColor", Color.yellow);
				}
				if (noOfmats > 1)
				{

					material2.shader = shader1;
					material2.SetColor("_RimColor", Color.yellow);
				}
				if (noOfmats > 2)
				{

					material3.shader = shader1;
					material3.SetColor("_RimColor", Color.yellow);
				}
				if (noOfmats > 3)
				{

					material4.shader = shader1;
					material4.SetColor("_RimColor", Color.yellow);
				}
			}
		}



		/*
	void OnMouseEnter () 
    {
        
    }

    void OnMouseDown () {
    }

    void OnMouseUp () {
    }

    void OnMouseExit () {

    }
     */
	}

using UnityEngine;
using System.Collections;

public class MaterialGradientSkill : MonoBehaviour
{

	public Gradient Gradient;
	private float timeMultiplier = 1.0f;
	public bool loop = false;

	//public bool resetTimeOnActivate;

	private Color curColor;
	private float time = 0.0f;

	private Renderer _Renderer;

	public float TimeMultiplier {
		set{ timeMultiplier = value; }
	}

	void Start ()
	{
		_Renderer = GetComponent<Renderer> ();
		_Renderer.material.SetColor ("_TintColor", new Color (0, 0, 0, 0));
	}

//	void OnEnable ()
//	{
//		if (resetTimeOnActivate == true)
//			time = 0.0f;
//
//	}

	void Update ()
	{
		time += Time.deltaTime * timeMultiplier;
		curColor = Gradient.Evaluate (time);

		_Renderer.material.SetColor ("_TintColor", curColor);

		if ((loop == true) && (time >= 1.0f))
			time -= 1.0f;
	}
}

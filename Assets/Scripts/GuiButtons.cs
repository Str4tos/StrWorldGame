using UnityEngine;
using System.Collections;

public class GuiButtons : MonoBehaviour {

	private StatusPlayer _StatusPlayer;
	// Use this for initialization
	void Start () {
		_StatusPlayer = GameObject.FindWithTag ("Player").GetComponent<StatusPlayer> ();
	}
	
	public void btnIncreaseStr(){
		_StatusPlayer.IncreaseStr ();
	}

	public void btnIncreaseAgi(){
		_StatusPlayer.IncreaseAgi ();
	}

	public void btnIncreaseVit(){
		_StatusPlayer.IncreaseVit ();
	}

	public void btnIncreaseEne(){
		_StatusPlayer.IncreaseEne ();
	}
}

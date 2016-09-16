using UnityEngine;
using System.Collections;

public class MenuPlayer : MonoBehaviour {

    public bool IsActive = false;

	// Use this for initialization
	void Start () {
        gameObject.SetActive(false);
	}

    public void ButnResume()
    {
        IsActive = !IsActive;
        gameObject.SetActive(IsActive);
        Time.timeScale = 1.0f - Time.timeScale;
    }

    public void ButnOptions()
    {

    }

    public void ButnExit()
    {
        SaveLoadPlayer _PlayerSaveLoad = GameObject.FindWithTag("Player").GetComponent<SaveLoadPlayer>();
        _PlayerSaveLoad.SavePlayerData();
        Application.Quit();
    }
}

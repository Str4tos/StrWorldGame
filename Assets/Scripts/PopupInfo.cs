using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupInfo : MonoBehaviour
{
	private string text;
	private int motion;
	private Color textColor;
	private Vector3 position;
	private Vector3 screanPointPosition;
	private GUIStyle styleText;
	private GUIStyle styleOutline;
	private int scaleIter;


	void Start ()
	{
		Font tempFont = (Font)Resources.Load ("Fonts/zeroes one");
		styleText = new GUIStyle ();
		styleText.normal.textColor = textColor;
		styleText.font = tempFont;
		styleText.alignment = TextAnchor.MiddleCenter;

		styleOutline = new GUIStyle ();
		styleOutline.normal.textColor = Color.black;
		styleOutline.font = tempFont;
		styleOutline.alignment = TextAnchor.MiddleCenter;


		styleText.fontSize = 20;
		styleOutline.fontSize = 21;



		screanPointPosition = Camera.main.WorldToScreenPoint (position);
		screanPointPosition.y -= 50;
		screanPointPosition.x -= Random.Range (0, 70);

		Destroy (gameObject, 1.0f);

	}

	public static void setText (string text, Transform _Transform)
	{
		setText (text, _Transform, -1, Color.white);
	}

	public static void setText (string text, Transform _Transform, int motion, Color textColor)
	{
		GameObject newInstance = new GameObject ("PopupText");
		var textPopup = newInstance.AddComponent<PopupInfo> ();
		textPopup.text = text;
		textPopup.position = _Transform.position;
		textPopup.textColor = textColor;
		textPopup.motion = motion;

			
	}

	void Update ()
	{
		screanPointPosition.y += motion;
//		if (critical) {
//			float timeAcc = Time.time + 0.2f;
//			if (timeAcc > Time.time) {
//				if (styleText.fontSize > 20) {
//					styleText.fontSize--;
//					styleOutline.fontSize--;
//				} else {
//					critical = false;
//				}
//			}
//		}
	}

	void OnGUI ()
	{
		GUI.Label (new Rect (screanPointPosition.x - 1, screanPointPosition.y + 1, 160, 40), text, styleOutline);
		GUI.Label (new Rect (screanPointPosition.x, screanPointPosition.y, 160, 40), text, styleText);
	}
	
}

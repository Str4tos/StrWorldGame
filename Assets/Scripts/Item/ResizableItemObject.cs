using UnityEngine;
using System.Collections;

public class ResizableItemObject : MonoBehaviour {

    void OnEnable()
    {
        Transform currTransform = transform;
        RectTransform currRect = currTransform.GetComponent<RectTransform>();
        RectTransform parentRect = currTransform.parent.GetComponent<RectTransform>();
        //currRect.sizeDelta = new Vector2(parentRect.sizeDelta.x, parentRect.sizeDelta.y);
        // IN PROGRESS
    }
	

}

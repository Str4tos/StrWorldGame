using UnityEngine;
using System.Collections.Generic;


namespace Utilities
{
    public delegate void MouseClickEventHandler(int button);

	class MouseUtility : MonoBehaviour
	{
        public static event MouseClickEventHandler MouseClick;

        void Update()
        {
            if (MouseClick != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    MouseClick(0);
                } 
                if (Input.GetMouseButtonDown(1))
                {
                    MouseClick(1);
                }
            }
        }

        public static bool IsMouseIn(Rect rectangle)
        {
            if (Input.mousePosition.x >= rectangle.x && Input.mousePosition.y >= rectangle.y && Input.mousePosition.y <= rectangle.y + rectangle.height && Input.mousePosition.x <= rectangle.x + rectangle.width)
                return true;
            return false;
        }
	}
}

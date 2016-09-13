using UnityEngine;
using System.Collections;

namespace Utilities
{
	public class GUIIcon : MonoBehaviour
	{
		public GUITexture icon;
        public GUITexture overlay;
        public GUIText shortcut;
        public Skill skill;

        public float CooldownActiveSince = 0;

        bool mouseOver = false;
        float mouseOverTime = 0;

        void Start()
        {

        }

        public void Initialize()
        {
            icon = GetComponent<GUITexture>();
            overlay = GetComponentsInChildren<GUITexture>()[1];

            shortcut = GetComponentInChildren<GUIText>();
        }

        void Update()
        {
            if (overlay.enabled)
            {
                 overlay.pixelInset = new Rect(overlay.pixelInset.x, overlay.pixelInset.y, overlay.pixelInset.width, overlay.pixelInset.height - Time.deltaTime * 70 / skill.CoolDown);

                if (overlay.pixelInset.height <= 0)
                {
                    overlay.enabled = false;
                    overlay.pixelInset = new Rect(overlay.pixelInset.x, overlay.pixelInset.y, overlay.pixelInset.width, 70);
                }
            }

            CooldownActiveSince += Time.deltaTime;


            // Can't use MouseEnter and Exit as the overlay messes with the detection
            if (!MouseUtility.IsMouseIn(icon.pixelInset))
            {
                mouseOver = false;
                mouseOverTime = 0;
            }
            else
            {
                mouseOver = true;
            }
        }

        public void StartCooldown()
        {
            overlay.enabled = true;
        }
	}
}


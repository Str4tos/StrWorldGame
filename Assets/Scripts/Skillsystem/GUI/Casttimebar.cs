using UnityEngine;
using System.Collections;


    public class Casttimebar : MonoBehaviour
    {
        static float casttime = 0;
        static float elapsedTime = 0;

        public Texture Bar;
        public Texture Mark;
        public Texture Border;

        const int MARKMAXX = 288;
        const int BARMAXWIDTH = 294;

        int markCurX = 0;
        int barCurWidth = 0;
        GUIStyle style = new GUIStyle();

        static private bool isChanneled = false;

        // Use this for initialization
        void Start()
        {
            style.fontSize = 17;
        }

        // Update is called once per frame
        void Update()
        {
            if (isChanneled)
            {
                // Makes the casttimebar run backwards
                if (elapsedTime > 0)
                {
                    markCurX = (int)((float)MARKMAXX * (elapsedTime / casttime));
                    barCurWidth = (int)((float)BARMAXWIDTH * (elapsedTime / casttime));
                    elapsedTime -= Time.deltaTime;
                }
                else
                {
                    elapsedTime = 0;
                }
            }
            else
            {
                if (elapsedTime < casttime)
                {
                    markCurX = (int)((float)MARKMAXX * (elapsedTime / casttime));
                    barCurWidth = (int)((float)BARMAXWIDTH * (elapsedTime / casttime));
                    elapsedTime += Time.deltaTime;
                }
                else
                {
                    elapsedTime = casttime;
                }
            }
        }

        /// <summary>
        /// Activates the casttimebar with the specified casttime. If casttime is 0, activation is ignored.
        /// </summary>
        /// <param name='Casttime'>
        /// Casttime to be shown.
        /// </param>
        public static void ActivateCasttime(float Casttime)
        {
            if (Casttime == 0)
                return;

            casttime = Casttime;
            elapsedTime = 0;
            isChanneled = false;
        }

        /// <summary>
        /// Activates the casttimebar for a channeled skill, meaning it starts full and decreases until it reaches 0. If ChannelTime is 0, activation is ignored.
        /// </summary>
        /// <param name="ChannelTime">Time to be shown</param>
        public static void ActivateChannelTime(float ChannelTime)
        {
            if (ChannelTime == 0)
                return;

            casttime = ChannelTime;
            elapsedTime = ChannelTime;
            isChanneled = true;
        }

        /// <summary>
        /// Aborts all kinds of casttimebars
        /// </summary>
        public static void Abort()
        {
            casttime = 0;
        }

        void OnGUI()
        {
            if (elapsedTime < casttime)
            {
                GUI.BeginGroup(new Rect(Screen.width / 2 - 150, Screen.height - 200, 300, 80));

                GUI.DrawTexture(new Rect(0, 25, 300, 30), Border);
                GUI.DrawTextureWithTexCoords(new Rect(3, 27, barCurWidth, 28), Bar, new Rect(0, 0, (float)barCurWidth / (float)BARMAXWIDTH, 1));
                GUI.DrawTexture(new Rect(markCurX - 3, 0, 20, 80), Mark); // Highlight 

                // Show the leftover casttime
                GUI.Label(new Rect(142, 30, 100, 40), (casttime - (casttime - elapsedTime)).ToString("F2"), style);

                GUI.EndGroup();
            }
        

    }
}

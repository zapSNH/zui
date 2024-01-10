using System.IO;
using UnityEngine;

namespace ZUI
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class AdaptiveNavball : MonoBehaviour
    {
        internal static AdaptiveNavball instance;
        internal UrlDir.UrlConfig[] navballConfigs;
        private string[] navballPaths = new string[3];
        private bool[] navballExists = new bool[3];

        public void Start()
        {
            instance = this;
            navballConfigs = GameDatabase.Instance.GetConfigs("ZUINavBall");
            foreach (UrlDir.UrlConfig config in navballConfigs)
            {
                if (config.config.HasValue("navball_surface"))
                {
                    navballPaths[0] = config.config.GetValue("navball_surface");
                    navballExists[0] = true;
                }
                if (config.config.HasValue("navball_orbit"))
                {
                    navballPaths[1] = config.config.GetValue("navball_orbit");
                    navballExists[1] = true;
                }
                if (config.config.HasValue("navball_target"))
                {
                    navballPaths[2] = config.config.GetValue("navball_target");
                    navballExists[2] = true;
                }
            }
            Debug.Log("[ZUI] navballExists: Surface: " + navballExists[0] + ", Orbit: " + navballExists[1] + ", Target: " + navballExists[2]);
            Debug.Log("[ZUI] navballPaths: Surface: " + navballPaths[0] + ", Orbit: " + navballPaths[1] + ", Target:" + navballPaths[2]);
            ChangeNavball(new FlightGlobals.SpeedDisplayModes());
            GameEvents.onSetSpeedMode.Add(ChangeNavball);
        }

        internal void ChangeNavball(FlightGlobals.SpeedDisplayModes speedMode)
        {
            foreach (Texture2D tex in (Texture2D[])(object)Resources.FindObjectsOfTypeAll(typeof(Texture2D)))
            {
                if (tex.name == "NavBall")
                {
                    Debug.Log("[ZUI] Switching Navball mode to " + FlightGlobals.speedDisplayMode);
                    switch (FlightGlobals.speedDisplayMode.ToString())
                    {
                        case "Surface":
                            if (navballExists[0])
                            {
                                ImageConversion.LoadImage(tex, File.ReadAllBytes(KSPUtil.ApplicationRootPath + navballPaths[0]));
                            }
                            break;
                        case "Orbit":
                            if (navballExists[1])
                            {
                                ImageConversion.LoadImage(tex, File.ReadAllBytes(KSPUtil.ApplicationRootPath + navballPaths[1]));
                            }
                            break;
                        case "Target":
                            if (navballExists[2])
                            {
                                ImageConversion.LoadImage(tex, File.ReadAllBytes(KSPUtil.ApplicationRootPath + navballPaths[2]));
                            }
                            break;
                    }
                }
            }
        }

        public void OnDisable()
        {
            GameEvents.onSetSpeedMode.Remove(ChangeNavball);
        }
    }
}
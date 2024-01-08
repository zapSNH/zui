using System.IO;
using UnityEngine;

namespace ZUI
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class AdaptiveNavball : MonoBehaviour
    {
        private static string pluginDataPath = "GameData/ZUI/PluginData/";
        public void Start()
        {
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
                            ImageConversion.LoadImage(tex, File.ReadAllBytes(KSPUtil.ApplicationRootPath + pluginDataPath + "navball_surface.png"));
                            break;
                        case "Orbit":
                            ImageConversion.LoadImage(tex, File.ReadAllBytes(KSPUtil.ApplicationRootPath + pluginDataPath + "navball_orbit.png"));
                            break;
                        case "Target":
                            ImageConversion.LoadImage(tex, File.ReadAllBytes(KSPUtil.ApplicationRootPath + pluginDataPath + "navball_target.png"));
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
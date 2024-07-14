using System.IO;
using UnityEngine;

namespace ZUI {
	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class AdaptiveNavball : MonoBehaviour {
		internal static AdaptiveNavball instance;
		internal UrlDir.UrlConfig[] navballConfigs;
		private string[] navballPaths = new string[] { null, null, null };
		private bool[] navballExists = new bool[3];
		private Texture2D navballTexture;
		private bool enableAdaptiveNavball = true;

		private const string ZUINAVBALL_NODE = "ZUINavBall";
		private const string ADAPTIVE_NAVBALL_ENABLED_CFG = "enabled";
		private const string NAVBALL_SURFACE = "navballSurface";
		private const string NAVBALL_ORBIT = "navballOrbit";
		private const string NAVBALL_TARGET = "navballTarget";
		private const string NAVBALL_TEXTURE = "NavBall";

		public void Start() {
			instance = this;
			LoadConfigs();
			if (!enableAdaptiveNavball) return;
			foreach (Texture2D tex in (Texture2D[])(object)Resources.FindObjectsOfTypeAll(typeof(Texture2D))) {
				if (tex.name == NAVBALL_TEXTURE) {
					navballTexture = tex;
					Debug.Log("[ZUI] Found NavBall texture!");
					break;
				}
			}

			Debug.Log("[ZUI] NavBall Texture Paths: Surface: " + (navballPaths[0] ?? "None") + " | Orbit: " + (navballPaths[1] ?? "None") + " | Target:" + (navballPaths[2] ?? "None"));
			ChangeNavball(new FlightGlobals.SpeedDisplayModes());
			GameEvents.onSetSpeedMode.Add(ChangeNavball);
		}
		private void LoadConfigs() {
			navballConfigs = GameDatabase.Instance.GetConfigs(ZUINAVBALL_NODE);
			foreach (UrlDir.UrlConfig config in navballConfigs) {
				if (config.config.HasValue(NAVBALL_SURFACE)) {
					Debug.Log("[ZUI] " + config.config.GetValue(NAVBALL_SURFACE));
					navballPaths[0] = config.config.GetValue(NAVBALL_SURFACE);
					navballExists[0] = true;
				}
				if (config.config.HasValue(NAVBALL_ORBIT)) {
					navballPaths[1] = config.config.GetValue(NAVBALL_ORBIT);
					navballExists[1] = true;
				}
				if (config.config.HasValue(NAVBALL_TARGET)) {
					navballPaths[2] = config.config.GetValue(NAVBALL_TARGET);
					navballExists[2] = true;
				}
				if (config.config.HasValue(ADAPTIVE_NAVBALL_ENABLED_CFG)) {
					config.config.TryGetValue(ADAPTIVE_NAVBALL_ENABLED_CFG, ref enableAdaptiveNavball);
				}
			}
		}

		internal void ChangeNavball(FlightGlobals.SpeedDisplayModes speedMode) {
			Debug.Log("[ZUI] Switching Navball mode to " + FlightGlobals.speedDisplayMode);
			switch (FlightGlobals.speedDisplayMode) {
				case FlightGlobals.SpeedDisplayModes.Surface:
					if (navballExists[0]) {
						ImageConversion.LoadImage(navballTexture, File.ReadAllBytes(KSPUtil.ApplicationRootPath + navballPaths[0]));
					}
					break;
				case FlightGlobals.SpeedDisplayModes.Orbit:
					if (navballExists[1]) {
						ImageConversion.LoadImage(navballTexture, File.ReadAllBytes(KSPUtil.ApplicationRootPath + navballPaths[1]));
					}
					break;
				case FlightGlobals.SpeedDisplayModes.Target:
					if (navballExists[2]) {
						ImageConversion.LoadImage(navballTexture, File.ReadAllBytes(KSPUtil.ApplicationRootPath + navballPaths[2]));
					}
					break;
			}
		}

		public void OnDisable() {
			GameEvents.onSetSpeedMode.Remove(ChangeNavball);
		}
	}
}

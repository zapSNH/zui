using System.IO;
using UnityEngine;

namespace ZUI {
	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class AdaptiveNavball : MonoBehaviour {
		internal static AdaptiveNavball Instance { get; private set; }
		internal ConfigNode[] navballConfigs;
		private string[] navballPaths = new string[] { null, null, null };
		private bool[] navballExists = new bool[3];
		private Texture2D navballTexture;
		private bool enableAdaptiveNavball = true;

		public void Start() {
			Instance = this;
			LoadConfigs();
			if (!enableAdaptiveNavball) return;
			foreach (Texture2D tex in (Texture2D[])(object)Resources.FindObjectsOfTypeAll(typeof(Texture2D))) {
				if (tex.name == Constants.NAVBALL_TEXTURE) {
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
			UrlDir.UrlConfig[] ZUINodes = GameDatabase.Instance.GetConfigs(Constants.ZUI_NODE);
			foreach (UrlDir.UrlConfig node in ZUINodes) {
				navballConfigs = node.config.GetNodes(Constants.ZUINAVBALL_NODE);
				foreach (ConfigNode config in navballConfigs) {
					if (config.HasValue(Constants.NAVBALL_SURFACE)) {
						Debug.Log("[ZUI] " + config.GetValue(Constants.NAVBALL_SURFACE));
						navballPaths[0] = config.GetValue(Constants.NAVBALL_SURFACE);
						navballExists[0] = true;
					}
					if (config.HasValue(Constants.NAVBALL_ORBIT)) {
						navballPaths[1] = config.GetValue(Constants.NAVBALL_ORBIT);
						navballExists[1] = true;
					}
					if (config.HasValue(Constants.NAVBALL_TARGET)) {
						navballPaths[2] = config.GetValue(Constants.NAVBALL_TARGET);
						navballExists[2] = true;
					}
					if (config.HasValue(Constants.ADAPTIVE_NAVBALL_ENABLED_CFG)) {
						config.TryGetValue(Constants.ADAPTIVE_NAVBALL_ENABLED_CFG, ref enableAdaptiveNavball);
					}
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

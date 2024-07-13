using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZUI
{
	[KSPAddon(KSPAddon.Startup.EveryScene, false)]
	public class ConfigManager : MonoBehaviour
	{
		internal static ConfigManager instance;

		private List<Config> currentConfigs = new List<Config>();

		private const string ZUI_CFG = "ZUI";

		private const string ZUICONFIG_CFG = "ZUIConfig";
		private const string ZUICONFIGNAME_CFG = "name";
		private const string ZUICONFIGOPTIONS_CFG = "ZUIConfigOptions";

		private const string HUDREPLACER_LOAD_CFG = "hudreplacer_load";
		private const string HUDREPLACER_LOAD_PRIORITY_CFG = "hudreplacer_load_priority";

		public void Start()
		{
			instance = this;
			UrlDir.UrlConfig[] ZUISettings = GameDatabase.Instance.GetConfigs(ZUICONFIG_CFG);
			foreach (UrlDir.UrlConfig urlConfig in ZUISettings) { 
				if (!urlConfig.config.HasValue(ZUICONFIGNAME_CFG)) {
					Debug.Log("[ZUI] Config does not have a name!");
					continue;
				}
				Config config = new Config(urlConfig.config);
				currentConfigs.Add(config);
				Debug.Log(currentConfigs[currentConfigs.Count - 1].name);
			}
		}
	}
}
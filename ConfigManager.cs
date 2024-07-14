using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZUI {
	// todo: make ddol and test
	[KSPAddon(KSPAddon.Startup.EveryScene, false)]
	public class ConfigManager : MonoBehaviour {
		internal static ConfigManager instance;

		private static List<Config> currentConfigs = new List<Config>();
		private static List<Config> enabledConfigs = new List<Config>();
		private static List<ConfigNode> currentConfigNodes = new List<ConfigNode>();

		private const string OPTIONS_SAVE_LOCATION = "GameData/999_ZUI/Config/options.cfg";

		private const string ZUI_NODE = "ZUI";

		private const string ZUICONFIG_NODE = "ZUIConfig";
		private const string ZUICONFIGNAME_VALUE = "name";
		private const string ZUICONFIGOPTIONS_NODE = "ZUIConfigOptions";
		private const string ZUICONFIGOPTIONENABLED_CFG = "enabled";

		public void Awake() {
			if (instance == null) {
				instance = this;
			} else {
				Destroy(gameObject);
				return;
			}

			LoadConfigs();
			SetConfigs();
		}
		private void LoadConfigs() {
			UrlDir.UrlConfig[] ZUINodes = GameDatabase.Instance.GetConfigs(ZUI_NODE);
			foreach (UrlDir.UrlConfig URLConfig in ZUINodes) {

				Debug.Log($"[ZUI] Loading configs from {URLConfig.url}");
				ConfigNode[] ZUIConfigs = URLConfig.config.GetNodes(ZUICONFIG_NODE);
				foreach (ConfigNode ZUIURLConfig in ZUIConfigs) {
					if (!ZUIURLConfig.HasValue(ZUICONFIGNAME_VALUE)) {
						Debug.Log("[ZUI] Config does not have a name!");
						continue;
					}

					Config config = new Config(ZUIURLConfig);
					if (currentConfigs.Exists(c => c.name == config.name)) {
						Debug.Log($"[ZUI] Discarding duplicate of the config '{config.name}'. Please make sure that all configs have unique names.");
						continue;
					}
					currentConfigs.Add(config);
				}

				ConfigNode[] ZUIConfigOptions = URLConfig.config.GetNodes(ZUICONFIGOPTIONS_NODE);
				foreach (ConfigNode ZUIConfigOption in ZUIConfigOptions) {
					if (!ZUIConfigOption.HasValue(ZUICONFIGOPTIONENABLED_CFG)) {
						Debug.Log($"[ZUI] Config option does not have '{ZUICONFIGOPTIONENABLED_CFG}'. There is nothing to enable.");
						continue;
					}
					string[] ZUIConfigOptionValues = ZUIConfigOption.GetValue(ZUICONFIGOPTIONENABLED_CFG).Replace(" ", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
					foreach (string configValue in ZUIConfigOptionValues) {
						if (currentConfigs.Exists(c => c.name == configValue)) {
							EnableConfig(currentConfigs.Find(c => c.name == configValue));
						} else {
							Debug.Log($"[ZUI] '{configValue}' does not exist.");
						}
					}
				}
			}
		}
		internal static void EnableConfig(Config config) {
			if (!enabledConfigs.Contains(config)) {
				enabledConfigs.Add(config);
			}
		}
		internal static bool DisableConfig(Config config) {
			return enabledConfigs.Remove(config);
		}
		private static void AddConfig(Config config) {
			if (config.HasHUDReplacerNode) {
				ConfigNode[] ConfigNodes = config.GetConfigNodesAsHUDReplacerNodes();
				foreach (var configNode in ConfigNodes) {
					currentConfigNodes.Add(configNode);
				}
			}
			if (config.HasRecolorNode) {
				ConfigNode[] recolorConfigNodes = config.GetRecolorConfigNodesAsHUDReplacerRecolorNodes();
				foreach (var configNode in recolorConfigNodes) {
					currentConfigNodes.Add(configNode);
				}
			}
		}
		internal static void SetConfigs() {
			foreach (Config config in enabledConfigs) { 
				AddConfig(config);
			}
			ConfigNode optionsFile = new ConfigNode();
			foreach (ConfigNode configNode in currentConfigNodes) {
				optionsFile.AddNode(configNode);
			}
			optionsFile.Save(KSPUtil.ApplicationRootPath + OPTIONS_SAVE_LOCATION, "ZUI Options. Any changes to this file may be overwritten by ZUI, use config.cfg instead.");
			GameDatabase.CompileConfig(optionsFile);
		}
		internal static List<Config> GetConfigs() {
			return currentConfigs;
		}
		internal static List<Config> GetEnabledConfigs() {
			return enabledConfigs;
		}
	}
}
using System.Collections.Generic;
using UnityEngine;

namespace ZUI
{
	// TODO: convert hudReplacerLoad into a separate node to allow for multiple folders to be loaded in one node
	public class Config
	{
		[Persistent] public string name;
		[Persistent] public string hudReplacerLoad;
		[Persistent] public string hudReplacerLoadPriority;
		public bool HasRecolorNode { 
			get {
				if (configNode != null) {
					return configNode.HasNode(ZUIHUDREPLACER_RECOLOR_NODE);
				} else {
					return false;
				}
			}
		}
		public ConfigNode configNode;

		private const string ZUIHUDREPLACER_RECOLOR_NODE = "ZUIHUDReplacerRecolor";
		private const string HUDREPLACER_PRIORITY_CFG = "priority";

		public Config(ConfigNode configNode) {
			this.configNode = configNode;
			ConfigNode.LoadObjectFromConfig(this, configNode);
		}
		// TODO: finish up recoloring and figure out how to fit priority here
		// why not make a new ConfigNode
		public Dictionary<string, Color> GetConfigColors() {
			Dictionary<string, Color> configColors = new Dictionary<string, Color>();
			ConfigNode[] recolorNodes = configNode.GetNodes(ZUIHUDREPLACER_RECOLOR_NODE);
			foreach (ConfigNode recolorNode in recolorNodes) {
				foreach (ConfigNode.Value value in recolorNode.values) {
					string[] colorStr = value.value.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
					if (colorStr.Length < 4) {
						Debug.Log("[ZUI] Invalid color!");
						continue;
					}
					string recolorTarget = value.name;
					Color color = new Color(float.Parse(colorStr[0]), float.Parse(colorStr[1]), float.Parse(colorStr[2]), float.Parse(colorStr[3]));
					configColors.Add(recolorTarget, color);
				}
			}
			return configColors;
		}
		public int[] GetConfigRecolorPriorities() {
			List<int> priorities = new List<int>();
			ConfigNode[] recolorNodes = configNode.GetNodes(ZUIHUDREPLACER_RECOLOR_NODE);
			foreach (ConfigNode recolorNode in recolorNodes) {
				int priority = 1;
				int.TryParse(recolorNode.GetValue(HUDREPLACER_PRIORITY_CFG), out priority);
				priorities.Add(priority);
			}
			return priorities.ToArray();
		}
	}
}
using System;
using System.Collections.Generic;
namespace ZUI
{
	public class Config
	{
		public string name;
		public string hudReplacerLoad;
		public string hudReplacerLoadPriority;

		public Config(ConfigNode configNode) {
			ConfigNode.LoadObjectFromConfig(this, configNode);
		}
	}
}
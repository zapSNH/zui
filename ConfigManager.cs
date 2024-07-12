using System;
using UnityEngine;

// Partially based on wheeeUI and Visual Studio's autocomplete feature
namespace ZUI
{
	[KSPAddon(KSPAddon.Startup.EveryScene, false)]
	public class ConfigManager : MonoBehaviour
	{
		internal static ConfigManager instance;
		internal UrlDir.UrlConfig[] transforms;

		public void Start()
		{
			instance = this;
		}
	}
}
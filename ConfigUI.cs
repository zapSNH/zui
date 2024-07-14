using System.Collections.Generic;
using UnityEngine;

namespace ZUI {
	[KSPAddon(KSPAddon.Startup.MainMenu, false)]
	internal static class ConfigUI {
		private static float windowWidth = 250;
		private static float windowHeight = 500;
		private static float buttonHeight = 24;

		private static int paddingBase = 4;
		private static int paddingSmall = 1 * paddingBase;
		private static int paddingRegular = 2 * paddingBase;
		private static int paddingLarge = 3 * paddingBase;
		private static int paddingWindow = 4 * paddingBase;

		internal static void ShowPopup() {
			// create dialog to attach elements to
			List<DialogGUIBase> dialog = new List<DialogGUIBase>();

			// add zuiconfigs
			List<DialogGUIToggleButton> buttons = new List<DialogGUIToggleButton>();
			List<Config> configs = ConfigManager.GetConfigs();
			foreach (Config config in configs) {
				DialogGUIToggleButton button = new DialogGUIToggleButton(ConfigManager.GetEnabledConfigs().Contains(config),
					config.name.CamelCaseToHumanReadable(),
					delegate (bool selected) {
						ToggleConfig(selected, config);
					},
					windowWidth - (2 * paddingRegular), buttonHeight);
				buttons.Add(button);
			}

			// attach configs to list
			DialogGUIScrollList scrollList = new DialogGUIScrollList(Vector2.one,
				false,
				true,
				new DialogGUIVerticalLayout(windowWidth - (2 * paddingWindow), 64, paddingRegular, new RectOffset(paddingRegular, paddingRegular, paddingRegular, paddingRegular), TextAnchor.MiddleLeft, buttons.ToArray()));

			PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
				new MultiOptionDialog("ZUI Config Options",
					"",
					"ZUI Config Options",
					HighLogic.UISkin,
					new Rect(0.5f, 0.5f, windowWidth, windowHeight),
					scrollList),
				false, HighLogic.UISkin, false);
		}
		private static void ToggleConfig(bool selected, Config config) {
			Debug.Log($"[ZUI] toggling {config.name} to {selected}");
			if (selected) {
				ConfigManager.EnableConfig(config);
			} else {
				ConfigManager.DisableConfig(config);
			}
			ConfigManager.SetConfigs();
		}
	}
}
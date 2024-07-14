using System.Collections.Generic;
using UnityEngine;

namespace ZUI {
	internal static class ConfigUI {
		private static float windowWidth = 250;
		private static float windowHeight = 300;
		private static float buttonHeight = 24;

		private static float paddingBase = 4;
		private static float paddingXSmall = 0.5f * paddingBase;
		private static float paddingSmall = 1 * paddingBase;
		private static float paddingRegular = 2 * paddingBase;
		private static float paddingLarge = 3 * paddingBase;
		private static float paddingWindow = 4 * paddingBase;

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
				new DialogGUIVerticalLayout(windowWidth - (2 * paddingWindow) - (2 * paddingXSmall), 64, paddingXSmall, new RectOffset((int)paddingXSmall, (int)paddingXSmall, (int)paddingXSmall, (int)paddingXSmall), TextAnchor.MiddleLeft, buttons.ToArray()));

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
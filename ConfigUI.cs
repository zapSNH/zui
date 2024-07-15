using System.Collections.Generic;
using UnityEngine;

namespace ZUI {
	// TODO: l10n
	internal static class ConfigUI {
		internal static PopupDialog popupDialog;

		internal enum ZUITab {
			Configuration,
			OtherSettings
		}

		internal static ZUITab currentTab = ZUITab.Configuration;

		private static float windowWidth = 250;
		private static float windowHeight = 250;
		private static float buttonHeight = 24;

		private static float paddingBase = 4;
		private static float paddingXSmall = 0.5f * paddingBase;
		private static float paddingSmall = 1 * paddingBase;
		private static float paddingRegular = 2 * paddingBase;
		private static float paddingLarge = 3 * paddingBase;
		private static float paddingWindow = 4 * paddingBase;

		internal static void TogglePopup() {
			if (popupDialog == null) {
				ShowPopup();
			} else {
				popupDialog.Dismiss();
				popupDialog = null;
			}
		}

		internal static void ShowPopup() {
			// create dialog to attach elements to
			List<DialogGUIBase> dialog = new List<DialogGUIBase>();

			// add zuiconfigs
			List<DialogGUIToggleButton> buttons = new List<DialogGUIToggleButton>();
			List<ZUIConfig> configs = ConfigManager.GetConfigs();
			foreach (ZUIConfig config in configs) {
				DialogGUIToggleButton button = new DialogGUIToggleButton(ConfigManager.GetEnabledConfigs().Contains(config),
					config.name.CamelCaseToHumanReadable(),
					delegate (bool selected) {
						ToggleConfig(selected, config);
					},
					windowWidth - (2 * paddingWindow) - (2 * paddingXSmall), buttonHeight);
				buttons.Add(button);
			}

			// attach configs to list
			DialogGUIScrollList scrollList = new DialogGUIScrollList(Vector2.one,
				false,
				true,
				new DialogGUIVerticalLayout(windowWidth - (2 * paddingWindow) - (2 * paddingXSmall), 64, paddingXSmall, new RectOffset((int)paddingXSmall, (int)paddingXSmall, (int)paddingXSmall, (int)paddingXSmall), TextAnchor.MiddleLeft, buttons.ToArray()));

			// tab buttons
			DialogGUIToggleButton configTab = new DialogGUIToggleButton(() => currentTab == ZUITab.Configuration,
				"Configuration",
				delegate (bool selected) {
					SetTab(ZUITab.Configuration);
				},
				(windowWidth / 2) - (2 * paddingSmall), buttonHeight
			);
			DialogGUIToggleButton otherSettingsTab = new DialogGUIToggleButton(() => currentTab == ZUITab.OtherSettings,
				"Other Settings",
				delegate (bool selected) {
					SetTab(ZUITab.OtherSettings);
				},
				(windowWidth / 2) - (2 * paddingSmall), buttonHeight
			);

			// tab container
			DialogGUIHorizontalLayout tabContainer = new DialogGUIHorizontalLayout(TextAnchor.MiddleCenter, configTab, otherSettingsTab);

			// ui container 
			//DialogGUIVerticalLayout UIContainer = new DialogGUIVerticalLayout(windowWidth - (2 * paddingWindow) - (2 * paddingXSmall), windowHeight - (2 * paddingWindow),
			//	tabContainer, scrollList);

			popupDialog = PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
				new MultiOptionDialog("ZUI Config Options",
					"",
					"ZUI Config Options",
					HighLogic.UISkin,
					new Rect(0.5f, 0.5f, windowWidth, windowHeight),
					tabContainer, scrollList),
				false, HighLogic.UISkin, false);
		}
		private static void ToggleConfig(bool selected, ZUIConfig config) {
			Debug.Log($"[ZUI] toggling {config.name} to {selected}");
			if (selected) {
				ConfigManager.EnableConfig(config);
			} else {
				if (ConfigManager.DisableConfig(config)) {
					Debug.Log("[ZUI] Successfully removed");
				} else {
					Debug.Log("[ZUI] Unable to remove!");
				}
			}
			ConfigManager.SaveConfigOverrides();
			ConfigManager.SetConfigs();
		}
		private static void SetTab(ZUITab tab) {
			if (tab == currentTab) return;
			currentTab = tab;
			Debug.Log($"[ZUI] current tab: {tab}");
		}
	}
}
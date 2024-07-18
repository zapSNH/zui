﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ZUI {
	// TODO: l10n
	internal static class ConfigUI {
		internal static PopupDialog popupDialog;
		internal static PopupDialog applyNotice = null;
		internal static DialogGUIBase configPage;
		internal static DialogGUIBase settingsPage;

		internal static DialogGUIBox currentPage;

		internal enum ZUITab {
			Configuration,
			OtherSettings
		}

		internal static ZUITab currentTab = ZUITab.Configuration;

		private static float windowWidth = 250;
		private static float windowHeight = 250;
		private static float buttonHeight = 24;

		private static float paddingBase = 3;
		private static float paddingXSmall = 0.5f * paddingBase;
		private static float paddingSmall = 1 * paddingBase;
		private static float paddingRegular = 2 * paddingBase;
		private static float paddingLarge = 3 * paddingBase;
		private static float paddingWindow = 4 * paddingBase;

		private static List<ZUIConfig> changedConfigs = new List<ZUIConfig>();

		internal static void TogglePopup() {
			if (popupDialog == null) {
				ShowPopup();
			} else {
				popupDialog.Dismiss();
				popupDialog = null;
			}
		}

		// some gui stuff from https://github.com/neuoy/KSPTrajectories/blob/master/src/Plugin/Display/MainGUI.cs
		internal static void ShowPopup() {
			// create dialog to attach elements to
			List<DialogGUIBase> dialog = new List<DialogGUIBase>();

			// Config Page
			// add zuiconfigs
			List<DialogGUIBase> buttons = new List<DialogGUIBase>();
			List<ZUIConfig> configs = ConfigManager.GetConfigs();
			buttons.Add(new DialogGUIContentSizer(ContentSizeFitter.FitMode.Unconstrained, ContentSizeFitter.FitMode.PreferredSize, true));
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
				new DialogGUIVerticalLayout(windowWidth - (2 * paddingWindow) - (2 * paddingXSmall), 64,
				paddingXSmall,
				new RectOffset((int)paddingXSmall, (int)paddingXSmall, (int)paddingXSmall, (int)paddingXSmall),
				TextAnchor.MiddleLeft,
				buttons.ToArray()));

			// apply button
			DialogGUIButton applyButton = new DialogGUIButton("Apply",
				ApplyConfigWindow,
				windowWidth - (2 * paddingRegular), buttonHeight, false);

			configPage = new DialogGUIVerticalLayout(true, false, 0, new RectOffset(), TextAnchor.UpperCenter, scrollList, applyButton);

			// Other Settings Page
			
			settingsPage = new DialogGUIVerticalLayout(true, false, 0, new RectOffset(), TextAnchor.UpperCenter, applyButton);

			// tab buttons
			DialogGUIToggleButton configTab = new DialogGUIToggleButton(() => currentTab == ZUITab.Configuration,
				"Configuration",
				delegate (bool selected) {
					if (selected) SetTab(ZUITab.Configuration);
				},
				(windowWidth / 2) - (2 * paddingRegular), buttonHeight
			);
			DialogGUIToggleButton otherSettingsTab = new DialogGUIToggleButton(() => currentTab == ZUITab.OtherSettings,
				"Other Settings",
				delegate (bool selected) {
					if (selected) SetTab(ZUITab.OtherSettings);
				},
				(windowWidth / 2) - (2 * paddingRegular), buttonHeight
			); 

			// tab container
			DialogGUIHorizontalLayout tabContainer = new DialogGUIHorizontalLayout(TextAnchor.MiddleCenter, configTab, otherSettingsTab);

			DialogGUIBox configPageBox = new DialogGUIBox(null, -1, -1, () => currentTab == ZUITab.Configuration, configPage);
			DialogGUIBox settingsPageBox = new DialogGUIBox(null, -1, -1, () => currentTab == ZUITab.OtherSettings, settingsPage);
			/*switch (currentTab) {
				case ZUITab.Configuration:
					currentPage.children.Add(configPage);
					break;
				case ZUITab.OtherSettings:
				default:
					currentPage.children.Add(settingsPage);
					break;
			}*/

			popupDialog = PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
				new MultiOptionDialog("ZUIConfigWindow",
					"",
					"ZUI Config Optionss",
					HighLogic.UISkin,
					new Rect(0.5f, 0.5f, windowWidth, windowHeight),
					new DialogGUIBase[] { tabContainer, configPageBox, settingsPageBox } ),
				false, HighLogic.UISkin, false);
		}
		private static void ToggleConfig(bool selected, ZUIConfig config) {
			Debug.Log($"[ZUI] toggling {config.name} to {selected}");
			changedConfigs.Add(config);
			if (selected) {
				ConfigManager.EnableConfig(config);
			} else {
				ConfigManager.DisableConfig(config);
			}
		}
		private static void ApplyConfigWindow() {
			if (changedConfigs.Exists(c => c.requireSceneReload || c.requireRestart)) {
				List<string> requireSceneReload = changedConfigs.Where(c => c.requireSceneReload).ToList().ConvertAll(c => c.name.CamelCaseToHumanReadable());
				List<string> requireRestart = changedConfigs.Where(c => c.requireRestart).ToList().ConvertAll(c => c.name.CamelCaseToHumanReadable());
				List<DialogGUIBase> container = new List<DialogGUIBase>();
				DialogGUIButton okButton = new DialogGUIButton("OK", ApplyConfigs, windowWidth - (2 * paddingRegular), buttonHeight, true);
				if (requireSceneReload.Count != 0) {
					DialogGUILabel needSceneReloadBox = new DialogGUILabel("need a scene reload to fully apply:\n\t" + string.Join("\n\t", requireSceneReload), windowWidth - (2 * paddingWindow) - (2 * paddingRegular));
					container.Add(needSceneReloadBox);
				}
				if (requireRestart.Count != 0) {
					DialogGUILabel needRestartBox = new DialogGUILabel("need a game restart to fully apply:\n\t" + string.Join("\n\t", requireRestart), windowWidth - (2 * paddingWindow) - (2 * paddingRegular));
					container.Add(needRestartBox);
				}
				container.Add(okButton);
				DialogGUIVerticalLayout dialog = new DialogGUIVerticalLayout(container.ToArray());
				if (applyNotice != null) {
					applyNotice.Dismiss();
					applyNotice = null;
				}
				changedConfigs.Clear();
				applyNotice = PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
					new MultiOptionDialog("ApplyConfigWindow",
						"The following configs...",
						"Information",
						HighLogic.UISkin,
						new Rect(0.5f, 0.5f, windowWidth, -1),
						new DialogGUIContentSizer(ContentSizeFitter.FitMode.Unconstrained, ContentSizeFitter.FitMode.PreferredSize, true),
						dialog),
					false, HighLogic.UISkin, true);
			} else {
				ApplyConfigs();
			}
		}
		private static void ApplyConfigs() {
			if (applyNotice != null) {
				applyNotice.Dismiss();
				applyNotice = null;
			}
			PopupDialog reloadDialog = PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
				new MultiOptionDialog("ReloadWindow",
					"Please wait...",
					"Loading configs",
					HighLogic.UISkin,
					new Rect(0.5f, 0.5f, windowWidth, -1),
					new DialogGUIContentSizer(ContentSizeFitter.FitMode.Unconstrained, ContentSizeFitter.FitMode.PreferredSize, true)),
				false, HighLogic.UISkin, true);
			ConfigManager.SaveConfigOverrides();
			ConfigManager.SetConfigs();
			reloadDialog.Dismiss();
		}
		private static void SetTab(ZUITab tab) {
			if (tab == currentTab) return;
			currentTab = tab;
			Debug.Log($"[ZUI] current tab: {tab}");
			/*currentPage.children[0].uiItem.gameObject.DestroyGameObjectImmediate();
			currentPage.children.Clear();
			switch (currentTab) {
				case ZUITab.Configuration:
					currentPage.children.Add(configPage);
					break;
				case ZUITab.OtherSettings:
				default:
					currentPage.children.Add(settingsPage);
					break;
			}
			Stack<Transform> stack = new Stack<Transform>();
			stack.Push(currentPage.uiItem.gameObject.transform);
			currentPage.children[0].Create(ref stack, HighLogic.UISkin);*/
		}
	}
}
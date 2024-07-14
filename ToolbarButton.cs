using UnityEngine;
using KSP.UI.Screens;

namespace ZUI {
	[KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
	public class ToolbarButton : MonoBehaviour {
		private Texture configTexture;
		private ApplicationLauncherButton toolbarButton;

		private const string CONFIG_TEXTURE_PATH = "999_ZUI/Assets/zui_config";
		private const string TOOLBAR_BUTTON_ENABLED = "toolbarButtonEnabled";

		public void Awake() {
			configTexture = GameDatabase.Instance.GetTexture(CONFIG_TEXTURE_PATH, false);
			toolbarButton = ApplicationLauncher.Instance.AddModApplication(ConfigUI.ShowPopup, null, null, null, null, null, ApplicationLauncher.AppScenes.ALWAYS, configTexture);
		}
	}
}
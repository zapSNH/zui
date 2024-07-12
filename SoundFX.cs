using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

namespace ZUI
{
	[KSPAddon(KSPAddon.Startup.EveryScene, false)]
	public class SoundFX : MonoBehaviour
	{
		internal static SoundFX instance;
		private static AudioClip hoverAudio;

		private AudioSource audioSource;

		private PointerEventData pointerEventData;
		private GameObject currentHoverObject;

		public static string audioPath = "GameData/ZUI/PluginData/audio/";
		public static string hoverAudioPath = "hover.wav";

		public static float pitchRange = 0.5f;

		async public void Start()
		{
			instance = this;

			string hoverPath = KSPUtil.ApplicationRootPath + audioPath + hoverAudioPath;
			hoverAudio = await LoadClip(hoverPath);
			audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.ignoreListenerPause = true;
			pointerEventData = new PointerEventData(EventSystem.current);
		}

		// https://discussions.unity.com/t/load-audioclip-from-folder-on-computer-into-game-in-runtime/209967
		async Task<AudioClip> LoadClip(string path) {
			AudioClip clip = null;
			using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV)) {
				uwr.SendWebRequest();

				// wrap tasks in try/catch, otherwise it'll fail silently
				try {
					while (!uwr.isDone) await Task.Delay(5);

					if (uwr.isNetworkError || uwr.isHttpError) Debug.Log($"{uwr.error}");
					else {
						clip = DownloadHandlerAudioClip.GetContent(uwr);
					}
				} catch (Exception err) {
					Debug.Log($"{err.Message}, {err.StackTrace}");
				}
			}
			return clip;
		}
		public void Update()
		{
			pointerEventData.position = (Vector2)Input.mousePosition;
			List<RaycastResult> raycastResults = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pointerEventData, raycastResults);
			GameObject hoverObject = null;
			if (raycastResults.Count != 0 && raycastResults[0].gameObject.GetComponentsInChildren<Selectable>().Count() != 0) {
				hoverObject = raycastResults[0].gameObject;
			}
			if (currentHoverObject != hoverObject) {
				audioSource.pitch = 1 + ((Mathf.Clamp01(pointerEventData.position.y / Screen.height) - 0.5f) * pitchRange);
				audioSource.clip = hoverAudio;
				audioSource.Play();
			}
			currentHoverObject = hoverObject;
		}
	}
}
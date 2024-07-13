using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Smooth.Pools;

// Partially based on wheeeUI and Visual Studio's autocomplete feature
namespace ZUI
{
	[KSPAddon(KSPAddon.Startup.EveryScene, false)]
	public class Transformer : MonoBehaviour
	{
		internal static Transformer instance;
		internal UrlDir.UrlConfig[] transforms;
		internal List<GameObject> transformObjects = new List<GameObject>();
		internal List<bool> relativeTransform = new List<bool>();
		public List<Vector3> translateAmounts = new List<Vector3>();
		public List<Vector3> rotateAmounts = new List<Vector3>(); // rotation will always non-relative
		//public List<Vector3> scaleAmounts = new List<Vector3>(); // merely adding the code to handle scale without doing anything in the cfg files seems to break the rotation code

		private const string ZUITRANSFORM_CFG = "ZUITransform";
		private const string ZUISETTINGS_CFG = "ZUISettings";

		private const string TARGET_TRANSFORM_CFG = "target";
		private const string RELATIVE_TRANSFORM_CFG = "relative";
		private const string MULTI_OBJECT_TRANSFORM_CFG = "multi";

		private const string TRANSLATE_CFG = "translate";
		private const string ROTATE_CFG = "rotate";
		//private const string SCALE_CFG = "scale";

		private const string DEBUG_CFG = "debug_mode";

		private bool debugMode = false;

		public void Start()
		{
			if (instance == null) {
				instance = this;
			} else {
				Destroy(gameObject);
				return;
			}
			transforms = GameDatabase.Instance.GetConfigs(ZUITRANSFORM_CFG);

			foreach (UrlDir.UrlConfig config in transforms)
			{
				if (!config.config.HasValue(TARGET_TRANSFORM_CFG))
				{
					Debug.Log("[ZUI] Node does not have a transform target!");
					continue;
				}
				string target = config.config.GetValue(TARGET_TRANSFORM_CFG);
				bool isMulti = false;
				if (config.config.HasValue(MULTI_OBJECT_TRANSFORM_CFG)) {
					bool.TryParse(config.config.GetValue(MULTI_OBJECT_TRANSFORM_CFG), out isMulti);
				}
				List<GameObject> gameObjects = new List<GameObject>();
				if (isMulti) {
					gameObjects = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == target) as List<GameObject>;
				} else {
					gameObjects.Add(GameObject.Find(target));
				}

				// Some GameObjects return null even tho they exist. Calling the Start() function again fixes this.
				foreach (var gameObject in gameObjects) {
					if (gameObject != null) {
						Vector3 translate = Vector3.negativeInfinity;
						Vector3 rotate = Vector3.negativeInfinity;
						//Vector3 scale = Vector3.negativeInfinity;
						bool isRelative = false;

						if (config.config.HasValue(RELATIVE_TRANSFORM_CFG)) {
							bool.TryParse(config.config.GetValue(RELATIVE_TRANSFORM_CFG), out isRelative);
						}
						if (config.config.HasValue(TRANSLATE_CFG)) {
							config.config.TryGetValue(TRANSLATE_CFG, ref translate);
						}
						if (config.config.HasValue(ROTATE_CFG)) {
							config.config.TryGetValue(ROTATE_CFG, ref rotate);
						}
						//if (config.config.HasValue(SCALE_CFG)) {
						//	config.config.TryGetValue(SCALE_CFG, ref scale);
						//}

						transformObjects.Add(gameObject);
						rotateAmounts.Add(rotate);
						translateAmounts.Add(translate);
						relativeTransform.Add(isRelative);
						//scaleAmounts.Add(scale);
						Debug.Log($"[ZUI] target: {gameObject.name} | translate: {translate} | rotate: {rotate} | scale: ");
					} else {
						Debug.Log($"[ZUI] Invalid transform target! ({target})");
						continue;
					}
				}
			}

			// This could probably be merged with the loop above but just in
			// case I find a gameObject that can only be moved in Update() or
			// something then it would be very easy to move this loop there.
			int i = 0;
			foreach (GameObject gameObject in transformObjects)
			{
				if (gameObject != null)
				{
					if (relativeTransform[i]) {
						if (translateAmounts[i] != Vector3.negativeInfinity)
							gameObject.transform.localPosition += translateAmounts[i];
						//if (scaleAmounts[i] != Vector3.negativeInfinity)
						//	gameObject.transform.localScale = Vector3.Scale(scaleAmounts[i], gameObject.transform.localScale);
					} else {
						if (translateAmounts[i] != Vector3.negativeInfinity)
							gameObject.transform.localPosition = translateAmounts[i];
						//if (scaleAmounts[i] != Vector3.negativeInfinity)
						//	gameObject.transform.localScale = scaleAmounts[i];
					}
					gameObject.transform.localEulerAngles = rotateAmounts[i];
				}
				i++;
			}

			UrlDir.UrlConfig[] ZUISettings = GameDatabase.Instance.GetConfigs(ZUISETTINGS_CFG);
			foreach (UrlDir.UrlConfig config in ZUISettings)
			{
				config.config.TryGetValue(DEBUG_CFG, ref debugMode);
			}
		}
		public void Update()
		{
			if (debugMode)
			{
				// use debugstuff instead of this
				if (Input.GetKeyUp(KeyCode.E))
				{
					GameObject[] gameObjects = FindObjectsOfType<GameObject>();
					foreach (GameObject gameObject in gameObjects)
					{
						if (gameObject != null)
						{
							Debug.Log($"[ZUI] Name: " + gameObject.name + " | Translate: " + gameObject.transform.localPosition.ToString() + " | Rotate: " + gameObject.transform.localPosition.ToString());
						}
					}
				}
				//if (Input.GetKeyUp(KeyCode.Q))
				//{
				//	Start(); // Unfortunately the configs don't get reloaded so all this does is nudge the textures more
				//}
			}
		}
	}
}